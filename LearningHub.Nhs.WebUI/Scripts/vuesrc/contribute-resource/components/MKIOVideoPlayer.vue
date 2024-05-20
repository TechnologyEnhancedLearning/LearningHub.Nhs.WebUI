<link href="~/css/mkplayer-ui.css" rel="stylesheet" asp-append-version="true" />
<template>
    <div>
        <div class="container" id="playerContainer">
            <div class="video-container" id="videoContainer"></div>
            <!--<div class="controlsMkioPlayer">-->
                <!--<div class="progress-bar-container" id="progressBarContainer">
                    <div id="progressBar" class="progress-bar"></div>
                </div>-->
                <!--<div class="control-buttons">-->
                    <!--<button @click="stopVideo"><i class="fas fa-stop"></i></button>-->
                    <!--<button @click="togglePlayPause">
                        <i :class="isPlaying ? 'fas fa-pause' : 'fas fa-play'"></i>
                    </button>-->
                    <!--<button @click="togglePlayPause"><i :class="playPauseIcon"></i></button>
                    <button @click="toggleMute">
                        <i :class="isMuted ? 'fas fa-volume-off' : 'fas fa-volume-up'"></i>
                    </button>
                    <input type="range" min="0" max="100" step="1" v-model="volume">
                    <span>{{ currentTime }}</span><span>/</span><span>{{ duration }}</span>
                    <button @click="toggleFullscreen"><i class="fas fa-expand"></i></button>
                </div>
            </div>-->
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
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
                playerContainer: null,
                mkioKey: '',
                playBackUrl: '',
                playPauseIcon: 'fas fa-pause',
                sourceLoaded: true,
                //playerConfig: {
                //    key: "d0167b1c-9767-4287-9ddc-e0fa09d31e02", // Replace with your actual license key
                //    ui: true,
                //    playback: {
                //        muted: true,
                //        autoplay: true,
                //    },
                //    // Add other player configuration options here
                //},
            };
        },
        created() {
            this.getMKIOPlayerKey();
            this.getMediaPlayUrl();
        },
        mounted() {
            // Grab the video container
            this.videoContainer = document.getElementById('videoContainer');
            this.playerContainer = document.getElementById('playerContainer');


            //const videoContainer = this.$refs.videoContainer.value;

            // Prepare the player configuration
            const playerConfig = {
                key: this.mkioKey,
                ui: false,
                theme: "dark",
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
                    onpause: this.onpause,
                    onplay: this.onplay,
                    muted: this.onMuted,
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
                //hls: "https://ep-defaultlhdev-mediakind02-dev-by-am-sl.uksouth.streaming.mediakind.com/942c6a47-e940-4229-848e-2b1c97c6a2e9/RathamaareyMassTamilan.dev.ism/manifest(format=m3u8-cmaf,encryption=cbc)",
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
            //onPlayerError(event) {
            //    console.log("Encountered player error: ", JSON.stringify(event));
            //},
            //onTimeChanged(event) {
            //    console.log("Current player position: ", event.time);
            //},
            onpause() {
                this.playPauseIcon = 'fas fa-pause';
            },
            onplay() {
                this.playPauseIcon = 'fas fa-play';
            },
            onMuted() {
                this.playPauseIcon = 'fas fa-pause';
            },
            onUnmuted() {
                this.playPauseIcon = 'fas fa-play';
            },
            onPlayerReady() {
                debugger
                document.getElementById("bitmovinplayer-video-videoContainer").controls = true;
                //    console.log("Player is ready for playback!");
                // Add subtitles
                const subtitleTrack = {
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
                const subtitleTrack = {
                    id: "sub1",
                    lang: "en",
                    label: "Custom Subtitle",
                    url: "https://bitdash-a.akamaihd.net/content/sintel/subtitles/subtitles_en.vtt",
                    kind: "subtitle"
                };
                this.player.addSubtitle(subtitleTrack);
            },
            toggleFullscreen() {
                //if (!document.fullscreenElement && !document.webkitFullscreenElement && !document.mozFullScreenElement && !document.msFullscreenElement) {
                //    // Enter fullscreen mode
                //    if (this.playerContainer.requestFullscreen) {
                //        this.playerContainer.requestFullscreen();
                //    } else if (this.playerContainer.webkitRequestFullscreen) { /* Safari */
                //        this.playerContainer.webkitRequestFullscreen();
                //    } else if (this.playerContainer.msRequestFullscreen) { /* IE11 */
                //        this.playerContainer.msRequestFullscreen();
                //    }
                //} else {
                //    // Exit fullscreen mode
                //    if (document.exitFullscreen) {
                //        document.exitFullscreen();
                //    } else if (document.webkitExitFullscreen) { /* Safari */
                //        document.webkitExitFullscreen();
                //    } else if (document.msExitFullscreen) { /* IE11 */
                //        document.msExitFullscreen();
                //    }
                //}
            },
            togglePlayPause() {
                if (this.player) {
                    //if (!sourceLoaded) {
                    //    // load a new source to start playback
                    //    const sourceConfig = {
                    //        hls: "https://ep-defaultlhdev-mediakind02-dev-by-am-sl.uksouth.streaming.mediakind.com/d01d30e4-461f-4045-bc10-3c88a296f3af/manifest.ism/manifest(format=m3u8-cmaf,encryption=cbc)",
                    //        drm: {
                    //            clearkey: {
                    //                LA_URL: "HLS_AES",
                    //                headers: {
                    //                    "Authorization": "Bearer="
                    //                }
                    //            }
                    //        }
                    //    };
                    //    this.player.load(sourceConfig)
                    //        .then(() => {
                    //            sourceLoaded = true;
                    //            console.log("Source loaded successfully!");
                    //        })
                    //        .catch((error) => {
                    //            console.error(`Source load failed with error: ${JSON.stringify(error)}`);
                    //        });
                    //    return;
                    //}
                    // Handling for play/pause
                    this.player.isPlaying() ? this.player.pause() : this.player.play();
                    this.player.isPlaying() ? this.playPauseIcon = 'fas fa-pause' : this.playPauseIcon = 'fas fa-play';
                }
            },
            toggleMute() {
                if (this.player) {
                    if (this.player.isMuted()) {
                        this.player.unmute();
                    } else {
                        this.player.mute();
                    }
                }
            },
            stopVideo() {
                if (this.player && this.sourceLoaded) {
                    this.player.pause()
                    //this.player.unload();
                }
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
            //onPlaybackSpeed(data) {
            //    console.log("Video playback quality changed to: ", data.speed);
            //}
        },
        computed: {

        },
    })
</script>

<style>
    /*.container {
           position: relative;
       }*/

    /* .video-container {
        height: 0;
        width: 100%;
        overflow: hidden;
        position: relative;
    }
    */
    /*.video {
           position: absolute;
           top: 0;
           left: 0;
           width: 100%;
           height: 100%;
       }

       .controlsMkioPlayer {
           display: flex;
           flex-direction: column;
           align-items: center;
           position: relative;
           background-color: black;
           padding: 10px;
       }

           .controlsMkioPlayer #currentTime,
           .controlsMkioPlayer #timeSeparator,
           .controlsMkioPlayer #duration {
               color: white;*/ /* Set text color to white */
    /*}

       .progress-bar-container {
           position: relative;
           width: 100%;
           height: 10px;*/ /* Set height of progress bar */
    /*background-color: #777;*/ /* Set default color of progress bar */
    /*}

       .progress-bar-container {
           width: 100%;
           height: 4px;
           background-color: #777;
           z-index: 1;
       }

       .progress-bar {
           height: 100%;
           width: 0;
           background-color: #4caf50;
       }

       .control-buttons {
           display: flex;
           justify-content: space-between;
           width: 100%;
           margin-top: 10px;*/ /* Adjust spacing between progress bar and buttons */
    /*}

       .controlsMkioPlayer button {
           background: none;
           border: none;
           color: #fff;
           cursor: pointer;
           font-size: 14px;
       }

       .controlsMkioPlayer input[type="range"] {
           -webkit-appearance: none;
           width: 30%;
           background-color: transparent;*/ /* Make the background transparent */
    /*}

           .controlsMkioPlayer input[type="range"]::-webkit-slider-thumb {
               -webkit-appearance: none;
               appearance: none;
               width: 20px;*/ /* Set width of thumb */
    /*height: 20px;*/ /* Set height of thumb */
    /*background-color: #fff;*/ /* Set color of thumb */
    /*border-radius: 50%;*/ /* Make thumb round */
    /*cursor: pointer;
               position: relative;*/ /* Ensure position is relative */
    /*top: -8px;*/ /* Adjust the top position to align thumb on top of the slider track */
    /*}

           .controlsMkioPlayer input[type="range"]::-webkit-slider-runnable-track {
               width: 100%;
               height: 4px;*/ /* Set height of track */
    /*background-color: rgba(255, 255, 255, 0.5);*/ /* Set color of track with transparency */
    /*border-radius: 2px;*/ /* Make track slightly rounded */
    /*}*/

    #bitmovinplayer-video-videoContainer {
        width: 100%;
    }
</style>
