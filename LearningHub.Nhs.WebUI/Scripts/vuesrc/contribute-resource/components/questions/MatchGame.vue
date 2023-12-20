<template>
    <div>
        <div class="d-flex flex-row flex-wrap">
            <div class="col-8 p-0">
                Add between 2 and 10 pairs
                <Tick :complete="questionBlock.twoAnswers()" class="pr-3 align-with-cc" />
            </div>
            <div class="col-8 p-0">
                Order will be randomised for the learner.
            </div>
            <div class="col-8 p-0">
                <div v-for="answer in questionBlock.answers">
                    <MatchGameAnswer :answer="answer"
                                     @deleteMatchAnswer="deleteMatchAnswer" />
                </div>
                <Button :disabled="questionBlock.matchGameSettings === undefined || questionBlock.isFull()" @click="createMatchAnswer">
                    + {{ addPairLabel }}
                </Button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import Button from "../../../globalcomponents/Button.vue";
    import Tick from "../../../globalcomponents/Tick.vue";
    import { QuestionBlockModel } from "../../../models/contribute-resource/blocks/questionBlockModel";
    import MatchGameAnswer from "./MatchGameAnswer.vue";
    import { AnswerModel } from "../../../models/contribute-resource/blocks/questions/answerModel";
    import { AnswerTypeEnum } from "../../../models/contribute-resource/blocks/questions/answerTypeEnum";

    export default Vue.extend({
        components: {
            Button,
            MatchGameAnswer,
            Tick
        },
        props: {
            questionBlock: { type: Object } as PropOptions<QuestionBlockModel>
        },
        computed: {
            addPairLabel() {
                const hasMatchingPairs = this.questionBlock.answers.length;
                return `Add ${hasMatchingPairs ? 'another' : ''} pair`;
            }
        },
        methods: {
            createMatchAnswer() {
                this.questionBlock.addAnswer(AnswerTypeEnum.Best);
            },
            deleteMatchAnswer(answer: AnswerModel) {
                this.questionBlock.deleteAnswer(answer);
            },
        }
    });
</script>
