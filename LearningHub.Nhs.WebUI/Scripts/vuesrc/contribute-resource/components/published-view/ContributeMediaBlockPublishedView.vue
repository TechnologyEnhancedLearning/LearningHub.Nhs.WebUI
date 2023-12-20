<template>
    <div class="contribute-media-block">

        <AttachmentPublishedView v-if="mediaType === MediaTypeEnum.Attachment" v-bind:attachment="attachment"/>
        <ImagePublishedView v-if="mediaType === MediaTypeEnum.Image" v-bind:image="image"/>
        <VideoPlayerContainer v-if="mediaType === MediaTypeEnum.Video" :video="video"/>

    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import FileInfo from '../content-tab/FileInfo.vue';
    import AttachmentPublishedView from './AttachmentPublishedView.vue';
    import ImagePublishedView from './ImagePublishedView.vue';
    import VideoPlayerContainer from '../VideoPlayerContainer.vue';

    import { AttachmentModel } from '../../../models/contribute-resource/blocks/attachmentModel';
    import { MediaBlockModel } from '../../../models/contribute-resource/blocks/mediaBlockModel';
    import { MediaTypeEnum } from '../../../models/contribute-resource/blocks/mediaTypeEnum';
    import { ImageModel } from '../../../models/contribute-resource/blocks/imageModel';
    import { VideoMediaModel } from '../../../models/contribute-resource/blocks/videoMediaModel';
    import { FileStore } from '../../../models/contribute-resource/files/fileStore';

    export default Vue.extend({
        components: {
            FileInfo,
            AttachmentPublishedView,
            ImagePublishedView,
            VideoPlayerContainer,
        },
        props: {
            mediaBlock: { type: Object } as PropOptions<MediaBlockModel>,
        },
        data() {
            return {
                FileStore: FileStore,
                MediaTypeEnum: MediaTypeEnum,
            }
        },
        created() {
            // We poll for files (e.g. videos) that did not finish processing when the Resource was being created.
            // So, when the file processing succeeds/fails, this is can be reflected in the published view.
            this.FileStore.enablePolling();
        },
        computed: {
            mediaType(): MediaTypeEnum {
                return this.mediaBlock?.mediaType;
            },
            attachment(): AttachmentModel {
                return this.mediaBlock.attachment;
            },
            image(): ImageModel {
                return this.mediaBlock.image;
            },
            video(): VideoMediaModel {
                return this.mediaBlock.video;
            }
        },
    })
</script>