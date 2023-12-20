<template>
    <div>
        <VideoPlayer v-if="isVideoReadyToShow"
                     :fileId="video.getFileModel().fileId"
                     :videoFile="video.getFileModel().videoFile"
                     :azureMediaServicesToken="azureMediaServicesAuthToken"
        />
        <div v-if="isVideoBeingProcessed">
            <p>
                <Spinner></Spinner>
                <span>Your video is being processed...</span>
            </p>
        </div>
        <div v-if="isVideoBeingFetched">
            <p>
                <Spinner></Spinner>
                <span>Fetching your video...</span>
            </p>
        </div>
        <p v-if="video.getFileModel().videoFile.processingErrorMessage" v-html="video.getFileModel().videoFile.processingErrorMessage"></p>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    
    import Spinner from '../../globalcomponents/Spinner.vue';
    import VideoPlayer from './VideoPlayer.vue';
    
    import { VideoMediaModel } from '../../models/contribute-resource/blocks/videoMediaModel';
    
    import { resourceData } from '../../data/resource';
    
    export default Vue.extend({
        components: {
            Spinner,
            VideoPlayer,
        },
        props: {
            video: { type: Object } as PropOptions<VideoMediaModel>
        },
        async created() {
            // This allows us to immediately fetch videos that are already processed and ready to be displayed
            await this.setAuthTokenIfVideoIsProcessed();
        },
        async updated() {
            // This allows us to live fetch videos once they have been processed after being uploaded
            await this.setAuthTokenIfVideoIsProcessed();
        },
        data() {
            return {
                azureMediaServicesAuthToken: null as string,
            }
        },
        computed: {
            isVideoBeingProcessed(): boolean {
                return !this.video.getFileModel().videoFile.azureAssetOutputFilePath &&
                    !this.video.getFileModel().hasOngoingFileUpload() &&
                    this.video.getFileModel().isUploadComplete();
            },
            isVideoBeingFetched(): boolean {
                return this.video.getFileModel().videoFile.azureAssetOutputFilePath &&
                    !this.azureMediaServicesAuthToken &&
                    !this.video.getFileModel().hasOngoingFileUpload() &&
                    this.video.getFileModel().isUploadComplete();
            },
            isVideoReadyToShow(): boolean {
                return !!this.azureMediaServicesAuthToken;
            }
        },
        methods: {
            async setAuthTokenIfVideoIsProcessed(): Promise<void> {
                if (this.video.getFileModel().isUploadComplete() && this.video.getFileModel().videoFile.azureAssetOutputFilePath && !this.azureMediaServicesAuthToken) {
                    this.azureMediaServicesAuthToken = await resourceData.getVideoFileAuthToken(this.video.getFileModel().videoFile.azureAssetOutputFilePath);
                    this.$emit('updatePublishingStatus');
                }
            },
        }
    })
</script>
