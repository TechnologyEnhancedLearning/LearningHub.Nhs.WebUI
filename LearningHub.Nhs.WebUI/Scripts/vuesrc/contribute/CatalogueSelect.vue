<template>
    <div>
        <div class="row mt-5">
            <div class="col-12">
                <h2 id="catalogue-label" class="nhsuk-heading-l">Catalogue<i v-if="selectedCatalogue.nodeId == 0" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h2>
                <div class="mb-3">Select a catalogue to add this resource to.</div>
            </div>
        </div>
        <div class="row my-2">
            <div class="accordion col-12" id="cat-accordion">
                <div class="pt-0 pb-4">
                    <div class="heading" id="headingCat">
                        <div class="mb-0">
                            <a href="#" class="collapsed" data-toggle="collapse" data-target="#collapseCat" aria-expanded="false" aria-controls="collapseCat">
                                <div class="accordion-arrow">More information on catalogues</div>
                            </a>
                        </div>
                    </div>
                    <div id="collapseCat" class="collapse" aria-labelledby="headingCat" data-parent="#cat-accordion">
                        <div class="content col-12">
                            <p>
                                A catalogue is a curated set of resources that has its own web page.
                            </p>
                            <p>
                                You can contribute a resource as an editor of a catalogue or in your own name.
                                To contribute a resource in your own name, select Community contributions from the drop down menu.
                            </p>
                            <p>
                                You can manage all resources that you have contributed in the My contributions area.
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="form-group col-12">
                <select class="form-control" aria-labelledby="type-label" v-model="selectedCatalogue" @change="catalogueChange">
                    <option disabled v-bind:value="{ nodeId: 0 }">Please choose...</option>
                    <option v-for="catalogue in userCatalogues" :value="catalogue">
                        {{getCatalogueName(catalogue)}}
                    </option>
                </select>
            </div>
            <div v-if="selectedCatalogue.hidden" class="highlighted-info">
                You have selected a hidden catalogue.
                If you publish this resource, learners will only be able to access it when the catalogue is made available by Learning Hub platform administrators.
            </div>
        </div>
    </div>

</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { CatalogueModel } from '../models/catalogueModel';

    export default Vue.extend({
        props: {
            value: Number as PropOptions<number>
        },
        components: {
        },
        data() {
            return {
                selectedCatalogue: new CatalogueModel({ nodeId: 0 })
            };
        },
        computed: {
            userCatalogues(): CatalogueModel[] {
                return this.$store.state.userCatalogues;
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
        },
        created() {
            if (this.value > 0) {
                this.selectedCatalogue = this.userCatalogues.find(c => c.nodeId == this.value);
            }
        }
    })

</script>

