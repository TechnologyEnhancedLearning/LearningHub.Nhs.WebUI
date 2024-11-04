<template>
    <div class="answer-block ">
        <div>
            <div class="flex align-items-center">
                <span v-if="answer.status === AnswerTypeEnum.Best">
                    <img src="/images/medal-icon.svg" alt="Gold Medal" class="answer-icon medal-icon"/>
                    Best Answer
                </span>
                <span v-else-if="answer.status === AnswerTypeEnum.Reasonable && multipleAnswers">
                    <i class="answer-icon fa-solid fa-circle-check green"></i>
                    Correct Answer
                </span>
                <span v-else-if="answer.status === AnswerTypeEnum.Reasonable">
                    <i class="answer-icon fa-solid fa-circle-check blue"></i>
                    Reasonable Answer
                </span>
                <span v-else-if="answer.status === AnswerTypeEnum.Incorrect">
                    <i class="answer-icon fas fa-times-circle red"></i>
                    Incorrect Answer
                </span>
                <span class="bin">
                    <IconButton v-if="answer.status !== AnswerTypeEnum.Best"
                        v-on:click="$emit('deleteAnswer', answer)"
                        iconClasses="fa-solid fa-trash-can-alt" />
                </span>    
            </div>
            <hr class="cutoff-line">
        </div>
        <div>
            <input type="text" aria-describedby="messageError" class="form-control text-input" maxlength="120" v-model="message" />
            <div class="footer-text" id="messageError">
                You have {{ charactersRemaining }} characters remaining.
            </div>
        </div>
    </div>
</template>

<script lang='ts'>
    import Vue, { PropOptions } from 'vue';
    import { AnswerModel } from "../../../models/contribute-resource/blocks/questions/answerModel";
    import { AnswerTypeEnum } from "../../../models/contribute-resource/blocks/questions/answerTypeEnum";
    import IconButton from "../../../globalcomponents/IconButton.vue";

    export default Vue.extend({
        components: {
          IconButton
        },
        props: {
            answer: { type: Object } as PropOptions<AnswerModel>,
            multipleAnswers: Boolean,
        },
        data() {
            return {
                message: this.answer.blockCollection.blocks[0].textBlock.content,
                charactersRemaining: 120 - this.answer.blockCollection.blocks[0].textBlock.content.length,
                AnswerTypeEnum: AnswerTypeEnum
            }
        },
        watch: {
            message() {
                this.answer.blockCollection.blocks[0].textBlock.content = this.message;
                this.charactersRemaining = 120 - this.message.length;
            },
            answer: {
                deep: true,
                handler() {
                    this.message = this.answer.blockCollection.blocks[0].textBlock.content;
                }
            }
        }

    })

</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .answer-block {
        font-size: 18px; 
        border-radius: 4px; 
        border: 1px solid $nhsuk-grey; 
        background: $nhsuk-grey-white; 
        margin: 20px 20px 20px 0px; 
        padding: 10px 15px 15px 15px;
    }

    .cutoff-line {
        border-top: 1px solid black; 
        margin: 10px -15px 15px -15px;
    }

    .flex {
        display: flex;
    }

    .bin {
        margin-left: auto;
    }
    
    .text-input {
        color: $nhsuk-black;
        border-width: 2px;
    }
    
    .footer-text {
        color: $nhsuk-grey;
    }

    .answer-icon {
        margin-right: 20px;
        font-size: 28px;
    }

    .blue {
        color: $nhsuk-blue;
    }

    .gold {
        color: $nhsuk-warm-yellow;
    }

    .green {
        color: $nhsuk-green;
    }

    .red {
        color: $nhsuk-red;
    }
    
    .medal-icon {
        width: 35px;
        height: 35px;
        margin-left: -5px;
    }
</style>