<template>
    <div class="blocks-view">
        <div v-for="(block, index) in blocks"
             :key="index">
            <TextBlock v-if="block.blockType === BlockTypeEnum.Text"
                       class="my-5"
                       :block="block"
                       :sanitization="sanitization"/>
            <WholeSlideImageBlock v-if="block.blockType === BlockTypeEnum.WholeSlideImage && !imageZone"
                                  :block="block"
                                  class="my-5"/>
            <div v-if="block.blockType === BlockTypeEnum.Media"
                 class="my-5">
                <h3 class="case-resource-item-title nhsuk-heading-m">{{ block.title }}</h3>
                <ContributeMediaBlockPublishedView :media-block="block.mediaBlock"/>
            </div>
            <ImageCarouselBlock v-if="block.blockType === BlockTypeEnum.ImageCarousel"
                                :block="block"
                                class="my-5"/>
        </div>
        <ExpansionPanel :value="selectedQuestionValue"
                        v-if="allowQuestions"
                        class="blocks-view-expansion-panel-wrapper">
            <QuestionBlock v-for="(block, index) in blocks"
                           v-if="block.blockType === BlockTypeEnum.Question" 
                           :id=generateId(index)
                           :key="block.blockRef"
                           :block="block"
                           :matchQuestionsState="matchQuestionsState.filter(qs => qs.questionNumber === block.order)"
                           :isEnabled="!answerInOrder || index === 0 || pagesProgress[index-1] || isOnSummaryPage"
                           :canContinue="block === lastQuestionBlock && allQuestionsAnswered && !isLast"
                           :currentPage="currentPage"
                           :previouslySubmittedAnswers="previouslySubmittedAnswer(index)"
                           :isOnSummaryPage="isOnSummaryPage"
                           :firstUnansweredQuestion="pagesProgress.map((e, i) => i < questionInFocus || e).findIndex(p => !p) === index"
                           :isQuestionInFocus="questionInFocus === index"
                           :answerInOrder="answerInOrder"
                           @updateQuestionProgress="isComplete => pushUpdate(index, isComplete)"
                           @nextPage="$emit('nextPage')"
                           @submitAssessmentAnswers="answers => $emit(`submitAssessmentAnswers`, answers, block.blockRef)"
                           @goToPage="$emit('goToPage', block.order)"
                           @setThisOnFocus="$emit('setQuestionInFocus', index, false)"
                           @removeThisFromFocus="$emit('setQuestionInFocus', index, true)"/>
        </ExpansionPanel>
    </div>
</template>


