import axios from 'axios';
import { CatalogueBasicModel } from '../models/content-structure/catalogueModel';
import { NodeContentAdminModel } from '../models/content-structure/NodeContentAdminModel';
import { HierarchyEditModel } from '../models/content-structure/hierarchyEditModel';
import { LearningHubValidationResultModel } from '../models/learningHubValidationResultModel';
import { FolderNodeModel } from '../models/content-structure/folderNodeModel';
import { NodePathModel } from '../models/nodePathModel';
import { NodePathDisplayVersionModel } from '../models/content-structure/nodePathDisplayVersionModel';
import { ResourceReferenceDisplayVersionModel } from '../models/content-structure/resourceReferenceDisplayVersionModel';

const getCatalogue = async function (id: number): Promise<CatalogueBasicModel> {
    return await axios.get<CatalogueBasicModel>('/api/hierarchy/GetCatalogue/' + id)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.error('getCatalogue:' + e);
            throw e;
        });
};

const getNodeContentsAdmin = async function (nodePathId: number, readOnly: boolean): Promise<NodeContentAdminModel[]> {
    return await axios.get<NodeContentAdminModel[]>('/api/hierarchy/GetNodeContentsAdmin/' + nodePathId + '/' + readOnly + `?timestamp=${new Date().getTime()}`)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getNodeContentsAdmin:' + e);
            throw e;
        });
};

const getActiveNodePathToNode = async function (nodeId: number): Promise<NodePathModel> {
    return await axios.get<NodePathModel>('/api/hierarchy/GetActiveNodePathToNode/' + nodeId + `?timestamp=${new Date().getTime()}`)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getActiveNodePathToNode:' + e);
            throw e;
        });
};

const getHierarchyEdit = async function (rootNodePathId: number): Promise<HierarchyEditModel[]> {

    const url = `/api/hierarchy/GetHierarchyEdit/${rootNodePathId}`;

    return await axios.get<HierarchyEditModel[]>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('GetHierarchyEdit:' + e);
            throw e;
        });
};

const createHierarchyEdit = async function (rootNodePathId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/CreateHierarchyEdit/${rootNodePathId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('createHierarchyEdit:' + e);
            throw e;
        });
};

const discardHierarchyEdit = async function (hierarchyEditId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/DiscardHierarchyEdit/${hierarchyEditId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('discardHierarchyEdit:' + e);
            throw e;
        });
};

const submitHierarchyEditForPublish = async function (hierarchyEditId: number, notes: string): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/SubmitHierarchyEditForPublish`;
    const params = {
        hierarchyEditId: hierarchyEditId,
        notes: notes
    }
    return await axios.put<LearningHubValidationResultModel>(url, params)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('publishHierarchyEdit:' + e);
            throw e;
        });
};

const getFolder = async function (nodeVersionId: number): Promise<FolderNodeModel> {

    const url = `/api/hierarchy/GetFolder/${nodeVersionId}`;

    return await axios.get<FolderNodeModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getFolder:' + e);
            throw e;
        });
};

const createFolder = async function (requestModel: FolderNodeModel): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/CreateFolder`;

    return await axios.post<LearningHubValidationResultModel>(url, requestModel)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('createFolder:' + e);
            throw e;
        });
};

const updateFolder = async function (requestModel: FolderNodeModel): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/UpdateFolder`;

    return await axios.post<LearningHubValidationResultModel>(url, requestModel)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('updateFolder:' + e);
            throw e;
        });
};

const updateNodePathDisplayVersion = async function (requestModel: NodePathDisplayVersionModel): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/UpdateNodePathDisplayVersion`;

    return await axios.post<LearningHubValidationResultModel>(url, requestModel)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('updateNodePathDisplayVersion:' + e);
            throw e;
        });
};

const updateResourceReferenceDisplayVersion = async function (requestModel: ResourceReferenceDisplayVersionModel): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/UpdateResourceReferenceDisplayVersion`;

    return await axios.post<LearningHubValidationResultModel>(url, requestModel)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('UpdateResourceReferenceDisplayVersion:' + e);
            throw e;
        });
};

const deleteFolder = async function (hierarchyEditDetailId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/DeleteFolder/${hierarchyEditDetailId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('deleteFolder:' + e);
            throw e;
        });
};

const deleteFolderReference = async function (hierarchyEditDetailId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/DeleteFolderReference/${hierarchyEditDetailId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('deleteFolderReference:' + e);
            throw e;
        });
};

const moveNodeUp = async function (hierarchyEditDetailId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/MoveNodeUp/${hierarchyEditDetailId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('moveNodeUp:' + e);
            throw e;
        });
};

const moveNodeDown = async function (hierarchyEditDetailId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/MoveNodeDown/${hierarchyEditDetailId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('moveNodeDown:' + e);
            throw e;
        });
};

const moveNode = async function (hierarchyEditDetailId: number, moveToHierarchyEditDetailId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/MoveNode`;

    return await axios.post<LearningHubValidationResultModel>(url, { hierarchyEditDetailId: hierarchyEditDetailId, moveToHierarchyEditDetailId: moveToHierarchyEditDetailId })
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('moveNode:' + e);
            throw e;
        });
};

