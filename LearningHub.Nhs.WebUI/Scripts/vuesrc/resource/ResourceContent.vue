<template>
    <div v-show="resourceLoaded">
        <div v-if="resourceItem.id > 0 && !resourceItem.catalogue.hidden && !(resourceItem.versionStatusEnum === VersionStatus.UNPUBLISHED && !resourceItem.displayForContributor)">
            <div class="resource-main lh-container-xl">

                <!-- Content Object -->
                <div v-show="hasCatalogueAccess">

                    <!--Video-->
                    <div id="mediacontainer" class="resource-item nhsuk-u-margin-bottom-7" v-if="hasMediaAccess && (resourceItem.resourceTypeEnum === ResourceType.VIDEO)">

                        <!--<video id="resourceAzureMediaPlayer" data-setup='{"logo": { "enabled": false }, "techOrder": ["azureHtml5JS", "flashSS",  "silverlightSS", "html5"], "nativeControlsForTouch": false}' controls class="azuremediaplayer amp-default-skin amp-big-play-centered resource-video nhsuk-u-margin-bottom-4">
                            <source :src="resourceItem.videoDetails.resourceAzureMediaAsset.locatorUri" type="application/vnd.ms-sstr+xml" :data-setup='getAESProtection(resourceItem.videoDetails.resourceAzureMediaAsset.authenticationToken)' />
                            <source :src="getMediaAssetProxyUrl(resourceItem.videoDetails.resourceAzureMediaAsset)" type="application/vnd.apple.mpegurl" disableUrlRewriter="true" />
                            <track v-if="resourceItem.videoDetails.closedCaptionsFile" default srclang="en" kind="captions" label="english" :src="'/api/resource/DownloadResource?filePath=' +  resourceItem.videoDetails.closedCaptionsFile.filePath + '&fileName=' +  resourceItem.videoDetails.closedCaptionsFile.fileName" />
                            <p class="amp-no-js">
                                To view this media please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video
                            </p>
                        </video>-->
                        <div id="resourceMediaPlayer" class="video-container"></div>
                        <noscript>
                            <p class="amp-no-js">
                                To view this media please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video
                            </p>
                        </noscript>

                        <div v-if="resourceItem.videoDetails && resourceItem.videoDetails.transcriptFile">
                            <p class="nhsuk-u-margin-bottom-7">
                                <a :href="getFileLink(resourceItem.videoDetails.transcriptFile.filePath, resourceItem.videoDetails.transcriptFile.fileName)">Download transcription</a>
                            </p>
                        </div>
                    </div>

                    <!--Audio-->
                    <div id="mediacontainer" data-setup='{"logo": { "enabled": false }, "techOrder": ["azureHtml5JS", "flashSS",  "silverlightSS", "html5"], "nativeControlsForTouch": false}' controls class="resource-item" v-if="hasMediaAccess && (resourceItem.resourceTypeEnum == ResourceType.AUDIO)">

                        <!--<video id="resourceAzureMediaPlayer" controls="controls" data-setup='{}' class="azuremediaplayer amp-default-skin amp-big-play-centered resource-video nhsuk-u-margin-bottom-7">
                            <source :src="resourceItem.audioDetails.resourceAzureMediaAsset.locatorUri" type="application/vnd.ms-sstr+xml" :data-setup='getAESProtection(resourceItem.audioDetails.resourceAzureMediaAsset.authenticationToken)' />
                            <source :src="getMediaAssetProxyUrl(resourceItem.audioDetails.resourceAzureMediaAsset)" type="application/vnd.apple.mpegurl" disableUrlRewriter="true" />
                            <p class="amp-no-js">
                                To view this media please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video
                            </p>
                        </video>-->
                        <div id="resourceMediaPlayer" class="video-container"></div>
                        <noscript>
                            <p class="amp-no-js">
                                To view this media please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video
                            </p>
                        </noscript>
                        <div v-if="resourceItem.audioDetails && resourceItem.audioDetails.transcriptFile">
                            <p class="nhsuk-u-margin-bottom-7">
                                <a :href="getFileLink(resourceItem.audioDetails.transcriptFile.filePath, resourceItem.audioDetails.transcriptFile.fileName)">Download transcription</a>
                            </p>
                        </div>
                    </div>

                    <!-- Scorm -->
                    <div v-if="hasScormAccess">
                        <div class="nhsuk-card nhsuk-bg-light-blue">
                            <div class="nhsuk-card__content">
                                <button class="nhsuk-button" @click="launchScorm()">
                                    Play elearning resource
                                </button>
                                <p>This resource will launch in a new window</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!--Case or Assessment Content-->
            <div class="resource-panel-container" v-if="hasCatalogueAccess && hasCaseAssessmentAccees">
                <div class="resource-item-row">
                    <div class="col-12 d-flex flex-column align-items-left p-0">
                        <CaseOrAssessmentResource :resourceItem="resourceItem" :resourceActivityId="launchedResourceActivityId" :assessmentProgress="assessmentProgress" :keepUserSessionAliveIntervalSeconds="keepUserSessionAliveIntervalSeconds" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Vuelidate from "vuelidate";
    import '../filters';
    import { resourceData } from '../data/resource';
    import { userData } from '../data/user';
    import { VersionStatus, ResourceType, ResourceAccessibility, ActivityStatus, MediaResourceActivityType, AzureMediaPlayerOptions, RoleEnum } from '../constants';
    import { ResourceItemModel } from '../models/resourceItemModel';
    import { ScormContentDetailsModel } from '../models/scormModel';
    import { RoleUserGroupModel } from '../models/roleUserGroupModel';
    import { InteractionQueueModel } from '../models/interactionQueueModel';
    import { ResourceAzureMediaAssetModel } from "../models/resourceAzureMediaAssetModel";
    import { commonlib } from '../common';
    import { activityRecorder } from '../activity';
    import CaseOrAssessmentResource from './CaseOrAssessmentResource.vue';
    import { assessmentResourceHelper } from './helpers/assessmentResourceHelper';
    import { AssessmentProgressModel } from '../models/mylearning/assessmentProgressModel';
    import { getQueryParam } from './helpers/getQueryParam';
    import { setResourceCetificateLink } from './helpers/resourceCertificateHelper';
    import { MKPlayer } from '@mediakind/mkplayer';

    Vue.use(Vuelidate as any);

    export default Vue.extend({
        components: {
            CaseOrAssessmentResource,
        },
        props: {
            userAuthenticated: Boolean,
            mediaActivityPlayingEventIntervalSeconds: Number,
            keepUserSessionAliveIntervalSeconds: Number,
        },
        data(): any {
            return {
                resourceItem: new ResourceItemModel(),
                resourceLoaded: false,
                ResourceType: ResourceType,
                ResourceAccessibility: ResourceAccessibility,
                launchedResourceActivityId: 0,
                mediaResourceActivityId: 0,
                activityLogged: false,
                activityEndLogged: false, // Applies to media only.
                mediaResourceActivityLogged: false,
                VersionStatus: VersionStatus,
                commonlib: commonlib,
                roleUserGroups: [] as RoleUserGroupModel[],
                mediaPlayingTimer: 0,
                isFirstPlay: true,
                mediaStartTime: 0,
                createFirstInteraction: true,
                createdFirstInteraction: false,
                interactionQueue: new Array<InteractionQueueModel>(),
                scormContentDetailsModel: new ScormContentDetailsModel(),
                assessmentProgress: null as AssessmentProgressModel,
                isGeneralUser: false,
                isSystemAdmin: false,
                initialCertificateStatus: null,
                player: null,
                videoContainer: null,
                mkioKey: '',
                playBackUrl: '',
                playBackDashUrl: '',
                sourceLoaded: true,
                playerConfig: {
                }
            }
        },
        computed: {
            hasCatalogueAccess(): boolean {
                return (!this.resourceItem.catalogue.restrictedAccess || this.IsSystemAdmin
                    ||
                    (this.roleUserGroups.filter((rug: any) => rug.catalogueNodeId == this.resourceItem.catalogue.nodeId
                        && (rug.roleEnum == RoleEnum.LocalAdmin || rug.roleEnum == RoleEnum.Editor || rug.roleEnum == RoleEnum.Reader)).length > 0)
                );
            },
            hasMediaAccess(): boolean {
                return this.userAuthenticated && (this.resourceItem.resourceTypeEnum == this.ResourceType.AUDIO || this.resourceItem.resourceTypeEnum == this.ResourceType.VIDEO) && (!(this.isGeneralUser && this.resourceItem.resourceAccessibilityEnum == this.ResourceAccessibility.FullAccess))
            },
            hasScormAccess(): boolean {
                return this.userAuthenticated && this.resourceItem.resourceTypeEnum == this.ResourceType.SCORM && (!(this.isGeneralUser && this.resourceItem.resourceAccessibilityEnum == this.ResourceAccessibility.FullAccess))
            },
            hasCaseAssessmentAccees(): boolean {
                return this.userAuthenticated && (this.resourceItem.resourceTypeEnum == this.ResourceType.CASE || this.resourceItem.resourceTypeEnum == this.ResourceType.ASSESSMENT) && (!(this.isGeneralUser && this.resourceItem.resourceAccessibilityEnum == this.ResourceAccessibility.FullAccess))
            },
            isVideoReadyToShow(): boolean {
                return true;// this.azureMediaServicesAuthToken;
            }
        },
        async created(): Promise<void> {
            this.resourceLoaded = false;
            if (this.$route.query.mediaStartTime) {
                this.mediaStartTime = parseInt(this.$route.query.mediaStartTime as string);
            }
            await this.getGeneralUser();
            await this.getUserRole();
            await this.loadResourceItem(Number(this.$route.params.resId));

            if (this.userAuthenticated && this.resourceItem.catalogue.restrictedAccess) {
                await this.loadRoleUserGroups();
            }

            if (this.userAuthenticated && (this.resourceItem.resourceTypeEnum == ResourceType.VIDEO || this.resourceItem.resourceTypeEnum == ResourceType.AUDIO) && this.hasResourceAccess()) {
                await this.getMKIOPlayerKey();
                await this.getMediaPlayUrl();
                await this.setupMKPlayer();
                this.addMediaEventListeners();
                //this.checkForAutoplay();
            }

            if (this.userAuthenticated && (this.resourceItem.resourceTypeEnum === ResourceType.VIDEO || this.resourceItem.resourceTypeEnum === ResourceType.AUDIO) && this.hasResourceAccess()) {
                window.addEventListener('resize', this.handleResize);
                this.handleResize();
            }

            // call complete activity if resource type is media or scorm.
            if (this.userAuthenticated && (this.resourceItem.resourceTypeEnum === ResourceType.VIDEO || this.resourceItem.resourceTypeEnum === ResourceType.AUDIO) && this.hasResourceAccess()) {
                //var isIE11 = (!!window.MSInputMethodContext && !!((<any>document).documentMode));
                var isIE11 = (!!((window as any).MSInputMethodContext) && !!((document as any).documentMode));
                var self = this;

                if (isIE11) {
                    // IE11 doesn't handle this being async, it always prompts for user confirmation when navigating away.
                    // Making the function non-async seems to fix it.
                    window.addEventListener("beforeunload", event => { self.recordActivityComplete(); delete event['returnValue']; });
                } else {
                    // pagehide for iphone/ipad. beforeunload for others.
                    // iphone pagehide HAS to use sendbeacon. Sendbeacon is used for all devices now.
                    window.addEventListener("pagehide", async event => { await this.recordActivityComplete(); }, false);
                    window.addEventListener("beforeunload", async event => { await self.recordActivityComplete(); }, false);
                }
            }

            if (this.userAuthenticated && this.resourceItem.resourceTypeEnum === ResourceType.SCORM) {
                resourceData.getScormContentDetails(this.resourceItem.resourceVersionId).then(response => {
                    this.$set(this.scormContentDetailsModel, 'externalReferenceId', response.externalReferenceId);
                    this.$set(this.scormContentDetailsModel, 'esrLinkType', response.esrLinkType);
                    this.$set(this.scormContentDetailsModel, 'contentFilePath', response.contentFilePath);
                    this.$set(this.scormContentDetailsModel, 'externalReference', response.externalReference);
                    this.$set(this.scormContentDetailsModel, 'manifestUrl', response.manifestUrl);
                    this.$set(this.scormContentDetailsModel, 'hostedContentUrl', response.hostedContentUrl);
                    this.$set(this.scormContentDetailsModel, 'isOwnerOrEditor', response.isOwnerOrEditor);
                });
            }

            this.resourceLoaded = true;
        },
        beforeDestroy(): void {
            window.clearInterval(this.mediaPlayingTimer);
        },
        mounted() {

        },
        methods: {
            onPlayerReady() {

                const videoElement = document.getElementById("bitmovinplayer-video-resourceMediaPlayer") as HTMLVideoElement;
                if (videoElement) {
                    videoElement.controls = true;

                    // Add the track element
                    var captionsInfo = this.resourceItem.videoDetails.closedCaptionsFile;
                    if (captionsInfo) {
                        const trackElement = document.createElement('track');
                        var srcPath = this.getFileLink(captionsInfo.filePath, captionsInfo.fileName);
                        trackElement.kind = 'captions'; // Or 'subtitles' or 'descriptions' depending on your track type
                        trackElement.label = captionsInfo.language || 'english';
                        trackElement.srclang = captionsInfo.language || 'en';
                        trackElement.src = srcPath;

                        // Append the track to the video element
                        videoElement.appendChild(trackElement);
                    }
                }

                this.checkForAutoplay(this.player);
            },
            onpause() {
                this.recordMediaResourceActivityInteraction(this.player, "pause");
            },
            onplay() {
                this.recordMediaResourceActivityInteraction(this.player, "play");
            },
            onplaybackfinished() {
                this.recordMediaResourceActivityInteraction(this.player, "ended");
            },
            onSubtitleAdded() {
                // this.player.subtitles.enable("subid");
            },
            setupMKPlayer() {
                // Grab the video container
                this.videoContainer = document.getElementById("resourceMediaPlayer");

                // Prepare the player configuration
                const playerConfig = {
                    key: this.mkioKey,
                    ui: false,
                    theme: "dark",
                    playback: {
                        muted: false,
                        autoplay: false,
                    },
                    events: {
                        //error: this.onPlayerError,
                        //timechanged: this.onTimeChanged,
                        paused: this.onpause,
                        play: this.onplay,
                        playbackfinished: this.onplaybackfinished,
                        //muted: this.onMuted,
                        ////unmuted: this.onUnmuted,
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
                    LA_URL:"HLS_AES",
                    headers: {
                        "Authorization": this.getBearerToken()
                    }
                };

                // Load source
                const sourceConfig = {
                    hls: this.playBackUrl,
                    //dash: this.playBackDashUrl,
                    drm: {
                        clearkey: clearKeyConfig
                    }
                };

                // Load source
                //const sourceConfig = {
                //    source: {
                //        options: [
                //            //{
                //            //    type: 'application/vnd.apple.mpegurl',
                //            //    src: getMediaAssetProxyUrl(resourceItem.videoDetails.resourceAzureMediaAsset),
                //            //    drm: {
                //            //        clearkey: clearKeyConfig
                //            //    }
                //            //},
                //            {
                //                type: 'application/vnd.ms-sstr+xml',
                //                src: this.playBackUrl,
                //                drm: {
                //                    clearkey: clearKeyConfig
                //                }
                //            }
                //        ]
                //    }
                //};


                this.player.load(sourceConfig)
                    .then(() => {
                        console.log("Source loaded successfully!");
                    })
                    .catch(() => {
                        console.error("An error occurred while loading the source!");
                    });


            },
            async getMKIOPlayerKey() {
                this.mkioKey = await resourceData.getMKPlayerKey();
            },
            getBearerToken() {
                var token;
                if (this.resourceItem.resourceTypeEnum === ResourceType.AUDIO) {
                    token = this.resourceItem.audioDetails.resourceAzureMediaAsset.authenticationToken;
                }
                else {
                    token = this.resourceItem.videoDetails.resourceAzureMediaAsset.authenticationToken;
                }
                return "Bearer=" + token;
            },
            getMediaPlayUrl() {
                if (this.resourceItem.resourceTypeEnum === ResourceType.AUDIO) {
                    this.playBackUrl = this.resourceItem.audioDetails.resourceAzureMediaAsset.locatorUri;
                    this.playBackDashUrl = this.resourceItem.audioDetails.resourceAzureMediaAsset.locatorUri;
                } else {
                    this.playBackUrl = this.resourceItem.videoDetails.resourceAzureMediaAsset.locatorUri;
                    this.playBackDashUrl = this.resourceItem.videoDetails.resourceAzureMediaAsset.locatorUri;
                }
                this.playBackUrl = this.playBackUrl.substring(0, this.playBackUrl.lastIndexOf("manifest")) + "manifest(format=m3u8-cmaf,encryption=cbc)";
            },
            getMediaAssetProxyUrl(playBackUrl: string): string {
                playBackUrl = playBackUrl.substring(0, playBackUrl.lastIndexOf("manifest")) + "manifest(format=mpd-time-cmaf,encryption=cenc)";
                return playBackUrl;
            },
            initialise(): void {
                // record activity on page created for resource article
                if (this.userAuthenticated && this.resourceItem.resourceTypeEnum === ResourceType.CASE) {
                    this.recordActivityLaunched();
                }
                else if (this.userAuthenticated && this.resourceItem.resourceTypeEnum === ResourceType.ASSESSMENT) {
                    this.getCurrentAssessmentActivity();
                }
                else if (this.resourceItem.resourceTypeEnum === ResourceType.AUDIO || this.resourceItem.resourceTypeEnum === ResourceType.VIDEO) {
                    window.setTimeout(this.handleResize, 100);
                }
            },
            checkForAutoplay(player: MKPlayer): void {
                // Autoplay the media if the mediaStartTime has been set. Happens if user came to screen via a "Play" link on My Learning progress dialog.
                if ((this.resourceItem.resourceTypeEnum === ResourceType.VIDEO || this.resourceItem.resourceTypeEnum === ResourceType.AUDIO) && this.mediaStartTime > 0) {

                    // Remove the mediaStartTime query string parameter so that if the user refreshes the page the video plays from the beginning.
                    this.$router.replace({ query: Object.assign({}, this.$route.query, { mediaStartTime: undefined }) });

                    //[BY]
                    //let resourceAzureMediaPlayer = amp("resourceAzureMediaPlayer");
                    //resourceAzureMediaPlayer.play();
                    if (player) {
                        player.play();
                        player.seek(this.mediaStartTime);
                    }
                }
            },
            async loadResourceItem(id: number): Promise<void> {
                this.resourceItem = await resourceData.getItem(id);
                await this.checkUserCertificateAvailability();
                this.initialise();
            },
            async loadRoleUserGroups(): Promise<void> {
                this.roleUserGroups = await userData.getRoleUserGroups();
            },
            async getGeneralUser(): Promise<void> {
                this.isGeneralUser = await userData.isGeneralUser();
            },
            async getUserRole(): Promise<void> {
                this.IsSystemAdmin = await userData.IsSystemAdmin();
            },

            hasResourceAccess(): boolean {
                return this.userAuthenticated && (!(this.isGeneralUser && this.resourceItem.resourceAccessibilityEnum == this.ResourceAccessibility.FullAccess))
            },
            async recordActivityLaunched(): Promise<void> {

                if (!this.activityLogged) {
                    this.activityLogged = true;
                    await activityRecorder.recordActivityLaunched(this.resourceItem.resourceTypeEnum, this.resourceItem.resourceVersionId, this.resourceItem.nodePathId, new Date())
                        // await activityRecorder.recordActivityLaunched(this.resourceItem.resourceVersionId, this.resourceItem.nodePathId, new Date())
                        .then(response => {
                            this.launchedResourceActivityId = response.createdId;
                            this.checkUserCertificateAvailability();
                        })
                        .catch(e => {
                            console.log(e);
                            window.location.pathname = './Home/Error';
                        });
                }
            },
            getAssessmentProgress(): Promise<AssessmentProgressModel> {
                const assessmentResourceActivityId = parseInt(getQueryParam('attempt'));
                if (assessmentResourceActivityId && !isNaN(assessmentResourceActivityId)) {
                    return assessmentResourceHelper.getAssessmentProgressFromActivity(assessmentResourceActivityId);
                }
                return assessmentResourceHelper.getAssessmentProgressFromResourceVersion(this.resourceItem.resourceVersionId);
            },
            async getCurrentAssessmentActivity(): Promise<void> {
                if (!this.activityLogged) {
                    this.activityLogged = true;
                    const assessmentProgress = await this.getAssessmentProgress();
                    if (assessmentProgress === null) {
                        this.activityLogged = false;
                        await this.recordActivityLaunched();
                    } else {
                        this.assessmentProgress = assessmentProgress;
                    }
                }
            },
            async recordActivity(activityStart: Date, activityEnd: Date, activityStatus: ActivityStatus): Promise<void> {
                if (!this.activityLogged) {
                    this.activityLogged = true;

                    await activityRecorder.recordActivity(this.resourceItem.resourceVersionId, this.resourceItem.nodePathId, activityStart, activityEnd, activityStatus)
                        .then(response => {
                            this.launchedResourceActivityId = response.createdId;
                            this.checkUserCertificateAvailability();
                        })
                        .catch(e => {
                            console.log(e);
                            window.location.pathname = './Home/Error';
                        });
                }
            },
            async recordActivityComplete(): Promise<void> {
                if (this.activityLogged && this.launchedResourceActivityId > 0 && !this.activityEndLogged) {
                    this.activityEndLogged = true;
                    var completeDate = new Date();

                    // If media is still playing, we need to record the end of the activity AND a pause media interaction all in one call.
                    if (this.resourceItem.resourceTypeEnum === ResourceType.VIDEO || this.resourceItem.resourceTypeEnum === ResourceType.AUDIO) {
                        //let resourceAzureMediaPlayer = amp("resourceAzureMediaPlayer");
                        if (!this.player.isPaused()) {

                            let currentMediaTime = this.player.getCurrentTime();
                            let clientDateTime = new Date();

                            await activityRecorder.recordActivityAndInteractionTogether(
                                // Activity params:
                                this.resourceItem.resourceVersionId,
                                this.resourceItem.nodePathId,
                                null,
                                completeDate,
                                ActivityStatus.Completed,
                                this.launchedResourceActivityId,
                                true, // Sendbeacon is used for all devices now.
                                // Interaction params:
                                this.mediaResourceActivityId,
                                currentMediaTime,
                                MediaResourceActivityType.Pause,
                                clientDateTime
                            );
                            return;
                        }
                    }

                    await activityRecorder.recordActivity(this.resourceItem.resourceVersionId, this.resourceItem.nodePathId, null, completeDate, ActivityStatus.Completed, this.launchedResourceActivityId, this.mediaResourceActivityId, true);
                }
            },
            async recordMediaResourceActivity(): Promise<void> {
                if (this.launchedResourceActivityId > 0 && !this.mediaResourceActivityLogged) {
                    this.mediaResourceActivityLogged = true;

                    await activityRecorder.recordMediaResourceActivity(this.launchedResourceActivityId, new Date())
                        .then(response => {
                            this.mediaResourceActivityId = response.createdId;
                        })
                        .catch(e => {
                            console.log(e);
                            window.location.pathname = './Home/Error';
                        });
                }
            },
            async recordMediaResourceActivityInteraction(player: MKPlayer, event: string): Promise<void> {
                // let resourceAzureMediaPlayer = amp("resourceAzureMediaPlayer");
                // If user has come via the 'Play' link in My Learning, set the video start time.
                if (this.isFirstPlay && this.mediaStartTime > 0) {
                    this.isFirstPlay = false;
                    player.seek(this.mediaStartTime);
                }

                // On iPhone/iPad, if the user uses the browser back/forward buttons to come to the page the browser doesn't reload it from scratch. So we have to reset the tracking variables to create a whole new activity session.
                if (this.activityEndLogged) {
                    this.activityLogged = false;
                    this.activityEndLogged = false;
                    this.mediaResourceActivityLogged = false;
                    this.launchedResourceActivityId = undefined;
                    this.createFirstInteraction = true;
                    this.createdFirstInteraction = false;
                }

                let currentMediaTime = player.getCurrentTime();
                let clientDateTime = new Date();

                // If this is the first interaction, we need to create the ResourceActivity first, then the MediaResourceActivity, then the MediaResourceActivityInteraction.
                // Any further interactions can't be recorded until this first one has finished being recorded, but they cannot be lost!
                if (this.createFirstInteraction) {
                    this.createFirstInteraction = false;
                    this.interactionQueue.push(new InteractionQueueModel({ mediaResourceActivityType: event, clientDateTime: clientDateTime, currentMediaTime: currentMediaTime }));
                    await this.recordActivityLaunched();
                    await this.recordMediaResourceActivity();
                    await this.createQueuedInteractions();
                    this.createdFirstInteraction = true;
                }
                else if (!this.createdFirstInteraction) {
                    // If more interactions take place whilst the above is still in the process of being recorded (i.e. this event handler gets triggered again), we need to queue the new
                    // interactions to be created in the last step of the above code block.
                    this.interactionQueue.push(new InteractionQueueModel({ mediaResourceActivityType: event, clientDateTime: clientDateTime, currentMediaTime: currentMediaTime }));
                }
                else {
                    // Otherwise the top level activities have already been created, so just go ahead and create the a new interaction.
                    this.createInteraction(event, clientDateTime, currentMediaTime);
                }
            },

            async createQueuedInteractions(): Promise<void> {
                while (this.interactionQueue.length > 0) {
                    let interaction: InteractionQueueModel = this.interactionQueue.shift();
                    await this.createInteraction(interaction.mediaResourceActivityType, interaction.clientDateTime, interaction.currentMediaTime);
                }
            },
            async createInteraction(interactionType: string, clientDateTime: Date, currentMediaTime: number): Promise<void> {
                let mediaResourceActivityType = null;

                if (interactionType == "play") {
                    mediaResourceActivityType = MediaResourceActivityType.Play;
                    this.mediaPlayingTimer = window.setInterval(this.recordMediaPlayingEvent, this.mediaActivityPlayingEventIntervalSeconds * 1000);
                }
                else if (interactionType == "pause") {
                    window.clearInterval(this.mediaPlayingTimer);

                    // ignore pause event if fired by ended event
                    if (this.hasReachedEndOfMedia()) {
                        return;
                    }

                    mediaResourceActivityType = MediaResourceActivityType.Pause;
                }
                else if (interactionType == "ended") {
                    window.clearInterval(this.mediaPlayingTimer);

                    mediaResourceActivityType = MediaResourceActivityType.End;
                }

                if (mediaResourceActivityType) {
                    await activityRecorder.recordMediaResourceActivityInteraction(this.mediaResourceActivityId, currentMediaTime, mediaResourceActivityType, clientDateTime)
                        .catch(e => {
                            console.log(e);
                            window.location.pathname = './Home/Error';
                        });
                }

                if (interactionType == "ended") {
                    setTimeout(() => { this.checkUserCertificateAvailability() }, 10000);
                    await this.recordActivityComplete();
                }
            },
            async recordMediaPlayingEvent(): Promise<void> {
                let currentMediaTime = this.player.getCurrentTime();// this.getMediaPlayerDisplayTime();
                let clientDateTime = new Date();

                await activityRecorder.recordMediaResourceActivityInteraction(this.mediaResourceActivityId, currentMediaTime, MediaResourceActivityType.Playing, clientDateTime)
                    .catch(e => {
                        console.log(e);
                        window.location.pathname = './Home/Error';
                    });
            },
            getMediaPlayerDisplayTime(): number {
                // On iPhone, 'amp("resourceAzureMediaPlayer").currentMediaTime()' returns undefined for some reason.
                // So dig the value out of the player's internal properties instead.
                let resourceAzureMediaPlayer = amp("resourceAzureMediaPlayer");
                let isOnIPhone = navigator.userAgent.match(/iPhone/i);
                if (isOnIPhone) {
                    return (amp("resourceAzureMediaPlayer") as any).cache_.currentTime;
                }
                else {
                    return resourceAzureMediaPlayer.currentMediaTime();
                }
            },
            hasReachedEndOfMedia(): boolean {
                // Round the duration to nearest second. The Azure media player current media time at the end of the video is not always equal to the duration
                // extracted from AMS when the resource was published. So cannot do a direct compare. Rounding the numbers is the only way around this.
                // Also, it's not worth using the duration property of the Azure media player because the value returned is inconsistent. For videos of .0 to .5 of
                // a second in length, it returns the duration as an exact value, but for those .5 to .99 in length it returns the duration rounded up to the
                // next whole number. Also, on iPhone the duration is always returned as a rounded down number.
                let duration: number;
                if (this.resourceItem.resourceTypeEnum === ResourceType.VIDEO) {
                    duration = Math.round(this.resourceItem.videoDetails.durationInMilliseconds / 1000);
                }
                else if (this.resourceItem.resourceTypeEnum === ResourceType.AUDIO) {
                    duration = Math.round(this.resourceItem.audioDetails.durationInMilliseconds / 1000);
                }

                let currentMediaTime = Math.round(this.player.getCurrentTime());
                return currentMediaTime == duration;
            },
            handleResize() {
                if ($('#mediacontainer .resource-video').length > 0) {
                    var mediaElement = $('#mediacontainer .resource-video').first();

                    // keep 16:9 ratio
                    let height = mediaElement.width() * 0.5625;
                    let fontSize = mediaElement.css('font-size');

                    if (height > 0 && fontSize === '0px') {
                        let newFontSize = Math.trunc(height * 1.778);
                        mediaElement.attr('style', 'font-size: ' + newFontSize + 'px');
                        mediaElement.width('100%');
                    }
                    mediaElement.height(height);
                }
            },
            addMediaEventListeners(): void {
                //let resourceAzureMediaPlayer = amp("resourceAzureMediaPlayer", AzureMediaPlayerOptions);

                //resourceAzureMediaPlayer.addEventListener(amp.eventName.play, this.recordMediaResourceActivityInteraction);
                //resourceAzureMediaPlayer.addEventListener("pause", this.recordMediaResourceActivityInteraction);
                //resourceAzureMediaPlayer.addEventListener("ended", this.recordMediaResourceActivityInteraction);
                //$(resourceAzureMediaPlayer).bind("fullscreenchange", this.handleResize);
            },
            getFileLink(filePath: string, fileName: string): string {
                return "/api/resource/DownloadResource?filePath=" + filePath + "&fileName=" + encodeURIComponent(fileName);
            },
            downloadResource(filePath: string, fileName: string): void {
                let downloadLocation = this.getFileLink(filePath, fileName);
                window.open(downloadLocation);
            },
            //getMediaAssetProxyUrl(azureMediaAsset: ResourceAzureMediaAssetModel): string {

            //    let playBackUrl = azureMediaAsset.locatorUri;
            //    playBackUrl = playBackUrl.substring(0, playBackUrl.lastIndexOf("manifest")) + "manifest(format=m3u8-aapl)";

            //    return "/Media/MediaManifest?playBackUrl=" + playBackUrl + "&token=" + azureMediaAsset.authenticationToken;
            //},
            getAESProtection(token: string): string {
                return '{"protectionInfo": [{"type": "AES", "authenticationToken":"Bearer=' + token + '"}], "streamingFormats":["SMOOTH","DASH"]}';
            },
            async launchScorm() {
                var targetWin;
                var targetWinName = "lhContent" + this.resourceItem.resourceId;

                var activeContent = await userData.getActiveContent();

                if (activeContent.filter(ac => ac.resourceId === this.resourceItem.resourceId).length > 0) {
                    targetWin = window.open("", targetWinName, "location=0,menubar=0,resizable=0,width=" + this.resourceItem.scormDetails.popupWidth + ",height=" + this.resourceItem.scormDetails.popupHeight);
                    if (targetWin.location.href == "about:blank") {
                        targetWin.close();
                        alert('Error: This Scorm content is already active!\r\nIf you cannot find the active content window, then please clear this message, log out of the Hub and log back in to Play this session.')
                    }
                    else {
                        targetWin.focus();
                    }
                }
                else if (activeContent.length > 0) {
                    alert('Error: You already have active Scorm content!\r\nIf you cannot find the active content window, then please clear this message, log out of the Learning Hub and log back in to Play this session.');
                }
                else {
                    targetWin = window.open("/LearningSessions/Scorm/" + this.$route.params.resId, targetWinName, "location=0,menubar=0,resizable=0,width=" + this.resourceItem.scormDetails.popupWidth + ",height=" + this.resourceItem.scormDetails.popupHeight);
                    targetWin.focus();
                }
            },
            async checkUserCertificateAvailability() {
                if (this.initialCertificateStatus == null) {
                    this.initialCertificateStatus = await resourceData.userHasResourceCertificate(this.$route.params.resId);
                }
                else if (this.userAuthenticated && this.resourceItem.certificateEnabled && this.initialCertificateStatus == false) {
                    let check = await resourceData.userHasResourceCertificate(this.$route.params.resId);
                    if (check == true) {
                        this.initialCertificateStatus = true;
                        setResourceCetificateLink(this.$route.params.resId);
                    }
                }

            },
        }
    })

</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    // From Resource.vue:

    #mediacontainer video, .resource-item {
        width: 100% !important;
        max-width: 800px !important;
        background-color: none;
    }

    .resource-main {
        word-wrap: break-word;
    }

    #mediacontainer video {
        height: auto !important;
        width: 100%;
        min-height: 190px;
    }

    .resource-item-row {
        margin-top: 32px;
    }

    .border-bottom {
        border-bottom: 1px solid $nhsuk-grey-lighter;
        padding-bottom: 20px;
    }

    .scorm-download-container {
        width: 100%;
        height: 88px;
        background: #212B32;
        margin-top: 0px;
        padding-top: 24px;
        vertical-align: middle;

        @media(max-width: 414px) {
            padding-top: 16px;
            width: 100%;
            height: 53px;
        }
    }

    // From ResourceItem.vue:

    .azuremediaplayer {
        height: 450px !important;
    }

    @media (max-width: 768px) {
        .azuremediaplayer {
            height: 387px !important;
        }
    }

    @media (max-width: 576px) {
        .azuremediaplayer {
            height: 216px !important;
        }
    }

    .scorm-bg-image {
        min-width: 360px;
    }

    .btn-green {
        max-height: 40px;
        font-size: 18px;
    }

    .btn-green:hover {
        max-height: 40px;
    }

    .scorm-download-container {
        width: 100%;
        height: 88px;
        min-height: 80px;
        background: #212B32;
        vertical-align: middle !important;
        min-width: 360px;

        @media(max-width: 414px) {
            width: 100%;
            height: 50px;
        }
    }

    .btn-scorm-download {
        max-height: 40px;
        font-size: 18px;
        color: #ffffff;
        background-color: transparent;
    }

    .btn-scorm-download:hover {
        max-height: 40px;
        color: #ffffff;
        background-color: transparent;
    }
</style>

<style lang="scss">
    // NOTE: Not `scoped` because we want this section to apply to children
    @use '../../../Styles/abstracts/all' as *;

    .accessible-link:focus {
        outline: none;
        text-decoration: none;
        color: $nhsuk-black;
        background-color: $govuk-focus-highlight-yellow;
        box-shadow: 0 -2px $govuk-focus-highlight-yellow,0 4px $nhsuk-black;
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
