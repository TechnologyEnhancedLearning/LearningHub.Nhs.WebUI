<template>    
    <div>
        <svg v-if="isFirstBlock" :aria-labelledby="matchAnswer.id" width="334" height="206" viewBox="0 0 334 206" fill="none"
             xmlns="http://www.w3.org/2000/svg">
            <desc :id="matchAnswer.id">{{ matchAnswer.mediaBlock.image.altText }}</desc>

            <image width="166" height="166" x="3.5" y="36" preserveAspectRatio="none" :href="href" v-bind:alt="matchAnswer.mediaBlock.image.altText"></image>
            <image href="/images/plus.svg" alt="add" x="13.5" y="44" class="fullscreen-button" @click="openImageFullScreen" />

            <path class="main-border" fill="transparent"
                  d="M5 35V5H306.405L327.291 17.3926C328.551 18.1405 328.606 19.9454 327.393 20.7679L306.405 35H5Z"
                  @click="selectAnswer" />
            <path class="focus-border"
                  d="M2 38V2H306.886L321 10.5L327.5 14.5L330.356 16.5403C330.77 16.8356 331.055 17.2773 331.155 17.7755L331.5 19.5L330.5 21.5L327.5 24L320.5 29L306.886 38H2Z" />

            <text x="13" y="27" fill="black" @click="selectAnswer">{{ this.matchAnswer.mediaBlock.image.description }}</text>
        </svg>

        <svg v-else width="346" height="40" viewBox="0 0 346 40" fill="none"
             xmlns="http://www.w3.org/2000/svg">
            <path class="main-border"
                  d="M341 35V5H16L36.8047 17.3943C38.0611 18.1428 38.1155 19.9426 36.9066 20.7657L16 35H341Z"
                  @click="selectAnswer" />
            <path class="focus-border"
                  d="M344 38V2H6C6 2 34.4579 18.5 34.4579 19C34.4579 19.5 7.4978 38 7.4978 38H344Z" />
            <text x="50" y="27" fill="black" @click="selectAnswer">{{ this.matchAnswer.textBlock.content }}</text>
        </svg>

    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { BlockModel } from "../../models/contribute-resource/blocks/blockModel";
    
    export default Vue.extend({
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
            selectAnswer() {
                this.$emit('selectAnswer');
            },
            openImageFullScreen(e: any) {
                this.$emit('openImageFullScreen', this.href);
                e.stopPropagation();
            }
        }
    });
</script>

<style lang="scss" scoped>
    text {
        font-size: 16px;
    }

    .main-border,
    text {
        pointer-events: visible;
        cursor: pointer;
    }
</style>