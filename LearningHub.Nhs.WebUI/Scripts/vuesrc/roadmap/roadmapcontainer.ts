import Vue from 'vue';

import RoadMap from './roadmap.vue';

new Vue({
    el: '#roadMap',      
    components: {        
        RoadMap,
    },
    data() {
        return {         
        };
    },
    async created() {       
    },
    methods: {       
    },
    watch: {
        '$route'(to, from) {            
        }
    }
});

