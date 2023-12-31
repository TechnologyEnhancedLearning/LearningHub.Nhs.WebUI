﻿import Vue from 'vue';
import AxiosWrapper from '../axiosWrapper';
import * as _ from "lodash";
import Vuex, { ActionContext, Store } from 'vuex';
import { GetterTree, MutationTree, ActionTree } from "vuex";
import { contentStructureData } from '../data/contentStructure';
import { NodeContentAdminModel } from '../models/content-structure/NodeContentAdminModel';
import { EditModeEnum } from '../models/content-structure/editModeEnum';
import { HierarchyEditStatusEnum, HierarchyEditModel } from '../models/content-structure/hierarchyEditModel';
import { NodeType } from '../constants';
import { CatalogueBasicModel } from '../models/content-structure/catalogueModel';
import { FolderNodeModel } from '../models/content-structure/folderNodeModel';
import { forEach } from 'lodash';

Vue.use(Vuex);

export class State {
    isLoading: boolean = true;
    canCreateEdit: boolean = false;
    publishInProgress: boolean = false;
    catalogue: CatalogueBasicModel = null;
    hierarchyEdit: HierarchyEditModel = new HierarchyEditModel();
    hierarchyEditLastPublished: HierarchyEditModel = new HierarchyEditModel();
    rootNode: NodeContentAdminModel = new NodeContentAdminModel();
    moveToRootNode: NodeContentAdminModel = new NodeContentAdminModel();
    editMode: EditModeEnum = EditModeEnum.None;
    editingFolderNode: FolderNodeModel = null;
    editingTreeNode: NodeContentAdminModel = null;
    movingResource: NodeContentAdminModel = null;
    updatedNode: NodeContentAdminModel = null;
    inError: boolean = false;
    lastErrorMessage: string = "";
    readOnly = true;
}

const state = new State();

function refreshHierarchyEdit(state: State) {
    state.isLoading = true;
    state.inError = false;

    contentStructureData.getHierarchyEdit(state.catalogue.nodeId).then(response => {
        state.hierarchyEdit = response[0];
        state.hierarchyEditLastPublished = response[1];
    }).then(x => {
        contentStructureData.getCurrentUserId()
            .then(x => {
                var hierarchyEditExists = !(state.hierarchyEdit === null);
                state.canCreateEdit = (!hierarchyEditExists
                    || state.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Published)
                    || (state.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Discarded)
                    || (state.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.FailedToPublish);

                state.publishInProgress = hierarchyEditExists
                    && ((state.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.SubmittedToPublishingQueue)
                        || (state.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Publishing));

                state.editMode = (hierarchyEditExists && state.hierarchyEdit.userId === x && state.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Draft) ? EditModeEnum.Structure : EditModeEnum.None;
                state.readOnly = !(hierarchyEditExists && state.hierarchyEdit.userId === x && state.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Draft);
            }).then(async x => {
                state.rootNode = new NodeContentAdminModel();
                state.rootNode.nodeTypeId = NodeType.Catalogue;
                state.rootNode.nodeId = state.catalogue.nodeId;
                if (!(state.hierarchyEdit === null)) state.rootNode.hierarchyEditDetailId = state.hierarchyEdit.rootHierarchyEditDetailId;
                state.rootNode.depth = 0;
                state.rootNode.parent = null;
                state.rootNode.path = state.catalogue.name;
                state.rootNode.name = state.catalogue.name;
                state.rootNode.childrenLoaded = false;
                state.rootNode.inEdit = state.editMode == EditModeEnum.Structure;
                state.rootNode.showInTreeView = false;
            }).then(async y => {
                await contentStructureData.getNodeContentsAdmin(state.catalogue.nodeId, state.readOnly).then(response => {
                    state.rootNode.children = response;
                    state.rootNode.children.forEach((child) => {
                        processChildNodeContents(child, state.rootNode);
                    });
                    state.rootNode.childrenLoaded = true;
                    state.isLoading = false;
                }).catch(e => {
                    state.inError = true;
                    state.isLoading = false;
                    state.lastErrorMessage = "Error loading content structure.";
                });
            });
    });
}

