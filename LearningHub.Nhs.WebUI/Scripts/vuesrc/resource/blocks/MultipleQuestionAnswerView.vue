<template>
    <div>
        <div v-for="(answer) in answersInAlphabeticalOrder"
             :key="answer.order"
             class="answer-div">
            <div class="mt-15 answer">{{ answer.blockCollection.blocks[0].textBlock.content }}</div>
            <i class="icons"
               :class="showAnswers ? getStyleFromAnswerType(answer, 1) : ''"/>
            <label class="answer">
                <input class="radio-button mr-2"
                       type="radio"
                       :class='{
                            "hidden": showAnswers && getStyleFromAnswerType(answer, 1) !== ""
                       }'
                       :value="AnswerTypeEnum.Reasonable"
                       :disabled="isSubmitted"
                       v-model="selectedAnswersProperty[answer.order]"
                /> Yes
            </label>
            <span class="ml-50"></span>
            <i class="icons"
               :class="showAnswers ? getStyleFromAnswerType(answer, 0) : ''"/>
            <label class="answer">
                <input class="radio-button mr-2"
                       type="radio"
                       :class='{
                            "hidden": showAnswers && getStyleFromAnswerType(answer, 0) !== ""
                       }'
                       :value="AnswerTypeEnum.Incorrect"
                       :disabled="isSubmitted"
                       v-model="selectedAnswersProperty[answer.order]"
                /> No
            </label>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";
    import BlocksView from "./BlocksView.vue";
    import _ from "lodash";
    import { AnswerTypeEnum } from "../../models/contribute-resource/blocks/questions/answerTypeEnum";

    export default Vue.extend({
        props: {
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            selectedAnswersProperty: { type: Array } as PropOptions<number[]>,
            isSubmitted: { type: Boolean } as PropOptions<boolean>,
            showAnswers: { type: Boolean, default: false } as PropOptions<boolean>,
        },
        beforeCreate() {
            this.$options.components.BlocksView = BlocksView;
        },
        data() {
            return {
                AnswerTypeEnum: AnswerTypeEnum,
            };
        },
        computed: {
            answersInAlphabeticalOrder(): AnswerModel[] {
                return _.orderBy(this.answers, 'blockCollection.blocks[0].textBlock.content', 'asc');
            }
        },
        methods: {
            getStyleFromAnswerType(answer: AnswerModel, yesOrNo: number) {
                if (!this.isSubmitted) {
                    return "";
                } else {
                    const answerType = answer.status;
                    const selectedAnswer = this.selectedAnswersProperty[answer.order];

                    if (answerType === AnswerTypeEnum.Reasonable && selectedAnswer === 1
                        || answerType === AnswerTypeEnum.Incorrect && selectedAnswer === 0) {
                        if (yesOrNo === selectedAnswer) {
                            return "fa-solid fa-circle-check green-circle mr-2";
                        } else {
                            return "";
                        }
                    } else {
                        if (yesOrNo === selectedAnswer) {
                            return "fas fa-times-circle mr-2";
                        } else {
                            return "fa-solid fa-circle-check grey-circle mr-2";
                        }
                    }
                }
            },
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .answer {
        padding: 8px 0;
        font-family: $font-stack;
        word-break: break-word;
    }

    .fa-times-circle {
        color: $nhsuk-red;
    }

    .green-circle {
        color: $nhsuk-green;
    }

    .grey-circle {
        color: $nhsuk-grey;
    }

    .hidden {
        display: none;
    }

    .answer-div:not(:last-child) {
        border-bottom: 1px solid $nhsuk-grey-lighter;
    }
</style>