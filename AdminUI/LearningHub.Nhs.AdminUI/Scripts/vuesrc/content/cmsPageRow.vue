<template>
    <div class="information-page__row" :style="getStyle">
        <div class="nhsuk-width-container app-width-container">

            <div v-if="pageSectionDetail && pageSectionDetail.sectionTitle" :class="[`nhsuk-grid-row ${section.topMargin ? 'information-page__row--padding-top' : '' }`]">
                <div class="nhsuk-grid-column-full">
                    <h1 v-if="pageSectionDetail.sectionTitleElement=='h1'">{{pageSectionDetail.sectionTitle}}</h1>
                    <h2 v-if="pageSectionDetail.sectionTitleElement=='h2'">{{pageSectionDetail.sectionTitle}}</h2>
                    <h3 v-if="pageSectionDetail.sectionTitleElement=='h3'">{{pageSectionDetail.sectionTitle}}</h3>
                    <h4 v-if="pageSectionDetail.sectionTitleElement=='h4'">{{pageSectionDetail.sectionTitle}}</h4>
                </div>
            </div>

            <div :class="[`nhsuk-grid-row  ${section.topMargin && (!pageSectionDetail || !pageSectionDetail.sectionTitle) ? 'information-page__row--padding-top' : '' } ${section.bottomMargin ? 'information-page__row--padding-bottom' : '' }`]">

                <div v-if="sectionTemplateType === SectionTemplateType.Video" class="nhsuk-grid-column-two-thirds">
                    <div class="information-page__video-text-container" :style="getTextBackbroundStyle">
                        <div v-html="getDescription" />
                    </div>
                    <div class="information-page__asset-container">
                        <div id="mediaContainer" :class="[`${disableVideoControl ? 'videoControlDisabled' : ''}`]" v-show="sectionTemplateType === SectionTemplateType.Video && displayAVFlag" class="w-100">
                            <!--<video controls v-show="section.id" :id="[`azureMediaPlayer${section.id}`]"
                                   data-setup='{"logo": { "enabled": false }, "techOrder": ["azureHtml5JS", "flashSS",  "silverlightSS", "html5"], "nativeControlsForTouch": false, "fluid": true}'
                                   class="azuremediaplayer amp-default-skin amp-big-play-centered" style="height:250px;">
                                <p class="amp-no-js">
                                    To view this media please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video
                                </p>
                            </video>-->
                            <div v-show="sectionTemplateType === SectionTemplateType.Video && displayAVFlag">
                                <div class="container" id="playerContainer">
                                    <div class="video-container" id="videoContainer"></div>
                                    <div class="controls">
                                        <div class="progress-bar-container" id="progressBarContainer">
                                            <div id="progressBar" class="progress-bar"></div>
                                        </div>
                                        <div class="control-buttons">
                                            <button @click="stopVideo"><i class="fas fa-stop"></i></button>
                                            <button @click="togglePlayPause">
                                                <i :class="isPlaying ? 'fas fa-pause' : 'fas fa-play'"></i>
                                            </button>
                                            <button @click="toggleMute">
                                                <i :class="isMuted ? 'fas fa-volume-off' : 'fas fa-volume-up'"></i>
                                            </button>
                                            <input type="range" min="0" max="100" step="1" v-model="volume">
                                            <span>{{ currentTime }}</span><span>/</span><span>{{ duration }}</span>
                                            <button @click="toggleFullscreen"><i class="fas fa-expand"></i></button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="information-page__asset-link-container" v-if="pageSectionDetail && pageSectionDetail.videoAsset && pageSectionDetail.videoAsset.transcriptFile" :style="getTextBackbroundStyle">
                                <a download :style="getLinkStyle" :href="[`/file/download/${pageSectionDetail.videoAsset.transcriptFile.filePath}/${pageSectionDetail.videoAsset.transcriptFile.fileName}`]">
                                    Download transcript
                                </a>
                            </div>
                        </div>
                    </div>
                    <div v-if="!displayAVFlag">
                        <div v-html="audioVideoUnavailableView"></div>
                    </div>
                </div>
                <div v-if="sectionTemplateType === SectionTemplateType.Image && section.imageAsset" class="nhsuk-grid-column-full">
                    <div :class="[`information-page__container ${section.sectionLayoutType == SectionLayoutType.Left ? 'information-page__container--reverse-child-order' : '' } ${section.hasBorder ? 'information-page__container--border' : '' }`]">
                        <div class="nhsuk-grid-column-one-half information-page__container-column">
                            <div class="information-page__asset-container">
                                <img v-if="sectionTemplateType == SectionTemplateType.Image && section.imageAsset"
                                     :alt="section.imageAsset.altTag"
                                     :src="[`/file/download/${section.imageAsset.imageFile.filePath}/${section.imageAsset.imageFile.fileName}`]"
                                     class="information-page__img">
                                <img v-if="sectionTemplateType == SectionTemplateType.Image && section.imageAsset == null"
                                     src="/images/generic_rectangle.png" class="information-page__img">
                            </div>
                        </div>
                        <div class="nhsuk-grid-column-one-half information-page__container-column">
                            <div class="information-page__text-container" :class="GetPaddingClass" :style="getTextBackbroundStyle">
                                <div v-html="getDescription" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { SectionTemplateType } from '../models/content/pageSectionModel';
    import { PageSectionDetailModel, SectionLayoutType, } from '../models/content/pageSectionDetailModel';
    import { contentData } from '../data/content';
    import { AzureMediaAssetModel } from '../models/content/videoAssetModel';
    import { MKPlayer } from '@mediakind/mkplayer';

    export default Vue.extend({
        props: {
            section: { Type: PageSectionDetailModel, required: true } as PropOptions<PageSectionDetailModel>,
            sectionTemplateType: { Type: SectionTemplateType, required: true } as PropOptions<SectionTemplateType>,
            isHidden: { Type: Boolean, required: true } as PropOptions<Boolean>
        },
        data() {
            return {
                SectionLayoutType: SectionLayoutType,
                SectionTemplateType: SectionTemplateType,
                pageSectionDetail: null as PageSectionDetailModel,
                disableVideoControl: false,
                displayAVFlag: false,
                audioVideoUnavailableView: '' as string,
                player: null,
                videoContainer: null,
                playerContainer: null,
                mkioKey: '',
                playBackUrl: '',
                sourceLoaded: true,
                azureMediaServicesToken: '',
            };
        },
        created() {
            this.load();
            this.getDisplayAVFlag();
            this.getAudioVideoUnavailableView();
            this.getMKIOPlayerKey();
            this.getMediaPlayUrl();
        },
        computed: {
            getStyle() {
                console.log("getLinkStyle", (this.pageSectionDetail || {}).draftHidden);
                let style = `background:${this.section.backgroundColour};color:${this.section.textColour}; a { color: ${this.section.hyperLinkColour} !important} `;
                if (this.pageSectionDetail && (this.pageSectionDetail.draftHidden !== null ? this.pageSectionDetail.draftHidden : this.isHidden)) {
                    style = 'opacity:0.5;' + style;
                }
                if (this.pageSectionDetail && this.pageSectionDetail.topMargin) {
                    style = 'padding-top:48px;' + style;
                }
                if (this.pageSectionDetail && this.pageSectionDetail.bottomMargin) {
                    style = 'padding-bottom:48px;' + style;
                }
                return style;
            },
            getLinkStyle() {
                console.log("getLinkStyle", (this.pageSectionDetail || {}).draftHidden);
                let style = `color: ${this.section.hyperLinkColour} !important;`;
                if (this.pageSectionDetail && (this.pageSectionDetail.draftHidden !== null ? this.pageSectionDetail.draftHidden : this.isHidden)) {
                    style = 'opacity:0.5; ' + style;
                }
                return style;
            },
            getTextBackbroundStyle() {
                let style = '';
                if (this.section.textBackgroundColour) {
                    style = `background-color: ${this.section.textBackgroundColour} !important;`;
                    if (this.pageSectionDetail && (this.pageSectionDetail.draftHidden !== null ? this.pageSectionDetail.draftHidden : this.isHidden)) {
                        style = 'opacity:0.5; ' + style;
                    }
                }
                return style;
            },
            GetPaddingClass() {
                let returnClass = '';
                if (!this.section.textBackgroundColour && !this.section.hasBorder) {
                    if (this.section.sectionLayoutType == SectionLayoutType.Left) {
                        returnClass = "information-page__text-container--no-padding-left";
                    } else {
                        returnClass = "information-page__text-container--no-padding-right";
                    }
                }
                return returnClass;
            },
            getDescription() {
                if (this.section.description) {
                    var tempDiv = $(`<div>${this.section.description}</div>`);
                    tempDiv.find("a").attr('style', `color:${this.section.hyperLinkColour} !important`);
                    return tempDiv.html();
                } else {
                    return "Description to be added";
                }
            },
            isRightSectionLayout() {
                return this.section.sectionLayoutType == SectionLayoutType.Right;
            },
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
                events: {
                    //error: this.onPlayerError,
                    //timechanged: this.onTimeChanged,
                    //muted: this.onMuted,
                    //unmuted: this.onUnmuted,
                    //ready: this.onPlayerReady,
                    //subtitleadded: this.onSubtitleAdded,
                    //playbackspeed: this.onPlaybackSpeed
                }
            };

            // Initialize the player with video container and player configuration
            this.player = new MKPlayer(this.videoContainer, playerConfig);

            // Load source
            const sourceConfig = {
                hls: "https://ep-defaultlhdev-mediakind02-dev-by-am-sl.uksouth.streaming.mediakind.com/d01d30e4-461f-4045-bc10-3c88a296f3af/manifest.ism/manifest(format=m3u8-cmaf,encryption=cbc)",
                //hls: this.playBackUrl,
                drm: {
                    clearkey: {
                        LA_URL: "HLS_AES",
                        headers: {
                            "Authorization": this.getBearerToken()
                        }
                    }
                }
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
            getDisplayAVFlag() {
                contentData.getDisplayAVFlag().then(response => {
                    this.displayAVFlag = response;
                });
            },
            getAudioVideoUnavailableView() {
                contentData.getAVUnavailableView().then(response => {
                    this.audioVideoUnavailableView = response;
                });
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
                    this.player.unload();
                }
            },
            getMKIOPlayerKey() {
                this.mkioKey = 'd0167b1c-9767-4287-9ddc-e0fa09d31e02';
            },
            getBearerToken() {
                debugger;
                return "Bearer=" + "";
            },
            getDRMConfig() {
                return {
                    drm: {
                        clearkey: {
                            LA_URL: "HLS_AES",
                            headers: {
                                "Authorization": "Bearer="
                            }
                        }
                    }
                };
            },           
            load() {
                if (this.sectionTemplateType === SectionTemplateType.Video) {
                    debugger;
                    contentData.getPageSectionDetailVideo(this.section.id).then(response => {
                        this.pageSectionDetail = response;


                        if (!this.pageSectionDetail.videoAsset)
                            return;
                        debugger
                        this.azureMediaServicesToken = this.pageSectionDetail.videoAsset.azureMediaAsset.authenticationToken;
                        this.playBackUrl = response.videoAsset.azureMediaAsset.locatorUri;
                        debugger
                        //const id = 'azureMediaPlayer' + this.pageSectionDetail.id;
                        //let azureMediaPlayer = amp(id);

                        if (this.pageSectionDetail.videoAsset.azureMediaAsset) {
                            //$(`#${id}`).css({ 'height': '', 'border': '1px solid #768692' });
                            this.disableVideoControl = false;
                        } else {
                            this.disableVideoControl = true;
                        }

                        //if (this.pageSectionDetail.videoAsset.thumbnailImageFile) {
                        //    azureMediaPlayer.poster(`/file/download/${this.pageSectionDetail.videoAsset.thumbnailImageFile.filePath}/${this.pageSectionDetail.videoAsset.thumbnailImageFile.fileName}`);
                        //}
                        //if (this.pageSectionDetail.videoAsset.azureMediaAsset && this.pageSectionDetail.videoAsset.closedCaptionsFile) {
                        //    azureMediaPlayer.src([{
                        //        type: "application/vnd.ms-sstr+xml",
                        //        src: this.pageSectionDetail.videoAsset.azureMediaAsset.locatorUri,
                        //        protectionInfo: [{ type: 'AES', authenticationToken: `Bearer=${this.pageSectionDetail.videoAsset.azureMediaAsset.authenticationToken}` }]
                        //    }],
                        //        [{ kind: "captions", src: `/file/download/${this.pageSectionDetail.videoAsset.closedCaptionsFile.filePath}/${this.pageSectionDetail.videoAsset.closedCaptionsFile.fileName}`, srclang: "en", label: "english" }]);
                        //}
                        //else if (this.pageSectionDetail.videoAsset.azureMediaAsset && !this.pageSectionDetail.videoAsset.closedCaptionsFile) {
                        //    azureMediaPlayer.src([{
                        //        type: "application/vnd.ms-sstr+xml",
                        //        src: this.pageSectionDetail.videoAsset.azureMediaAsset.locatorUri,
                        //        protectionInfo: [{ type: 'AES', authenticationToken: `Bearer=${this.pageSectionDetail.videoAsset.azureMediaAsset.authenticationToken}` }]
                        //    }]);
                        //}
                    });
                } else {
                    contentData.getPageSectionDetail(this.section.id).then(x => this.pageSectionDetail = x);
                }
            },
            getAESProtection(token: string): string {
                debugger;
                var aesProtectionInfo = '{"protectionInfo": [{"type": "AES", "authenticationToken":"Bearer=' + token + '"}], "streamingFormats":["SMOOTH","DASH"]}';
                return aesProtectionInfo;
            },
            getMediaAssetProxyUrl(azureMediaAsset: AzureMediaAssetModel): string {

                //let playBackUrl = azureMediaAsset.locatorUri;
                //playBackUrl = playBackUrl.substring(0, playBackUrl.lastIndexOf("manifest")) + "manifest(format=m3u8-aapl)";

                //let sourceUrl = "/Media/MediaManifest?playBackUrl=" + playBackUrl + "&token=" + azureMediaAsset.authenticationToken;

                return '';// return sourceUrl;
            },
            getMediaPlayUrl() {
                debugger;
                this.playBackUrl = this.pageSectionDetail.videoAsset.azureMediaAsset.locatorUri;
                this.playBackUrl = this.playBackUrl.substring(0, this.playBackUrl.lastIndexOf("manifest")) + "manifest(format=m3u8-cmaf,encryption=cbc)";

                this.playBackUrl = "https://ep-defaultlhdev-mediakind02-dev-by-am-sl.uksouth.streaming.mediakind.com/d01d30e4-461f-4045-bc10-3c88a296f3af/manifest.ism/manifest(format=m3u8-cmaf,encryption=cbc)"
            },
        },
        watch: {
            section() {
                this.load();
            }
        }
    })