async function refreshNodeContents(node: NodeContentAdminModel, refreshParentPath: boolean) {
    state.updatedNode = null;
    await contentStructureData.getNodeContentsAdmin(node.nodeId, state.readOnly).then(async response => {

        if (!node.childrenLoaded) {
            node.children = response;
        }
        else {
            // If node content item is not in the existing collection then add it.
            // Ensure display order and name are up to date (IT1)
            response.forEach((child) => {
                var existing = node.children.filter(r => (r.nodeId != null && r.nodeId == child.nodeId) || (r.resourceId != null && r.resourceId == child.resourceId))[0];
                if (existing === undefined) {
                    node.children.splice(0, 0, child);
                }
                else {
                    existing.displayOrder = child.displayOrder;
                    existing.name = child.name;
                }
            });

            // If existing child is not in new node content list then remove it it.
            for (var i = 0; i < node.children.length; i++) {
                var existing = response.filter(r => (r.nodeId != null && r.nodeId == node.children[i].nodeId) || (r.resourceId != null && r.resourceId == node.children[i].resourceId))[0];
                if (existing === undefined) {
                    node.children.splice(i, 1);
                }
            }
        }

        node.children.forEach((child) => {
            processChildNodeContents(child, node);
        });
        node.childrenLoaded = true;

        node.hasResourcesInd = node.children.filter(c => c.nodeTypeId === 0).length > 0;
        node.hasResourcesInBranchInd = node.hasResourcesInd || node.children.filter(c => c.hasResourcesInBranchInd).length > 0;

        state.updatedNode = node;

    }).then(async response => { // IT1 - refresh "hasResourcesInBranchInd" on parent path after 'move node'
        if (refreshParentPath && node.depth > 0) {
            var parent = node.parent;
            while (parent.depth > 0) {

                var hadResources = parent.hasResourcesInBranchInd;
                var hasResources = parent.hasResourcesInd || parent.children.filter(c => c.hasResourcesInBranchInd).length > 0;

                if (hadResources != hasResources) {
                    parent.hasResourcesInBranchInd = hasResources;
                    state.updatedNode = parent;
                }

                parent = parent.parent;
            }
        }
    }).catch(e => {
        state.inError = true;
        state.lastErrorMessage = `Error refreshing Node contents of ${node.name}`;
    });
}

async function processChildNodeContents(child: NodeContentAdminModel, parent: NodeContentAdminModel) {
    child.parent = parent;
    child.inEdit = child.parent.inEdit;
    child.showInTreeView = true;
    child.depth = parent.depth + 1;
    child.path = child.depth === 0 ? child.name : `${child.parent.path} > ${child.name}`;
}

const mutations = {
    initialise(state: State, payload: { readOnly: boolean }) {
        state.readOnly = payload.readOnly;
        state.rootNode.nodeTypeId = 1;
    },
    setEditMode(state: State, payload: EditModeEnum) {
        state.editMode = payload;
    },
    setCreatingFolder(state: State, payload: { parentNode: NodeContentAdminModel }) {
        state.inError = false;
        var editingFolderNode: FolderNodeModel =
        {
            hierarchyEditId: state.hierarchyEdit.id,
            hierarchyEditDetailId: 0,
            nodeId: 0,
            nodeVersionId: 0,
            name: "",
            description: "",
            parentNodeId: payload.parentNode.nodeId,
            path: payload.parentNode.path,
            parentNode: null
        }
        state.editingFolderNode = editingFolderNode;
        state.editingTreeNode = payload.parentNode;
        state.editMode = EditModeEnum.Folder;
        state.updatedNode = null;
    },
    setEditingFolder(state: State, payload: { folderNode: NodeContentAdminModel }) {
        state.inError = false;
        contentStructureData.getFolder(payload.folderNode.nodeVersionId).then(response => {
            var editingFolderNode = response;

            editingFolderNode.hierarchyEditId = state.hierarchyEdit.id;
            editingFolderNode.hierarchyEditDetailId = payload.folderNode.hierarchyEditDetailId;
            editingFolderNode.path = payload.folderNode.path;
            state.editingFolderNode = editingFolderNode;
            state.editingTreeNode = payload.folderNode;
            state.editMode = EditModeEnum.Folder;
        });
    },
    setDeletingFolder(state: State, payload: { folderNode: NodeContentAdminModel }) {
        state.editingTreeNode = payload.folderNode;
    },
    setMovingNode(state: State, payload: { node: NodeContentAdminModel }) {
        state.editingTreeNode = payload.node;
        state.editMode = EditModeEnum.MoveNode;
    },
    cancelMoveNode(state: State, payload: { node: NodeContentAdminModel }) {
        state.editMode = EditModeEnum.Structure;
    },
    cancelEdit(state: State) {
        state.editMode = EditModeEnum.Structure;
    },
    setMovingResource(state: State, payload: { node: NodeContentAdminModel }) {
        state.editingTreeNode = payload.node.parent;
        state.movingResource = payload.node;
        state.editMode = EditModeEnum.MoveResource;
    },
    cancelMoveResource(state: State, payload: { node: NodeContentAdminModel }) {
        state.editMode = EditModeEnum.Structure;
        state.movingResource = null;
    },
    setError(state: State, payload: { errorMessage: string }) {
        state.inError = true;
        state.lastErrorMessage = payload.errorMessage;
    },
    removeError(state: State) {
        state.inError = false;
        state.lastErrorMessage = null;
    }

} as MutationTree<State>;

const getters = <GetterTree<State, any>>{};

