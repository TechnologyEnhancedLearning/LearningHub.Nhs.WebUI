<template>
    <div class="image-attachment d-flex align-items-center py-20 px-24 ">
        <div class="image-attachment--thumbnail">
            <picture>
                <img :src="iconUrl" :alt="iconAltText">
            </picture>
        </div>
        <div class="image-attachment--details">
            <FileInfo v-bind:file-model="file"/>
        </div>
        <div class="image-attachment--download">
            <a v-bind:href="downloadPath" v-bind:download="file.fileName">
                <IconButton iconClasses="fa-solid fa-download"
                            ariaLabel="Download attachment"
                            size="large"/>
            </a>
        </div>
    </div>
</template>

<script lang="ts">
import Vue, { PropOptions } from 'vue';

import { FileModel } from "../../../models/contribute-resource/files/fileModel";
import FileUploader from "../../../globalcomponents/FileUploader.vue";
import FileInfo from "./FileInfo.vue";
import IconButton from "../../../globalcomponents/IconButton.vue";
import {
    getImageAltText,
    getImageIconUrl,
} from "../../../helpers/attachmentTypeHelper";


export default Vue.extend({
    components: {
        IconButton,
        FileInfo,
    },
    props: {
        file: { type: Object } as PropOptions<FileModel>,
    },
    computed: {
        imageUrl(): string {
            return `/api/resource/DownloadResource`
                + `?filePath=${encodeURIComponent(this.file.filePath)}`
                + `&fileName=${encodeURIComponent(this.file.fileName)}`
        },
        downloadPath(): string {
            return this.file.getDownloadResourceLink();
        },
        iconUrl(): string {
            return getImageIconUrl();
        },
        iconAltText(): string {
            return getImageAltText();
        }
    },
})
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .image-attachment {
        width: min(100%,600px);
        flex: 1 1 0;
        max-height: 75px;
        border: 1px solid $nhsuk-grey-light;
        border-radius: 6px;

        @media screen and (-ms-high-contrast: active), (-ms-high-contrast: none)
        {
            width: 100%;
            max-width: 600px;
        }

        &--thumbnail {
            flex-shrink: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 30px;
            height: 30px;

            picture {
                img {
                    object-fit: contain;
                    margin: 0 auto;
                    display: block;
                }
            }
        }

        &--details {
            max-width: 756px;
            font-size: 16px;
            padding: 0 16px;
        }

        &--download {
            margin-left: auto;

            a i {
                font-size: 32px;
                font-weight: 200;
                color: $nhsuk-grey;
            }
        }
    }
    picture {
        img {
            object-fit: contain;
            margin: 0 auto;
            display: block;
            max-width: 100%;
            max-height: 600px;
        }
    }
</style>