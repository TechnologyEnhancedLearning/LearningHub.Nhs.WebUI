<template>
    <div class="media-block-image">
        <FileUploader v-if="showFileUploader"
                      instructions="Choose an image"
                      v-bind:file="imageFileModel"
                      v-bind:fileCategory="restrictAdvancedImageTypes ? FileUploadType.RestrictedImage : FileUploadType.Image"
                      v-on:uploadSuccess="updateImageFileModel"
                      v-on:newFileId="newFileId"/>
        <div v-else class="d-flex flex-wrap justify-content-center">
            <div class="image-viewer p-24" v-if="displayImage">
                <picture>
                    <img v-bind:class="imageClass" v-bind:src="imageUrl" v-bind:alt="imageAltText">
                </picture>
                <div v-if="withLabel">
                    <EditSaveFieldWithCharacterCount v-model="image.description"
                                                     addEditLabel="label"
                                                     v-bind:characterLimit="descriptionCharacterLimit"/>
                </div>
            </div>
            <div v-else class="d-flex flex-column align-items-center justify-content-center">
                <MediaBlockImageAttachment v-bind:file="imageFileModel" />
                <div class="download-descriptor">
                    <p>
                        Only .jpeg, .png and .gif files can be viewed in a browser.
                        Other image types will be available to the learner as a download.
                    </p>
                </div>
            </div>
            <div class="alt-text-editor">
                <b>Alt text</b>
                <Tick :complete="!!image.altText"></Tick>
                <p>
                    Provide a short description of the image.
                    This will not be shown on screen but it will help those using screen readers.
                </p>
                <EditSaveFieldWithCharacterCount v-model="image.altText"
                                                 addEditLabel="alt text"
                                                 v-bind:characterLimit="125"
                                                 block-view/>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue, {PropOptions} from 'vue';

import EditSaveFieldWithCharacterCount from "../../../globalcomponents/EditSaveFieldWithCharacterCount.vue";

import { FileModel } from "../../../models/contribute-resource/files/fileModel";
import { FileUploadType } from "../../../helpers/fileUpload";
import { ImageModel } from "../../../models/contribute-resource/blocks/imageModel";
import FileUploader from "../../../globalcomponents/FileUploader.vue";
import FileInfo from "./FileInfo.vue";
import IconButton from "../../../globalcomponents/IconButton.vue";
import { isImageFileViewable } from "../../../helpers/attachmentTypeHelper";
import MediaBlockImageAttachment from "./MediaBlockImageAttachment.vue";
import Tick from "../../../globalcomponents/Tick.vue";

export default Vue.extend({
        components: {
            Tick,
            MediaBlockImageAttachment,
            IconButton,
            FileInfo,
            EditSaveFieldWithCharacterCount,
            FileUploader,
        },
        props: {
            image: { type: Object } as PropOptions<ImageModel>,
            restrictAdvancedImageTypes: { type: Boolean, default: false },
            imageClass: String,
            withLabel: { type: Boolean, default: false },
            descriptionCharacterLimit: { type: Number, default: 125 }
        },
        data() {
            return {
                FileUploadType: FileUploadType,
                imageFileModel: this.image.getFileModel()
            }
        },
        watch: {
            image: {
                deep: true,
                handler() {
                    this.imageFileModel = this.image.getFileModel();
                }
            }  
        },
        computed: {
            imageUrl(): string {
                return `/api/resource/DownloadResource`
                + `?filePath=${encodeURIComponent(this.imageFileModel.filePath)}`
                + `&fileName=${encodeURIComponent(this.imageFileModel.fileName)}`
            },
            showFileUploader(): boolean {
                const fileUploadComplete = this.image
                    && this.image.getFileModel()
                    && this.image.getFileModel().isUploadComplete();
                this.$emit("updatePublishingStatus");
                return !fileUploadComplete;
            },
            displayImage() : boolean {
                return isImageFileViewable(this.imageFileModel.fileName);
            },
            imageAltText() : string {
                return this.image.altText;
            }
        },
        methods: {
            newFileId(fileId: number) {
                // A fileId is available from the FileUploader
                // Set the fileId on this ImageModel
                this.image.setFileId(fileId);
            },
            updateImageFileModel() {
                this.imageFileModel = this.image.getFileModel();
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .media-block-image {
        .image-viewer {
            width: min(100%,200px);
            flex: 1 1 0;
            background-color: $nhsuk-grey-white;

            @media screen and (-ms-high-contrast: active), (-ms-high-contrast: none)
            {
                width: 100%;
                max-width: 600px;
            }
        }
        .alt-text-editor {
            margin-top: 10px;
            width: 345px;
            font-size: 16px;
            padding: 0 20px;
            line-height: 28px;
        }
    }
    picture {
        img {
            object-fit: contain;
            margin: 0 auto;
            display: block;
            max-width: 100%;
            max-height: 600px;
        }
    }

    .match-game-image {
        width: 166px;
        height: 166px;
        object-fit: fill;
    }
</style>