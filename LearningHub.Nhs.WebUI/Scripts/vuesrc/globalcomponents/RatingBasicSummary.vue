<template>
    <div id="ratingSummary" class="modal rating-summary-modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
        <div class="modal-dialog modal-dialog-centered small" role="document">
            <div class="modal-content rating-modal">
                <div class="modal-header">
                    <div class="close-button">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>

                    <span v-if="errorLoadingRating">Error loading rating summary!</span>

                    <h1 class="nhsuk-heading-xl">{{ ratingSummary.ratingCount }} rating{{ (ratingSummary.ratingCount != 1) ? "s" : "" }}</h1>
                    <div class="d-flex">
                        <div class="rating-stars">
                            <div class="rating-star star-1">
                                <div class="rating-star-empty"></div>
                                <div class="rating-star-full" :style="{ width: star1Width }"></div>
                            </div>
                            <div class="rating-star star-2">
                                <div class="rating-star-empty"></div>
                                <div class="rating-star-full" :style="{ width: star2Width }"></div>
                            </div>
                            <div class="rating-star star-3">
                                <div class="rating-star-empty"></div>
                                <div class="rating-star-full" :style="{ width: star3Width }"></div>
                            </div>
                            <div class="rating-star star-4">
                                <div class="rating-star-empty"></div>
                                <div class="rating-star-full" :style="{ width: star4Width }"></div>
                            </div>
                            <div class="rating-star star-5">
                                <div class="rating-star-empty"></div>
                                <div class="rating-star-full" :style="{ width: star5Width }"></div>
                            </div>
                        </div>
                        <div>{{ ratingSummary.averageRating }} out of 5</div>
                    </div>
                </div>

                <div class="modal-body">
                    <div class="rating-row">
                        <div class="rating-label">5 star</div>
                        <div class="rating-progress-bar">
                            <div class="progress">
                                <div id="progressBar5star" class="progress-bar bg-warning rating-bar" aria-valuemin="0" aria-valuemax="100" :style="{ width: this.ratingSummary.rating5StarPercent + '%' }"></div>
                            </div>
                        </div>
                        <div class="rating-value">{{ ratingSummary.rating5StarPercent }}%</div>
                    </div>
                    <div class="rating-row">
                        <div class="rating-label">4 star</div>
                        <div class="rating-progress-bar">
                            <div class="progress">
                                <div id="progressBar4star" class="progress-bar bg-warning rating-bar" aria-valuemin="0" aria-valuemax="100" :style="{ width: this.ratingSummary.rating4StarPercent + '%' }"></div>
                            </div>
                        </div>
                        <div class="rating-value">{{ ratingSummary.rating4StarPercent }}%</div>
                    </div>
                    <div class="rating-row">
                        <div class="rating-label">3 star</div>
                        <div class="rating-progress-bar">
                            <div class="progress">
                                <div id="progressBar3star" class="progress-bar bg-warning rating-bar" aria-valuemin="0" aria-valuemax="100" :style="{ width: this.ratingSummary.rating3StarPercent + '%' }"></div>
                            </div>
                        </div>
                        <div class="rating-value">{{ ratingSummary.rating3StarPercent }}%</div>
                    </div>
                    <div class="rating-row">
                        <div class="rating-label">2 star</div>
                        <div class="rating-progress-bar">
                            <div class="progress">
                                <div id="progressBar2star" class="progress-bar bg-warning rating-bar" aria-valuemin="0" aria-valuemax="100" :style="{ width: this.ratingSummary.rating2StarPercent + '%' }"></div>
                            </div>
                        </div>
                        <div class="rating-value">{{ ratingSummary.rating2StarPercent }}%</div>
                    </div>
                    <div class="rating-row">
                        <div class="rating-label">1 star</div>
                        <div class="rating-progress-bar">
                            <div class="progress">
                                <div id="progressBar1star" class="progress-bar bg-warning rating-bar" aria-valuemin="0" aria-valuemax="100" :style="{ width: this.ratingSummary.rating1StarPercent + '%' }"></div>
                            </div>
                        </div>
                        <div class="rating-value">{{ ratingSummary.rating1StarPercent }}%</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { RatingSummaryModel } from '../models/ratingSummaryModel';
    import { ratingData } from '../data/rating';

    export default Vue.extend({
        name: 'RatingBasicSummary',
        props: {
        },
        data() {
            return {
                ratingSummary: new RatingSummaryModel(),
                errorLoadingRating: false,
            }
        },
        computed: {
            star1Width(): string {
                var rating = this.ratingSummary.averageRating;
                return rating >= 1 ? "100%" : "0%";
            },
            star2Width(): string {
                var rating = this.ratingSummary.averageRating;
                return rating >= 2 ? "100%" : rating <= 1 ? "0%" : ((rating - 1) * 100) + "%";
            },
            star3Width(): string {
                var rating = this.ratingSummary.averageRating;
                return rating >= 3 ? "100%" : rating <= 2 ? "0%" : ((rating - 2) * 100) + "%";
            },
            star4Width(): string {
                var rating = this.ratingSummary.averageRating;
                return rating >= 4 ? "100%" : rating <= 3 ? "0%" : ((rating - 3) * 100) + "%";
            },
            star5Width(): string {
                var rating = this.ratingSummary.averageRating;
                return rating == 5 ? "100%" : rating <= 4 ? "0%" : ((rating - 4) * 100) + "%";
            }
        },
        async created() {
        },
        async mounted() {
        },
        methods: {
            async showRatingSummary(entityVersionId: number) {
                await this.loadRatingSummary(entityVersionId).then(() => $('#ratingSummary').modal('show'));
            },
            async loadRatingSummary(entityVersionId: number) {
                this.errorLoadingRating = false;
                await ratingData.getRatingSummary(entityVersionId)
                    .then(response => {
                        this.ratingSummary = response;
                    })
                    .catch(e => {
                        console.log(e);
                        this.errorLoadingRating = true;
                    });
            },
        }
    })
</script>
<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .close-button {
        float: right;
    }

    .rating-stars {
        flex-wrap: wrap;
        margin-bottom: 5px;
        margin-right: 0px;
        flex-shrink: 1;
    }

    .rating-stars > div {
        margin-bottom: 5px;
    }

    .rating-modal {
        margin-left: auto;
        margin-right: auto;
    }

    .modal-header, .modal-body {
        padding: 20px 28px 20px 28px !important;
    }

    /* Rating Summary modal. Includes styling of the average rating stars. */
    .rating-summary-modal .modal-content {
        width: 500px;
        padding: 0;
    }

    .rating-summary-modal .modal-header {
        display: block;
        border-bottom-color: rgb(233, 236, 239);
        border-bottom-style: solid;
        border-bottom-width: 1px;
        padding-top: 20px;
        padding-bottom: 20px;
    }

    .rating-summary-modal .modal-body {
        padding-top: 20px;
        padding-bottom: 20px;
    }

    .rating-row {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-top: 10px;
        margin-bottom: 10px;
    }

    .rating-label {
        width: auto;
    }

    .rating-value {
        width: 30px;
    }

    .rating-progress-bar {
        flex: 1 1 auto;
        margin-left: 10px;
        margin-right: 10px;
    }

    .rating-progress-bar .progress {
        height: 15px;
    }
</style>
