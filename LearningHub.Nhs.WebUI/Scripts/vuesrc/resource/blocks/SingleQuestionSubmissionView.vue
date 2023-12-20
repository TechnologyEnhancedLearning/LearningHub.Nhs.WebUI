<template>
    <div>
        <div v-if="isSubmitted && showAnswers"
             class="px-20">
            <div class="header-title mb-15">
                <span class="mr-3"
                      v-if="iconStyle !== 'fast-forward'"
                      :class="iconStyle"/>
                <img v-else
                     src="/images/fast-forward-circle.svg"
                     :class="iconStyle"/>
                <h2 class="mb-0">{{ headerMessage }}</h2>
                <img v-if="this.answers.find(a => a.order === this.selectedAnswers).status === AnswerTypeEnum.Best && !isRevealed"
                     class="medal-icon ml-auto"
                     src="/images/medal-icon.svg"/>
            </div>
            <div v-if="!isBest() && !completed">
                <p>Would you like to try again?</p>
                <div class="d-flex align-items-center">
                    <Button @click="tryAgain" color="green">Try again</Button>
                    <span class="ml-10" v-if="allowReveal">
                        or <a href="javascript:void(0);" @click="revealAnswer">Reveal the answer</a>
                    </span>
                </div>
            </div>
            <div v-else>
                <BlocksView class="feedback-block-collection"
                            :blocks="feedback.blocks"
                            :sanitization="sanitization"/>
                <div class="mt-40"
                     v-if="canContinue">
                    <Button @click="$emit('nextPage')">Continue to next page</Button>
                </div>
                <div class="mt-40"
                     v-if="completed">
                    <Button color="green"
                            @click="$emit('goToPage')">Visit the page
                    </Button>
                </div>
            </div>
        </div>
        <div class="d-flex align-items-center" v-else>
            <Button
                color="green"
                :disabled="this.selectedAnswersProperty === undefined || (isSubmitted && !showAnswers)"
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
    import * as sanitization from '../sanitization';
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";
    import _ from "lodash";

    export default Vue.extend({
        props: {
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            selectedAnswersProperty: { type: Number } as PropOptions<number>,
            allowReveal: { type: Boolean } as PropOptions<boolean>,
            feedback: { type: Object } as PropOptions<BlockCollectionModel>,
            isSubmitted: { type: Boolean } as PropOptions<boolean>,
            showAnswers: { type: Boolean, default: false } as PropOptions<boolean>,
            completed: { type: Boolean, default: false } as PropOptions<boolean>,
            canContinue: Boolean,
        },
        components: {
            Button,
        },
        beforeCreate() {
            this.$options.components.BlocksView = BlocksView;
        },
        data() {
            return {
                selectedAnswers: this.selectedAnswersProperty,
                bestAnswerIndex: this.answers.find(a => a.status === AnswerTypeEnum.Best).order,
                isRevealed: false,
                AnswerTypeEnum: AnswerTypeEnum,
                sanitization: sanitization.richText,
            };
        },
        computed: {
            iconStyle(): string {
                if (this.isRevealed) {
                    return "fast-forward";
                }
                switch (this.getSelectedAnswerStatus()) {
                    case AnswerTypeEnum.Incorrect:
                        return "fas fa-meh";
                    case AnswerTypeEnum.Reasonable:
                        return "fas fa-smile";
                    case AnswerTypeEnum.Best:
                        return "fas fa-smile-beam";
                    default:
                        return "";
                }
            },
            headerMessage(): string {
                if (this.isRevealed) {
                    return "You revealed the answers.";
                }
                switch (this.getSelectedAnswerStatus()) {
                    case AnswerTypeEnum.Incorrect:
                        return "Not quite right";
                    case AnswerTypeEnum.Reasonable:
                        return "This is a good answer, but there is a better one.";
                    case AnswerTypeEnum.Best:
                        const answersInAlphabeticalOrder = _.orderBy(this.answers, 'blockCollection.blocks[0].textBlock.content', 'asc');
                        return `Option ${ answersInAlphabeticalOrder.findIndex(a => a.status === AnswerTypeEnum.Best) + 1 } is the best answer`;
                    default:
                        return "";
                }
            },
        },
        methods: {
            isIncorrect() {
                return this.isSubmitted && this.getSelectedAnswerStatus() === AnswerTypeEnum.Incorrect;
            },
            isReasonable() {
                return this.isSubmitted && this.getSelectedAnswerStatus() === AnswerTypeEnum.Reasonable;
            },
            isBest() {
                return this.isSubmitted && this.getSelectedAnswerStatus() === AnswerTypeEnum.Best;
            },
            revealAnswer() {
                this.$emit('update:isSubmitted', true);
                this.$emit('answerRevealed');
                this.isRevealed = true;
                this.selectedAnswers = this.bestAnswerIndex;
                this.$emit('changedSingleChoiceSelection', this.bestAnswerIndex);
            },
            submitAnswer() {
                this.$emit('update:isSubmitted', true);
                this.selectedAnswers = this.selectedAnswersProperty;
                this.$emit('changedSingleChoiceSelection', this.selectedAnswers);
            },
            tryAgain() {
                this.$emit('update:isSubmitted', false);
                this.selectedAnswers = undefined;
                this.$emit('changedSingleChoiceSelection', undefined);
            },
            getSelectedAnswerStatus(): AnswerTypeEnum {
                return this.answers.find(a => a.order === this.selectedAnswersProperty).status;
            }
        }
    });
</script>

<style lang="scss">
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

    .fa-smile {
        font-size: 40px;
        color: $nhsuk-blue;
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
</style>
