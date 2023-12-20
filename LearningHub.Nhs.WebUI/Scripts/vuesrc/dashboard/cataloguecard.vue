<template>
    <div class="catalogue-card">
        <div class="d-flex m-0 p-2">
            <div class="flex">
                <span v-if="item.restrictedAccess" class="catalogue-restricted"><i class="fas fa-lock-open-alt mr-2" v-if="item.hasAccess"></i><i class="fas fa-lock-alt mr-2" v-if="!item.hasAccess"></i>Restricted access</span>
            </div>
            <div class="d-flex ml-auto pr-2">
                <toggle-bookmark :show-label="false"
                                 :item="toggleBookmarkModel"
                                 @onToggled="onToggled($event)" />

            </div>
        </div>
        <div class="catalogue-banner" :style="getBannerUrl()">
        </div>
        <div class="catalogue-card-details">
            <div class="row catalogue-card-header">
                <div class="col col-9"> <h2><a class="card-title" :href="getCatalogueUrl()"><v-clamp autoresize :max-lines="2">{{ item.name }}</v-clamp></a></h2></div>
                <div class="col col-3" v-if="item.badgeUrl"><img :src="getBadgeUrl()" class="catalogue-badge" alt="" /></div>
            </div>
            <div class="card-description">
                <v-clamp autoresize :max-lines="getLinesCount()">{{ commonlib.stripHTMLFromString(item.description) }}</v-clamp>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import $ from 'jquery';
    import '../filters';
    import VClamp from 'vue-clamp';
    import { commonlib } from '../common';
    import { bookmarkData } from '../data/bookmark';
    import { CatalogueCardModel } from '../models/dashboardModel';
    import RatingComponent from '../globalcomponents/RatingComponent.vue';
    import { ToggleBookmarkModel, BookmarkType } from '../models/userBookmark';
    import ToggleBookmark from '../bookmark/togglebookmark.vue';

    export default Vue.extend({
        components: {
            RatingComponent,
            VClamp,
            ToggleBookmark
        },
        props: {
            item: { type: Object } as PropOptions<CatalogueCardModel>,
            index: { type: Number } as PropOptions<Number>,
            type: { type: String } as PropOptions<String>
        },
        data() {
            return {
                commonlib: commonlib,
                toggleBookmarkModel: null as ToggleBookmarkModel,
            };
        },
        computed: {
        },
        created() {
            this.toggleBookmarkModel = new ToggleBookmarkModel(
                {
                    id: this.item.bookmarkId,
                    bookmarkType: BookmarkType.NODE,
                    title: this.item.name,
                    isBookmarked: this.item.isBookmarked,
                    nodeId: this.item.nodeId,
                    link: this.getCatalogueUrl(),
                });
        },
        methods: {
            getCatalogueUrl: function () {
                if (this.item) {
                    return ('../catalogue/' + this.item.url)
                }
                return "";
            },
            getBannerUrl(): string {
                if (this.item.bannerUrl) {
                    return "background-image: url('" + this.getFileLink(this.item.bannerUrl) + "');"
                }
                return "height:auto;";
            },
            getBadgeUrl(): string {
                return "/api/dashboard/download-image/" + encodeURIComponent(this.item.badgeUrl);
            },
            getFileLink(fileName: string): string {
                return "/api/catalogue/download-image/" + encodeURIComponent(fileName);
            },
            getLinesCount(): number {
                var lineCount = 8;

                if (this.type === 'all-catalogues' && this.item.bannerUrl)
                    lineCount = 4;

                if (this.type === 'all-catalogues' && !this.item.bannerUrl)
                    lineCount = 9;

                if (this.type !== 'all-catalogues' && !this.item.bannerUrl)
                    lineCount = 10;

                if (this.type !== 'all-catalogues' && this.item.bannerUrl)
                    lineCount = 5;

                return this.item.restrictedAccess ? lineCount - 1 : lineCount;
            },
            onToggled(toggleBookmarkModel: ToggleBookmarkModel) {
                this.toggleBookmarkModel = toggleBookmarkModel;         

                if (!this.toggleBookmarkModel.isBookmarked) {
                    this.toggleBookmarkModel.title = this.item.name;
                }
            }
        }
    })

</script>

<style lang="scss" scoped=scoped>
    @use "../../../Styles/abstracts/all" as *;

    .catalogue-banner {
        height: 120px;
        background-repeat: no-repeat;
        background-size: cover;
    }

    .catalogue-card {
        margin: 18px 5px 0px 5px;
        height: 430px;
        width: auto;
        background-color: #F0F4F5;
        min-height: 180px !important;
        box-shadow: 0px 4px 0px #AEB7BD;
        line-height: 28px;
        font-size: 19px;
        font-weight: 400;

        @media(max-width: 1024px) {
            width: auto;
            height: 410px;
        }

        @media(max-width: 414px) {
            width: auto;
        }
    }

    .catalogue-card-details {
        margin-left: 26px;
        margin-right: 26px;

        @media(max-width: 1024px) {
            margin-left: 24px;
            margin-right: 24px;
        }
    }

    .catalogue-card-header {
        padding-top: 18px;
        font-size: 22px;
        line-height: 32px;

        @media(max-width: 1024px) {
            padding-top: 26px;
            font-size: 20px;
            line-height:32px;
        }

        @media(max-width: 414px) {
            padding-top: 26px;
            font-size: 20px;
            line-height:28px;
        }
    }

    .catalogue-restricted {
        color: $nhsuk-purple;
    }

    .card-title {
        font-size: 22px;
        line-height: 32px;
        font-weight: 700;

        @media(max-width: 1024px) {
            font-size: 22px;
            line-height:32px;
        }

        @media(max-width: 414px) {
            font-size: 20px;
            line-height:28px;
        }
    }

    .card-description {
        text-align: left;
        font-size: 16px;
        line-height: 28px;
        margin-top: 24px;
        margin-bottom: 36px;
        font-weight: 400;
        min-width: 80%;

        @media(max-width: 1024px) {
            font-size: 16px;
            line-height:28px;
            min-width:80%;
        }

        @media(max-width: 414px) {
            font-size: 16px;
            line-height:24px;
            min-width:80%;
        }
    }

    .catalogue-badge {
        height: 56px;
        width: 56px;
    }
</style>
