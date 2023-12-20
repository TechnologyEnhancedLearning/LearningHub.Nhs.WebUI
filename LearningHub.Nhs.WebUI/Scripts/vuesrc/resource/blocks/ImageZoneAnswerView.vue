<template>
    <div>
        <WholeSlideImageBlock v-if="blocks[1].blockType === BlockTypeEnum.WholeSlideImage"
                              class="my-5"
                              :block="blocks[1]"
                              :disabled="isSubmitted"
                              :feedbackVisible="feedbackVisible"
                              :imageZone="true"
                              :answers="answers"
                              :selectedAnswersProperty="selectedAnswersProperty"
                              @updateSelectedAnswersProperty="updateSelectedAnswersProperty"/>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { AnswerModel } from '../../models/contribute-resource/blocks/questions/answerModel';
    import BlocksView from './BlocksView.vue';
    import { AnswerTypeEnum } from '../../models/contribute-resource/blocks/questions/answerTypeEnum';
    import { BlockModel } from "../../models/contribute-resource/blocks/blockModel";
    import { BlockTypeEnum } from "../../models/contribute-resource/blocks/blockTypeEnum";
    import WholeSlideImageBlock from "./WholeSlideImageBlock.vue";

    export default Vue.extend({
        props: {
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            isSubmitted: { type: Boolean } as PropOptions<boolean>,
            isRevealed: { type: Boolean } as PropOptions<boolean>,
            showAnswers: { type: Boolean, default: false } as PropOptions<boolean>,
            completed: { type: Boolean, default: false } as PropOptions<boolean>,
            blocks: { type: Array } as PropOptions<BlockModel[]>,
            selectedAnswersProperty: { type: Array } as PropOptions<number[]>,
        },
        beforeCreate() {
            this.$options.components.BlocksView = BlocksView;
        },
        data() {
            return {
                BlockTypeEnum,
                isBest: false,
            };
        },
        components: {
            WholeSlideImageBlock
        },
        watch: {
            selectedAnswersProperty: {
                deep: true,
                handler() {
                    this.isBest = this.answers.find(a => a.order === this.selectedAnswersProperty[0])?.status === AnswerTypeEnum.Best;
                }
            },
        },
        methods: {
            bestSubmitted() {
                return this.isRevealed || this.isBest && this.isSubmitted;
            },

            updateSelectedAnswersProperty(selectedAnswersProperty: number[]) {
                this.$emit("update:selectedAnswersProperty", selectedAnswersProperty);
            }
        },
        computed: {
            feedbackVisible(): boolean {
                return (this.showAnswers && this.bestSubmitted()) || this.completed;
            },
        }
    });
</script>