const referenceNode = async function (hierarchyEditDetailId: number, moveToHierarchyEditDetailId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/ReferenceNode`;

    return await axios.post<LearningHubValidationResultModel>(url, { hierarchyEditDetailId: hierarchyEditDetailId, moveToHierarchyEditDetailId: moveToHierarchyEditDetailId })
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('referenceNode:' + e);
            throw e;
        });
};

const hierarchyEditMoveResourceUp = async function (hierarchyEditDetailId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/HierarchyEditMoveResourceUp/${hierarchyEditDetailId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('hierarchyEditMoveResourceUp:' + e);
            throw e;
        });
};

const hierarchyEditMoveResourceDown = async function (hierarchyEditDetailId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/HierarchyEditMoveResourceDown/${hierarchyEditDetailId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('hierarchyEditMoveResourceDown:' + e);
            throw e;
        });
};

const hierarchyEditMoveResource = async function (hierarchyEditDetailId: number, moveToHierarchyEditDetailId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/HierarchyEditMoveResource`;

    return await axios.post<LearningHubValidationResultModel>(url, { hierarchyEditDetailId: hierarchyEditDetailId, moveToHierarchyEditDetailId: moveToHierarchyEditDetailId })
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('hierarchyEditMoveResource:' + e);
            throw e;
        });
};


const hierarchyEditReferenceResource = async function (hierarchyEditDetailId: number, moveToHierarchyEditDetailId: number): Promise<LearningHubValidationResultModel> {
    const url = `/api/hierarchy/HierarchyEditReferenceResource`;

    return await axios.post<LearningHubValidationResultModel>(url, { hierarchyEditDetailId: hierarchyEditDetailId, moveToHierarchyEditDetailId: moveToHierarchyEditDetailId })
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('hierarchyEditReferenceResource:' + e);
            throw e;
        });
};

const moveResourceUp = async function (nodeId: number, resourceId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/MoveResourceUp/${nodeId}/${resourceId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('moveResourceUp:' + e);
            throw e;
        });
};

const moveResourceDown = async function (nodeId: number, resourceId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/MoveResourceDown/${nodeId}/${resourceId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('moveResourceDown:' + e);
            throw e;
        });
};

const moveResource = async function (sourceNodeId: number, destinationNodeId: number, resourceId: number): Promise<LearningHubValidationResultModel> {

    const url = `/api/hierarchy/MoveResource/${sourceNodeId}/${destinationNodeId}/${resourceId}`;

    return await axios.put<LearningHubValidationResultModel>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('moveResource:' + e);
            throw e;
        });
};

const getReferencableCatalogues = async function (nodePathId: number): Promise<CatalogueBasicModel[]> {
    return await axios.get<CatalogueBasicModel[]>('/api/hierarchy/GetReferencableCatalogues/' + nodePathId + `?timestamp=${new Date().getTime()}`)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('GetReferencableCatalogues:' + e);
            throw e;
        });
};

const getCurrentUserId = async function (): Promise<number> {

    const url = `/api/hierarchy/GetCurrentUserId`;

    return await axios.get<number>(url)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getCurrentUserId:' + e);
            throw e;
        });
};

export const contentStructureData = {
    getCatalogue,
    getNodeContentsAdmin,
    getActiveNodePathToNode,
    getHierarchyEdit,
    createHierarchyEdit,
    discardHierarchyEdit,
    submitHierarchyEditForPublish,
    createFolder,
    updateFolder,
    updateNodePathDisplayVersion,
    updateResourceReferenceDisplayVersion,
    deleteFolder,
    deleteFolderReference,
    getFolder,
    moveNodeUp,
    moveNodeDown,
    moveNode,
    referenceNode,
    hierarchyEditMoveResourceUp,
    hierarchyEditMoveResourceDown,
    hierarchyEditMoveResource,
    hierarchyEditReferenceResource,
    moveResourceUp,
    moveResourceDown,
    moveResource,
    getReferencableCatalogues,
    getCurrentUserId
}
