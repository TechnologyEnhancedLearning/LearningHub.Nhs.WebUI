<template>
    <div class="reveal-answer-block">
        <span class="reveal-answer-content">
            <div class="mb-3">
                Do you want learners to be able to reveal the answer without answering the question?
            </div>
            <form>
               
                <label class="my-0" for="no">
                    <input class="radio-button" type="radio" :value="false"  v-model="questionBlock.allowReveal" id="no"/>
                    No
                </label>
                <br>
                <label class="my-0" for="yes">
                    <input class="radio-button" type="radio" :value="true"  v-model="questionBlock.allowReveal" id="yes"/>
                    Yes
                </label>
            </form>
        </span>
    </div>
</template>


<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { QuestionBlockModel } from '../../../models/contribute-resource/blocks/questionBlockModel';

    export default Vue.extend({
        props:{
            questionBlock: {type: Object} as PropOptions<QuestionBlockModel>
        }
    });
</script>
<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;
    .reveal-answer-block {
        display: flex;
        font-size: 18px;
        flex-flow: row;
    }
    .reveal-answer-content {
        flex:50%;
    }

    .radio-button {
        width: 18px;
        height: 18px !important;
        filter: grayscale(1);
        vertical-align: middle;
    } 
</style>