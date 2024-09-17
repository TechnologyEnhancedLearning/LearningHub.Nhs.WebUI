<link href="~/css/mkplayer-ui.css" rel="stylesheet" asp-append-version="true" />
<template>
    <div>
        <div :id="getPlayerUniqueId" class="video-container"></div>
        <noscript>
            <p class="amp-no-js">
                To view this media please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video
            </p>
        </noscript>
    </div>

    <!--<div class="video-player mt-5 border-bottom" ref="videoPlayer">
        <video :id="getAMPUniqueId"
               data-setup='{"logo": { "enabled": false }, "techOrder": ["azureHtml5JS", "flashSS",  "silverlightSS", "html5"], "nativeControlsForTouch": false}'
               class="azuremediaplayer amp-default-skin amp-big-play-centered"
               controls>
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
    </div>-->
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { FileModel } from '../../models/contribute-resource/files/fileModel';
    import { VideoFileModel } from '../../models/contribute-resource/blocks/videoFileModel';
    import { MKPlayer } from '@mediakind/mkplayer';
    import { resourceData } from '../../data/resource';
    import { MKPlayerType, MKStreamType } from '../../MKPlayerConfigEnum';
    import { MKPlayerControlbar } from '../../mkioplayer-controlbar';
    //import { MKPlayerType } from '@mediakind/mkplayer/types/enums/MKPlayerType.d';
    //import { MKStreamType } from '@mediakind/mkplayer/types/enums/MKStreamType.d';
    export default Vue.extend({
        props: {
            fileId: Number,
            videoFile: { type: Object } as PropOptions<VideoFileModel>,
            azureMediaServicesToken: String,
        },
        data() {
            return {
                player: null,
                videoContainer: null,
                mkioKey: '',
                playBackUrl: '',
                sourceLoaded: true,
            };
        },
        async created() {
            await this.getMKIOPlayerKey();
            this.getMediaPlayUrl();
            this.load();
        },
        methods: {
            onPlayerReady() {
                MKPlayerControlbar(this.player.videoContainer.id, this.player);

                // [BY] When we set UI to false we need to manually add the controls to the video element
                //const videoElement = document.getElementById("bitmovinplayer-video-" + this.getPlayerUniqueId) as HTMLVideoElement;

                //if (videoElement) {
                //    videoElement.controls = true;

                //    // Add the track element
                //    var captionsInfo = this.captionsTrackAvailable;
                //    if (captionsInfo) {
                //        const trackElement = document.createElement('track');
                //        var srcPath = this.captionsUrl;
                //        trackElement.kind = 'captions';
                //        trackElement.label = 'english';
                //        trackElement.srclang = 'en';
                //        trackElement.src = srcPath;

                //        // Append the track to the video element
                //        videoElement.appendChild(trackElement);
                //    }
                //}
            },
            onSubtitleAdded() {

            },
            async getMKIOPlayerKey(): Promise<void> {
                this.mkioKey = await resourceData.getMKPlayerKey();
            },
            getBearerToken() {
                return "Bearer=" + this.azureMediaServicesToken;
            },
            getMediaPlayUrl() {
                this.playBackUrl = this.videoFile.locatorUri;
                this.playBackUrl = this.playBackUrl.substring(0, this.playBackUrl.lastIndexOf("manifest")) + "manifest(format=m3u8-cmaf,encryption=cbc)";
            },
            load() {
                // Grab the video container
                this.videoContainer = document.getElementById(this.getPlayerUniqueId);

                if (!this.mkioKey) {
                    this.getMKIOPlayerKey();
                }

                // Prepare the player configuration
                const playerConfig = {
                    key: this.mkioKey,
                    ui: true,
                    theme: "dark",
                    playback: {
                        muted: false,
                        autoplay: false,
                        preferredTech: [{ player: MKPlayerType.Html5, streaming: MKStreamType.Hls }]   // to force the player to use html5 player instead of native on safari
                    },
                    events: {
                        //error: this.onPlayerError,
                        //timechanged: this.onTimeChanged,
                        //onpause: this.onpause,
                        //onplay: this.onplay,
                        //muted: this.onMuted,
                        //unmuted: this.onUnmuted,
                        ready: this.onPlayerReady,
                        subtitleadded: this.onSubtitleAdded,

                        //playbackspeed: this.onPlaybackSpeed
                    }
                };

                // Initialize the player with video container and player configuration
                this.player = new MKPlayer(this.videoContainer, playerConfig);

                // ClearKey DRM configuration
                var clearKeyConfig = {
                    //LA_URL: "https://ottapp-appgw-amp.prodc.mkio.tv3cloud.com/drm/clear-key?ownerUid=azuki",
                    LA_URL: "HLS_AES",
                    headers: {
                        "Authorization": this.getBearerToken()
                    }
                };

                // Load source
                const sourceConfig = {
                    hls: this.playBackUrl,
                    drm: {
                        clearkey: clearKeyConfig
                    }
                };

                this.player.load(sourceConfig)
                    .then(() => {
                        console.log("Source loaded successfully!");
                    })
                    .catch(() => {
                        console.error("An error occurred while loading the source!");
                    });
            }
        },
        computed: {
            getPlayerUniqueId(): string {
                // We need to generate a new unique ID for the video player on every render otherwise the video player will not load properly.
                // This is because the AMP is only intended to be a static player and so is not optimised for componentisation.
                // So, when we switch between tabs on the Case resource creation page, the video player unmounts and then remounts if
                // we go back to the Contents tab. As a result, a new unique ID is needed so that the media player knows it has to
                // initalise itself again, which we do in the `mounted()` lifecycle method above.
                const time = new Date().getTime()
                return `videoContainer_${this.fileId}_${time}`
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
    })
</script>

<style>
    .video-player {
        overflow-x: auto;
    }

    video[id^="bitmovinplayer-video"] {
        width: 100%;
    }

    .bmpui-ui-controlbar .control-right {
        float: right;
    }
</style>

<style scoped>
    /* Base styles for video container */
    .video-container {
        width: 100%;
        margin: auto;
        position: relative;
        --min-width: 0px; /* default value */
    }

    /* Media queries to set different min-width values */
    @media (min-width: 576px) {
        .video-container {
            --min-width: 576px;
        }
    }

    @media (min-width: 768px) {
        .video-container {
            --min-width: 768px;
        }
    }

    @media (min-width: 992px) {
        .video-container {
            --min-width: 992px;
        }
    }

    @media (min-width: 1024px) {
        .video-container {
            --min-width: 1024px;
        }
    }

    /* Applying min-width to the video container using the CSS variable */
    /*.video-container {
        min-width: var(--min-width) !important;
    }*/

    /* Targeting specific div with dynamic ID pattern */
    [id^="videoContainer_"] {
        min-width: var(--min-width) !important; /* Inheriting min-width */
    }

    /* Example child element inheriting min-width from video container */
    .video-container .child-element {
        min-width: var(--min-width) !important;
        padding: 10px;
        text-align: center;
    }

    /* Style for the video element */
    .video-container video {
        width: 100%;
        height: auto;
    }
</style>
