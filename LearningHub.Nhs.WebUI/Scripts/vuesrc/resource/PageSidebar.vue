<template>
    <div class="sidebar">
        <div v-for="(pageNum, pageIndex) in pageCount"
             class="mx-2 p-3"
             :class="{ 'sidebar-selected' : (pageIndex === currentPage),
                                'sidebar-button' : navigatable(pageIndex),
                                'sidebar-locked' :  !navigatable(pageIndex) && pageIndex !== currentPage }"

             :key="pageNum"
             @click="goToPage(pageIndex)">
            <i :class="computeClassForStatus(pagesProgress.pageStatuses[pageIndex])"></i>
            Page {{ pageNum }}
        </div>
        <div v-if="resourceType === ResourceType.ASSESSMENT"
             class="mx-2 p-3"
             :class="{ 'sidebar-selected' : currentPage === pageCount,
                             'sidebar-button' : allPagesCompleted,
                             'sidebar-locked' :  !allPagesCompleted && currentPage !== pageCount + 1 }"
             :key="pageCount + 1"
             @click="goToPage(pageCount)">
            <i :class="computeClassForStatus(summaryStatus())"></i>
            Summary
        </div>
    </div>

</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Button from "../globalcomponents/Button.vue";
    import { PageStatusEnum } from '../models/pageStatusEnum';
    import { PagesProgressModel } from '../models/pagesProgressModel';
    import { ResourceInjection } from "./interfaces/injections";
    import { ResourceType } from "../constants";

    const StatusClasses = {
        [PageStatusEnum.Completed]: ['fa-check-circle', 'green'],
        [PageStatusEnum.Reading]: ['fa-adjust', 'gold'],
        [PageStatusEnum.Locked]: ['fa-lock-alt', 'grey'],
        [PageStatusEnum.Available]: ['fa-ellipsis-h', 'grey'],
    };

    export default (Vue as Vue.VueConstructor<Vue & ResourceInjection>).extend({
        inject: ["resourceType"],
        components: {
            Button,
        },
        props: {
            currentPage: Number,
            pageCount: Number,
            currentPageProgress: String,
            pagesProgress: Object as PropOptions<PagesProgressModel>,
        },
        data() {
            return {
                ResourceType,
                pagesCompleted: 0,
            };
        },
        computed: {
            allPagesCompleted(): boolean {
                return this.pagesProgress.allPagesCompleted;
            },
        },
        methods: {
            computeClassForStatus(status: PageStatusEnum) {
                return [
                    'answer-icon',
                    'fas',
                    ...StatusClasses[status],
                ];
            },
            summaryStatus() {
                if (this.pagesCompleted === this.pageCount + 1 || this.currentPage === this.pageCount) {
                    this.pagesCompleted = this.pageCount + 1;
                    this.$emit('update:summaryViewed', true);
                    return PageStatusEnum.Completed;
                } else if (this.allPagesCompleted) {
                    this.$emit('update:summaryViewed', false);
                    return PageStatusEnum.Available;
                } else {
                    this.$emit('update:summaryViewed', false);
                    return PageStatusEnum.Locked;
                }
            },
            goToPage(page: number) {
                if (this.navigatable(page)) {
                    this.$emit("goToPage", page);
                }
            },
            navigatable(page: number) {
                return page !== this.currentPage && (page < this.pageCount ?
                    this.pagesProgress.pageStatuses[page] !== PageStatusEnum.Locked :
                    this.allPagesCompleted);
            },
        }

    });
</script>

<style lang="scss"
       scoped>
    @use '../../../Styles/abstracts/all' as *;

    .sidebar {
        background-color: $nhsuk-white;
        border-left: 1px solid $nhsuk-grey-light;
    }

    .sidebar-button {
        cursor: pointer;
    }

    .sidebar-button:hover {
        border-left: 5px solid $nhsuk-blue;
        background-color: $nhsuk-grey-lighter;
    }

    .sidebar-button:active {
        border-left: 5px solid $nhsuk-blue;
        background-color: $nhsuk-grey-light;
    }

    .sidebar-selected {
        border-left: 3px solid $nhsuk-navbar-blue;
        background-color: $nhsuk-grey-white;
        cursor: default;
    }

    .sidebar-locked {
        cursor: not-allowed;
        color: $nhsuk-grey-placeholder;
    }

    .grey {
        color: $nhsuk-grey-placeholder;
    }

    .gold {
        color: $nhsuk-warm-yellow;
    }

    .green {
        color: $nhsuk-green;
    }

    .answer-icon {
        margin-right: 20px;
    }


</style>