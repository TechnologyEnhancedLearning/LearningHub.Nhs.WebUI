<!-- INSTRUCTIONS FOR USE: -->
<!--
    Use as per the example below:

    <LinkTextAndIcon v-on:click="..."
                     iconClasses="fa-solid fa-pencil">
        Edit title
    </LinkTextAndIcon>

-->

<template>
    <a href="#"
       v-on:click.prevent="$emit('click')"
       class="link-text-and-icon-component">
        <span>
            <slot></slot>
        </span>
        <i v-if="iconClasses" v-bind:class="iconClasses"></i>
    </a>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        props: {
            iconClasses: String
        },
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .link-text-and-icon-component {
        text-decoration: none !important;
    }
    .link-text-and-icon-component:focus {
        outline: none;
        color: $nhsuk-black;
        background-color: $govuk-focus-highlight-yellow;
        box-shadow: 0 -2px $govuk-focus-highlight-yellow,0 4px $nhsuk-black;
    }

    .link-text-and-icon-component > span {
        text-decoration: underline;
        display: inline-block;
    }
    .link-text-and-icon-component:focus > span {
        text-decoration: none;
    }

    .link-text-and-icon-component > i {
        color: $nhsuk-grey;
    }
</style>