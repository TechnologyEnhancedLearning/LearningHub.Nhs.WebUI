<template>
    <div class="lh-container-xl">
        <h3 class="search-error mt-5" v-if="searchResults.errorOnAPI">An error has occurred with search. Please try again in a few minutes.</h3>
        <div class="result-item-block mx-xl-0" v-if="searchResults.documentModel && searchResults.documentModel.length">
            <div v-for="(item, index) in searchResults.documentModel" class="mt-5">
                <p class="result-item-title heading-sm">
                    <div v-if="!hideRestrictedBadge && item.catalogueRestrictedAccess" class="catalogue-restricted-badge"><i class="fas fa-lock-open-alt" v-if="item.catalogueHasAccess"></i><i class="fas fa-lock-alt" v-if="!item.catalogueHasAccess"></i> Restricted access</div>
                    <div class="fake-link" v-on:click="navigateToResource(item.resource_reference_id, index, item.nodePathId)"><span>{{ item.title }}</span></div>
                <p>
                    <div class="result-item-type-rating d-flex flex-column flex-lg-row">
                        <div class="mb-3 mr-sm-3">
                            <span class="result-item-type"><i :class="getResourceTypeIconClass(item.resource_type)"></i>&nbsp;{{ GetPrettifiedResourceTypeName(item.resource_type) }}</span>
                        </div>
                        <div class="pl-sm-1 d-flex">
                            <RatingComponent ref="ratingComponent" :entityVersionId="item.resourceVersionId" :userAuthenticated="false" :singleLineLayout="true"></RatingComponent>
                        </div>
                    </div>
                    <div class="result-item-short-description"> {{ concat(item.description, searchResults.descriptionMaxLength) }}</div>
                    <div class="d-flex flex-column flex-lg-row">
                        <div class="result-item-catalogue mr-3" v-if="item.catalogueName && catalogueId == null">
                            <img :src="'/Search/Image/' + item.catalogueBadgeUrl" v-if="item.catalogueBadgeUrl" class="catalogue-resource-search-result-badge" />
                            <a :href="'/Catalogue/' + item.catalogueUrl" class="catalogue-resource-search-result-name">{{item.catalogueName}}</a>
                        </div>
                        <div class="result-item-attribution">
                            <div class="result-item-attribution-author pt-1">
                                <span>{{ GetAttribution(item.authors) }}</span>
                                <span v-if="item.authored_date">{{ GetInOn(item.authored_date) }}</span>
                                <span class="result-item-date" v-if="item.authored_date">{{ item.authored_date }}</span>
                            </div>

                        </div>
                    </div>
            </div>

            <div class="load-more mt-5">
                <button class="btn btn-outline-custom" @click="loadNextRecordBatch()" v-if="this.searchResults.documentModel.length < this.searchResults.totalHits">{{ getLoadMoreText() }}</button>
            </div>
            <h3 class="search-error" v-if="searchResults.errorOnAPI">An error has occurred with search. Please try again in a few minutes.</h3>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import 'core-js/features/url-search-params';
    import { SearchResultModel } from '../models/searchResultModel';
    import { ResourceType } from '../constants';
    import { searchResults as SearchResults } from './search';
    import { EventTypeEnum } from '../EventTypeEnum';
    import RatingComponent from '../globalcomponents/RatingComponent.vue';

    export default Vue.extend({
        name: 'searchresult',
        components: {
            RatingComponent
        },
        props: {
            searchResults: { Type: SearchResultModel } as PropOptions<SearchResultModel>,
            filterQuery: { Type: String } as PropOptions<string>,
            catalogueId: { Type: Number } as PropOptions<number>,
            hideRestrictedBadge: { Type: Boolean } as PropOptions<boolean>,
        },
        data: function () {
            return {
            };
        },
        methods: {
            navigateToResource: async function (resource_reference_id: number, currentItem: number, nodePathId: number): Promise<void>  {
                let searchSignal = this.searchResults.feedback.feedbackAction.payload.searchSignal;

                await SearchResults.recordClickedSearchResult((nodePathId || 0), currentItem, this.searchResults.hits, this.searchResults.totalHits,
                    resource_reference_id, this.searchResults.groupId, searchSignal.searchId, searchSignal.timeOfSearch, this.searchResults.searchString, searchSignal.userQuery, searchSignal.query)                
                    window.location.href = "/Resource/" + resource_reference_id;          
            },                 
            getResourceTypeIconClass(resourceType: string): any {
                var resourceTypeUpper = resourceType.toUpperCase().replace(/\s/g, '');

                switch (resourceTypeUpper) {                  
                    case ResourceType[ResourceType.ARTICLE]:
                        return ["fa-regular", "fa-file-alt"];
                    case ResourceType[ResourceType.AUDIO]:
                        return ["fa-solid", "fa-volume-up"];
                    case ResourceType[ResourceType.EQUIPMENT]:
                        return ["fa-solid", "fa-map-marker-alt"];
                    case ResourceType[ResourceType.GENERICFILE]:
                        return ["fa-regular", "fa-file"];
                    case ResourceType[ResourceType.IMAGE]:
                        return ["fa-regular", "fa-image"];
                    case ResourceType[ResourceType.SCORM]:
                        return ["fa-solid", "fa-cube"];
                    case ResourceType[ResourceType.VIDEO]:
                        return ["fa-solid", "fa-video"];
                    case ResourceType[ResourceType.WEBLINK]:
                        return ["fa-solid", "fa-globe"];
                    case ResourceType[ResourceType.CASE]:
                        return ["fa-solid", "fa-microscope"];
                    default:
                        return ["fa-regular", "fa-file"];
                }
            },
            GetPrettifiedResourceTypeName(resourceType: string): string {
                var resourceTypeUpper = resourceType.toUpperCase().replace(/\s/g, '');

                switch (resourceTypeUpper) {
                    case ResourceType[ResourceType.WEBLINK]:
                        return "Web link";
                    case ResourceType[ResourceType.GENERICFILE]:
                        return "File";
                    case ResourceType[ResourceType.SCORM]:
                        return "elearning";
                    default:
                        return resourceType;
                }
            },
            GetInOn(dateString: string): string {
                const onRegexString = "[0-9]{1,2} [A-Za-z]{3,4}";

                if (dateString.match(onRegexString)) {
                    return "on";
                }
                else {
                    return "in";
                }
            },
            async loadNextRecordBatch(): Promise<void> {

                var nextResults = await SearchResults.getSearchResults(
                    this.searchResults.searchString,
                    this.filterQuery,
                    this.searchResults.sortItemSelected.searchSortType.toString(),
                    this.searchResults.hits.toString(),
                    this.searchResults.searchId.toString(),
                    EventTypeEnum.SearchLoadMore.toString(),
                    this.searchResults.groupId,
                    0,
                    this.catalogueId);

                this.searchResults.errorOnAPI = nextResults.errorOnAPI;

                if (!this.searchResults.errorOnAPI) {

                    this.searchResults.documentModel = this.searchResults.documentModel.concat(nextResults.documentModel);
                    this.searchResults.hits += nextResults.hits;
                    this.searchResults.feedback = nextResults.feedback;
                }
                
            },
            getLoadMoreText(): string {
                if (this.searchResults.totalHits - this.searchResults.documentModel.length > 10)
                    return 'Load 10 more results';

                if (this.searchResults.totalHits - this.searchResults.documentModel.length === 1)
                    return 'Load 1 more result';
                else {
                    return 'Load ' + (this.searchResults.totalHits - this.searchResults.documentModel.length).toString() + ' more results';
                }
            },
            GetAttribution(authors: string): string {

                var attribution = "";

                if (authors && authors.length > 0 && authors[0]) {
                    attribution += "Authored by ";

                    if (Array.isArray(authors)) {
                        for (var author in authors) {
                            attribution += authors[author];
                            attribution += ", ";
                        }
                    }
                    else {
                        attribution += authors += ", ";
                    }
                }

                attribution = attribution.substring(0, attribution.length - 2);

                if (attribution.length > 40) {
                    attribution = attribution.substring(0, 40) + "...";
                };

                return attribution;
            },
            concat(textToConcat: string, length: number): string {
                if (!textToConcat) { return ''; }
                if (textToConcat.length <= length) { return textToConcat; }
                var subString = textToConcat.substring(0, length - 1);
                return subString.substring(0, subString.lastIndexOf(' ')) + '...';
            },
        },
    })
</script>
