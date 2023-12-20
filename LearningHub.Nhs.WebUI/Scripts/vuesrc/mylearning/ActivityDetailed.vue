<template>
    <div>
        <div class="lh-padding-fluid bg-white">
            <div class="lh-container-xl">
                <myLearningHeaderComponent></myLearningHeaderComponent>
            </div>
        </div>

        <div v-if="isLoading" class="d-flex flex-column align-items-center">
            <div class="loading-title">Loading your activity</div>
            <div class="loading-spinner">
                <i class="fa fa-spinner fa-spin fa-3x"></i>
            </div>
        </div>

        <div v-if="!isLoading">
            <div class="lh-padding-fluid">
                <div class="lh-container-xl">
                    <div class="input-group py-4" id="input-group-searchbar-md">
                        <input class="form-control pl-4" v-model="searchTextInBox" type="search" placeholder="Search your learning activity" aria-label="Search your learning activity" id="input-search-md" v-on:keyup="searchBoxKeyUp($event.keyCode)">
                        <span class="input-group-append">
                            <button class="btn btn-outline-secondary btn-search" type="button" name="button-search" aria-label="search" v-on:click="applyFilters()" style="margin-top: 0;">
                                <i class="far fa-search" aria-hidden="true"></i>
                            </button>
                        </span>
                    </div>

                    <div class="quicklink-bar d-flex flex-row" v-if="!isShowingFilters">
                        <div class="quicklinks flex-grow-1 d-flex align-items-center">
                            <span class="quicklinks-desktop">
                                <a href="#" v-on:click="filterByTimePeriod('thisWeek')" onclick="return false;">This week</a>
                                <a href="#" v-on:click="filterByTimePeriod('thisMonth')" onclick="return false;">This month</a>
                                <a href="#" v-on:click="filterByTimePeriod('last12Months')" onclick="return false;">Last 12 months</a>
                                <a href="#" v-on:click="showFilters" onclick="return false;">More filters<span class="quicklink-arrow"></span></a>
                            </span>
                            <span class="quicklinks-mobile d-none">
                                <a href="#" v-on:click="showFilters" onclick="return false;">Filters<span class="quicklink-arrow"></span></a>
                            </span>
                        </div>
                        <div class="generate-report">
                            <button class="btn btn-green" :disabled="activityModel.totalCount === 0" @click="generateReport()">
                                Generate report
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div class="lh-padding-fluid bg-white" v-if="areAnyFiltersSelected() && !isShowingFilters">
                <div class="lh-container-xl">
                    <div class="chips-container d-flex flex-row">
                        <div class="chip small" v-if="searchText !== undefined"><button class="btn btn-link" aria-label="Remove text filter" @click="searchText=undefined;searchTextInBox=undefined;loadActivity()"><i class="fa-solid fa-xmark"></i></button>{{ searchText }}</div>

                        <div class="chip small" v-if="startDate !== undefined || endDate !== undefined"><button class="btn btn-link" aria-label="Remove date filters" @click="removeDateFilters"><i class="fa-solid fa-xmark"></i></button>{{ getDateFilterChipText() }}</div>
                        <div class="chip small" v-if="timePeriod === 'thisWeek'"><button class="btn btn-link" aria-label="Remove this week filter" @click="timePeriod=undefined;loadActivity();"><i class="fa-solid fa-xmark"></i></button>This week</div>
                        <div class="chip small" v-if="timePeriod === 'thisMonth'"><button class="btn btn-link" aria-label="Remove this month filter" @click="timePeriod=undefined;loadActivity();"><i class="fa-solid fa-xmark"></i></button>This month</div>
                        <div class="chip small" v-if="timePeriod === 'last12Months'"><button class="btn btn-link" aria-label="Remove last 6 months filter" @click="timePeriod=undefined;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Last 12 months</div>

                        <div class="chip small" v-if="complete && !areAllActivityStatusesSelected()"><button class="btn btn-link" aria-label="Remove complete filter" @click="complete=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Complete</div>
                        <div class="chip small" v-if="incomplete && !areAllActivityStatusesSelected()"><button class="btn btn-link" aria-label="Remove incomplete filter" @click="incomplete=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Incomplete</div>
                        <div class="chip small" v-if="passed && !areAllActivityStatusesSelected()"><button class="btn btn-link" aria-label="Remove passed filter" @click="passed=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Passed</div>
                        <div class="chip small" v-if="failed && !areAllActivityStatusesSelected()"><button class="btn btn-link" aria-label="Remove failed filter" @click="failed=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Failed</div>
                        <div class="chip small" v-if="downloaded && !areAllActivityStatusesSelected()"><button class="btn btn-link" aria-label="Remove downloaded filter" @click="downloaded=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Downloaded</div>

                        <div class="chip small" v-if="weblink && !areAllResourceTypesSelected()"><button class="btn btn-link" aria-label="Remove web link filter" @click="weblink=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Web link</div>
                        <div class="chip small" v-if="file && !areAllResourceTypesSelected()"><button class="btn btn-link" aria-label="Remove file filter" @click="file=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>File</div>
                        <div class="chip small" v-if="video && !areAllResourceTypesSelected()"><button class="btn btn-link" aria-label="Remove video filter" @click="video=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Video</div>
                        <div class="chip small" v-if="article && !areAllResourceTypesSelected()"><button class="btn btn-link" aria-label="Remove article filter" @click="article=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Article</div>
                        <div class="chip small" v-if="image && !areAllResourceTypesSelected()"><button class="btn btn-link" aria-label="Remove image filter" @click="image=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Image</div>
                        <div class="chip small" v-if="audio && !areAllResourceTypesSelected()"><button class="btn btn-link" aria-label="Remove audio filter" @click="audio=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Audio</div>
                        <div class="chip small" v-if="elearning && !areAllResourceTypesSelected()"><button class="btn btn-link" aria-label="Remove elearning filter" @click="elearning=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>ELearning</div>
                        <div class="chip small" v-if="assessment && !areAllResourceTypesSelected()"><button class="btn btn-link" aria-label="Remove assessment filter" @click="assessment=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Assessment</div>
                        <div class="chip small" v-if="ecase && !areAllResourceTypesSelected()"><button class="btn btn-link" aria-label="Remove case filter" @click="ecase=false;loadActivity();"><i class="fa-solid fa-xmark"></i></button>Case</div>
                        <div class="chip small" v-if="areAllResourceTypesSelected()"><button class="btn btn-link" aria-label="Remove all resource type filters" @click="removeAllResourceTypeFilters"><i class="fa-solid fa-xmark"></i></button>All resource types</div>

                        <div class="clear-filters"><a href="#" v-on:click="clearFiltersAndReload" onclick="return false;">Clear all filters</a></div>
                    </div>
                </div>
            </div>

            <div class="lh-padding-fluid error-text" v-if="errorMessage">
                <span class="text-danger">{{ errorMessage }}</span>
            </div>

            <div class="lh-padding-fluid" v-if="!isShowingFilters" id="printTest">
                <div class="lh-container-xl">
                    <div class="activity-results-count">
                        <span>{{ activityModel.totalCount }} activity result{{ activityModel.totalCount == 1 ? '' : 's' }}</span>
                    </div>

                    <table class="activity-results-table" v-if="activityModel.totalCount > 0">
                        <thead>
                            <tr class="small">
                                <td>Title</td>
                                <td>Version</td>
                                <td>Type</td>
                                <td>Activity</td>
                                <td>Attempts</td>
                                <td style="text-align: right;">Status</td>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class="activity-result-item small" v-for="activity in activityModel.activities">
                                <td>
                                    <a v-if="activity.isCurrentResourceVersion && activity.resourceReferenceId > 0" :href="getResourceLink(activity)">{{ activity.title }}</a>
                                    <span v-else>{{ activity.title }}</span>
                                </td>
                                <td class="no-wrap"><span class="hide-if-desktop">Version </span>{{ activity.version }}<span class="hide-if-desktop"> - {{ getResourceTypeText(activity) }}</span></td>
                                <td class="no-wrap hide-if-mobile">{{ getResourceTypeText(activity) }}</td>
                                <td>
                                    {{ getResourceTypeVerb(activity) }} on {{ activity.activityDate | formatDate('DD MMMM YYYY [at] HH:mm') }} <br />

                                    <span v-if="canShowAssessmentAttemptLink(activity)">
                                        <a :href="getAssessmentAttemptLink(activity)">{{ canShowScore(activity) ? 'View summary' : 'Resume' }}</a> -
                                    </span>

                                    <span v-if="canViewPercentage(activity)" class="no-wrap">
                                        {{ activity.completionPercentage }}% complete
                                        <span v-if="activity.isMostRecent && canViewProgress(activity)"> - <a href="#" v-on:click="viewProgress(activity.resourceId, activity.resourceReferenceId, activity.majorVersion, Math.round(activity.resourceDurationMilliseconds/1000), activity.isCurrentResourceVersion)" onclick="return false;">view progress</a></span>
                                    </span>

                                    <span v-else-if="canShowScore(activity)">
                                        Scored {{ activity.scorePercentage }}%
                                    </span>

                                    <div class="reason" v-if="activity.assessmentDetails && activity.assessmentDetails.extraAttemptReason">
                                        Reason: {{activity.assessmentDetails.extraAttemptReason}}
                                    </div>
                                </td>
                                <td>
                                    <span v-if="activity.assessmentDetails && activity.assessmentDetails.maximumAttempts" class="d-flex align-items-center">
                                        <PercentageCircle :percentage="(activity.assessmentDetails.currentAttempt / activity.assessmentDetails.maximumAttempts) * 100"
                                                          :circle-radius="12"
                                                          :circle-width="2"
                                                          :color="activity.assessmentDetails.currentAttempt >= activity.assessmentDetails.maximumAttempts ? 'red' : 'green'"
                                                          :hide-line="true"
                                                          text-color="#D5281B"
                                                          :text="activity.assessmentDetails.currentAttempt > activity.assessmentDetails.maximumAttempts ? '!' : ''"
                                                          :text-size="15"
                                                          class="m-10" />
                                        {{ activity.assessmentDetails.currentAttempt}}/{{activity.assessmentDetails.maximumAttempts }}
                                    </span>
                                </td>

                                <td class="no-wrap">
                                    <div class="progress-indicator d-flex flex-row">
                                        <div>{{ getActivityStatusText(activity) }}</div>
                                        <div>
                                            <i v-if="(getActivityStatus(activity) === ActivityStatus.Completed || getActivityStatus(activity) === ActivityStatus.Passed)" class="fa-solid fa-circle-check complete-icon" />
                                            <span v-if="(getActivityStatus(activity) === ActivityStatus.Downloaded)" class="fa-stack fa-1x">
                                                <i class="fas fa-circle fa-stack-1x downloaded-circle"></i>
                                                <i class="fas fa-arrow-down fa-stack-1x downloaded-arrow"></i>
                                            </span>
                                            <span v-if="(getActivityStatus(activity) === ActivityStatus.InProgress)" class="fa-stack fa-1x">
                                                <i class="fas fa-circle fa-stack-1x incomplete-circle" />
                                                <i class="fas fa-ellipsis-h fa-stack-1x incomplete-ellipsis" />
                                            </span>
                                            <span v-if="(getActivityStatus(activity) === ActivityStatus.Failed)" class="fa-stack fa-1x">
                                                <i class="fas fa-circle fa-stack-1x failed-circle"></i>
                                                <i class="fas fa-times fa-stack-1x failed-times"></i>
                                            </span>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <div class="load-more">
                        <button class="btn btn-outline-custom" @click="loadNextRecordBatch()" v-if="this.activityModel.activities && this.activityModel.activities.length < this.activityModel.totalCount" :disabled="disableLoadMore">
                            {{ loadMoreButtonText }}
                            <i class="fa fa-spinner fa-spin ml-3" v-if="disableLoadMore"></i>
                        </button>
                    </div>

                    <played-segment-component ref="playedSegmentComponent"></played-segment-component>
                </div>
            </div>

            <div class="lh-padding-fluid" v-if="isShowingFilters">
                <div class="lh-container-xl">
                    <div class="filters">
                        <div>
                            <a href="#" v-on:click="cancelFilterChanges()" onclick="return false;" class="small" style="text-decoration:none;"><i class="fa-solid fa-chevron-left" /> Go back</a>
                        </div>

                        <div class="d-flex flex-row flex-wrap">
                            <label class="checkContainer mb-0">
                                All dates
                                <input type="radio" name="timePeriod" v-model="timePeriod" :value="undefined" />
                                <span class="radioButton"></span>
                            </label>
                            <div class="datepickers d-flex flex-column">
                                <div class="error-text mt-3" v-if="$v.startDate.$invalid && $v.startDate.$dirty">
                                    <span class="text-danger">{{ returnError('startDate') }}</span>
                                </div>
                                <div class="error-text mt-3" v-if="$v.endDate.$invalid && $v.endDate.$dirty">
                                    <span class="text-danger">{{ returnError('endDate') }}</span>
                                </div>
                                <div class="d-flex flex-row">
                                    <label class="checkContainer date-range-label mb-0">
                                        <input type="radio" name="timePeriod" v-model="timePeriod" value="dateRange" />
                                        <span class="radioButton"></span>
                                    </label>
                                    <div class="datepicker-container d-flex flex-row mr-4">
                                        <label for="startDate" class="control-label">Start date</label>
                                        <lh-date-picker id="startDate" display-style="1" v-model="startDate" unique-name="startDate"
                                                        @dateValid="setStartDateValid($event)" useLocalTimezone v-bind:class="{ 'input-validation-error': $v.startDate.$error}"></lh-date-picker>
                                    </div>
                                    <div class="datepicker-container d-flex flex-row">
                                        <label for="endDate" class="control-label">End date</label>
                                        <lh-date-picker class="" id="endDate" display-style="1" v-model="endDate" unique-name="endDate"
                                                        @dateValid="setEndDateValid($event)" useLocalTimezone v-bind:class="{ 'input-validation-error': $v.endDate.$error}"></lh-date-picker>
                                    </div>
                                </div>
                            </div>
                            <div class="flex-break hide-if-mobile"></div>
                            <label class="checkContainer mb-0">
                                This week
                                <input type="radio" name="timePeriod" v-model="timePeriod" value="thisWeek" />
                                <span class="radioButton"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                This month
                                <input type="radio" name="timePeriod" v-model="timePeriod" value="thisMonth" />
                                <span class="radioButton"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Last 12 months
                                <input type="radio" name="timePeriod" v-model="timePeriod" value="last12Months" />
                                <span class="radioButton"></span>
                            </label>
                        </div>
                        <hr />

                        <div class="d-flex flex-row flex-wrap">
                            <label class="checkContainer mb-0">
                                Complete
                                <input type="checkbox" v-model="complete" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Incomplete
                                <input type="checkbox" v-model="incomplete" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Passed
                                <input type="checkbox" v-model="passed" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Failed
                                <input type="checkbox" v-model="failed" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Downloaded
                                <input type="checkbox" v-model="downloaded" />
                                <span class="checkmark"></span>
                            </label>
                        </div>
                        <hr />
                        <div class="d-flex flex-row flex-wrap">
                            <label class="checkContainer mb-0">
                                Web link
                                <input type="checkbox" v-model="weblink" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                File
                                <input type="checkbox" v-model="file" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Video
                                <input type="checkbox" v-model="video" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Article
                                <input type="checkbox" v-model="article" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Image
                                <input type="checkbox" v-model="image" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Audio
                                <input type="checkbox" v-model="audio" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Elearning
                                <input type="checkbox" v-model="elearning" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Assessment
                                <input type="checkbox" v-model="assessment" />
                                <span class="checkmark"></span>
                            </label>
                            <label class="checkContainer mb-0">
                                Case
                                <input type="checkbox" v-model="ecase" />
                                <span class="checkmark"></span>
                            </label>
                        </div>
                        <hr />

                        <div class="my-5">
                            <button class="btn btn-green mr-5" @click="applyFilters">
                                Apply
                            </button>
                            <button class="btn btn-outline-custom" @click="clearFilters">
                                Clear all filters
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import '../filters';
    import myLearningHeaderComponent from './HeaderComponent.vue';
    import playedSegmentComponent from './PlayedSegmentComponent.vue';
    import LhDatePicker from '../datepicker.vue';
    import moment from 'moment';
    import { requiredIf } from "vuelidate/lib/validators";
    import { ActivityDetailedModel } from '../models/mylearning/activityDetailedModel';
    import { ActivityDetailedItemModel } from '../models/mylearning/activityDetailedItemModel';
    import { MyLearningRequestModel } from '../models/mylearning/myLearningRequestModel';
    import { myLearningData } from '../data/myLearning';
    import { commonlib } from '../common';
    import { ActivityStatus, ResourceType } from '../constants';
    import PercentageCircle from "../globalcomponents/PercentageCircle.vue";

    const startDateMustBeBeforeEndDate = (value: Date, vm: any) => {
        if (value !== undefined && vm.endDate !== undefined) {
            return value <= vm.endDate;
        }
        return true;
    };
    const isValidStartDate = (value: any, vm: any) => {
        console.log(vm.startDateValid);
        if (value !== undefined && vm.startDateValid !== undefined) {
            return vm.startDateValid;
        }
        return true;
    };
    const isValidEndDate = (value: any, vm: any) => {
        console.log(vm.endDateValid);
        if (value !== undefined && vm.endDateValid !== undefined) {
            return vm.endDateValid;
        }
        return true;
    };
    export default Vue.extend({
        components: {
            'myLearningHeaderComponent': myLearningHeaderComponent,
            'playedSegmentComponent': playedSegmentComponent,
            LhDatePicker,
            PercentageCircle,
        },
        data() {
            return {
                isLoading: true,
                disableLoadMore: false,
                loadMoreButtonText: '',
                isShowingFilters: false,
                currentFilters: '',
                activityModel: new ActivityDetailedModel(),
                errorMessage: '',
                filters: new MyLearningRequestModel(),
                searchTextInBox: '',
                skip: 0,
                take: 10,
                ActivityStatus: ActivityStatus,
                startDateValid: false,
                endDateValid: false
            };
        },
        async created() {
            this.searchTextInBox = this.searchText as string;

            this.loadActivity();
        },
        methods: {
            async loadActivity() {
                this.isLoading = true;
                this.skip = 0;
                await myLearningData.getActivitiesDetailed(this.getFilterModel())
                    .then(response => {
                        this.activityModel = response;
                        this.errorMessage = '';
                        this.isLoading = false;
                        this.updateLoadMoreButtonText();
                        this.determineMostRecentActivities();
                    })
                    .catch(error => {
                        this.errorMessage = error;
                        this.isLoading = false;
                    })
            },
            async loadNextRecordBatch() {
                this.disableLoadMore = true;
                this.skip += this.take;
                let nextBatch = await myLearningData.getActivitiesDetailed(this.getFilterModel());
                this.activityModel.activities = this.activityModel.activities.concat(nextBatch.activities);
                this.updateLoadMoreButtonText();
                this.determineMostRecentActivities();
                this.disableLoadMore = false;
            },
            determineMostRecentActivities() {
                // Work out which activities are the most recent for the each resource. Only these ones will have the "view progress" link available.
                let mostRecentResources: number[] = [];
                this.activityModel.activities.forEach(activity => {
                    if (mostRecentResources.indexOf(activity.resourceId) == -1) {
                        activity.isMostRecent = true;
                        mostRecentResources.push(activity.resourceId);
                    }
                });
            },
            showFilters() {
                this.currentFilters = this.$route.fullPath;
                this.isShowingFilters = true;
            },
            cancelFilterChanges() {
                if (this.$route.fullPath !== this.currentFilters) {
                    this.$router.replace(this.currentFilters);
                }
                this.isShowingFilters = false;
            },
            canViewPercentage(activity: ActivityDetailedItemModel): boolean {
                return ((activity.resourceType == ResourceType.VIDEO || activity.resourceType == ResourceType.AUDIO) && activity.activityStatus == ActivityStatus.InProgress)
                    || (activity.resourceType == ResourceType.ASSESSMENT && activity.complete == false);
            },
            canViewProgress(activity: ActivityDetailedItemModel): boolean {
                return (activity.resourceType == ResourceType.VIDEO || activity.resourceType == ResourceType.AUDIO) && activity.activityStatus == ActivityStatus.InProgress;
            },
            canShowScore(activity: ActivityDetailedItemModel) {
                return (activity.resourceType === ResourceType.SCORM && (activity.activityStatus == ActivityStatus.Passed || activity.activityStatus == ActivityStatus.Failed))
                    || (activity.resourceType === ResourceType.ASSESSMENT || activity.resourceType === ResourceType.CASE && activity.complete);
            },
            canShowAssessmentAttemptLink(activity: ActivityDetailedItemModel): boolean {
                return activity.isCurrentResourceVersion && activity.resourceType === ResourceType.ASSESSMENT;
            },
            getAssessmentAttemptLink(activity: ActivityDetailedItemModel): string {
                return `/Resource/${activity.resourceReferenceId}/Item?attempt=${activity.assessmentResourceActivityId}`;
            },
            applyFilters() {
                if (this.$v.$invalid) {
                    this.$v.startDate.$touch();
                    this.$v.endDate.$touch();
                } else {
                    if (this.searchText !== this.searchTextInBox) {
                        this.searchText = this.searchTextInBox;
                    }
                    this.$v.$reset();
                    this.isShowingFilters = false;
                    this.loadActivity();
                }
            },
            searchBoxKeyUp(keycode: number) {
                // Enter key applies the filters.
                if (keycode === 13) {
                    this.applyFilters();
                }
            },
            areAllResourceTypesSelected() {
                return this.weblink && this.file && this.video && this.article && this.image && this.audio && this.elearning && this.assessment && this.ecase;
            },
            areAllActivityStatusesSelected() {
                return this.complete && this.incomplete && this.passed && this.failed && this.downloaded;
            },
            areAnyFiltersSelected() {
                return this.weblink || this.file || this.video || this.article || this.image || this.audio || this.elearning || this.assessment || this.ecase || this.complete || this.incomplete || this.passed || this.failed || this.downloaded || this.timePeriod || this.startDate || this.endDate || this.searchText;
            },
            removeAllResourceTypeFilters() {
                this.weblink = false;
                this.file = false;
                this.video = false;
                this.article = false;
                this.image = false;
                this.audio = false;
                this.elearning = false;
                this.assessment = false;
                this.ecase = false;

                this.loadActivity();
            },
            removeDateFilters() {
                this.timePeriod = undefined;
                this.startDate = undefined;
                this.endDate = undefined;
                this.loadActivity();
            },
            clearFilters() {
                if (this.areAnyFiltersSelected()) {
                    this.$router.replace('/');
                }
                this.searchTextInBox = undefined;
            },
            clearFiltersAndReload() {
                this.clearFilters();
                this.loadActivity();
            },
            filterByTimePeriod(timePeriod: string) {
                if (this.startDate !== undefined) {
                    this.startDate = undefined;
                }
                if (this.endDate !== undefined) {
                    this.endDate = undefined;
                }
                this.setQueryStringValue({ timePeriod: timePeriod });
                this.loadActivity();
            },
            setQueryStringValue(keyValuePair: Object) {
                this.$router.replace({ query: Object.assign({}, this.$route.query, keyValuePair) });
            },
            getActivityStatus(activity: ActivityDetailedItemModel) {

                if (activity.activityStatus == ActivityStatus.Launched
                    && (activity.resourceType == ResourceType.ARTICLE
                        || activity.resourceType == ResourceType.WEBLINK
                        || activity.resourceType == ResourceType.IMAGE
                        || activity.resourceType == ResourceType.CASE)) {
                    return ActivityStatus.Completed
                }
                else if (activity.activityStatus == ActivityStatus.Launched
                    && (activity.resourceType == ResourceType.GENERICFILE)) {
                    return ActivityStatus.Downloaded;
                }
                else if (activity.resourceType == ResourceType.ASSESSMENT) {
                    if (activity.complete)
                        return activity.scorePercentage >= activity.assessmentDetails.passMark ? ActivityStatus.Passed : ActivityStatus.Failed;
                    else
                        return activity.scorePercentage >= activity.assessmentDetails.passMark ? ActivityStatus.Passed : ActivityStatus.InProgress;
                }
                else {
                    return activity.activityStatus
                };
            },
            getActivityStatusText(activity: ActivityDetailedItemModel) {
                return commonlib.getActivityStatusDisplayText(activity);
            },
            getResourceTypeText(activity: ActivityDetailedItemModel) {
                let typeText = commonlib.getResourceTypeText(activity.resourceType);

                if (activity.resourceType == ResourceType.VIDEO || activity.resourceType == ResourceType.AUDIO) {
                    typeText = commonlib.getDurationText(activity.resourceDurationMilliseconds) + typeText.toLowerCase();
                }

                return typeText;
            },
            getResourceTypeVerb(activity: ActivityDetailedItemModel) {
                let verbText = commonlib.getResourceTypeVerb(activity);

                if (activity.resourceType == ResourceType.VIDEO || activity.resourceType == ResourceType.AUDIO) {

                    verbText += " " + commonlib.getDurationText(activity.activityDurationSeconds * 1000);
                }

                return verbText;
            },
            getDateFilterChipText() {
                if (this.startDate !== undefined && this.endDate !== undefined) {
                    return "Between " + moment(String(this.startDate)).format('DD MMMM YYYY') + " and " + moment(String(this.endDate)).format('DD MMMM YYYY');
                }
                else if (this.startDate !== undefined && this.endDate === undefined) {
                    return "From " + moment(String(this.startDate)).format('DD MMMM YYYY');
                }
                else if (this.startDate === undefined && this.endDate !== undefined) {
                    return "Up to " + moment(String(this.endDate)).format('DD MMMM YYYY');
                }
                else {
                    return "";
                }
            },
            getResourceLink(activity: ActivityDetailedItemModel) {
                return "/Resource/" + activity.resourceReferenceId;
            },
            getFilterModel() {
                let filters = new MyLearningRequestModel();

                filters.searchText = this.searchText;
                filters.weblink = this.weblink;
                filters.file = this.file;
                filters.video = this.video;
                filters.article = this.article;
                filters.image = this.image;
                filters.audio = this.audio;
                filters.elearning = this.elearning;
                filters.assessment = this.assessment;
                filters.case = this.ecase;
                filters.complete = this.complete;
                filters.incomplete = this.incomplete;
                filters.passed = this.passed;
                filters.failed = this.failed;
                filters.downloaded = this.downloaded;
                filters.timePeriod = this.timePeriod;
                filters.startDate = new Date(this.startDate);
                filters.endDate = new Date(this.endDate);
                filters.skip = this.skip;
                filters.take = this.take;

                return filters;
            },
            toggleAllResourceTypes() {

            },
            setStartDateValid(dateValid: boolean) {                
                this.startDateValid = dateValid;
            },
            setEndDateValid(dateValid: boolean) {                
                this.endDateValid = dateValid;
            },
            returnError(ctrl: string) {
                var errorMessage = '';
                switch (ctrl) {
                    case 'startDate':
                        if (this.$v.startDate.$invalid) {
                            if (!this.$v.startDate.required) {
                                errorMessage = "Please enter a valid start and/or end date.";
                            }
                            if (!this.$v.startDate.$isValidStartDate) {
                                errorMessage = "Enter a valid start date.";
                            }                           
                            if (!this.$v.startDate.startDateMustBeBeforeEndDate) {
                                errorMessage = "The end date cannot be earlier than the start date.";
                            }
                        }
                        break;
                    case 'endDate':                                               
                        if (this.$v.endDate.$invalid) {                           
                            if (!this.$v.endDate.$isValidEndDate) {
                                errorMessage = "Enter a valid end date.";
                            }
                        }
                        break;
                }
                return errorMessage;
            },
            updateLoadMoreButtonText() {
                let remaining = this.activityModel.totalCount - (this.skip + 10);
                remaining = Math.min(10, remaining);
                this.loadMoreButtonText = `Load ${remaining} more`;
            },
            viewProgress(resourceId: number, resourceReferenceId: number, majorVersion: number, mediaLengthInSeconds: number, isCurrentResourceVersion: boolean) {
                (this.$refs.playedSegmentComponent as any).viewProgress(resourceId, resourceReferenceId, majorVersion, mediaLengthInSeconds, isCurrentResourceVersion);
            },
            generateReport() {
                this.$router.push({
                    name: 'ActivityDetailedReport',
                    query: {
                        searchText: this.searchText as string,
                        weblink: (this.weblink) ? 'true' : undefined,
                        file: (this.file) ? 'true' : undefined,
                        video: (this.video) ? 'true' : undefined,
                        article: (this.article) ? 'true' : undefined,
                        image: (this.image) ? 'true' : undefined,
                        audio: (this.audio) ? 'true' : undefined,
                        elearning: (this.elearning) ? 'true' : undefined,
                        assessment: (this.assessment) ? 'true' : undefined,
                        ecase: (this.ecase) ? 'true' : undefined,
                        complete: (this.complete) ? 'true' : undefined,
                        incomplete: (this.incomplete) ? 'true' : undefined,
                        passed: (this.passed) ? 'true' : undefined,
                        failed: (this.failed) ? 'true' : undefined,
                        downloaded: (this.downloaded) ? 'true' : undefined,
                        timePeriod: this.timePeriod as string,
                        startDate: this.startDate as string,
                        endDate: this.endDate as string,
                    }
                });

            }
        },
        computed: {
            searchText: {
                get(): string { return this.$route.query.searchText as string; },
                set(value: string) { this.searchTextInBox = value as string; this.setQueryStringValue({ searchText: value || undefined }); }
            },

            weblink: {
                get(): boolean { return Boolean(this.$route.query.weblink); },
                set(value: boolean) { this.setQueryStringValue({ weblink: value || undefined }); }
            },
            file: {
                get(): boolean { return Boolean(this.$route.query.file); },
                set(value: boolean) { this.setQueryStringValue({ file: value || undefined }); }
            },
            video: {
                get(): boolean { return Boolean(this.$route.query.video); },
                set(value: boolean) { this.setQueryStringValue({ video: value || undefined }); }
            },
            article: {
                get(): boolean { return Boolean(this.$route.query.article); },
                set(value: boolean) { this.setQueryStringValue({ article: value || undefined }); }
            },
            image: {
                get(): boolean { return Boolean(this.$route.query.image); },
                set(value: boolean) { this.setQueryStringValue({ image: value || undefined }); }
            },
            audio: {
                get(): boolean { return Boolean(this.$route.query.audio); },
                set(value: boolean) { this.setQueryStringValue({ audio: value || undefined }); }
            },
            elearning: {
                get(): boolean { return Boolean(this.$route.query.elearning); },
                set(value: boolean) { this.setQueryStringValue({ elearning: value || undefined }); }
            },
            assessment: {
                get(): boolean { return Boolean(this.$route.query.assessment); },
                set(value: boolean) { this.setQueryStringValue({ assessment: value || undefined }); }
            },
            ecase: {
                get(): boolean { return Boolean(this.$route.query.ecase); },
                set(value: boolean) { this.setQueryStringValue({ ecase: value || undefined }); }
            },
            complete: {
                get(): boolean { return Boolean(this.$route.query.complete); },
                set(value: boolean) { this.setQueryStringValue({ complete: value || undefined }); }
            },
            incomplete: {
                get(): boolean { return Boolean(this.$route.query.incomplete); },
                set(value: boolean) { this.setQueryStringValue({ incomplete: value || undefined }); }
            },
            passed: {
                get(): boolean { return Boolean(this.$route.query.passed); },
                set(value: boolean) { this.setQueryStringValue({ passed: value || undefined }); }
            },
            failed: {
                get(): boolean { return Boolean(this.$route.query.failed); },
                set(value: boolean) { this.setQueryStringValue({ failed: value || undefined }); }
            },
            downloaded: {
                get(): boolean { return Boolean(this.$route.query.downloaded); },
                set(value: boolean) { this.setQueryStringValue({ downloaded: value || undefined }); }
            },

            timePeriod: {
                get(): string { return this.$route.query.timePeriod as string; },
                set(value: string) {
                    if (value !== "dateRange") {
                        if (this.startDate) { this.startDate = ''; }
                        if (this.endDate) { this.endDate = ''; }
                    }
                    this.setQueryStringValue({ timePeriod: value || undefined });
                }
            },
            startDate: {
                get(): string { return this.$route.query.startDate as string; },
                set(value: string) {
                    if (value && this.timePeriod !== "dateRange") {
                        this.timePeriod = "dateRange";
                    }
                    if (value != this.$route.query.startDate) {
                        this.setQueryStringValue({ startDate: value || undefined });
                    }
                }
            },
            endDate: {
                get(): string { return this.$route.query.endDate as string; },
                set(value: string) {
                    if (value && this.timePeriod !== "dateRange") {
                        this.timePeriod = "dateRange";
                    }
                    if (value != this.$route.query.endDate) {
                        this.setQueryStringValue({ endDate: value || undefined });
                    }
                }
            }
        },
        validations: {
            startDate: {
                required: requiredIf((vueInstance) => {
                    return vueInstance.timePeriod === 'dateRange' && vueInstance.endDate === undefined;
                }),
                isValidStartDate,                
                startDateMustBeBeforeEndDate
            },
            endDate: {               
                isValidEndDate
            }
        }
    })

