<template>
    <div class="edit-save-field-value-display"
         v-bind:class="{ 'block-view': blockView }">
        <h1 v-if="isH1">
            {{ value }}
        </h1>
        <h3 v-else-if="isH3">
            {{ value }}
        </h3>
        <h4 v-else-if="isH4">
            {{ value }}
        </h4>
        <span v-else>
            {{ value }}
        </span>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    
    export default Vue.extend({
        props: {
            value: String,
            isH1: Boolean,
            isH3: Boolean,
            isH4: Boolean,
            size: String,
            blockView: Boolean,
        },
    });
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .edit-save-field-value-display {
        &.block-view {
            background-color: $nhsuk-grey-white;
            padding: 10px;
            margin: 0;
        }

        h3, h4 {
            font-size: 19px;
        }

        &--value {
            margin: 8.5px 3px 5px 7px;
        }

        &--value-large {
            font-size: 4.8rem;
        }
    }
</style>