<template>
    <div class="content-structure">
        <div v-if="(!isLoading && isReady)">
            <div v-if="inError" class="error-header alert alert-dismissible fade show mt-4" role="alert">
                <i class="fas fa-exclamation-triangle text-danger mr-3 mt-1" aria-hidden="true"></i>
                {{ lastErrorMessage }}
                <div class="admin-validation-summary" asp-validation-summary="All"></div>
                <button type="button" class="close" @click="removeError" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div id="content-structure-header" class="mt-5 mb-3">
                <div v-if="editMode === EditModeEnum.None && !publishInProgress">
                    <div class="d-flex">
                        <input id="editFolderStructureButton" type="button" class="btn btn-admin" @click="onEdit()" v-bind:value="editFolderStructureButtonText" :disabled="editFolderStructureButtonDisabled" />
                        <span v-if="!(hierarchyEdit === null) && hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Draft" class="ml-auto"><i class="warning-triangle fas fa-exclamation-triangle pr-3"></i>{{ hierarchyEdit.username }} is currently making changes to this folder structure</span>
                        <span v-if="!(hierarchyEdit === null) && !(hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Draft) && !(hierarchyEditLastPublished === null)" class="ml-auto mt-3">
                            Last published by {{ hierarchyEditLastPublished.username }} on {{ hierarchyEditLastPublished.lastPublishedDate  | formatDate('DD MMM YYYY') }} at {{ hierarchyEditLastPublished.lastPublishedDate  | formatDate('HH:mm') }}
                        </span>
                    </div>
                    <div v-if="rootNode.children.length === 0" class="mt-3 pt-2">
                        This catalogue does not have any folders. Folders are a way to organise resources.
                    </div>
                </div>
                <div v-if="editMode === EditModeEnum.Structure" class="d-flex">
                    <input id="publishHierarchyEdit" type="button" class="btn btn-custom-green mr-3" @click="onPublish()" value="Publish changes" />
                    <input id="cancelHierarchyEdit" type="button" class="btn btn-admin btn-cancel" @click="onCancelModal()" value="Cancel changes" />
                    <div class="ml-auto"><i class="warningTriangle fas fa-exclamation-triangle pr-3 pt-3 pl-3"></i></div>
                    <span style="max-width: 700px;">You are currently editing the folder structure. Other system admins will not be able to make changes whilst you are editing.</span>
                </div>
                <div v-if="publishInProgress">
                    <div class="mb-3"><i class="warningTriangle fas fa-exclamation-triangle pr-3 pt-3"></i><h2 class="d-inline">Publish in progress</h2></div>
                    <div>Changes to the content structure of this Catalogue are currently being published. Editing will be possible once this process has completed.</div>
                </div>
            </div>
            <div id="treeView" v-if="!publishInProgress" v-show="editMode === EditModeEnum.Structure || editMode === EditModeEnum.None || editMode === EditModeEnum.MoveNode || editMode === EditModeEnum.ReferenceNode || editMode === EditModeEnum.MoveResource" class="node-contents-treeview">
                <tree-item id="1" class="root-tree-item-container"
                           :key="rootKey"
                           :item="rootNode"
                           :expandNodes="expandNodes"
                           @delete-folder="onTreeItemDeleteFolder">
                </tree-item>
            </div>
            <div v-if="editMode === EditModeEnum.Folder" id="editFolder">
                <div class="col-12">
                    <div>
                        <i class="fa-regular fa-folder" aria-hidden="true"></i>
                        <label class="control-label">Folder title</label>
                    </div>
                    <input v-model="editingFolderNode.name" class="form-control" autocomplete="arandomstring" maxlength="255" />
                    <div class="small mt-3">
                        You have {{ folderNameCharactersRemaining }} characters remaining.
                    </div>
                </div>
                <div class="col-12" style="margin-top: 40px;">
                    <label class="control-label">Folder description (optional)</label>
                    <ckeditorwithhint :key="editingFolderNode.nodeId" :initialValue="editingFolderNode.description" :maxLength="1800" @change="changeFolderDescription"  />
                </div>
                <div class="col-12" style="margin-top: 40px;">
                    <label class="control-label">Folder location</label>
                    <div> {{ editingFolderNode.path }} </div>
                </div>
                <div class="col-12 d-flex" style="margin-top: 40px; margin-bottom: 40px;">
                    <input type="button" class="btn btn-custom-green mr-3" @click="onSaveFolderEdit()" v-bind:class="{disabled: !canSaveFolderEdit}" v-bind:disabled="!canSaveFolderEdit" value="Save changes" />
                    <input type="button" class="btn btn-admin btn-cancel" @click="onCancelFolderEdit()" value="Cancel" />

                    <span v-if="editingFolderNode.nodeVersionId != 0 && editingFolderCanBeDeleted" class="ml-auto mt-3">
                        <a class="delete-folder-link" @click.prevent="onEditFolderDeleteFolder" href="#">
                            Delete this folder <i class="fa-solid fa-trash-can delete-folder ml-2"></i>
                        </a>
                    </span>
                </div>
            </div>

            <div id="deleteFolderModal" class="modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
                <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                    <div class="modal-content">
                        <div class="modal-header alert-modal-header text-center">
                            <h2 class="heading-lg w-100"><i class="delete-folder-warning-triangle fas fa-exclamation-triangle pr-3"></i>Delete folder</h2>
                        </div>

                        <div class="modal-body alert-modal-body">
                            <div class="mt-3">You have chosen to delete the folder <span id="deleteFolderName">{{ deleteFolderName}}</span>. This folder and all it's sub folders will be deleted.</div>
                        </div>

                        <div class="modal-footer alert-modal-footer">
                            <div class="form-group col-12 p-0 m-0">
                                <div class="d-flex">
                                    <input type="button" class="btn btn-action-cancel" data-dismiss="modal" value="Cancel" />
                                    <input type="button" class="btn btn-action-red ml-auto" @click="onDeleteFolder()" value="Continue" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="cancelHierarchyEditModal" class="modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
                <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                    <div class="modal-content">
                        <div class="modal-header alert-modal-header text-center">
                            <h2 class="heading-lg w-100"><i class="cancel-hierarchy-edit-warning-triangle fas fa-exclamation-triangle pr-3"></i>Cancel changes</h2>
                        </div>

                        <div class="modal-body alert-modal-body">
                            <div class="mt-3">All changes you have made in this editing session will be lost.</div>
                        </div>

                        <div class="modal-footer alert-modal-footer">
                            <div class="form-group col-12 p-0 m-0">
                                <div class="d-flex">
                                    <input type="button" class="btn btn-action-cancel" data-dismiss="modal" value="Cancel" />
                                    <input type="button" class="btn btn-action-red ml-auto" @click="onCancelHierarchyEdit()" value="Continue" />
                                </div>
                            </div>
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
import { contentStructureData } from '../data/contentStructure';
import { HierarchyEditModel, HierarchyEditStatusEnum } from '../models/content-structure/hierarchyEditModel';
import { NodeContentAdminModel } from '../models/content-structure/NodeContentAdminModel';
import { EditModeEnum } from '../models/content-structure/editModeEnum';
import { CatalogueBasicModel } from '../models/content-structure/catalogueModel';
import { FolderNodeModel } from '../models/content-structure/folderNodeModel';
import { NodeType } from '../constants';
import CKEditorToolbar from '../models/ckeditorToolbar';
import ckeditorwithhint from '../ckeditorwithhint.vue';

