<template>
    <div>
        <div class="block pb-4">
            <div class="text-box col-8 p-0">
                <div class="mr-20">
                    <ckeditorwithhint v-if="richText" :initialValue="message" :maxLength="textMaxLength" @change="updateByCkEditor" />
                    <div v-else>
                        <textarea class="form-control text-input nhsuk-textarea"
                                  placeholder="Text input"
                                  rows="4"
                                  :maxlength="textMaxLength"
                                  v-model="message"></textarea>
                        <div class="footer-text">
                            You have {{ charactersRemaining }} characters remaining.
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-4 p-0 block-text">
                {{ placeholderText }}
                <br/>
            </div>
        </div>
        <div class="padded-view pt-0"
             v-if="!imageZone">
            <hr/>
            <h3 class="supplementary-content-text">Add more content to support this question?</h3>
            <FilteredBlockCollectionView :blockCollection="blockCollection"
                                         :selection="blockCollection => blockCollection.blocks.filter(block => block.blockType !== BlockTypeEnum.Text)"
                                         :can-be-duplicated="false"/>
            <AddBlocksMenu @add-media-block="addMediaBlock"/>
        </div>
    </div>
</template>


<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import _ from 'lodash';
    import { BlockTypeEnum } from '../../../models/contribute-resource/blocks/blockTypeEnum';
    import { BlockCollectionModel } from '../../../models/contribute-resource/blocks/blockCollectionModel';
    import AddBlocksMenu from './AddBlocksMenu.vue';
    import FilteredBlockCollectionView from './FilteredBlockCollectionView.vue';
    import type { MediaTypeEnum } from '../../../models/contribute-resource/blocks/mediaTypeEnum';
    import ckeditorwithhint from '../../../ckeditorwithhint.vue';

    export default Vue.extend({
        components: {
            AddBlocksMenu,
            FilteredBlockCollectionView,
            ckeditorwithhint,
        },
        props: {
            blockCollection: { type: Object } as PropOptions<BlockCollectionModel>,
            textMaxLength: { type: Number },
            placeholderText: { type: String },
            imageZone: Boolean,
            richText: Boolean
        },
        data() {
            const messageOrUndefined = this.blockCollection.blocks[0]?.textBlock.content;
            const charactersUsed = messageOrUndefined?.length || 0;
            return {
                isReady: false,
                charactersRemaining: this.textMaxLength - charactersUsed,
                message: messageOrUndefined || "",
                BlockTypeEnum,
            };
        },

        created() {
            this.updateContent = _.debounce(this.updateContentDebounced, 100);
            if (this.blockCollection.blocks.length === 0) {
                this.blockCollection.addBlock(BlockTypeEnum.Text);
            }
        },

        watch: {
            message() {
                this.blockCollection.blocks[0].textBlock.content = this.message;
                this.updateContent();
            },
            blockCollection: {
                deep: true,
                handler() {
                    this.message = this.blockCollection.blocks[0]?.textBlock.content || "";
                }
            },
        },

        methods: {
            updateContentDebounced() {
                this.isReady = ((this.message.length > 0) && (this.message.length <= this.textMaxLength));
                this.charactersRemaining = this.textMaxLength - this.message.length;
                this.$emit("updated");
            },

            updateContent() {
                // replaced with debounced in created()
            },

            addMediaBlock(fileId: number, mediaType: MediaTypeEnum) {
                this.blockCollection.addMediaBlock(fileId, mediaType);
            },

            updateByCkEditor(message: string, valid: boolean) {
                this.message = message;
                this.updateContentDebounced();
            }
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .block {
        display: flex;
        flex-flow: row;
        font-size: 18px;
    }

    .text-box {
        vertical-align: top;
    }


    .text-input {
        border-radius: 4px;
        border: 2px solid $nhsuk-grey;
        color: $nhsuk-black;
    }

    .footer-text {
        color: $nhsuk-grey;
        margin-top: 5px;
    }

    .block-text {
        color: $nhsuk-grey;
    }

    .supplementary-content-text {
        margin: 15px 0px 15px 0px;
    }

    .padded-view {
        padding: 0px;
    }
</style>
