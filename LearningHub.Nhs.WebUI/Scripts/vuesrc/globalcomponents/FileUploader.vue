<template>

    <div class="file-uploader-component">
        <div v-if="isUploading() || isPaused() || isSucceeded() || isInterrupted() || isFailed()">
            <div>
                <b>
                    <span v-if="isUploading()">Uploading:</span>
                    <span v-if="isPaused()">Paused:</span>
                    <span v-if="isSucceeded() && !isWholeSlideImageFile()">
                        Upload complete:
                        <Tick v-bind:complete="true"></Tick>
                        <br />
                    </span>
                    <span v-if="isSucceeded() && isWholeSlideImageFile()">
                        Processing image for slide viewer...
                        <Spinner></Spinner>
                        <br />
                    </span>
                    <span v-if="isInterrupted()"
                          class="file-uploader-component-interrupted">
                        <i class="fa-solid fa-triangle-exclamation"></i>
                        Upload interrupted:
                    </span>
                    <span v-if="isFailed()"
                          class=" file-uploader-component-failed">
                        <i class="fa-solid fa-triangle-exclamation"></i>
                        {{ file.isTooLargeForUpload() ? 'File too big:' : '' }}
                        {{ file.isWrongType(fileCategory) ? 'Wrong type of file:' : '' }}
                    </span>
                </b>
                <FileInfo v-bind:allowDownload="false" v-bind:file-model="file"/>
            </div>

            <div v-if="isUploading() || isPaused() || isInterrupted()"
                 class="file-uploader-component-progress-bar"
                 v-bind:class="{ 'file-uploader-component-progress-bar-paused': (isPaused() || isInterrupted()) }"
                 role="progressbar"
                 v-bind:aria-valuenow="fileUploadPercentage()"
                 aria-valuemin="0"
                 aria-valuemax="100">
                <div v-bind:style="{ width: fileUploadPercentage() + '%' }"></div>
            </div>

            <div v-if="isFailed()"
                 class="file-uploader-component-error-text my-3">
                <p v-if="file.isTooLargeForUpload()">
                    This file exceeds the {{ maxFileSizeFormatted }} maximum file size. Browse for a different file
                </p>
                <div v-if="file.excludedFromAllowedList(fileCategory)">
                    <p>
                        Upload a file in any of the following formats: {{ allowedFileExtensionsFormatted }}.
                    </p>
                    <div v-if="isWholeSlideImageFile() && file.fileName.toLowerCase().endsWith('.mrxs')">
                        <p>
                            It looks like you're trying to upload a Mirax whole-slide image. 
                            We accept Mirax whole-slide images in .zip format.
                            The .zip file should contain:
                        </p>
                        <ul>
                            <li>
                                The .mrxs file (e.g. 'slide_123.mrxs')
                            </li>
                            <li>
                                A folder of the same name (e.g. 'slide_123') containing the related .dat files
                            </li>
                        </ul>
                    </div>
                </div>
                <p v-if="file.includedInBlockedList()">
                    The following formats are not supported: {{ blockedFileExtensionsFormatted }}.
                </p>
            </div>

            <div v-if="isUploading() || isPaused() || isInterrupted()"
                 class="d-flex flex-wrap justify-content-between">
                <span class="no-wrap pr-10">
                    {{ fileUploadPercentage() }}% complete
                </span>
                <span class="no-wrap">
                    <a href="#"
                       v-if="isUploading()"
                       v-on:click.prevent="pauseFileUpload">
                        Pause upload
                    </a>
                    <a href="#"
                       v-if="isPaused()"
                       v-on:click.prevent="unPauseFileUpload">
                        Resume upload
                    </a>
                    <a href="#"
                       v-if="isInterrupted()"
                       v-on:click.prevent="clickToResumeFileUpload">
                        Resume upload
                    </a>
                    <template v-if="isInterrupted()">
                        <input type="file"
                               ref="fileToResume"
                               v-bind:accept="fileExtensionOfResumingFile()"
                               v-on:change="resumeFileUpload"
                               class="visually-hidden" />
                    </template>
                </span>
            </div>

            <div v-if="isInterrupted()"
                 class="mt-10">
                This upload was interrupted.
                To resume, browse for the file that you originally uploaded.
                The file needs to be the same name ({{ file.fileName }}) and size ({{ formattedFileSize() }}) as the original
            </div>

            <div v-if="isFailed()">
                <Button v-on:click="chooseAnotherFile">
                    Choose another file
                </Button>
                <input type="file"
                       ref="chooseAnotherFileInput"
                       v-bind:accept="allowedFileExtensionsInAcceptFormat"
                       v-on:change="uploadNewFile"
                       class="visually-hidden" />
            </div>
        </div>


        <template v-if="canUploadNewFile()">
            <label>
                <input type="file"
                       v-bind:accept="allowedFileExtensionsInAcceptFormat"
                       v-on:change="uploadNewFile"
                       class="visually-hidden" />
                {{ instructions }}
            </label>
        </template>

        <Modal v-if="wrongFileChosen" v-on:cancel="wrongFileChosen = false">
            <template v-slot:title>
                <WarningTriangle color="yellow"></WarningTriangle>
                Wrong file selected
            </template>
            <template v-slot:body>
                To continue uploading this file, you must select the same file:
                <br />
                <b>{{ file.fileName }}</b> ({{ formattedFileSize() }})
            </template>
            <template v-slot:buttons>
                <button class="nhsuk-button nhsuk-button--secondary mx-12 my-2" v-on:click="wrongFileChosen = false">Cancel</button>
                <button class="nhsuk-button mx-12 my-2" v-on:click="wrongFileChosen = false; clickToResumeFileUpload();">Try again</button>
            </template>
        </Modal>

    </div>
    
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import { FileModel } from '../models/contribute-resource/files/fileModel';
    import {
        FileUploadState,
        FileUploadType,
        getAllowedFileExtensionsInAcceptFormat,
        getFileExtensionAllowedListFormatted,
        getFileExtensionBlockListFormatted,
        getMaxFileSizeFormatted,
        resumeFileUpload,
        startFileUpload,
    } from '../helpers/fileUpload';
    import { FileSizeHelper } from '../helpers/fileSizeHelper';
    import { userData } from '../data/user';

    import Tick from './Tick.vue';
    import Modal from './Modal.vue';
    import Button from './Button.vue';
    import FileInfo from "../contribute-resource/components/content-tab/FileInfo.vue";
    import Spinner from './Spinner.vue';
    import WarningTriangle from './WarningTriangle.vue';

    export default Vue.extend({
        components: {
            Tick,
            Modal,
            Button,
            FileInfo,
            Spinner,
            WarningTriangle,
        },
        props: {
            file: { type: Object } as PropOptions<FileModel>,
            fileCategory: { type: Number } as PropOptions<FileUploadType>,
            instructions: { type: String, default: "Choose a file" } as PropOptions<String>
        },
        data() {
            return {
                wrongFileChosen: false,
                wrongFileSelectedFileName: '',
                allowedFileExtensionsFormatted: getFileExtensionAllowedListFormatted(this.fileCategory),
                allowedFileExtensionsInAcceptFormat: getAllowedFileExtensionsInAcceptFormat(this.fileCategory),
                blockedFileExtensionsFormatted: getFileExtensionBlockListFormatted(),
                maxFileSizeFormatted: getMaxFileSizeFormatted(),
                uploadedFileSize: this.file?.partialFile?.uploadedFileSize,
                totalFileSize: this.file?.partialFile?.totalFileSize,
                keepUserSessionAliveIntervalSeconds: undefined,
                keepUserSessionAliveTimer: '' as any,
            };
        },
         async created() {
            await userData.getkeepUserSessionAliveInterval().then(response => {
                this.keepUserSessionAliveIntervalSeconds = response;
                if (this.keepUserSessionAliveIntervalSeconds) {
                  this.keepUserSessionAliveTimer = setInterval(() => { userData.keepUserSessionAlive() }, this.keepUserSessionAliveIntervalSeconds);
                }
            });
        },
        methods: {
            canUploadNewFile(): boolean {
                return !this.file;
            },
            isInterrupted(): boolean {
                return this.file &&
                    !this.file.fileUpload &&
                    this.file.partialFile !== undefined &&
                    !this.file.isNotAllowedToUpload(this.fileCategory);
            },
            isUploading(): boolean {
                return this.file &&
                    this.file.fileUpload &&
                    this.file.fileUpload.state === FileUploadState.Uploading;
            },
            isPaused(): boolean {
                return this.file &&
                    this.file.fileUpload &&
                    this.file.fileUpload.state === FileUploadState.Paused;
            },
            isSucceeded(): boolean {
                var isFileUploadSuccess = this.file &&
                    this.file.fileUpload &&
                    this.file.fileUpload.state === FileUploadState.Succeeded;
                if(isFileUploadSuccess){                 
                   clearInterval(this.keepUserSessionAliveTimer);
                }
                return isFileUploadSuccess;
            },
            isFailed(): boolean {
                return this.file &&
                    ((this.file.fileUpload &&
                            this.file.fileUpload.state === FileUploadState.Failed) ||
                        this.file.isNotAllowedToUpload(this.fileCategory));
            },
            isWholeSlideImageFile(): boolean {
                return this.file &&
                    this.file.wholeSlideImageFile !== undefined;
            },
            formattedFileSize(): string {
                return FileSizeHelper.getFormattedFileSize(this.file.partialFile.totalFileSize);
            },
            fileUploadPercentage(): string {
                if (!!this.file && !!this.file.partialFile && !!this.file.partialFile.uploadedFileSize && !!this.file.partialFile.totalFileSize) {
                    let percent = this.file.partialFile.uploadedFileSize / this.file.partialFile.totalFileSize * 100;
                    if (percent > 99) {
                        // There is a delay after reaching 100% before the file is completely uploaded.
                        // At this point, we want to make it clear to the user that the file hasn't finished uploading
                        // So, we just restrict the upload percentage to 99%
                        // Once the file has completed, we will show a different screen, so this percentage will disappear
                        percent = 99;
                    }
                    return percent.toFixed(1);
                }
                else {
                    return '0';
                }
            },
            fileExtensionOfResumingFile(): string {
                const fileName = this.file.fileName;
                return fileName.substring(fileName.lastIndexOf('.'));
            },
            async uploadNewFile(event: any): Promise<void> {
                if (event.target.value !== '') {
                    const file = event.target.files[0] as File;
                    const fileId = await startFileUpload(file, this.fileCategory, this.updateUploadingProgress);
                    this.$emit('newFileId', fileId);

                    // Empty the list of files
                    // The "onchange" event only fires if the list of files "changes"
                    // So, it doesn't fire if you select the same file twice
                    // To allow this, we empty the list of files after each one if chosen
                    event.target.value = ''; // Empties the list of files - the only value you can set a <input type="file">.value to is '' (the empty string)
                }
            },
            pauseFileUpload(): void {
                this.file.fileUpload.pause();
            },
            unPauseFileUpload(): void {
                this.file.fileUpload.resume();
            },
            clickToResumeFileUpload(): void {
                let inputElement = this.$refs.fileToResume as any;
                inputElement.click();
            },
            resumeFileUpload(event: any): void {
                if (event.target.value !== '') {
                    const file = event.target.files[0] as File;

                    if (file.name === this.file.fileName) {
                        resumeFileUpload(file, this.file, this.updateUploadingProgress);
                    }
                    else {
                        this.wrongFileSelectedFileName = file.name;
                        this.wrongFileChosen = true;
                    }

                    // Empty the list of files
                    // The "onchange" event only fires if the list of files "changes"
                    // So, it doesn't fire if you select the same file twice
                    // To allow this, we empty the list of files after each one if chosen
                    event.target.value = ''; // Empties the list of files - the only value you can set a <input type="file">.value to is '' (the empty string)
                }
            },
            chooseAnotherFile() {
                let inputElement = this.$refs.chooseAnotherFileInput as any;
                inputElement.click();
            },
            updateUploadingProgress() {
                this.$forceUpdate();
                if (this.isSucceeded()) {
                    this.$emit('uploadSuccess');
                }
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .file-uploader-component {
    }

    .file-uploader-component .file-uploader-component-interrupted > i {
        color: $nhsuk-warm-yellow;
        font-size: 20px;
        margin-right: 4px;
    }

    .file-uploader-component .file-uploader-component-failed > i {
        color: $nhsuk-red;
        font-size: 20px;
        margin-right: 4px;
    }

    .file-uploader-component .file-uploader-component-progress-bar {
        height: 40px;
        margin: 8px 0;
        background-color: $nhsuk_white;
        border: 1px solid $nhsuk-grey-placeholder;
        border-radius: 6px;
        overflow: hidden;
    }

    .file-uploader-component .file-uploader-component-progress-bar > div {
        height: 40px;
        background-color: $nhsuk-green;
    }

    .file-uploader-component .file-uploader-component-progress-bar.file-uploader-component-progress-bar-paused > div {
        background-color: $nhsuk-warm-yellow;
    }

    .file-uploader-component-error-text {
        color: $nhsuk-red;
    }
</style>