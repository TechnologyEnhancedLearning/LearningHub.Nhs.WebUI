<template>
    <div class="media-block-carousel-image">
        <FileUploader v-if="showFileUploader"
                      v-bind:file="imageFileModel"
                      v-bind:fileCategory="FileUploadType.Media"
                      v-on:newFileId="newFileId"/>
        <div v-else>
            <div v-if="!displayImage" class="d-flex flex-column align-items-center justify-content-center">
                <div class="download-descriptor">
                    <p>
                        Only .jpeg, .jpg and .png files can be viewed in an image carousel.
                        Other image types cannot be used.
                    </p>
                </div>
            </div>
            <div v-else class="d-flex flex-wrap justify-content-center">
                <div class="image-viewer p-24">
                    <picture>
                        <img v-bind:src="imageUrl" v-bind:alt="imageAltText">
                    </picture>
                </div>
                <div class="carousel-text-editor">
                    <label for="description"> <b>Description text</b> (Optional)</label>
                    <p>
                        Provide a short description of the image, which will be displayed to the user under the image in the carousel.
                    </p>
                    <EditSaveFieldWithCharacterCount v-model="image.description"
                                                     addEditLabel="Description"
                                                     v-bind:characterLimit="150"
                                                     v-bind:inputId="description"
                                                     block-view/>
                </div>
                <div class="carousel-text-editor">
                    <b>Alt text</b>
                    <Tick :complete="!!image.altText"></Tick>
                    <p>
                        Provide a short description of the image.
                        This will not be shown on screen but it will help those using screen readers.
                    </p>
                    <EditSaveFieldWithCharacterCount v-model="image.altText"
                                                     addEditLabel="alt text"
                                                     v-bind:characterLimit="125"
                                                     block-view />
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    
    import EditSaveFieldWithCharacterCount from "../../../globalcomponents/EditSaveFieldWithCharacterCount.vue";
    import { FileModel } from "../../../models/contribute-resource/files/fileModel";
    import { FileUploadType } from "../../../helpers/fileUpload";
    import { ImageModel } from "../../../models/contribute-resource/blocks/imageModel";
    import FileUploader from "../../../globalcomponents/FileUploader.vue";
    import FileInfo from "./FileInfo.vue";
    import IconButton from "../../../globalcomponents/IconButton.vue";
    import { isImageValidForCarousel } from "../../../helpers/attachmentTypeHelper";
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
             description: { type: String, default: 'description' },
        },
        data() {
            return {
                altText: '',
                FileUploadType: FileUploadType,
            }
        },
        computed: {
            imageFileModel(): FileModel {
                return this.image.getFileModel();
            },
            imageUrl(): string {
                return `/api/resource/DownloadResource`
                    + `?filePath=${encodeURIComponent(this.imageFileModel.filePath)}`
                    + `&fileName=${encodeURIComponent(this.imageFileModel.fileName)}`
            },
            showFileUploader(): boolean {
                const fileUploadComplete = this.image
                    && this.image.getFileModel()
                    && this.image.getFileModel().isUploadComplete();
                return !fileUploadComplete;
            },
            displayImage(): boolean {
                return isImageValidForCarousel(this.imageFileModel.fileName);
            },
            imageAltText(): string {
                return this.image.altText;
            },
        },
        methods: {
            newFileId(fileId: number) {
                // A fileId is available from the FileUploader
                // Set the fileId on this ImageModel
                this.image.setFileId(fileId);
            },
        }
});
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;
    
    .media-block-carousel-image {
        .image-viewer {
            width: 100%;
            max-width: 300px;
            flex: 1 1 0;
            background-color: $nhsuk-grey-white;
        }
        .carousel-text-editor {
            margin: 15px;
            width: 300px;
            font-size: 16px;
            padding: 0 20px;
            line-height: 28px;
            border-left: 1px solid $nhsuk-grey-lighter;
            flex: 1 0 auto;
        }
    }
    picture {
        img {
            object-fit: contain;
            margin: 0 auto;
            display: block;
            max-width: 250px;
            max-height: 100%;
        }
    }
</style>