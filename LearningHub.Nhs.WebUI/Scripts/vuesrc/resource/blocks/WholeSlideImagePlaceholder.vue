<template>
    <div class="slide-viewer-with-annotations d-flex align-items-center" 
        v-bind:class="{ 'slide-viewer-fullscreen': isFullScreen }">

        <div class="flex centered-box">
            <div class="d-flex">
                <i class="fas fa-info-circle blue pr-2"></i>
                <h2>This slide isn't ready yet</h2>
            </div>
            <br>
            {{ placeholderText }}
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
  
    export default Vue.extend({
        props: {
            placeholderText: String,
            isFullScreen: Boolean,
        },
    });
 
  
</script>

<style lang="scss" scoped>
    @use "../../../../Styles/abstracts/all" as *;
    
    .slide-viewer-with-annotations {
        background-color: $nhsuk-blue;
        position: relative;
        height: 800px;
            
        &.slide-viewer-fullscreen {
            .slide-viewer-controls-wrapper {
                @media screen and (orientation: portrait) {
                    bottom: 70px;
                }
            }
        }
    }

    .centered-box {
        background-color: $nhsuk-grey-white;
        height: 25%;
        width: 100%;
        border-radius: 6px;
        margin: 20px;
        padding: 20px;
        word-break: break-word;
    }

    .blue {
        color: $nhsuk-blue;
        font-size: 22px;
    }
</style>

