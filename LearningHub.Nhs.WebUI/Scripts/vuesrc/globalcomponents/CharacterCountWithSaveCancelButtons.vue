<template>
    <div class="character-count-with-save-cancel-buttons-component">
        <CharacterCount v-model="inputValue"
                        v-bind:characterLimit="characterLimit"
                        v-bind:size="size"
                        v-bind:inputId="inputId"
                        focusOnLoad></CharacterCount>
        <div class="d-inline-flex">
            <Button v-on:click="save"
                    size="small" class="mr-2"
                    color="green">
                Save
            </Button>
            <Button v-on:click="cancel"
                    size="small">
                Cancel
            </Button>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    import Button from './Button.vue';
    import CharacterCount from './CharacterCount.vue';

    export default Vue.extend({
        components: {
            Button,
            CharacterCount
        },
        props: {
            value: String,
            characterLimit: Number,
            size: String,
            inputId:String
        },
        data() {
            return {
                inputValue: this.value,
            };
        },
        methods: {
            save() {
                this.$emit('input', this.inputValue);
                this.$emit('close');
            },
            cancel() {
                this.$emit('close');
            },
        },
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

</style>