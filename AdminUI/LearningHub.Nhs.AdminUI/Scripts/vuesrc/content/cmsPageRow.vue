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
                            <div class="video-container" :id="getPlayerUniqueId"></div>

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
    import { MKPlayerType, MKStreamType } from '../MKPlayerConfigEnum';

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
                mkioKey: '',
                isIphone: false
            };
        },
         async created(): Promise<void> {
            await this.getMKIOPlayerKey();
            this.load();
            this.getDisplayAVFlag();
            this.getAudioVideoUnavailableView();
        },
        mounted() {
            this.checkIfIphone();
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
            getPlayerUniqueId(): string {
                return `videoContainer_${this.section.id}`
            },
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
            onPlayerReady() {
                const videoElement = document.getElementById("bitmovinplayer-video-" + this.getPlayerUniqueId) as HTMLVideoElement;
                if (videoElement) {
                    videoElement.controls = true;
                }
            },
            async getMKIOPlayerKey(): Promise<void> {
                this.mkioKey = await contentData.getMKPlayerKey();
                //return this.mkioKey;
            },
            load() {
                if (this.sectionTemplateType === SectionTemplateType.Video) {
                    contentData.getPageSectionDetailVideo(this.section.id).then(response => {
                        this.pageSectionDetail = response;

                        if (!this.pageSectionDetail.videoAsset)
                            return;

                        // Grab the video container
                        this.videoContainer = document.getElementById(this.getPlayerUniqueId);

                        if(!this.mkioKey) {
                            this.getMKIOPlayerKey();
                        }

                        // Prepare the player configuration
                        const playerConfig = {
                            key: this.mkioKey,
                            ui: false,
                            playback: {
                                muted: false,
                                autoplay: false,
                                preferredTech: [{ player: MKPlayerType.Html5, streaming: MKStreamType.Hls }]   // to force the player to use html5 player instead of native on safari
                            },
                            theme: "dark",
                            events: {
                                ready: this.onPlayerReady,
                            }
                        };

                        // Initialize the player with video container and player configuration
                        this.player = new MKPlayer(this.videoContainer, playerConfig);

                        // Load source
                        const sourceConfig = {
                            hls: this.getMediaPlayUrl(this.pageSectionDetail.videoAsset.azureMediaAsset.locatorUri),
                            drm: {
                                clearkey: {
                                    LA_URL: "HLS_AES",
                                    headers: {
                                        "Authorization": this.getBearerToken(this.pageSectionDetail.videoAsset.azureMediaAsset.authenticationToken)
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

                        //const id = 'azureMediaPlayer' + this.pageSectionDetail.id;
                        //let azureMediaPlayer = amp(id);

                        //if (this.pageSectionDetail.videoAsset.azureMediaAsset) {
                        //    $(`#${id}`).css({ 'height': '', 'border': '1px solid #768692' });
                        //    this.disableVideoControl = false;
                        //} else {
                        //    this.disableVideoControl = true;
                        //}

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
                var aesProtectionInfo = '{"protectionInfo": [{"type": "AES", "authenticationToken":"Bearer=' + token + '"}], "streamingFormats":["SMOOTH","DASH"]}';
                return aesProtectionInfo;
            },
            getMediaAssetProxyUrl(azureMediaAsset: AzureMediaAssetModel): string {
                let playBackUrl = azureMediaAsset.locatorUri;
                playBackUrl = playBackUrl.substring(0, playBackUrl.lastIndexOf("manifest")) + "manifest(format=m3u8-aapl)";

                let sourceUrl = "/Media/MediaManifest?playBackUrl=" + playBackUrl + "&token=" + azureMediaAsset.authenticationToken;

                return sourceUrl;
            },
            getBearerToken(token: string): string {
                return "Bearer=" + token;
            },
            getMediaPlayUrl(url: string): string {
                let sourceUrl = url.substring(0, url.lastIndexOf("manifest")) + "manifest(format=m3u8-cmaf,encryption=cbc)";

                if (this.isIphone) {
                    var token = this.pageSectionDetail.videoAsset.azureMediaAsset.authenticationToken;
                    sourceUrl = "/Media/MediaManifest?playBackUrl=" + sourceUrl + "&token=" + token;
                }   

                return sourceUrl;
            },
            checkIfIphone() {
                const userAgent = navigator.userAgent || navigator.vendor;
                this.isIphone = /iPhone/i.test(userAgent);
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

    .video-container {
        height: 0;
        width: 100%;
        overflow: hidden;
        position: relative;
        padding-top: 56.25%; /* 16:9 aspect ratio */
        background-color: #000;
    }

    video {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
    }

        video[id^="bitmovinplayer-video"] {
            width: 100%;
        }
</style>