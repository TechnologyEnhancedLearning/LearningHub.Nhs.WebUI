<template>
    <div class="select-resource-page nhsuk-u-font-size-19">
        <div v-if="hierarchyEditLoaded && catalogueLockedForEdit" class="mt-4">            
            <div class="lh-padding-fluid bg-grey-white">
                <div class="lh-container-xl">
                    <div class="d-flex flex-row">
                        <i class="fa-solid fa-triangle-exclamation mr-4 mt-2 text-warning" aria-hidden="true"></i>
                        <div class="small">A Learning Hub system administrator is currently making changes to this catalogue. You can browse the catalogue but cannot add / edit or move resources at this time.</div>
                    </div>
                </div>
            </div>
        </div>

        <div v-if="hierarchyEditLoaded && !catalogueLockedForEdit">
            <div class="lh-padding-fluid grey-banner">
                <div class="lh-container-xl">
                    <div class="nhsuk-back-link nhsuk-u-padding-bottom-3 nhsuk-u-padding-top-3">           
                        <a v-bind:href="myContributionsPageUrl"> < My Contributions</a>
                    </div>
                    <div class="py-3">
                        <h1 class="nhsuk-heading-xl nhsuk-u-margin-bottom-2">Contribute a resource</h1>
                        <div>Before you can continue, give this new resource a title and select a resource type.</div>
                    </div>
                </div>
            </div>

            <SelectResourceTitle v-bind:resourceDetails="resourceDetails"
                                 v-bind:isTypeSelected="isTypeSelected"></SelectResourceTitle>

            <SelectResourceTypeContainer v-bind:resourceDetails="resourceDetails"></SelectResourceTypeContainer>

            <div class="lh-padding-fluid">
                <div class="lh-container-xl py-5">
                    <Button color="green" v-bind:disabled="(isIncomplete || isCreating)" v-on:click="createResource">Next</Button>
                    <p class="create-resource nhsuk-u-font-size-16" v-if="isCreating">Please wait, creating your resource...</p>
                </div>
            </div>
        </div>
    </div>
</template>


<script lang="ts">
    import Vue from 'vue';
    import 'core-js/features/url-search-params';

    import SelectResourceTitle from './SelectResourceTitle.vue';
    import SelectResourceTypeContainer from './SelectResourceTypeContainer.vue';
    import ContributeApi from './contributeApi';
    import { resourceData } from '../data/resource';

    import Button from '../globalcomponents/Button.vue';
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
    import { ResourceType } from '../constants';
    import { CatalogueBasicModel } from "../models/catalogueModel";

    import { contentStructureData } from '../data/contentStructure';
    import { HierarchyEditStatusEnum, HierarchyEditModel } from '../models/content-structure/hierarchyEditModel';

    export default Vue.extend({
        components: {
            Button,
            SelectResourceTitle,
            SelectResourceTypeContainer
        },
        data() {
            const resourceDetails = new ContributeResourceDetailModel();

            const params = new URLSearchParams(window.location.search);
            const catalogueId = params.get('catalogueId');
            if (catalogueId && !isNaN(parseInt(catalogueId))) {
                resourceDetails.resourceCatalogueId = parseInt(catalogueId);
                resourceDetails.nodeId = parseInt(catalogueId);
                resourceDetails.primaryCatalogueNodeId = resourceDetails.nodeId;
            }

            const nodeId = params.get('nodeId');
            if (nodeId && !isNaN(parseInt(nodeId))) {
                resourceDetails.nodeId = parseInt(nodeId);
                resourceDetails.primaryCatalogueNodeId = resourceDetails.nodeId;
            }
            
            return {
                resourceDetails: resourceDetails,
                resourceVersionId: 0,
                isCreating: false,
                userCatalogues: null as CatalogueBasicModel[],
                hierarchyEdit: null as HierarchyEditModel,
                hierarchyEditLoaded: false as boolean,
            }
        },
        computed: {
            isTypeSelected(): boolean {
                return this.resourceDetails.resourceType !== ResourceType.UNDEFINED;
            },
            isIncomplete(): boolean {
                return this.resourceDetails.title.trim().length === 0
                    || this.resourceDetails.resourceType === ResourceType.UNDEFINED;
            },
            myContributionsPageUrl() {
                let myContributionsUrl = '/my-contributions/draft';

                const catalogueId = this.resourceDetails.resourceCatalogueId;
                if (this.userCatalogues && catalogueId > 1) {
                    const catalogue: CatalogueBasicModel = this.userCatalogues.find(c => c.nodeId === catalogueId);
                    if (catalogue !== undefined) {
                        myContributionsUrl += '/' + catalogue.url;
                    }
                }

                return myContributionsUrl;
            },
            catalogueLockedForEdit(): boolean {
                return !(this.hierarchyEdit === null)
                    && (this.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Draft
                        || this.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.SubmittedToPublishingQueue
                        || this.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Publishing);
            },
        },
        async created() {
            // TODO: resourceCatalogueId needs to be replaced by the root node path id
            contentStructureData.getHierarchyEdit(this.resourceDetails.resourceCatalogueId).then(response => {
                this.hierarchyEdit = response[0];
                this.hierarchyEditLoaded = true;
            });
            this.userCatalogues = await resourceData.getCataloguesForUser();
        },
        methods: {
            async createResource() {
                this.isCreating = true;
                this.resourceVersionId = await ContributeApi.saveResourceDetail(this.resourceDetails);
                const resourceType = this.resourceDetails.resourceType;
                if (resourceType === ResourceType.CASE || resourceType === ResourceType.ASSESSMENT) {
                    window.location.href = `./edit/${this.resourceVersionId}?initialCreate=true`;
                } else {
                    window.location.href = `/Contribute/contribute-a-resource/${this.resourceVersionId}?initialCreate=true`;
                }
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .select-resource-page {
        background-color: $nhsuk-white;
    }
    .create-resource {
        margin-top: 15px;
    }
</style>

<style lang="scss">// NOTE: Not `scoped` because we want this section to apply to children 
    @use '../../../Styles/abstracts/all' as *;
    
    .accessible-link:focus {
        outline: none;
        text-decoration: none;
        color: $nhsuk-black;
        background-color: $govuk-focus-highlight-yellow;
        box-shadow: 0 -2px $govuk-focus-highlight-yellow,0 4px $nhsuk-black;
    }
</style>
