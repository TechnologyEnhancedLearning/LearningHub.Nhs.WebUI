<template>
    <div v-if="!dataLoaded" class="d-flex justify-content-center p-50">
        <div>
            <Spinner></Spinner>
        </div>
    </div>
    <div v-else class="assessment-results">
        <div class="assessment-summary-title">
            <h1 class="nhsuk-heading-l" v-if="assessmentResourceTitle">{{assessmentResourceTitle}} Summary</h1>
            <h1 class="nhsuk-heading-l" v-else>Assessment Summary</h1>
        </div>
        <div class="results-diagrams pb-40">
            <div class="circular-diagram">
                <h2 class="text-center">Your Score</h2>
                <div class="circle-center">
                    <PercentageCircle
                        :percentage="marksPercentage"
                        :text="`${ marksPercentage }%`"
                        :textSize="30"
                        :color="passMark !== null && marksPercentage < passMark ? 'orange' : 'green'"
                        class="m-10"/>
                </div>
                <div class="text-center" v-if="passMark && marksPercentage >= passMark">
                    <img class="medal-icon" src="/images/medal-icon.svg"/>
                    Passed
                </div>
                <div class="text-center" v-else-if="passMark">
                    <i class="fas fa-times-circle failed-icon" />
                    Failed
                </div>
            </div>
            <div class="circular-diagram">
                <h2 class="text-center">Attempts</h2>
                <div class="circle-center">
                    <PercentageCircle
                        :percentage="maximumAttempts === null ? 0 : (numberOfAttempts / maximumAttempts) * 100"
                        :text="attemptsLeftText"
                        :textSize="20"
                        :color="maximumAttempts && numberOfAttempts >= maximumAttempts ? 'red' : 'blue'"
                        class="m-10"/>
                </div>
            </div>
            <div class="try-again-box">
                <div class="more-attempts mr-20 d-flex flex-column justify-content-center" v-if="!maximumAttempts || maximumAttempts > numberOfAttempts">
                    <p class="text-center">{{ attemptsText }}</p>
                    <div class="d-flex justify-content-center" >
                        <Button v-on:click="retryAssessment" color="blue" :disabled="retrying">Try again</Button>
                    </div>
                </div>
                <div class="extra-attempt mr-20 flex-column justify-content-center text-center" v-else-if="passMark && marksPercentage < passMark && !this.assessmentSummary.passed">
                    <b>You have failed this assessment.</b>
                    <div class="info">To request an additional attempt and retake this assessment, please click the button below.</div>
                    <Button v-on:click="requestExtraAttempt" color="blue">Retake assessment</Button>
                </div>
                <div class="no-more-attempts mr-20 flex-column justify-content-center" v-else>
                    <p>There has been a limit set on the number of attempts for this assessment.</p>
                    <p>As you have passed this assessment, there are no more attempts or retries available.</p>
                </div>
                <ExtraAssessmentAttemptRequestModal v-if="showExtraAssessmentAttemptRequestModal"
                                                    :retrying="retrying"
                                                    :maxLength="240"
                                                    v-on:closeExtraAttemptRequestModal="showExtraAssessmentAttemptRequestModal = false"
                                                    v-on:requestExtraAttempt="retryAssessment"/>
            </div>

        </div>
        <template v-if="hasEndGuidance">
            <h2>What to focus on next</h2>
            <BlocksView
                :blocks="endGuidance.blocks"
                :allow-questions="false"
                :is-last="false"/>
        </template>
        <div class="flex" v-if="assessmentDetails.assessmentType === AssessmentTypeEnum.Informal">
            <h2>Summary</h2>
            <div class="mark-count text-right pr-20">{{ userScore }} out of {{ maxScore }} marks</div>
            <BlocksView :blocks="summaryQuestions.blocks"
                        :previouslySubmittedAnswers="previouslySubmittedAnswers"
                        :matchQuestionsState="matchQuestionsState"
                        allowQuestions
                        isLast
                        isOnSummaryPage
                        @goToPage="value => $emit('goToQuestion', value)"/>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import _ from "lodash";
    import Button from '../../globalcomponents/Button.vue';
    import PercentageCircle from "../../globalcomponents/PercentageCircle.vue";
    import BlocksView from './BlocksView.vue';
    import { ResourceInjection } from "../interfaces/injections";
    import { AssessmentTypeEnum } from "../../models/contribute-resource/blocks/assessments/assessmentTypeEnum";
    import { BlockTypeEnum } from "../../models/contribute-resource/blocks/blockTypeEnum";
    import QuestionBlock from "./QuestionBlock.vue";
    import { BlockCollectionModel } from "../../models/contribute-resource/blocks/blockCollectionModel";
    import { assessmentResourceHelper } from "../helpers/assessmentResourceHelper";
    import { AssessmentProgressModel } from "../../models/mylearning/assessmentProgressModel";
    import ExtraAssessmentAttemptRequestModal from "../ExtraAssessmentAttemptRequestModal.vue";
    import Spinner from "../../globalcomponents/Spinner.vue";
    
    export default (Vue as Vue.VueConstructor<Vue & ResourceInjection>).extend({
        inject: ["resourceType", "assessmentDetails"],
        components: {
            ExtraAssessmentAttemptRequestModal,
            PercentageCircle,
            Button,
            BlocksView,
            QuestionBlock,
            Spinner
        },
        props: {
            assessmentResourceActivityId: Number,
            assessmentResourceTitle: String,
            previouslySubmittedAnswers: { type: Array } as PropOptions<number[][]>,
            initialAssessmentProgress: { type: Object } as PropOptions<AssessmentProgressModel>,
        },
        beforeCreate() {
            this.$options.components.BlocksView = BlocksView;
        },
        data() {
            return {
                numberOfAttempts: 0,
                userScore: 0,
                maxScore: 1,
                marksPercentage: 0,
                content: new BlockCollectionModel(),
                endGuidance: this.assessmentDetails.endGuidance,
                maximumAttempts: this.assessmentDetails.maximumAttempts,
                passMark: this.assessmentDetails.passMark,
                AssessmentTypeEnum: AssessmentTypeEnum,
                assessmentSummary: undefined,
                retrying: false,
                showExtraAssessmentAttemptRequestModal: false,
                dataLoaded: false,
                matchQuestionsState: [],
            }
        },
        async created() {
            if (typeof this.initialAssessmentProgress?.userScore === 'number') {
                this.assessmentSummary = this.initialAssessmentProgress;
            } else {
                this.assessmentSummary = await this.getAssessmentSummary();
            }
            this.content = this.assessmentSummary.assessmentViewModel.assessmentContent;
            this.numberOfAttempts = this.assessmentSummary.numberOfAttempts;
            this.userScore = this.assessmentSummary.userScore;
            this.maxScore = this.assessmentSummary.maxScore;
            this.matchQuestionsState = this.assessmentSummary.matchQuestions;
            this.marksPercentage = _.round(100 * (this.assessmentSummary.userScore / this.assessmentSummary.maxScore));
            this.dataLoaded = true;
        },
        computed: {
            attemptsText(): string {
                if (this.maximumAttempts === null) {
                    return "You can have as many attempts as you like.";
                } else {
                    return `You have ${this.maximumAttempts - this.numberOfAttempts} more attempt${this.maximumAttempts - this.numberOfAttempts === 1 ? '' : 's'} available.`;
                }
            },
            
            attemptsLeftText(): string {
                if (this.maximumAttempts === null) {
                    return `${this.numberOfAttempts}`;
                } else {
                    return `${this.numberOfAttempts} of ${this.maximumAttempts}`;
                }
            },

            summaryQuestions(): BlockCollectionModel {
                const questions = this.content.blocks.filter(block =>
                    block.blockType == BlockTypeEnum.Question);
                const blockCollection = new BlockCollectionModel();
                blockCollection.blocks = questions;
                return blockCollection;
            },

            hasEndGuidance(): boolean {
                return this.endGuidance?.blocks?.some(block => block.blockType !== BlockTypeEnum.Text || block.textBlock?.content?.length > 0);
            }
        },
        methods: {
            async getAssessmentSummary(): Promise<AssessmentProgressModel> {
                return await assessmentResourceHelper.getAssessmentProgressFromActivity(this.assessmentResourceActivityId);
            },
            retryAssessment(reason: string) {
                this.retrying = true;
                this.$emit('retryAssessment', reason);
            },
            requestExtraAttempt() {
                this.showExtraAssessmentAttemptRequestModal = true;
            }
        }
    });
</script>

<style lang="scss">
    @use '../../../../Styles/abstracts/all' as *;
    
    .assessment-results {
        width: 100%;
    }
    
    .assessment-summary-title{
        display: flex;
        justify-content: center;
        text-align: center;
        margin: 5px 0 10px 0;
    }
    
    .circular-diagram {
        flex: 1 1 auto;
        
        .circle-center {
            text-align: center;
        }
    }
    
    .results-diagrams {
        display: flex;
        flex-wrap: wrap;
        width: 100%;
    }
    
    .try-again-box {
        background: $nhsuk-grey-white;
        flex: 1 1 auto;
        margin: 35px 0 35px 0;
        border: 1px solid $nhsuk-grey-light;
        border-radius: 3px;
        .more-attempts {
            padding: 40px 50px;
        }
        .no-more-attempts {
            padding: 20px 20px;
        }
        .extra-attempt {
            padding: 30px;
            .info {
                padding: 50px 0;
            }
            button {
                width: max-content;
                margin: 0 auto;
            }
        }
    }
    
    .mark-count {
        color: $nhsuk-grey;
    }

    .medal-icon {
        width: 30px;
        height: 30px;
    }
    
    .failed-icon {
        margin: 0;
        vertical-align: middle;
    }
</style>
