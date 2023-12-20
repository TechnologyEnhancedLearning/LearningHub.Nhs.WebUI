import AxiosWrapper from '../axiosWrapper';
import { SearchResultModel } from '../models/searchResultModel';

const getCatalogueSearchResults = async function (
    searchString: string,
    pageIndex: number,
    eventTypeEnum: string,
    groupId: string,
    pageSize: number = null,
    searchId: number = null
) {
    const url = `/api/search/GetCatalogueSearchResults/`;
    const params = new URLSearchParams();
    params.append('searchString', searchString); 
    params.append('pageIndex', pageIndex.toString());
    params.append('groupId', groupId);

    if (pageSize) {
        params.append('pageSize', pageSize.toString());
    }
    if (searchId) {
        params.append('searchId', searchId.toString());
    }
    params.append('eventTypeEnum', eventTypeEnum);    

    return await AxiosWrapper.axios.get(url, {
        params: params
    })
    .then(response => {
        return response.data;
    })
    .catch(e => {
        console.log(e);
        window.location.reload();
    });
}

const getSearchResults = async function (searchString: string, filterQuery: string, sortItemIndex: string, hits: string, searchId: string, eventTypeEnum: string, groupId: string, offset = 0, catalogueId: number = null): Promise<SearchResultModel> {

    const url = `/api/search/GetSearchResults/`;

    const params = new URLSearchParams();
    params.append('searchString', searchString); 
    params.append('filterString', filterQuery); 
    params.append('sortItemIndex', sortItemIndex);
    params.append('hits', hits);
    params.append('searchId', searchId);
    params.append('eventTypeEnum', eventTypeEnum);
    params.append('groupId', groupId);
    params.append('offset', offset.toString());
    
    if (catalogueId != null) params.append('catalogueId', catalogueId.toString());
    
    
    return await AxiosWrapper.axios.get(url, {
        params: params})
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log(e);
        });
};

const recordClickedSearchResult = async function (
    nodePathId: number, itemIndex: number, numberOfHits: number, totalNumberOfHits: number, resourceReferenceId: number,
    groupId: string, searchId: string, timeOfSearch: number, searchText: string, userQuery: string, query: string
) {

    const params = {
        NodePathId: nodePathId,
        ItemIndex: itemIndex,
        NumberOfHits: numberOfHits,
        TotalNumberOfHits: totalNumberOfHits,
        ResourceReferenceId: resourceReferenceId,
        GroupId: groupId,
        SearchId: searchId,
        TimeOfSearch: timeOfSearch,
        SearchText: searchText,
        Query: query,
        UserQuery: userQuery,
    }
    
    return await AxiosWrapper.axios.post<boolean>('/api/Search/RecordClickedSearchResult', params)
    .then(() => {
        return true
    })
    .catch(e => {
        console.log('recordClickedSearchResult:' + e);
        throw e;
    });    
}

const recordClickedCatalogueSearchResult = async function (
    nodePathId: number, itemIndex: number, numberOfHits: number, totalNumberOfHits: number, catalogueId: number,
    groupId: string, searchId: string, timeOfSearch: number, searchText: string, userQuery: string, query: string
) {

    const params = {
        NodePathId: nodePathId,
        ItemIndex: itemIndex,
        NumberOfHits: numberOfHits,
        TotalNumberOfHits: totalNumberOfHits,
        CatalogueId: catalogueId,
        GroupId: groupId,
        SearchId: searchId,
        TimeOfSearch: timeOfSearch,
        SearchText: searchText,
        Query: query,
        UserQuery: userQuery,
    }

    return await AxiosWrapper.axios.post<boolean>('/api/Search/RecordClickedCatalogueSearchResult', params)
        .then(() => {
            return true
        })
        .catch(e => {
            console.log('recordClickedCatalogueSearchResult:' + e);
            throw e;
        });
}

export const searchResults = {
    getSearchResults,
    getCatalogueSearchResults,
    recordClickedSearchResult,
    recordClickedCatalogueSearchResult
};