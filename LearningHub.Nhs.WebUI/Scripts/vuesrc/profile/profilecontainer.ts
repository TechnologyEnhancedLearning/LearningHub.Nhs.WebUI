import Vue from 'vue';
import Profile from './profile.vue';
import store from './profileState';

new Vue({
    el: '#profilecontainer',  
    store,
    components: {        
        Profile,
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

