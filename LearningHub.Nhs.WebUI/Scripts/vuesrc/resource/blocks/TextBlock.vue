<template>
    <div class="view-resource-text-block-component">
        <h3 class="view-resource-text-block-component-item-title nhsuk-heading-m">{{ block.title }}</h3>
        <div v-html="sanitizedContent" class="view-resource-text-block-content" />
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { BlockModel } from '../../models/contribute-resource/blocks/blockModel';
    import { SanitizationRules, cleanHtml } from '../sanitization';

    export default Vue.extend({
        props: {
            block: { type: Object } as PropOptions<BlockModel>,
            sanitization: { type: Object } as PropOptions<SanitizationRules>,
        },
        computed: {
            sanitizedContent(): string {
                return cleanHtml(this.block.textBlock.content, this.sanitization);
            }
        }
    })
</script>

<style lang="scss" scoped>
    .view-resource-text-block-component {
        .view-resource-text-block-component-item-title {
            padding-bottom: 8px;
        }

        .view-resource-text-block-content {
            word-break: break-word;
        }
    }
</style>
