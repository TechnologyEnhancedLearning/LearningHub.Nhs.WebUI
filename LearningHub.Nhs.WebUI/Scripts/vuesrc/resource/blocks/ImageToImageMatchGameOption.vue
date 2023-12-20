<template>
    <div>
        <svg v-if="isFirstBlock" :aria-labelledby="matchAnswer.id" width="266" height="220" viewBox="0 0 266 220"
             fill="none"
             xmlns="http://www.w3.org/2000/svg">
            <desc :id="matchAnswer.id">{{ matchAnswer.mediaBlock.image.altText }}</desc>

            <mask id="path-1-inside-1_0_1" fill="white">
                <path fill-rule="evenodd" clip-rule="evenodd" d="M231.942 3H3V217H231.942L263 110.805L231.942 3Z" />
            </mask>
            <path id="path-main-border" fill-rule="evenodd" clip-rule="evenodd"
                  d="M231.942 3H3V217H231.942L263 110.805L231.942 3Z" />
            <path id="main-border"
                  d="M3 3V0H0V3H3ZM231.942 3L234.825 2.16949L234.2 0H231.942V3ZM3 217H0V220H3V217ZM231.942 217V220H234.19L234.821 217.842L231.942 217ZM263 110.805L265.879 111.647L266.124 110.811L265.883 109.974L263 110.805ZM3 6H231.942V0H3V6ZM6 217V3H0V217H6ZM231.942 214H3V220H231.942V214ZM234.821 217.842L265.879 111.647L260.121 109.962L229.063 216.158L234.821 217.842ZM265.883 109.974L234.825 2.16949L229.059 3.83051L260.117 111.635L265.883 109.974Z"
                  fill="#005EB8" mask="url(#path-1-inside-1_0_1)" />
            <mask id="path-3-inside-2_0_1" fill="white">
                <path fill-rule="evenodd" clip-rule="evenodd"
                      d="M234 0H0V220H234V217.5L266 110.5L261.5 95L257.5 81L249.5 53.5L241.5 26.5L234 0.5V0Z" />
            </mask>
            <path id="focus-border"
                  d="M0 0V-3H-3V0H0ZM234 0H237V-3H234V0ZM0 220H-3V223H0V220ZM234 220V223H237V220H234ZM234 217.5L231.126 216.64L231 217.061V217.5H234ZM266 110.5L268.874 111.36L269.128 110.513L268.881 109.664L266 110.5ZM261.5 95L258.615 95.8242L258.619 95.8364L261.5 95ZM257.5 81L260.385 80.1758L260.383 80.1689L260.381 80.162L257.5 81ZM249.5 53.5L252.381 52.662L252.379 52.6549L252.376 52.6477L249.5 53.5ZM241.5 26.5L238.618 27.3315L238.621 27.3419L238.624 27.3523L241.5 26.5ZM234 0.5H231V0.924047L231.118 1.33148L234 0.5ZM0 3H234V-3H0V3ZM3 220V0H-3V220H3ZM234 217H0V223H234V217ZM231 217.5V220H237V217.5H231ZM236.874 218.36L268.874 111.36L263.126 109.64L231.126 216.64L236.874 218.36ZM268.881 109.664L264.381 94.1636L258.619 95.8364L263.119 111.336L268.881 109.664ZM264.385 94.1758L260.385 80.1758L254.615 81.8242L258.615 95.8242L264.385 94.1758ZM260.381 80.162L252.381 52.662L246.619 54.338L254.619 81.838L260.381 80.162ZM252.376 52.6477L244.376 25.6477L238.624 27.3523L246.624 54.3523L252.376 52.6477ZM244.382 25.6685L236.882 -0.331482L231.118 1.33148L238.618 27.3315L244.382 25.6685ZM231 0V0.5H237V0H231Z"
                  fill="#FFED00" mask="url(#path-3-inside-2_0_1)" />

            <image width="166" height="166" x="32" y="25" preserveAspectRatio="none" :href="href" v-bind:alt="matchAnswer.mediaBlock.image.altText"></image>
            <image href="/images/plus.svg" alt="add" x="42" y="33" class="fullscreen-button" @click="openImageFullScreen" />
        </svg>

        <svg v-else :aria-labelledby="matchAnswer.id" width="267" height="220" viewBox="0 0 267 220" fill="none"
             xmlns="http://www.w3.org/2000/svg">
            <desc :id="matchAnswer.id">{{ matchAnswer.mediaBlock.image.altText }}</desc>

            <mask id="path-1-inside-1_0_2" fill="white">
                <path fill-rule="evenodd" clip-rule="evenodd" d="M264 3H4L35.058 111.008L4 217H264V3Z" />
            </mask>
            <path id="path-main-border" fill-rule="evenodd" clip-rule="evenodd"
                  d="M264 3H4L35.058 111.008L4 217H264V3Z" />
            <path id="main-border"
                  d="M4 3V0H0.0157694L1.11683 3.82907L4 3ZM264 3H267V0H264V3ZM35.058 111.008L37.937 111.851L38.1819 111.015L37.9412 110.178L35.058 111.008ZM4 217L1.12105 216.156L-0.00520323 220H4V217ZM264 217V220H267V217H264ZM4 6H264V0H4V6ZM1.11683 3.82907L32.1749 111.837L37.9412 110.178L6.88317 2.17093L1.11683 3.82907ZM32.1791 110.164L1.12105 216.156L6.87895 217.844L37.937 111.851L32.1791 110.164ZM264 214H4V220H264V214ZM261 3V217H267V3H261Z"
                  fill="#005EB8" mask="url(#path-1-inside-1_0_2)" />
            <mask id="path-3-inside-2_0_2" fill="white">
                <path fill-rule="evenodd" clip-rule="evenodd" d="M267 0H0L31.8942 111.036L0 220H267V0Z" />
            </mask>
            <path id="focus-border"
                  d="M0 0V-3H-3.98304L-2.8834 0.828236L0 0ZM267 0H270V-3H267V0ZM31.8942 111.036L34.7734 111.879L35.0178 111.044L34.7776 110.208L31.8942 111.036ZM0 220L-2.8792 219.157L-4.00398 223H0V220ZM267 220V223H270V220H267ZM0 3H267V-3H0V3ZM-2.8834 0.828236L29.0108 111.864L34.7776 110.208L2.8834 -0.828236L-2.8834 0.828236ZM29.015 110.193L-2.8792 219.157L2.8792 220.843L34.7734 111.879L29.015 110.193ZM267 217H0V223H267V217ZM264 0V220H270V0H264Z"
                  fill="#FFED00" mask="url(#path-3-inside-2_0_2)" />

            <image width="166" height="166" x="62" y="25" preserveAspectRatio="none" :href="href" v-bind:alt="matchAnswer.mediaBlock.image.altText"></image>
            <image href="/images/plus.svg" alt="add" x="72" y="33" class="fullscreen-button" @click="openImageFullScreen" />
        </svg>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { BlockModel } from "../../models/contribute-resource/blocks/blockModel";
    import Button from "../../globalcomponents/Button.vue";
    import ImagePublishedView from "../../contribute-resource/components/published-view/ImagePublishedView.vue";
    
    export default Vue.extend({
        components: {
            Button,
            ImagePublishedView
        },
        props: {
            matchAnswer: { type: Object } as PropOptions<BlockModel>,
            isFirstBlock: Boolean
        },
        computed: {
            href(): string {
                return this.matchAnswer.mediaBlock.image.getFileModel().getDownloadResourceLink();
            }
        },
        methods: {
            openImageFullScreen(e: any) {
                this.$emit('openImageFullScreen', this.href);
                e.stopPropagation();
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;
    
    #focus-border {
        display: none;
    }
    
    .selected-block {
        #focus-border {
            display: block;
        }
    
        #main-border {
            fill: $nhsuk-grey;
        }
    }
    
    .matched-block {
        #path-main-border {
            fill: $nhsuk-blue;
        }
    }
</style>