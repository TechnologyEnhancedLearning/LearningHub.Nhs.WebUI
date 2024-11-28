<template>
    <ExpansionPanel class="elevation-0 mb-0 my-20 contribute-block"
                    :value="0"
                    :class="{'copy-contribute-block': canBeDuplicated}">
        <v-expansion-panel-content expand-icon=""
                                   v-model="isOpen"
                                   readonly
                                   value=true>
            <template v-slot:header>
                <div class="contribute-block-component-header d-flex align-center align-items-center">
                    <label v-if="canBeDuplicated"
                           class="checkContainer mb-0">
                        <span class="my-0 ml-2">{{ block.title }}</span>
                        <input :checked="selectedToDuplicate"
                               type="checkbox"
                               @change="$emit('duplicateBlock')">
                        <span class="checkmark"></span>
                    </label>
                    <div class="icon-button"
                         v-if="!canBeDuplicated">
                        <IconButton :iconClasses="isOpen ? `fas fa-minus-circle blue` : `fas fa-plus-circle blue`"
                                    :ariaLabel="isOpen ? `hide content` : `reveal content`"
                                    @click="isOpen = !isOpen"></IconButton>
                    </div>
                    <div class="text-field align-left d-flex align-items-center"
                         v-if="!canBeDuplicated">
                        <EditSaveFieldWithCharacterCount v-if="block.blockType !== BlockTypeEnum.Question && !canBeDuplicated"
                                                         v-model="block.title"
                                                         addEditLabel="title"
                                                         :characterLimit="60"
                                                         :isH3="true"
                                                         :inputId="title"></EditSaveFieldWithCharacterCount>
                        <h3 class="my-0"
                            v-else>
                            {{ block.title }}
                        </h3>
                        <Tick :complete="block.isReadyToPublish()"
                              class="pl-10"></Tick>
                    </div>
                    <div class="d-flex justify-content-center icon-button"
                         v-if="!canBeDuplicated">
                        <IconButton v-if="enableUp"
                                    @click="$emit('up')"
                                    iconClasses="fa-solid fa-arrow-up"
                                    ariaLabel="Move section up"
                                    class="contribute-block-component-button"></IconButton>
                    </div>
                    <div class="d-flex justify-content-center icon-button"
                         v-if="!canBeDuplicated">
                        <IconButton v-if="enableDown"
                                    @click="$emit('down')"
                                    iconClasses="fa-solid fa-arrow-down"
                                    ariaLabel="Move section down"
                                    class="contribute-block-component-button"></IconButton>
                    </div>
                    <IconButton v-if="!canBeDuplicated && !contributeResourceAVFlag && block.blockType === BlockTypeEnum.Media"></IconButton>
                    <IconButton v-else
                                @click="duplicateBlock"
                                iconClasses="fa-regular fa-clone"
                                ariaLabel="Duplicate section"
                                class="contribute-block-component-button icon-button"></IconButton>
                    <IconButton v-if="!canBeDuplicated"
                                @click="discardBlockModalOpen = true"
                                iconClasses="fa-solid fa-trash-can-alt"
                                ariaLabel="Delete section"
                                class="contribute-block-component-button icon-button"></IconButton>
                    <Modal v-if="discardBlockModalOpen"
                           @cancel="discardBlockModalOpen = false">
                        <template v-slot:title>
                            <WarningTriangle color="yellow"></WarningTriangle>
                            Delete section
                        </template>
                        <template v-slot:body>
                            This cannot be undone.
                            Do you want to continue?
                        </template>
                        <template v-slot:buttons>
                            <Button @click="discardBlockModalOpen = false"
                                    class="mx-12 my-2">
                                Cancel
                            </Button>
                            <Button color="red"
                                    @click="$emit('delete'); discardBlockModalOpen = false;"
                                    class="mx-12 my-2">
                                Delete section
                            </Button>
                        </template>
                    </Modal>
                </div>
            </template>

            <v-card class="border-top">
                <ContributeTextBlock v-if="block.blockType === BlockTypeEnum.Text"
                                     :textBlock="block.textBlock"
                                     :order="block.order"/>
                <ContributeWholeSlideImageBlock v-if="block.blockType === BlockTypeEnum.WholeSlideImage"
                                                :wholeSlideImageBlock="block.wholeSlideImageBlock"
                                                :resourceType="resourceType"
                                                @annotateWholeSlideImage="showSlideWithAnnotations"/>
                <ContributeMediaBlock v-if="block.blockType === BlockTypeEnum.Media"
                                      :mediaBlock="block.mediaBlock"/>
                <ContributeQuestionBlock v-if="block.blockType === BlockTypeEnum.Question"
                                         :questionBlock="block.questionBlock"
                                         :blockRef="block.blockRef"
                                         @annotateWholeSlideImage="showSlideWithAnnotations"/>
                <ContributeImageCarouselBlock v-if="block.blockType === BlockTypeEnum.ImageCarousel"
                                              :imageCarouselBlock="block.imageCarouselBlock"
                                              :resourceType="resourceType"/>
            </v-card>
        </v-expansion-panel-content>

    </ExpansionPanel>
