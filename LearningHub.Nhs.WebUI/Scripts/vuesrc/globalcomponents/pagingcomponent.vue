<template>
    <div class="d-flex justify-content-between pt-4" v-if="this.totalItems > this.pageSize">

        <div class="text-left navigate">
            <template v-if="this.currentPage > 1">
                <i class="paging-text fa-solid fa-chevron-left"></i>
                <a class="paging-text pl-2" @click="loadPage(currentPage - 1, $event)" href="#">Previous</a>
            </template>
        </div>

        <ul class="paging-page-list list-inline mx-auto">
            <li v-if="minPage > 1">
                <a @click="loadPage(1, $event)" href="#">1</a>
            </li>

            <li v-if="minPage > 1 && minPage > 2">
                ...
            </li>

            <li v-for="item in pageRange" v-if="(item > 0) && (item <= totalPages)">
                <template v-if="item == currentPage">
                    {{item}}
                </template>

                <template v-else>
                    <a @click="loadPage(item, $event)" href="#">{{item}}</a>
                </template>
            </li>

            <template v-if="this.totalPages > (this.currentPage + 1)">
                <li v-if="(maxPage + 1) < this.totalPages">
                    ...
                </li>

                <li v-if="maxPage < this.totalPages">
                    <a @click="loadPage(totalPages, $event)" href="#">{{this.totalPages}}</a>
                </li>
            </template>
        </ul>

        <div class="text-right navigate">
            <template v-if="this.currentPage < this.totalPages">
                <a class="paging-text pr-2" @click="loadPage(currentPage + 1, $event)" href="#">Next</a>
                <i class="paging-text fa-solid fa-chevron-right"></i>
            </template>
        </div>
    </div>
</template>


<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    export default Vue.extend({
        name: 'paging',
        props: {
            currentPage: { Type: Number } as PropOptions<number>,
            pageSize: { Type: Number } as PropOptions<number>,
            totalItems: { Type: Number } as PropOptions<number>,
            totalPages: { Type: Number } as PropOptions<number>,
        },
        data() {
            return {};
        },
        computed: {
            minPage() {
                var currentPage = this.currentPage;
                var minPage = currentPage == 1 ? 1 : currentPage - 1;
                if (currentPage == this.totalPages) {
                    minPage = currentPage - 2;
                }
                return minPage;
            },

            maxPage() {
                var currentPage = this.currentPage;
                return currentPage == 1 ? currentPage + 2 : currentPage + 1;
            },

            pageRange() {
                var list = [];
                for (var i = (this as any).minPage; i <= (this as any).maxPage; i++) {
                    list.push(i);
                }
                return list;
            }
        },
        methods: {
            loadPage: function (pageIndex: number, event: any) {
                this.$emit('loadPage', pageIndex);
                event.preventDefault();
            },
        },
    })

</script>