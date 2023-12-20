<template>
  <button @click="$emit(`click`)"><slot/></button>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({}); 
</script>

<style scoped lang="scss">
    @use '../../../Styles/abstracts/all' as *;

    button {
        padding: 10px;
        border-bottom: none;
        border-left: none;
        border-right: none;
        border-top: 1px solid $nhsuk-grey-light;
        outline: none;
        background: inherit;
        text-align: left;
    }
    
    button:hover {
        background: $nhsuk-grey-lighter;
        cursor: pointer;
    }
</style>