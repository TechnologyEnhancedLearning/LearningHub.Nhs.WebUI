
<!-- NOTE: There is now a lightweight rating component that is designed to be used on screens where multiple rating components are being displayed (RatingBasic.vue) -->

<template>
    <div :id=this.entityVersionId>
        <div class="rating-component mt-4">
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

                <div class="ml-2" v-if="errorLoadingRating">Error loading rating!</div>
                <div class="ml-2" v-if="ratingSummary.ratingCount == 0 && !errorLoadingRating">Not yet rated</div>
                <div class="ml-2" v-if="ratingSummary.ratingCount > 0">{{ ratingSummary.averageRating }} out of 5</div>

                <div class="flex-break" v-if="!singleLineLayout" />
                <div v-if="singleLineLayout">&nbsp;</div>

                <div v-if="ratingSummary.ratingCount > 0">(<a v-on:click="showRatingSummary()" :href="'#' + getRatingSummaryId()" data-toggle="modal" :data-target="'#' + getRatingSummaryId()">{{ ratingSummary.ratingCount }} rating{{ (ratingSummary.ratingCount != 1) ? "s" : "" }}</a>)&nbsp;</div>
                <div v-if="userAuthenticated && ratingsLoaded && !ratingSummary.userIsContributor && !readOnly" class="rating-add-change">
                    <div v-if="ratingSummary.userHasAlreadyRated">You gave this resource a {{ ratingSummary.userRating }} star rating.&nbsp;</div>
                    <div><a href="#rateThis" data-toggle="modal" data-target="#rateThis">{{ getRateThisText }}</a></div>
                </div>
            </div>
        </div>

        <div id="rateThis" class="modal rate-this-modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content rating-modal">
                    <div class="modal-header text-center">
                        <div class="close-button">
                            <button type="button" class="close" @click="closeRateThisModal" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <h1 class="nhsuk-heading-xl">Rate this resource</h1>
                    </div>

                    <div class="modal-body" v-show="userCanRate || ratingSummary.userCanRate">
                        <div class="error-message">{{ errorMessage }}</div>
                        <div class="star-buttons">
                            <button class="star-button" aria-label="1 stars" @mouseover="highlightRating(1)" @mouseleave="unhighlightRating()" @click="selectRating(1)"></button>
                            <button class="star-button" aria-label="2 stars" @mouseover="highlightRating(2)" @mouseleave="unhighlightRating()" @click="selectRating(2)"></button>
                            <button class="star-button" aria-label="3 stars" @mouseover="highlightRating(3)" @mouseleave="unhighlightRating()" @click="selectRating(3)"></button>
                            <button class="star-button" aria-label="4 stars" @mouseover="highlightRating(4)" @mouseleave="unhighlightRating()" @click="selectRating(4)"></button>
                            <button class="star-button" aria-label="5 stars" @mouseover="highlightRating(5)" @mouseleave="unhighlightRating()" @click="selectRating(5)"></button>
                        </div>
                        <div class="star-descriptions small">
                            <div>Poor</div>
                            <div>Excellent</div>
                        </div>
                        <div class="submit-panel">
                            <button type="submit" class="btn btn-green" @click="submitRating" :disabled="formSubmitting" v-bind:class="{ 'button-processing': formSubmitting }">
                                {{submitCaption}}
                                <i class="fa fa-spinner fa-spin" v-if="formSubmitting"></i>
                            </button>
                        </div>
                    </div>

                    <div v-show="!ratingSummary.userCanRate && !userCanRate">
                        <div class="cant-rate-box">
                            You need to access this resource before you can rate it.
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div :id=getRatingSummaryId() class="modal rating-summary-modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
            <div class="modal-dialog modal-dialog-centered small" role="document">
                <div class="modal-content rating-modal">
                    <div class="modal-header">
                        <div class="close-button">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>

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
                                    <div :id="this.entityVersionId + 'progressBar5star'" class="progress-bar bg-warning rating-bar" aria-valuemin="0" aria-valuemax="100" :style="{ width: this.ratingSummary.rating5StarPercent + '%' }"></div>
                                </div>
                            </div>
                            <div class="rating-value">{{ ratingSummary.rating5StarPercent }}%</div>
                        </div>
                        <div class="rating-row">
                            <div class="rating-label">4 star</div>
                            <div class="rating-progress-bar">
                                <div class="progress">
                                    <div :id="this.entityVersionId + 'progressBar4star'" class="progress-bar bg-warning rating-bar" aria-valuemin="0" aria-valuemax="100" :style="{ width: this.ratingSummary.rating4StarPercent + '%' }"></div>
                                </div>
                            </div>
                            <div class="rating-value">{{ ratingSummary.rating4StarPercent }}%</div>
                        </div>
                        <div class="rating-row">
                            <div class="rating-label">3 star</div>
                            <div class="rating-progress-bar">
                                <div class="progress">
                                    <div :id="this.entityVersionId + 'progressBar3star'" class="progress-bar bg-warning rating-bar" aria-valuemin="0" aria-valuemax="100" :style="{ width: this.ratingSummary.rating3StarPercent + '%' }"></div>
                                </div>
                            </div>
                            <div class="rating-value">{{ ratingSummary.rating3StarPercent }}%</div>
                        </div>
                        <div class="rating-row">
                            <div class="rating-label">2 star</div>
                            <div class="rating-progress-bar">
                                <div class="progress">
                                    <div :id="this.entityVersionId + 'progressBar2star'" class="progress-bar bg-warning rating-bar" aria-valuemin="0" aria-valuemax="100" :style="{ width: this.ratingSummary.rating2StarPercent + '%' }"></div>
                                </div>
                            </div>
                            <div class="rating-value">{{ ratingSummary.rating2StarPercent }}%</div>
                        </div>
                        <div class="rating-row">
                            <div class="rating-label">1 star</div>
                            <div class="rating-progress-bar">
                                <div class="progress">
                                    <div :id="this.entityVersionId + 'progressBar1star'" class="progress-bar bg-warning rating-bar" aria-valuemin="0" aria-valuemax="100" :style="{ width: this.ratingSummary.rating1StarPercent + '%' }"></div>
                                </div>
                            </div>
                            <div class="rating-value">{{ ratingSummary.rating1StarPercent }}%</div>
                        </div>
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
        name: 'RatingComponent',
        props: {
            /* The averageRating and ratingCount parameters are optional. If supplied, the component will display
             * those values instead of making a call to get the data from the database.*/
            averageRating: { Type: Number, required: false } as PropOptions<number>,
            ratingCount: { Type: Number, required: false } as PropOptions<number>,

            entityVersionId: { Type: Number, required: true } as PropOptions<number>,
            userAuthenticated: { Type: Boolean, required: true } as PropOptions<boolean>,
            readOnly: { Type: Boolean, default: false, required: false } as PropOptions<boolean>,
            userCanRate: { Type: Boolean, default: false, required: false } as PropOptions<boolean>,
            singleLineLayout: { Type: Boolean, default: false, required: false } as PropOptions<boolean>
        },
        data() {
            return {
                ratingSummary: new RatingSummaryModel(),
                ratingsLoaded: false,
                errorLoadingRating: false,
                errorMessage: '',
                userRatingNew: 0,
                formSubmitting: false
            }
        },
        computed: {
            getRateThisText(): string {
                return (this.ratingSummary.userHasAlreadyRated) ? "Change rating" : "Rate this resource";
            },
            submitCaption: function submitCaption() {
                if (this.formSubmitting) {
                    return 'Processing...';
                } else {
                    return 'Submit';
                }
            },
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
                return rating>= 3 ? "100%" : rating <= 2 ? "0%" : ((rating - 2) * 100) + "%";
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
            if (this.averageRating !== undefined) {
                this.ratingSummary.averageRating = this.averageRating;
                this.ratingSummary.ratingCount = this.ratingCount;
            }
            else {
                await this.loadRatingSummary();
            }

            // Fix to force the rating summary modal to appear in the centre of the screen. On the dashboard screen, in the resources carousel,
            // the rating summary modal only appeared in the correct position for the first resource. It would appear in the same place for
            // resources 2 to 10, i.e. too far to the left. Solution is to move the summary modal to be a child of the 'main' element instead.
            $('main').append($('#' + this.entityVersionId + 'ratingSummary'));
        },
        async mounted() {
            
        },
        methods: {
            async showRatingSummary() {
                if (!this.ratingsLoaded) {
                    await this.loadRatingSummary().then(() => $('#' + this.entityVersionId + 'ratingSummary').modal('show'));
                }
                else {
                    $('#' + this.entityVersionId + 'ratingSummary').modal('show');
                }
            },
            getRatingSummaryId() {
                return this.entityVersionId + "ratingSummary";
            },
            async loadRatingSummary() {
                this.errorLoadingRating = false;
                await ratingData.getRatingSummary(this.entityVersionId)
                    .then(response => {
                        /* If the user has just accessed the resource, the userCanRate flag is set to true by the resource.vue component.
                           We need to make sure that isn't overwritten by the response from the ratings service, which could still come
                           back as false. It depends on the exact timing of the activity being recorded and of the ratings data being returned. */
                        var reenableRating = false;
                        if (this.userCanRate) {
                            reenableRating = true;
                        }

                        this.ratingSummary = response;

                        if (reenableRating) {
                            this.ratingSummary.userCanRate = true;
                        }

                        this.highlightRating(this.ratingSummary.userRating);
                        this.ratingsLoaded = true;
                    })
                    .catch(e => {
                        console.log(e);
                        this.errorLoadingRating = true;
                    });
            },
            highlightRating(stars: number) {
                $('.star-buttons button').slice(0, stars).addClass('star-button-selected');
                $('.star-buttons button').slice(stars).removeClass('star-button-selected');
            },
            unhighlightRating() {
                if (this.userRatingNew > 0) {
                    $('.star-buttons button').slice(0, this.userRatingNew).addClass('star-button-selected');
                    $('.star-buttons button').slice(this.userRatingNew).removeClass('star-button-selected');
                }
                else {
                    $('.star-buttons button').removeClass('star-button-selected');
                }
            },
            selectRating(stars: number) {
                this.errorMessage = '';
                this.userRatingNew = stars;
            },
            submitRating() {
                if (this.userRatingNew == 0) {
                    this.errorMessage = "Select a star rating."
                }
                else {
                    this.formSubmitting = true;
                    if (!this.ratingSummary.userHasAlreadyRated) {
                        ratingData.createRating(this.entityVersionId, this.userRatingNew)
                            .then(response => {
                                this.ratingSummary.userRating = this.userRatingNew;
                                $('#rateThis').modal('hide');
                                this.loadRatingSummary();
                                this.formSubmitting = false;
                            })
                            .catch(e => {
                                console.log(e);
                                this.errorMessage = "An error has occurred.";
                                this.formSubmitting = false;
                            });
                    }
                    else {
                        ratingData.updateRating(this.entityVersionId, this.userRatingNew)
                            .then(response => {
                                this.ratingSummary.userRating = this.userRatingNew;
                                $('#rateThis').modal('hide');
                                this.loadRatingSummary();
                                this.formSubmitting = false;
                            })
                            .catch(e => {
                                console.log(e);
                                this.errorMessage = "An error has occurred.";
                                this.formSubmitting = false;
                            });
                    }
                }
            },
            closeRateThisModal() {
                this.errorMessage = '';
                this.userRatingNew = this.ratingSummary.userRating;
                this.unhighlightRating();
            }
        }
    })
