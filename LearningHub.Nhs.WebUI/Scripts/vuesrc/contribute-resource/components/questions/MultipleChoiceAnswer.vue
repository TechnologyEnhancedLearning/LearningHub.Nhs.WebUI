<template>
    <div class="answer-block ">
        <div>
            <div class="flex align-items-center">
                <div class="flex">
                    <Tick v-bind:complete="this.answer.isReadyToPublish()" class="pr-3 align-with-cc"/>
                    <EditSaveFieldWithCharacterCount
                                    v-model="message"
                                    addEditLabel="response text"
                                    v-bind:characterLimit="120"
                                    v-bind:isH3="true"></EditSaveFieldWithCharacterCount>
                </div>
                <span class="bin">
                    <IconButton v-on:click="$emit('deleteAnswer', answer)"
                                iconClasses="fa-solid fa-trash-can-alt" />
                </span>    
            </div>
            <hr class="cutoff-line">
        </div>
        <div class="align-items-center">
            <span>How should the reader answer?</span>
                <label class="my-0 pl-4 label-text">
                    <input class="radio-button" type="radio" :value="AnswerTypeEnum.Reasonable" v-model="status"/>
                    Yes
                </label>
                <label class="my-0 pl-4 label-text">
                    <input class="radio-button" type="radio" :value="AnswerTypeEnum.Incorrect" v-model="status"/>
                    No
                </label>
        </div>
    </div>
</template>

<script lang='ts'>
    import Vue, { PropOptions } from 'vue';
    import { AnswerModel } from "../../../models/contribute-resource/blocks/questions/answerModel";
    import { AnswerTypeEnum } from "../../../models/contribute-resource/blocks/questions/answerTypeEnum";
    import IconButton from "../../../globalcomponents/IconButton.vue";
    import EditSaveFieldWithCharacterCount from "../../../globalcomponents/EditSaveFieldWithCharacterCount.vue";
    import Tick from "../../../globalcomponents/Tick.vue";
    
    export default Vue.extend({
        components: {
          IconButton,
          EditSaveFieldWithCharacterCount,
          Tick,
        },
      
        props: {
            answer: { type: Object } as PropOptions<AnswerModel>,
            multipleAnswers: Boolean,
        },
        
        data() {
            return {
                message: this.answer.blockCollection.blocks[0].textBlock.content,
                charactersRemaining: 120 - this.answer.blockCollection.blocks[0].textBlock.content.length,
                AnswerTypeEnum: AnswerTypeEnum,
                status: this.answer.status
            }
        },
        
        watch: {
            message() {
                this.answer.blockCollection.blocks[0].textBlock.content = this.message;
                this.charactersRemaining = 120 - this.message.length;
            },
            status() {
                this.answer.status = this.status;
            },
            answer: {
                deep: true,
                handler() {
                    this.message = this.answer.blockCollection.blocks[0].textBlock.content;
                    this.status = this.answer.status;
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

    .radio-button {
        width: 18px;
        height: 18px !important;
        filter: grayscale(1);
        vertical-align: middle;
    } 

    .align-with-cc {
        padding-top: 21px;
    }

    .label-text {
        font-weight: 100;
        font-family: $font-stack;
    }
</style>