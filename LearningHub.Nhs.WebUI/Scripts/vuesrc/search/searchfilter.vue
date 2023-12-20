<template>
    <div class="">
        <div class="results-count-text mx-xl-0">
            <div class="row mt-5" v-if="showResultCount">
                <div class="col-12">
                    <h3 v-if="searchResults.totalHits === 1 && searchResults.searchString !=''" class="d-inline resources-header">{{ searchResults.totalHits }}  resource</h3>
                    <h3 v-if="searchResults.totalHits != 1 && searchResults.searchString !=''" class="d-inline resources-header">{{ searchResults.totalHits }}  resources</h3>
                </div>
            </div>
            <div class="row">
                <div class="col-12">
                    <div v-if="searchResults.totalHits != 0" class="d-block d-md-inline-block pt-3 pt-md-0">
                        <div class="dropdown searchsort-dropdown mt-4">
                            <a class="dropdown-toggle" role="button" id="dropdownSearchSortItems" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                Sort by: {{ searchResults.sortItemSelected.name }}
                            </a>
                            <div class="dropdown-menu" aria-labelledby="dropdownSearchSortItems">
                                <div v-for="(sortListItem, index) in this.searchResults.sortItemList">
                                    <a class="dropdown-item" @click="sort(index)">{{ sortListItem.name }} </a>
                                </div>
                            </div>
                            <a class="dropdown-toggle ml-5" role="button" id="filterResults" v-on:click="toggleFilterPanel()" aria-expanded="false">
                                {{ this.filterLabel }}
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="divider" v-if="!filterPanel"></div>
        <div id="filterDrawer" class="search-filter-panel" v-if="filterPanel">
            <div class="row">
                <div class="col-10 col-sm-11">
                    <div class="row pt-3 pt-sm-0">
                        <div class="col-sm-4">
                            <label class="checkContainer search-filter mb-0" v-bind:class="{ 'disabledCheckContainer': !filterEnabled('article') }">
                                Article {{ getCount('article') }}
                                <input id="articleResourceType" type="checkbox" v-model="articleResourceType" :true-value="true" :false-value="false" v-bind:class="{ 'disabled': !filterEnabled('article') }" v-bind:disabled="!filterEnabled('article')">
                                <span class="checkmark" v-bind:class="{ 'checkDisabled': !filterEnabled('article') }"></span>
                            </label>
                        </div>
                        <div class="col-sm-4">
                            <label class="checkContainer search-filter mb-0" v-bind:class="{ 'disabledCheckContainer': !filterEnabled('audio') }">
                                Audio {{ getCount('audio') }}
                                <input id="audioResourceType" type="checkbox" v-model="audioResourceType" :true-value="true" :false-value="false" v-bind:class="{ 'disabled': !filterEnabled('audio') }" v-bind:disabled="!filterEnabled('audio')">
                                <span class="checkmark" v-bind:class="{ 'checkDisabled': !filterEnabled('audio') }"></span>
                            </label>
                        </div>
                        <div class="col-sm-4">
                            <label class="checkContainer search-filter mb-0" v-bind:class="{ 'disabledCheckContainer': !filterEnabled('image') }">
                                Image {{ getCount('image') }}
                                <input id="imageResourceType" type="checkbox" v-model="imageResourceType" :true-value="true" :false-value="false" v-bind:class="{ 'disabled': !filterEnabled('image') }" v-bind:disabled="!filterEnabled('image')">
                                <span class="checkmark" v-bind:class="{ 'checkDisabled': !filterEnabled('image') }"></span>
                            </label>
                        </div>
                        <div class="col-sm-4">
                            <label class="checkContainer search-filter mb-0" v-bind:class="{ 'disabledCheckContainer': !filterEnabled('video') }">
                                Video {{ getCount('video') }}
                                <input id="videoResourceType" type="checkbox" v-model="videoResourceType" :true-value="true" :false-value="false" v-bind:class="{ 'disabled': !filterEnabled('video') }" v-bind:disabled="!filterEnabled('video')">
                                <span class="checkmark" v-bind:class="{ 'checkDisabled': !filterEnabled('video') }"></span>
                            </label>
                        </div>
                        <div class="col-sm-4">
                            <label class="checkContainer search-filter mb-0" v-bind:class="{ 'disabledCheckContainer': !filterEnabled('weblink') }">
                                Web link {{ getCount('weblink') }}
                                <input id="webLinkResourceType" type="checkbox" v-model="webLinkResourceType" :true-value="true" :false-value="false" v-bind:class="{ 'disabled': !filterEnabled('weblink') }" v-bind:disabled="!filterEnabled('weblink')">
                                <span class="checkmark" v-bind:class="{ 'checkDisabled': !filterEnabled('weblink') }"></span>
                            </label>
                        </div>
                        <div class="col-sm-4">
                            <label class="checkContainer search-filter mb-0" v-bind:class="{ 'disabledCheckContainer': !filterEnabled('genericfile') }">
                                Generic file {{ getCount('genericfile') }}
                                <input id="genericFileResourceType" type="checkbox" v-model="genericFileResourceType" :true-value="true" :false-value="false" v-bind:class="{ 'disabled': !filterEnabled('genericfile') }" v-bind:disabled="!filterEnabled('genericfile')">
                                <span class="checkmark" v-bind:class="{ 'checkDisabled': !filterEnabled('genericfile') }"></span>
                            </label>
                        </div>
                        <div class="col-sm-4">
                            <label class="checkContainer search-filter mb-0" v-bind:class="{ 'disabledCheckContainer': !filterEnabled('scorm') }">
                                elearning {{ getCount('scorm') }}
                                <input id="scormResourceType" type="checkbox" v-model="scormResourceType" :true-value="true" :false-value="false" v-bind:class="{ 'disabled': !filterEnabled('scorm') }" v-bind:disabled="!filterEnabled('scorm')">
                                <span class="checkmark" v-bind:class="{ 'checkDisabled': !filterEnabled('scorm') }"></span>
                            </label>
                        </div>
                        <div class="col-sm-4">
                            <label class="checkContainer search-filter mb-0" v-bind:class="{ 'disabledCheckContainer': !filterEnabled('case') }">
                                Case {{ getCount('case') }}
                                <input id="caseResourceType" type="checkbox" v-model="caseResourceType" :true-value="true" :false-value="false" v-bind:class="{ 'disabled': !filterEnabled('case') }" v-bind:disabled="!filterEnabled('case')">
                                <span class="checkmark" v-bind:class="{ 'checkDisabled': !filterEnabled('case') }"></span>
                            </label>
                        </div>
                        <div class="col-sm-4">
                            <label class="checkContainer search-filter mb-0" v-bind:class="{ 'disabledCheckContainer': !filterEnabled('assessment') }">
                                Assessment {{ getCount('assessment') }}
                                <input id="assessmentResourceType" type="checkbox" v-model="assessmentResourceType" :true-value="true" :false-value="false" v-bind:class="{ 'disabled': !filterEnabled('assessment') }" v-bind:disabled="!filterEnabled('assessment')">
                                <span class="checkmark" v-bind:class="{ 'checkDisabled': !filterEnabled('assessment') }"></span>
                            </label>
                        </div>
                    </div>
                </div>

                <div class="search-panel-close col-2 col-sm-1">
                    <a style="text-decoration:none; color:black" aria-label="Close" @click="toggleFilterPanel()" class="fa-pull-right pr-2">
                        <span aria-hidden="true"><i class="fa-solid fa-xmark" style="cursor:pointer;"></i></span>
                    </a>
                </div>
            </div>

            <div class="search-filter-button mt-4 pb-4 mt-sm-12">
                <button type="button" class="btn btn-green mr-4" v-bind:class="{ 'disabled': !canApply }" @click="applyFilter(false, false)" v-bind:disabled="!canApply">
                    {{applyCaption}}
                    <i class="fa fa-spinner fa-spin" v-if="!resultsLoaded"></i>
                </button>
                <button type="button" class="btn btn-outline-custom clear-all-filters" @click="clearAllFilters()">Clear all filters</button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import 'core-js/features/url-search-params';

    import { SearchResultModel, facets } from '../models/searchResultModel';
    import { searchResults as SearchResults } from './search';
    import { EventTypeEnum } from '../EventTypeEnum';

    export default Vue.extend({
        name: 'searchfilter',
        props: {
            searchResults: { Type: SearchResultModel } as PropOptions<SearchResultModel>,
            showResultCount: { Type: Boolean } as PropOptions<boolean>,
            catalogueId: { Type: Number } as PropOptions<number>,
        },
        data: function () {
            return {
                filterPanel: false,
                selectedFilters: [],
                articleResourceType: false,
                audioResourceType: false,
                imageResourceType: false,
                videoResourceType: false,
                webLinkResourceType: false,
                genericFileResourceType: false,
                scormResourceType: false,
                caseResourceType: false,
                assessmentResourceType: false,
                filterLabel: 'Filter results',
                resultsLoaded: true,
            };
        },
        computed: {
            canApply(): boolean {
                return this.filterSelected();
            },
            applyCaption: function applyCaption(): string {
                if (this.resultsLoaded) {
                    return 'Apply';
                } else {
                    return 'Processing...';
                }
            },
        },
        methods: {
            sort: function (sortItemIndex: number): void {
                this.searchResults.sortItemSelected.searchSortType = sortItemIndex;

                if (this.selectedFilters.length == 0) {
                    this.loadFiltered('', true, false);
                }
                else {
                    this.applyFilter(true, false);
                }
            },
            toggleFilterPanel: function (): void {
                event.preventDefault();
                this.filterPanel = !this.filterPanel;
            },
            filterEnabled(resource_type: string): boolean {
                var count = this.getCount(resource_type);
                if (count == '(0)') { return false; }
                return true;
            },
            getCount(resource_type: string): string {
                let rtFacet: facets = null;
                for (var x = 0; x < this.searchResults.facets.length; x++) {
                    if (this.searchResults.facets[x].id === "resource_type") {
                        rtFacet = this.searchResults.facets[x];
                    }
                }
                if (rtFacet) {
                    for (var i = 0; i < rtFacet.filters.length; i++) {
                        if (rtFacet.filters[i].displayName == resource_type) {
                            return '(' + rtFacet.filters[i].count + ')';
                        }
                    }
                }

                return '(0)';
            },
            applyFilter(fromSort: boolean, fromClearAll: boolean): void {
                this.resultsLoaded = false;
                this.selectedFilters = [];

                var filterQuery = '&';
                if (this.articleResourceType) { filterQuery = this.buildFilterQuery(filterQuery, 'articleResourceType', 'Article'); }
                if (this.audioResourceType) { filterQuery = this.buildFilterQuery(filterQuery, 'audioResourceType', 'Audio'); }
                if (this.imageResourceType) { filterQuery = this.buildFilterQuery(filterQuery, 'imageResourceType', 'Image'); }
                if (this.videoResourceType) { filterQuery = this.buildFilterQuery(filterQuery, 'videoResourceType', 'Video'); }
                if (this.webLinkResourceType) { filterQuery = this.buildFilterQuery(filterQuery, 'webLinkResourceType', 'WebLink'); }
                if (this.genericFileResourceType) { filterQuery = this.buildFilterQuery(filterQuery, 'genericFileResourceType', 'GenericFile'); }
                if (this.scormResourceType) { filterQuery = this.buildFilterQuery(filterQuery, 'scormResourceType', 'Scorm'); }
                if (this.caseResourceType) { filterQuery = this.buildFilterQuery(filterQuery, 'caseResourceType', 'Case'); }
                if (this.assessmentResourceType) { filterQuery = this.buildFilterQuery(filterQuery, 'assessmentResourceType', 'Assessment'); }

                this.loadFiltered(filterQuery, fromSort, fromClearAll);
                this.updateFilterLabel();
            },
            updateFilterLabel(): void {
                if (this.filterLabel && this.selectedFilters.length > 0) {
                    this.filterLabel = this.selectedFilters.length + " filter" + ((this.selectedFilters.length > 1) ? "s" : "") + " applied";
                }
                else {
                    this.filterLabel = 'Filter results';
                }
            },
            clearAllFilters(): void {
                var checks = document.querySelectorAll('#filterDrawer input[type="checkbox"]');

                for (var i = 0; i < checks.length; i++) {
                    var check = <HTMLInputElement>document.getElementById(checks[i].id);
                    check.checked = false;
                }

                this.selectedFilters = [];
                this.articleResourceType = false;
                this.audioResourceType = false;
                this.imageResourceType = false;
                this.videoResourceType = false;
                this.webLinkResourceType = false;
                this.genericFileResourceType = false;
                this.scormResourceType = false;
                this.caseResourceType = false;
                this.assessmentResourceType = false;
                this.loadFiltered('', false, true);
                this.updateFilterLabel()
            },
            buildFilterQuery(currentQuery: string, resourceType: string, newFilter: string): string {
                if (currentQuery != '&') {
                    currentQuery += '&';
                }
                this.selectedFilters.push(resourceType);
                return currentQuery + 'resource_type=' + newFilter;
            },
            loadFiltered: async function (filterQuery: string, fromSort: boolean, fromClearAll: boolean): Promise<void> {
                var event = ((fromSort) ? EventTypeEnum.SearchSort.toString() : EventTypeEnum.SearchFilter.toString());

                this.searchResults.documentModel = [];

                this.$emit('filterQuery', filterQuery);

                var filteredResults = await SearchResults.getSearchResults(
                    this.searchResults.searchString,
                    filterQuery,
                    this.searchResults.sortItemSelected.searchSortType.toString(),
                    '0',
                    this.searchResults.searchId.toString(),
                    event,
                    this.searchResults.groupId,
                    0,
                    this.catalogueId);

                this.searchResults.documentModel = filteredResults.documentModel;
                this.searchResults.hits = filteredResults.hits;
                this.searchResults.totalHits = filteredResults.totalHits;
                this.searchResults.sortItemSelected = filteredResults.sortItemSelected;
                this.searchResults.facets = filteredResults.facets;
                this.searchResults.sortItemList = filteredResults.sortItemList;

                this.resultsLoaded = true;

                if (!fromSort && !fromClearAll) {
                    this.toggleFilterPanel();
                }
            },
            filterSelected(): boolean {
                return this.articleResourceType ||
                    this.audioResourceType ||
                    this.imageResourceType ||
                    this.videoResourceType ||
                    this.webLinkResourceType ||
                    this.genericFileResourceType ||
                    this.scormResourceType ||
                    this.caseResourceType ||
                    this.assessmentResourceType;
            },
        },
    })
</script>
