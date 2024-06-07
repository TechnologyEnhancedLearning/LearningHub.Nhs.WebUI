<!-- INSTRUCTIONS FOR USE: -->
<!--
    Use as per the examples below:
      
    COLOR:
    * Default (white background, black icon)
        <IconButton iconClasses="fa-solid fa-trash-can-alt"
                    ariaLabel="Delete section"></IconButton>
    
    * Blue (blue background, white icon)
        <IconButton iconClasses="fa-solid fa-trash-can-alt"
                    ariaLabel="Delete section"
                    color="blue"></IconButton>
                    
    SHAPE:
    * Default (square)
        <IconButton iconClasses="fa-solid fa-trash-can-alt"
                    ariaLabel="Delete section"></IconButton>
    
    * Circle
        <IconButton iconClasses="fa-solid fa-trash-can-alt"
                    ariaLabel="Delete section"
                    shape="circle"></IconButton>
             
    SIZE:
    * Default (40px x 40px)
        <IconButton iconClasses="fa-solid fa-trash-can-alt"
                    ariaLabel="Delete section"></IconButton>
    
    * Small (20px x 20px)
        <IconButton iconClasses="fa-solid fa-trash-can-alt"
                    ariaLabel="Delete section"
                    size="small"></IconButton>
-->

<template>
    <button v-on:click="$emit('click', $event)"
            class="icon-button-component"
            v-bind:class="{
                'icon-button-component-color-default': (!color),
                'icon-button-component-color-blue': (color === 'blue'),
                'icon-button-component-shape-default': (!shape),
                'icon-button-component-shape-circle': (shape === 'circle'),
                'icon-button-component-size-default': (!size),
                'icon-button-component-size-small': (size === 'small'),
                'icon-button-component-size-large': (size === 'large'),
                'icon-button-component-disabled': (disabled),
            }"
            v-bind:disabled="disabled">
        <i v-bind:class="iconClasses"
           v-bind:aria-label="ariaLabel"
           v-bind:role="iconRole"></i>
        <span>{{ label }}</span>
    </button>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        props: {
            disabled: Boolean,
            iconClasses: String,
            label: String,
            ariaLabel: String,
            color: String,
            shape: String,
            size: String,
            iconRole: String,
        },
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    button.icon-button-component {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        font: inherit;
        cursor: pointer;
        outline: none;

        &:focus {
            border-color: $govuk-focus-highlight-yellow;
            color: $nhsuk-black;
            background-color: $govuk-focus-highlight-yellow;
            box-shadow: 0 2px 0 $nhsuk-black;
            border-bottom: 1px solid $nhsuk-black;
        }

        &.icon-button-component-color-default {
            background: none;
            border: 1px solid transparent;
            color: $nhsuk-grey;
            
            &:hover {
                color: $nhsuk-navbar-blue;
            }
            &:focus {
                background-color: $govuk-focus-highlight-yellow;
            }
        }

        &.icon-button-component-color-blue {
            background: $nhsuk-blue;
            border: 1px solid transparent;
            color: $nhsuk-white;
            
            &:hover {
                background: $nhsuk-btn-blue-hover;
                color: $nhsuk-white;
            }
            &:focus {
                background: $govuk-focus-highlight-yellow;
                color: $nhsuk-black;
            }
        }

        &.icon-button-component-shape-default {
            border-radius: 0;
        }

        &.icon-button-component-shape-circle {
            border-radius: 50%;
            
            &:focus {
                box-shadow: 0 0 0 3px #212b32 inset;
                border: none;
            }
        }

        &.icon-button-component-size-default {
            width: 40px;
            height: 40px;
            padding: 10px;

            &.icon-button-component-shape-circle {
                width: 50px;
                height: 50px;
            }
        }

        &.icon-button-component-size-small {
            width: 20px;
            height: 20px;
            padding: 0;
            font-size: 9px;
        }
        &.icon-button-component-size-large {
            width: 32px;
            height: 32px;
            padding: 0;
            font-size: 32px;
        }

        &.icon-button-component-disabled {
            background-color: $nhsuk-grey-light;
            border-color: $nhsuk-grey-light;
            color: $nhsuk-black-hover;
            opacity: 0.5;
            box-shadow: none;
            
            &:hover, &:focus {
                background-color: $nhsuk-grey-light;
                border-color: $nhsuk-grey-light;
                cursor: not-allowed;
                box-shadow: none;
            }
        }
    }
    
    .blue {
        color: $nhsuk-blue;
    }
</style>