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
                        <div id="mediaContainer" :class="[`${disableVideoControl ? 'videoControlDisabled' : ''}`]" v-show="sectionTemplateType === SectionTemplateType.Video && learnResourceAVFlag" class="w-100">
                            <video controls v-show="section.id" :id="[`azureMediaPlayer${section.id}`]"
                                   data-setup='{"logo": { "enabled": false }, "techOrder": ["azureHtml5JS", "flashSS",  "silverlightSS", "html5"], "nativeControlsForTouch": false, "fluid": true}'
                                   class="azuremediaplayer amp-default-skin amp-big-play-centered" style="height:250px;">
                                <p class="amp-no-js">
                                    To view this media please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video
                                </p>
                            </video>
                            <div class="information-page__asset-link-container" v-if="pageSectionDetail && pageSectionDetail.videoAsset && pageSectionDetail.videoAsset.transcriptFile" :style="getTextBackbroundStyle">
                                <a download :style="getLinkStyle" :href="[`/file/download/${pageSectionDetail.videoAsset.transcriptFile.filePath}/${pageSectionDetail.videoAsset.transcriptFile.fileName}`]">
                                    Download transcript
                                </a>
                            </div>
                        </div>
                    </div>
                    <div v-if="!learnResourceAVFlag">
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
                learnResourceAVFlag: false,
                audioVideoUnavailableView : '' as string,
            };
        },
        created() {
            this.load();
            this.getLearnResourceAVFlag();
            this.getAudioVideoUnavailableView();
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
        methods: {
            getLearnResourceAVFlag() {
                contentData.getLearnAVResourceFlag().then(response => {
                this.learnResourceAVFlag = response;
               });
            },
            getAudioVideoUnavailableView() {
                contentData.getAVUnavailableView().then(response => {
                this.audioVideoUnavailableView =  response;
                });
            },
            load() {
                if (this.sectionTemplateType === SectionTemplateType.Video) {
                    contentData.getPageSectionDetailVideo(this.section.id).then(response => {
                        this.pageSectionDetail = response;

                        if (!this.pageSectionDetail.videoAsset)
                            return;

                        const id = 'azureMediaPlayer' + this.pageSectionDetail.id;
                        let azureMediaPlayer = amp(id);

                        if (this.pageSectionDetail.videoAsset.azureMediaAsset) {
                            $(`#${id}`).css({ 'height': '', 'border': '1px solid #768692' });
                            this.disableVideoControl = false;
                        } else {
                            this.disableVideoControl = true;
                        }

                        if (this.pageSectionDetail.videoAsset.thumbnailImageFile) {
                            azureMediaPlayer.poster(`/file/download/${this.pageSectionDetail.videoAsset.thumbnailImageFile.filePath}/${this.pageSectionDetail.videoAsset.thumbnailImageFile.fileName}`);
                        }
                        if (this.pageSectionDetail.videoAsset.azureMediaAsset && this.pageSectionDetail.videoAsset.closedCaptionsFile) {
                            azureMediaPlayer.src([{
                                type: "application/vnd.ms-sstr+xml",
                                src: this.pageSectionDetail.videoAsset.azureMediaAsset.locatorUri,
                                protectionInfo: [{ type: 'AES', authenticationToken: `Bearer=${this.pageSectionDetail.videoAsset.azureMediaAsset.authenticationToken}` }]
                            }],
                                [{ kind: "captions", src: `/file/download/${this.pageSectionDetail.videoAsset.closedCaptionsFile.filePath}/${this.pageSectionDetail.videoAsset.closedCaptionsFile.fileName}`, srclang: "en", label: "english" }]);
                        }
                        else if (this.pageSectionDetail.videoAsset.azureMediaAsset && !this.pageSectionDetail.videoAsset.closedCaptionsFile) {
                            azureMediaPlayer.src([{
                                type: "application/vnd.ms-sstr+xml",
                                src: this.pageSectionDetail.videoAsset.azureMediaAsset.locatorUri,
                                protectionInfo: [{ type: 'AES', authenticationToken: `Bearer=${this.pageSectionDetail.videoAsset.azureMediaAsset.authenticationToken}` }]
                            }]);
                        }
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
</style>