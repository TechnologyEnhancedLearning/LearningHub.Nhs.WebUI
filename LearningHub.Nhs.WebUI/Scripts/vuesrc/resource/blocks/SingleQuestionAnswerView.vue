<template>
    <div>
        <div v-for="(answer) in answersInAlphabeticalOrder"
             :key="answer.order">
            <label class="answer d-flex">
                <input :disabled="isSubmitted"
                       class="radio-button mr-3"
                       :class="{
                            'hidden-radio-buttons': showAnswers && bestSubmitted(),
                       }"
                       type="radio"
                       :value="answer.order"
                       v-model="selectedAnswersProperty[0]"
                       @click="$emit('update:selectedAnswersProperty', [answer.order])"/>
                <i v-if="feedbackVisible && answer.status !== AnswerTypeEnum.Best"
                   class="icons mr-3 my-12 align-self-center"
                   :class="getStyleFromAnswerType(answer.status)"/>
                <img v-else-if="feedbackVisible"
                     class="answer-medal-icon align-self-center mr-2"
                     src="/images/medal-icon.svg"
                     alt="Medal Icon"/>
                {{ answer.blockCollection.blocks[0].textBlock.content }}
            </label>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import _ from 'lodash';
    import { AnswerModel } from '../../models/contribute-resource/blocks/questions/answerModel';
    import BlocksView from './BlocksView.vue';
    import { AnswerTypeEnum } from '../../models/contribute-resource/blocks/questions/answerTypeEnum';

    export default Vue.extend({
        props: {
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            selectedAnswersProperty: { type: Array } as PropOptions<number[]>,
            isSubmitted: { type: Boolean } as PropOptions<boolean>,
            isRevealed: { type: Boolean } as PropOptions<boolean>,
            showAnswers: { type: Boolean, default: false } as PropOptions<boolean>,
            completed: { type: Boolean, default: false } as PropOptions<boolean>,
        },
        beforeCreate() {
            this.$options.components.BlocksView = BlocksView;
        },
        data() {
            return {
                AnswerTypeEnum: AnswerTypeEnum,
                isBest: false,
            };
        },
        watch: {
            selectedAnswersProperty: {
                deep: true,
                handler() {
                    this.isBest = this.answers.find(a => a.order === this.selectedAnswersProperty[0]).status === AnswerTypeEnum.Best;
                }
            },
        },
        methods: {
            getStyleFromAnswerType(answerType: number) {
                switch (answerType) {
                    case AnswerTypeEnum.Incorrect:
                        return "fas fa-times-circle";
                    case AnswerTypeEnum.Reasonable:
                        return "fa-solid fa-circle-check blue-circle";
                    default:
                        return "";
                }
            },

            bestSubmitted() {
                return this.isRevealed || this.isBest && this.isSubmitted;
            },
        },
        computed: {
            feedbackVisible(): boolean {
                return (this.showAnswers && this.bestSubmitted()) || this.completed;
            },
            answersInAlphabeticalOrder(): AnswerModel[] {
                return _.orderBy(this.answers, 'blockCollection.blocks[0].textBlock.content', 'asc');
            }
        }
    });
</script>
<style lang="scss">
    @use '../../../../Styles/abstracts/all' as *;

    .answer {
        padding: 8px 0;
        font-family: $font-stack;
        word-break: break-word;
        min-height: 63px;
        align-items: center;
    }

    .radio-button {
        width: 23px;
        height: 23px;
        filter: grayscale(1);
        vertical-align: middle;
    }

    .hidden-radio-buttons {
        display: none;
    }

    [type=radio] + i {
        cursor: default;
    }

    .answer-medal-icon {
        width: 32px;
        height: 32px;
        margin-left: -5px;
    }

    .fa-times-circle {
        color: $nhsuk-red;
    }

    .blue-circle {
        color: $nhsuk-blue;
    }

    .icons {
        font-size: 23px;
        vertical-align: middle;
    }
</style>
