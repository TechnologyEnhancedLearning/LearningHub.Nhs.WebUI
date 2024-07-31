import Vue from 'vue';
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
import { NodePathDisplayVersionModel } from '../models/content-structure/nodePathDisplayVersionModel';
import { ResourceReferenceDisplayVersionModel } from '../models/content-structure/resourceReferenceDisplayVersionModel';

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
    editingFolderNodeReference: NodePathDisplayVersionModel = null;
    editingResourceNodeReference: ResourceReferenceDisplayVersionModel = null;
    editingTreeNode: NodeContentAdminModel = null;
    movingResource: NodeContentAdminModel = null;
    referencingResource: NodeContentAdminModel = null;
    updatedNode: NodeContentAdminModel = null;
    inError: boolean = false;
    lastErrorMessage: string = "";
    readOnly = true;
    availableReferenceCatalogues: CatalogueBasicModel[] = null;
    rootExtReferencedNode: NodeContentAdminModel = new NodeContentAdminModel();
}

const state = new State();

function refreshHierarchyEdit(state: State) {
    state.isLoading = true;
    state.inError = false;

    contentStructureData.getHierarchyEdit(state.catalogue.rootNodePathId).then(response => {
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
                state.rootNode.nodePathId = state.catalogue.rootNodePathId;
                if (!(state.hierarchyEdit === null)) state.rootNode.hierarchyEditDetailId = state.hierarchyEdit.rootHierarchyEditDetailId;
                state.rootNode.depth = 0;
                state.rootNode.parent = null;
                state.rootNode.path = state.catalogue.name;
                state.rootNode.name = state.catalogue.name;
                state.rootNode.childrenLoaded = false;
                state.rootNode.inEdit = state.editMode == EditModeEnum.Structure;
                state.rootNode.showInTreeView = false;
                state.rootNode.isReference = false;
                state.rootNode.isResource = false;
            }).then(async y => {
                await contentStructureData.getNodeContentsAdmin(state.catalogue.rootNodePathId, state.readOnly).then(response => {
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
    await contentStructureData.getNodeContentsAdmin(node.nodePathId, state.readOnly).then(async response => {

        if (!node.childrenLoaded) {
            node.children = response;
        }
        else {
            // If node content item is not in the existing collection then add it.
            // Ensure display order, name, node path display version id and node paths are up to date (IT1)
            response.forEach((child) => {
                var existing = node.children.filter(r => (r.nodeId != null && r.nodeId == child.nodeId) || (r.resourceId != null && r.resourceId == child.resourceId))[0];
                if (existing === undefined) {
                    child.isResource = child.nodeTypeId === NodeType.Resource;
                    node.children.splice(0, 0, child);
                }
                else {
                    existing.displayOrder = child.displayOrder;
                    existing.name = child.name;
                    existing.nodePathDisplayVersionId = child.nodePathDisplayVersionId;
                    existing.nodePaths = child.nodePaths;
                    existing.isResource = child.nodeTypeId === NodeType.Resource;
                    if (child.nodePaths) {
                        existing.isReference = child.nodePaths.length > 1;
                    }
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

async function refreshNodeIfMatchingNodeId(node: NodeContentAdminModel, nodeId: number, hierarchyEditDetailId: number) {
    if (node.nodeId === nodeId && node.hierarchyEditDetailId != hierarchyEditDetailId) {
        await refreshNodeContents(node.parent, false);
    }
    if (node.childrenLoaded) {
        if (node.children.filter(c => c.hierarchyEditDetailId === hierarchyEditDetailId).length == 0) {
            for (const child of node.children) {
                await refreshNodeIfMatchingNodeId(child, nodeId, hierarchyEditDetailId);
            }
        }
    }
}

async function processChildNodeContents(child: NodeContentAdminModel, parent: NodeContentAdminModel) {
    child.parent = parent;
    child.inEdit = child.parent.inEdit;
    child.showInTreeView = true;
    child.depth = parent.depth + 1;
    child.path = child.depth === 0 ? child.name : `${child.parent.path} > ${child.name}`;
    child.isResource = child.nodeTypeId === NodeType.Resource;
    if (child.nodePaths) {
        child.isReference = child.nodePaths.length > 1;
    }
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
            parentNodePathId: payload.parentNode.nodePathId,
            path: payload.parentNode.path,
            nodePaths: payload.parentNode.nodePaths,
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
            editingFolderNode.nodePaths = payload.folderNode.nodePaths;
            state.editingFolderNode = editingFolderNode;
            state.editingTreeNode = payload.folderNode;
            state.editMode = EditModeEnum.Folder;
        });
    },
    setEditingNodePathDisplayVersion(state: State, payload: { folderNode: NodeContentAdminModel }) {
        state.inError = false;

        var nodePathDisplayVersionModel: NodePathDisplayVersionModel = new NodePathDisplayVersionModel;

        nodePathDisplayVersionModel.hierarchyEditId = state.hierarchyEdit.id;
        nodePathDisplayVersionModel.nodePathDisplayVersionId = payload.folderNode.nodePathDisplayVersionId;
        nodePathDisplayVersionModel.hierarchyEditDetailId = payload.folderNode.hierarchyEditDetailId;
        nodePathDisplayVersionModel.path = payload.folderNode.path;
        nodePathDisplayVersionModel.nodePaths = payload.folderNode.nodePaths;
        nodePathDisplayVersionModel.name = payload.folderNode.name;
        nodePathDisplayVersionModel.nodePathId = payload.folderNode.nodePathId;
        state.editingFolderNodeReference = nodePathDisplayVersionModel;
        state.editingTreeNode = payload.folderNode; // Is this needed?
        state.editMode = EditModeEnum.FolderReference;
    },
    setEditingResourceReferenceDisplayVersion(state: State, payload: { resourceNode: NodeContentAdminModel }) {
        state.inError = false;

        var resourceReferenceDisplayVersionModel: ResourceReferenceDisplayVersionModel = new ResourceReferenceDisplayVersionModel;

        resourceReferenceDisplayVersionModel.hierarchyEditId = state.hierarchyEdit.id;
        resourceReferenceDisplayVersionModel.resourceReferenceDisplayVersionId = payload.resourceNode.resourceReferenceDisplayVersionId;
        resourceReferenceDisplayVersionModel.hierarchyEditDetailId = payload.resourceNode.hierarchyEditDetailId;
        resourceReferenceDisplayVersionModel.path = payload.resourceNode.path;
        resourceReferenceDisplayVersionModel.nodePaths = payload.resourceNode.nodePaths;
        resourceReferenceDisplayVersionModel.name = payload.resourceNode.name;
        resourceReferenceDisplayVersionModel.resourceReferenceId = payload.resourceNode.resourceReferenceId;
        state.editingResourceNodeReference = resourceReferenceDisplayVersionModel;
        state.editingTreeNode = payload.resourceNode; // Is this needed?
        state.editMode = EditModeEnum.ResourceReference;
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
    setReferencingNode(state: State, payload: { node: NodeContentAdminModel }) {
        state.editingTreeNode = payload.node;
        state.editMode = EditModeEnum.ReferenceNode;
    },
    cancelReferenceNode(state: State, payload: { node: NodeContentAdminModel }) {
        state.editMode = EditModeEnum.Structure;
    },
    setReferencingExternalContent(state: State, payload: { parentNode: NodeContentAdminModel }) {
        state.editingTreeNode = payload.parentNode;
        state.editMode = EditModeEnum.ReferenceExternalContent;
    },
    canceReferencingExternalContent(state: State, payload: { node: NodeContentAdminModel }) {
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
    setReferencingResource(state: State, payload: { node: NodeContentAdminModel }) {
        state.editingTreeNode = payload.node.parent;
        state.referencingResource = payload.node;
        state.editMode = EditModeEnum.ReferenceResource;
    },
    cancelReferenceResource(state: State, payload: { node: NodeContentAdminModel }) {
        state.editMode = EditModeEnum.Structure;
        state.referencingResource = null;
    },
    setError(state: State, payload: { errorMessage: string }) {
        state.inError = true;
        state.lastErrorMessage = payload.errorMessage;
    },
    removeError(state: State) {
        state.inError = false;
        state.lastErrorMessage = null;
    },
    selectReferencableCatalogue(state: State, payload: { catalogueNodePathId: number }) {
        var selectedCatalogue = state.availableReferenceCatalogues.find(c => c.rootNodePathId === payload.catalogueNodePathId);
        state.rootExtReferencedNode = new NodeContentAdminModel();
        state.rootExtReferencedNode.nodeTypeId = NodeType.Catalogue;
        state.rootExtReferencedNode.nodeId = selectedCatalogue.nodeId;
        state.rootExtReferencedNode.nodePathId = selectedCatalogue.rootNodePathId;
        state.rootExtReferencedNode.depth = 0;
        state.rootExtReferencedNode.parent = null;
        state.rootExtReferencedNode.path = selectedCatalogue.name;
        state.rootExtReferencedNode.name = selectedCatalogue.name;
        state.rootExtReferencedNode.childrenLoaded = false;
        state.rootExtReferencedNode.inEdit = false;
        state.rootExtReferencedNode.showInTreeView = true;
        state.rootExtReferencedNode.children = [];
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
    deleteFolderReference(context: ActionContext<State, State>) {
        state.inError = false;
        state.updatedNode = null;
        contentStructureData.deleteFolderReference(state.editingTreeNode.hierarchyEditDetailId).then(async response => {
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
            state.lastErrorMessage = "Error deleting folder reference.";
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
    async referenceNode(context: ActionContext<State, State>, payload: { destinationNode: NodeContentAdminModel }) {
        state.inError = false;
        contentStructureData.referenceNode(state.editingTreeNode.hierarchyEditDetailId, payload.destinationNode.hierarchyEditDetailId).then(async response => {
            context.commit("setEditMode", EditModeEnum.Structure);
            await refreshNodeContents(payload.destinationNode, true).then(async x => {
                state.editingTreeNode.parent.childrenLoaded = false; // force reload of current now to show references
                await refreshNodeContents(state.editingTreeNode.parent, true).then(y => {
                });
            });
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = `Error referencing Node ${state.editingTreeNode.name}`;
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
    async removeReferencingNode(context: ActionContext<State, State>, payload: { node: NodeContentAdminModel }) {
        state.inError = false;
        contentStructureData.removeReferenceNode(payload.node.hierarchyEditDetailId).then(async response => {
            await refreshNodeContents(payload.node.parent, false);
            await refreshNodeContents(payload.node.parent.parent, false);
            await refreshNodeIfMatchingNodeId(payload.node.parent, state.editingTreeNode.nodeId, state.editingTreeNode.hierarchyEditDetailId);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error removing resource refrence.";
        });
    },
    async referenceResource(context: ActionContext<State, State>, payload: { destinationNode: NodeContentAdminModel }) {
        contentStructureData.hierarchyEditReferenceResource(state.referencingResource.hierarchyEditDetailId, payload.destinationNode.hierarchyEditDetailId).then(async response => {
            await refreshNodeContents(state.referencingResource.parent, true).then(async x => {
                await refreshNodeContents(payload.destinationNode, true);
            });

            context.commit("setEditMode", EditModeEnum.Structure);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error referencing resource.";
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
                if (state.editingTreeNode.nodePathDisplayVersionId == 0) {
                    state.editingTreeNode.name = state.editingFolderNode.name;
                }
                await refreshNodeIfMatchingNodeId(state.rootNode, state.editingTreeNode.nodeId, state.editingTreeNode.hierarchyEditDetailId);
                state.updatedNode = state.editingTreeNode;
                context.commit("setEditMode", EditModeEnum.Structure);
            }).catch(e => {
                state.inError = true;
                state.lastErrorMessage = "Error updating folder.";
            });
        }
    },
    async saveNodePathDisplayVersion(context: ActionContext<State, State>) {
        contentStructureData.updateNodePathDisplayVersion(state.editingFolderNodeReference).then(async response => {
            state.editingFolderNodeReference.nodePathDisplayVersionId = response.createdId;
            await refreshNodeContents(state.editingTreeNode.parent, false);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error creating folder reference.";
        });
        context.commit("setEditMode", EditModeEnum.Structure);
    },
    async saveResourceReferenceDisplayVersion(context: ActionContext<State, State>) {
        contentStructureData.updateResourceReferenceDisplayVersion(state.editingResourceNodeReference).then(async response => {
            state.editingResourceNodeReference.resourceReferenceDisplayVersionId = response.createdId;
            await refreshNodeContents(state.editingTreeNode.parent, false);
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error creating folder reference.";
        });
        context.commit("setEditMode", EditModeEnum.Structure);
    },
    async populateReferencableCatalogues(context: ActionContext<State, State>) {
        contentStructureData.getReferencableCatalogues(state.rootNode.nodePathId).then(async response => {
            state.availableReferenceCatalogues = response;

            var originalCatalogue = state.availableReferenceCatalogues.find(c => c.rootNodePathId === state.rootNode.nodePathId);
            state.rootExtReferencedNode = new NodeContentAdminModel();
            state.rootExtReferencedNode.nodeTypeId = NodeType.Catalogue;
            state.rootExtReferencedNode.nodeId = originalCatalogue.nodeId;
            state.rootExtReferencedNode.nodePathId = originalCatalogue.rootNodePathId;
            state.rootExtReferencedNode.depth = 0;
            state.rootExtReferencedNode.parent = null;
            state.rootExtReferencedNode.path = originalCatalogue.name;
            state.rootExtReferencedNode.name = originalCatalogue.name;
            state.rootExtReferencedNode.childrenLoaded = false;
            state.rootExtReferencedNode.inEdit = false;
            state.rootExtReferencedNode.showInTreeView = true;
        }).catch(e => {
            state.inError = true;
            state.lastErrorMessage = "Error populating referencable catalogues.";
        });
    },
    async referenceExternalItem(context: ActionContext<State, State>, payload: { selectedItem: NodeContentAdminModel }) {
        if (state.rootExtReferencedNode.nodePathId === state.rootNode.nodePathId) { // same as editing catalogue
            if (payload.selectedItem.nodeTypeId === NodeType.Folder) {
                // folder
                state.inError = false;
                contentStructureData.referenceNode(payload.selectedItem.hierarchyEditDetailId, state.editingTreeNode.hierarchyEditDetailId).then(async response => {
                    context.commit("setEditMode", EditModeEnum.Structure);
                    state.editingTreeNode.childrenLoaded = false; // force reload of current now to show references
                    await refreshNodeContents(state.editingTreeNode, true).then(y => {
                    });
                }).catch(e => {
                    state.inError = true;
                    state.lastErrorMessage = `Error referencing Node ${state.editingTreeNode.name}`;
                });
            } else {
                // resource
                contentStructureData.hierarchyEditReferenceResource(payload.selectedItem.hierarchyEditDetailId, state.editingTreeNode.hierarchyEditDetailId).then(async response => {
                    await refreshNodeContents(state.editingTreeNode, true).then(async x => {
                    });
                    context.commit("setEditMode", EditModeEnum.Structure);
                }).catch(e => {
                    state.inError = true;
                    state.lastErrorMessage = "Error referencing resource.";
                });
            }
        }
        else {
            if (payload.selectedItem.nodeTypeId === NodeType.Folder || payload.selectedItem.nodeTypeId === NodeType.Catalogue) {
                // folder or catalogue
                state.inError = false;
                contentStructureData.referenceExternalNode(payload.selectedItem.nodePathId, state.editingTreeNode.hierarchyEditDetailId).then(async response => {
                    context.commit("setEditMode", EditModeEnum.Structure);
                    state.editingTreeNode.childrenLoaded = false; // force reload of current now to show references
                    await refreshNodeContents(state.editingTreeNode, true).then(y => {
                    });
                }).catch(e => {
                    state.inError = true;
                    state.lastErrorMessage = `Error referencing External Node ${state.editingTreeNode.name}`;
                });
            } else {
                // resource
                contentStructureData.hierarchyEditReferenceExternalResource(payload.selectedItem.resourceId, state.editingTreeNode.hierarchyEditDetailId).then(async response => {
                    await refreshNodeContents(state.editingTreeNode, true).then(async x => {
                    });
                    context.commit("setEditMode", EditModeEnum.Structure);
                }).catch(e => {
                    state.inError = true;
                    state.lastErrorMessage = "Error referencing resource.";
                });
            }
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