</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    /* Also uses some styles in common with ActivityDetailedReport.vue, which are located in site.scss. */

    .quicklink-bar {
        padding-bottom: 15px;
    }

    .quicklinks a {
        margin-right: 20px;
    }

    .quicklinks a:last-child {
        margin-right: 0px;
    }

    label.checkContainer {
        font-family: $font-stack;
    }

    .activity-results-count {
        padding-top: 15px;
        padding-bottom: 10px;
    }

    .activity-results-table {
        width: 100%;
        border-collapse: separate;
        border-spacing: 0 12px;
    }

    .activity-results-table tbody tr {
        padding: 12px;
        margin-bottom: 20px;
        background-color: $nhsuk-white;
    }

    .activity-results-table thead td {
        padding: 12px 12px 0 12px;
        margin-right: 20px;
        font-weight: bold;
    }

    .activity-results-table tbody td {
        padding: 12px;
        margin-right: 20px;
    }

    .activity-results-table td:last-child {
        margin-right: 0px;
    }

    .progress-indicator {
        align-items: center;
        justify-content: flex-end;
    }

    .load-more {
        margin-top: 40px;
        padding-bottom: 40px;
    }

    .quicklink-arrow {
        margin-left: 8px;
    }

    .quicklink-arrow::before {
        content: url('/images/triangle-down.svg');
        position: absolute;
    }

    .filters label.checkContainer {
        min-width: 250px;
        margin-top: 20px;
        margin-bottom: 20px !important;
    }

    .filters {
        margin-left: 0px;
    }

    .filters label {
        font-size: 19px;
    }

    .filters .checkContainer {
        padding-top: 2px;
        padding-left: 35px;
    }

    .datepicker-container {
        margin-top: 14px;
    }

    .date-range-label {
        min-width: 35px !important;
        margin-right: 0px !important;
    }

    .datepickers label {
        font-family: $font-stack !important;
        margin-right: 10px;
        padding-top: 8px;
    }

    .clear-filters {
        margin-left: auto;
        margin-top: 10px;
        margin-bottom: 10px;
        align-self: center;
    }

    .reason {
        color: $nhsuk-grey-placeholder;
    }

    @media (max-width: 768px) {
        .activity-results-table tr {
            display: table;
            width: 100%;
            padding-top: 5px !important;
            padding-bottom: 0px !important;
        }

        .activity-results-table tr td {
            display: table-row;
        }

        .activity-results-table thead {
            display: none;
        }

        .progress-indicator {
            flex-direction: row-reverse !important;
            margin-left: -5px;
        }

        .quicklinks-mobile {
            display: block !important;
        }

        .quicklinks-desktop {
            display: none;
        }

        .filters label {
            font-size: 14px;
        }

        .filters .checkContainer {
            padding-top: 5px;
        }

        .filters label.checkContainer {
            min-width: 165px;
            margin-top: 15px;
            margin-bottom: 15px !important;
        }

        .datepickers {
            order: 1;
        }

        .datepicker-container {
            flex-direction: column !important;
        }

        .loading-title {
            margin-top: 40px;
        }

        .load-more {
            text-align: center;
        }
    }
</style>

<style lang="scss">
    /* To override datepicker width, need to use an unscoped (global) style. */
    #mylearningcontainer .filters .lh-datepicker {
        max-width: 155px !important;
    }
</style>