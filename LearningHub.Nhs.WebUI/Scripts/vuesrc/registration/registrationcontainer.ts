import Vue from 'vue';
import PluginFunction from 'vue';
import router from './registration-router';
import Vuelidate from "vuelidate";
import store from './registrationState';
import CreateAccount from './CreateAccount.vue';
import NewUserNotEligible from './NewUserNotEligible.vue';
import ExistingUserNotEligible from './ExistingUserNotEligible.vue';
import ExistingUserIsEligible from './ExistingUserIsEligible.vue';
import PersonalInformation from './PersonalInformation.vue';
import CurrentJobRole from './CurrentJobRole.vue';
import PlaceOfWork from './PlaceOfWork.vue';
import Complete from './Complete.vue';

Vue.use(Vuelidate as any);

new Vue({
    el: '#registrationcontainer',
    router,
    store,
    components: {
        CreateAccount,
        NewUserNotEligible,
        ExistingUserNotEligible,
        ExistingUserIsEligible,
        PersonalInformation,
        CurrentJobRole,
        PlaceOfWork,
        Complete
    }
});

