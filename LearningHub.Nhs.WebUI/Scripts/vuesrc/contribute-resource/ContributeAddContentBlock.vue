<template>
    <div class="contribute-add-content-block my-20 p-25 d-flex flex-row align-items-center">

        <Button @click="$emit('add')">
            + Add content
        </Button>

        <div class="mx-20 col-9">
            Content blocks allow you to create different types of content on the same page.
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Button from '../globalcomponents/Button.vue';

    export default Vue.extend({
        components: {
            Button,
        },
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-add-content-block {
        background-color: $nhsuk-white;
        border: 1px solid $nhsuk-blue;
    }
</style>