<template>
    <div class="white-bg">
        <div class="menu-buttons" v-if="isLoaded">
            <div class="lh-container-xl d-flex flex-row justify-content-end">
                <Button size="medium" @click="continueEditing" class="mx-12">
                    <i class="fa fa-chevron-left fa-chevron-left--normal mr-2"></i>Continue editing {{ resourceName }}
                </Button>                
                <Button size="medium" color="green" @click="attemptPublishResource" class="mx-12">Publish</Button>
            </div>
        </div>
        <div class="lh-padding-fluid">
            <div class="lh-container-xl d-flex flex-row justify-content-between align-items-center">
                <div class="resource-panel-container">
                    <div class="resource-item-row" v-if="isLoaded">
                        <!-- title -->
                        <div class="resource-item-row">
                            <h1 class="nhsuk-heading-xl" v-if="resourceDetails.title">{{ resourceDetails.title }}</h1>
                        </div>
                        <!-- description -->
                        <div class="resource-item-row mb-xs-0 mb-lg-32">
                            <div v-if="resourceDetails.description && resourceDetails.resourceTypeEnum !== ResourceType.ARTICLE"
                                 v-html="resourceDetails.description"/>
                        </div>
                        <div class="resource-item-row border-bottom pb-0 d-xs-block d-lg-none"></div>

                        <sensitive-content v-if="this.acceptContentPending"
                                           @acceptcontent="() => this.acceptContentPending = false"></sensitive-content>
                        <div v-else class="col-12 d-flex flex-column align-items-left p-0">
                            <CaseOrAssessmentResource :resourceItem="resourceItem" :keepUserSessionAliveIntervalSeconds="keepUserSessionAliveIntervalSeconds"></CaseOrAssessmentResource>
                        </div>
                    </div>
                    <div v-else
                         class="contribute-component-loading">
                        <div>Loading...</div>
                        <div class="contribute-component-loading-spinner">
                            <Spinner></Spinner>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <PublishWithPlaceholderModal v-if="showPlaceholderModal"
                                     v-on:closePlaceholderModal="showPlaceholderModal = false"
                                     v-on:proceedWithPublishing="publishResource"/>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { CaseResourceModel } from '../models/contribute-resource/caseResourceModel';
    import CaseOrAssessmentResource from '../resource/CaseOrAssessmentResource.vue';
    import { resourceData } from '../data/resource';
    import { ResourceType } from '../constants';
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
    import Spinner from '../globalcomponents/Spinner.vue';
    import Button from '../globalcomponents/Button.vue';
    import SensitiveContent from '../resource/SensitiveContent.vue';
    import { publishResource } from './helpers/Publishing';
    import PublishWithPlaceholderModal from './PublishWithPlaceholderModal.vue';
    import { CatalogueBasicModel } from '../models/catalogueModel';
    import { AssessmentModel } from "../models/contribute-resource/assessmentModel";
    import { ResourceItemModel } from "../models/resourceItemModel";
    import { getDisplayNameForResourceType } from "../resource/helpers/resourceHelper";

    export default Vue.extend({
        components: {
            CaseOrAssessmentResource,
            Spinner,
            Button,
            SensitiveContent,
            PublishWithPlaceholderModal,
        },
        props: {
            resourceVersionId: String,
            keepUserSessionAliveIntervalSeconds: Number,
        },
        data() {
            return {
                details: null as (CaseResourceModel | AssessmentModel),
                resourceDetails: null as ContributeResourceDetailModel,
                userCatalogues: null as CatalogueBasicModel[],
                acceptContentPending: true,
                showPlaceholderModal: false,
                ResourceType,
            }
        },
        computed: {
            isLoaded(): boolean {
                return !!this.details;
            },
            isCaseResourceType(): boolean {
                return this.resourceDetails.resourceType === ResourceType.CASE;
            },
            resourceItem(): Partial<ResourceItemModel> {
                if (this.isCaseResourceType) {
                    return {
                        caseDetails: this.details as CaseResourceModel,
                        resourceTypeEnum: ResourceType.CASE
                    };
                } else {
                    return {
                        assessmentDetails: this.details as AssessmentModel,
                        resourceTypeEnum: ResourceType.ASSESSMENT
                    };
                }
            },
            resourceName(): string {
                return getDisplayNameForResourceType(this.resourceDetails.resourceType);
            }
        },
        async created() {
            await this.populateResource();
        },
        methods: {
            async populateResource() {
                const resourceVersionIdInt = parseInt(this.resourceVersionId);

                try {
                    const [userCatalogues, resourceDetails] = await Promise.all([
                        resourceData.getCataloguesForUser(),
                        resourceData.getResourceVersion(resourceVersionIdInt)
                    ]);
                    this.userCatalogues = userCatalogues;
                    this.resourceDetails = resourceDetails;

                    this.acceptContentPending = this.resourceDetails.sensitiveContent;

                    switch (this.resourceDetails.resourceType) {
                        case ResourceType.CASE:
                            this.details = new CaseResourceModel(await resourceData.getCaseDetail(resourceVersionIdInt));
                            break;
                        case ResourceType.ASSESSMENT:
                            this.details = new AssessmentModel(await resourceData.getAssessmentDetail(resourceVersionIdInt));
                            break;
                        default:
                            break;
                    }
                } catch (e) {
                    console.log(e);
                }
            },
            async attemptPublishResource() {
                if (this.details
                    && this.resourceDetails.resourceType == ResourceType.CASE && (this.details as CaseResourceModel).blockCollection.missingSlides()
                    || this.resourceDetails.resourceType == ResourceType.ASSESSMENT && (this.details as AssessmentModel).assessmentContent.missingSlides()) {
                    this.showPlaceholderModal = true;
                } else {
                    await this.publishResource();
                }
            },
            async publishResource() {
                // Call this only after the user has confirmed any modals
                try {
                    await publishResource(this.resourceDetails, this.userCatalogues);
                } catch (error) {
                    console.log(`Failed to publish: ${error.message}`);
                }
            },
            async continueEditing() {
                window.location.href = `/contribute-resource/edit/${this.resourceVersionId}`;
            },
        }
    });
</script>
<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .resource-item-row {
        margin-top: 32px;
    }

    .white-bg {
        background: white;
    }

    .resource-panel-container {
        width: 100%;
        padding: 15px 0;
    }

    .contribute-component-loading {
        display: flex;
        flex-direction: column;
        align-items: center;
        margin: 40px 0;
        color: $nhsuk-grey;

        &-spinner {
            font-size: 30px;
            margin-top: 10px;
        }
    }

    .menu-buttons {
        border-bottom: 1px solid $nhsuk-grey-light;
        width: 100%;
        padding: 15px 0;
    }

    .fa-chevron-left--normal
    {
        line-height:revert;
    }
</style>
