<template>
    <ExpansionPanel class="assessment-settings-expansion-panel-wrapper" :value="firstTimeOpen ? 0 : undefined"> 
        <v-expansion-panel-content expand-icon="" v-model="isOpen">
            <template v-slot:header>
                <div class="lh-padding-fluid">
                    <div class="lh-container-xl d-flex align-items-center">
                        <div class="pr-4">
                            <IconButton :iconClasses="isOpen ? `fas fa-minus-circle` : `fas fa-plus-circle`" shape="circle" color="blue" size="large"/>
                        </div>
                        <div class="assessment-settings-title">{{assessmentDetails.assessmentType === AssessmentTypeEnum.Informal ? "Inf" : 'F'}}ormal assessment settings</div>
                        <div>This assessment has {{ questionsCompleted }} completed question{{ questionsCompleted === 1 ? '' : 's' }}</div>
                        <div class="pl-3">
                            <i class="assessment-settings-tick fas fa-adjust warm-yellow" v-if="questionsCompleted === 0"/>
                            <i class="assessment-settings-tick fa-solid fa-circle-check green" v-else/>
                        </div>
                    </div>
                </div>
            </template>
            <v-card class="border-top">
                <div class="py-5 lh-padding-fluid">
                    <div class="lh-container-xl">
                        <div v-if="firstTimeOpen" class="starter-text">Before you start creating content, check these assessment settings to make sure they offer the correct experience for learners. These can be changed at any time.</div>
                        <div class="d-flex">
                            <div class="selection pr-50">
                                <div class="mb-4">Allow the learner to work through the assessment pages in any order.</div>
                                <label class="my-0 label-text">
                                    <input class="radio-button" type="radio" :value="false"  v-model="assessmentDetails.answerInOrder"/>
                                    Yes
                                </label>
                                <label class="my-0 pl-5 label-text">
                                    <input class="radio-button" type="radio" :value="true"  v-model="assessmentDetails.answerInOrder"/>
                                    No
                                </label>
                            </div>
                            <div class="tip">
                                <h3>Tip</h3>
                                If you select ‘No’ the questions and pages will be unlocked after the learner has completed each page in sequence.
                            </div>
                        </div>
                        <hr class="my-30">

                        <div class="d-flex">
                            <div class="selection pr-50">
                                <div class="mb-4">Add a pass mark for this assessment. This is the percentage that the learner must achieve in order to pass.</div>
                                <input class="text-input" type="text" v-model="assessmentDetails.passMark"/>
                            </div>
                            <div class="tip" v-if="assessmentDetails.assessmentType === AssessmentTypeEnum.Informal">
                                <h3>Tip</h3>
                                Leave this blank if this assessment does not require a pass mark.
                            </div>
                        </div>
                        <hr class="my-30">

                        <div class="d-flex" v-if="assessmentDetails.assessmentType === AssessmentTypeEnum.Formal">
                            <div class="selection pr-50">
                                <div class="mb-4">Define how many attempts the learner can have.</div>
                                <input class="text-input" type="text" v-model="assessmentDetails.maximumAttempts" />
                            </div>
                            <div class="tip">
                                <h3>Tip</h3>
                                Leave this blank if the learner can have unlimited attempts at this assessment.
                            </div>
                        </div>
                        <hr class="my-30" v-if="assessmentDetails.assessmentType === AssessmentTypeEnum.Formal">

                        <div class="d-flex">
                            <div class="selection pr-50">
                                <div>Provide guidance for the learner at the end of this assessment.</div>
                                <EditSaveFieldWithCharacterCount   
                                                                    v-model="assessmentDetails.endGuidance.blocks[0].title"
                                                                    addEditLabel="title"
                                                                    v-bind:characterLimit="60"
                                                                    v-bind:isH3="true" />
                                <ckeditorwithhint v-on:blur="setEndGuidance" 
                                                  v-on:inputValidity="setGuidanceValidity"
                                                  :maxLength="1000" 
                                                  :initialValue="endGuidance" />
                            </div>
                            <div class="tip">
                                <h3>Tip</h3>
                                You can offer guidance to the learner at the end of the assessment such as next steps or recommendations on other learning resources to try.                            </div>
                        </div>
                        <Button class="mt-5" color="green" v-on:click="isOpen = false" :disabled="!canSaveAll">Save settings</Button>
                    </div>
                </div>
            </v-card>
        </v-expansion-panel-content>
    </ExpansionPanel>
</template>


