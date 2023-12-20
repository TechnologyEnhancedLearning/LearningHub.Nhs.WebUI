<template>
    <div class="lh-container-xl" style="background:white;">
        <a href="/home" class="previous-nav-link">
            <svg viewBox="0 0 26 26" aria-hidden="true">
                <path d="M8.5 12c0-.3.1-.5.3-.7l5-5c.4-.4 1-.4 1.4 0s.4 1 0 1.4L10.9 12l4.3 4.3c.4.4.4 1 0 1.4s-1 .4-1.4 0l-5-5c-.2-.2-.3-.4-.3-.7z"></path>
            </svg>Go back</a>
        <h2 class="nhsuk-heading-l catalogue-list-header">All catalogues</h2>
        <div class="row" v-for="chunk in chunkedCatalogues">
            <div class="col" v-for="(item, index) in chunk">
                <catalogue-card v-bind:type="'all-catalogues'" v-bind:item="item" v-bind:index="index" v-if="item.name"></catalogue-card>
            </div>
        </div>
        <div class="load-more">
            <button class="btn btn-outline-custom" @click="loadCatalogues()" v-if="catalogueCardsResult && catalogueCardsResult.catalogues && catalogueCardsResult.catalogues.length < catalogueCardsResult.totalCount">Load more</button>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';        
    import Chunk from 'lodash';
    import '../filters';
    import CatalogueCard from './cataloguecard.vue';
    import { CatalogueCardModel, CatalogueCardsResultModel } from '../models/dashboardModel';
    import { dashboardData } from '../data/dashboard';
    var cardsdict = {
        "374": 1,
        "738": 2,
        "992": 3,
    };
    export default Vue.extend({
        components: {
            CatalogueCard
        },
        props: {
            item: { type: Object } as PropOptions<CatalogueCardModel>,
            index: { type: Number } as PropOptions<Number>
        },
        data() {
            return {
                catalogueCardsResult: new CatalogueCardsResultModel(), 
                chunkedCatalogues: null,
                fetchRows: 9,
                fetchPerLoadMore: 9,
                chunkSize:3
            };
        },
        mounted() {
            window.addEventListener('resize', this.handleChunks);
        },
        computed: {           
        },
        created() {
            this.loadCatalogues();
        },
        methods: {
            handleChunks() {
                var width = window.innerWidth;

                switch (true) {
                    case width >= 992:
                        this.chunkSize = cardsdict["992"];
                        break;
                    case width >= 738:
                        this.chunkSize = cardsdict["738"];
                        break;
                    case width < 738:
                        this.chunkSize = cardsdict["374"];
                        break;
                    default:
                        this.chunkSize = cardsdict["992"];
                        break;
                }           
                this.chunkedCatalogues = Chunk.chunk(this.catalogueCardsResult.catalogues, this.chunkSize);   

                let chunkedCatalogues = this.chunkedCatalogues[this.chunkedCatalogues.length - 1];
                while (chunkedCatalogues.length != this.chunkSize) {
                    chunkedCatalogues.push(new CatalogueCardModel ());
                }
            },
            loadCatalogues() {
                dashboardData.getCatalogues('all-catalogues', this.fetchRows).then(response => {
                    this.catalogueCardsResult = response;  
                    if (this.catalogueCardsResult.catalogues.length === this.fetchRows) {
                        this.fetchRows = this.fetchRows + this.fetchPerLoadMore;
                    }
                    this.handleChunks();
                });
            },
        }
    })

</script>

<style lang="scss" scoped=scoped>
    @use "../../../Styles/abstracts/all" as *;
    .row {
        margin-bottom: 20px;
    }
    .catalogue-card {
        width: 372px;
        height: 385px;

        @media(max-width: 1024px) {
            width: auto;
            max-height: 470px;
        }

        @media(max-width: 414px) {
            width:auto;
        }
    }

    .catalogue-list-header {
        margin-top: 20px;
        font-size: 24px;

        @media(max-width: 414px) {
            font-size: 20px;
        }
    }
</style>
