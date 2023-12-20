<!-- <template>
    <div class="mx-0 pb-0 mb-5 row expandedContent" :key="expanded" v-if="expanded == true" id="source">
        <div class="col-12 w-100">
            <div class="mt-4 d-flex flex-nowrap justify-content-between">
                <div v-if="this.showActionPanel" class="action-panel">
                    <div v-if="currentCardExpanded.versionStatusEnum === this.CardVersionStatus.DRAFT && !currentCardExpanded.inEdit" class="d-flex align-items-center mr-3 mt-4">
                        <button class="btn btn-edit" v-on:click="edit(currentCardExpanded.id)">Edit</button>
                    </div>
                    <div v-if="currentCardExpanded.inEdit && !currentCardExpanded.isFlagged" class="d-flex align-items-center mr-3 mt-4">
                        <span class="small">This resource has edits which are waiting to be published</span>
                        <button class="btn btn-edit ml-4" v-on:click="edit(currentCardExpanded.id)">Edit</button>
                    </div>
                    <div v-if="currentCardExpanded.versionStatusEnum  === this.CardVersionStatus.PUBLISHED && !currentCardExpanded.isFlagged" class="d-flex align-items-center mr-3 mt-4">
                        <button class="btn btn-unpublish" data-toggle="modal" data-target="#unpublish">Unpublish</button>
                        <button class="btn btn-edit ml-4" v-on:click="edit(currentCardExpanded.id)">Edit</button>
                    </div>
                    <div v-if="currentCardExpanded.versionStatusEnum  === this.CardVersionStatus.UNPUBLISHED && currentCardExpanded.unpublishedByAdmin" class="d-flex align-items-center mr-3 mt-4">
                        <i class="fas fa-exclamation-circle unpublished-by-admin-icon"></i>
                        <span class="mx-4 small">This resource has been unpublished by Admin and is under review.</span>
                        <a href="#" class="mx-4 small">Read the details</a>
                        <button class="btn btn-edit ml-4" v-on:click="edit(currentCardExpanded.id)">Edit</button>
                    </div>
                    <div v-if="currentCardExpanded.isFlagged && !currentCardExpanded.flaggedByAdmin" class="d-flex align-items-center mr-3 mt-4">
                        <i class="fas fa-exclamation-circle resource-flagged-icon"></i>
                        <span class="mx-4 small">This resource has been flagged by a learner.</span>
                        <a href="#" class="mx-4 small">Read the details</a>
                        <button class="btn btn-unpublish ml-4" data-toggle="modal" data-target="#unpublish">Unpublish</button>
                        <button class="btn btn-edit ml-4" v-on:click="edit(currentCardExpanded.id)">Edit</button>
                    </div>
                </div>             
                <div class="float-right"><i v-on:click="collapse" class="fal fa-times closeButton"></i></div>
            </div>
            <div class="expandedCardHeaderText my-5 d-sm-block d-md-none">
                <h1 class="heading-lg">{{ currentCardExpanded.title }}</h1>
            </div>

            <div id="unpublish" ref="unpublishResourceModal" class="modal unpublish-modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
                <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                    <div class="modal-content">
                        <div class="modal-header text-center">
                            <h1 class="heading-lg w-100">Unpublish</h1>
                        </div>

                        <div class="modal-body">
                            <div class="mt-3">This resource will be not be available to anyone that had access to it and will not be found in search results.</div>
                        </div>

                        <div class="modal-footer">
                            <button class="btn btn-cancel" data-dismiss="modal">Cancel</button>
                            <button class="btn btn-unpublish" @click="unpublish(currentCardExpanded.id)">Unpublish this resource</button>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mt-4">

                <!- - Video/Audio - ->
                <div id="mediacontainer" class="col-12 col-md-6 order-md-2 text-left" v-if="currentCardExpanded.resourceTypeEnum  == CardResourceType.VIDEO || currentCardExpanded.resourceTypeEnum  == CardResourceType.AUDIO">
                    <div class="mt-1"><a href="#" class="small">Transcript available</a></div>
                </div>

                <!- - Image - ->
                <div class="col-12 col-md-6 order-md-2 text-left" v-if="currentCardExpanded.resourceTypeEnum  == CardResourceType.IMAGE">
                    <div class="resource resource-image text-center">
                        <img class="img-main" v-bind:src="getFileLink(currentCardExpanded.imageDetails.file.filePath, currentCardExpanded.imageDetails.file.fileName)" v-bind:alt="currentCardExpanded.imageDetails.altTag" />
                        <div class="row">
                            <div class="col-12">
                                <div>
                                    <button class="btn btn-preview btn-large mt-4" data-toggle="modal" data-target="#previewImageModal">Preview this image</button>
                                    <button class="btn btn-download btn-large mt-4 ml-auto" @click="downloadResource(currentCardExpanded.imageDetails.file.filePath, currentCardExpanded.imageDetails.file.fileName)">Download this image</button>
                                </div>
                            </div>
                        </div>
                        <div class="text-center small mt-3">
                            For guidance on downloading images, visit our <a href="#">support website</a>.
                        </div>
                        <div id="previewImageModal" class="modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
                            <div class="modal-dialog modal-dialog-centered modal-lg" role="document">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="closeButton close" data-dismiss="modal" aria-label="Close">
                                            <span aria-hidden="true"><i class="fal fa-times"></i></span>
                                        </button>
                                    </div>
                                    <div class="modal-body  text-center">
                                        <img class="img-preview" v-bind:src="getFileLink(currentCardExpanded.imageDetails.file.filePath, currentCardExpanded.imageDetails.file.fileName)" v-bind:alt="currentCardExpanded.imageDetails.altTag" />
                                        <div class="mt-3">{{  currentCardExpanded.title }} </div>
                                        <div class="mt-4"><button class="btn btn-download" @click="downloadResource(currentCardExpanded.imageDetails.file.filePath, currentCardExpanded.imageDetails.file.fileName)">Download</button></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Article - ->
                <div class="col-12 col-md-6 order-md-2 text-left" v-if="currentCardExpanded.resourceTypeEnum == CardResourceType.ARTICLE">
                    <div class="resource-article">
                        <div class="col-12 d-flex flex-column align-items-center">
                            <div class="fa-stack fa-2x doc-icon-stack">
                                <i class="fas fa-circle fa-stack-1x circle-icon"></i>
                                <i class="fal fa-file-alt fa-stack-1x doc-icon"></i>
                            </div>
                            <div>
                                <button class="btn btn-outline-custom" v-on:click="goToResourceItem">Read the full article</button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Equipment - ->
                <div class="col-12 col-md-6 order-md-2 resource-panel-container resource-equipment" v-if="currentCardExpanded.resourceTypeEnum == CardResourceType.EQUIPMENT">
                    <div class="resource-panel p-3 p-md-5">
                        <div class="col-12 d-flex flex-column align-items-left p-0">

                            <span class="section-header">Contact</span>
                            <span>{{ currentCardExpanded.equipmentDetails.contactName }}</span>
                            <span>{{ currentCardExpanded.equipmentDetails.contactTelephone }}</span>
                            <span>{{ currentCardExpanded.equipmentDetails.contactEmail }}</span>

                            <span class="mt-3 equipment-details-section-header">Location</span>
                            <span>{{ currentCardExpanded.equipmentDetails.address.address1 }}</span>
                            <span>{{ currentCardExpanded.equipmentDetails.address.address2 }}</span>
                            <span>{{ currentCardExpanded.equipmentDetails.address.address3 }}</span>
                            <span>{{ currentCardExpanded.equipmentDetails.address.address4 }}</span>
                            <span>{{ currentCardExpanded.equipmentDetails.address.town }}</span>
                            <span>{{ currentCardExpanded.equipmentDetails.address.county }}</span>
                            <span>{{ currentCardExpanded.equipmentDetails.address.postCode }}</span>

                        </div>
                        <hr class="mt-md-5 mx-n3 mx-md-n5 resource-panel-divider" />
                        <div class="col-12 d-flex flex-column align-items-left p-0 mt-md-5 ">
                            <span>You can include use of this piece of equipment or facility in My records which enables you to generate learning reports</span>
                            <button class="btn btn-custom mt-3" style="max-width:220px" data-toggle="modal" data-target="#addToMyRecords">Add to My records</button>
                        </div>

                        <div id="addToMyRecords" ref="addToMyRecordsModal" class="modal unpublish-modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
                            <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h1 class="heading-lg">Add to My Records</h1>
                                    </div>

                                    <div class="modal-body">
                                        <div class="d-flex flex-column">
                                            <label id="accessedDateLb" for="accessedDate" class="control-label">Access date</label>
                                            <div class="mt-3">Tell us the date you accessed this piece of equipment or facility.  Use this format DD/MM/YYYY (for example 20/12/2020)</div>
                                            <div class="user-entry ml-0 mt-3 pb-0" v-bind:class="{ 'input-validation-error': $v.selectedAccessedDate.$error}">
                                                <div class="error-text" v-if="$v.selectedAccessedDate.$invalid && $v.selectedAccessedDate.$dirty">
                                                    <span class="text-danger">{{returnError('accessedDate')}}</span>
                                                </div>
                                                <lh-date-picker label-text="accessedDateLb" display-style="1" unique-name="accessedDate" v-model="accessedDate" v-bind:class="{ 'input-validation-error': $v.selectedAccessedDate.$error}"></lh-date-picker>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="modal-footer">
                                        <button class="btn btn-cancel" data-dismiss="modal">Cancel</button>
                                        <button class="btn btn-add" @click="addToMyRecords()">Add</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- File Download - ->
                <div class="col-12 col-md-6 order-md-2 text-center" v-if="currentCardExpanded.resourceTypeEnum == CardResourceType.GENERICFILE">

                    <!-- NON SCORM/AICC - ->
                    <div class="resource-panel p-3 p-md-5" v-if="!currentCardExpanded.genericFileDetails.scormAiccContent">
                        <div class="col-12 d-flex flex-column align-items-center p-0">
                            <div class="fa-stack fa-2x doc-icon-stack">
                                <i class="fas fa-circle fa-stack-1x circle-icon"></i>
                                <i class="fal fa-file fa-stack-1x doc-icon"></i>
                            </div>
                            <div>
                                <a v-bind:href="getFileLink(currentCardExpanded.genericFileDetails.file.filePath, currentCardExpanded.genericFileDetails.file.fileName)" v-on:click="downloadResource(currentCardExpanded.genericFileDetails.file.filePath, currentCardExpanded.genericFileDetails.file.fileName)">{{ currentCardExpanded.genericFileDetails.file.fileName }} ({{currentCardExpanded.genericFileDetails.file.fileSizeKb}}KB)</a>
                            </div>
                            <div>
                                <button class="btn btn-custom my-5" v-on:click="downloadResource(currentCardExpanded.genericFileDetails.file.filePath, currentCardExpanded.genericFileDetails.file.fileName)">View this file</button>
                            </div>
                            <div class="small">
                                For guidance on downloading documents, visit our <a href="#">support website</a>.
                            </div>
                        </div>
                    </div>

                    <!-- SCORM/AICC - ->
                    <div class="col-12 col-md-6 order-md-2 text-center" v-if="currentCardExpanded.genericFileDetails.scormAiccContent">
                        <div class="resource-panel p-3 p-md-5">
                            <div class="col-12 d-flex flex-column align-items-center p-0">
                                <div class="p-3">
                                    The Learning Hub is planning on supporting this elearning format in the near future which will enable you to play this elearning module.
                                </div>
                                <div class="p-3">
                                    In the meantime you can download this package and add it to another system that supports this format.
                                </div>
                                <div>
                                    <button class="btn btn-custom my-5" v-on:click="downloadResource(currentCardExpanded.genericFileDetails.file.filePath, currentCardExpanded.genericFileDetails.file.fileName)">Download this elearning package</button>
                                </div>
                                <div class="small">
                                    For guidance on downloading elearning packages,<br/>visit our <a href="#">support website</a>.
                                </div>
                            </div>
                        </div>
                    </div>
                </div>               

                <!-- Weblink - ->
                <div class="col-12 col-md-6 order-md-2 text-center" v-if="currentCardExpanded.resourceTypeEnum  == CardResourceType.WEBLINK">
                    <div class="resource-panel p-3 p-md-5">
                        <div class="col-12 d-flex flex-column align-items-center p-0">
                            <div class="fa-stack fa-2x doc-icon-stack">
                                <i class="fas fa-circle fa-stack-1x circle-icon"></i>
                                <i class="fal fa-globe fa-stack-1x doc-icon"></i>
                            </div>
                            <div>
                                <a v-bind:href="this.currentCardExpanded.webLinkDetails.url" target="_blank">{{ currentCardExpanded.webLinkDetails.displayText }}</a>
                            </div>
                            <div>
                                <button class="btn btn-custom my-5" v-on:click="goToWeblink">Visit this weblink</button>
                            </div>
                            <div class="small">
                                This link will open in a new tab. You can close this new tab and return to the Learning Hub at any point.
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-12 col-md-6 order-md-1">
                    <div class="resourcedetails">
                        <div class="expandedCardHeaderText d-none d-sm-none d-md-block">
                            <h2 class="heading-lg">{{ currentCardExpanded.title }}</h2>
                        </div>
                        <div class="expandedCardTextLight small mt-4" v-if="currentCardExpanded.versionDescription">Version {{ currentCardExpanded.versionDescription }}</div>
                        <div class="expandedCardTextLight small"><span v-if="durationInMinutes">{{ durationInMinutes }}</span> {{ currentCardExpanded.resourceTypeDescription }} contributed by {{currentCardExpanded.publishedBy}} on {{ currentCardExpanded.publishedDate | formatDate('DD MMM YYYY')}}</div>

                        <div class="expandedCardTextLight small">
                            Authored by
                            <span v-for="(item, index) in currentCardExpanded.authors">
                                {{ item }}{{ (index+1 < currentCardExpanded.authors.length) ? ', ' : '' }}
                            </span><span v-if="currentCardExpanded.resourceTypeEnum == CardResourceType.GENERICFILE"> {{ authoredDate }}</span>
                        </div>
                        <div class="expandedCardText mr-5 mt-4">
                            <extendedtruncate :href="'/Resource/' + currentCardExpanded.resourceReferenceId + '/item'" length="450" suffix="..." v-bind:text="currentCardExpanded.description"></extendedtruncate>
                        </div>
                        <div class="expandedCardText additionalInfo mt-4 d-flex align-items-center p-4" v-if="currentCardExpanded.additionalInformation">{{ currentCardExpanded.additionalInformation }}</div>
                        <div class="expandedCardTexts small mt-4" v-if="(currentCardExpanded.resourceFree || currentCardExpanded.cost)">
                            <div v-if="currentCardExpanded.resourceFree">
                                There is no cost to use this resource
                            </div>
                            <div v-else-if="currentCardExpanded.cost">
                                Cost: {{ currentCardExpanded.cost | currency }}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row mt-3"><div class="py-1 mx-0 col-12"><hr /></div></div>
            <div class="row pb-3">
                <div class="col-12" v-if="currentCardExpanded">
                    <div class="d-block d-md-inline resourcelinks">
                        <a :href="'/Resource/' + currentCardExpanded.resourceReferenceId +  '/item'"><i class="fas fa-info-circle fa-lg mr-2"></i>More about this resource</a>
                    </div>
                    <div class="d-block d-md-inline resourcelinks">
                        <a href="#"><i class="fal fa-share-alt fa-lg mr-2"></i>Share this resource</a>
                    </div>
                    <div v-if="currentCardExpanded.showFlagResourceLink == true" class="d-block d-md-inline resourcelinks">
                        <a href="#"><i class="fal fa-flag fa-lg mr-2"></i>Report an issue</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Vuelidate from "vuelidate";
    import { VersionStatus, ResourceType } from './constants';    
    import { activityRecorder } from './activity';    
    import { commonlib } from './common';
    import { AxiosResponse } from 'axios';
    import AxiosWrapper from './axiosWrapper';
    import { TrayCardExtendedModel } from './models/trayCardExtendedModel';
    import { LearningHubValidationResultModel } from './models/learningHubValidationResultModel';
    import extendedtruncate from './globalcomponents/extendedtruncate.vue';
    import LhDatePicker from './datepicker.vue';
    import { required, maxValue } from "vuelidate/lib/validators";
    import { ResourceAzureMediaAssetModel } from './models/resourceAzureMediaAssetModel';
    
    Vue.use(Vuelidate as any);

    export default Vue.extend({
        name: 'expandedcardcomp',  
        props: {
            'showActionPanel': { Type: Boolean } as PropOptions<boolean>,
        },
        computed: {
            selectedAccessedDate(): Date {
                return this.accessedDate ? new Date(this.accessedDate) : null;
            },
            authoredDate(): string {
                let genericFileDetails = this.currentCardExpanded.genericFileDetails;
                return commonlib.getAuthoredDate(genericFileDetails.authoredDayOfMonth, genericFileDetails.authoredMonth, genericFileDetails.authoredYear);
            },
            durationInMinutes(): number  {
                if (this.currentCardExpanded.resourceTypeEnum == this.CardResourceType.VIDEO) {
                    return this.currentCardExpanded.videoDetails.durationInMilliseconds / 1000 / 60;
                }
                else if (this.currentCardExpanded.resourceTypeEnum == this.CardResourceType.AUDIO) {
                    return this.currentCardExpanded.audioDetails.durationInMilliseconds / 1000 / 60;
                }
                else
                    return null;
            }
        },
        data() {
            return {
                CardResourceType: ResourceType,
                CardVersionStatus: VersionStatus,
                expanded: false,
                currentCard: null as TrayCardExtendedModel,
                currentCardExpanded: null as TrayCardExtendedModel,
                accessedDate: ''
            };
        },
        components: {
            extendedtruncate,
            LhDatePicker
        },
        updated() {

            if (this.expanded) {
                if (this.currentCardExpanded.resourceTypeEnum == this.CardResourceType.VIDEO) {
                    let proxyUrl = this.getMediaAssetProxyUrl(this.currentCardExpanded.videoDetails.resourceAzureMediaAsset);
                    showMedia(proxyUrl, this.currentCardExpanded.videoDetails.file.filePath);
                }
                if (this.currentCardExpanded.resourceTypeEnum == this.CardResourceType.AUDIO) {
                    let proxyUrl = this.getMediaAssetProxyUrl(this.currentCardExpanded.audioDetails.resourceAzureMediaAsset);
                    showMedia(proxyUrl, this.currentCardExpanded.audioDetails.file.filePath);
                }
                if ($("#traytitle").length) {
                    commonlib.scrollTo('#traytitle');
                }
            }
        },
        methods: {
            expand(card: TrayCardExtendedModel) {
                this.currentCard = card;
                if (this.expanded == false) {
                    AxiosWrapper.axios.get<TrayCardExtendedModel>('/api/card/GetExtendedCardContent', {
                        params: {
                            id: this.currentCard.id
                        }
                    })
                        .then(response => {
                            this.expanded = true;
                            this.currentCardExpanded = response.data;
                        })
                        .catch(e => {
                            console.log(e);
                        });
                }
                else {
                    this.$emit('collapse');
                    this.expanded = false;
                }
            },
            edit(id: number) {
                window.location.pathname = './Contribute/contribute-a-resource/' + id.toString();
            },
            unpublish(id: number) {

                AxiosWrapper.axios.post<number, AxiosResponse<LearningHubValidationResultModel>>('/api/resource/UnpublishResourceVersion?id=' + id.toString())
                    .then(response => {
                        if (response.data.isValid) {
                            this.currentCardExpanded.versionStatusEnum = VersionStatus.UNPUBLISHED;
                            this.$emit("unpublish", id);
                            $('#unpublish').modal('hide');
                        }
                        else { }
                    })
                    .catch(e => {
                        console.log(e);
                    });
            },
            collapse() {
                this.expanded = false;
                this.$emit('collapse');
            },           
            goToResourceItem() {
                location.href = '/Resource/' + this.currentCardExpanded.resourceReferenceId + '/Item';
            },
            downloadResource(filePath :string, fileName:string) {
                window.open(this.getFileLink(filePath, fileName));
            },
            getFileLink(filePath :string, fileName :string): string {
                return "/api/resource/DownloadResource?filePath=" + filePath  + "&fileName=" + fileName;
            },
            goToWeblink() {
                window.open(this.currentCardExpanded.webLinkDetails.url);                
            },
            returnError(ctrl: string) {
                var errorMessage = '';
                switch (ctrl) {
                    case 'accessedDate':
                        if (this.$v.selectedAccessedDate.$invalid) {
                            if (!this.$v.selectedAccessedDate.required) {
                                errorMessage = "Please add the date that this piece of equipment or facility was accessed.";
                            }
                            if (!this.$v.selectedAccessedDate.maxValue) {
                                errorMessage = "The date you accessed this piece of equipment or facility cannot be in the future.";
                            }
                        }
                        break;
                }
                return errorMessage;
            },
            async addToMyRecords() {

                if (this.$v.selectedAccessedDate.$invalid) {

                    this.$v.selectedAccessedDate.$touch();
                }
                else {

                    await activityRecorder.recordActivityLaunched(this.currentCardExpanded.resourceVersionId, this.selectedAccessedDate)
                        .then(response => {
                            $('#addToMyRecords').modal('hide');
                        })
                        .catch(e => {
                            console.log(e);
                            window.location.pathname = './Home/Error';
                        });
                }
            },
            getMediaAssetProxyUrl(azureMediaAsset: ResourceAzureMediaAssetModel): string {
                return "/Resource/MediaManifest?playBackUrl=" + azureMediaAsset.locatorUri + "&token=" + azureMediaAsset.authenticationToken;
            }
        },
        validations: {
            selectedAccessedDate: {
                required,
                maxValue: maxValue(new Date(new Date().toISOString()))
            }
        }
    })
