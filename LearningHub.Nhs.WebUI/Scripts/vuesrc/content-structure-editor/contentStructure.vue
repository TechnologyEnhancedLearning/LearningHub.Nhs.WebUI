<template>
    <div class="content-structure">
        <div v-if="!isLoading && isReady">
            <div v-if="inError" class="error-header alert alert-dismissible fade show mt-4" role="alert">
                <i class="fa-solid fa-triangle-exclamation text-danger mr-3 mt-1" aria-hidden="true"></i>
                {{ lastErrorMessage }}
                <div class="admin-validation-summary" asp-validation-summary="All"></div>
                <button type="button" class="close" @click="removeError" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>

            <div id="treeView" v-show="editMode === EditModeEnum.Structure || editMode === EditModeEnum.None || editMode === EditModeEnum.MoveNode || editMode === EditModeEnum.MoveResource" class="node-contents-treeview">
                <tree-item id="1" class="root-tree-item-container"
                           :key="rootKey"
                           :item="rootNode"
                           :expandNodes="expandNodes"
                           :readOnly="readOnly"
                           @promptEditResource="promptEditResource">
                </tree-item>
            </div>

            <div id="createResourceEdit" ref="createResourceEditModal" class="modal create-edit-resource-modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
                <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                    <div class="modal-content">
                        <div class="modal-header text-center">
                            <h2>Edit resource</h2>
                        </div>

                        <div class="modal-body">
                            You are about to make a change to this resource. If you continue, changes cannot be undone.
                        </div>

                        <div class="modal-footer">
                            <button class="nhsuk-button nhsuk-button--secondary" data-dismiss="modal">Cancel</button>
                            <button class="nhsuk-button" @click="createResourceVersion">Continue</button>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import treeItem from './treeItem.vue';
    import '../filters';
    import { EditModeEnum } from '../models/content-structure/editModeEnum';
    import { NodeContentEditorModel } from '../models/content-structure/nodeContentEditorModel';
    import { CatalogueBasicModel } from '../models/content-structure/catalogueModel';
    import { FolderNodeModel } from '../models/content-structure/folderNodeModel';

    export default Vue.extend({
        name: 'contentStructure',
        components: {
            'treeItem': treeItem
        },
        props: {
            readOnly: { Type: Boolean, required: false, default: true } as PropOptions<boolean>,
            expandNodes: { Type: String, required: false, default: '' } as PropOptions<string>,
        },
        watch: {
            isLoading: function (newVal, oldVal) {
                if (oldVal && !newVal) {
                    this.isReady = true;
                }
            },

            rootNode: function (newVal, oldVal) {
                console.log('rootNode=' + newVal.nodeId);
                this.rootKey++;
            }

        },
        data() {
            return {
                isReady: false,
                EditModeEnum: EditModeEnum,
                selectedResourceId: 0,
                rootKey: 1,
            }
        },
        computed: {
            isLoading(): boolean {
                return this.$store.state.contentStructureState.isLoading;
            },
            inError(): boolean {
                return this.$store.state.contentStructureState.inError;
            },
            lastErrorMessage(): boolean {
                return this.$store.state.contentStructureState.lastErrorMessage;
            },
            //catalogue(): CatalogueBasicModel {
            //    return this.$store.state.contentStructureState.catalogue;
            //},
            rootNode(): NodeContentEditorModel {
                return this.$store.state.contentStructureState.rootNode;
            },
            moveToRootNode(): NodeContentEditorModel {
                return this.$store.state.contentStructureState.moveToRootNode;
            },
            //defaultEditMode(): EditModeEnum {
            //    return this.$store.state.contentStructureState.defaultEditMode;
            //},
            editMode(): EditModeEnum {
                return this.$store.state.contentStructureState.editMode;
            },
            editingFolderNode(): FolderNodeModel {
                return this.$store.state.contentStructureState.editingFolderNode;
            },
        },
        created() {
        },
        mounted() {
        },
        methods: {
            promptEditResource(resourceId: number) {
                this.selectedResourceId = resourceId;
                $('#createResourceEdit').modal('show');
            },
            createResourceVersion() {
                $('#createResourceEdit').modal('hide');
                window.location.pathname = './Contribute/create-version/' + this.selectedResourceId.toString();
            },
            removeError() {
                this.$store.commit('contentStructureState/removeError');
            }
        },
    });
</script>


<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .node-contents-treeview {
        border-bottom: 1px solid $nhsuk-grey-divider;
    }

    .error-header {
        border: 2px solid $nhsuk-red;
        border-radius: 0;
        margin-bottom: 20px !important;
        background-color: $nhsuk-grey-white;
        font-size: 1.6rem;
    }
</style>