<template>
    <div class="content-structure">
        <div class="row">
            <div class="form-group col-12 d-flex flex-row flex-grow-1">
                <label class="">Reference location: {{pathLocation}}</label>
                <input id="btnCancelReferencing" type="button" class="btn btn-admin btn-cancel ml-auto" @click="cancelReferencing()" value="Cancel Referencing" />
            </div>
        </div>
        <div class="row">
            <div class="form-group col-12">
                <label for="catalogueSelect" class="pt-10">Catalogue</label>
                <select id="catalogueSelect" class="form-control" aria-labelledby="type-label" v-model="selectedCatalogue" @change="catalogueChange">
                    <option disabled v-bind:value="{ nodeId: 0 }">Please choose catalogue...</option>
                    <option v-for="catalogue in catalogues" :value="catalogue">
                        {{catalogue.name}}
                    </option>
                </select>
            </div>
        </div>

        <div class="content-structure">
            <div id="treeView" class="node-contents-treeview">
                <content-search-item id="1" class="root-tree-item-container"
                                     :key="1"
                                     :item="rootNode">
                </content-search-item>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue, { PropOptions } from 'vue';
import contentSearchItem from './contentSearchItem.vue';
import { CatalogueBasicModel } from '../models/content-structure/catalogueModel';
import { NodeContentAdminModel } from '../models/content-structure/NodeContentAdminModel';

export default Vue.extend({
    name: 'contentSearch',
    components: {
        'contentSearchItem': contentSearchItem,
    },
    props: {
        editingCatalogueNodePathId: { Type: Number, required: true } as PropOptions<number>,
    },
    watch: {
        catalogues: function (newVal, oldVal) {
            this.selectedCatalogue = this.catalogues.find(c => c.rootNodePathId === this.editingCatalogueNodePathId);
        }
    //    selectedCatalogue: function (newVal, oldVal) {
    //        alert('selectedCatalogue: ' + newVal.name);
    //    }
    },
    data: function () {
        return {
            selectedCatalogue: { rootNodePathId: 0, name: '' } as CatalogueBasicModel,
        };
    },
    computed: {
        catalogues(): CatalogueBasicModel[] {
            return this.$store.state.contentStructureState.availableReferenceCatalogues;
        },
        rootNode(): NodeContentAdminModel {
            return this.$store.state.contentStructureState.rootExtReferencedNode;
        },
        pathLocation(): string {
            return this.$store.state.contentStructureState.editingTreeNode.path;
        },
    },
    created() {
        this.$store.dispatch('contentStructureState/populateReferencableCatalogues', { editingCatalogueNodePathId: this.editingCatalogueNodePathId });
    },
    mounted() {

    },
    methods: {
        catalogueChange() {
            this.$store.commit('contentStructureState/selectReferencableCatalogue', { catalogueNodePathId: this.selectedCatalogue.rootNodePathId });
        },
        cancelReferencing() {
            this.$store.commit('contentStructureState/canceReferencingExternalContent');
        },
    },
});
</script>


<style lang="scss" scoped>
    @use "../../../Styles/Abstracts/all" as *;


    .node-contents-treeview {
        border-bottom: 1px solid $nhsuk-grey-divider;
    }

    .content-structure {
        ul {
            list-style-type: none;
        }
    }


</style>