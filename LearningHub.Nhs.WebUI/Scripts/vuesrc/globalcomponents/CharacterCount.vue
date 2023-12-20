<!-- INSTRUCTIONS FOR USE: -->
<!--

    Use as per the examples below:

    <template>
        ...
        <CharacterCount v-model="name"
                        :characterLimit="60"
                        :hasOtherError="descriptionIsValid"></CharacterCount>

        <button :disabled="!nameIsValid || !descriptionIsValid">
            Submit form
        </button>
        ...
    </template>

    <script lang="ts">
        import Vue from 'vue';
        import CharacterCount from '............/CharacterCount.vue';

        export default Vue.extend({
            components: {
                CharacterCount
            },
            data() {
                return {
                    name: '',
                    nameIsValid: true,
                    descriptionIsValid: true,
                };
            },
        });
    </script>

-->

<template>
    <div class="character-count-component"
         :class="{
                        'character-count-component--error': (hasOtherError),
                        'character-count-component--large': (size === 'large'),
                       }">
        <h2 class="nhsuk-heading-l">
            <slot name="title"></slot>
        </h2>
        <div class="mb-2">
            <slot id="character-count-component-description"
                  name="description"></slot>
        </div>
        <div class="character-count-component-other-error-message"
             v-if="hasOtherError">
            <slot id="character-count-component-other-error-message"
                  name="otherErrorMessage"></slot>
        </div>
        <div class="d-flex align-items-center">
            <textarea v-if="rows"
                      class="form-control"
                      :rows="rows"
                      :id="inputId"
                      :value="inputValue"
                      :disabled="disabled"
                      :maxlength="characterLimit"
                      @input="onInput($event.target.value)"
                      ref="characterCountComponentInputBox"
                      aria-describedby="character-count-component-description character-count-component-status-message character-count-component-other-error-message"></textarea>
            <input v-else
                   type="text"
                   :id="inputId"
                   :value="inputValue"
                   :disabled="disabled"
                   :maxlength="characterLimit"
                   @input="onInput($event.target.value)"
                   ref="characterCountComponentInputBox"
                   aria-describedby="character-count-component-description character-count-component-status-message character-count-component-other-error-message"/>
            <slot name="afterInput"></slot>
        </div>
        <div id="character-count-component-status-message"
             class="character-count-component-status-message"
             aria-live="polite">
            <span>
                You have {{ charactersDiff }} character{{ charactersDiffPlural ? 's' : '' }} remaining
            </span>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        props: {
            rows: Number,
            value: String,
            characterLimit: Number,
            hasOtherError: Boolean,
            inputId: String,
            disabled: Boolean,
            focusOnLoad: Boolean,
            size: String,
        },
        data() {
            return {
                inputValue: this.value || '',
            };
        },
        computed: {
            charactersDiff(): number {
                return Math.abs(this.characterLimit - this.inputValue.length);
            },
            charactersDiffPlural(): boolean {
                return this.charactersDiff !== 1;
            }
        },
        watch: {
            value: {
                handler(newVal, oldVal) {
                    this.inputValue = newVal || '';
                }
            }
        },
        mounted() {
            if (this.focusOnLoad) {
                const inputElement = this.$refs.characterCountComponentInputBox as any;
                inputElement.focus();
            }
        },
        methods: {
            onInput(newValue: string): void {
                this.inputValue = newValue;
                this.$emit('input', newValue);
            }
        },
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../Styles/abstracts/all' as *;

    .character-count-component {
    }

    .character-count-component input {
        height: 40px !important;
    }

    .character-count-component input,
    .character-count-component textarea {
        width: 100%;
        padding: 5px;
        font-family: $font-stack-bold;
        color: $nhsuk-black;
        border: 2px solid $nhsuk-grey;
        background: $nhsuk-white;
    }

    .character-count-component input:focus,
    .character-count-component textarea:focus {
        outline: 3px solid $govuk-focus-highlight-yellow;
        outline-offset: 0;
        box-shadow: inset 0 0 0 2px;
    }

    .character-count-component input:disabled,
    .character-count-component textarea:disabled {
        background-color: $nhsuk-grey-lighter;
        color: $nhsuk-grey-placeholder;
        border-color: $nhsuk-grey-placeholder;
    }

    .character-count-component.character-count-component--error {
        border-left: 4px solid $nhsuk-red;
        padding-left: 8px;
    }

    .character-count-component.character-count-component--error input,
    .character-count-component.character-count-component--error textarea {
        border-color: $nhsuk-red;
    }

    .character-count-component.character-count-component--error input:focus,
    .character-count-component.character-count-component--error textarea:focus {
        box-shadow: inset 0 0 0 2px $nhsuk-red;
    }

    .character-count-component .character-count-component-status-message {
        color: $nhsuk-grey;
        font-size: 16px;
        margin: 5px 0;
    }

    .character-count-component.character-count-component--error
    .character-count-component-other-error-message {
        color: $nhsuk-red;
    }

    .character-count-component.character-count-component--large input,
    .character-count-component.character-count-component--large textarea {
        font-size: 36px;
        height: 74px !important;
    }
</style>