import Vue, { PropOptions } from 'vue'
import { email, required } from "vuelidate/lib/validators";
import Vuelidate from "vuelidate";
import AxiosWrapper from './axiosWrapper';
import './filters'

Vue.use(Vuelidate as any);

var forgotPasswordApp = new Vue({
    el: '#forgotPassword',
    data: {
        emailAddress: "",
        submitted: false,
        result: null,
        showError: false
    },
    methods: {
        async submit() {
            this.$v.$touch();
            if (this.$v.$invalid) {
                this.showError = true;
            } else {
                this.submitted = true;
                this.showError = false;
                await AxiosWrapper.axios.post("/Account/ForgotPassword", { EmailAddress: this.emailAddress }, {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                    .then(x => {
                        this.submitted = false;
                        this.result = !x.data.duplicate;
                    });
            }
        }
    },
    validations: {
        emailAddress: {
            required,
            email
        }
    }

});