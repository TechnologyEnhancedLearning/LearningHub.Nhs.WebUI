import Vue from 'vue';
import PageList from './PageList.vue';

new Vue({
    el: '#pageList',
    components: {
        PageList: PageList,
    },
    data: function () {
        return {};
    },
    methods: {},
    watch: {
        '$route': function (to, from) {
        }
    }
});