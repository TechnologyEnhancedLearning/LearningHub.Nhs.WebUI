export class SearchResultModel {
    documentModel: documentModel[];
    resource_reference_id: number;
    searchId: number;
    hits: number;
    totalHits: number;
    searchString: string;
    groupId: string;
    sortItemSelected: SortItemViewModel;
    sortItemList: SortItemViewModel[];
    facets: facets[];
    feedback: SearchFeedbackModel;
    errorOnAPI: boolean;
    constructor(init?: Partial<SearchResultModel>) {
        Object.assign(this, init);
    }
}

class documentModel {
    title: string;
    description: string;
    resourceVersionId: number;
    _id: number;
};

class SortItemViewModel {
    name: string;
    value: string;
    sortDirection: string;
    searchSortType: SearchSortTypeEnum
}

export class facets {
    id: string;
    filters: filters[];
}

class filters {
    displayName: string;
    count: number;
}

enum SearchSortTypeEnum {
    Relevance = 0,
    AToZ = 1,
    DateAuthored = 2,
    Rating = 3
}

class SearchFeedbackModel {
    feedbackAction: SearchFeedbackActionModel;

    public constructor(init?: Partial<SearchFeedbackModel>) {
        Object.assign(this, init);
    }
}

export class SearchFeedbackActionModel {
    payload: SearchFeedbackPayloadModel;
}

export class SearchFeedbackPayloadModel {
    searchSignal: SearchFeedbackSignalModel;
    hitNumber: Number;
    timeOfClick: Number;
    clickTargetUrl: string;

    public constructor(init?: Partial<SearchFeedbackPayloadModel>) {
        Object.assign(this, init);
    }
}

export class SearchFeedbackSignalModel {
    profileSignature: SearchFeedbackProfileSignatureModel;
    stats: SearchFeedbackStatsModel;
    userQuery: string;
    query: string;
    searchId: string;
    timeOfSearch: number;

    public constructor(init?: Partial<SearchFeedbackSignalModel>) {
        Object.assign(this, init);
    }
}

export class SearchFeedbackProfileSignatureModel {
    applicationId: string;
    profileType: string;
    profileId: string;

    public constructor(init?: Partial<SearchFeedbackProfileSignatureModel>) {
        Object.assign(this, init);
    }
}

export class SearchFeedbackStatsModel {
    totalHits: number;    

    public constructor(init?: Partial<SearchFeedbackStatsModel>) {
        Object.assign(this, init);
    }
}

