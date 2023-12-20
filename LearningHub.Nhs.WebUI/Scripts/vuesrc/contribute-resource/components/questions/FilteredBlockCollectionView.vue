<template>
    <div class="view">
        <div v-for="(block, index) in filteredBlocks"
             :key="index">
            <ContributeBlock :block="block"
                             :enableUp="!BlockCollectionModel.isFirstBlock(block, filteredBlocks)"
                             :enableDown="!BlockCollectionModel.isLastBlock(block, filteredBlocks)"
                             :resourceType="resourceType"
                             :selectedToDuplicate="blocksToDuplicate.some(id => id === block.id)"
                             :canBeDuplicated="canBeDuplicated"
                             @up="blockCollection.moveBlockUp(block)"
                             @down="blockCollection.moveBlockDown(block)"
                             @duplicate="blockCollection.duplicateBlock(block)"
                             @delete="blockCollection.deleteBlock(block)"
                             @duplicateBlock="$emit('duplicateBlock', block.id)"
                             @annotateWholeSlideImage="showSlideWithAnnotations"></ContributeBlock>

        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { BlockCollectionModel } from '../../../models/contribute-resource/blocks/blockCollectionModel';
    import AddBlocksMenu from './AddBlocksMenu.vue';
    import ContributeBlock from '../../ContributeBlock.vue';
    import { BlockModel } from '../../../models/contribute-resource/blocks/blockModel';
    import { WholeSlideImageModel } from "../../../models/contribute-resource/blocks/wholeSlideImageModel";
    import { ResourceType } from "../../../constants";
    import { QuestionBlockModel } from "../../../models/contribute-resource/blocks/questionBlockModel";

    export default Vue.extend({
        components: {
            AddBlocksMenu,
        },
        props: {
            blockCollection: { type: Object } as PropOptions<BlockCollectionModel>,
            selection: { type: Function } as PropOptions<(blockCollection: BlockCollectionModel) => BlockModel[]>,
            resourceType: { type: Number } as PropOptions<ResourceType>,
            canBeDuplicated: { type: Boolean, default: true } as PropOptions<Boolean>,
            blocksToDuplicate: {
                type: Array,
                default: () => []
            } as PropOptions<number[]>,
        },
        computed: {
            filteredBlocks() {
                return this.selection(this.blockCollection);
            }
        },
        data() {
            return {
                BlockCollectionModel,
            };
        },
        beforeCreate() {
            // The ContributeBlock is imported like this to resolve a circular dependency issue
            this.$options.components.ContributeBlock = ContributeBlock;
        },
        methods: {
            showSlideWithAnnotations(wholeSlideImageToShow: WholeSlideImageModel, questionBlock: QuestionBlockModel) {
                this.$emit('annotateWholeSlideImage', wholeSlideImageToShow, questionBlock);
            },
        }
    });
</script>