</template>


<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import ContributeTextBlock from './ContributeTextBlock.vue';
    import ContributeWholeSlideImageBlock from './ContributeWholeSlideImageBlock.vue';
    import ContributeMediaBlock from "./components/content-tab/ContributeMediaBlock.vue";
    import ContributeQuestionBlock from "./ContributeQuestionBlock.vue";
    import Button from '../globalcomponents/Button.vue';
    import EditSaveFieldWithCharacterCount from '../globalcomponents/EditSaveFieldWithCharacterCount.vue';
    import IconButton from '../globalcomponents/IconButton.vue';
    import LinkTextAndIcon from '../globalcomponents/LinkTextAndIcon.vue';
    import Modal from '../globalcomponents/Modal.vue';
    import Tick from '../globalcomponents/Tick.vue';
    import WarningTriangle from '../globalcomponents/WarningTriangle.vue';
    import ExpansionPanel from '../globalcomponents/ExpansionPanel.vue';
    import { BlockTypeEnum } from '../models/contribute-resource/blocks/blockTypeEnum';
    import { BlockModel } from '../models/contribute-resource/blocks/blockModel';
    import { WholeSlideImageModel } from "../models/contribute-resource/blocks/wholeSlideImageModel";
    import { ResourceType } from "../constants";
    import ClickEvent = JQuery.ClickEvent;
    import ContributeImageCarouselBlock from "./ContributeImageCarouselBlock.vue";
    import { QuestionBlockModel } from "../models/contribute-resource/blocks/questionBlockModel";
    import { EventBus } from './contributeResourceEvents';
    import { resourceData } from '../data/resource';

    export default Vue.extend({
        components: {
            ContributeImageCarouselBlock,
            ContributeTextBlock,
            ContributeWholeSlideImageBlock,
            ContributeMediaBlock,
            ContributeQuestionBlock,
            Button,
            EditSaveFieldWithCharacterCount,
            IconButton,
            LinkTextAndIcon,
            Modal,
            Tick,
            WarningTriangle,
            ExpansionPanel,
        },
        props: {
            block: { type: Object } as PropOptions<BlockModel>,
            enableUp: Boolean,
            enableDown: Boolean,
            resourceType: { type: Number } as PropOptions<ResourceType>,
            canBeDuplicated: Boolean,
            selectedToDuplicate: Boolean,
            title: { type: String, default: 'title' },
        },
        data() {
            return {
                editing: false,
                discardBlockModalOpen: false,
                BlockTypeEnum: BlockTypeEnum,
                isOpen: true,
                contributeResourceAVFlag: true
            };
        },
        created() {
            this.isOpen = true;
            this.getContributeResAVResourceFlag();
        },
        watch: {
            isOpen(newVal, oldVal) {
                EventBus.$emit('ContributeBlock.Expansion.Event', newVal);
            }
        },
        methods: {
            showSlideWithAnnotations(wholeSlideImageToShow: WholeSlideImageModel, questionBlock: QuestionBlockModel) {
                this.$emit('annotateWholeSlideImage', wholeSlideImageToShow, questionBlock);
            },
            duplicateBlock(event: ClickEvent) {
                event.target.blur();
                event.target.parentElement.blur();
                this.$emit('duplicate');
            },
            getContributeResAVResourceFlag() {
                resourceData.getContributeAVResourceFlag().then(response => {
                    this.contributeResourceAVFlag = response;
                });
            }
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-block {
        background-color: $nhsuk-white;
        border: 1px solid $nhsuk-grey-lighter;
    }

    .copy-contribute-block {
        border: 2px solid #FFB81C;
    }

    .contribute-block-component {
        background-color: $nhsuk-white;
        border: 1px solid $nhsuk-grey-lighter;
    }

    .contribute-block-component-header {
        min-height: 58px;

    }

    .contribute-block-component .contribute-block-component-header {
        border-bottom: 1px solid $nhsuk-grey-light;
        min-height: 60px;
    }

    .contribute-block-component .contribute-block-component-buttons {
        padding-right: 5px;
    }

    .contribute-block-component .contribute-block-component-button {
        margin: 8px 2px;
    }

    .block-title {
        margin: 19px;
    }

    .icon-button {
        flex-basis: 45px;
    }

    .tick {
        flex-basis: 30px;
    }

    .text-field {
        flex: 66%;
        word-break: break-word;
    }

</style>