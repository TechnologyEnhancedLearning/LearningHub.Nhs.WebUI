<template>
    <div class="contribute-whole-slide-image">
        <div class="contribute-whole-slide-image-header d-flex justify-content-between">
            <div class="d-flex flex-grow-1">
                <Tick :complete="wholeSlideImageItem.isReadyToPublish()"
                      class="pt-20 pl-15 pr-10"></Tick>
                <div class="flex-grow-1 mr-10">                 
                    <EditSaveFieldWithCharacterCount v-model="wholeSlideImage.title"
                                                     addEditLabel="title"
                                                     :characterLimit="60"
                                                     :isH4="true"
                                                     :inputId="title"></EditSaveFieldWithCharacterCount>
                </div>
            </div>
            <div class="contribute-whole-slide-image-buttons d-flex align-items-start justify-content-end">
                <IconButton v-if="enableUp"
                            @click="$emit('up')"
                            iconClasses="fa-solid fa-chevron-up"
                            ariaLabel="Move slide up"
                            class="contribute-whole-slide-image-button"></IconButton>
                <IconButton v-if="enableDown"
                            @click="$emit('down')"
                            iconClasses="fa-solid fa-chevron-down"
                            ariaLabel="Move slide down"
                            class="contribute-whole-slide-image-button"></IconButton>
                <IconButton v-if="wholeSlideImage.file.fileId || !imageZone"
                            @click="discardSlideModalOpen = true"
                            iconClasses="fa-solid fa-trash-can-alt"
                            ariaLabel="Delete slide"
                            class="contribute-whole-slide-image-button"></IconButton>
            </div>
        </div>

        <div class="d-flex align-items-center">
            <picture v-if="imageFinishedUploading"
                     class="whole-slide-image-thumbnail">
                <img :src="`/api/Resource/SlideImageTile/${wholeSlideImage.getFileModel().filePath}/0/8/0_0.jpg`"/>
            </picture>
            <div v-else
                 class="whole-slide-image-thumbnail">
                <i class="fa-solid fa-microscope"></i>
            </div>
            <input type="file"
                   ref="chooseFileInput"
                   :accept="allowedFileExtensionsInAcceptFormat"
                   @change="uploadNewFile"
                   class="visually-hidden"/>

            <div class="whole-slide-image-details pr-25">
                <FileUploader v-if="showFileUploader && wholeSlideImage.getFileModel()"
                              :file="wholeSlideImage.getFileModel()"
                              :fileCategory="FileUploadPostProcessingOptions.WholeSlideImage"
                              @newFileId="newFileId"></FileUploader>

                <div v-if="!wholeSlideImage.getFileModel() && !imageZone">
                    <h3 class="flex pl-2 pt-10"
                        v-if="showPlaceholderText"><label for="placeholder">Placeholder text:</label></h3>
                    <div class="d-flex align-items-center">
                        <EditSaveFieldWithCharacterCount
                            class="flex"
                            addEditLabel="placeholder text"
                            v-model="wholeSlideImageItem.placeholderText"
                            :characterLimit="500"
                            :inputId="placeholder"
                            @updateIsEditing="value => isEditingPlaceholder(value)"/>
                    </div>
                    <div>
                        <div class="pl-2">or</div>
                        <Button color="blue"
                                @click="chooseFile">Upload an Image
                        </Button>
                    </div>
                </div>
                <div v-else-if="!wholeSlideImage.getFileModel()">
                    <Button style="min-width:115px"
                            @click="chooseFile">Upload a Whole Slide Image
                    </Button>
                </div>
                <div v-else>
                    <div>
                        <div class="whole-slide-image-file-status">
                            <span v-if="isFailed"
                                  class="whole-slide-image-status-failed">
                                <i class="fa-solid fa-triangle-exclamation"></i>
                                <strong>
                                    Processing error:
                                </strong>
                            </span>
                            <span v-else-if="isProcessingFile">
                                <strong>
                                    Processing image for slide viewer...
                                </strong>
                                <Spinner></Spinner>
                                <br/>
                            </span>
                            <div v-else-if="imageFinishedUploading">
                                <div v-if="this.wholeSlideImage.annotations.length !== 1">
                                    There are <strong>{{ this.wholeSlideImage.annotations.length }}</strong>
                                    {{ answerOptionOrAnnotation }}s on this slide
                                </div>
                                <div v-else>
                                    There is <strong>{{ this.wholeSlideImage.annotations.length }}</strong>
                                    {{ answerOptionOrAnnotation }} on this slide
                                </div>

                                <div class="whole-slide-image-status-add-annotation-label">
                                    <Button size="medium" class="nhsuk-u-font-size-16"
                                            @click="showSlideWithAnnotations">
                                        <div v-if="this.wholeSlideImage.annotations.length === 0">
                                            + Add {{ answerOptionOrAnnotation }}s
                                        </div>
                                        <div v-else-if="!imageZone || this.wholeSlideImage.annotations.length < 10">
                                            + Add or edit {{ answerOptionOrAnnotation }}s
                                        </div>
                                        <div v-else>
                                            View answer options
                                        </div>
                                    </Button>
                                </div>
                            </div>
                        </div>
                        <div v-if="isFailed">
                            <div class="file-uploader-component-error-text my-3">
                                Something went wrong while processing this image.
                            </div>
                            <div>
                                <Button @click="chooseFile">
                                    Try again
                                </Button>
                            </div>
                            <div class="my-3">
                                If you need help
                                <a :href="supportFormUrl"
                                   target="_blank"
                                   class="accessible-link">contact support</a>.
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-4 prompt-text pt-10">
                <div v-if="noPlaceholderText && !imageZone">
                    <h3>Tip</h3>
                    You can upload a whole-slide image now or add some placeholder text. Be aware, this text will be
                    shown to readers when the {{ resourceName }} is published.
                </div>
                <div v-else-if="showPlaceholderText">
                    <h3>Be Aware</h3>
                    This placeholder text will be visible to readers once this {{ resourceName }} is published.
                </div>
            </div>
        </div>
        <div class="whole-slide-image-title">
            <span v-if="!showFileUploader && !isFailed && !isProcessingFile">
                {{ wholeSlideImage.getFileModel().fileName }}
            </span>
        </div>

        <Modal v-if="discardSlideModalOpen"
               @cancel="discardSlideModalOpen = false">
            <template v-slot:title>
                <WarningTriangle color="yellow"></WarningTriangle>
                Delete slide
            </template>
            <template v-slot:body>
                This cannot be undone.
                Do you want to continue?
            </template>
            <template v-slot:buttons>
                <Button @click="discardSlideModalOpen = false"
                        class="mx-12 my-2">
                    Cancel
                </Button>
                <Button color="red"
                        @click="$emit('delete'); discardSlideModalOpen = false;"
                        class="mx-12 my-2">
                    Delete slide
                </Button>
            </template>
        </Modal>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { WholeSlideImageModel } from '../models/contribute-resource/blocks/wholeSlideImageModel';
    import { FileStore } from '../models/contribute-resource/files/fileStore';
    import { WholeSlideImageFileStatusEnum } from '../models/contribute-resource/files/wholeSlideImageFileStatusEnum';
    import {
        FileUploadPostProcessingOptions,
        FileUploadType,
        getAllowedFileExtensionsInAcceptFormat,
        startFileUpload
    } from '../helpers/fileUpload';
    import Button from '../globalcomponents/Button.vue';
    import EditSaveFieldWithCharacterCount from '../globalcomponents/EditSaveFieldWithCharacterCount.vue';
    import FileInfo from "./components/content-tab/FileInfo.vue";
    import FileUploader from '../globalcomponents/FileUploader.vue';
    import IconButton from '../globalcomponents/IconButton.vue';
    import LinkTextAndIcon from '../globalcomponents/LinkTextAndIcon.vue';
    import Modal from '../globalcomponents/Modal.vue';
    import Spinner from '../globalcomponents/Spinner.vue';
    import Tick from '../globalcomponents/Tick.vue';
    import WarningTriangle from '../globalcomponents/WarningTriangle.vue';
    import SlideViewerWithAnnotations from "../resource/blocks/SlideViewerWithAnnotations.vue";
    import { WholeSlideImageBlockItemModel } from '../models/contribute-resource/blocks/wholeSlideImageBlockItemModel';
    import { ResourceType } from "../constants";
    import { getDisplayNameForResourceType } from "../resource/helpers/resourceHelper";
    import SupportUrls from '../data/supportUrls';

    export default Vue.extend({
        components: {
            SlideViewerWithAnnotations,
            Button,
            EditSaveFieldWithCharacterCount,
            FileInfo,
            FileUploader,
            IconButton,
            LinkTextAndIcon,
            Modal,
            Spinner,
            Tick,
            WarningTriangle,
        },
        props: {
            wholeSlideImageItem: { type: Object } as PropOptions<WholeSlideImageBlockItemModel>,
            enableUp: Boolean,
            enableDown: Boolean,
            resourceType: { type: Number } as PropOptions<ResourceType>,
            imageZone: { type: Boolean, default: false },
            title: { type: String, default: 'title' },
            placeholder: { type: String, default: 'placeholder' },
        },
        data() {
            return {
                WholeSlideImageFileStatusEnum: WholeSlideImageFileStatusEnum,
                discardSlideModalOpen: false,
                FileStore: FileStore /* It looks like FileStore isn't used, but we need it to be exposed here to allow Vue to make the files list reactive */,
                editingPlaceholder: false,
                FileUploadPostProcessingOptions: FileUploadPostProcessingOptions,
                allowedFileExtensionsInAcceptFormat: getAllowedFileExtensionsInAcceptFormat(FileUploadType.WholeSlideImage),
                answerOptionOrAnnotation: this.imageZone ? "answer option" : "annotation"
            };
        },
        computed: {
            wholeSlideImage(): WholeSlideImageModel {
                return this.wholeSlideImageItem.wholeSlideImage;
            },
            noPlaceholderText(): boolean {
                return (typeof this.wholeSlideImageItem.placeholderText !== "string" || this.wholeSlideImageItem.placeholderText.length === 0) && !this.wholeSlideImage.getFileModel();
            },
            showPlaceholderText(): boolean {
                return (this.editingPlaceholder || (typeof this.wholeSlideImageItem.placeholderText === "string" && this.wholeSlideImageItem.placeholderText.length > 0)) && !this.wholeSlideImage.getFileModel();
            },
            showFileUploader(): boolean {
                const file = this.wholeSlideImage.getFileModel();
                return !file ||
                    !file.wholeSlideImageFile ||
                    file.wholeSlideImageFile.status === WholeSlideImageFileStatusEnum.Uploading;
            },
            isProcessingFile(): boolean {
                return this.isWholeSlideImageWithStatus(WholeSlideImageFileStatusEnum.QueuedForProcessing);
            },
            isFailed(): boolean {
                return this.isWholeSlideImageWithStatus(WholeSlideImageFileStatusEnum.ProcessingFailed);
            },
            imageFinishedUploading(): boolean {
                this.wholeSlideImage.updatePublishingStatus();
                return this.isWholeSlideImageWithStatus(WholeSlideImageFileStatusEnum.ProcessingComplete);
            },
            resourceName(): string {
                return getDisplayNameForResourceType(this.resourceType);
            },
            supportFormUrl(): string {
                return SupportUrls.supportForm;
            }
        },
        methods: {
            newFileId(fileId: number) {
                // A fileId is available from the FileUploader
                // Set the fileId on this WholeSlideImage
                this.wholeSlideImage.setFileId(fileId);
            },
            chooseFile() {
                let inputElement = this.$refs.chooseFileInput as any;
                inputElement.click();
            },
            async uploadNewFile(event: any): Promise<void> {
                if (event.target.value !== '') {
                    const file = event.target.files[0] as File;
                    const fileId = await startFileUpload(file, FileUploadType.WholeSlideImage);
                    this.newFileId(fileId);

                    // Empty the list of files
                    // The "onchange" event only fires if the list of files "changes"
                    // So, it doesn't fire if you select the same file twice
                    // To allow this, we empty the list of files after each one if chosen
                    event.target.value = ''; // Empties the list of files - the only value you can set a <input type="file">.value to is '' (the empty string)
                }
            },
            showSlideWithAnnotations() {
                this.$emit('annotateWholeSlideImage', this.wholeSlideImage);
            },
            isEditingPlaceholder(value: boolean) {
                this.editingPlaceholder = value;
            },
            isWholeSlideImageWithStatus(status: WholeSlideImageFileStatusEnum): boolean {
                if (!this.wholeSlideImage) {
                    return false;
                }
                const file = this.wholeSlideImage.getFileModel();
                return file && file.wholeSlideImageFile && file.wholeSlideImageFile.status == status;
            }

        },

    });
