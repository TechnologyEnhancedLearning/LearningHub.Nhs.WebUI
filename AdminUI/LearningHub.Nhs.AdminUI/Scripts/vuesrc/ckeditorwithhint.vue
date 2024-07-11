<template>
    <div>
        <ckeditor v-model="description" :config="editorConfig" @ready="onEditorReady" @blur="onBlur"></ckeditor>
        <div :class="[`pt-2 footer-text${this.valid ? '' : ' text-danger'}`]">{{ hint }}</div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import CKEditorToolbar from './models/ckeditorToolbar';
    import CKEditor from 'ckeditor4-vue/dist/legacy.js';
    import {getRemainingCharacters, getRemainingCharactersFromHtml} from "./helpers/ckeditorValidationHelper";

    const getCharactersText = (n: number) => n === 1 ? `${n} character` : `${n} characters`;

    export default Vue.extend({
        components: {
            ckeditor: CKEditor.component
        },
        props: {
            initialValue: { Type: String } as PropOptions<string>,
            maxLength: { Type: Number } as PropOptions<number>,
        },
        data() {
            return {
                editorConfig: {
                    toolbar: CKEditorToolbar.default,
                    versionCheck: false
                },
                description: this.initialValue,
                hint: `You have ${this.maxLength} characters remaining`,
                valid: getRemainingCharactersFromHtml(this.maxLength, this.initialValue) >= 0,
            };
        },
        mounted() {
            this.emitValidity();
        },
        methods: {
            onBlur() {
                this.$emit('blur', this.description, this.valid);
            },
            onEditorReady(editor: any) {
                var current = this;
                editor.on('change', function () {
                    current.calculateCharactersRemaining(current, editor);

                    current.emitValidity();
                    current.$emit('change', current.description, current.valid);
                });

                this.calculateCharactersRemaining(current, editor);
            },
            calculateCharactersRemaining(current: any, editor: any) {
                const remaining = getRemainingCharactersFromHtml(current.maxLength, editor.getData());

                current.valid = remaining >= 0;
                current.hint = current.valid ? `You have ${getCharactersText(remaining)} remaining` : `You have ${getCharactersText(-1 * remaining)} too many`;
            },
            emitValidity() {
                this.$emit('inputValidity', this.valid);
            },
        }
    })
</script>