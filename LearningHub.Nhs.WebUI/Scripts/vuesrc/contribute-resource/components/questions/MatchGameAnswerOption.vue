<template>
    <div class="answer-option">
        <div class="d-flex align-items-center border-bottom">
            <div v-if="isFirstBlock && blockType === BlockTypeEnum.Text">
                Text option {{ this.pairOrder }}
            </div>
            <div v-else-if="isFirstBlock && blockType === BlockTypeEnum.Media">
                Image option {{ this.pairOrder }}
            </div>
            <div v-else-if="blockType === BlockTypeEnum.Media">
                Matching image {{ this.pairOrder }}
            </div>
            <div v-else>
                Matching text {{ this.pairOrder }}
            </div>
        </div>
        <div v-if="blockType === BlockTypeEnum.Text">
            <input type="text" class="form-control text-input" maxlength="40" v-model="answerOption"/>
            <div class="footer-text">
                You have {{ charactersRemaining }} characters remaining.
            </div>
        </div>
        <div v-else>
            <MediaBlockImage imageClass="match-game-image"
                             :image="image"
                             :restrictAdvancedImageTypes="true"
                             :withLabel="true"
                             :descriptionCharacterLimit="40"
                             @updatePublishingStatus="updatePublishingStatus"/>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from "vue";
    import Button from "../../../globalcomponents/Button.vue";
    import IconButton from "../../../globalcomponents/IconButton.vue";
    import Tick from "../../../globalcomponents/Tick.vue";
    import { BlockModel } from "../../../models/contribute-resource/blocks/blockModel";
    import { BlockTypeEnum } from "../../../models/contribute-resource/blocks/blockTypeEnum";
    import AddBlocksMenu from "./AddBlocksMenu.vue";
    import MediaBlockImage from "../content-tab/MediaBlockImage.vue";
    
    export default Vue.extend({
        components: {
            Tick,
            IconButton,
            Button,
            AddBlocksMenu,
            MediaBlockImage
        },
        props: {
            block: BlockModel,
            isFirstBlock: { type: Boolean, default: false },
            pairOrder: Number
        },
        data() {
            return {
                answerOption: this.block?.textBlock?.content,
                charactersRemaining: 40 - this.block?.textBlock?.content?.length,
                image: this.block?.mediaBlock?.image,
                BlockTypeEnum: BlockTypeEnum
            }
        },
        computed: {
            blockType(): BlockTypeEnum {
                return this.block?.blockType || BlockTypeEnum.Media;
            }
        },
        watch: {
            answerOption() {
                if (this.block.blockType === BlockTypeEnum.Text) {
                    this.block.textBlock.content = this.answerOption;
                    this.charactersRemaining = 40 - this.answerOption?.length;
                }
            },
            block: {
                deep: true,
                handler() {
                    this.answerOption = this.block?.textBlock?.content;
                    this.charactersRemaining = 40 - this.answerOption?.length;
                    this.image = this.block?.mediaBlock?.image;
                }
            }
        },
        methods: {
            updatePublishingStatus() {
                this.block.mediaBlock.updatePublishingStatus();
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .answer-option {
        text-align: center;
        width: 100%;
        margin: 15px;
        padding: 10px;
        border: 1px solid $nhsuk-grey;

        .footer-text {
            text-align: left;
        }
    }
</style>
