<template>
    <div>
        <div v-if="isSubmitted && showAnswers" class="px-20">
            <div class="header-title mb-15 d-flex" v-if="!isOnSummaryPage">
                <img class="mr-3 icon" v-if="iconStyle === 'partially-correct'" src="/images/partially-correct.svg"/>
                <span class="mr-3 icon" v-else-if="iconStyle !== 'fast-forward'" :class="iconStyle"/>
                <img class="icon" v-else src="/images/fast-forward-circle.svg" :class="iconStyle"/>
                <h2 class="mb-0">{{ headerMessage }}</h2>
            </div>

            <div v-if="!allMatchAnswersCorrect() && !isOnSummaryPage">
                <p>Would you like to try again?</p>
                <div class="d-flex align-items-center">
                    <Button color="green" @click="tryAgain">Try again</Button>
                    <span class="ml-10" v-if="allowReveal">
                        or <a href="javascript:void(0);" @click="revealAnswer">Reveal the correct answer</a>
                    </span>
                </div>
            </div>

            <div v-else>
                <BlocksView class="feedback-block-collection" :blocks="feedback.blocks" :sanitization="sanitization"/>
                <div v-if="!allMatchAnswersCorrect() && isOnSummaryPage && correctMatches" class="mb-1">
                    You matched {{ correctMatches }} pair{{ correctMatches === 1 ? '' : 's' }} correctly
                </div>
                <details v-if="!allMatchAnswersCorrect() && isOnSummaryPage">
                    <summary class="mb-10">
                        Reveal the correct answer
                    </summary>
                    <div class="correct-answer-accordion-content">
                        <MatchGameView class="match-game-answer-container my-35 pl-9 pb-35"
                                       showCorrectAnswers
                                       canShowAnswers
                                       :matchQuestionType="matchQuestionType"
                                       :belongsToCase="false"
                                       :answers="answers"
                                       :matchQuestionsState="matchQuestionsState"/>
                    </div>
                </details>

                <div class="mt-40" v-if="canContinue">
                    <Button @click="$emit('nextPage')">Continue to next page</Button>
                </div>
                <div class="mt-40" v-if="isOnSummaryPage">
                    <Button color="green" @click="$emit('goToPage')">Visit the page</Button>
                </div>
            </div>
        </div>
        <div class="d-flex align-items-center" v-else>
            <Button color="green"
                    :disabled="!allPairsMatched || (isSubmitted && !showAnswers)"
                    @click="submitAnswer">
                Submit my answers
            </Button>
            <Button color="green"
                    :disabled="(!somePairsMatched && !isSubmitted) || (isSubmitted && !showAnswers)"
                    @click="resetMatches"  class="nhsuk-u-margin-left-3">
                Clear answers
            </Button>
            <span class="ml-10" v-if="allowReveal">
                or <a href="javascript:void(0);" onclick="return false;" @click="revealAnswer">Reveal the correct answer</a>
            </span>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import Button from "../../globalcomponents/Button.vue";
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";
    import { BlockCollectionModel } from "../../models/contribute-resource/blocks/blockCollectionModel";
    import BlocksView from "./BlocksView.vue";
    import * as sanitization from "../sanitization";
    import { MatchQuestionState } from "../../models/mylearning/matchQuestionState";
    import { questionHelper } from "../helpers/questionHelper";
    import { MatchGameSettings } from "../../models/contribute-resource/blocks/questions/matchGameSettings";
    import MatchGameView from "./MatchGameView.vue";
    
    export default Vue.extend({
        components: {
            Button,
            MatchGameView
        },
        props: {
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            selectedAnswersProperty: { type: Array } as PropOptions<number[]>,
            allowReveal: { type: Boolean } as PropOptions<boolean>,
            feedback: { type: Object } as PropOptions<BlockCollectionModel>,
            isSubmitted: { type: Boolean } as PropOptions<boolean>,
            showAnswers: { type: Boolean, default: false } as PropOptions<boolean>,
            canContinue: Boolean,
            isOnSummaryPage: { type: Boolean, default: false } as PropOptions<boolean>,
            allPairsMatched: Boolean,
            matchQuestionsState: { type: Array } as PropOptions<MatchQuestionState[]>,
            somePairsMatched: Boolean,
            matchQuestionType: { type: Number } as PropOptions<MatchGameSettings>,
        },
        beforeCreate() {
            this.$options.components.BlocksView = BlocksView;
        },
        data() {
            return {
                isRevealed: false,
                sanitization: sanitization.richText,
            }
        },
        computed: {
            correctMatches(): number {
                return questionHelper.getNumberOfCorrectMatches(this.selectedAnswersProperty);
            },
            headerMessage(): string {
                const incorrectMatches = this.selectedAnswersProperty.length - this.correctMatches;
                if (this.isRevealed) {
                    return "You revealed the answers.";
                } else if (!incorrectMatches) {
                    return "Correct";
                } else if (!this.correctMatches) {
                    return "Incorrect" + (this.isOnSummaryPage ? "" : ". You matched all pairs incorrectly");
                } else {
                    return "Partially correct, you matched " + this.correctMatches + " correctly and " + incorrectMatches + " incorrectly";
                }
            },
            iconStyle(): string {
                if (this.isRevealed) {
                    return "fast-forward";
                }
                if (this.allMatchAnswersCorrect()) {
                    return "fa-solid fa-circle-check";
                }
                if (!this.correctMatches) {
                    return "fas fa-times-circle";
                }
                return "partially-correct";
            },
        },
        methods: {
            submitAnswer() {
                this.$emit('update:isSubmitted', true);
                this.$emit("updateQuestionProgress", !this.showAnswers || this.allMatchAnswersCorrect());
            },
            tryAgain() {
                this.resetMatches();
                this.$emit('update:isSubmitted', false);
            },
            resetMatches() {
                this.$emit('resetMatchAnswers');
            },
            allMatchAnswersCorrect(): boolean {
                return questionHelper.getNumberOfCorrectMatches(this.selectedAnswersProperty) === this.selectedAnswersProperty.length;
            },
            mostMatchAnswersCorrect(): boolean {
                return questionHelper.areMostOfTheMatchesCorrect(this.selectedAnswersProperty);
            },
            revealAnswer() {
                this.$emit('update:isSubmitted', true);
                this.$emit('answerRevealed');
                this.isRevealed = true;
                this.$emit("updateQuestionProgress", true);
            },
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .feedback-block-collection {
        margin-top: -3rem;
    }
    
    .header-title {
        display: flex;
        align-items: center;
        cursor: default;
    }

    .fast-forward {
        width: 45px;
        height: 45px;
        margin-right: 8px;
    }

    .match-game-answer-container {
        max-height: 600px;
        overflow-y: auto;
    }

    summary {
        outline: none;
        color: $nhsuk-blue;
        text-decoration: underline;
    }

    .correct-answer-accordion-content {
        padding: 0 25px;
    }

    .icon {
        width: 45px;
        font-size: 45px;
    }

    .icon.fa-check-circle {
        color: $nhsuk-green;
    }
</style>