const actions = <ActionTree<State, any>>{
    populateCatalogue(context: ActionContext<State, State>, payload: number) {
        state.isLoading = true;
        contentStructureData.getCatalogue(payload).then(response => {
            state.catalogue = response;
        }).then(x => {
            refreshHierarchyEdit(state);
        });
        state.isLoading = false;
    },
    createHierarchyEdit(context: ActionContext<State, State>, payload: number) {
        state.inError = false;
        contentStructureData.createHierarchyEdit(payload).then(response => {
            if (response.isValid) {
                refreshHierarchyEdit(state);
            }
            else {
                state.inError = true;
                state.lastErrorMessage = "Error: " + response.details.join(",");
            }
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error creating Hierarchy Edit.";
        });
    },
    discardHierarchyEdit(context: ActionContext<State, State>, payload: number) {
        state.inError = false;
        contentStructureData.discardHierarchyEdit(payload).then(response => {
            if (response.isValid) {
                refreshHierarchyEdit(state);
            }
            else {
                state.inError = true;
                state.lastErrorMessage = "Error: " + response.details.join(",");
            }
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error discarding Hierarchy Edit.";
        });
    },
    submitHierarchyEditForPublish(context: ActionContext<State, State>, payload: number) {
        state.inError = false;
        contentStructureData.submitHierarchyEditForPublish(payload, '').then(response => {
            if (response.isValid) {
                refreshHierarchyEdit(state);
            }
            else {
                state.inError = true;
                state.lastErrorMessage = "Error: " + response.details.join(",");
            }
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error publishing Hierarchy Edit.";
        });
    },
    deleteFolder(context: ActionContext<State, State>) {
        state.inError = false;
        state.updatedNode = null;
        contentStructureData.deleteFolder(state.editingTreeNode.hierarchyEditDetailId).then(async response => {
            if (response.isValid) {
                var parent = state.editingTreeNode.parent;
                await refreshNodeContents(parent, false);
                state.updatedNode = parent;
                context.commit("setEditMode", EditModeEnum.Structure);
            }
            else {
                state.inError = true;
                state.lastErrorMessage = "Error: " + response.details.join(",");
            }
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error deleting folder.";
        });
    },
    async moveNodeUp(context: ActionContext<State, State>, payload: { node: NodeContentAdminModel }) {
        state.inError = false;
        contentStructureData.moveNodeUp(payload.node.hierarchyEditDetailId).then(async response => {
            await refreshNodeContents(payload.node.parent, false);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error moving node up.";
        });
    },
    async moveNodeDown(context: ActionContext<State, State>, payload: { node: NodeContentAdminModel }) {
        state.inError = false;
        contentStructureData.moveNodeDown(payload.node.hierarchyEditDetailId).then(async response => {
            await refreshNodeContents(payload.node.parent, false);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error moving node down.";
        });
    },
    async moveNode(context: ActionContext<State, State>, payload: { destinationNode: NodeContentAdminModel }) {
        state.inError = false;
        contentStructureData.moveNode(state.editingTreeNode.hierarchyEditDetailId, payload.destinationNode.hierarchyEditDetailId).then(async response => {
            context.commit("setEditMode", EditModeEnum.Structure);
            await refreshNodeContents(payload.destinationNode, true).then(async x => {
                await refreshNodeContents(state.editingTreeNode.parent, true).then(y => {
                });
            });
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = `Error moving Node ${state.editingTreeNode.name}`;
        });
    },
    async moveResourceUp(context: ActionContext<State, State>, payload: { node: NodeContentAdminModel }) {
        contentStructureData.hierarchyEditMoveResourceUp(payload.node.hierarchyEditDetailId).then(async response => {
            await refreshNodeContents(payload.node.parent, false);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error moving resource up.";
        });
    },
    async moveResourceDown(context: ActionContext<State, State>, payload: { node: NodeContentAdminModel }) {
        contentStructureData.hierarchyEditMoveResourceDown(payload.node.hierarchyEditDetailId).then(async response => {
            await refreshNodeContents(payload.node.parent, false);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error moving resource down.";
        });
    },
    async moveResource(context: ActionContext<State, State>, payload: { destinationNode: NodeContentAdminModel }) {
        contentStructureData.hierarchyEditMoveResource(state.movingResource.hierarchyEditDetailId, payload.destinationNode.hierarchyEditDetailId).then(async response => {
            await refreshNodeContents(state.movingResource.parent, true).then(async x => {
                await refreshNodeContents(payload.destinationNode, true);
            });

            context.commit("setEditMode", EditModeEnum.Structure);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error moving resource.";
        });
    },
    async saveFolder(context: ActionContext<State, State>) {
        if (state.editingFolderNode.nodeVersionId == 0) {
            contentStructureData.createFolder(state.editingFolderNode).then(async response => {
                await refreshNodeContents(state.editingTreeNode, false);
            }).catch(e => {
                state.inError = true;
                state.lastErrorMessage = "Error creating folder.";
            });
            context.commit("setEditMode", EditModeEnum.Structure);
        }
        else {
            contentStructureData.updateFolder(state.editingFolderNode).then(async response => {
                state.editingTreeNode.name = state.editingFolderNode.name;
                state.updatedNode = state.editingTreeNode;
                context.commit("setEditMode", EditModeEnum.Structure);
            }).catch(e => {
                state.inError = true;
                state.lastErrorMessage = "Error updating folder.";
            });
        }
    }
};

export const contentStructureState = {
    namespaced: true,
    state,
    mutations,
    actions,
    getters
}

export default new Vuex.Store({
    modules: {
        contentStructureState
    }
});