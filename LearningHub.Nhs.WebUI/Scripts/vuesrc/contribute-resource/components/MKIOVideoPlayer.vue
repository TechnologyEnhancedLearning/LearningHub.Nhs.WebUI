<link href="~/css/mkplayer-ui.css" rel="stylesheet" asp-append-version="true" />
<template>
    <div>
        <div :id="getPlayerUniqueId" class="video-container"></div>
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
                playerConfig: {
                    key: "d0167b1c-9767-4287-9ddc-e0fa09d31e02", // Replace with your actual license key
                    //ui: false,
                    //hls: "https://ep-defaultlhdev-mediakind02-dev-by-am-sl.uksouth.streaming.mediakind.com/d01d30e4-461f-4045-bc10-3c88a296f3af/manifest.ism/manifest(format=m3u8-cmaf,encryption=cbc)",
                    //playback: {
                    //    muted: true,
                    //    autoplay: true,
                    //},
                    //drm: {
                    //        clearkey: {
                    //            LA_URL: "HLS_AES",
                    //            headers: {
                    //                "Authorization": "Bearer=" + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJMZWFybmluZ0h1YiIsImF1ZCI6IkxlYXJuaW5nSHViVXNlcnMifQ.gb6H6k4cazXCQHqWxPucV0fWdOlU3Y5rYahCNV8HB2Y"
                    //            }
                    //        }
                    //    },
                    //subtitles: [
                    //    {
                    //        src: 'https://bitdash-a.akamaihd.net/content/sintel/subtitles/subtitles_en.vtt', // VTT subtitle file URL
                    //        label: 'English', // Human-readable label for the track
                    //    },
                    //    // ... more subtitle track objects
                    //],
                    // Add other player configuration options here
                },
            };
        },
        created() {
            this.getMKIOPlayerKey();
            this.getMediaPlayUrl();
        },
        mounted() {
            // Grab the video container
            this.videoContainer = document.getElementById(this.getPlayerUniqueId);

            // Prepare the player configuration
            const playerConfig = {
                key: this.mkioKey,
                ui: false,
                theme: "dark",
                playback: {
                    muted: true,
                    autoplay: true,
                },
                tracks: [
                    {
                        kind: "captions",
                        src: "https://bitdash-a.akamaihd.net/content/sintel/subtitles/subtitles_en.vtt",
                        srclang: "en",
                        label: "English",
                    },
                ],
                subtitles: [
                    {
                        id: "sub1",
                        lang: "en",
                        label: "Custom Subtitle",
                        url: "https://bitdash-a.akamaihd.net/content/sintel/subtitles/subtitles_en.vtt",
                        kind: "subtitle"
                    }
                ],
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

            // Load source
            const sourceConfig = {
                hls: this.playBackUrl,
                drm: {
                    clearkey: {
                        LA_URL: "HLS_AES",
                        headers: {
                            "Authorization": this.getBearerToken()
                        }
                    }
                },
                playback: {
                    muted: false,
                    autoplay: false
                },
                tracks: [
                    {
                        src: 'https://bitdash-a.akamaihd.net/content/sintel/subtitles/subtitles_en.vtt', // VTT subtitle file URL
                        label: 'English', // Human-readable label for the track
                    },
                    // ... more subtitle track objects
                ],
            };

            this.player.load(sourceConfig)
                .then(() => {                  
                    console.log("Source loaded successfully!");
                })
                .catch(() => {
                    console.error("An error occurred while loading the source!");
                });
        },
        methods: {
            onPlayerReady() {
                const videoElement = document.getElementById("bitmovinplayer-video-" + this.getPlayerUniqueId) as HTMLVideoElement;
                if (videoElement) {
                    videoElement.controls = true;
                }
                //document.getElementById("bitmovinplayer-video-" + this.getPlayerUniqueId).controls = true;
               // const mkPlayer = MKPlayer.createPlayer(document.getElementById('bitmovinplayer-video-videoContainer'));
                //    console.log("Player is ready for playback!");
                // Add subtitles
                var subtitleTrack = {
                    id: "sub1",
                    lang: "en",
                    label: "Custom Subtitle",
                    url: "https://bitdash-a.akamaihd.net/content/sintel/subtitles/subtitles_en.vtt",
                    kind: "subtitle"
                };
                //  this.player.seek(10);
                this.player.addSubtitle(subtitleTrack);
            },
            onSubtitleAdded() {
                //const subtitleTrack = {
                //    id: "sub1",
                //    lang: "en",
                //    label: "Custom Subtitle",
                //    url: "https://bitdash-a.akamaihd.net/content/sintel/subtitles/subtitles_en.vtt",
                //    kind: "subtitle"
                //};
                //this.player.addSubtitle(subtitleTrack);
            },
            getMKIOPlayerKey() {
                this.mkioKey = 'd0167b1c-9767-4287-9ddc-e0fa09d31e02';
            },
            getBearerToken() {
                return "Bearer=" + this.azureMediaServicesToken;
            },
            getMediaPlayUrl() {
                this.playBackUrl = this.videoFile.locatorUri;
                this.playBackUrl = this.playBackUrl.substring(0, this.playBackUrl.lastIndexOf("manifest")) + "manifest(format=m3u8-cmaf,encryption=cbc)";
                // this.playBackUrl = "https://ep-defaultlhdev-mediakind02-dev-by-am-sl.uksouth.streaming.mediakind.com/d01d30e4-461f-4045-bc10-3c88a296f3af/manifest.ism/manifest(format=m3u8-cmaf,encryption=cbc)"
            },
        },
        computed: {
            getPlayerUniqueId(): string {
                // We need to generate a new unique ID for the video player on every render otherwise the video player will not load properly.
                // This is because the AMP is only intended to be a static player and so is not optimised for componentisation.
                // So, when we switch between tabs on the Case resource creation page, the video player unmounts and then remounts if
                // we go back to the Contents tab. As a result, a new unique ID is needed so that the media player knows it has to
                // initalise itself again, which we do in the `mounted()` lifecycle method above.
                return `videoContainer_${this.fileId}`
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
</style>
