<!-- INSTRUCTIONS FOR USE: -->
<!--

-->

<template>
    <i class="fas fa-spinner spinner-component"></i>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        props: {
        },
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    @keyframes spinner-component-animation {
        from {
            transform: rotate(0);
        }

        to {
            transform: rotate(360deg);
        }
    }

    .spinner-component {
        animation-name: spinner-component-animation;
        animation-duration: 1.5s;
        animation-timing-function: linear;
        animation-iteration-count: infinite;
    }
</style>