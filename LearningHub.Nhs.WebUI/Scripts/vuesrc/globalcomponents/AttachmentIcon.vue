<template>
    <picture>
        <img :src="iconUrl" :alt="altText">
    </picture>
</template>

<script lang="ts">
    import Vue from 'vue';

    import {
        getAltText,
        getAttachmentTypeByExtension,
        getIconUrl
    } from "../helpers/attachmentTypeHelper";

    import { AttachmentTypeEnum } from "../models/contribute-resource/files/attachmentTypeEnum";
    
    export default Vue.extend({
        props: {
            fileExtension: String,
        },
        data() {
            return {
                AttachmentTypeEnum: AttachmentTypeEnum,
            }
        },
        computed: {
            attachmentType(): AttachmentTypeEnum {
                return getAttachmentTypeByExtension(this.fileExtension);
            },
            iconUrl(): string {
                return getIconUrl(this.attachmentType);
            },
            altText(): string {
                return getAltText(this.attachmentType);
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    picture {
        img {
            object-fit: contain;
            margin: 0 auto;
            display: block;
        }
    }
</style>