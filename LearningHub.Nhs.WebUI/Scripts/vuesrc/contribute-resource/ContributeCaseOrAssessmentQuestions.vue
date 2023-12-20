<template>
    <div class="contribute-case-component lh-padding-fluid">
        <hr class="dividing-line lh-container-xl nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-0"/>
        <div class="lh-container-xl py-15">
            <div v-if="!hasQuestionsOnPage"
                 class="py-10 text-center placeholder-text">You have not added any questions to this page yet
            </div>
            <template v-else>
                <FilteredBlockCollectionView
                    :blockCollection="blockCollection"
                    :can-be-duplicated="duplicatingBlocks"
                    :blocksToDuplicate="blocksToDuplicate"
                    :selection="blockCollection => blockCollection.getBlocksByPage(page).filter(block => block.blockType === BlockTypeEnum.Question)"
                    @duplicateBlock="blockId => $emit('duplicateBlock', blockId)"
                    @annotateWholeSlideImage="showSlideWithAnnotations"/>
            </template>
            <ContributeAddQuestionBlock @add="choosingNewQuestionBlock = true"></ContributeAddQuestionBlock>
            <ContributeChooseQuestionBlockType v-if="choosingNewQuestionBlock"
                                               @choose="chooseNewQuestionBlock"
                                               @cancel="choosingNewQuestionBlock = false"></ContributeChooseQuestionBlockType>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import ContributeBlock from './ContributeBlock.vue';
    import ContributeChooseQuestionBlockType from "./ContributeChooseQuestionBlockType.vue";
    import ContributeAddQuestionBlock from './ContributeAddQuestionBlock.vue';
    import { BlockTypeEnum } from '../models/contribute-resource/blocks/blockTypeEnum';
    import { QuestionTypeEnum } from '../models/contribute-resource/blocks/questions/questionTypeEnum';
    import { BlockCollectionModel } from '../models/contribute-resource/blocks/blockCollectionModel';
    import FilteredBlockCollectionView from './components/questions/FilteredBlockCollectionView.vue';
    import { ContributeCaseQuestionsInjection } from './interfaces/injections';
    import { WholeSlideImageModel } from "../models/contribute-resource/blocks/wholeSlideImageModel";
    import { QuestionBlockModel } from "../models/contribute-resource/blocks/questionBlockModel";

    export default Vue.extend({
        props: {
            blockCollection: { type: Object } as PropOptions<BlockCollectionModel>,
            page: Number,
            duplicatingBlocks: Boolean,
            blocksToDuplicate: { type: Array } as PropOptions<number[]>,
        },
        components: {
            ContributeBlock,
            ContributeAddQuestionBlock,
            ContributeChooseQuestionBlockType,
            FilteredBlockCollectionView,
        },
        data() {
            return {
                choosingNewQuestionBlock: false,
                BlockTypeEnum,
                currentQuestion: 0,
            };
        },
        computed: {
            hasQuestionsOnPage(): boolean {
                return this.blockCollection?.getBlocksByPage(this.page)?.filter(block => block.blockType === BlockTypeEnum.Question).length > 0;
            }
        },
        methods: {
            chooseNewQuestionBlock(questionType: QuestionTypeEnum) {
                this.choosingNewQuestionBlock = false;
                if (!this.blockCollection) {
                    this.$emit('update:blockCollection', new BlockCollectionModel());
                }
                const newBlock = this.blockCollection.addBlock(BlockTypeEnum.Question, this.page);
                newBlock.questionBlock.questionType = questionType;
                this.currentQuestion = newBlock.blockRef;
            },
            showSlideWithAnnotations(wholeSlideImageToShow: WholeSlideImageModel, questionBlock: QuestionBlockModel) {
                this.$emit('annotateWholeSlideImage', wholeSlideImageToShow, true, questionBlock);
            },
        },

        provide() {
            const questionData = {};
            Object.defineProperty(questionData, "currentQuestion", {
                enumerable: true,
                get: () => this.currentQuestion
            });
            return {
                updateCurrentQuestion: (question: number) => {
                    this.currentQuestion = question;
                },
                questionData,
            } as ContributeCaseQuestionsInjection;
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

    .dividing-line {
        color: $nhsuk-grey;
        border-width: 2px;
    }
</style>