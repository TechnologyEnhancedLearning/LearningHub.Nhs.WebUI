<template>
    <Modal @cancel="$emit('cancel')">
        <div class="contribute-add-question-block-component">
            <h4 class="contribute-add-question-block-component-title">
                Choose a question block
            </h4>
            <div class="mt-10">
                <!--TODO 10988: get the correct icons-->
                <ChooseContentBlockOption block-type-name="Single Best Answer"
                                          block-description="Allow learners to choose one option from a list of possibilities."
                                          img-src="/images/content-block-type-icons/text.svg"
                                          @choose="$emit('choose', QuestionTypeEnum.SingleChoice)"/>
                <ChooseContentBlockOption block-type-name="Consider the Possibilities"
                                          block-description="Allow learners to choose Yes or No in response to multiple questions."
                                          img-src="/images/content-block-type-icons/wholeslideimage.svg"
                                          @choose="$emit('choose', QuestionTypeEnum.MultipleChoice)"/>
                <ChooseContentBlockOption block-type-name="Select Pinned Areas on a Whole Slide Image"
                                          block-description="Allow learners to answer a question by selecting one or more pinned areas within a whole slide image."
                                          img-src="/images/question-block-type-icons/imagezone.svg"
                                          @choose="$emit('choose', QuestionTypeEnum.ImageZone)"/>
                <ChooseContentBlockOption block-type-name="Match game"
                                          block-description="Allow learners to match answers together."
                                          img-src="/images/match-game.svg"
                                          @choose="$emit('choose', QuestionTypeEnum.MatchGame)"/>
            </div>
            <div class="d-flex justify-content-end mt-10">
                <a href="#"
                   @click.prevent="$emit('cancel')"
                   class="accessible-link">
                    Cancel
                </a>
            </div>
        </div>
    </Modal>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Button from '../globalcomponents/Button.vue';
    import ChooseContentBlockOption from "./components/content-tab/ChooseContentBlockOption.vue";
    import Modal from '../globalcomponents/Modal.vue';
    import { QuestionTypeEnum } from '../models/contribute-resource/blocks/questions/questionTypeEnum';

    export default Vue.extend({
        components: {
            Button,
            ChooseContentBlockOption,
            Modal,
        },
        data() {
            return {
                QuestionTypeEnum
            };
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-add-question-block-component {
        display: flex;
        flex-direction: column;
        overflow-y: auto; // Add scrollbars if needed

        .contribute-add-question-block-component-title {
            font-size: 3.2rem;
            font-weight: bold;
            text-align: center;
        }
    }
</style>