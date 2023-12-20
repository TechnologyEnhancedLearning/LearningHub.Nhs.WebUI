import Vue from 'vue';
import Vuelidate from "vuelidate";
import router from './mylearning-router';
import LhDatePicker from '../datepicker.vue';
import ActivityDetailed from './ActivityDetailed.vue';
import ActivityDetailedReport from './ActivityDetailedReport.vue';

Vue.use(Vuelidate as any);

new Vue({
    el: '#mylearningcontainer',
    router,
    components: {
        LhDatePicker,
        ActivityDetailed,
        ActivityDetailedReport
    },
});

