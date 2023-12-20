<template>
    <ExpansionPanel class="contribute-match-answer-block" :value="0">
        <v-expansion-panel-content expand-icon="" v-model="isOpen" readonly value=true>
            <template v-slot:header>
                <div class="match-answer-header d-flex align-items-center">
                    <div class="icon-button">
                        <IconButton :iconClasses="isOpen ? `fas fa-minus-circle blue` : `fas fa-plus-circle blue`"
                                    :ariaLabel="isOpen ? `hide content` : `reveal content`"
                                    @click="isOpen = !isOpen"/>
                    </div>
                    <div class="align-left d-flex align-items-center">
                        Answer pair {{ answer.order + 1 }} out of a max of 10
                        <Tick :complete="answer.isReadyToPublish()" class="pl-3 align-with-cc"/>
                    </div>
                    <div class="match-answer-button">
                        <IconButton iconClasses="fa-solid fa-trash-can-alt"
                                    ariaLabel="Delete section"
                                    class="match-answer-button icon-button"
                                    @click="$emit('deleteMatchAnswer', answer)"/>
                    </div>
                </div>
            </template>

            <v-card class="border-top">
                <div class="match-answer-options">
                    <MatchGameAnswerOption :block="firstBlock"
                                           :pairOrder="pairOrder"
                                           isFirstBlock
                                           @addMediaBlock="addMediaBlock"/>
                    <MatchGameAnswerOption :block="secondBlock"
                                           :pairOrder="pairOrder"
                                           @addMediaBlock="addMediaBlock"/>
                </div>
            </v-card>
        </v-expansion-panel-content>
    </ExpansionPanel>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import Button from "../../../globalcomponents/Button.vue";
    import { AnswerModel } from "../../../models/contribute-resource/blocks/questions/answerModel";
    import ExpansionPanel from "../../../globalcomponents/ExpansionPanel.vue";
    import IconButton from "../../../globalcomponents/IconButton.vue";
    import Tick from "../../../globalcomponents/Tick.vue";
    import MatchGameAnswerOption from "./MatchGameAnswerOption.vue";
    import { MediaTypeEnum } from "../../../models/contribute-resource/blocks/mediaTypeEnum";
    import { BlockModel } from "../../../models/contribute-resource/blocks/blockModel";
    
    export default Vue.extend({
        components: {
            MatchGameAnswerOption,
            Tick,
            IconButton,
            ExpansionPanel,
            Button,
        },
        props: {
            answer: { type: Object } as PropOptions<AnswerModel>
        },
        data() {
            return {
                isOpen: true
            }
        },
        created() {
            this.isOpen = true;
        },
        computed: {
            firstBlock(): BlockModel {
                return this.answer.blockCollection.blocks[0];
            },
            secondBlock(): BlockModel {
                return this.answer.blockCollection.blocks[1];
            },
            pairOrder(): number {
                return this.answer.order + 1;
            }
        },
        methods: {
            addMediaBlock(fileId: number, mediaType: MediaTypeEnum) {
                this.answer.blockCollection.addMediaBlock(fileId, mediaType);
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .contribute-match-answer-block {
        border-radius: 4px;
        border: 1px solid $nhsuk-grey;
        background-color: $nhsuk-grey-white;
        margin: 20px;
    }

    .match-answer-header {
        min-height: 50px;
    }
    
    .match-answer-options {
        padding: 20px;
        display: flex;
        flex-wrap: wrap;
        justify-content: space-evenly;
    }

    .icon-button {
        flex-basis: 45px;
        margin: 8px 2px;
        padding-right: 5px;
    }

    .match-answer-button {
        margin-left: auto;
    }
</style>