<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { BlockTypeEnum } from '../models/contribute-resource/blocks/blockTypeEnum';
    import { AssessmentTypeEnum } from "../models/contribute-resource/blocks/assessments/assessmentTypeEnum";
    import { BlockCollectionModel } from '../models/contribute-resource/blocks/blockCollectionModel';
    import { ResourceType } from '../constants';
    import ExpansionPanel from '../globalcomponents/ExpansionPanel.vue';
    import IconButton from "../globalcomponents/IconButton.vue";
    import Button from "../globalcomponents/Button.vue";
    import Tick from "../globalcomponents/Tick.vue";
    import EditSaveFieldWithCharacterCount from "../globalcomponents/EditSaveFieldWithCharacterCount.vue";
    import { ContributeInjection } from './interfaces/injections';
    import CKEditorWithHint from '../ckeditorwithhint.vue';

    export default (Vue as Vue.VueConstructor<Vue & ContributeInjection>).extend({
        inject: ['resourceType', 'assessmentDetails'],
        props: {
            blockCollection: { type: Object } as PropOptions<BlockCollectionModel>,
            firstTimeOpen: Boolean,
        },
        components: {
            ExpansionPanel,
            IconButton,
            Tick,
            EditSaveFieldWithCharacterCount,
            ckeditorwithhint: CKEditorWithHint,
            Button,
        },
        created() {
                if (this.assessmentDetails.endGuidance.blocks.length === 0)
                {
                    this.assessmentDetails.endGuidance.addBlock(BlockTypeEnum.Text);
                }
                this.endGuidance = this.assessmentDetails.endGuidance.blocks[0].textBlock.content;
        },
        data() {
            return {
                AssessmentTypeEnum,
                isOpen: false,
                ResourceType,
                endGuidance: "",
                initialGuidance: "",
                guidanceValid: true,
            }
        },
        watch: {
            endGuidance() {
                if (this.assessmentDetails.endGuidance.blocks.length === 0)
                {
                    this.assessmentDetails.endGuidance.addBlock(BlockTypeEnum.Text);
                }
                this.assessmentDetails.endGuidance.blocks[0].textBlock.content = this.endGuidance;
            },
            ["assessmentDetails.passMark"](value){ this.assessmentDetails.passMark = this.capNumberFieldBy(value, 100)},
            ["assessmentDetails.maximumAttempts"](value){ this.assessmentDetails.maximumAttempts = this.capNumberFieldBy(value, 10)},
        },
        computed: {
            questionsCompleted(): number {
                return this.blockCollection.blocks.filter(block => block.blockType === BlockTypeEnum.Question && block.isReadyToPublish()).length;
            },
            canSaveAll(): boolean {
                let settingsAreValid = this.guidanceValid;
                
                if (this.assessmentDetails.assessmentType === AssessmentTypeEnum.Formal){
                    settingsAreValid = this.guidanceValid && !!this.assessmentDetails.passMark;
                } 
                
                this.assessmentDetails.assessmentSettingsAreValid = settingsAreValid;
                return settingsAreValid;
            },
        },
        methods: {
            capNumberFieldBy(fieldValue: string, max: number) {
                const result = fieldValue.toString().replace(/[^0-9]/g, '');
                return result === '' ? null : Math.min(max, parseInt(result));
            },
            setEndGuidance(description: string, valid: boolean) {
                if (valid)
                {
                    this.endGuidance = description;
                }
            },
            setGuidanceValidity(valid: boolean) {
                this.guidanceValid = valid;
            }
        }
    });
</script>

<style lang="scss">
    @use '../../../Styles/abstracts/all' as *;

    .assessment-settings-expansion-panel-wrapper {
        .v-expansion-panel__header {
            background-color: $nhsuk-grey;
            color: $nhsuk-white;
            padding-left: 0px;
            padding-right: 0px;
        }

        .v-expansion-panel__container:first-child {
            border-top: solid 1px $nhsuk-black-hover !important;
        }

        .icon-button {
            flex-basis: 40px;
            justify-self: center;
        }
        .cke {
            border: 2px solid $nhsuk-grey;
        }
    }

</style>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .assessment-settings-title {
        margin-right: auto;
    }
    .assessment-settings-tick {
        background-color: $nhsuk-white;
        border-radius: 50%;
    }
    .warm-yellow {
        color: $nhsuk-warm-yellow;
    }
    .green {
        color: $nhsuk-green;
    }
    .selection {
        flex: 67%;
    }
    .tip {
        flex: 33%;
        color: $nhsuk-grey;
    }
    .text-input {
        width: 200px;
        border-width: 2px;
        padding-left: 7px;
    }
    .guidance-box {
        border: 1px solid $nhsuk-grey;
        border-radius: 4px;
    }
    .starter-text {
        padding: 20px;
        margin: 20px 0px;
        background: rgba($nhsuk-green,0.1);
        border: 1px solid $nhsuk-green;
    }
    .label-text {
        font-weight: 100;
        font-family: $font-stack;
    }

</style>