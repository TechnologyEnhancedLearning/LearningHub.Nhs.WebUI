import Vue from 'vue';
import Notifications from './notifications.vue';

new Vue({
    el: '#notificationcontainer',      
    components: {        
        Notifications,
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