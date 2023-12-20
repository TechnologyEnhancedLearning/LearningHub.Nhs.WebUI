import Vue from 'vue';
import Home from './home.vue';
new Vue({
    el: '#home',
    components: {
        Home: Home,
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