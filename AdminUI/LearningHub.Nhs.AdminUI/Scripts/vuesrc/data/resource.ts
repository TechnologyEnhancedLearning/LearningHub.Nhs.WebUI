import axios from 'axios';
import AxiosWrapper from '../axiosWrapper';

// Note: Copied in from WebUI to keep the TypeScript build compiling correctly. Needed by tree-item.vue in WebUI but this functionality isn't used yet in the AdminUI. Might be needed in IT2+.

const duplicateResource = async function (resourceVersionId: number, resourceCatalogueId: number): Promise<number> {
    const params = {
        resourceVersionId,
        resourceCatalogueId,
    };

    return await axios.post('/api/Resource/DuplicateResource', params)
        .then(response => {
            return response.data.createdId;
        })
        .catch(e => {
            console.log('contributeApi.duplicateResource:' + e);
            throw e;
        });
};


export const resourceData = {
    duplicateResource
};