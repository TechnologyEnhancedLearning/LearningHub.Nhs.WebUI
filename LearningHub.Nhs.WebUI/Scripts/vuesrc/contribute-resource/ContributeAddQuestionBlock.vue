<template>
    <div class="contribute-add-question-block my-20 p-25 d-flex flex-row align-items-center">

        <Button v-on:click="$emit('add')">
            + Add question
        </Button>

        <div class="mx-20 col-9">
            You can help readers engage with your content by adding questions to answer as they progress through the pages.
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

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-add-question-block {
        background-color: $nhsuk-white;
        border: 1px solid $nhsuk-blue;
    }
</style>