<template>
    <ExpansionPanel class="elevation-0 mb-0"
                    :value="currentPanelOpen"
                    @expansionPanelValueChanged="changeExpansionSelection"
                    @click.native="updateCurrentQuestion(blockRef)">
        <ExpansionPanelContent v-if="questionBlock.questionType === QuestionTypeEnum.MatchGame"
                               :hideCompletionStatus="true"
                               header="Set the match game type">
            <MatchGameConfiguration :questionBlock="questionBlock" :questionBlockRef="blockRef"/>
        </ExpansionPanelContent>
        <ExpansionPanelContent :isReady="questionBlock.isTitleReady()"
                               :header="`Set the question`">
            <QuestionBlockCollectionInput
                :blockCollection="questionBlock.questionBlockCollection"
                :textMaxLength="500"
                :placeholderText="'Type a question for the reader here.'"
                :imageZone="imageZone"/>
        </ExpansionPanelContent>
        <ExpansionPanelContent :isReady="questionBlock.answerIsReadyToPublish()" 
                               :header="contributeSectionTitle">
            <SingleBestAnswer v-if="questionBlock.questionType === QuestionTypeEnum.SingleChoice"
                              :questionBlock="questionBlock"/>
            <ConsiderTheOptions v-else-if="questionBlock.questionType === QuestionTypeEnum.MultipleChoice"
                                :questionBlock="questionBlock"/>
            <ImageZone v-else-if="questionBlock.questionType === QuestionTypeEnum.ImageZone"
                       :questionBlock="questionBlock"
                       :resourceType="resourceType"
                       @annotateWholeSlideImage="showSlideWithAnnotations"/>
            <MatchGame v-else v-bind:questionBlock="questionBlock"/>
        </ExpansionPanelContent>
        <ExpansionPanelContent v-if="isFeedbackVisible"
                               :isReady="feedbackIsReady"
                               :header="`Provide some feedback`">
            <QuestionBlockCollectionInput
                :blockCollection="questionBlock.feedbackBlockCollection"
                :textMaxLength="1000"
                :placeholderText="'Type feedback for the reader here.'"
                @updated="updateFeedback"
                :imageZone="imageZone"
                :richText="true"/>
        </ExpansionPanelContent>
        <ExpansionPanelContent v-if="isAllowRevealVisible"
                               :isReady="(typeof questionBlock.allowReveal === 'boolean')"
                               :header="`Decide how the question should behave`">
            <AllowReveal :questionBlock="questionBlock"
                         @updatedFeedback="updateFeedback"/>
        </ExpansionPanelContent>
    </ExpansionPanel>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import SingleBestAnswer from "./components/questions/SingleBestAnswer.vue";
    import ConsiderTheOptions from "./components/questions/ConsiderTheOptions.vue";
    import ImageZone from "./components/questions/ImageZone.vue";
    import AllowReveal from "./components/questions/AllowReveal.vue";
    import { QuestionBlockModel } from '../models/contribute-resource/blocks/questionBlockModel';
    import { QuestionTypeEnum } from '../models/contribute-resource/blocks/questions/questionTypeEnum';
    import Tick from '../globalcomponents/Tick.vue';
    import ExpansionPanel from "../globalcomponents/ExpansionPanel.vue";
    import QuestionBlockCollectionInput from './components/questions/QuestionBlockCollectionInput.vue';
    import ExpansionPanelContent from "../globalcomponents/ExpansionPanelContent.vue";
    import Button from '../globalcomponents/Button.vue';
    import { ContributeCaseQuestionsInjection, ContributeInjection } from './interfaces/injections';
    import { BlockCollectionModel } from '../models/contribute-resource/blocks/blockCollectionModel';
    import { BlockTypeEnum } from '../models/contribute-resource/blocks/blockTypeEnum';
    import { resourceTypeHasAllowReveal, resourceTypeHasFeedback } from './helpers/CaseOrAssessmentsHelper';
    import MatchGame from "./components/questions/MatchGame.vue";
    import MatchGameConfiguration from "./components/questions/MatchGameConfiguration.vue";
    import { WholeSlideImageModel } from "../models/contribute-resource/blocks/wholeSlideImageModel";
    import { ResourceType } from "../constants";

    /**
     * Creates an event of the given type. If the event cannot be constructed using the Event constructor (IE11),
     * then the event is created using the deprecated document.createEvent.
     * 
     * @param type - The type/name of the event
     */
    const createEvent = (type: string): Event => {
        try {
            return new Event(type);
        } catch (err) {
            const event = document.createEvent('UIEvents');
            event.initEvent(type, true, false);
            return event;
        }
    };

    export default (Vue as Vue.VueConstructor<Vue & ContributeCaseQuestionsInjection & ContributeInjection>).extend({
        inject: ['questionData', 'updateCurrentQuestion', 'resourceType', 'assessmentDetails'],
        props: {
            questionBlock: { type: Object } as PropOptions<QuestionBlockModel>,
            blockRef: Number,
            resourceType: { type: Number } as PropOptions<ResourceType>,
        },
        components: {
            MatchGame,
            SingleBestAnswer,
            ConsiderTheOptions,
            ImageZone,
            AllowReveal,
            Tick,
            ExpansionPanel,
            QuestionBlockCollectionInput,
            ExpansionPanelContent,
            Button,
            MatchGameConfiguration
        },

        data() {
            return {
                feedbackIsReady: false,
                QuestionTypeEnum,
                currentPanelOpen: this.questionData.currentQuestion === this.blockRef ? 0 : null,
                imageZone: this.questionBlock.questionType === QuestionTypeEnum.ImageZone,
            }
        },

        created() {
            // Set max length on feedback TextBlockModel - allows invalid length to immediately update the green tick statuses.
            if (this.questionBlock.feedbackBlockCollection.blocks.length === 0) {
                this.questionBlock.feedbackBlockCollection.addBlock(BlockTypeEnum.Text);
            }
            this.questionBlock.feedbackBlockCollection.blocks[0].textBlock.maxLength = 1000;
            this.questionBlock.feedbackBlockCollection.blocks[0].textBlock.richText = true;

            this.updateFeedback();

            if (!this.isAllowRevealVisible) {
                this.questionBlock.allowReveal = false;
            }
            if (!this.isFeedbackVisible) {
                this.questionBlock.feedbackBlockCollection = new BlockCollectionModel();
                this.questionBlock.feedbackBlockCollection.addBlock(BlockTypeEnum.Text);
                this.questionBlock.feedbackBlockCollection.blocks[0].textBlock.content = "Unspecified";
            }
        },

        computed: {
            isAllowRevealVisible(): boolean {
                return resourceTypeHasAllowReveal(this.resourceType);
            },
            isFeedbackVisible(): boolean {
                return resourceTypeHasFeedback(this.resourceType, this.assessmentDetails);
            },
            contributeSectionTitle(): string {
                switch(this.questionBlock.questionType) {
                    case QuestionTypeEnum.SingleChoice:
                        return "Set choices for answers";
                    case QuestionTypeEnum.MultipleChoice:
                        return "Set responses for the reader to consider";
                    case QuestionTypeEnum.MatchGame:
                        return "Set content pairs for the reader to match";
                    case QuestionTypeEnum.ImageZone:
                        return "Upload an image and add answer options for the learner to choose between";
                }
            }
        },

        watch: {
            'questionData.currentQuestion'() {
                if (this.questionData.currentQuestion === this.blockRef) {
                    if (this.currentPanelOpen === null) {
                        this.currentPanelOpen = 0;
                    }
                } else {
                    this.currentPanelOpen = null;
                }
            },
            currentPanelOpen() {
                // This event needs to be emitted so video player components can resize into the expanded/closed block view.
                requestAnimationFrame(() => window.dispatchEvent(createEvent('resize')));
            },
        },
        methods: {
            updateFeedback() {
                this.feedbackIsReady = this.questionBlock.feedbackBlockCollection.isReadyToPublish();
            },
            changeExpansionSelection(newVal: number) {
                this.currentPanelOpen = newVal;
            },
            showSlideWithAnnotations(wholeSlideImageToShow: WholeSlideImageModel, questionBlock: QuestionBlockModel) {
                this.$emit('annotateWholeSlideImage', wholeSlideImageToShow, questionBlock);
            }
        }
    });
</script>