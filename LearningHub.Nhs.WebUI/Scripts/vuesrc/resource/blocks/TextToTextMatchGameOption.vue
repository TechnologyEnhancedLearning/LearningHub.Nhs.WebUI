<template>
    <div>
        <svg v-if="isFirstBlock" xmlns="http://www.w3.org/2000/svg" width="440" height="70" viewBox="0 0 440 70" fill="none">
            <path class="main-border"
                  d="M5 65V5H396.875L433.059 33.7116C434.057 34.5039 434.07 36.0157 433.084 36.8243L398.75 65H5Z"/>
            <path class="focus-border"
                  d="M437.132 36.8112L399.866 68H2V2H397.733L437.103 33.7201C438.085 34.5111 438.099 36.0021 437.132 36.8112Z"/>
            <text x="20" y="40" fill="black">
                {{ this.matchAnswer.textBlock.content }}
            </text>
        </svg>

        <svg v-else width="440" height="70" viewBox="0 0 440 70" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path class="main-border"
                  d="M15.0536 65H452.967V8L13.0402 5L49.3452 33.9457C50.3457 34.7434 50.3502 36.2623 49.3546 37.0659L35.1875 48.5L15.0536 65Z"/>
            <path class="focus-border"
                  d="M6.5 68H456V5L4.5 2L28.9799 21.5L39.5 29.5L44.7033 33.96C45.6431 34.7655 45.6329 36.2226 44.6821 37.0149L37.5 43L27.5 51L22 55.5L13.5 62.5L6.5 68Z"/>

            <text x="70" y="40" fill="black">
                {{ this.matchAnswer.textBlock.content }}
            </text>
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
        }
    });
</script>