</script>
<style type="text/css">
    .videoControlDisabled {
        pointer-events: none;
        opacity: 0.5;
    }
    .container {
        position: relative;
    }

    /* .video-container {
     height: 0;
     width: 100%;
     overflow: hidden;
     position: relative;
 }
 */
    .video {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
    }

    .controls {
        display: flex;
        flex-direction: column;
        align-items: center;
        position: relative;
        background-color: black;
        padding: 10px;
    }

        .controls #currentTime,
        .controls #timeSeparator,
        .controls #duration {
            color: white; /* Set text color to white */
        }

    .progress-bar-container {
        position: relative;
        width: 100%;
        height: 10px; /* Set height of progress bar */
        background-color: #777; /* Set default color of progress bar */
    }

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
        margin-top: 10px; /* Adjust spacing between progress bar and buttons */
    }

    .controls button {
        background: none;
        border: none;
        color: #fff;
        cursor: pointer;
        font-size: 14px;
    }

    .controls input[type="range"] {
        -webkit-appearance: none;
        width: 30%;
        background-color: transparent; /* Make the background transparent */
    }

        .controls input[type="range"]::-webkit-slider-thumb {
            -webkit-appearance: none;
            appearance: none;
            width: 20px; /* Set width of thumb */
            height: 20px; /* Set height of thumb */
            background-color: #fff; /* Set color of thumb */
            border-radius: 50%; /* Make thumb round */
            cursor: pointer;
            position: relative; /* Ensure position is relative */
            top: -8px; /* Adjust the top position to align thumb on top of the slider track */
        }

        .controls input[type="range"]::-webkit-slider-runnable-track {
            width: 100%;
            height: 4px; /* Set height of track */
            background-color: rgba(255, 255, 255, 0.5); /* Set color of track with transparency */
            border-radius: 2px; /* Make track slightly rounded */
        }

    #bitmovinplayer-video-videoContainer {
        width: 100%;
    }
</style>