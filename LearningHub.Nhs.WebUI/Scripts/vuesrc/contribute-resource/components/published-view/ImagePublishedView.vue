<template>
    <div class="image-published-view">
        <picture v-if="displayImage">
            <img v-bind:src="imageUrl" v-bind:alt="image.altText">
        </picture>
        <MediaBlockImageAttachment v-else v-bind:file="imageFileModel" />
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { ImageModel } from "../../../models/contribute-resource/blocks/imageModel";
    import { FileModel } from "../../../models/contribute-resource/files/fileModel";
    import { isImageFileViewable } from "../../../helpers/attachmentTypeHelper";
    import MediaBlockImageAttachment from "../content-tab/MediaBlockImageAttachment.vue";
    
    export default Vue.extend({
        components: { MediaBlockImageAttachment },
        props: {
            image: { type: Object } as PropOptions<ImageModel>
        },
        computed: {
            imageFileModel(): FileModel {
                return this.image.getFileModel();
            },
            imageUrl(): string {
                return this.imageFileModel.getDownloadResourceLink();
            },
            displayImage() : boolean {
                return isImageFileViewable(this.imageFileModel.fileName);
            }
        },
    })
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .image-published-view {
        width: 100%;
        
        picture {
            img {
                object-fit: contain;
                display: block;
                max-width: 100%;
                max-height: 600px;
            }
        }
    }
</style>