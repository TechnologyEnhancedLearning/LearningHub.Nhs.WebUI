<template>
    <div class="contribute-media-block">
        <MediaBlockAttachment v-if="mediaBlock.mediaType === MediaTypeEnum.Attachment"
                              v-on:updatePublishingStatus="updatePublishingStatus"
                              v-bind:attachment="attachment" />
        <MediaBlockImage v-if="mediaBlock.mediaType === MediaTypeEnum.Image"
                         v-on:updatePublishingStatus="updatePublishingStatus"
                         v-bind:image="image" />

        <MediaBlockVideo v-if="mediaBlock.mediaType === MediaTypeEnum.Video && contributeResourceAVFlag"
                         v-on:updatePublishingStatus="updatePublishingStatus"
                         v-bind:video="video" />

        <div v-if="!contributeResourceAVFlag && (mediaBlock.mediaType === MediaTypeEnum.Video || mediaBlock.mediaType === MediaTypeEnum.Attachment)">
            <div v-html="audioVideoUnavailableView"></div>
        </div>        
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import FileInfo from "./FileInfo.vue";
    import { FileStore } from "../../../models/contribute-resource/files/fileStore";
    import FileUploader from "../../../globalcomponents/FileUploader.vue";
    import MediaBlockAttachment from "./MediaBlockAttachment.vue";
    import MediaBlockImage from "./MediaBlockImage.vue";
    import MediaBlockVideo from "./MediaBlockVideo.vue";
    import Tick from "../../../globalcomponents/Tick.vue";
    import { resourceData } from '../../../data/resource';

    import { AttachmentModel } from "../../../models/contribute-resource/blocks/attachmentModel";
    import { FileUploadType } from '../../../helpers/fileUpload';
    import { ImageModel } from "../../../models/contribute-resource/blocks/imageModel";
    import { MediaBlockModel } from "../../../models/contribute-resource/blocks/mediaBlockModel";
    import { MediaTypeEnum } from "../../../models/contribute-resource/blocks/mediaTypeEnum";
    import { VideoMediaModel } from "../../../models/contribute-resource/blocks/videoMediaModel";

    export default Vue.extend({
        components: {
            FileInfo,
            FileUploader,
            MediaBlockAttachment,
            MediaBlockImage,
            MediaBlockVideo,
            Tick
        },
        props: {
            mediaBlock: { type: Object } as PropOptions<MediaBlockModel>,
        },
        data() {
            return {
                FileUploadType: FileUploadType,
                FileStore: FileStore /* It looks like FileStore isn't used, but we need it to be exposed here to allow Vue to make the files list reactive */,
                MediaTypeEnum: MediaTypeEnum,
                contributeResourceAVFlag: true
            }
        },
        created() {
            this.getContributeResAVResourceFlag();
        },
        computed: {
            attachment(): AttachmentModel {
                return this.mediaBlock.attachment;
            },
            image(): ImageModel {
                return this.mediaBlock.image;
            },
            video(): VideoMediaModel {
                return this.mediaBlock.video;
            },
            audioVideoUnavailableView(): string {
                return this.$store.state.getAVUnavailableView;
            }
        },
        methods: {
            updatePublishingStatus() {
                this.mediaBlock.updatePublishingStatus();
            },
            getContributeResAVResourceFlag() {
                resourceData.getContributeAVResourceFlag().then(response => {
                    this.contributeResourceAVFlag = response;
                });
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .contribute-media-block {
        margin: 25px;
    }
</style>