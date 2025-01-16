<template>
    <div class="contribute-image-carousel-block">
        <div class="contribute-image-carousel-block-description">
            <div>
                <label for="description"> <b>Description text</b></label>
                <Tick v-bind:complete="!!imageCarouselBlock.description"></Tick>
                <p>
                    Provide a short description for the Carousel. This will be displayed to the user in the header bar beneath the title.
                </p>
            </div>
            <div class="text-field d-flex align-items-center">
                <div class="contribute-image-carousel-block-description-field">
                    <EditSaveFieldWithCharacterCount v-model="imageCarouselBlock.description"
                                                     addEditLabel="Description"
                                                     v-bind:characterLimit="250"
                                                     v-bind:isH3="true"
                                                     v-bind:inputId="description"></EditSaveFieldWithCharacterCount>
                </div>
            </div>
        </div>
        <div v-if="imageBlockCollection.blocks.length" class="contribute-image-carousel-block-images">
            <div v-for="imageBlock in imageCarouselBlock.imageBlockCollection.blocks" class="contribute-image-carousel-block-image-margins">
                <div>
                    <ContributeImageCarouselBlockItem :block="imageBlock"
                                                      :enableUp="!imageBlockCollection.isFirstBlock(imageBlock, imageBlockCollection)"
                                                      :enableDown="!imageBlockCollection.isLastBlock(imageBlock, imageBlockCollection)"
                                                      :resourceType="resourceType"
                                                      @up="imageBlockCollection.moveBlockUp(imageBlock)"
                                                      @down="imageBlockCollection.moveBlockDown(imageBlock)"
                                                      @delete="imageBlockCollection.deleteBlock(imageBlock)" />
                </div>

            </div>
        </div>
        <div class="contribute-image-carousel-block-add-image">
            <Button class="contribute-image-carousel-block-add-image--button"
                    @click="addMediaBlock(FileUploadType.Image)"
                    size="medium">
                + Add {{ this.imageBlockCollection.blocks > 0 ? 'more' : '' }} carousel images
            </Button>
            <div class="contribute-image-carousel-block-add-image--info">
                You can upload .jpg, .jpeg and .png files from your computer or other storage device you are connected to. Maximum file size 10.0 GB.
            </div>
            <input type="file"
                   ref="addMediaInput"
                   @change="uploadNewMediaFiles"
                   class="visually-hidden" />
        </div>
        <Modal v-if="fileUploadWarningOpen" v-on:cancel="fileUploadWarningOpen = false">
            <template v-slot:title>
                <WarningTriangle color="red"></WarningTriangle>
                Invalid image carousel file type.
            </template>
            <template v-slot:body>
                Only .jpeg, .jpg and .png files can be viewed in an image carousel.
                Other file types cannot be used. Please select an image in the correct format.
            </template>
            <template v-slot:buttons>
                <Button v-on:click="fileUploadWarningOpen = false"
                        class="mx-12 my-2">
                    Okay
                </Button>
            </template>
        </Modal>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { ResourceType } from "../constants";
    import { ImageCarouselBlockModel } from "../models/contribute-resource/blocks/imageCarouselBlockModel";
    import ContributeTextBlock from "./ContributeTextBlock.vue";
    import { BlockCollectionModel } from "../models/contribute-resource/blocks/blockCollectionModel";
    import EditSaveFieldWithCharacterCount from "../globalcomponents/EditSaveFieldWithCharacterCount.vue";
    import Tick from "../globalcomponents/Tick.vue";
    import Button from '../globalcomponents/Button.vue';
    import {
        FileUploadType,
        getFileExtensionAllowedList,
        startUploadsFromFileElement
    } from "../helpers/fileUpload";
    import { MediaTypeEnum } from "../models/contribute-resource/blocks/mediaTypeEnum";
    import MediaBlockImage from "./components/content-tab/MediaBlockImage.vue";
    import { MediaBlockModel } from "../models/contribute-resource/blocks/mediaBlockModel";
    import ContributeBlock from "./ContributeBlock.vue";
    import ContributeImageCarouselBlockItem from "./ContributeImageCarouselBlockItem.vue";
    import { isImageValidForCarousel } from "../helpers/attachmentTypeHelper";
    import Modal from "../globalcomponents/Modal.vue";
    import WarningTriangle from "../globalcomponents/WarningTriangle.vue";

    export default Vue.extend({
        components: {
            WarningTriangle,
            Modal,
            ContributeImageCarouselBlockItem,
            MediaBlockImage, Button, Tick, EditSaveFieldWithCharacterCount, ContributeTextBlock
        },
        props: {
            imageCarouselBlock: { type: Object } as PropOptions<ImageCarouselBlockModel>,
            resourceType: { type: Number } as PropOptions<ResourceType>,
            description: { type: String, default: 'description' },
        },
        data() {
            return {
                FileUploadType,
                MediaTypeEnum,
                fileUploadWarningOpen: false,
            }
        },
        beforeCreate() {
            // The ContributeBlock is imported like this to resolve a circular dependency issue
            this.$options.components.ContributeBlock = ContributeBlock;
        },
        computed: {
            description(): string {
                return this.imageCarouselBlock?.description;
            },
            imageBlockCollection(): BlockCollectionModel {
                return this.imageCarouselBlock.imageBlockCollection;
            }
        },
        methods: {
            addMediaBlock(fileUploadType: FileUploadType) {
                let inputElement = this.$refs.addMediaInput as any;
                inputElement.accept = getFileExtensionAllowedList(fileUploadType).join(',');
                inputElement.click();
            },
            async uploadNewMediaFiles(event: Event): Promise<void> {
                if (event.target && (event.target as HTMLInputElement).value) {
                    if (this.validImageCarouselImage(event.target as HTMLInputElement)) {
                        await startUploadsFromFileElement(
                            event.target as HTMLInputElement,
                            (fileId, mediaType) => this.addMediaBlockToImageBlockCollection(fileId, mediaType)
                        );
                    } else {
                        this.fileUploadWarningOpen = true;
                    }
                }
            },
            addMediaBlockToImageBlockCollection(fileId: number, mediaType: MediaTypeEnum) {
                this.imageBlockCollection.addMediaBlock(fileId, mediaType);
            },
            updateMediaBlockPublishingStatus(mediaBlock: MediaBlockModel) {
                mediaBlock.updatePublishingStatus();
            },
            validImageCarouselImage(target: HTMLInputElement): boolean {
                for (let i = 0; i < target.files.length; i++) {
                    const file = target.files[i] as File;
                    const fileName = file.name;
                    if (isImageValidForCarousel(fileName)) {
                        return true;
                    }
                }

                // Empty the list of files to fire "onchange" event
                target.value = '';
                return false;
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-image-carousel-block {
        .contribute-image-carousel-block-description {
            padding: 25px;
        }

        .contribute-image-carousel-block-description-tick {
            margin: 10px 10px 10px 0;
        }

        .contribute-image-carousel-block-description-field {
            width: 100%;
            margin-right: 10px;
        }

        .contribute-image-carousel-block-images {
            padding: 25px;
            border-top: 1px solid $nhsuk-grey-light;
        }

        .contribute-image-carousel-block-image-margins {
            margin-bottom: 25px;
        }

        .contribute-image-carousel-block-image-margins:last-child {
            margin-bottom: 0;
        }

        .contribute-image-carousel-block-add-image {
            display: flex;
            justify-content: flex-start;
            padding: 25px;
            border-top: 1px solid $nhsuk-grey-light;

            &--info {
                max-width: 700px;
                font-size: 16px;
                margin-left: 24px;
                margin-bottom: 0;
            }

            & > &--button {
                min-width: fit-content;
                height: 48px;
                margin: auto 0;
            }
        }
    }
</style>