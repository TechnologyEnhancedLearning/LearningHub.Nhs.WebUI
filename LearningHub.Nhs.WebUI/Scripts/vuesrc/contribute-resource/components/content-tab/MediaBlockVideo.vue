<template>
    <div class="media-block-video d-flex">
        <div class="media-block-video--video-viewer p-10 d-flex justify-content-center align-items-center">
            <FileUploader v-if="showFileUploader"
                          v-bind:file="video.getFileModel()"
                          v-bind:fileCategory="FileUploadType.Video"
                          v-on:newFileId="newFileId"
            />
            <VideoPlayerContainer :video="video" v-on:updatePublishingStatus="updatePublishingStatus" />
        </div>
        <!-- 10161: Hide captions and transcripts for now -->
        <div class="media-block-video--subtitles" v-show="false">
            <div class="mb-40">
                <b>Transcript</b> (optional)
                <p>
                    Please upload a transcript file to support learners that need to use an 
                    alternative format of this resource.
                    This must be either a Word (.doc or .docx), PDF (.pdf) or Text (.txt) file.
                </p>
                <Button @click="" size="thin">Browse</Button>
                <div class="media-block-video--transcript-file">
                    <span>transcript.docx</span>
                    <i class="fas fa-times"></i>
                </div>
            </div>
            <div class="mb-40">
                <b>Closed captions</b> (optional)
                <p>
                    Please upload a closed captions file to support learners that need the 
                    audio displayed as text on this video. 
                    This must be a file that has a .VTT file extension.
                </p>
                <Button size="thin">Browse</Button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import Button from '../../../globalcomponents/Button.vue';
    import EditSaveFieldWithCharacterCount from '../../../globalcomponents/EditSaveFieldWithCharacterCount.vue';
    import FileUploader from '../../../globalcomponents/FileUploader.vue';
    import VideoPlayerContainer from '../VideoPlayerContainer.vue';

    import { VideoMediaModel } from '../../../models/contribute-resource/blocks/videoMediaModel';
    import { FileUploadType } from '../../../helpers/fileUpload';
    
    export default Vue.extend({
        components: {
            Button,
            EditSaveFieldWithCharacterCount,
            FileUploader,
            VideoPlayerContainer,
        },
        props: {
            video: { type: Object } as PropOptions<VideoMediaModel>
        },
        data() {
            return {
                FileUploadType: FileUploadType,
                showTranscriptUpload: false,
                showClosedCaptionsUpload: false,
            }
        },
        computed: {
            showFileUploader(): boolean {
                const fileUploadComplete = this.video
                    && this.video.getFileModel()
                    && this.video.getFileModel().isUploadComplete();

                return !fileUploadComplete;
            },
        },
        methods: {
            newFileId(fileId: number) {
                // A fileId is available from the FileUploader
                // Set the fileId on this VideoMediaModel
                this.video.setFileId(fileId);
            },
            updatePublishingStatus() {
                this.$emit('updatePublishingStatus');
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .media-block-video {
        &--video-viewer {
            flex-grow: 2;
            width: 67%;
            background-color: $nhsuk-grey-white;
        }

        &--subtitles {
            width: 33%;
            flex-grow: 1;
            font-size: 16px;
            padding: 0 20px;
            
            p {
                margin-bottom: 16px;
                line-height: 28px;
            }
        }

        &--transcript-file {
            background-color: $nhsuk-grey-white;
            padding: 10px;
            margin: 0;

            i {
                font-size: 12px;
                color: $nhsuk-red;
                vertical-align: middle;
            }
        }
    }
    picture {
        img {
            object-fit: contain;
            margin: 0 auto;
            display: block;
            max-width: 100%;
        }
    }
</style>