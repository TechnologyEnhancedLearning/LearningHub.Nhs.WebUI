<template>
    <div class="contribute-component">
        <div v-if="loading"
             class="contribute-component-loading">
            <div>Loading...</div>
            <div class="contribute-component-loading-spinner">
                <Spinner></Spinner>
            </div>
        </div>
        <div v-else>
            <div v-if="hierarchyEditLoaded && catalogueLockedForEdit"
                 class="mt-4">
                <div class="lh-padding-fluid">
                    <div class="lh-container-xl">
                        <div class="d-flex flex-row">
                            <i class="fa-solid fa-triangle-exclamation mr-4 mt-2 text-warning"
                               aria-hidden="true"></i>
                            <div class="small">
                                A Learning Hub system administrator is currently making changes to this
                                catalogue. You can browse the catalogue but cannot add / edit or move
                                resources at this time.
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div v-if="hierarchyEditLoaded && !catalogueLockedForEdit">
                <div v-if="resourceDetails.resourceType === ResourceType.ASSESSMENT &&
                     assessmentDetails !== null && assessmentDetails.assessmentType === AssessmentTypeEnum.Undefined">
                    <ContributeChooseAssessmentType @updateType="updateAssessmentType" />
                </div>
                <div v-else-if="!editSlideAnnotations">
                    <div v-if="isCaseOrAssessmentResourceType && allowPublish"
                         class="terms-and-conditions-reminder">
                        <TermsAndConditionsReminder class="lh-container-xl py-20" />
                    </div>
                    <ContributeActionsBar :savingState="savingState"
                                          :duplicatingBlocks.sync="duplicatingBlocks"
                                          :allowPublish="allowPublish"
                                          :isCaseOrAssessmentResourceType="isCaseOrAssessmentResourceType"
                                          :isDuplicatingResource="isDuplicatingResource"
                                          :isResourceDuplicationSuccess="isResourceDuplicationSuccess"
                                          :errorMessage="errorMessage"
                                          @discardDraft="discardDraft"
                                          @saveForLater="saveForLater"
                                          @createDuplicate="createDuplicate"
                                          @publishResource="attemptPublishResource"
                                          @presentPreview="presentPreview"></ContributeActionsBar>

                    <div class="contribute-title-wrapper lh-padding-fluid">
                        <div class="lh-container-xl pt-20 pb-2">
                            <EditSaveFieldWithCharacterCount v-model="resourceDetails.title"
                                                             addEditLabel="title"
                                                             :characterLimit="255"
                                                             :isH1="true"
                                                             size="large"></EditSaveFieldWithCharacterCount>
                        </div>
                        <h3 v-if="resourceDetails.resourceType === ResourceType.ASSESSMENT"
                            class="pb-15 lh-container-xl">
                            {{
                                assessmentDetails.assessmentType === AssessmentTypeEnum.Informal ? "Informal" : "Formal"
                            }} assessment
                        </h3>
                    </div>
                    <div v-if="showProviders">
                        <PageTabs :navbarPadded="true">
                            <template v-slot:tab_0>
                                <Tick :complete="contentTabComplete"></Tick>
                                Content
                            </template>
                            <template v-slot:page_0>
                                <ContributeCaseOrAssessmentPage v-if="resourceDetails.resourceType === ResourceType.CASE && !!caseDetails"
                                                                :blockCollection="caseDetails.blockCollection"
                                                                :firstTimeOpen="firstTimeOpen"
                                                                :resourceId="resourceDetails.resourceId"
                                                                :resourceType="resourceDetails.resourceType"
                                                                :duplicatingBlocks="duplicatingBlocks"
                                                                @cancelDuplicatingBlocks="cancelDuplicatingBlocks"
                                                                @annotateWholeSlideImage="toggleSlideWithAnnotations" />
                                <ContributeCaseOrAssessmentPage v-else-if="resourceDetails.resourceType === ResourceType.ASSESSMENT && !!assessmentDetails"
                                                                :blockCollection="assessmentDetails.assessmentContent"
                                                                :firstTimeOpen="firstTimeOpen"
                                                                :duplicatingBlocks="duplicatingBlocks"
                                                                :resourceId="resourceDetails.resourceId"
                                                                :resourceType="resourceDetails.resourceType"
                                                                @cancelDuplicatingBlocks="cancelDuplicatingBlocks"
                                                                @annotateWholeSlideImage="toggleSlideWithAnnotations" />
                            </template>
                            <template v-slot:tab_1>
                                <Tick :complete="resourceAccessTabComplete"></Tick>
                                Access
                            </template>
                            <template v-slot:page_1>
                                <ContributeCaseResourceAccessTab :resourceDetails="resourceDetails" />
                            </template>
                            <template v-slot:tab_2>
                                <Tick :complete="providedbyPermissionTabComplete"></Tick>
                                Content developed with
                            </template>
                            <template v-slot:page_2>
                                <ContributeProvideByTab :resourceDetails="resourceDetails" :userProviders="userProviders" />
                            </template>
                            <template v-slot:tab_3>
                                <Tick :complete="resourceInfoTabComplete"></Tick>
                                Resource info
                            </template>
                            <template v-slot:page_3>
                                <ContributeCaseResourceInfo :resourceDetails="resourceDetails" />
                            </template>
                            <template v-slot:tab_4>
                                <Tick :complete="authorTabComplete"></Tick>
                                Authors
                            </template>
                            <template v-slot:page_4>
                                <ContributeAuthorsTab :resourceDetails="resourceDetails"
                                                      :configuration="configuration" />
                            </template>
                            <template v-slot:tab_5>
                                <Tick :complete="licenceTabComplete"></Tick>
                                Licence
                            </template>
                            <template v-slot:page_5>
                                <ContributeLicenceTab v-if="!newStyleLicensePage"
                                                      :resourceDetails="resourceDetails"
                                                      :licences="licences" />
                                <ContributeLicenceTabWithTable v-if="newStyleLicensePage"
                                                               :selected-licence-id="resourceDetails.resourceLicenceId"
                                                               :licences="licences"
                                                               @choose-licence="resourceDetails.resourceLicenceId = $event" />
                            </template>
                            <template v-slot:tab_6>
                                <Tick :complete="locationTabComplete"></Tick>
                                Location
                            </template>
                            <template v-slot:page_6>
                                <ContributeLocationTab :resourceDetails="resourceDetails"
                                                       :userCatalogues="userCatalogues" />
                            </template>
                            <template v-slot:tab_7>
                                <Tick :complete="certificateTabComplete"></Tick>
                                Certificate
                            </template>
                            <template v-slot:page_7>
                                <ContributeCertificateTab v-if="resourceDetails.resourceType === ResourceType.CASE || resourceDetails.resourceType === ResourceType.ASSESSMENT"
                                                          :resourceDetails="resourceDetails" />
                            </template>
                        </PageTabs>
                    </div>
                    <div v-else>
                        <PageTabs :navbarPadded="true">
                            <template v-slot:tab_0>
                                <Tick :complete="contentTabComplete"></Tick>
                                Content
                            </template>
                            <template v-slot:page_0>
                                <ContributeCaseOrAssessmentPage v-if="resourceDetails.resourceType === ResourceType.CASE && !!caseDetails"
                                                                :blockCollection="caseDetails.blockCollection"
                                                                :firstTimeOpen="firstTimeOpen"
                                                                :resourceId="resourceDetails.resourceId"
                                                                :resourceType="resourceDetails.resourceType"
                                                                :duplicatingBlocks="duplicatingBlocks"
                                                                @cancelDuplicatingBlocks="cancelDuplicatingBlocks"
                                                                @annotateWholeSlideImage="toggleSlideWithAnnotations" />
                                <ContributeCaseOrAssessmentPage v-else-if="resourceDetails.resourceType === ResourceType.ASSESSMENT && !!assessmentDetails"
                                                                :blockCollection="assessmentDetails.assessmentContent"
                                                                :firstTimeOpen="firstTimeOpen"
                                                                :duplicatingBlocks="duplicatingBlocks"
                                                                :resourceId="resourceDetails.resourceId"
                                                                :resourceType="resourceDetails.resourceType"
                                                                @cancelDuplicatingBlocks="cancelDuplicatingBlocks"
                                                                @annotateWholeSlideImage="toggleSlideWithAnnotations" />
                            </template>
                            <template v-slot:tab_1>
                                <Tick :complete="resourceAccessTabComplete"></Tick>
                                Access
                            </template>
                            <template v-slot:page_1>
                                <ContributeCaseResourceAccessTab :resourceDetails="resourceDetails" />
                            </template>
                            <template v-slot:tab_2>
                                <Tick :complete="resourceInfoTabComplete"></Tick>
                                Resource info
                            </template>
                            <template v-slot:page_2>
                                <ContributeCaseResourceInfo :resourceDetails="resourceDetails" />
                            </template>
                            <template v-slot:tab_3>
                                <Tick :complete="authorTabComplete"></Tick>
                                Authors
                            </template>
                            <template v-slot:page_3>
                                <ContributeAuthorsTab :resourceDetails="resourceDetails"
                                                      :configuration="configuration" />
                            </template>
                            <template v-slot:tab_4>
                                <Tick :complete="licenceTabComplete"></Tick>
                                Licence
                            </template>
                            <template v-slot:page_4>
                                <ContributeLicenceTab v-if="!newStyleLicensePage"
                                                      :resourceDetails="resourceDetails"
                                                      :licences="licences" />
                                <ContributeLicenceTabWithTable v-if="newStyleLicensePage"
                                                               :selected-licence-id="resourceDetails.resourceLicenceId"
                                                               :licences="licences"
                                                               @choose-licence="resourceDetails.resourceLicenceId = $event" />
                            </template>
                            <template v-slot:tab_5>
                                <Tick :complete="locationTabComplete"></Tick>
                                Location
                            </template>
                            <template v-slot:page_5>
                                <ContributeLocationTab :resourceDetails="resourceDetails"
                                                       :userCatalogues="userCatalogues" />
                            </template>
                            <template v-slot:tab_6>
                                <Tick :complete="certificateTabComplete"></Tick>
                                Certificate
                            </template>
                            <template v-slot:page_6>
                                <ContributeCertificateTab v-if="resourceDetails.resourceType === ResourceType.CASE || resourceDetails.resourceType === ResourceType.ASSESSMENT"
                                                          :resourceDetails="resourceDetails" />
                            </template>
                        </PageTabs>
                    </div>
                </div>
                <div v-else
                     class="contribute-annotation-edit">
                    <AnnotationFullPageEdit :resourceType="resourceDetails.resourceType"
                                            :wholeSlideImage="wholeSlideImage"
                                            :savingState="savingState"
                                            :imageZone="imageZone"
                                            :questionBlock="questionBlock"
                                            @annotateWholeSlideImage="toggleSlideWithAnnotations" />
                </div>
            </div>
            <PublishWithPlaceholderModal v-if="showPlaceholderModal"
                                         @closePlaceholderModal="showPlaceholderModal = false"
                                         @proceedWithPublishing="publishResource" />
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import _ from 'lodash';
    import ContributeActionsBar from './ContributeActionsBar.vue';
    import ContributeTitle from './ContributeTitle.vue';
    import PageTabs from '../globalcomponents/PageTabs.vue';
    import ContributeCaseOrAssessmentPage from './ContributeCaseOrAssessmentPage.vue';
    import ContributeCaseResourceAccessTab from './ContributeCaseResourceAccessTab.vue';
    import ContributeProvideByTab from './ContributeProvideByTab.vue';
    import ContributeCaseResourceInfo from './ContributeCaseResourceInfo.vue';
    import ContributeAuthorsTab from './ContributeAuthorsTab.vue';
    import ContributeLicenceTab from './ContributeLicenceTab.vue';
    import ContributeLicenceTabWithTable from './ContributeLicenceTabWithTable.vue';
    import ContributeLocationTab from './ContributeLocationTab.vue';
    import ContributeCertificateTab from './ContributeCertificateTab.vue';
    import PublishWithPlaceholderModal from "./PublishWithPlaceholderModal.vue";
    import ContributeChooseAssessmentType from './ContributeChooseAssessmentType.vue';
    import EditSaveFieldWithCharacterCount from '../globalcomponents/EditSaveFieldWithCharacterCount.vue';
    import Tick from '../globalcomponents/Tick.vue';
    import Spinner from '../globalcomponents/Spinner.vue';
    import { getRemainingCharactersFromHtml } from "../helpers/ckeditorValidationHelper";
    import { AssessmentModel } from '../models/contribute-resource/assessmentModel';
    import { AssessmentTypeEnum } from '../models/contribute-resource/blocks/assessments/assessmentTypeEnum';
    import { CaseResourceModel } from '../models/contribute-resource/caseResourceModel';
    import { FileModel } from '../models/contribute-resource/files/fileModel';
    import ContributeApi from './contributeApi';
    import { resourceData } from '../data/resource';
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
    import { ResourceAccessibility, ResourceType } from '../constants';
    import { FileStore } from '../models/contribute-resource/files/fileStore';
    import { CatalogueBasicModel } from '../models/catalogueModel';
    import { ContributeConfiguration } from '../models/contribute-resource/contributeConfiguration';
    import { LicenceModel } from '../models/contribute/licenceModel';
    import { WholeSlideImageModel } from '../models/contribute-resource/blocks/wholeSlideImageModel';
    import AnnotationFullPageEdit from '../resource/blocks/AnnotationFullPageEdit.vue';
    import { publishResource, redirectToMyContributions, redirectToMyContributionsNode } from './helpers/Publishing';
    import { ContributeInjection } from './interfaces/injections';
    import TermsAndConditionsReminder from "../contribute/TermsAndConditionsReminder.vue";
    import { QuestionBlockModel } from "../models/contribute-resource/blocks/questionBlockModel";
    import { contentStructureData } from '../data/contentStructure';
    import { HierarchyEditStatusEnum, HierarchyEditModel } from '../models/content-structure/hierarchyEditModel';
    import { ProviderModel } from '../models/ProviderModel';
    import { providerData } from '../data/provider';

    FileStore.enablePolling();

    export default Vue.extend({
        props: {
            resourceVersionId: String,
        },
        components: {
            ContributeActionsBar,
            ContributeTitle,
            PageTabs,
            ContributeCaseOrAssessmentPage,
            ContributeCaseResourceInfo,
            ContributeCaseResourceAccessTab,
            ContributeProvideByTab,
            ContributeAuthorsTab,
            ContributeLicenceTab,
            ContributeLicenceTabWithTable,
            ContributeLocationTab,
            ContributeCertificateTab,
            EditSaveFieldWithCharacterCount,
            PublishWithPlaceholderModal,
            Spinner,
            Tick,
            AnnotationFullPageEdit,
            ContributeChooseAssessmentType,
            TermsAndConditionsReminder
        },
        data() {
            return {
                configuration: null as ContributeConfiguration,
                resourceDetails: null as ContributeResourceDetailModel,
                assessmentDetails: null as AssessmentModel,
                caseDetails: null as CaseResourceModel,
                userCatalogues: null as CatalogueBasicModel[],
                licences: null as LicenceModel[],
                userProviders: null as ProviderModel[],
                savingState: 'saved',
                caseOrAssessmentAutoSaveInProgress: false,
                resourceAutoSaveInProgress: false,
                loading: true,
                ResourceType: ResourceType,
                newStyleLicensePage: false,
                editSlideAnnotations: false,
                wholeSlideImage: null as WholeSlideImageModel,
                isDuplicatingResource: false,
                isResourceDuplicationSuccess: undefined,
                showPlaceholderModal: false,
                firstTimeOpen: false,
                imageZone: false as Boolean,
                questionBlock: null as QuestionBlockModel,
                AssessmentTypeEnum,
                hierarchyEdit: null as HierarchyEditModel,
                hierarchyEditLoaded: false as boolean,
                duplicatingBlocks: false,
                errorMessage: '',
                counterInterval: undefined as any,
            };
        },
        async created() {
            this.loading = true;
            await this.populateResource();
            this.loading = false;
            this.loadHierarchyEdit();
            this.counterInterval = setInterval(() => this.loadHierarchyEdit(), 10000);
            this.saveResourceDetails = _.debounce(this.saveResourceDetailsDebounced, 300, { 'maxWait': 1000 });
            this.saveCaseDetails = _.debounce(this.saveCaseDetailsDebounced, 300, { 'maxWait': 1000 });
            this.saveAssessmentDetails = _.debounce(this.saveAssessmentDetailsDebounced, 300, { 'maxWait': 1000 });

            window.addEventListener('beforeunload', this.checkForOngoingFileUploadsOnPageUnload);
        },
        provide(): ContributeInjection {
            const injection = {};

            Object.defineProperty(injection, "resourceType", {
                enumerable: true,
                get: () => this.resourceDetails?.resourceType,
            });

            Object.defineProperty(injection, "assessmentDetails", {
                enumerable: true,
                get: () => this.assessmentDetails,
            });

            return injection as any;
        },
        watch: {
            resourceDetails: {
                deep: true,
                handler(newVal, oldVal) {
                    if (oldVal) {
                        this.saveResourceDetails();
                    }
                }
            },
            caseDetails: {
                deep: true,
                handler(newVal, oldVal) {
                    if (oldVal && !this.duplicatingBlocks) {
                        this.saveCaseDetails();
                    }
                }
            },
            assessmentDetails: {
                deep: true,
                handler(newVal, oldVal) {
                    if (oldVal && !this.duplicatingBlocks) {
                        this.saveAssessmentDetails();
                    }
                }
            },
            async duplicatingBlocks() {
                if (this.duplicatingBlocks) {
                    await this.loadResourceDetails();
                }
            }
        },
        computed: {
            contentTabComplete(): boolean {
                switch (this.resourceDetails.resourceType) {
                    case ResourceType.CASE:
                        return this.caseDetails.isReadyToPublish();
                    case ResourceType.ASSESSMENT:
                        return this.assessmentDetails.isReadyToPublish();
                    default:
                        return false;
                }
            },
            resourceAccessTabComplete(): boolean {
                return this.resourceDetails.resourceAccessibilityEnum !== ResourceAccessibility.None;
            },
            resourceInfoTabComplete(): boolean {
                return !!this.resourceDetails.description
                    && getRemainingCharactersFromHtml(1000, this.resourceDetails.description) >= 0
                    && !!this.resourceDetails.resourceKeywords
                    && this.resourceDetails.resourceKeywords.length !== 0;
            },
            authorTabComplete(): boolean {
                return !!this.resourceDetails.resourceAuthors
                    && this.resourceDetails.resourceAuthors.length !== 0;
            },
            licenceTabComplete(): boolean {
                return this.resourceDetails.resourceLicenceId > 0;
            },
            locationTabComplete(): boolean {
                return this.resourceDetails.resourceCatalogueId > 0;
            },
            certificateTabComplete(): boolean {
                return this.resourceDetails.certificateEnabled !== null;
            },
            providedbyPermissionTabComplete(): boolean {
                if (Boolean(this.$route.query.initialCreate)) {
                    return this.resourceDetails.resourceProviderId !== null;
                }
                else {

                    return this.resourceDetails.resourceProviderId >= 0;
                }
            },
            filesWithOngoingUploads(): FileModel[] {
                return FileStore.getFilesWithOngoingUploads();
            },
            allowPublish(): boolean {
                return this.contentTabComplete
                    && this.resourceAccessTabComplete
                    && this.resourceInfoTabComplete
                    && this.authorTabComplete
                    && this.licenceTabComplete
                    && this.certificateTabComplete
                    && this.locationTabComplete;
            },
            isCaseOrAssessmentResourceType(): boolean {
                return this.resourceDetails.resourceType === ResourceType.CASE
                    || this.resourceDetails.resourceType === ResourceType.ASSESSMENT;
            },
            catalogueLockedForEdit(): boolean {
                return !(this.hierarchyEdit === null)
                    && (this.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Draft
                        || this.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.SubmittedToPublishingQueue
                        || this.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Publishing);
            },
            resourceVersionIdInt(): number {
                return parseInt(this.resourceVersionId);
            },
            showProviders(): boolean {
                return this.userProviders.length > 0;
            }
        },
        methods: {
            cancelDuplicatingBlocks() {
                this.duplicatingBlocks = false;
            },
            async populateResource() {
                try {
                    const [configuration, licences, userCatalogues, userProviders, resourceDetails] = await Promise.all([
                        resourceData.getContributeConfiguration(),
                        resourceData.getLicences(),
                        resourceData.getCataloguesForUser(),
                        providerData.getProvidersForUser(),
                        resourceData.getResourceVersion(this.resourceVersionIdInt)
                    ]);
                    this.configuration = configuration;
                    this.licences = licences;
                    this.userCatalogues = userCatalogues;
                    this.resourceDetails = resourceDetails;
                    this.userProviders = userProviders;
                    console.log(userProviders);
                    await this.loadResourceDetails();
                } catch (e) {
                    console.log(e);
                }
            },
            loadHierarchyEdit() {
                // TODO: resourceCatalogueId needs to be replaced by the root node path id
                contentStructureData.getHierarchyEdit(this.resourceDetails.resourceCatalogueId).then(response => {
                    this.hierarchyEdit = response[0];
                    this.hierarchyEditLoaded = true;
                    if (this.catalogueLockedForEdit) {
                        clearInterval(this.counterInterval);
                    }
                });
            },
            async saveResourceDetails() {
                // Replaced by debounced function in created()
            },
            async saveResourceDetailsDebounced() {
                if (!this.resourceAutoSaveInProgress) {
                    this.resourceAutoSaveInProgress = true;
                    this.savingState = 'saving';
                    try {
                        await ContributeApi.saveResourceDetail(this.resourceDetails);
                        this.savingState = 'saved';
                    } finally {
                        this.resourceAutoSaveInProgress = false;
                    }
                }
            },
            async saveCaseDetails() {
                // Replaced by debounced function in created()
            },
            async saveCaseDetailsDebounced() {
                if (!this.caseOrAssessmentAutoSaveInProgress) {
                    this.caseOrAssessmentAutoSaveInProgress = true;
                    this.savingState = 'saving';
                    try {
                        await ContributeApi.saveCaseDetail(this.caseDetails);
                        this.savingState = 'saved';
                    } finally {
                        this.caseOrAssessmentAutoSaveInProgress = false;
                    }
                }
            },
            async saveAssessmentDetails() {
                // Replaced by debounced function in created()
            },
            async saveAssessmentDetailsDebounced() {
                if (!this.caseOrAssessmentAutoSaveInProgress) {
                    this.caseOrAssessmentAutoSaveInProgress = true;
                    this.savingState = 'saving';
                    try {
                        await ContributeApi.saveAssessmentDetail(this.assessmentDetails);
                        this.savingState = 'saved';
                    } finally {
                        this.caseOrAssessmentAutoSaveInProgress = false;
                    }
                }
            },
            async discardDraft() {
                this.errorMessage = undefined;
                const resourceVersionIdInt = parseInt(this.resourceVersionId);
                let deleteResult = await resourceData.deleteResourceVersion(this.resourceVersionIdInt);

                if (deleteResult.isValid) {
                    this.redirectToMyContributionsDrafts();
                } else {
                    // Card 13181 raised to display errors to user.
                    this.errorMessage = "An error occurred whilst trying to delete the resource.";
                }
            },
            async saveForLater() {
                this.redirectToMyContributionsDrafts();
            },
            async attemptPublishResource() {
                if (this.getResourceBlockCollection().missingSlides()) {
                    this.showPlaceholderModal = true;
                } else {
                    await this.publishResource();
                }
            },
            async publishResource() {
                // Call this only after the user has confirmed any modals
                this.errorMessage = undefined;
                try {
                    await publishResource(this.resourceDetails, this.userCatalogues);
                } catch (error) {
                    console.log(`Failed to publish: ${error.message}`);
                    this.errorMessage = "An error occurred whilst trying to publish the resource.";
                }
            },
            redirectToMyContributionsDrafts() {
                redirectToMyContributions('draft', this.resourceDetails, this.userCatalogues);
                if (this.resourceDetails.resourceCatalogueId > 1) { // IT1 redirect to specific node in "all content" if not working in Community Contributions
                    redirectToMyContributionsNode('allcontent', this.resourceDetails, this.userCatalogues);
                } else {
                    redirectToMyContributions('draft', this.resourceDetails, this.userCatalogues);
                }
            },
            checkForOngoingFileUploadsOnPageUnload(e: any): void {
                if (FileStore.getFilesWithOngoingUploads().length > 0) {
                    // Cancel the event
                    e.preventDefault();
                    // Chrome requires returnValue to be set
                    e.returnValue = '';
                }
            },
            toggleSlideWithAnnotations(wholeSlideImageToShow: WholeSlideImageModel, imageZone: Boolean, questionBlock: QuestionBlockModel) {
                if (wholeSlideImageToShow) {
                    this.wholeSlideImage = wholeSlideImageToShow;
                } else {
                    this.wholeSlideImage = null;
                }
                this.imageZone = imageZone;
                this.questionBlock = questionBlock;
                this.editSlideAnnotations = !this.editSlideAnnotations;

                Vue.nextTick(function () {
                    window.scrollTo(0, 0);
                });
            },
            async createDuplicate(): Promise<void> {
                console.log(`Duplicating Draft Resource Version ${this.resourceDetails.resourceVersionId}`);

                this.isDuplicatingResource = true;
                const resourceCatalogueId = this.resourceDetails.resourceCatalogueId || this.resourceDetails.publishedResourceCatalogueId || 0;

                const childResourceVersionId = await resourceData.duplicateResource(this.resourceDetails.resourceVersionId, resourceCatalogueId)
                    .then((createdId) => {
                        this.isResourceDuplicationSuccess = true;
                        this.isDuplicatingResource = false;
                        return createdId;
                    })
                    .catch(() => {
                        this.isResourceDuplicationSuccess = false;
                        this.isDuplicatingResource = false;
                    });

                if (childResourceVersionId > 0 && this.isResourceDuplicationSuccess) {
                    window.location.href = '/contribute-resource/edit/' + childResourceVersionId;
                }
            },
            async presentPreview() {
                window.location.href = `/contribute-resource/preview/${this.resourceVersionId}`;
            },
            updateAssessmentType(value: AssessmentTypeEnum) {
                this.assessmentDetails.assessmentType = value;
                this.firstTimeOpen = true;
            },
            getResourceBlockCollection() {
                switch (this.resourceDetails.resourceType) {
                    case ResourceType.CASE:
                        return this.caseDetails.blockCollection;
                    case ResourceType.ASSESSMENT:
                        return this.assessmentDetails.assessmentContent;
                    default:
                        return undefined;
                }
            },
            async loadResourceDetails() {
                switch (this.resourceDetails.resourceType) {
                    case ResourceType.CASE:
                        this.caseDetails = new CaseResourceModel(await resourceData.getCaseDetail(this.resourceVersionIdInt));
                        break;
                    case ResourceType.ASSESSMENT:
                        const assessmentDetailsObject = await resourceData.getAssessmentDetail(this.resourceVersionIdInt);
                        this.assessmentDetails = new AssessmentModel(assessmentDetailsObject);
                        break;
                    default:
                        break;
                }
            }
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-component-loading {
        display: flex;
        flex-direction: column;
        align-items: center;
        margin-top: 40px;
        color: $nhsuk-grey;

        &-spinner {
            font-size: 30px;
            margin-top: 10px;
        }
    }

    .contribute-title-wrapper {
        background-color: $nhsuk-white;
    }

    .contribute-annotation-edit {
        overflow: hidden;
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
        box-shadow: 0 -2px $govuk-focus-highlight-yellow, 0 4px $nhsuk-black;
    }

    .terms-and-conditions-reminder {
        background-color: $nhsuk-white;
        border-bottom: 1px solid $nhsuk-grey-light;
        border-top: 1px solid $nhsuk-grey-light;
    }
</style>
