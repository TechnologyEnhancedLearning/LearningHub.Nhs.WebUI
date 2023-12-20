<template>
    <div>
        <div>
            Select the type of content for the learner to match.
        </div>
        <div>
            <div>
                <label class="answer">
                    <input class="radio-button mr-2"
                           type="radio"
                           :name="matchGameRadioGroupName"
                           :value="MatchGameSettings.TextToText"
                           :disabled="isReadOnly"
                           v-model="questionBlock.matchGameSettings"
                    /> Text to Text
                </label>
            </div>
            <div>
                <label class="answer">
                    <input class="radio-button mr-2"
                           type="radio"
                           :name="matchGameRadioGroupName"
                           :value="MatchGameSettings.ImageToText"
                           :disabled="isReadOnly"
                           v-model="questionBlock.matchGameSettings"
                    /> Image to Text
                </label>
            </div>
            <div>
                <label class="answer">
                    <input class="radio-button mr-2"
                           type="radio"
                           :name="matchGameRadioGroupName"
                           :value="MatchGameSettings.ImageToImage"
                           :disabled="isReadOnly"
                           v-model="questionBlock.matchGameSettings"
                    /> Image to Image
                </label>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from "vue";
    import Button from "../../../globalcomponents/Button.vue";
    import { QuestionBlockModel } from "../../../models/contribute-resource/blocks/questionBlockModel";
    import MatchGameAnswer from "./MatchGameAnswer.vue";
    import { MatchGameSettings } from "../../../models/contribute-resource/blocks/questions/matchGameSettings";
    
    export default Vue.extend({
        components: {
            Button,
            MatchGameAnswer,
        },
        props: {
            questionBlock: QuestionBlockModel,
            questionBlockRef: Number
        },
        data() {
            return {
                MatchGameSettings: MatchGameSettings
            }
        },
        computed: {
            isReadOnly(): boolean {
                return this.questionBlock.answers.length !== 0;
            },
            matchGameRadioGroupName(): string {
                return `match game type for question ${this.questionBlockRef}`;
            }
        },
    });
</script>
