<template>
    <div class="activity-report">
        <div class="lh-padding-fluid bg-white">
            <div class="lh-container-xl">
                <div class="d-flex flex-row justify-content-between" style="padding-top: 20px;">
                    <div class="no-print">
                        <a href="#" v-on:click="goBack()" onclick="return false;" class="small" style="text-decoration:none;"><i class="fa-solid fa-chevron-left" /> Go back</a>
                    </div>
                    <div class="no-print">
                        <button class="btn btn-green" @click="printReport">
                            Print
                        </button>
                    </div>
                </div>
                <div class="activity-report-heading pb-4">
                    <h1 class="nhsuk-heading-xl">Activity report: </h1> <span v-if="!isLoading" class="no-wrap">{{ userBasicModel.firstName }} {{ userBasicModel.lastName }} ({{ userBasicModel.userName }})</span>
                </div>
            </div>
        </div>

        <div v-if="isLoading" class="d-flex flex-column align-items-center">
            <div class="loading-title">Loading your activity</div>
            <div class="loading-spinner">
                <i class="fa fa-spinner fa-spin fa-3x"></i>
            </div>
        </div>

        <div v-if="!isLoading">

            <div class="lh-padding-fluid bg-white" v-if="!isShowingFilters" id="printableArea">
                <div class="lh-container-xl">
                    <div v-if="areAnyFiltersSelected()">
                        <div>
                            <span class="small">Filters used to create this report:</span>
                        </div>

                        <div class="chips-container d-flex flex-row">
                            <div class="chip small" v-if="searchText !== undefined">{{ searchText }}</div>

                            <div class="chip small" v-if="startDate !== undefined || endDate !== undefined">{{ getDateFilterChipText() }}</div>
                            <div class="chip small" v-if="timePeriod === 'thisWeek'">This week</div>
                            <div class="chip small" v-if="timePeriod === 'thisMonth'">This month</div>
                            <div class="chip small" v-if="timePeriod === 'last12Months'">Last 12 months</div>

                            <div class="chip small" v-if="complete && !areAllActivityStatusesSelected()">Complete</div>
                            <div class="chip small" v-if="incomplete && !areAllActivityStatusesSelected()">Incomplete</div>
                            <div class="chip small" v-if="passed && !areAllActivityStatusesSelected()">Passed</div>
                            <div class="chip small" v-if="failed && !areAllActivityStatusesSelected()">Failed</div>
                            <div class="chip small" v-if="downloaded && !areAllActivityStatusesSelected()">Downloaded</div>

                            <div class="chip small" v-if="weblink && !areAllResourceTypesSelected()">Web link</div>
                            <div class="chip small" v-if="file && !areAllResourceTypesSelected()">File</div>
                            <div class="chip small" v-if="video && !areAllResourceTypesSelected()">Video</div>
                            <div class="chip small" v-if="article && !areAllResourceTypesSelected()">Article</div>
                            <div class="chip small" v-if="image && !areAllResourceTypesSelected()">Image</div>
                            <div class="chip small" v-if="audio && !areAllResourceTypesSelected()">Audio</div>
                            <div class="chip small" v-if="elearning && !areAllResourceTypesSelected()">ELearning</div>
                            <div class="chip small" v-if="assessment && !areAllResourceTypesSelected()">Assessment</div>
                            <div class="chip small" v-if="ecase && !areAllResourceTypesSelected()">Case</div>
                            <div class="chip small" v-if="areAllResourceTypesSelected()">All resource types</div>
                        </div>
                    </div>

                    <div class="error-text" v-if="errorMessage">
                        <span class="text-danger">{{ errorMessage }}</span>
                    </div>

                    <table class="activity-report-table" v-if="activityModel.totalCount > 0">
                        <thead>
                            <tr class="small">
                                <td>ID</td>
                                <td>Title</td>
                                <td>Version</td>
                                <td>Type</td>
                                <td>Activity</td>
                                <td>Attempts</td>
                                <td>Status</td>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class="activity-report-item small" v-for="activity in activityModel.activities">
                                <td>
                                    <a v-if="activity.isCurrentResourceVersion" :href="getResourceLink(activity)">{{ activity.resourceReferenceId }}</a>
                                    <span v-else>{{ activity.resourceReferenceId }}</span>
                                </td>
                                <td>
                                    <a v-if="activity.isCurrentResourceVersion" :href="getResourceLink(activity)">{{ activity.title }}</a>
                                    <span v-else>{{ activity.title }}</span>
                                </td>
                                <td class="no-wrap"><span class="hide-if-desktop">Version </span>{{ activity.version }}</td>
                                <td class="no-wrap">{{ getResourceTypeText(activity) }}</td>
                                <td>
                                    <span class="hide-if-desktop">Activity: </span>{{ getResourceTypeVerb(activity) }} on {{ activity.activityDate | formatDate('DD MMMM YYYY [at] HH:mm') }} <br />

                                    <span v-if="canViewPercentage(activity)" class="no-wrap">{{ activity.completionPercentage }}% complete</span>

                                    <span v-else-if="canShowScore(activity)">Scored {{ activity.scorePercentage }}%</span>

                                    <div class="reason" v-if="activity.assessmentDetails && activity.assessmentDetails.extraAttemptReason">
                                        Reason: {{activity.assessmentDetails.extraAttemptReason}}
                                    </div>
                                </td>

                                <td>
                                    <span v-if="activity.assessmentDetails && activity.assessmentDetails.maximumAttempts">
                                        {{ activity.assessmentDetails.currentAttempt}}/{{activity.assessmentDetails.maximumAttempts }}
                                    </span>
                                </td>
                                <td class="no-wrap">
                                    <span class="hide-if-desktop">Status: </span>
                                    {{ getActivityStatusText(activity) }}
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import '../filters';
    import moment from 'moment';
    import { ActivityDetailedModel } from '../models/mylearning/activityDetailedModel';
    import { ActivityDetailedItemModel } from '../models/mylearning/activityDetailedItemModel';
    import { MyLearningRequestModel } from '../models/mylearning/myLearningRequestModel';
    import { UserBasicModel } from '../models/userBasicModel';
    import { myLearningData } from '../data/myLearning';
    import { userData } from '../data/user';

    import { commonlib } from '../common';
    import { ActivityStatus, ResourceType } from '../constants';

    export default Vue.extend({
        components: {
        },
        data() {
            return {
                isLoading: true,
                hideLoadMore: false,
                isShowingFilters: false,
                currentFilters: '',
                activityModel: new ActivityDetailedModel(),
                userBasicModel: new UserBasicModel(),
                errorMessage: '',
                filters: new MyLearningRequestModel(),
                ResourceType: ResourceType,
                searchTextInBox: '',
                skip: 0,
                take: 999
            };
        },
        async created() {
            this.searchTextInBox = this.searchText as string;
            this.loadUserDetails();
            this.loadActivity();
        },
        methods: {
            async loadUserDetails() {
                await userData.getCurrentUserBasicDetails()
                    .then(response => {
                        this.userBasicModel = response;
                    })
                    .catch(error => {
                        this.errorMessage = error;
                    })
            },
            async loadActivity() {
                this.isLoading = true;
                this.skip = 0;
                await myLearningData.getActivitiesDetailed(this.getFilterModel())
                    .then(response => {
                        this.activityModel = response;
                        this.errorMessage = '';
                        this.isLoading = false;

                        // Post-processing to work out which activities are the most recent for the each resource.
                        let mostRecentResources: number[] = [];
                        this.activityModel.activities.forEach(activity => {
                            if (mostRecentResources.indexOf(activity.resourceReferenceId) == -1) {
                                activity.isMostRecent = true;
                                mostRecentResources.push(activity.resourceReferenceId);
                            }
                        });
                    })
                    .catch(error => {
                        this.errorMessage = error;
                        this.isLoading = false;
                    })
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
            canViewPercentage(activity: ActivityDetailedItemModel): boolean {
                return ((activity.resourceType == ResourceType.VIDEO || activity.resourceType == ResourceType.AUDIO) && activity.activityStatus == ActivityStatus.InProgress)
                    || (activity.resourceType == ResourceType.ASSESSMENT && activity.complete == false);
            },
            canShowScore(activity: ActivityDetailedItemModel): boolean {
                return (activity.resourceType === ResourceType.SCORM && (activity.activityStatus == ActivityStatus.Passed || activity.activityStatus == ActivityStatus.Failed))
                    || (activity.resourceType === ResourceType.ASSESSMENT && activity.complete);
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
            printReport() {
                window.print();
            },
            goBack() {
                this.$router.push({
                    name: 'ActivityDetailed',
                    query: {
                        searchText: this.searchText,
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
                        timePeriod: this.timePeriod,
                        startDate: this.startDate,
                        endDate: this.endDate,
                    }
                });
            }
        },
        computed: {
            searchText: {
                get(): string { return this.$route.query.searchText as string; },
            },
            weblink: {
                get(): boolean { return Boolean(this.$route.query.weblink); },
            },
            file: {
                get(): boolean { return Boolean(this.$route.query.file); },
            },
            video: {
                get(): boolean { return Boolean(this.$route.query.video); },
            },
            article: {
                get(): boolean { return Boolean(this.$route.query.article); },
            },
            image: {
                get(): boolean { return Boolean(this.$route.query.image); },
            },
            audio: {
                get(): boolean { return Boolean(this.$route.query.audio); },
            },
            elearning: {
                get(): boolean { return Boolean(this.$route.query.elearning); },
            },
            assessment: {
                get(): boolean { return Boolean(this.$route.query.assessment); },
            },
            ecase: {
                get(): boolean { return Boolean(this.$route.query.ecase); },
            },
            complete: {
                get(): boolean { return Boolean(this.$route.query.complete); },
            },
            incomplete: {
                get(): boolean { return Boolean(this.$route.query.incomplete); },
            },
            passed: {
                get(): boolean { return Boolean(this.$route.query.passed); },
            },
            failed: {
                get(): boolean { return Boolean(this.$route.query.failed); },
            },
            downloaded: {
                get(): boolean { return Boolean(this.$route.query.downloaded); },
            },
            timePeriod: {
                get(): string { return this.$route.query.timePeriod as string; },
            },
            startDate: {
                get(): string { return this.$route.query.startDate as string; },
            },
            endDate: {
                get(): string { return this.$route.query.endDate as string; },
            }
        }
    })

</script>