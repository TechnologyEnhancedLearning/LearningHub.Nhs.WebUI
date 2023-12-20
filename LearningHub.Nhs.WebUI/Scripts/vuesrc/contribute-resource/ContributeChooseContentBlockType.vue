<template>
    <Modal v-on:cancel="$emit('cancel')">

        <div class="contribute-add-content-block-component">

            <h4 class="contribute-add-content-block-component-title">
                Choose a content type
            </h4>

            <div><!-- This div might look redundant, but we use a :last-child CSS selector to hide the final border-bottom -->
                <ChooseContentBlockOption block-type-name="Text"
                                          block-description="Add text and apply styles like bold, italics and different font sizes."
                                          img-src="/images/content-block-type-icons/text.svg"
                                          v-on:choose="$emit('choose', BlockTypeEnum.Text)"/>

                <ChooseContentBlockOption block-type-name="Slide viewer"
                                          block-description="Upload specialist whole-slide image files, like Hamamatsu (.ndpi), Aperio (.svs, .tif), MIRAX (.mrxs) as .zip and Ventana (.tif)."
                                          img-src="/images/content-block-type-icons/wholeslideimage.svg"
                                          v-on:choose="$emit('choose', BlockTypeEnum.WholeSlideImage)"/>

                <ChooseContentBlockOption block-type-name="Media"
                                          block-description="Upload an image, video or audio file."
                                          img-src="/images/content-block-type-icons/media.svg"
                                          v-on:choose="$emit('choose', BlockTypeEnum.Media)"/>
                
                <ChooseContentBlockOption block-type-name="Image carousel"
                                          block-description="Upload multiple images to be viewed in a carousel."
                                          img-src="/images/content-block-type-icons/imagecarousel.svg"
                                          v-on:choose="$emit('choose', BlockTypeEnum.ImageCarousel)"/>
            </div>

            <div class="d-flex justify-content-end mt-10">
                <a href="#"
                   v-on:click.prevent="$emit('cancel')"
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

    import { BlockTypeEnum } from '../models/contribute-resource/blocks/blockTypeEnum';

    export default Vue.extend({
        components: {
            Button,
            ChooseContentBlockOption,
            Modal,
        },
        data() {
            return {
                BlockTypeEnum: BlockTypeEnum,
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-add-content-block-component {
        display: flex;
        flex-direction: column;
        overflow-y: auto; // Add scrollbars if needed

        .contribute-add-content-block-component-title {
            font-size: 32px;
            font-weight: bold;
            text-align: center;
        }
    }
</style>