</script>
<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .close-button {
        float: right;
    }

    .rating-component {
        display: flex;
        flex-direction: row;
        flex-wrap: wrap
    }

    .rating-add-change {
        display: flex;
        flex-wrap: wrap;
    }

    @media(max-width: 768px) {
        .rating-component {
            flex-direction: column;
        }

        .rating-add-change::before {
            content: "";
        }
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

    /* Rate this resource modal */
    .rate-this-modal .modal-header {
        display: block;
    }

        .rate-this-modal .modal-header .close-button {
            margin-right: -30px;
            margin-top: -20px;
        }

    .rate-this-modal .modal-content {
        width: 400px;
    }

    .rate-this-modal .submit-panel {
        display: flex;
        justify-content: center;
        margin-top: 20px;
    }

    .star-buttons {
        font-size: 0 /* Removes gaps between adjacent buttons. Stops flickering of stars from mouseleave triggering in the gaps. */
    }

    .star-button {
        background-image: url(/images/star-white.svg);
        background-repeat: no-repeat;
        background-size: contain;
        background-color: white;
        height: 52px;
        width: 57px; /* 5px wider than image to add a visual gap between adjacent stars. */
        border: none;
    }

    .star-button-selected {
        background-image: url('/images/star-gold.svg') !important;
    }

    .star-descriptions {
        display: flex;
        justify-content: space-between;
        padding-left: 10px;
    }

    .cant-rate-box {
        background-color: $nhsuk-grey-white;
        padding: 20px;
    }

    .error-message {
        color: $nhsuk-red;
        margin-bottom: 20px;
        text-align: center;
    }

    @media(max-width: 768px) {
        .rate-this-modal .modal-content {
            min-width: 300px;
            width: 300px;
        }

        .rate-this-modal .modal-body {
            padding-left: 5px;
            padding-right: 5px;
        }

        .star-button {
            height: 45px;
            width: 45px;
            margin-left: 2px;
            margin-right: 2px;
        }

        .star-descriptions {
            padding-left: 5px;
        }
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
