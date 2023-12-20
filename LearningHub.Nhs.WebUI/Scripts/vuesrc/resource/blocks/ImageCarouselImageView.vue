<template>
    <div class="image-carousel-image-view" :class="{'fullscreen-sizes': isFullscreen}">
        <picture v-if="imageResourceLink">
            <img v-bind:src="imageResourceLink" v-bind:alt="image.altText" loading="lazy">
        </picture>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { ImageModel } from "../../models/contribute-resource/blocks/imageModel";
    
    export default Vue.extend({
        props: {
            image: { type: Object } as PropOptions<ImageModel>,
            isFullscreen: Boolean,
        },
        computed: {
            imageResourceLink(): string {
                return this.image.getFileModel()?.getDownloadResourceLink();
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .image-carousel-image-view {
        display: flex;
        justify-content: center;
        picture {
            img {
                display: block;
                max-width: 100%;
                max-height: 400px;
                height: auto;
            }
        }

        &.fullscreen-sizes {
            img {
                max-height: 60vh;
            }
        }
    }
</style>