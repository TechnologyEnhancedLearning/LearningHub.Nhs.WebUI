<template>
    <div class="edit-save-field-with-character-count-component"
         v-bind:class="{ 'edit-save-field-with-character-count-component--large': (size === 'large') }">
        <CharacterCountWithSaveCancelButtons v-if="isEditing"
                                             v-model="inputValue"
                                             v-bind:characterLimit="characterLimit"
                                             v-bind:size="size"
                                             v-bind:inputId="inputId"
                                             v-on:input="$emit('input', $event)"
                                             v-on:close="isEditing = false"></CharacterCountWithSaveCancelButtons>
        <div v-else>
            <div class="d-flex flex-wrap align-items-baseline">
                <EditSaveFieldValueDisplay v-if="value && !blockView"
                                           v-bind:is-h1="isH1"
                                           v-bind:is-h3="isH3"
                                           v-bind:is-h4="isH4"
                                           v-bind:size="size"
                                           v-bind:value="value">
                </EditSaveFieldValueDisplay>
                <LinkTextAndIcon v-if="!isEditing && !!inputValue"
                                 v-on:click="isEditing = true"
                                 iconClasses="fa-solid fa-pencil"
                                 class="edit-save-field-with-character-count-component-edit-button" 
                                 v-bind:class="{ 'block-view': blockView }">
                    Edit {{ addEditLabel }}
                </LinkTextAndIcon>
                <LinkTextAndIcon v-if="!isEditing && !inputValue"
                                 v-on:click="isEditing = true"
                                 class="edit-save-field-with-character-count-component-edit-button"
                                 v-bind:class="{ 'block-view': blockView }">
                    <i class="fa-solid fa-plus"></i> Add {{ addEditLabel }}
                </LinkTextAndIcon>
            </div>
            <EditSaveFieldValueDisplay v-if="value && blockView"
                                       v-bind:is-h1="isH1"
                                       v-bind:is-h3="isH3"
                                       v-bind:is-h4="isH4"
                                       v-bind:size="size"
                                       v-bind:value="value"
                                       block-view>
            </EditSaveFieldValueDisplay>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    import Button from './Button.vue';
    import CharacterCountWithSaveCancelButtons from './CharacterCountWithSaveCancelButtons.vue';
    import EditSaveFieldValueDisplay from "./subcomponents/EditSaveFieldValueDisplay.vue";
    import LinkTextAndIcon from './LinkTextAndIcon.vue';

    export default Vue.extend({
        components: {
            Button,
            CharacterCountWithSaveCancelButtons,
            EditSaveFieldValueDisplay,
            LinkTextAndIcon,
        },
        props: {
            value: String,
            characterLimit: Number,
            isH1: Boolean,
            isH3: Boolean,
            isH4: Boolean,
            addEditLabel: String,
            blockView: Boolean,
            size: String,
            inputId: String,
        },
        data() {
            return {
                inputValue: this.value,
                isEditing: false,
            };
        },
        watch: {
            isEditing(value: boolean) {
                this.$emit("updateIsEditing", value);
            },
            value() {
                this.inputValue = this.value;
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .edit-save-field-with-character-count-component {
        margin: 10px 0;
        width: 100%;

        &-edit-button {
            font-size: 16px;
            margin: 9px 0 5px 7px;
            
            &.block-view {
                margin-left: 0;
            }
            
            .fa-plus {
                font-size: 14px;
            }
        }
    }

    .edit-save-field-with-character-count-component--large .edit-save-field-with-character-count-component-edit-button {
        font-size: 19px;
        margin-left: 10px;
    }
</style>