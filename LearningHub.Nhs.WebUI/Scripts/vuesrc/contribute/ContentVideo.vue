<template>
    <div>
        <div class="row">
            <div class="form-group col-12">
                <h3>Uploaded file</h3>
            </div>
        </div>
        <div class="row">
            <file-panel :file-id="localVideoDetail.file.fileId" :file-description="localVideoDetail.file.fileName" :file-size="localVideoDetail.file.fileSizeKb" @changefile="changeFile"></file-panel>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12">
                <h3>Transcript <span class="optional">(optional)</span></h3>
                <p>Please upload a transcript file to support learners that need to use an alternative format of this resource. This must be either a Word (.doc or .docx), PDF (.pdf) or Text (.txt) file.</p>
            </div>
        </div>
        <div v-if="localVideoDetail.transcriptFile" class="row">
            <file-panel :file-id="localVideoDetail.transcriptFile.fileId" :file-description="localVideoDetail.transcriptFile.fileName" :file-size="localVideoDetail.transcriptFile.fileSizeKb" @changefile="changeTranscriptFile" :showDelete="true" @deletefile="deleteTranscriptFile"></file-panel>
        </div>
        <div class="row" v-show="!localVideoDetail.transcriptFile">
            <div class="form-group col-12">
                <div class="p-4 uploadInnerBox">
                    <div class="upload-btn-wrapper nhsuk-u-font-size-16">
                        <label for="transcriptFileUpload" class="nhsuk-button nhsuk-button--secondary">Choose file</label> No file chosen
                        <input type="file" id="transcriptFileUpload" aria-label="Choose transcript file" ref="transcriptFileUpload" @change="onTranscriptFileChange" hidden />
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12">
                <h3>Closed captions <span class="optional">(optional)</span></h3>
                <p>Please upload a closed captions file to support learners that need the audio displayed as text on this video. This must be a file that has a .VTT file extension.</p>
            </div>
        </div>
        <div v-if="localVideoDetail.closedCaptionsFile" class="row">
            <file-panel :file-id="localVideoDetail.closedCaptionsFile.fileId" :file-description="localVideoDetail.closedCaptionsFile.fileName" :file-size="localVideoDetail.closedCaptionsFile.fileSizeKb" @changefile="changeCaptionsFile" :showDelete="true" @deletefile="deleteCaptionsFile"></file-panel>
        </div>
        <div class="row" v-show="!localVideoDetail.closedCaptionsFile">
            <div class="form-group col-12">
                <div class="p-4 uploadInnerBox">
                    <div class="upload-btn-wrapper nhsuk-u-font-size-16">
                        <label for="closedCaptionsFileUpload" class="nhsuk-button nhsuk-button--secondary">Choose file</label> No file chosen
                        <input type="file" id="closedCaptionsFileUpload" aria-label="Choose closed captions file" ref="closedCaptionsFileUpload" @change="onClosedCaptionsFileChange" hidden />
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12">
                <h3 id="additionalinfo-label">Additional information <span class="optional">(optional)</span></h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                Add any further information that is relevant to this resource or will help learners to use it,
                for example, how it was developed or what is required for it to be used.
            </div>
            <div class="col-12 mt-3">
                <textarea class="form-control" aria-labelledby="additionalinfo-label" rows="4" maxlength="250" v-model="additionalInformation" @change="setAdditionalInformation($event.target.value)"></textarea>
            </div>
            <div class="col-12 footer-text">
                You can enter a maximum of 250 characters
            </div>
        </div>

        <file-upload :resource-version-id="resourceVersionId"
                     ref="attachedFileUploader"
                     @fileuploadcomplete="fileUploadComplete"
                     @fileuploadcancelled="fileUploadCancelled"
                     @childfileuploaderror="childFileUploadError">
        </file-upload>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import * as _ from "lodash";
    import { resourceData } from '../data/resource';
    import { VideoResourceModel, ResourceFileModel } from '../models/contribute/contributeResourceModel';
    import { FileUploadResult } from "../models/contribute/fileUploadResult";
    import { FileErrorTypeEnum } from './fileErrors';
    import { file_extension_validation, file_no_extension, file_size_validation, transcriptfile_extension_validation, closedcaptionsfile_extension_validation } from './fileValidation';
    import { EventBus } from './contributeEvents';
    import FilePanel from './FilePanel.vue';
    import { ResourceType } from '../constants';
    import FileUpload from './fileUpload.vue';

    export default Vue.extend({
        components: {
            FilePanel,
            FileUpload
        },
        data() {
            return {
                localVideoDetail: { resourceVersionId: 0 } as VideoResourceModel,
                additionalInformation: '' as string,
                uploadingFile: null as File,
                uploadingTranscriptFile: null as File,
                uploadingCaptionsFile: null as File,
            };
        },
        computed: {
            resourceVersionId(): number {
                return this.$store.state.resourceDetail.resourceVersionId;
            },
            videoDetail(): VideoResourceModel {
                return this.$store.state.videoDetail;
            },
            videoResourceVersionId(): number {
                return this.$store.state.videoDetail.resourceVersionId;
            },
            fileUpdated(): ResourceFileModel {
                return this.$store.state.fileUpdated;
            }
        },
        created() {
            this.setInitialValues();
            EventBus.$on('deleteFile', (fileTypeToBeDeleted: number) => {
                this.processDeleteFile(fileTypeToBeDeleted);
            });
        },
        beforeDestroy() {
            EventBus.$off('deleteFile', (fileTypeToBeDeleted: number) => {
                this.processDeleteFile(fileTypeToBeDeleted);
            });
        },
        methods: {
            changeFile() {
                this.$emit('filechanged');
            },
            changeTranscriptFile() {
                $('#transcriptFileUpload').val(null);
                $('#transcriptFileUpload').click();
            },
            deleteTranscriptFile() {
                this.$emit('confirmdeletefile', 1);
            },
            onTranscriptFileChange() {
                this.uploadingTranscriptFile = (this.$refs.transcriptFileUpload as any).files[0];
                if (!this.$v.uploadingTranscriptFile.$invalid) {
                    this.uploadingFile = this.uploadingTranscriptFile;
                    this.onFileChange(1);
                } else {
                    $('#transcriptFileUpload').val(null);
                    this.$emit('childfileuploaderror', FileErrorTypeEnum.InvalidTranscriptType, '');
                }
            },
            changeCaptionsFile() {
                $('#closedCaptionsFileUpload').val(null);
                $('#closedCaptionsFileUpload').click();
            },
            deleteCaptionsFile() {
                this.$emit('confirmdeletefile', 2);
                console.log('deleteCaptionsFile');
            },
            onClosedCaptionsFileChange() {
                this.uploadingCaptionsFile = (this.$refs.closedCaptionsFileUpload as any).files[0];
                if (!this.$v.uploadingCaptionsFile.$invalid) {
                    this.uploadingFile = this.uploadingCaptionsFile;
                    this.onFileChange(2);
                } else {
                    $('#closedCaptionsFileUpload').val(null);
                    this.$emit('childfileuploaderror', FileErrorTypeEnum.InvalidCaptionsType, '');
                }
            },
            onFileChange(fileType: number) {
                if (!this.$v.uploadingFile.$invalid) {
                    (this.$refs.attachedFileUploader as any).uploadResourceAttachedFile(this.uploadingFile, fileType, ResourceType.VIDEO);
                } else {
                    $('#transcriptFileUpload').val(null);
                    $('#closedCaptionsFileUpload').val(null);
                    if (!this.$v.uploadingFile.file_size_validation) {
                        this.$emit('childfileuploaderror', FileErrorTypeEnum.TooLarge, '');
                    } else if (!this.$v.uploadingFile.file_extension_validation) {
                        this.$emit('childfileuploaderror', FileErrorTypeEnum.InvalidType, '');
                    } else if (!this.$v.uploadingFile.file_no_extension) {
                        this.$emit('childfileuploaderror', FileErrorTypeEnum.NoExtension, '');
                    }
                }
            },
            fileUploadComplete(uploadResult: FileUploadResult) {
                if (!uploadResult.invalid) {
                    this.$store.commit('setSaveStatus', 'Saved');
                    let file: ResourceFileModel = new ResourceFileModel();
                    file.resourceVersionId = this.resourceVersionId;
                    file.fileId = uploadResult.fileId;
                    file.fileTypeId = uploadResult.fileTypeId;
                    file.fileName = uploadResult.fileName;
                    file.fileSizeKb = uploadResult.fileSizeKb;
                    if (uploadResult.attachedFileTypeId == 1) {
                        this.localVideoDetail.transcriptFile = file;
                        this.$store.commit('setVideoTranscriptFile', file);
                    } else {
                        this.localVideoDetail.closedCaptionsFile = file;
                        this.$store.commit('setVideoclosedCaptionsFile', file);
                    }
                } else {
                    this.$emit('childfileuploaderror', FileErrorTypeEnum.Custom, 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.');
                    this.$store.commit('setSaveStatus', '');
                }
                $('#transcriptFileUpload').val(null);
                $('#closedCaptionsFileUpload').val(null);
            },
            fileUploadCancelled() {
                console.log('File upload cancelled');
            },
            childFileUploadError(errorType: FileErrorTypeEnum, customError: string) {
                this.$emit('childfileuploaderror', errorType, customError);
            },
            async processDeleteFile(fileTypeId: number) {
                //console.log('processDeleteFile ' + fileTypeId);
                let response = await resourceData.deleteResourceAttributeFile(ResourceType.VIDEO, this.resourceVersionId, fileTypeId);
                if (response) {
                    this.$store.commit('removeVideoAttributeFile', fileTypeId);
                    switch (fileTypeId) {
                        case 1:
                            this.localVideoDetail.transcriptFile = null;
                            break;
                        case 2:
                            this.localVideoDetail.closedCaptionsFile = null;
                            break;
                    }
                }
            },
            setInitialValues() {
                this.localVideoDetail = _.cloneDeep(this.videoDetail);
                this.additionalInformation = this.$store.state.resourceDetail.additionalInformation;
            },
            setProperty(field: string, value: string) {
                // "this.imageDetail[field as keyof ImageResourceModel]" equivalent to "this.imageDetail[field]"
                // TypeScript syntax is needed because noImplicitAny is set to true in the tsconfig.json file
                let storedValue: string = '';
                if (this.videoDetail[field as keyof VideoResourceModel] != null) {
                    storedValue = this.videoDetail[field as keyof VideoResourceModel].toString();
                }
                if (storedValue != value) {
                    this.$store.commit("saveVideoDetail", { field, value });
                }
            },
            setAdditionalInformation(value: string) {
                let field: string = 'additionalInformation';
                if (this.$store.state.resourceDetail.title != value) {
                    this.$store.commit("saveResourceDetail", { field, value });
                }
            }
        },
        watch: {
            videoResourceVersionId(value) {
                this.setInitialValues();
            },
            fileUpdated(value) {
                this.localVideoDetail.file = this.videoDetail.file;
            }
        },
        validations: {
            uploadingFile: {
                file_size_validation,
                file_extension_validation,
                file_no_extension
            },
            uploadingTranscriptFile: {
                transcriptfile_extension_validation
            },
            uploadingCaptionsFile: {
                closedcaptionsfile_extension_validation
            },
        }
    })

</script>
