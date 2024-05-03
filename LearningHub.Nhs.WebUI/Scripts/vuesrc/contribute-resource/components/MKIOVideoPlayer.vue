<template>
    <div class="video-player mt-5 border-bottom" ref="videoPlayer">
        <video :id="getAMPUniqueId"
               data-setup='{"logo": { "enabled": false }, "techOrder": ["azureHtml5JS", "flashSS",  "silverlightSS", "html5"], "nativeControlsForTouch": false}'
               class="azuremediaplayer amp-default-skin amp-big-play-centered"
               controls               
        >
            <source :src="videoFile.locatorUri" type="application/vnd.ms-sstr+xml" :data-setup='getAESProtection' />
            <source :src="getMediaAssetProxyUrl" type="application/vnd.apple.mpegurl" disableUrlRewriter="true" />
            <track v-if="captionsTrackAvailable" default srclang="en" kind="captions" label="english" :src="captionsUrl" />
            <p class="amp-no-js">
                To view this media please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video
            </p>
        </video>
        <div class="mt-3" v-if="transcriptAvailable">
            <a :href="transcriptUrl">Transcript available</a>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import { FileModel } from '../../models/contribute-resource/files/fileModel';
    import { VideoFileModel } from '../../models/contribute-resource/blocks/videoFileModel';
    import { EventBus } from '../contributeResourceEvents';

    export default Vue.extend({
        props: {
            fileId: Number,
            videoFile: { type: Object } as PropOptions<VideoFileModel>,
            azureMediaServicesToken: String,
        },
        mounted() {
            const options = {
                autoplay: false,
                controls: true,
            };
            
            // We have to make a call to `amp` otherwise the video player will not initialise on the page.
            // Add comment here about why we do it like this
            amp(this.getAMPUniqueId, options);
            
            window.addEventListener('resize', this.handleResize);
            this.handleResize();

            EventBus.$on('ContributeBlock.Expansion.Event', (val: boolean) => {
                if (val) {
                    setTimeout(() => this.handleResize(), 1);
                }
            })
        },
        beforeDestroy() {
            window.removeEventListener('resize', this.handleResize);
        },
        computed: {
            getAMPUniqueId(): string {
                // We need to generate a new unique ID for the video player on every render otherwise the video player will not load properly.
                // This is because the AMP is only intended to be a static player and so is not optimised for componentisation.
                // So, when we switch between tabs on the Case resource creation page, the video player unmounts and then remounts if
                // we go back to the Contents tab. As a result, a new unique ID is needed so that the media player knows it has to
                // initalise itself again, which we do in the `mounted()` lifecycle method above.
                const time = new Date().getTime()
                return `azuremediaplayer_${this.fileId}_${time}`
            },
            getAESProtection(): string {
                return '{"protectionInfo": [{"type": "AES", "authenticationToken":"Bearer=' + this.azureMediaServicesToken + '"}], "streamingFormats":["SMOOTH","DASH"]}';
            },
            getMediaAssetProxyUrl(): string {
                let playBackUrl = this.videoFile.locatorUri;
                playBackUrl = playBackUrl.substring(0, playBackUrl.lastIndexOf("manifest")) + "manifest(format=m3u8-aapl)";
                return "/Media/MediaManifest?playBackUrl=" + playBackUrl + "&token=" + this.azureMediaServicesToken;
            },
            captionsTrackAvailable(): boolean {
                return !!this.videoFile
                    && !!this.videoFile.captionsFile
                    && !!this.videoFile.captionsFile.getFileModel();
            },
            captionsFileModel(): FileModel {
                return this.videoFile.captionsFile.getFileModel();
            },
            captionsUrl(): string {
                return this.captionsFileModel.getDownloadResourceLink();
            },
            transcriptAvailable(): boolean {
                return !!this.videoFile
                    && !!this.videoFile.transcriptFile
                    && !!this.videoFile.transcriptFile.getFileModel();
            },
            transcriptFileModel(): FileModel {
                return this.videoFile.transcriptFile.getFileModel();
            },
            transcriptUrl(): string {
                return this.transcriptFileModel.getDownloadResourceLink();
            },
        },
        methods: {
            handleResize() {
                const videoPlayer = amp(this.getAMPUniqueId);
                if (videoPlayer) {
                    const container = (this.$refs.videoPlayer as HTMLElement).parentElement.parentElement;
                    const mediaBlockVideoWidth = parseInt(window.getComputedStyle(container, null).width.slice(0, -2));

                    // Maintain 16:9 ratio
                    let newHeight = mediaBlockVideoWidth * 0.5625;

                    if (newHeight > 0) {
                        videoPlayer.width(mediaBlockVideoWidth);
                        videoPlayer.height(newHeight);
                    }
                }
            },
        },
    })
</script>

<style lang="scss" scoped>
    .video-player {
        overflow-x: auto;
    }
</style>