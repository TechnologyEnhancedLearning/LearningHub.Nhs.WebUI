<template>
    <div>
        <div class="d-flex flex-row flex-wrap">
            <div class="col-8 p-0">
                <div class="tick-row my-20">
                    <div class="check-box align-self-center mr-3">
                        <Tick :complete="questionBlock.oneBestAnswer()"></Tick>
                    </div>
                    <div class="text-box mr-20">
                        There should be a best answer, but only one
                    </div>
                    <div class="check-box align-self-center mr-3">
                        <Tick :complete="questionBlock.twoAnswers()" />
                    </div>
                    <div class="text-box">
                        There should be at least two answers
                    </div>
                </div>
            </div>
            <div class="col-8 p-0">
                <div v-for="(answer, index) in questionBlock.answers"
                     :key="index">
                    <SingleChoiceAnswer :answer="answer"
                                        @deleteAnswer="deleteAnswer"></SingleChoiceAnswer>
                </div>
                <Button v-if="!questionBlock.isFull() && !creatingNew"
                        @click="creatingNew=true">
                    + Add {{ questionBlock.answers.length === 0 ? 'an' : 'another' }} answer
                </Button>
                <div v-if="creatingNew"
                     class="new-answer-block">
                    <div>
                        Choose an option
                        <hr class="cutoff-line">
                    </div>
                    <div class="new-answer-buttons">
                        <Button color="grey"
                                @click="createAnswer(answerTypeEnum.Reasonable)">
                            Reasonable answer
                        </Button>
                        or
                        <Button color="grey"
                                @click="createAnswer(answerTypeEnum.Incorrect)">
                            Incorrect answer
                        </Button>
                    </div>
                </div>
            </div>
            <div class="col-4 p-0 pt-15 description-text">
                <p>
                    You can add up to twenty answers here. These will be the options that
                    readers can choose from when answering your question.
                </p>
                <p>
                    You can add more than one Reasonable or Incorrect answer, but there
                    must be one Best answer for readers to aim for.
                </p>
                <p>
                    Text-based answers will be ordered alphabetically when shown to the
                    reader.
                </p>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { AnswerModel } from "../../../models/contribute-resource/blocks/questions/answerModel";
    import { AnswerTypeEnum } from "../../../models/contribute-resource/blocks/questions/answerTypeEnum";
    import SingleChoiceAnswer from "./SingleChoiceAnswer.vue";
    import Tick from "../../../globalcomponents/Tick.vue";
    import Button from "../../../globalcomponents/Button.vue";
    import { BlockCollectionModel } from '../../../models/contribute-resource/blocks/blockCollectionModel';
    import { BlockTypeEnum } from '../../../models/contribute-resource/blocks/blockTypeEnum';
    import { QuestionBlockModel } from '../../../models/contribute-resource/blocks/questionBlockModel';

    export default Vue.extend({
        props: {
            questionBlock: { type: Object } as PropOptions<QuestionBlockModel>
        },

        components: {
            SingleChoiceAnswer,
            Tick,
            Button
        },

        data() {
            return {
                answerTypeEnum: AnswerTypeEnum,
                creatingNew: false,
            };
        },

        created() {
            if (this.questionBlock.answers.length === 0) {
                let best = new AnswerModel();
                best.status = AnswerTypeEnum.Best;
                best.blockCollection = new BlockCollectionModel();
                best.blockCollection.addBlock(BlockTypeEnum.Text);
                this.questionBlock.answers.push(best);
            }

        },

        methods: {
            createAnswer(answerType: AnswerTypeEnum) {
                this.questionBlock.addAnswer(answerType);
                this.creatingNew = false;
            },
            deleteAnswer(answer: AnswerModel) {
                this.questionBlock.deleteAnswer(answer);
            }
        },
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .add-question-button {
        background-color: $nhsuk-white;
        border: 2px solid $nhsuk-blue;
        color: $nhsuk-blue;
        text-transform: none;
        border-radius: 10px;
    }

    .question-type-button {
        background-color: $nhsuk-white;
        border: 2px solid $nhsuk-grey;
        color: $nhsuk-grey;
        text-transform: none;
        border-radius: 10px;
    }

    .new-answer-block {
        font-size: 18px;
        border-radius: 4px;
        border: 1px solid $nhsuk-grey;
        background: $nhsuk-grey-white;
        margin: 20px 20px 20px 0px;
        padding: 10px 15px 15px 15px;
    }

    .new-answer-buttons {
        text-align: center;
    }

    .cutoff-line {
        border-top: 1px solid black;
        margin: 15px -15px 15px -15px
    }

    .tick-row {
        width: 70%;
        display: flex;
    }

    .text-box {
        color: $nhsuk-grey;
        flex: 95%;
        font-size: 14px;
        align-self: center;
    }

    .check-box {
        flex: 5%;
    }

    .collection-description {
        flex: 45%;
        font-size: 18px;
        padding: 15px 0px;
    }

    .answers-container {
        flex: 70%;
    }

    .description-text {
        color: $nhsuk-grey;
    }
</style>