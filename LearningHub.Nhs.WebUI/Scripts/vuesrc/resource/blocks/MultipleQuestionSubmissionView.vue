<template>
    <div>
        <div v-if="isSubmitted && showAnswers"
             class="px-20">
            <div class="header-title mb-15 d-flex">
                <span class="mr-3"
                      v-if="iconStyle !== 'fast-forward'"
                      :class="iconStyle"/>
                <img v-else
                     src="/images/fast-forward-circle.svg"
                     :class="iconStyle"/>
                <h2 class="mb-0">{{ headerMessage }}</h2>
                <img v-if="numCorrectMultipleChoiceAnswers() === answers.length && !isRevealed"
                     class="medal-icon ml-auto"
                     src="/images/medal-icon.svg"/>
            </div>

            <div>
                <BlocksView class="feedback-block-collection"
                            :blocks="feedback.blocks"
                            :sanitization="sanitization"/>
                <div class="mt-40"
                     v-if="canContinue">
                    <Button @click="$emit('nextPage')">Continue to next page</Button>
                </div>
                <div class="mt-40"
                     v-if="isOnSummaryPage">
                    <Button color="green"
                            @click="$emit('goToPage')">Visit the page
                    </Button>
                </div>
            </div>
        </div>
        <div class="d-flex align-items-center" v-else>
            <Button color="green"
                    :disabled="this.selectedAnswersProperty.some(answer => answer === undefined) || (isSubmitted && !showAnswers)"
                    @click="submitAnswer">
                Submit my answer
            </Button>
            <span class="ml-10" v-if="allowReveal">
                or <a href="javascript:void(0);" onclick="return false;" @click="revealAnswer">Reveal best answer</a>
            </span>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { AnswerTypeEnum } from '../../models/contribute-resource/blocks/questions/answerTypeEnum';
    import Button from '../../globalcomponents/Button.vue';
    import BlocksView from './BlocksView.vue';
    import { BlockCollectionModel } from '../../models/contribute-resource/blocks/blockCollectionModel';
    import { QuestionTypeEnum } from '../../models/contribute-resource/blocks/questions/questionTypeEnum';
    import * as sanitization from '../sanitization';
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";

    export default Vue.extend({
        props: {
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            selectedAnswersProperty: { type: Array } as PropOptions<number[]>,
            allowReveal: { type: Boolean } as PropOptions<boolean>,
            feedback: { type: Object } as PropOptions<BlockCollectionModel>,
            isSubmitted: { type: Boolean } as PropOptions<boolean>,
            showAnswers: { type: Boolean, default: false } as PropOptions<boolean>,
            canContinue: Boolean,
            isOnSummaryPage: { type: Boolean, default: false } as PropOptions<boolean>,
        },
        components: {
            Button,
        },
        beforeCreate() {
            this.$options.components.BlocksView = BlocksView;
        },
        data() {
            return {
                questionTypeEnum: QuestionTypeEnum,
                isRevealed: false,
                sanitization: sanitization.richText,
            };
        },
        computed: {
            iconStyle(): string {
                if (this.isRevealed) {
                    return "fast-forward";
                }
                if (this.numCorrectMultipleChoiceAnswers() === this.answers.length) {
                    return "fas fa-smile-beam";
                }
                return "fas fa-meh";
            },
            headerMessage(): string {
                if (this.isRevealed) {
                    return "The correct answers have been revealed";
                }
                if (this.numCorrectMultipleChoiceAnswers() === this.answers.length) {
                    return "Correct!";
                }
                if (this.numCorrectMultipleChoiceAnswers() === 0) {
                    return "Incorrect";
                }
                return "Not quite right";
            },
        },
        methods: {
            mapAnswerType(answerType: AnswerTypeEnum) {
                switch (answerType) {
                    case AnswerTypeEnum.Incorrect:
                        return 0;
                    case AnswerTypeEnum.Reasonable:
                        return 1;
                    case AnswerTypeEnum.Best:
                        return undefined;
                }
            },
            numCorrectMultipleChoiceAnswers() {
                return this.answers.filter((answer) => this.mapAnswerType(answer.status) === this.selectedAnswersProperty[answer.order])
                    .length;
            },
            revealAnswer() {
                this.$emit('update:isSubmitted', true);
                this.isRevealed = true;
                this.answers.forEach((answer) => {
                    switch (answer.status) {
                        case AnswerTypeEnum.Incorrect:
                            this.$emit('updateAnswer', answer.order, 0);
                            break;
                        case AnswerTypeEnum.Reasonable:
                            this.$emit('updateAnswer', answer.order, 1);
                            break;
                    }
                });
                this.$emit("updateQuestionProgress", true);

            },
            submitAnswer() {
                this.$emit('update:isSubmitted', true);
                this.$emit("updateQuestionProgress", true);
            },
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .feedback-block-collection {
        margin-top: -3rem;
    }

    .header-title {
        display: flex;
        align-items: center;
        cursor: default;
    }

    .fa-meh {
        font-size: 40px;
        color: $nhsuk-warm-yellow;
        margin-right: 5px;
    }

    .fa-smile-beam {
        font-size: 40px;
        color: $nhsuk-green;
        margin-right: 5px;
    }

    .fast-forward {
        width: 45px;
        height: 45px;
        margin-right: 8px;
    }

    .medal-icon {
        width: 45px;
        height: 45px;
    }
</style>
