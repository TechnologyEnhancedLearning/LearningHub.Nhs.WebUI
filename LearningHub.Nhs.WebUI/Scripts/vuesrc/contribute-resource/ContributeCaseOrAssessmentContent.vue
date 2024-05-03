<template>
    <div class="contribute-case-component lh-padding-fluid">
        <div class="lh-container-xl py-15">
            <div class="py-10 text-center placeholder-text"
                 v-if="!hasContentOnPage">
                You have not added any content to this page yet
            </div>
            <template v-else>
                <FilteredBlockCollectionView :resourceType="resourceType"
                                             :blockCollection="blockCollection"
                                             :selection="blockCollection => blockCollection.getBlocksByPage(page).filter(block => block.blockType !== BlockTypeEnum.Question)"
                                             :can-be-duplicated="duplicatingBlocks"
                                             :blocksToDuplicate="blocksToDuplicate"
                                             @annotateWholeSlideImage="showSlideWithAnnotations"
                                             @duplicateBlock="blockId => $emit('duplicateBlock', blockId)" />
            </template>

            <ContributeAddContentBlock @add="choosingNewContentBlock = true"></ContributeAddContentBlock>

            <ContributeChooseContentBlockType v-if="choosingNewContentBlock"
                                              @choose="chooseNewContentBlock"
                                              @cancel="choosingNewContentBlock = false"></ContributeChooseContentBlockType>

            <input type="file"
                   ref="addMediaInput"
                   multiple
                   @change="uploadNewMediaFiles"
                   class="visually-hidden" />

            <Modal v-if="avUnavailableMessage">
                <div v-html="audioVideoUnavailableView"></div>
                <button type="button" class="nhsuk-button nhsuk-button--secondary mt-2 col-4 col-sm-3 col-md-2" @click="cancelAVUnavailModal">Cancel</button>
            </Modal>   
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { resourceData } from '../data/resource';
    import ContributeBlock from './ContributeBlock.vue';
    import ContributeAddContentBlock from './ContributeAddContentBlock.vue';
    import ContributeChooseContentBlockType from './ContributeChooseContentBlockType.vue';
    import { BlockTypeEnum } from '../models/contribute-resource/blocks/blockTypeEnum';
    import { BlockCollectionModel } from '../models/contribute-resource/blocks/blockCollectionModel';
    import {
        FileUploadType,
        getAllowedFileExtensionsInAcceptFormat,
        startUploadsFromFileElement,
        getMediaTypeFromFileExtension
    } from '../helpers/fileUpload';
    import FilteredBlockCollectionView from './components/questions/FilteredBlockCollectionView.vue';
    import { WholeSlideImageModel } from "../models/contribute-resource/blocks/wholeSlideImageModel";
    import { ResourceType } from "../constants";
    import { MediaTypeEnum } from '../models/contribute-resource/blocks/mediaTypeEnum';
    import Modal from '../globalcomponents/Modal.vue';

    export default Vue.extend({
        props: {
            blockCollection: { type: Object } as PropOptions<BlockCollectionModel>,
            page: Number,
            resourceType: { type: Number } as PropOptions<ResourceType>,
            duplicatingBlocks: Boolean,
            blocksToDuplicate: { type: Array } as PropOptions<number[]>,
        },
        components: {
            ContributeBlock,
            ContributeAddContentBlock,
            ContributeChooseContentBlockType,
            FilteredBlockCollectionView,
            Modal,
        },
        data() {
            return {
                choosingNewContentBlock: false,
                BlockTypeEnum,
                contributeResourceAVFlag: true,
                avUnavailableMessage: false
            };
        },
        created() {
            this.getContributeResAVResourceFlag();
        },
        computed: {
            hasContentOnPage(): boolean {
                return this.blockCollection?.getBlocksByPage(this.page)?.filter(block => block.blockType !== BlockTypeEnum.Question).length > 0;
            },
            audioVideoUnavailableView(): string {
                return this.$store.state.getAVUnavailableView;
            }
        },
        methods: {
            chooseNewContentBlock(blockType: BlockTypeEnum) {
                this.choosingNewContentBlock = false;

                if (!this.blockCollection) {
                    this.$emit('update:blockCollection', new BlockCollectionModel());
                }

                switch (blockType) {
                    case BlockTypeEnum.Media:
                        let inputElement = this.$refs.addMediaInput as any;
                        inputElement.accept = getAllowedFileExtensionsInAcceptFormat(FileUploadType.Media);
                        inputElement.click();
                        break;
                    default:
                        this.blockCollection.addBlock(blockType, this.page);
                }
            },
            async uploadNewMediaFiles(event: any): Promise<void> {
                var targetItem = event.target as HTMLInputElement;
                var startUpload = true;

                if (targetItem.value !== '') {
                    for (let i = 0; i < targetItem.files.length; i++) {
                        const file = targetItem.files[i] as File;
                        const fileExtension = file.name.split('.').pop();
                        const mediaType = getMediaTypeFromFileExtension(`.${fileExtension}`);

                        if (!this.contributeResourceAVFlag && (mediaType === MediaTypeEnum.Video)) {
                            startUpload = false;
                            this.avUnavailableMessage = true;
                        }
                        else { startUpload = true; }
                    }
                }

                if (startUpload) {
                    startUploadsFromFileElement(
                        event.target as HTMLInputElement,
                        (fileId, mediaType) => this.blockCollection.addMediaBlock(fileId, mediaType, this.page)
                    );
                }
                targetItem.value = '';
            },
            showSlideWithAnnotations(wholeSlideImageToShow: WholeSlideImageModel) {
                this.$emit('annotateWholeSlideImage', wholeSlideImageToShow, false);
            },
            cancelAVUnavailModal() {
                this.avUnavailableMessage = false;
            },
            getContributeResAVResourceFlag() {
                resourceData.getContributeAVResourceFlag().then(response => {
                    this.contributeResourceAVFlag = response;
                });
            }
        },
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-case-component {
        background-color: $nhsuk-grey-white;
        overflow: auto;
    }

    .placeholder-text {
        color: $nhsuk-grey;
    }
</style>