export default Vue.extend({
    name: 'contentStructure',
    components: {
        'treeItem': treeItem,
        ckeditorwithhint
    },
    props: {
        readOnly: { Type: Boolean, required: false, default: true } as PropOptions<boolean>,
        expandNodes: { Type: String, required: false, default: '' } as PropOptions<string>,
    },
    watch: {
        isLoading: function (newVal, oldVal) {
            if (oldVal && !newVal) {
                this.editFolderStructureButtonText = (this.rootNode.children && this.rootNode.children.length > 0) ? "Edit folder structure" : "Create folder structure";
                this.editFolderStructureButtonDisabled = !this.canCreateEdit;
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
            HierarchyEditStatusEnum: HierarchyEditStatusEnum,
            editorConfig: { toolbar: CKEditorToolbar.default },
            deleteFolderName: '',
            editFolderStructureButtonText: '',
            editFolderStructureButtonDisabled: true,
            selectedResourceId: 0,
            rootKey: 1,
            folderDescriptionValid: true,
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
        catalogue(): CatalogueBasicModel {
            return this.$store.state.contentStructureState.catalogue;
        },
        hierarchyEdit(): HierarchyEditModel {
            return this.$store.state.contentStructureState.hierarchyEdit;
        },
        hierarchyEditLastPublished(): HierarchyEditModel {
            return this.$store.state.contentStructureState.hierarchyEditLastPublished;
        },
        rootNode(): NodeContentAdminModel {
            return this.$store.state.contentStructureState.rootNode;
        },
        moveToRootNode(): NodeContentAdminModel {
            return this.$store.state.contentStructureState.moveToRootNode;
        },
        defaultEditMode(): EditModeEnum {
            return this.$store.state.contentStructureState.defaultEditMode;
        },
        editMode(): EditModeEnum {
            return this.$store.state.contentStructureState.editMode;
        },
        editingFolderNode(): FolderNodeModel {
            return this.$store.state.contentStructureState.editingFolderNode;
        },
        editingFolderCanBeDeleted(): boolean {
            return !this.$store.state.contentStructureState.editingTreeNode.hasResourcesInBranchInd;
        },
        folderNameCharactersRemaining(): number {
            return 255 - this.editingFolderNode.name.length;
        },
        canCreateEdit(): boolean {
            return this.$store.state.contentStructureState.canCreateEdit;
        },
        publishInProgress(): boolean {
            return this.$store.state.contentStructureState.publishInProgress;
        },
        canSaveFolderEdit(): boolean {
            return this.editingFolderNode.name.trim().length > 0 && this.folderDescriptionValid;
        },
    },
    created() {
        this.$store.commit('contentStructureState/initialise', { readOnly: this.readOnly });
    },
    mounted() {

    },
    methods: {
        onCancelFolderEdit() {
            this.$store.commit('contentStructureState/cancelEdit');
        },
        onSaveFolderEdit() {
            this.$store.dispatch('contentStructureState/saveFolder');
        },
        onTreeItemDeleteFolder: function (item: NodeContentAdminModel) {
            this.$store.commit('contentStructureState/setDeletingFolder', { folderNode: item });
            this.deleteFolderName = item.name;
            $('#deleteFolderModal').modal('show');
        },
        onEditFolderDeleteFolder() {
            this.deleteFolderName = this.editingFolderNode.name;
            $('#deleteFolderModal').modal('show');
        },
        onDeleteFolder() {
            this.$store.dispatch('contentStructureState/deleteFolder');
            $('#deleteFolderModal').modal('hide');
        },
        onEdit() {
            this.editFolderStructureButtonDisabled = true;
            this.$store.dispatch('contentStructureState/createHierarchyEdit', this.catalogue.rootNodePathId);
        },
        onCancelModal() {
            $('#cancelHierarchyEditModal').modal('show');
        },
        onCancelHierarchyEdit() {
            this.$store.dispatch('contentStructureState/discardHierarchyEdit', this.hierarchyEdit.id);
            $('#cancelHierarchyEditModal').modal('hide');
        },
        onPublish() {
            this.$store.dispatch('contentStructureState/submitHierarchyEditForPublish', this.hierarchyEdit.id);
        },
        changeFolderDescription(description: string, valid: boolean) {
            this.editingFolderNode.description = description;
            this.folderDescriptionValid = valid;
        },
        removeError() {
            this.$store.commit('contentStructureState/removeError');
        }
    },
});
</script>


<style lang="scss" scoped>
    @use "../../../Styles/Abstracts/all" as *;
    
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
    
    .cancel-hierarchy-edit-warning-triangle {
        color: $nhsuk-red;
    }
    
    #deleteFolderName {
        font-weight: bold;
    }
    
    .delete-folder-warning-triangle {
        color: $nhsuk-red;
    }
    
    .delete-folder {
        cursor: pointer;
        color: $nhsuk-red;
    }
    
    .delete-folder-link {
        color: $nhsuk-red;
        text-decoration: none;
    }
    
    #publishHierarchyEdit {
        min-width: 180px;
        padding-left: 0px !important;
        padding-right: 0px !important;
    }
    
    #cancelHierarchyEdit {
        min-width: 180px;
        padding-left: 0px !important;
        padding-right: 0px !important;
    }
    
    .btn-action-red {
        background-color: $nhsuk-red !important;
        color: $nhsuk-white !important;
        font-size: 1.9rem !important;
        text-align: center !important;
        border: 5px solid $nhsuk-red !important;
        height: 40px;
        border-radius: 6px;
        min-width: 94px;
        padding: 0px 25px 0px 25px;
        i
    
        {
            font-size: 1.6rem !important;
            margin-right: 5px;
        }
    
        &:hover {
            background-color: $nhsuk-red-hover !important;
            border-color: $nhsuk-red-hover !important;
        }
    
    }
    
    .btn-action-cancel {
        background-color: #fff !important;
        color: #005EB8 !important;
        border: 1px solid $nhsuk-blue !important;
        font-size: 1.9rem !important;
        text-align: center !important;
        height: 40px;
        min-width: 108px;
        padding: 0px 25px 0px 25px;
        vertical-align: middle;
        i
    
        {
            font-size: 14px;
        }
    }

    .content-structure {
        ul {
            list-style-type: none;
        }
    }
</style>