<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { BlockTypeEnum } from '../../models/contribute-resource/blocks/blockTypeEnum';
    import TextBlock from './TextBlock.vue';
    import WholeSlideImageBlock from './WholeSlideImageBlock.vue';
    import ContributeMediaBlockPublishedView
        from '../../contribute-resource/components/published-view/ContributeMediaBlockPublishedView.vue';
    import QuestionBlock from './QuestionBlock.vue';
    import ExpansionPanel from '../../globalcomponents/ExpansionPanel.vue';
    import { BlockModel } from '../../models/contribute-resource/blocks/blockModel';
    import * as sanitization from '../sanitization';
    import { ResourceInjection } from "../interfaces/injections";
    import { ResourceType } from '../../constants';
    import ImageCarouselBlock from "./ImageCarouselBlock.vue";
    type PendingProgressUpdate = [number, boolean];

    export default (Vue as Vue.VueConstructor<Vue & ResourceInjection>).extend({
        inject: ["resourceType", "assessmentDetails"],
        props: {
            blocks: { type: Array } as PropOptions<BlockModel[]>,
            allowQuestions: { type: Boolean, default: false } as PropOptions<boolean>,
            isLast: { type: Boolean, default: false },
            sanitization: { type: Object, default: () => sanitization.richText },
            currentPage: { type: Number } as PropOptions<number>,
            previouslySubmittedAnswers: { type: Array } as PropOptions<number[][]>,
            isOnSummaryPage: { type: Boolean, default: false },
            questionInFocus: { type: Number, default: 0 },
            selectedQuestionValue: { type: Number, default: undefined },
            answerInOrder: { type: Boolean, default: true },
            matchQuestionsState: { type: Array } as PropOptions<any[]>,
            imageZone: Boolean,
        },
        components: {
            ImageCarouselBlock,
            TextBlock,
            WholeSlideImageBlock,
            ContributeMediaBlockPublishedView,
            ExpansionPanel,
        },
        beforeCreate() {
            this.$options.components.QuestionBlock = QuestionBlock;
        },
        watch: {
            blocks() {
                requestAnimationFrame(this.applyUpdates);
            },
            questionInFocus() {
                const question = this.blocks[this.questionInFocus];
                if (question && this.allQuestionsAnswered) {
                    const element = document.getElementById(this.generateId(question.order - this.blocks[0].order));
                    if (element) {
                        setTimeout(() => {
                            element.scrollIntoView({ behavior: "smooth" });
                        }, 700);
                    }
                }
            },
            previouslySubmittedAnswers() {
                this.updatePreviousQuestions();
            }
        },
        data() {
            return {
                BlockTypeEnum,
                // A list of booleans corresponding to the blocks on the page. false if they are blocking the user from progressing to the next page.
                // The only blocks that currently block the user are questions which are unanswered.
                pagesProgress: this.blocks.map(block => block.blockType !== BlockTypeEnum.Question),
                pendingUpdates: [] as PendingProgressUpdate[],
                ResourceType,
            };
        },
        computed: {
            lastQuestionBlock(): BlockModel {
                return [...this.blocks].reverse().find(block => block.blockType === BlockTypeEnum.Question);
            },
            allQuestionsAnswered(): boolean {
                return this.pagesProgress.every(Boolean);
            },
        },
        methods: {
            pushUpdate(index: number, value: boolean) {
                this.pendingUpdates.push([index, value]);
                if (this.resourceType === ResourceType.CASE) {
                    this.applyUpdates();
                }
            },
            applyUpdates() {
                if (this.pendingUpdates.length === 0) return;
                for (const [index, value] of this.pendingUpdates) {
                    this.pagesProgress[index] = value;
                }
                this.pagesProgress = [...this.pagesProgress];
                this.pendingUpdates = [];
                this.$emit('updated', this.allQuestionsAnswered);
            },
            generateId(index: number) {
                return `question-block-${ this.currentPage }-${ index }`;
            },
            updatePreviousQuestions() {
                if (Array.isArray(this.previouslySubmittedAnswers)) {
                    const questions = this.blocks.filter(block => block.blockType === BlockTypeEnum.Question);
                    this.previouslySubmittedAnswers.forEach((answer, index) => {
                        this.pagesProgress[this.blocks.findIndex(question => questions[index].blockRef === question.blockRef)] = (answer !== undefined);
                    });
                }
            },
            previouslySubmittedAnswer(index: number) {
                if (Array.isArray(this.previouslySubmittedAnswers)) {
                    const questions = this.blocks.filter(block => block.blockType === BlockTypeEnum.Question);
                    const questionIndex = questions.findIndex(question => this.blocks[index].blockRef === question.blockRef);
                    const answer = this.previouslySubmittedAnswers[questionIndex];
                    if (answer !== null) {
                        return answer;
                    }
                }
                return undefined;
            }
        },
        created() {
            this.updatePreviousQuestions();
        },
        mounted() {
            if (!this.answerInOrder) {
                if (this.blocks.filter(b => b.blockType === BlockTypeEnum.Question).length !== 0) {
                    this.$emit('created', this.blocks.filter(b => b.blockType === BlockTypeEnum.Question)[0].order);
                }
            }
        }
    });
</script>

<style lang="scss">
    @use '../../../../Styles/abstracts/all' as *;

    .blocks-view-expansion-panel-wrapper {
        padding-right: 20px;

        .v-expansion-panel__header {
            background-color: $nhsuk-grey-white;
            padding-left: 4px;
            padding-right: 16px;
        }

        .v-expansion-panel__container:first-child {
            border-top: solid 1px $nhsuk-grey-lighter !important;
        }
    }
</style>
