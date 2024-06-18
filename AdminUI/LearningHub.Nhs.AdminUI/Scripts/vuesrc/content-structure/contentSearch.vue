<template>
    <div class="content-structure">

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



        <div>
            editingCatalogueNodePathId = {{editingCatalogueNodePathId}}
        </div>
        <!--<div v-if="(!isLoading && isReady)">
        <div id="treeView" class="node-contents-treeview">
            <tree-item id="1" class="root-tree-item-container"
                       :key="rootKey"
                       :item="rootNode"
                       :expandNodes="expandNodes"
                       @delete-folder="onTreeItemDeleteFolder">
            </tree-item>
        </div>
    </div>-->
    </div>
</template>

<script lang="ts">
import Vue, { PropOptions } from 'vue';
import { CatalogueBasicModel } from '../models/content-structure/catalogueModel';

export default Vue.extend({
    name: 'contentSearch',
    components: {

    },
    props: {
        editingCatalogueNodePathId: { Type: Number, required: true } as PropOptions<number>,
    },
    watch: {

    },
    data: function () {
        return {
            catalogues: [] as CatalogueBasicModel[],
            selectedCatalogue: { rootNodePathId: 0, name: '' } as CatalogueBasicModel,
        };
    },
    computed: {
    },
    created() {
        //this.populateCatalogues();
    },
    mounted() {

    },
    methods: {
        populateCatalogues() {
            // TODO: Get catalogues from API
            this.catalogues = [
                { rootNodePathId: 1, name: 'Catalogue 1', catalogueNodeVersionId: 0, nodeId: 1, hidden: true, url: '', restrictedAccess: false },
                { rootNodePathId: 2, name: 'Catalogue 2', catalogueNodeVersionId: 0, nodeId: 2, hidden: true, url: '', restrictedAccess: false },
                { rootNodePathId: 3, name: 'Catalogue 3', catalogueNodeVersionId: 0, nodeId: 3, hidden: true, url: '', restrictedAccess: false },
                { rootNodePathId: 4, name: 'Catalogue 4', catalogueNodeVersionId: 0, nodeId: 4, hidden: true, url: '', restrictedAccess: false },
                { rootNodePathId: 5, name: 'Catalogue 5', catalogueNodeVersionId: 0, nodeId: 5, hidden: true, url: '', restrictedAccess: false },
            ];
        },
        catalogueChange() {
            // TODO: Process selected catalogue
            console.log('catalogueChange: ' + this.selectedCatalogue.name);
        },
    },
});
</script>


<style lang="scss" scoped>
    @use "../../../Styles/Abstracts/all" as *;
    


</style>