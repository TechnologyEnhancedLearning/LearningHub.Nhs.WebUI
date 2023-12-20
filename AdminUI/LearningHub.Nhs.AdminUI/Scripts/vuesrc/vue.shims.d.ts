// This file is required to allow us to use TypeScript in the .vue files.

declare module "*.vue" {
    import Vue from 'vue'
    export default Vue
}

declare module 'ckeditor4-vue/dist/legacy.js';