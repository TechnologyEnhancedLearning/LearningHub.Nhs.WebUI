<!--
    This component takes an additional "order" prop (unlike the other Contribute blocks) in order to recreate
    the CKEditor component when required. There is a bug using CKEditor 4 which causes the editor to disappear
    when reordering the component. To fix this, the height is saved and restored when recreating the component.
-->

<template>
    <div class="p-25" ref="ckeditor-container">
        <ckeditor v-model="textBlockContent" v-bind:config="ckEditorConfig" :key="order" @ready="restoreHeight"></ckeditor>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import _ from 'lodash';
    import CKEditorToolbar from '../models/ckeditorToolbar';
    import CKEditor from 'ckeditor4-vue/dist/legacy.js';
    import { TextBlockModel } from '../models/contribute-resource/blocks/textBlockModel';

    const getEditor = (el: HTMLElement): HTMLElement | undefined => el.getElementsByClassName('cke_contents')[0] as HTMLElement | undefined;

    const getPixelsString = (val: number) => `${val}px`;

    export default Vue.extend({
        props: {
            textBlock: { type: Object } as PropOptions<TextBlockModel>,
            order: Number,
        },
        components: {
            ckeditor: CKEditor.component,
        },
        data() {
            return {
                textBlockContent: this.textBlock.content,
                ckEditorConfig: { toolbar: CKEditorToolbar.default },
                prevHeight: undefined,
            };
        },
        created() {
            this.updateModelWithNewContent = _.debounce(this.updateModelWithNewContentDebounced, 1 * 1000, { 'maxWait': 10 * 1000 });
        },
        watch: {
            textBlockContent: {
                handler(newVal: string) {
                    this.updateModelWithNewContent(newVal);
                }
            },
            textBlock: {
                deep: true,
                handler() {
                    this.textBlockContent = this.textBlock.content;
                }
            },
            order: {
                handler() {
                    // Fix the height of the container while the CKEditor component is recreated, to prevent content jumping
                    const container = this.$refs["ckeditor-container"] as HTMLElement;
                    container.style.height = getPixelsString(container.clientHeight);

                    // Remember the height of the actual text box for when the CKEditor is recreated
                    this.prevHeight = getEditor(container)!.clientHeight;
                }
            }
        },
        methods: {
            updateModelWithNewContent(newContent: string) {
                // Replaced by debounced function in created()
            },
            updateModelWithNewContentDebounced(newContent: string) {
                this.textBlock.content = newContent;
            },
            restoreHeight() {
                if (typeof this.prevHeight !== 'undefined') {
                    // Unlock the height of CKEditor container so it can still be resized
                    const container = this.$refs["ckeditor-container"] as HTMLElement;
                    container.style.height = 'auto';

                    // Restore the height of the textbox 
                    getEditor(container)!.style.height = getPixelsString(this.prevHeight);
                    this.prevHeight = undefined;
                }
            }
        },
    });
</script>

<style lang="scss" scoped>
</style>