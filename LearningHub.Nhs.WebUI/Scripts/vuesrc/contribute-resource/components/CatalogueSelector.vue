<template>
    <div class="catalogue-selector p-24">
        <div class="mb-3">Select a catalogue to add this resource to.</div>
        
        <CatalogueSelectorAccordion/>
        
        <div class="form-group">
            <select class="form-control" aria-labelledby="primary-catalogue-heading" v-model="selectedCatalogue" @change="catalogueChange">
                <option disabled v-bind:value="{ nodeId: 0 }">Please choose...</option>
                <option v-for="catalogue in userCatalogues" :value="catalogue">
                    {{getCatalogueName(catalogue)}}
                </option>
            </select>
        </div>
        
        <div class="warning-content" v-if="selectedCatalogue.hidden">
            <span>
                You have selected a hidden catalogue. If you publish this resource, learners will only be able
                to access it when the catalogue is made available by Learning Hub platform administrators.
            </span>
        </div>
        
        <Button color="green" size="thin" class="save-button mt-20" v-on:click="savePrimaryCatalogue" v-bind:disabled="!selectedCatalogueIsValid">Save</Button>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import Button from "../../globalcomponents/Button.vue";
    import CatalogueSelectorAccordion from "./CatalogueSelectorAccordion.vue"
    
    import { CatalogueModel } from "../../models/catalogueModel";
    import { ContributeResourceDetailModel } from "../../models/contribute/contributeResourceModel";
    
    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>,
            userCatalogues: { type: Array } as PropOptions<CatalogueModel[]>,
        },
        components: {
            Button,
            CatalogueSelectorAccordion,
        },
        data() {
            return {
                selectedCatalogue: new CatalogueModel({ nodeId: 0 })
            }
        },
        computed: {
            selectedCatalogueIsValid(): boolean {
                return this.selectedCatalogue.nodeId > 0;
            },           
        },
        created() {
            if (this.resourceDetails.resourceCatalogueId > 0) {
                this.selectedCatalogue = this.userCatalogues.find(c => c.nodeId == this.resourceDetails.resourceCatalogueId);
            }
        },
        methods: {
            getCatalogueName(catalogue: CatalogueModel) {
                if (catalogue.hidden) {
                    return catalogue.name + ' ** Hidden **';
                } else {
                    return catalogue.name;
                }
            },
            catalogueChange() {
                this.$emit('input', this.selectedCatalogue.nodeId);
            },
            savePrimaryCatalogue() {
                this.resourceDetails.resourceCatalogueId = this.selectedCatalogue.nodeId
                this.resourceDetails.nodeId = this.selectedCatalogue.nodeId
                this.resourceDetails.primaryCatalogueNodeId = this.selectedCatalogue.nodeId
                this.$emit('save');
            }
        },
        
    })
</script>

<style lang="scss">
    @use '../../../../Styles/abstracts/all' as *;
    
    .catalogue-selector {
        background-color: $nhsuk-white;
    }
    button.save-button {
        font-size: 19px;
        padding: 8px 20px;
        min-width: unset
    }
    select.form-control {
        border: 2px solid $nhsuk-grey;
        height: 40px !important;
    }

    .warning-content {
        background-color: rgba($nhsuk-warm-yellow, 0.05);
        border: 1px solid $nhsuk-warm-yellow;
        padding: 15px 24px;
    }
</style>