<template>
    <ExpansionPanel class="blocks-view-expansion-panel-wrapper" v-bind:value="0">
        <v-expansion-panel-content expand-icon="" v-model="isOpen" readonly value=true>
            <template v-slot:header>
                <div class="contribute-image-carousel-image-component-header d-flex align-center align-items-center">
                    <div class="icon-button">
                        <IconButton v-on:click="isOpen = !isOpen"
                                    :iconClasses="'fas  blue ' + (isOpen ? `fa-minus-circle` : `fa-plus-circle`)"
                                    :ariaLabel="isOpen ? `hide content` : `reveal content`"></IconButton>
                    </div>
                    <div class="text-field align-left d-flex align-items-center">
                        <EditSaveFieldWithCharacterCount v-model="block.title"
                                                         addEditLabel="title"
                                                         v-bind:characterLimit="60"
                                                         v-bind:isH3="true"></EditSaveFieldWithCharacterCount>
                        <Tick v-bind:complete="block.isReadyToPublish()" class="p-10"></Tick>
                    </div>
                    <div class="d-flex justify-content-center icon-button">
                        <IconButton v-if="enableUp"
                                    v-on:click="$emit('up')"
                                    iconClasses="fa-solid fa-arrow-up"
                                    ariaLabel="Move section up"
                                    class="contribute-image-carousel-image-component-button"></IconButton>
                    </div>
                    <div class="d-flex justify-content-center icon-button">
                        <IconButton v-if="enableDown"
                                    v-on:click="$emit('down')"
                                    iconClasses="fa-solid fa-arrow-down"
                                    ariaLabel="Move section down"
                                    class="contribute-image-carousel-image-component-button"></IconButton>
                    </div>
                    <IconButton v-on:click="discardBlockModalOpen = true"
                                iconClasses="fa-solid fa-trash-can-alt"
                                ariaLabel="Delete section"
                                class="contribute-image-carousel-image-component-button icon-button"></IconButton>
                    <Modal v-if="discardBlockModalOpen" v-on:cancel="discardBlockModalOpen = false">
                        <template v-slot:title>
                            <WarningTriangle color="yellow"></WarningTriangle>
                            Delete carousel image
                        </template>
                        <template v-slot:body>
                            This cannot be undone.
                            Do you want to continue?
                        </template>
                        <template v-slot:buttons>
                            <Button v-on:click="discardBlockModalOpen = false"
                                    class="mx-12 my-2">
                                Cancel
                            </Button>
                            <Button color="red"
                                    v-on:click="deleteCarouselImage"
                                    class="mx-12 my-2">
                                Delete section
                            </Button>
                        </template>
                    </Modal>
                </div>
            </template>
            <v-card class="border-top contribute-image-carousel-background" v-if="block.mediaBlock.image">
                <div class="contribute-image-carousel-block">
                    <MediaBlockCarouselImage v-if="block.mediaBlock.mediaType === MediaTypeEnum.Image"
                                     v-on:updatePublishingStatus="updatePublishingStatus"
                                     v-bind:image="image"/>
                </div>
            </v-card>
        </v-expansion-panel-content>
    </ExpansionPanel>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import ContributeMediaBlock from "./components/content-tab/ContributeMediaBlock.vue";
    
    import Button from '../globalcomponents/Button.vue';
    import IconButton from '../globalcomponents/IconButton.vue';
    import Modal from '../globalcomponents/Modal.vue';
    import Tick from '../globalcomponents/Tick.vue';
    import WarningTriangle from '../globalcomponents/WarningTriangle.vue';
    import ExpansionPanel from '../globalcomponents/ExpansionPanel.vue';
    
    import { BlockModel } from "../models/contribute-resource/blocks/blockModel";
    import { ResourceType } from "../constants";
    import { BlockTypeEnum } from "../models/contribute-resource/blocks/blockTypeEnum";
    import { MediaTypeEnum } from "../models/contribute-resource/blocks/mediaTypeEnum";
    import EditSaveFieldWithCharacterCount from "../globalcomponents/EditSaveFieldWithCharacterCount.vue";
    import { ImageModel } from "../models/contribute-resource/blocks/imageModel";
    import MediaBlockImage from "./components/content-tab/MediaBlockImage.vue";
    import { FileUploadType } from "../helpers/fileUpload";
    import { FileStore } from "../models/contribute-resource/files/fileStore";
    import MediaBlockCarouselImage from "./components/content-tab/MediaBlockCarouselImage.vue";

export default Vue.extend({
    components: {
        MediaBlockCarouselImage,
        MediaBlockImage,
        EditSaveFieldWithCharacterCount,
        Button,
        WarningTriangle,
        Modal,
        Tick,
        IconButton,
        ExpansionPanel,
        ContributeMediaBlock
    },
    props: {
        block: {type: Object} as PropOptions<BlockModel>,
        enableUp: Boolean,
        enableDown: Boolean,
        resourceType: {type: Number} as PropOptions<ResourceType>,
    },
    data() {
        return {
            editing: false,
            discardBlockModalOpen: false,
            BlockTypeEnum: BlockTypeEnum,
            MediaTypeEnum: MediaTypeEnum,
            blockTitleForEditing: '',
            isOpen: true,
            FileUploadType: FileUploadType,
            FileStore: FileStore /* It looks like FileStore isn't used, but we need it to be exposed here to allow Vue to make the files list reactive */,
        };
    },
    created() {
        this.isOpen = true;
    },
    computed: {
        image(): ImageModel {
            return this.block?.mediaBlock.image;
        },
    },
    methods: {
        editTitle() {
            this.blockTitleForEditing = this.block.title;
            this.editing = true;
        },
        saveTitle() {
            this.block.title = this.blockTitleForEditing;
            this.editing = false;
        },
        cancelSaveTitle() {
            this.blockTitleForEditing = '';
            this.editing = false;
        },
        updatePublishingStatus() {
            this.block?.mediaBlock.updatePublishingStatus();
        },
        deleteCarouselImage() {
            this.$emit('delete'); 
            this.discardBlockModalOpen = false;
        }
    }
});
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;
    
    .contribute-image-carousel-image {
        border: 1px solid $nhsuk-grey-lighter;
    }

    .blocks-view-expansion-panel-wrapper {
        border: 1px solid $nhsuk-grey-lighter;
        padding: 0 0 0 0;
        
        .v-expansion-panel__header {
            background-color: $nhsuk-grey-white;
            padding: 0 0 0 0;
        }
    }
    
    .contribute-image-carousel-image-component {
        border: 1px solid $nhsuk-grey-lighter;

        .contribute-image-carousel-image-component-header {
            padding: 0 8px 0 20px;
            min-height: 60px;
            border-bottom: 1px solid $nhsuk-grey-light;
        }

        .contribute-image-carousel-image-component-button {
            margin: 8px 2px;
        }
    }
    
    .contribute-image-carousel-image-component-header {
        padding: 0 8px 0 20px;
        min-height: 60px;
    }
    
    
    .contribute-image-carousel-image-component-description {
        border-top: 1px solid $nhsuk-grey-lighter;
        padding: 25px;
    }
    
    .contribute-image-carousel-background {
        background: $nhsuk-grey-white;
        padding: 0 0 0 0;
    }
    
    .contribute-image-carousel-block {
        margin: 0 25px 0 25px;
        height: 100%;
    }
    
    .block-title {
        margin: 19px;
    }
    
    .icon-button {
        flex-basis: 45px;
    }
    
    .tick {
        flex-basis: 30px;
    }
    
    .text-field {
        flex: 66%;
        word-break: break-word;
    }
</style>