<template>
    <div :id="entityVersionId">
        <div class="rating-component mt-4">
            <div class="rating-stars">
                <div class="rating-star star-1">
                    <div class="rating-star-empty"></div>
                    <div class="rating-star-full" :style="{ width: star1Width }" v-once></div>
                </div>
                <div class="rating-star star-2">
                    <div class="rating-star-empty"></div>
                    <div class="rating-star-full" :style="{ width: star2Width }" v-once></div>
                </div>
                <div class="rating-star star-3">
                    <div class="rating-star-empty"></div>
                    <div class="rating-star-full" :style="{ width: star3Width }" v-once></div>
                </div>
                <div class="rating-star star-4">
                    <div class="rating-star-empty"></div>
                    <div class="rating-star-full" :style="{ width: star4Width }" v-once></div>
                </div>
                <div class="rating-star star-5">
                    <div class="rating-star-empty"></div>
                    <div class="rating-star-full" :style="{ width: star5Width }" v-once></div>
                </div>

                <div class="ml-2" v-if="ratingSummary.ratingCount == 0">Not yet rated</div>
                <div class="ml-2 no-wrap" v-if="ratingSummary.ratingCount > 0" v-once>{{ ratingSummary.averageRating }} out of 5</div>

                <div class="no-wrap" v-if="ratingSummary.ratingCount > 0">(<span class="hyperlink" v-on:click="displayRatingSummary()" v-once>{{ ratingSummary.ratingCount }} rating{{ (ratingSummary.ratingCount != 1) ? "s" : "" }}</span>)&nbsp;</div>
            </div>
        </div>

    </div>
</template>
<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { RatingSummaryModel } from '../models/ratingSummaryModel';

    export default Vue.extend({
        name: 'RatingBasic',
        props: {
            averageRating: { Type: Number, required: false } as PropOptions<number>,
            ratingCount: { Type: Number, required: false } as PropOptions<number>,

            entityVersionId: { Type: Number, required: true } as PropOptions<number>,
        },
        data() {
            return {
                ratingSummary: new RatingSummaryModel(),
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
            this.ratingSummary.averageRating = this.averageRating;
            this.ratingSummary.ratingCount = this.ratingCount;
        },
        async mounted() {
        },
        methods: {
            displayRatingSummary() {
                // Fire event that causes top level catalogue component to display rating summary modal.
                this.$emit('show-rating-summary');
            },
        }
    })
</script>
<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .rating-component {
        display: flex;
        flex-direction: row;
        flex-wrap: wrap
    }

    @media(max-width: 768px) {
        .rating-component {
            flex-direction: column;
        }
    }

    .rating-stars {
        flex-wrap: nowrap;
        margin-bottom: 5px;
        margin-right: 0px;
        flex-shrink: 1;
    }

    .rating-stars > div {
        margin-bottom: 5px;
    }

</style>
