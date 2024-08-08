<template>
    <div class="roadmap-container mb-40">
        <div v-for="(item, index) in this.roadMapResult.roadMapItems" :class="getRoadMapClass(item)">
            <img :src="downloadImage(item.imageName)" v-if="item.imageName" alt="Road map" />
            <h2>{{ item.title }}</h2>
            <p v-if="item.roadmapDate">{{ item.roadmapDate | formatDate('DD MMM YYYY') }}</p>
            <p v-html="item.description"></p>
        </div>
        <div class="load-more" v-if="roadMapResult && roadMapResult.roadMapItems &&  roadMapResult.roadMapItems.length <  roadMapResult.totalRecords">
            <div class="mt-40">
                <button class="nhsuk-button nhsuk-button--secondary" @click="loadUpdates()">{{ getLoadMoreText() }}</button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Vuelidate from "vuelidate";
    import { RoadMapResultModel, RoadMapModel } from '../models/roadmap';
    import { roadMapData } from '../data/roadmap';
    import '../filters';
    Vue.use(Vuelidate as any);

    export default Vue.extend({
        components: {

        },
        props: {
        },
        data() {
            return {
                fetchCount: 10,
                totalUpdates: 0,
                viewMore: false,
                roadMapResult: new RoadMapResultModel()
            }
        },
        computed: {

        },
        async created() {
            this.handleScrollPosition();
            this.loadUpdates();
        },
        methods: {
            initialise() {
            },
            async loadUpdates() {
                console.log('loading updates');
                await roadMapData.getUpdates(this.fetchCount).then(response => {
                    this.roadMapResult = response;
                    console.dir(this.roadMapResult);
                    this.fetchCount += 10;
                });
            },
            downloadImage(fileName: string): string {
                return "/api/roadmap/download-image/" + encodeURIComponent(fileName);
            },
            getRoadMapClass(roadMap: RoadMapModel) {
                return roadMap.imageName === (undefined || null || "") ? "updates-card pt-3" : "updates-card";
            },
            handleScrollPosition() {
                var scrollTop = window.pageYOffset || document.documentElement.scrollTop;
                // Remove the focus from the Load More button
                if (document.activeElement instanceof HTMLElement) {
                    document.activeElement.blur();
                }
                document.documentElement.scrollTop = scrollTop;
            },
            getLoadMoreText() {
                if (this.roadMapResult.totalRecords - this.roadMapResult.roadMapItems.length > 10)
                    return 'Load 10 more results';

                if (this.roadMapResult.totalRecords - this.roadMapResult.roadMapItems.length === 1)
                    return 'Load 1 more result';
                else {
                    return 'Load ' + (this.roadMapResult.totalRecords - this.roadMapResult.roadMapItems.length).toString() + ' more results';
                }
            },
        },
        validations: {
        }
    })

</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .search-bar {
        display: none !important;
    }
</style>