</script>

<style lang="scss"
       scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-whole-slide-image {
        margin: 20px 0;
        border: 1px solid $nhsuk-grey-light;
        background-color: $nhsuk-grey-white;
    }

    .contribute-whole-slide-image .contribute-whole-slide-image-header {
        border-bottom: 1px solid $nhsuk-grey-light;
        min-height: 60px;
    }

    .contribute-whole-slide-image .contribute-whole-slide-image-buttons {
        padding-right: 5px;
    }

    .contribute-whole-slide-image .contribute-whole-slide-image-button {
        margin: 8px 2px;
    }

    .contribute-whole-slide-image .whole-slide-image-thumbnail {
        flex-shrink: 0;
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100px;
        height: 100px;
        margin: 25px 25px 0 25px;
        border: 1px solid $nhsuk-grey-placeholder;
        border-radius: 50%;
        background-color: $nhsuk-grey-lighter;
        overflow: hidden;
    }

    .contribute-whole-slide-image .whole-slide-image-thumbnail img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        object-position: center center;
    }

    .contribute-whole-slide-image .whole-slide-image-thumbnail i {
        font-size: 40px;
        color: $nhsuk-grey-placeholder;
        margin-right: 4px;
    }

    .contribute-whole-slide-image .whole-slide-image-details {
        flex: 1 1 auto;
        font-size: 16px;
        -ms-overflow-x: hidden;

        .whole-slide-image-file-status {
            align-self: center;
            padding-top: 25px;

            .whole-slide-image-status-add-annotation-label {
                font-size: 19px;
                padding-top: 10px;
                white-space: nowrap;
            }
        }
    }

    .whole-slide-image-title {
        min-height: 25px;
        padding: 10px 14px 14px 0;
        font-size: 16px;
        text-align: right;
        color: $nhsuk-grey-placeholder;
    }

    .contribute-whole-slide-image .whole-slide-image-status-failed i {
        font-size: 20px;
        color: $nhsuk-red;
        margin-right: 4px;
    }

    .file-uploader-component-error-text {
        color: $nhsuk-red;
    }

    .prompt-text {
        color: $nhsuk-grey;
    }
</style>