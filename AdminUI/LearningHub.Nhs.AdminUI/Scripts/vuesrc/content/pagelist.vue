<template>
    <div id="pageList">
        <div class="table-responsive">
            <table class="table table-striped lh-datatable table-bordered">
                <thead>
                    <tr>
                        <th style="width:70%;">Title</th>
                        <th style="width: 30%; text-align:center">Status</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="page in pageResult.pages" :key="page.id">
                        <td><a :href="getPageLink(page)"> {{ page.name }}</a></td>
                        <td align="center"> {{ contentLib.getPageStatusText(page.pageStatus) }} <i :class="contentLib.getPageStatusIcon(page.pageStatus)"></i></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { contentData } from '../data/content';
    import { PageResultModel } from '../models/content/pageResultModel';
    import { PageModel, PageStatus } from '../models/content/pageModel';
    import { contentLib } from './common';

    export default Vue.extend({
        components: {

        },
        props: {

        },
        data() {
            return {
                contentLib: contentLib,
                pageResult: new PageResultModel(),
            }
        },
        created() {
            this.loadPages();
        },
        methods: {
            loadPages() {
                contentData.getPages().then(response => {
                    this.pageResult = response;
                });
            },
            getPageLink(page: PageModel) {
                return `/cms/page/${page.id}`;
            },
        },
    });
</script>

<style lang="scss" scoped>
    @use "../../../Styles/Abstracts/all" as *;

    .warningTriangle {
        color: #ffb81c;
        font-size: 18px;
    }

    .liveCircle {
        color: #007F3B;
        font-size: 18px;
    }
</style>