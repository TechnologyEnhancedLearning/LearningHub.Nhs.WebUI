<template>
    <i v-if="complete"
       class="fa-solid fa-circle-check contribute-page-tab-completion-status-complete"></i>
    <i v-else
       class="fas fa-adjust contribute-page-tab-completion-status-incomplete"></i>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        props: {
            complete: Boolean
        },
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-page-tab-completion-status-complete {
        color: $nhsuk-green;
    }

    .contribute-page-tab-completion-status-incomplete {
        color: $nhsuk-grey;
    }
</style>