</script>
<style lang="scss" scoped>
    @use "../../Styles/abstracts/all" as *;

    .expandedContent {
        background-color: $nhsuk-white;
        border-top: 1px solid $nhsuk-grey-light;
        box-shadow: 0px 6px 6px #dedfe1;
    }

    .description {
        border-radius: 8px;
        background-color: $nhsuk-white;
        min-height: 180px;
        box-shadow: 0px 6px 6px #dedfe1;
    }

    .expandedCardText {
        color: $nhsuk-black;
        font-family: FrutigerLT55Roman;
    }

    .expandedCardTextLight {
        color: $nhsuk-black;
        font-family: FrutigerLight;
    }

    .expandedCardHeaderText {
        .heading-lg

    {
        line-height: 48px !important;
    }

    }

    .closeButton, .costText {
        color: $nhsuk-grey;
    }

    .closeButton {
        font-size: 1.3em;
        cursor: pointer;
    }

    .resourcedetails {
        max-width: 800px;
    }

    .sourcedByBlock, .additionalInfo {
        background-color: $nhsuk-grey-white;
        border-radius: 6px;
    }

    .sourcedByBlock {
        width: auto;
        float: left;
        padding-right: 20px;
        height: 60px;
        font-size: 1.6rem;
    }

    .action-panel {
        .btn-publish

    {
        background-color: $nhsuk-blue;
        color: $nhsuk-white;
        min-width: 80px;
        height: 40px;
        font-size: 1.7rem !important;
        text-align: center !important;
    }

    .btn-publish-edits {
        background-color: $nhsuk-blue;
        color: $nhsuk-white;
        min-width: 120px;
        height: 40px;
        font-size: 1.7rem !important;
        text-align: center !important;
    }

    .btn-edit {
        background-color: $nhsuk-white;
        color: $nhsuk-blue;
        border: 1px solid $nhsuk-blue;
        min-width: 80px;
        height: 40px;
        font-size: 1.7rem !important;
        text-align: center !important;
    }

    .btn-unpublish {
        background-color: $nhsuk-white;
        color: $nhsuk-blue;
        border: 1px solid $nhsuk-blue;
        min-width: 90px;
        height: 40px;
        font-size: 1.7rem !important;
        text-align: center !important;
    }

    .resource-flagged-icon {
        color: #DA291C;
    }

    .unpublished-by-admin-icon {
        color: #DA291C;
    }

    .edits-pending-icon {
        color: $nhsuk-blue;
    }

    }

    .unpublish-modal {

        .btn-cancel
    {
        background-color: $nhsuk-white;
        color: $nhsuk-blue;
        border: 1px solid $nhsuk-blue;
        min-width: 80px;
        height: 40px;
        font-size: 1.7rem !important;
        text-align: center !important;
        flex: 1 1 0px;
    }

    .btn-unpublish {
        background-color: $nhsuk-blue;
        color: $nhsuk-white;
        border: 1px solid $nhsuk-blue;
        min-width: 90px;
        height: 40px;
        font-size: 1.7rem !important;
        text-align: center !important;
        flex: 1 1 0px;
    }

    }

    .resourcelinks {
        align-items: center;
        line-height: 40px;
        margin-right: 30px;        
    a
    {
        font-size: 1.6rem;
    }

    i {
        padding-right: 3px;
    }

    }

    #mediacontainer video, .resource {
        width: 100% !important;
        max-width: 600px !important;
        height: auto !important;
        min-height: 190px;
        background-color: none;
    }

    .resource-audio {
        display: block;
        height: 330px;
        background-image: url('/images/audio-bg.png');
        background-repeat: no-repeat;
        background-position: center;
        background-size: 595px 330px;
    }

    .resource-image {
        border: 1px solid $nhsuk-blue;
        background-color: $nhsuk-grey-white;
        border-radius: 5px;
        padding: 28px;
        .img-main

    {
        border: 1px solid $nhsuk-grey-lighter;
        border-radius: 5px;
        max-width: 100%;
        height: auto;
    }

    .btn-preview, .btn-download {
        height: 50px;
        border: 1px solid $nhsuk-blue;
        font-size: 1.9rem;
    }

    .btn-preview {
        background-color: $nhsuk-white;
        color: $nhsuk-blue;
    }

    .btn-download {
        background-color: $nhsuk-blue;
        color: $nhsuk-white;
        min-width: 140px;
    }

    .btn-large {
        width: 256px;
    }

    .modal-header .close {
        margin: -1rem -2rem 1rem auto;
        padding: 0px !important;
    }

    }

    .resource-article {
        border: 1px solid $nhsuk-blue;
        background-color: $nhsuk-grey-white;
        background: url('/images/faces-background.png') repeat center;
        border-radius: 5px;
        padding: 24px;
        margin-bottom: 10px;
        .doc-icon-stack

    {
        margin-bottom: 23px;
    }

    .doc-icon {
        color: $nhsuk-blue;
    }

    .circle-icon {
        color: white;
        font-size: 6rem;
    }

    }

    .resource-equipment{

        .section-header{
            font-weight:bold;
        }

        .resource-panel-divider
        {
            border-color: $nhsuk-blue;
        }

        .btn-cancel
        {
            background-color: $nhsuk-white;
            color: $nhsuk-blue;
            border: 1px solid $nhsuk-blue;
            max-width: 80px;
            height: 40px;
            font-size: 1.7rem !important;
            text-align: center !important;
            flex: 1 1 0px;
        }

        .btn-add {
            background-color: $nhsuk-blue;
            color: $nhsuk-white;
            border: 1px solid $nhsuk-blue;
            max-width: 80px;
            height: 40px;
            font-size: 1.7rem !important;
            text-align: center !important;
            flex: 1 1 0px;
        }
    }

    .resource-panel {
        border: 1px solid $nhsuk-blue;
        background-color: $nhsuk-grey-white;
        border-radius: 5px;
        margin-bottom: 10px;

        .doc-icon-stack
        {
            margin-bottom: 23px;
        }

    .doc-icon {
        color: white;
    }

    .circle-icon {
        color: $nhsuk-blue;
        font-size: 6rem;
    }

    }

    .img-preview {
        max-width: 100%;
        height: auto;
    }

    @media (max-width: 768px) {
        .expandedContent {
            margin-left: -30px;
            margin-right: -30px;
            padding-left: 15px;
            padding-right: 15px;
        }

        .expandedCardHeaderText {
            .heading-lg

    {
        line-height: 36px !important;
    }

    }

    .resource-image {
        border: none;
    }
    }
</style>

-->