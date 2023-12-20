<template>
    <div class="viewer-background">
        <Carousel3d ref="imageCarousel"
                    :class="{'carousel-3d-fullscreen-wrapper' : isFullscreen}"
                    @after-slide-change="onPageChange"
                    @before-slide-change="beforeChange"
                    :controls-visible="true" 
                    :height="slideHeight" 
                    :width="slideWidth" 
                    :clickable="false" 
                    :display="3" 
                    :perspective="25" 
                    :space="300"
                    :border="0">
            <Slide v-for="(imageBlock, index) in this.imageBlocks" :index="index" :key="index">
                <div class="slide-background" >
                    <div class="slide-text" v-if="index === activeSlideIndex"> 
                        <h3> {{imageBlock.title}} </h3>
                    </div>
                    <div :class="[`slide-image ${index === activeSlideIndex ? '' : ' transparent-image'}`]">
                        <ImageCarouselImageView v-bind:image="imageBlock.mediaBlock.image" 
                                                v-bind:is-fullscreen="isFullscreen"/>
                    </div>
                    <div class="slide-text" v-if="index === activeSlideIndex && imageBlock.mediaBlock.image.description"> 
                        {{imageBlock.mediaBlock.image.description}}
                    </div>
                </div>
            </Slide>
        </Carousel3d>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { BlockModel } from "../../models/contribute-resource/blocks/blockModel";
    import { BlockCollectionModel } from "../../models/contribute-resource/blocks/blockCollectionModel";
    import IconButton from "../../globalcomponents/IconButton.vue";
    import ImagePublishedView from "../../contribute-resource/components/published-view/ImagePublishedView.vue";
    import { Carousel3d, Slide } from 'vue-carousel-3d';
    import ImageCarouselImageView from "./ImageCarouselImageView.vue";
    
    export default Vue.extend({
        components: {
            ImageCarouselImageView,
            ImagePublishedView,
            IconButton,
            Carousel3d,
            Slide
        },
        data() {
            return {
                counterInterval: 0,  
            }
        },
        created() {
            this.createSlideIntervalIfAutoPlay();
        },
        mounted() {
            setTimeout(() => {
                this.triggerResizeEvent();
            }, 500);
        },
        beforeDestroy() {
            this.destroySlideInterval();
        },
        props: {
            imageBlockCollection: { type: Object } as PropOptions<BlockCollectionModel>,
            slideHeight: Number,
            slideWidth: Number,
            activeSlideIndex: Number,
            isAutoPlay: Boolean,
            isFullscreen: Boolean,
        },
        computed: {
            imageBlocks(): BlockModel[] {
                return this.imageBlockCollection.blocks;
            },
        },
        watch: {
            activeSlideIndex: function() {
                this.destroySlideInterval();
                
                if (this.activeSlideIndex != -1) {
                    (this.$refs.imageCarousel as any).goSlide(this.activeSlideIndex);
                    this.$emit('slideChanged', this.activeSlideIndex);
                    this.createSlideIntervalIfAutoPlay();
                }
            },
            isAutoPlay: function() {
                if (this.isAutoPlay) {
                    this.createSlideIntervalIfAutoPlay();
                } else {
                    this.destroySlideInterval();
                }
            },
        },
        methods: {
            triggerResizeEvent() {
                if (typeof(Event) === 'function') {
                    window.dispatchEvent(new Event('resize'));
                } else {
                    // for IE and other old browsers
                    // causes deprecation warning on modern browsers
                    var evt = window.document.createEvent('UIEvents');
                    evt.initEvent('resize', true, false);
                    window.dispatchEvent(evt);
                }
            },
            onPageChange(index : number) {
                this.$emit('slideChanged', index);
            },
            beforeChange() {
                // Set descriptions to disappear while slides change
                this.$emit('slideChanged', -1);
            },
            goToNextSlide() {
                if (this.activeSlideIndex != -1) {
                    (this.$refs.imageCarousel as any).goSlide(this.activeSlideIndex + 1);
                    this.$emit('slideChanged', this.activeSlideIndex);
                }
            },
            createSlideIntervalIfAutoPlay() {
                if (this.isAutoPlay) {
                    this.counterInterval = setInterval(() => this.goToNextSlide(), 5000); // 5 second delay between changing slides
                } else {
                    this.destroySlideInterval();
                }
            },
            destroySlideInterval() {
                clearInterval(this.counterInterval);
            }
        }
    });
</script>

<style>
    .carousel-3d-fullscreen-wrapper .carousel-3d-slider {
        height: 100% !important;
    }
</style>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .viewer-background {
        background: $nhsuk-grey;
        height: 100%;
    }
    
    .arrow-background {
        background: $nhsuk-grey-white;
    }
    
    .carousel-3d-container {
        margin: 0;
        height: 100% !important;
    }

    // Styles the control arrows on the carousel
    ::v-deep .prev,  ::v-deep .next {
        background: $nhsuk-grey-white;
        display: flex;
        justify-content: center;
        border-radius: 50%;
        width: 35px !important;
        height: 35px !important;
        line-height: 30px !important;
        
        span {
            display: inline-block;
            vertical-align: middle;
            font-size: 40px;
        }
    }

    .carousel-3d-slide {
        background: transparent;
        height: 100% !important;
    }    

    .slide-background {
        width: 100%;
        height: 100%;
        background: transparent;
        display: flex;
        flex-direction: column;
        justify-content: center;
        
        .slide-text {
            padding: 10px;
            background: $nhsuk-grey-white;
            border: solid 1px $nhsuk-grey-light;
        }
        
        .slide-image {
            flex-grow: 0;
            flex-shrink: 0;
            background: $nhsuk-white;
        }

        .transparent-image {
            opacity: 0.5;
            background: transparent;
        }
    }
</style>