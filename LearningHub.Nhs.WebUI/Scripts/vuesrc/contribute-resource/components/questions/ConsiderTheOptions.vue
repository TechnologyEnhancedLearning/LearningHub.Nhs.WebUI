<template>
    <div>
        <div class="d-flex flex-row flex-wrap">
            <div class="col-8 p-0">
                <h3>What kind of responses do you want to set?</h3>
            </div>
            <div class="col-8 p-0">
                <div v-for="answer in questionBlock.answers" :key="answer.blockCollection.blocks[0].blockRef">
                    <MultipleChoiceAnswer @deleteAnswer="deleteAnswer" v-bind:answer="answer"></MultipleChoiceAnswer>
                </div>
                <Button v-if="!questionBlock.isFull()" v-on:click="createAnswer()">+ Add {{ questionBlock.answers.length === 0 ? 'a' : 'another' }} response</Button>
            </div>
            <div class="col-4 p-0 pt-15 description-text">
                <h3>Tip!</h3>
                <p>
                    You must offer between two and twenty responses for your reader to consider. For each response you add, the reader will be asked to choose Yes or No to decide whether it is a valid response to the question you have set.
                </p>
                <p>
                    Text-based options will be ordered alphabetically when shown to the reader.
                </p>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { AnswerModel } from "../../../models/contribute-resource/blocks/questions/answerModel";
    import { AnswerTypeEnum } from "../../../models/contribute-resource/blocks/questions/answerTypeEnum";
    import MultipleChoiceAnswer from "./MultipleChoiceAnswer.vue";
    import Vuetify from "vuetify";
    import Tick from "../../../globalcomponents/Tick.vue";
    import Button from "../../../globalcomponents/Button.vue";
    import { QuestionBlockModel } from '../../../models/contribute-resource/blocks/questionBlockModel';

    Vue.use(Vuetify);

    export default Vue.extend({
        props: {
            questionBlock: { type: Object } as PropOptions<QuestionBlockModel>
        },
        components: {
            MultipleChoiceAnswer,
            Tick,
            Button
        },
        data() {
            return {
                answerTypeEnum: AnswerTypeEnum,
            };
        },
        methods: {
            createAnswer() {
                this.questionBlock.addAnswer();
            },
            deleteAnswer(answer: AnswerModel) {
                this.questionBlock.deleteAnswer(answer);
            }
        },

    });
</script>

<style lang="scss" scoped>
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
        padding: 15px
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