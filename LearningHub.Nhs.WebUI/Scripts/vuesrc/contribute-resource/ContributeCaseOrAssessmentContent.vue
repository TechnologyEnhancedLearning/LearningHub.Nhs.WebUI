<template>
    <div class="contribute-case-component lh-padding-fluid">
        <div class="lh-container-xl py-15">
            <div class="py-10 text-center placeholder-text"
                 v-if="!hasContentOnPage">You have not added any content to this page yet
            </div>
            <template v-else>
                <FilteredBlockCollectionView
                    :resourceType="resourceType"
                    :blockCollection="blockCollection"
                    :selection="blockCollection => blockCollection.getBlocksByPage(page).filter(block => block.blockType !== BlockTypeEnum.Question)"
                    :can-be-duplicated="duplicatingBlocks"
                    :blocksToDuplicate="blocksToDuplicate"
                    @annotateWholeSlideImage="showSlideWithAnnotations"
                    @duplicateBlock="blockId => $emit('duplicateBlock', blockId)"/>
            </template>

            <ContributeAddContentBlock @add="choosingNewContentBlock = true"></ContributeAddContentBlock>

            <ContributeChooseContentBlockType v-if="choosingNewContentBlock"
                                              @choose="chooseNewContentBlock"
                                              @cancel="choosingNewContentBlock = false"></ContributeChooseContentBlockType>

            <input type="file"
                   ref="addMediaInput"
                   multiple
                   @change="uploadNewMediaFiles"
                   class="visually-hidden"/>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import ContributeBlock from './ContributeBlock.vue';
    import ContributeAddContentBlock from './ContributeAddContentBlock.vue';
    import ContributeChooseContentBlockType from './ContributeChooseContentBlockType.vue';
    import { BlockTypeEnum } from '../models/contribute-resource/blocks/blockTypeEnum';
    import { BlockCollectionModel } from '../models/contribute-resource/blocks/blockCollectionModel';
    import {
        FileUploadType,
        getAllowedFileExtensionsInAcceptFormat,
        startUploadsFromFileElement
    } from '../helpers/fileUpload';
    import FilteredBlockCollectionView from './components/questions/FilteredBlockCollectionView.vue';
    import { WholeSlideImageModel } from "../models/contribute-resource/blocks/wholeSlideImageModel";
    import { ResourceType } from "../constants";

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
        },
        data() {
            return {
                choosingNewContentBlock: false,
                BlockTypeEnum,
            };
        },
        computed: {
            hasContentOnPage(): boolean {
                return this.blockCollection?.getBlocksByPage(this.page)?.filter(block => block.blockType !== BlockTypeEnum.Question).length > 0;
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
                startUploadsFromFileElement(
                    event.target as HTMLInputElement,
                    (fileId, mediaType) => this.blockCollection.addMediaBlock(fileId, mediaType, this.page)
                );
            },
            showSlideWithAnnotations(wholeSlideImageToShow: WholeSlideImageModel) {
                this.$emit('annotateWholeSlideImage', wholeSlideImageToShow, false);
            },
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