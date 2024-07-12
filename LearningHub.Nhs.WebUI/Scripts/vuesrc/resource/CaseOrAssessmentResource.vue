<template>
    <div v-if="loading"
         class="d-flex justify-content-center p-50">
        <div>
            <Spinner></Spinner>
        </div>
        <div class="ml-10">Loading...</div>
    </div>
    <div v-else
         class="flex"
         ref="resource">
        <PageSelector v-if="enablePagination" 
                      :currentPage="currentPage"
                      :pageCount="pageCount"
                      :canContinue="canContinue"
                      @goToPage="page => goToPage(page)"/>
        <div class="stop-overflow d-flex">
            <div class="case-resource-component">
                <BlocksView v-for="(pageNum,pageIndex) in pageCount"
                            v-if="currentPage === pageIndex"
                            class="case-resource-item"
                            :key="pageIndex"
                            :allowQuestions="true"
                            :matchQuestionsState="matchQuestionsState"
                            :blocks="blockCollection.getBlocksByPage(pageIndex)"
                            :isLast="pageNum === pageCount + (resourceItem.resourceTypeEnum === ResourceType.ASSESSMENT ? 1 : 0)"
                            :currentPage="pageNum"
                            :questionInFocus="questionsInFocus[pageIndex]"
                            :selectedQuestionValue="selectedQuestionValues[pageIndex]"
                            :previouslySubmittedAnswers="pageQuestionProgress[pageIndex]"
                            :answerInOrder="resourceItem.resourceTypeEnum !== ResourceType.ASSESSMENT || resourceItem.assessmentDetails.answerInOrder"
                            @nextPage="goToPage(++currentPage)"
                            @submitAssessmentAnswers="submitAssessmentAnswers"
                            @updated="isCompleted => updateProgress(pageIndex, isCompleted)"
                            @setQuestionInFocus="(value, remove) => {setQuestionInFocus(pageIndex, value, remove)}"
                            @created="value => openQuestionTab(pageIndex, value)"/>
                <AssessmentSummary class="case-resource-item"
                                   v-if="enableAssessmentSummary"
                                   :assessmentResourceActivityId="assessmentResourceActivityId"
                                   :assessment-resource-title="resourceItem.title"
                                   :previouslySubmittedAnswers="pagesProgress.questionProgress"
                                   v-show="currentPage === pageCount && !isPreview"
                                   @goToQuestion="goToQuestion"
                                   @retryAssessment="reason => retryAssessment(reason)"
                />
                <div v-if="this.isPreview && currentPage === pageCount">Summary is not available in preview mode</div>
                <div v-if="lastPage && allPagesCompleted && resourceItem.resourceTypeEnum === ResourceType.ASSESSMENT">
                    <p>You have answered all the questions in this assessment.</p>
                    <Button v-if="!summaryViewed && !pagesProgress.isNonQuestionPage(currentPage)"
                            color="green"
                            @click="goToPageFromButton(++currentPage)" class="nhsuk-u-margin-bottom-3">
                        Submit my answers 
                    </Button>
                    <Button v-else
                            :disabled="!canContinue"
                            color="green"
                            @click="goToPageFromButton(++currentPage)">
                        View Summary
                    </Button>
                </div>
                <div
                    v-else-if="(!!pagesProgress && currentPage < pageCount && pagesProgress.isNonQuestionPage(currentPage) || !pagesProgress.isIncomplete(currentPage)) && resourceItem.resourceTypeEnum === ResourceType.ASSESSMENT">
                    <p v-if="pagesProgress.isNonQuestionPage(currentPage)">This page has no questions.</p>
                    <p v-else>You have answered all the questions in this page.</p>
                    <Button :disabled="!canContinue"
                            color="green"
                            @click="goToPageFromButton(++currentPage)">
                        Continue to next page
                    </Button>
                </div>
            </div>
            <PageSidebar v-if="enablePagination"
                         class="case-page-sidebar"
                         :currentPage="currentPage"
                         :pageCount="pageCount"
                         :summaryViewed.sync="summaryViewed"
                         :pagesProgress="pagesProgress"
                         @goToPage="goToPage"/>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { userData } from '../data/user';
    import { resourceData } from '../data/resource';
    import BlocksView from './blocks/BlocksView.vue';
    import PageSelector from "./PageSelector.vue";
    import PageSidebar from "./PageSidebar.vue";
    import { PagesProgressModel } from '../models/pagesProgressModel';
    import { PageStatusEnum } from '../models/pageStatusEnum';
    import { BlockCollectionModel } from '../models/contribute-resource/blocks/blockCollectionModel';
    import { ResourceInjection } from "./interfaces/injections";
    import Button from '../globalcomponents/Button.vue';
    import Spinner from "../globalcomponents/Spinner.vue";
    import { ResourceType } from "../constants";
    import { ResourceItemModel } from '../models/resourceItemModel';
    import { activityRecorder } from '../activity';
    import AssessmentSummary from "./blocks/AssessmentSummary.vue";
    import { BlockTypeEnum } from '../models/contribute-resource/blocks/blockTypeEnum';
    import { AssessmentProgressModel } from '../models/mylearning/assessmentProgressModel';
    import { assessmentResourceHelper } from './helpers/assessmentResourceHelper';
    import { QuestionTypeEnum } from "../models/contribute-resource/blocks/questions/questionTypeEnum";
    import { AnswerModel } from "../models/contribute-resource/blocks/questions/answerModel";
    import {setResourceCetificateLink} from './helpers/resourceCertificateHelper';

    export default Vue.extend({
        components: {
            AssessmentSummary,
            BlocksView,
            PageSelector,
            PageSidebar,
            Button,
            Spinner,
        },
        props: {
            resourceItem: { type: Object } as PropOptions<ResourceItemModel>,
            resourceActivityId: { type: Number } as PropOptions<number>,
            assessmentProgress: { type: Object } as PropOptions<AssessmentProgressModel>,
            keepUserSessionAliveIntervalSeconds: { type: Number } as PropOptions<number>,
        },
        data() {
            return {
                ResourceType,
                PageStatusEnum,
                currentPage: 0,
                pageCount: 0,
                blockCollection: undefined as BlockCollectionModel,
                pagesProgress: undefined as PagesProgressModel,
                pageQuestionProgress: [],
                assessmentResourceActivityId: 0,
                allAssessmentInteractionsSubmitted: false,
                questionsInFocus: [],
                selectedQuestionValues: [],
                summaryViewed: false,
                initialCertificateStatus: null,
                matchQuestionsState: this.assessmentProgress?.matchQuestions || []
            };
        },
        provide(): ResourceInjection {
            const injection = {};

            Object.defineProperty(injection, "resourceType", {
                enumerable: true,
                get: () => this.resourceItem?.resourceTypeEnum,
            });

            Object.defineProperty(injection, "assessmentDetails", {
                enumerable: true,
                get: () => this.resourceItem?.assessmentDetails,
            });

            return injection as any;
        },
        computed: {
            lastPage(): boolean {
                return this.currentPage === this.pageCount - 1;
            },
            allPagesCompleted(): boolean {
                return this.pagesProgress.allPagesCompleted;
            },
            endGuidance(): BlockCollectionModel {
                return this.resourceItem?.assessmentDetails.endGuidance;
            },
            loading(): boolean {
                return !this.blockCollection ||
                    (!this.isPreview && this.isAssessment &&
                        (this.assessmentResourceActivityId === null || this.assessmentResourceActivityId === 0));
            },
            enablePagination(): boolean {
                return this.pageCount > 1 || this.isAssessment;
            },
            canContinue(): boolean {
                if (this.lastPage) {
                    return this.isAssessment && this.allPagesCompleted;
                }
                return this.pagesProgress.pageStatuses[this.currentPage + 1] !== PageStatusEnum.Locked;
            },
            assessmentProgressIsComplete(): boolean {
                return typeof this.assessmentProgress?.userScore === 'number';
            },
            enableAssessmentSummary(): boolean {
                return !this.isPreview && (this.allAssessmentInteractionsSubmitted || this.assessmentProgressIsComplete);
            },
            isPreview(): boolean {
                return this.resourceActivityId === null || this.resourceActivityId === undefined;
            },
            isAssessment(): boolean {
                return this.resourceItem.resourceTypeEnum === ResourceType.ASSESSMENT;
            }
        },
        methods: {
            shuffledArray(array: any[]) {
                return array.sort(() => 0.5 - Math.random());
            },
            shuffleMatchQuestionsState() {
                const matchQuestions = this.blockCollection.blocks.filter(b => b.blockType === BlockTypeEnum.Question && b.questionBlock.questionType === QuestionTypeEnum.MatchGame);
                this.matchQuestionsState = [];
                matchQuestions.forEach((q: any) => {
                    const answersToDisplay = this.shuffledArray([...q.questionBlock.answers]);
                    const firstAnswers = this.shuffledArray([...answersToDisplay]);
                    const secondAnswers = this.shuffledArray([...answersToDisplay]);
                    firstAnswers.forEach((firstAnswerId: any, index: number) => {
                        this.matchQuestionsState.push({
                            questionNumber: q.order,
                            firstMatchAnswerId: firstAnswerId.id,
                            secondMatchAnswerId: secondAnswers[index].id,
                            order: index
                        });
                    });
                });
            },
            goToPage(page: number) {
                this.currentPage = page;
                if (page !== this.pageCount) {
                    if (this.isAssessment && !this.resourceItem.assessmentDetails.answerInOrder) {
                        this.pagesProgress.readingPageAnswerInAnyOrder(page);
                    } else {
                        this.pagesProgress.readingPage(page);
                    }
                    if (this.pagesProgress.isNonQuestionPage(page)) {
                        this.updateProgress(page, true);
                    }
                }
            },
            goToQuestion(block: number) {
                this.currentPage = this.blockCollection
                    .blocks.filter((b, index) => index < block && b.blockType === BlockTypeEnum.PageBreak).length;
                this.selectedQuestionValues[this.currentPage] =
                    this.blockCollection.getBlocksByPage(this.currentPage)
                        .filter(b => b.blockType === BlockTypeEnum.Question)
                        .findIndex(x => x.order === block);
                this.questionsInFocus[this.currentPage] = this.getQuestionOrderInPage(this.currentPage, block);
                this.questionsInFocus = [...this.questionsInFocus];
                this.selectedQuestionValues = [...this.selectedQuestionValues];
            },
            updateProgress(page: number, isCompleted: boolean) {
                if (isCompleted) {
                    this.pagesProgress.completePage(page);
                }
                if (this.allPagesCompleted && this.isAssessment) {
                    this.allAssessmentInteractionsSubmitted = true;
                }
            },
            getBlockCollection() {
                switch (this.resourceItem.resourceTypeEnum) {
                    case ResourceType.CASE:
                        return this.resourceItem.caseDetails.blockCollection;
                    case ResourceType.ASSESSMENT:
                        return this.resourceItem.assessmentDetails.assessmentContent;
                    default:
                        return undefined;
                }
            },
            async submitAssessmentAnswers(answers: number[], blockRef: number) {
                const questions = this.blockCollection.blocks.filter(block => block.blockType === BlockTypeEnum.Question);
                const questionBlock = questions.find(block => block.blockRef === blockRef);
                const questionNumber = questions.indexOf(questionBlock);

                let assessmentContent: BlockCollectionModel;
                if (this.isPreview) {
                    assessmentContent = this.resourceItem.assessmentDetails.assessmentContent;
                } else {
                    const newAssessmentDetails = await activityRecorder.recordAssessmentResourceActivityInteraction(this.assessmentResourceActivityId, questionNumber, answers);
                    assessmentContent = newAssessmentDetails.assessmentContent;
                }
                this.pagesProgress.updateQuestionProgress(questionNumber, answers);
                this.updatePageQuestionProgress();

                if (assessmentContent !== null) {
                    this.applyBlockCollectionPatch(
                        new BlockCollectionModel(assessmentContent),
                        this.blockCollection.blocks.indexOf(questionBlock) + 1
                    );
                }
            },
            applyBlockCollectionPatch(newBlockCollection: BlockCollectionModel, applyPatchesAfter: number) {
                this.blockCollection.blocks = [
                    ...this.blockCollection.blocks.slice(0, applyPatchesAfter),
                    ...newBlockCollection.blocks.slice(applyPatchesAfter),
                ];
            },
            getQuestionOrderInPage(pageIndex: number, questionBlockOrder: number) {
                if (this.currentPage === 0) {
                    return questionBlockOrder;
                }
                const pageBreakOrder = this.blockCollection.blocks.filter(b => b.blockType === BlockTypeEnum.PageBreak)[this.currentPage - 1]?.order || 0;
                return questionBlockOrder - pageBreakOrder - 1;
            },
            setQuestionInFocus(pageIndex: number, value: number, remove: boolean) {
                if ((remove && this.questionsInFocus[pageIndex] === value)) {
                    this.questionsInFocus[pageIndex] = undefined;
                } else {
                    this.questionsInFocus[pageIndex] = value;
                    this.questionsInFocus = [...this.questionsInFocus];
                }
            },
            openQuestionTab(pageIndex: number, value: number) {
                this.selectedQuestionValues[pageIndex] =
                    this.blockCollection.getBlocksByPage(pageIndex)
                        .filter(b => b.blockType === BlockTypeEnum.Question)
                        .findIndex(x => x.order === value);
                this.selectedQuestionValues = [...this.selectedQuestionValues];
            },
            async retryAssessment(reason: string) {
                const latest = await assessmentResourceHelper.getAssessmentProgressFromResourceVersion(this.resourceItem.resourceVersionId);

                // Only make a new activity if the latest activity is finished
                if (typeof latest.userScore === 'number') {
                    const result = await activityRecorder.recordActivityLaunched(this.resourceItem.resourceTypeEnum, this.resourceItem.resourceVersionId, this.resourceItem.nodePathId, new Date(), reason);
                    this.shuffleMatchQuestionsState();
                    await activityRecorder.recordAssessmentResourceActivity(result.createdId, this.matchQuestionsState, reason);
                }

                window.location.href = window.location.pathname;
            },
            async goToPageFromButton(page: number) {
                this.$el.scrollIntoView({ behavior: "smooth" });
                this.checkUserCertificateAvailability();
                this.goToPage(page);
            },
            updatePageQuestionProgress() {
                this.pageQuestionProgress = Array(this.pageCount).fill(0).map((page, index) => this.pagesProgress.getQuestionProgressByPage(index));
            },
            keepUserSessionAlive() {
                setInterval(() => { userData.keepUserSessionAlive() }, this.keepUserSessionAliveIntervalSeconds);
            },
            async checkUserCertificateAvailability() {
                if (this.initialCertificateStatus == null) {
                    this.initialCertificateStatus = await resourceData.userHasResourceCertificate(this.resourceItem.id);
                }
                else if (this.resourceItem.certificateEnabled && this.initialCertificateStatus == false) {
                    let check = await resourceData.userHasResourceCertificate(this.resourceItem.id);
                    if (check == true) {
                        this.initialCertificateStatus = true;
                        setResourceCetificateLink(this.resourceItem.id.toString());
                        }
               }
               
           }
        },
        async created() {
            this.blockCollection = this.getBlockCollection();
            this.pageCount = this.blockCollection.getPages().length;
            this.pagesProgress = new PagesProgressModel(this.blockCollection);
            if (this.isAssessment) {
                if (typeof this.assessmentProgress?.userScore === 'number') {
                    this.currentPage = this.pageCount;
                }
                if (!this.resourceItem.assessmentDetails.answerInOrder) {
                    this.pagesProgress.pageStatuses = Array(this.pageCount).fill(PageStatusEnum.Available);
                }
            }
            if (!this.isAssessment || this.isPreview) {
                this.shuffleMatchQuestionsState();
            }
            this.checkUserCertificateAvailability();
            this.keepUserSessionAlive();
        },
        watch: {
            async resourceActivityId(value: number) {
                if (value && this.isAssessment) {
                    this.shuffleMatchQuestionsState();
                    const response = await activityRecorder.recordAssessmentResourceActivity(this.resourceActivityId, this.matchQuestionsState);
                    this.assessmentResourceActivityId = response.createdId;
                    this.goToPage(0);
                }
            },
            assessmentProgress(progress: AssessmentProgressModel) {
                if (progress !== null) {
                    this.assessmentResourceActivityId = progress.assessmentResourceActivityId;
                    this.matchQuestionsState = [...progress.matchQuestions];
                    this.pagesProgress.initialiseQuestionProgress(progress.answers);
                    this.updatePageQuestionProgress();
                    this.blockCollection = progress.assessmentViewModel.assessmentContent;

                    if (typeof progress.userScore === 'number') {
                        this.currentPage = this.pageCount;
                        this.pagesProgress.pageStatuses = this.pagesProgress.pageStatuses.map(v => PageStatusEnum.Completed);
                    } else {
                        this.goToPage(this.pagesProgress.getAvailableIncompletePage());
                    }
                }
            }
        }
    });
</script>

<style lang="scss">
    .case-resource-item-title {
        padding-bottom: 8px;
    }

    .stop-overflow {
        flex: 1 1 auto;
        max-width: 100%;
    }

    .case-resource-component {
        flex: 80%;
    }

    .case-page-sidebar {
        flex: 20%;
    }
</style>