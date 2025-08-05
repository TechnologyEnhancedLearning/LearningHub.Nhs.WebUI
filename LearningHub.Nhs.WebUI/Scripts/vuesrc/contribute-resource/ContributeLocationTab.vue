<template>
    <div class="lh-padding-fluid">
        <div class="contribute-location-tab lh-container-xl py-15">
            <h2 class="nhsuk-heading-l pt-15">
                Location
            </h2>

            <!-- The following heading has an id to allow the CatalogueSelector component to reference this (using aria-labelledby) -->
            <h3 id="primary-catalogue-heading" class="nhsuk-heading-m mt-5 mb-10">Primary catalogue</h3>

            <div class="contribute-location-tab-catalogue-block">
                <!-- When catalogue has a value -->
                <template v-if="savedCatalogue">
                    <CatalogueView v-if="resourceDetails.resourceCatalogueId >= 0"
                                   :catalogue="savedCatalogue"
                                   @change-click="showSelector"
                                   :selection-in-progress="selectionInProgress"
                                   :allow-change="allowCatalogueChange"
                                   :resourceCatalogueId=resourceDetails.resourceCatalogueId />
                    <div class="contribute-location-tab-divider"
                         v-if="isDividerVisible"></div>
                    <CatalogueSelector v-if="selectionInProgress"
                                       :resource-details="resourceDetails"
                                       :user-catalogues="userCatalogues"
                                       @save="hideSelector"
                                       :resourceCatalogueId=resourceDetails.resourceCatalogueId />
                </template>

                <!-- When catalogue has no value -->
                <div v-else>
                    <p>No catalogue available.</p>
                    <CatalogueView :catalogue="savedCatalogue"
                                   @change-click="showSelector"
                                   :selection-in-progress="selectionInProgress"
                                   :allow-change="allowCatalogueChange"
                                   :resourceCatalogueId="0" />
                    <div class="contribute-location-tab-divider"
                         v-if="isDividerVisible"></div>
                    <CatalogueSelector v-if="selectionInProgress"
                                       :resource-details="resourceDetails"
                                       :user-catalogues="userCatalogues"
                                       @save="hideSelector"
                                       :resourceCatalogueId="0" />
                </div>
            </div>



        </div>
    </div>
</template>
<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import DescriptionEditor from "./components/DescriptionEditor.vue";
    import CatalogueSelector from "./components/CatalogueSelector.vue";
    import CatalogueView from "./components/CatalogueView.vue";

    import { CatalogueModel } from "../models/catalogueModel";
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';

    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>,
            userCatalogues: { type: Array } as PropOptions<CatalogueModel[]>,
        },
        components: {
            DescriptionEditor,
            CatalogueSelector,
            CatalogueView,
        },
        data() {
            return {
                selectionInProgress: null
            }
        },
        created() {
            this.selectionInProgress = this.resourceDetails.resourceCatalogueId <= 0;
        },
        computed: {
            savedCatalogue(): CatalogueModel {
                if (this.resourceDetails.resourceCatalogueId > 0) {
                    return this.userCatalogues.find(c => c.nodeId == this.resourceDetails.resourceCatalogueId);
                }

                return new CatalogueModel({ nodeId: 0 });
            },
            resourceDescription(): string {
                return this.resourceDetails.description;
            },
            isDividerVisible(): boolean {
                return this.resourceDetails.resourceCatalogueId >= 0 && this.selectionInProgress;
            },
            allowCatalogueChange(): boolean {
                return (!Boolean(this.$route.query.initialCreate));   // allow if user is contributing into the catalogue root // or if the user is editing an existing draft (initialCreate=false)
                //this.resourceDetails.resourceCatalogueId === this.resourceDetails.nodeId) ||
            },
            onChangeClick() {
                this.$emit('change-click');
            },
        },
        methods: {
            hideSelector() {
                this.selectionInProgress = false;
            },
            showSelector() {
                this.selectionInProgress = true;
            }
        },
    })
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-location-tab {
        min-height: 571px;

        &-catalogue-block {
            background-color: $nhsuk-white;
        }

        &-divider {
            border: 1px solid $nhsuk-grey-light;
            border-bottom: 0;
            margin: 0 24px;
        }
    }
</style>