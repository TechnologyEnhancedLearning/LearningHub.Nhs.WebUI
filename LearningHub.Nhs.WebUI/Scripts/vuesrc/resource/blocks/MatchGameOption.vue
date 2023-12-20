<template>
    <div class="col d-flex flex-column flex-wrap pl-0 pr-0"
         :class="{'first-column': isFirstBlock, 'second-column': !isFirstBlock}">
        <div class="match-option-container">
            <div v-if="matchQuestionType === MatchGameSettings.ImageToImage"
                 class="label">{{ this.matchAnswer.mediaBlock.image.description }}
            </div>
            <button class="match-game-answer-block"
                    :id="'button' + matchAnswer.id"
                    :disabled="matchQuestionType === MatchGameSettings.ImageToText"
                    :class="{
                        'matched-block': isMatched,
                        'selected-block': isSelected
                    }"
                    @click="selectAnswer">
                <TextToTextMatchGameOption v-if="matchQuestionType === MatchGameSettings.TextToText"
                                           :isFirstBlock="isFirstBlock"
                                           :matchAnswer="matchAnswer"/>
                <ImageToTextMatchGameOption v-if="matchQuestionType === MatchGameSettings.ImageToText"
                                            :isFirstBlock="isFirstBlock"
                                            :matchAnswer="matchAnswer"
                                            @openImageFullScreen="openImageFullScreen"
                                            @selectAnswer="selectAnswer"/>
                <ImageToImageMatchGameOption v-if="matchQuestionType === MatchGameSettings.ImageToImage"
                                             :isFirstBlock="isFirstBlock"
                                             :matchAnswer="matchAnswer"
                                             @openImageFullScreen="openImageFullScreen"/>
            </button>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { BlockModel } from "../../models/contribute-resource/blocks/blockModel";
    import Button from "../../globalcomponents/Button.vue";
    import ImagePublishedView from "../../contribute-resource/components/published-view/ImagePublishedView.vue";
    import TextToTextMatchGameOption from "./TextToTextMatchGameOption.vue";
    import { MatchGameSettings } from "../../models/contribute-resource/blocks/questions/matchGameSettings";
    import ImageToTextMatchGameOption from "./ImageToTextMatchGameOption.vue";
    import ImageToImageMatchGameOption from "./ImageToImageMatchGameOption.vue";
    
    export default Vue.extend({
        components: {
            TextToTextMatchGameOption,
            Button,
            ImagePublishedView,
            ImageToTextMatchGameOption,
            ImageToImageMatchGameOption
        },
        props: {
            matchAnswer: { type: Object } as PropOptions<BlockModel>,
            isSelected: Boolean,
            isMatched: Boolean,
            isFirstBlock: { type: Boolean, default: false },
            matchQuestionType: { type: Number } as PropOptions<MatchGameSettings>,
        },
        data() {
            return {
                MatchGameSettings
            }
        },
        methods: {
            selectAnswer() {
                if (!this.isMatched && !this.isSelected) {
                    this.$emit('selectMatchAnswer');
                }
            },
            openImageFullScreen(href: string) {
                this.$emit('showFullScreen', href);
            }
        },
    });
</script>

<style lang="scss">
    @use '../../../../Styles/abstracts/all' as *;
    
    .match-option-container {
        width: 100%;
    
        .main-border {
            stroke: $nhsuk-blue;
            stroke-width: 3;
        }
    
        .focus-border {
            stroke: $nhsuk-yellow;
            stroke-width: 3;
            display: none;
        }
    
        .label {
            padding-left: 2%;
            min-height: 33px;
        }
    }
    
    .match-game-answer-block {
        all: unset;
        cursor: pointer;
        color: black;
        text-decoration: none;
        position: relative;
        display: block;
        float: left;
        width: 100%;
    
        .focus-border {
            display: none;
        }
    
        svg {
            overflow: visible;
            width: 100%;
        }
    
        .main-border {
            fill: $nhsuk-white;
        }
    
    }
    
    .match-game-answer-block:focus,
    .selected-block {
        .focus-border {
            display: block;
        }
    
        .main-border {
            stroke: $nhsuk-grey;
        }
    
        text {
            font-weight: bold;
        }
    }
    
    .matched-block {
        pointer-events: none;
    
        &:focus {
            .focus-border {
                display: none;
            }
        }
    
        .main-border {
            fill: $nhsuk-blue;
        }
    
        text {
            fill: $nhsuk-white;
        }
    
        .fullscreen-button {
            pointer-events: visible;
            cursor: pointer;
        }
    }
    
    .fullscreen-button {
        height: 26px;
        width: 26px;
        position: absolute;
        z-index: 2;
        top: 41px;
        left: 17px;
        pointer-events: visible;
        cursor: pointer;
    }
    
    .second-column .matched-block {
        margin-left: -10.5%;
    }
</style>