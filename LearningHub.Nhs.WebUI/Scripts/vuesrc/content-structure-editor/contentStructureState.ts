import Vue from 'vue';
import * as _ from "lodash";
import Vuex, { ActionContext, Store } from 'vuex';
import { GetterTree, MutationTree, ActionTree } from "vuex";
import { contentStructureData } from '../data/contentStructure';
import { resourceData } from '../data/resource';
import { EditModeEnum } from '../models/content-structure/editModeEnum';
import { NodeContentEditorModel } from '../models/content-structure/nodeContentEditorModel';
import { HierarchyEditStatusEnum, HierarchyEditModel } from '../models/content-structure/hierarchyEditModel';
import { NodeType, VersionStatus } from '../constants';
import { CatalogueBasicModel } from '../models/content-structure/catalogueModel';

Vue.use(Vuex);

export class State {
    isLoading: boolean = true;
    catalogue: CatalogueBasicModel = null;
    hierarchyEdit: HierarchyEditModel = new HierarchyEditModel();
    rootNode: NodeContentEditorModel = new NodeContentEditorModel();
    editMode: EditModeEnum = EditModeEnum.None;
    editingTreeNode: NodeContentEditorModel = null;
    movingResource: NodeContentEditorModel = null;
    updatedNode: NodeContentEditorModel = null;
    inError: boolean = false;
    lastErrorMessage: string = "";
    hasExternalReference: boolean = false;
}

const state = new State();

function loadNodeContents(state: State) {
    state.isLoading = true;
    state.inError = false;

    contentStructureData.getHierarchyEdit(state.catalogue.rootNodePathId).then(response => {
        state.hierarchyEdit = response[0];
    }).then(x => {
        contentStructureData.getCurrentUserId()
            .then(x => {
                var hierarchyEditExists = !(state.hierarchyEdit === null);
                state.editMode = hierarchyEditExists && state.hierarchyEdit.hierarchyEditStatus !== HierarchyEditStatusEnum.Discarded &&
                    state.hierarchyEdit.hierarchyEditStatus !== HierarchyEditStatusEnum.Published ? EditModeEnum.Structure : EditModeEnum.None;
            }).then(async x => {
                state.rootNode = new NodeContentEditorModel();
                state.rootNode.nodeTypeId = NodeType.Catalogue;
                state.rootNode.nodeId = state.catalogue.nodeId;
                state.rootNode.nodePathId = state.catalogue.rootNodePathId;
                state.rootNode.depth = 0;
                state.rootNode.parent = null;
                state.rootNode.path = state.catalogue.name;
                state.rootNode.name = state.catalogue.name;
                state.rootNode.childrenLoaded = false;
                state.rootNode.inEdit = state.editMode == EditModeEnum.Structure;
                state.rootNode.showInTreeView = false;
            }).then(async y => {
                await contentStructureData.checkCatalogueHasExternalReference(state.catalogue.nodeId).then(response => {
                    state.hasExternalReference = response;
                })
                    .catch(e => {
                        console.log(e);
                    });
            }).then(async y => {
                await contentStructureData.getNodeContentsForCatalogueEditor(state.catalogue.rootNodePathId).then(response => {
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

async function refreshNodeContents(node: NodeContentEditorModel) {
    state.updatedNode = null;
    await contentStructureData.getNodeContentsForCatalogueEditor(node.nodeId).then(async response => {

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
                    existing.versionStatusId = child.versionStatusId;
                    existing.inEdit = child.inEdit;
                    existing.resourceInEdit = child.resourceInEdit;
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
        state.updatedNode = node;

    }).catch(e => {
        state.inError = true;
        state.lastErrorMessage = `Error refreshing Node contents of ${node.name}`;
    });
}

async function processChildNodeContents(child: NodeContentEditorModel, parent: NodeContentEditorModel) {
    child.parent = parent;
    child.inEdit = child.parent.inEdit;
    child.showInTreeView = true;
    child.depth = parent.depth + 1;
    child.path = child.depth === 0 ? child.name : `${child.parent.path} > ${child.name}`;
}

function searchForResourceVersion(resourceVersionId: number, node: NodeContentEditorModel): NodeContentEditorModel {
    for (var i = 0; i < node.children.length; i++) {
        if (node.children[i].draftResourceVersionId == resourceVersionId) {
            // Found it. Return the parent node.
            return node;
        }

        if (node.children[i].nodeTypeId == NodeType.Folder && node.children[i].childrenLoaded) {
            return searchForResourceVersion(resourceVersionId, node.children[i]);
        }
    }

    return undefined;
}

const mutations = {
    setEditMode(state: State, payload: EditModeEnum) {
        state.editMode = payload;
    },
    setMovingResource(state: State, payload: { node: NodeContentEditorModel }) {
        state.editingTreeNode = payload.node.parent;
        state.movingResource = payload.node;
        state.editMode = EditModeEnum.MoveResource;
    },
    cancelMoveResource(state: State, payload: { node: NodeContentEditorModel }) {
        state.editMode = EditModeEnum.None;
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
            loadNodeContents(state);
        });
        state.isLoading = false;
    },
    async refreshNodeContents(context: ActionContext<State, State>, payload: { node: NodeContentEditorModel }) {
        await refreshNodeContents(payload.node);
    },
    async refreshContainingNodeContents(context: ActionContext<State, State>, payload: { resourceVersionId: number }) {
        // This is needed to update the content structure treeview after deleting a failed resource in the Action Required tab of My Contributions screen.
        var node = searchForResourceVersion(payload.resourceVersionId, state.rootNode);

        if (node != undefined) {
            await refreshNodeContents(node);
        }
    },
    async moveResourceUp(context: ActionContext<State, State>, payload: { node: NodeContentEditorModel }) {
        contentStructureData.moveResourceUp(payload.node.parent.nodeId, payload.node.resourceId).then(async response => {
            await refreshNodeContents(payload.node.parent);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error moving resource up.";
        });
    },
    async moveResourceDown(context: ActionContext<State, State>, payload: { node: NodeContentEditorModel }) {
        contentStructureData.moveResourceDown(payload.node.parent.nodeId, payload.node.resourceId).then(async response => {
            await refreshNodeContents(payload.node.parent);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error moving resource down.";
        });
    },
    async moveResource(context: ActionContext<State, State>, payload: { destinationNode: NodeContentEditorModel }) {
        contentStructureData.moveResource(state.movingResource.parent.nodeId, payload.destinationNode.nodeId, state.movingResource.resourceId).then(async response => {
            await refreshNodeContents(payload.destinationNode).then(async x => {
                await refreshNodeContents(state.movingResource.parent);
            });

            context.commit("setEditMode", EditModeEnum.None);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error moving resource.";
        });
    },
    async deleteResourceVersion(context: ActionContext<State, State>, payload: { node: NodeContentEditorModel }) {
        if (payload.node.versionStatusId == VersionStatus.DRAFT || payload.node.versionStatusId == VersionStatus.FAILED) {
            resourceData.deleteResourceVersion(payload.node.draftResourceVersionId).then(async response => {
                await refreshNodeContents(payload.node.parent);
            }).catch(e => {
                state.inError = true;
                state.lastErrorMessage = "Error deleting resource.";
            });
        }
    },
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