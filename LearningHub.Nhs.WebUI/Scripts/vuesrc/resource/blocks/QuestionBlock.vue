<template>
    <v-expansion-panel-content expand-icon=""
                               v-model="isOpen"
                               class="blocks-view-expansion-panel"
                               :disabled="!isEnabled">
        <template v-slot:header>
            <div class="header d-flex align-items-center"
                 ref="expansioncontent">
                <IconButton v-if="isEnabled"
                            class="expansion-button"
                            :iconClasses="isOpen ? `fas fa-minus-circle blue` : `fas fa-plus-circle blue`"
                            :ariaLabel="isOpen ? `hide content` : `reveal content`"
                            iconRole="img"></IconButton>
                <i v-else
                   class="fas fa-lock expansion-button grey pl-12 pr-12"></i>
                <div class="text-field d-flex">
                    <span class="mr-auto">{{ block.title }}</span>
                    <span v-if="isOnSummaryPage && isSubmitted"
                          class="d-flex align-items-center">
                        <span class="px-2 grey">{{ summaryAnswerText() }}</span>
                        <img v-if="questionType === questionTypeEnum.MatchGame && overallAnswerType() === AnswerTypeEnum.Reasonable"
                             class="partially-correct-icon"
                             src="/images/partially-correct.svg"/>
                        <i v-else class="icon ml-2" :class="summaryIconStyle()" />
                    </span>
                    <span v-else-if="questionIsReadyToProgress()"
                          class="d-flex align-items-center">
                        <i v-if="belongsToCase"
                           class="fa-solid fa-circle-check px-1"/>
                        <span class="px-2 grey">This question is complete</span>
                        <div v-if="belongsToCase && questionType !== questionTypeEnum.MatchGame" class="medal-icon px-3">
                            <img v-if="areCorrectAnswersSubmitted()" alt="Medal icon" class="medal-icon" src="/images/medal-icon.svg"/>
                        </div>
                    </span>
                </div>
            </div>
        </template>
        <v-card class="border-top">
            <div class="question-container p-10 mb-50">
                <BlocksView :blocks="content.blocks"
                            class="mt-n5"
                            :sanitization="sanitization"
                            :class="{'font-weight-bold': questionType === questionTypeEnum.MatchGame && isOnSummaryPage}"
                            :imageZone="questionType === questionTypeEnum.ImageZone"/>
                <SingleQuestionAnswerView v-if="questionType === questionTypeEnum.SingleChoice"
                                          class="answer-container my-35 py-10 pl-9"
                                          :selectedAnswersProperty.sync="selectedAnswersProperty"
                                          :answers="answers"
                                          :isSubmitted="isSubmitted"
                                          :showAnswers="isOnSummaryPage || belongsToCase"
                                          :completed="isOnSummaryPage"
                                          :isRevealed="isRevealed"/>
                <MultipleQuestionAnswerView v-if="questionType === questionTypeEnum.MultipleChoice"
                                            class="answer-container my-35 pl-9"
                                            :answers="answers"
                                            :is-submitted="isSubmitted"
                                            :showAnswers="isOnSummaryPage || belongsToCase"
                                            :selectedAnswersProperty.sync="selectedAnswersProperty"
                                            @updateAnswer="updateAnswer"/>
                <MatchGameView v-if="questionType === questionTypeEnum.MatchGame"
                               class="match-game-answer-container my-35 pl-9"
                               :matchQuestionType="block.questionBlock.matchGameSettings"
                               :answers="answers"
                               :isSubmitted="isSubmitted"
                               :showAnswers="isOnSummaryPage || belongsToCase"
                               :completed="isOnSummaryPage || isRevealed"
                               :belongsToCase="belongsToCase"
                               :matchQuestionsState="matchQuestionsState"
                               :selectedAnswersProperty.sync="selectedAnswersProperty"
                               @selectedAnswersUpdated="selectedMatchAnswersUpdated"/>
                <ImageZoneAnswerView v-if="questionType === questionTypeEnum.ImageZone"
                                     class="answer-container my-35 py-10 pl-9"
                                     :blocks="content.blocks"
                                     :selectedAnswersProperty.sync="selectedAnswersProperty"
                                     :answers="answers"
                                     :isSubmitted="isSubmitted"
                                     :showAnswers="isOnSummaryPage || belongsToCase"
                                     :completed="isOnSummaryPage"
                                     :isRevealed="isRevealed"/>
                <SingleQuestionSubmissionView v-if="questionType === questionTypeEnum.SingleChoice"
                                              :answers="answers"
                                              :allowReveal="allowReveal"
                                              :selectedAnswersProperty="selectedAnswersProperty[0]"
                                              :feedback="feedback"
                                              :isSubmitted.sync="isSubmitted"
                                              :canContinue="canContinue"
                                              :showAnswers="isOnSummaryPage || belongsToCase"
                                              :completed="isOnSummaryPage"
                                              @nextPage="$emit('nextPage')"
                                              @answerRevealed="isRevealed = true"
                                              @disableRadio="disableRadio"
                                              @changedSingleChoiceSelection="changedSingleChoiceSelection"
                                              @goToPage="$emit('goToPage')"/>
                <MultipleQuestionSubmissionView v-if="questionType === questionTypeEnum.MultipleChoice"
                                                :answers="answers"
                                                :allowReveal="allowReveal"
                                                :selectedAnswersProperty="selectedAnswersProperty"
                                                :feedback="feedback"
                                                :isSubmitted.sync="isSubmitted"
                                                :canContinue="canContinue"
                                                :showAnswers="isOnSummaryPage || belongsToCase"
                                                :isOnSummaryPage="isOnSummaryPage"
                                                @nextPage="$emit('nextPage')"
                                                @updateQuestionProgress="value => $emit('updateQuestionProgress', true)"
                                                @disableRadio="disableRadio"
                                                @changedSingleChoiceSelection="changedSingleChoiceSelection"
                                                @updateAnswer="updateAnswer"
                                                @goToPage="$emit('goToPage')"/>
                <MatchGameSubmissionView v-if="questionType === questionTypeEnum.MatchGame"
                                         :answers="answers"
                                         :allowReveal="allowReveal"
                                         :selectedAnswersProperty="selectedAnswersProperty"
                                         :feedback="feedback"
                                         :isSubmitted.sync="isSubmitted"
                                         :matchQuestionType="block.questionBlock.matchGameSettings"
                                         :canContinue="canContinue"
                                         :showAnswers="isOnSummaryPage || belongsToCase"
                                         :isOnSummaryPage="isOnSummaryPage"
                                         :allPairsMatched="allPairsMatched"
                                         :matchQuestionsState="matchQuestionsState"
                                         :somePairsMatched="somePairsMatched"
                                         @resetMatchAnswers="resetMatchAnswers"
                                         @answerRevealed="matchAnswersRevealed"
                                         @updateQuestionProgress="value => $emit('updateQuestionProgress', value)"
                                         @goToPage="$emit('goToPage')"
                                         @nextPage="$emit('nextPage')"/>
                <ImageZoneSubmissionView v-if="questionType === questionTypeEnum.ImageZone"
                                         :answers="answers"
                                         :questionType="questionType"
                                         :allowReveal="allowReveal"
                                         :selectedAnswersProperty="selectedAnswersProperty[0]"
                                         :feedback="feedback"
                                         :isSubmitted.sync="isSubmitted"
                                         :canContinue="canContinue"
                                         :showAnswers="isOnSummaryPage || belongsToCase"
                                         :completed="isOnSummaryPage"
                                         @nextPage="$emit('nextPage')"
                                         @answerRevealed="isRevealed = true"
                                         @disableRadio="disableRadio"
                                         @changedSingleChoiceSelection="changedSingleChoiceSelection"
                                         @goToPage="$emit('goToPage')"/>
            </div>
        </v-card>
    </v-expansion-panel-content>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import IconButton from '../../globalcomponents/IconButton.vue';
    import { BlockModel } from '../../models/contribute-resource/blocks/blockModel';
    import BlocksView from './BlocksView.vue';
    import SingleQuestionAnswerView from './SingleQuestionAnswerView.vue';
    import MultipleQuestionAnswerView from './MultipleQuestionAnswerView.vue';
    import ImageZoneAnswerView from "./ImageZoneAnswerView.vue";
    import SingleQuestionSubmissionView from './SingleQuestionSubmissionView.vue';
    import MultipleQuestionSubmissionView from './MultipleQuestionSubmissionView.vue';
    import ImageZoneSubmissionView from "./ImageZoneSubmissionView.vue";
    import { QuestionTypeEnum } from '../../models/contribute-resource/blocks/questions/questionTypeEnum';
    import { AnswerTypeEnum } from '../../models/contribute-resource/blocks/questions/answerTypeEnum';
    import * as sanitization from '../sanitization';
    import { ResourceInjection } from "../interfaces/injections";
    import { ResourceType } from "../../constants";
    import MatchGameView from "./MatchGameView.vue";
    import MatchGameSubmissionView from "./MatchGameSubmissionView.vue";
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";
    import { questionHelper } from "../helpers/questionHelper";
    import { MatchQuestionState } from "../../models/mylearning/matchQuestionState";
    import ExpansionPanel from "../../globalcomponents/ExpansionPanel.vue";
    
    export default (Vue as Vue.VueConstructor<Vue & ResourceInjection>).extend({
        inject: ["resourceType"],
        props: {
            block: { type: Object } as PropOptions<BlockModel>,
            canContinue: Boolean,
            isEnabled: Boolean,
            currentPage: { type: Number } as PropOptions<number>,
            previouslySubmittedAnswers: { type: Array } as PropOptions<number[]>,
            isOnSummaryPage: { type: Boolean, default: false },
            isQuestionInFocus: { type: Boolean, default: false },
            firstUnansweredQuestion: Boolean,
            matchQuestionsState: { type: Array } as PropOptions<MatchQuestionState[]>,
            answerInOrder: Boolean,
            allQuestAnswered: { type: Boolean, default: false }
        },
        components: {
            MatchGameSubmissionView,
            MatchGameView,
            ImageZoneAnswerView,
            SingleQuestionAnswerView,
            MultipleQuestionAnswerView,
            ImageZoneSubmissionView,
            SingleQuestionSubmissionView,
            MultipleQuestionSubmissionView,
            IconButton,
            ExpansionPanel
        },
        beforeCreate() {
            this.$options.components.BlocksView = BlocksView;
        },
        mounted() {
            this.setExpansion(this.isEnabled && this.firstUnansweredQuestion && !this.previouslySubmittedAnswers);
        },
        data() {
            const length = (this.block.questionBlock.questionType === QuestionTypeEnum.SingleChoice) ? 1 : this.block.questionBlock.answers.length;
            return {
                content: this.block.questionBlock.questionBlockCollection,
                answers: this.block.questionBlock.answers as AnswerModel[],
                feedback: this.block.questionBlock.feedbackBlockCollection,
                allowReveal: this.block.questionBlock.allowReveal,
                questionType: this.block.questionBlock.questionType,
                questionTypeEnum: QuestionTypeEnum,
                selectedAnswersProperty: Array(length).fill(undefined),
                isSubmitted: false,
                isRevealed: false,
                isOpen: false,
                sanitization: sanitization.escapeAllTags,
                belongsToCase: this.resourceType == ResourceType.CASE,
                allPairsMatched: false,
                somePairsMatched: false,
                AnswerTypeEnum: AnswerTypeEnum
            };
        },
        computed: {
            isAssessment(): boolean {
                return this.resourceType === ResourceType.ASSESSMENT;
            }
        },
        watch: {
            firstUnansweredQuestion(value) {
                if (!this.answerInOrder) {
                    this.setExpansion(this.isEnabled && value && !this.previouslySubmittedAnswers);
                }
            },
            isEnabled(value) {
                if (!this.isOnSummaryPage) {
                    if (this.isAssessment) {
                        this.setExpansion(value);
                        if (value) {
                            this.$emit('setThisOnFocus');
                        }
                    } else {
                        this.isOpen = value;
                    }
                }
            },
            isSubmitted(value: boolean) {
                if (value && this.isAssessment && this.previouslySubmittedAnswers === undefined) {
                    if (this.questionType === QuestionTypeEnum.MultipleChoice) {
                        const answersToReturn = this.selectedAnswersProperty.map((answer, index) => answer === 1 ? index : false)
                            .filter(answer => answer !== false);
                        this.$emit("submitAssessmentAnswers", answersToReturn);
                    } else {
                        this.$emit("submitAssessmentAnswers", this.selectedAnswersProperty);
                    }
                }
            },
            block() {
                this.answers = this.block.questionBlock.answers;
            },
            previouslySubmittedAnswers(value: number[]) {
                this.checkPreviouslySubmittedAnswers(value);
            },
            isOpen(open) {
                if (open) {
                    this.$emit('setThisOnFocus');
                } else {
                    this.$emit('removeThisFromFocus');
                }
            },
            isQuestionInFocus(value) {
                this.isOpen = value;
                if (this.allQuestAnswered) {
                    this.isOpen = false;
                }
            }
        },
        methods: {
            selectedMatchAnswersUpdated() {
                this.allPairsMatched = this.selectedAnswersProperty.every(a => a !== undefined);
                this.somePairsMatched = this.selectedAnswersProperty.some(a => a !== undefined);
            },
            matchAnswersRevealed() {
                this.selectedAnswersProperty = Array.from({ length: this.answers.length }, (v, i) => i);
            },
            changedSingleChoiceSelection(value: number) {
                this.selectedAnswersProperty[0] = value;
                if (this.isAssessment) {
                    this.$emit("updateQuestionProgress", this.isSubmitted);
                } else {
                    this.$emit("updateQuestionProgress", this.isBestSubmitted());
                }
            },
            disableRadio(value: boolean) {
                this.isSubmitted = value;
            },
            updateAnswer(index: number, value: number) {
                this.selectedAnswersProperty[index] = value;
            },
            isBestSubmitted() {
                return this.isSubmitted &&
                    this.getSelectedAnswerStatus() === AnswerTypeEnum.Best;
            },
            questionIsReadyToProgress() {
                if (this.isAssessment) {
                    return this.isSubmitted;
                }

                return this.isRevealed || this.areCorrectAnswersSubmitted() ||
                    (this.questionType === QuestionTypeEnum.MultipleChoice && this.isSubmitted);
            },
            areCorrectAnswersSubmitted() {
                return this.isSubmitted && !this.isRevealed && 
                    (this.areMultipleChoiceAnswersCorrect() || this.areSingleChoiceOrImageZoneAnswersCorrect() || this.areMatchGameAnswersCorrect());
            },
            areMultipleChoiceAnswersCorrect() {
                return this.questionType === QuestionTypeEnum.MultipleChoice &&
                    this.selectedAnswersProperty.every((answer, i) => this.isMultipleChoiceAnswerCorrect(answer, i));
            },
            areMatchGameAnswersCorrect() {
                return this.questionType === QuestionTypeEnum.MatchGame && questionHelper.getNumberOfCorrectMatches(this.selectedAnswersProperty) === this.selectedAnswersProperty.length;
            },
            isMultipleChoiceAnswerCorrect(answer: AnswerTypeEnum, index: number) {
                return this.answers.find(a => a.order === index).status === answer;
            },
            areSingleChoiceOrImageZoneAnswersCorrect() {
                return (this.questionType === QuestionTypeEnum.SingleChoice || this.questionType === QuestionTypeEnum.ImageZone) &&
                    this.selectedAnswersProperty[0] !== undefined &&
                    this.getSelectedAnswerStatus() === AnswerTypeEnum.Best;
            },
            overallAnswerType() {
                if (this.areMultipleChoiceAnswersCorrect() || this.areSingleChoiceOrImageZoneAnswersCorrect() || this.areMatchGameAnswersCorrect()) {
                    return AnswerTypeEnum.Best;
                } else if ((this.questionType === QuestionTypeEnum.MatchGame && questionHelper.areMostOfTheMatchesCorrect(this.selectedAnswersProperty)) ||
                    (this.questionType === QuestionTypeEnum.SingleChoice && this.getSelectedAnswerStatus() === AnswerTypeEnum.Reasonable)) {
                    return AnswerTypeEnum.Reasonable;
                } else {
                    return AnswerTypeEnum.Incorrect;
                }
            },
            summaryIconStyle() {
                switch (this.overallAnswerType()) {
                    case AnswerTypeEnum.Best:
                        return "fa-solid fa-circle-check";
                    case AnswerTypeEnum.Reasonable:
                        return "fa-solid fa-circle-check fa-check-circle-blue";
                    case AnswerTypeEnum.Incorrect:
                        return "fas fa-times-circle";
                }
            },
            summaryAnswerText() {
                switch (this.overallAnswerType()) {
                    case AnswerTypeEnum.Best:
                        return "Correct (2 marks)";
                    case AnswerTypeEnum.Reasonable:
                        if (this.questionType === QuestionTypeEnum.MatchGame) {
                            return "Partially correct (1 mark)";
                        }
                        return "Reasonable choice (1 mark)";
                    case AnswerTypeEnum.Incorrect:
                        return "Incorrect (0 marks)";
                }
            },
            setExpansion(opened: boolean) {
                if (this.isOpen !== opened) {
                    (this.$refs.expansioncontent as Vue & { click: () => boolean }).click();
                }
            },
            checkPreviouslySubmittedAnswers(value: number[]) {
                if (value !== undefined) {
                    if (this.questionType !== QuestionTypeEnum.MultipleChoice) {
                        this.selectedAnswersProperty = value;
                    } else {
                        this.selectedAnswersProperty = (Array(this.block.questionBlock.answers.length).fill(0)).map(
                            (arrayValue, index) => value.includes(index) ? 1 : 0);
                    }
                    this.isSubmitted = true;
                }
            },
            getSelectedAnswerStatus(): AnswerTypeEnum {
                return this.answers.find(a => a.order === this.selectedAnswersProperty[0]).status;
            },
            resetMatchAnswers() {
                this.selectedAnswersProperty = Array(this.block.questionBlock.answers.length).fill(undefined);
            },
        },
        created() {
            this.checkPreviouslySubmittedAnswers(this.previouslySubmittedAnswers);
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .view-resource-text-block-content {
        font-weight: bold;
    }

    .question-container {
        background: $nhsuk-white;
    }

    .answer-container,
    .match-game-answer-container {
        border-top: 1px solid $nhsuk-grey-light;
        border-bottom: 1px solid $nhsuk-grey-light;
    }
    
    .match-game-answer-container {
        border-top: none;
        max-height: 600px;
        overflow-y: auto;
    }
    
    .expansion-button {
        flex-basis: 30px;
    }

    .text-field {
        flex: 45%;
    }

    .completed-field {
        flex: 55%;
    }

    .icon {
        font-size: 27px;
    }

    .fa-times-circle {
        color: $nhsuk-red;
    }

    .fa-check-circle {
        color: $nhsuk-green;
    }

    .fa-check-circle-blue {
        color: $nhsuk-blue;
    }

    .medal-icon {
        height: 30px;
        width: 30px;
    }

    .medal-icon-parent {
        min-width: 30px;
    }

    .partially-correct-icon {
        height: 27px;
    }

    .blocks-view-expansion-panel {
        background-color: $nhsuk-white;
        border: 1px solid $nhsuk-grey-lighter;
        margin: 20px 0px 20px 0px;
    }

    .grey {
        color: $nhsuk-grey;
    }

    .blocks-view {
        margin-top: -20px;
    }
</style>
