<!-- INSTRUCTIONS FOR USE: -->
<!--
    Use as per the examples below:

    SIZES:
    * Default
        <Button>Save</Button>

    * Medium
        <Button size="medium">Save</Button>
        
    * Small
        <Button size="small">Save</Button>

    COLOURS:
    * Default (white background, blue text)
        <Button>Save</Button>

    * Blue (blue background, white text)
        <Button color="blue">Save</Button>

    * Green (green background, white text)
        <Button color="green">Save</Button>

    * Red (red background, white text)
        <Button color="red">Save</Button>

    * Light-Red (light-red background, red text)
        <Button color="light-red">Save</Button>
        
    * Grey (light-grey background, black text)
        <Button color="grey">Save</Button>

-->

<template>
    <button v-on:click="$emit('click')"
            class="nhsuk-button nhsuk-u-margin-bottom-0 nhsuk-button--no-shrink d-block"
            v-bind:class="{
                'nhsuk-button': (color === 'green'),
                'button-component-color-red': (color === 'red'),
                'button-component-color-light-red': (color === 'light-red'),
                'nhsuk-button--secondary': (color === 'grey' || !color || (!size && color !== 'green')),
                'nhsuk-button--disabled': (disabled),
            }"
            v-bind:disabled="disabled">
        <slot></slot>
    </button>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        props: {
            size: String,
            color: String,
            disabled: Boolean,
        },
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    button.button-component {
        text-align: center !important;
        border: 1px solid transparent;
        border-radius: 5px;
    }

    button.button-component-size-default {
        font-size: 1.9rem;
        min-width: 115px;
        padding: 0px 25px 0px 25px;
        min-height: 50px;
    }

    button.button-component-color-default {
        background-color: $nhsuk-white;
        color: $nhsuk-blue;
        border: 1px solid $nhsuk-blue;
    }
    button.button-component-color-default:hover {
        background-color: $nhsuk-btn-outline-hover;
    }

    button.button-component-color-blue {
        background-color: $nhsuk-blue;
        color: $nhsuk-white;
    }
    button.button-component-color-blue:hover {
        background-color: $nhsuk-btn-blue-hover;
    }

    button.button-component-color-green {
        background-color: $nhsuk-green;
        border-color: $nhsuk-green;
        color: $nhsuk-white;
    }
    button.button-component-color-green:hover {
        background-color: $nhsuk-green-hover;
        border-color: $nhsuk-green-hover;
    }

    button.button-component-color-red {
        background-color: $nhsuk-red;
        color: $nhsuk-white;
        border: 1px solid $nhsuk-red;
    }

    button.button-component-color-grey {
      color: $nhsuk-black;
      border: 1px solid $nhsuk-black;
    }
    button.button-component-color-grey:hover {
      background-color: $nhsuk-grey-lighter;
      color: $nhsuk-black;
      border: 1px solid $nhsuk-black;
    }

    button.button-component-color-light-red {
        background-color: rgba($nhsuk-red, 0.05);
        color: $nhsuk-red;
        border: 1px solid $nhsuk-red;
    }

    button.button-component-disabled {
        background-color: $nhsuk-grey-light;
        border-color: $nhsuk-grey-light;
        color: $nhsuk-black-hover;
        opacity: 0.5;
    }
    button.button-component-disabled:hover {
        background-color: $nhsuk-grey-light;
        border-color: $nhsuk-grey-light;
        cursor: not-allowed;
    }

    button.button-component:focus {
        border-color: $govuk-focus-highlight-yellow;
        color: $nhsuk-black;
        background-color: $govuk-focus-highlight-yellow;
        box-shadow: 0 2px 0 $nhsuk-black;
        border-bottom: 1px solid $nhsuk-black;
    }

    button.button-component-size-medium:focus {
        border-color: transparent;
        color: $nhsuk-black;
        background-color: $govuk-focus-highlight-yellow;
        box-shadow: 0 3px 0 $nhsuk-black;
    }
</style>