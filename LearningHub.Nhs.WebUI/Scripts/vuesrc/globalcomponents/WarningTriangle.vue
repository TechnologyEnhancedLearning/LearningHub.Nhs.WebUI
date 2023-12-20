<!-- INSTRUCTIONS FOR USE: -->
<!--
    Use as per the examples below:

    COLOURS:
    * Yellow
        <WarningTriangle color="yellow"></WarningTriangle>

    * Red (red background, white text)
        <WarningTriangle color="red"></WarningTriangle>

-->

<template>
    <i v-on:click="$emit('click')"
            class="fa-solid fa-triangle-exclamation"
            v-bind:class="{
                'warning-triangle-color-yellow': (color === 'yellow'),
                'warning-triangle-color-red': (color === 'red'),
            }">
    </i>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        props: {
            color: String,
        },
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    i.warning-triangle-color-yellow {
        color: $nhsuk-warm-yellow;
    }
    
    i.warning-triangle-color-red {
        color: $nhsuk-red;
    }
</style>