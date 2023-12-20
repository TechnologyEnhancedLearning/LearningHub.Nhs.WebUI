<template>
    <div>
        <div class="lh-padding-fluid">
            <div class="lh-container-xl">
                <div class="user-entry mx-0">
                    <form @submit.prevent="onSubmit">

                        <h1 class="heading-xl">Create an account</h1>
                        <p>
                            Your work email address helps us check if you already have an account or if you can have one.
                        </p>
                        <p>
                            <a :href="whoCanAccessUrl" target="_blank">Who can access the Learning Hub?</a>
                        </p>
                        <div class="row">
                            <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.emailAddress.$error}">
                                <label for="emailAddress">Your work email address</label>
                                <div class="error-text" v-if="$v.emailAddress.$invalid && $v.emailAddress.$dirty">
                                    <span class="text-danger">{{returnError('emailAddress')}}</span>
                                </div>
                                <input type="text" class="form-control" id="emailAddress" aria-describedby="emailAddress" autocomplete="off"
                                       v-model.trim="emailAddress"
                                       @blur="$v.emailAddress.$touch()"
                                       v-bind:class="{ 'input-validation-error': $v.emailAddress.$error}">
                            </div>
                        </div>
                        <div class="row">
                            <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.confirmEmailAddress.$error}">
                                <label for="confirmEmailAddress">Confirm your work email address</label>
                                <div class="error-text" v-if="$v.confirmEmailAddress.$invalid && $v.confirmEmailAddress.$dirty">
                                    <span class="text-danger">{{returnError('confirmEmailAddress')}}</span>
                                </div>
                                <input type="text" class="form-control" id="confirmEmailAddress" aria-describedby="confirmEmailAddress" autocomplete="off"
                                       v-model.trim="confirmEmailAddress"
                                       @blur="$v.confirmEmailAddress.$touch()"
                                       v-bind:class="{ 'input-validation-error': $v.confirmEmailAddress.$error}">
                            </div>
                        </div>
                        <div class="mt-4">
                            <button type="submit" class="btn btn-custom my-2">Next</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>

        <div class="user-entry fullwidth whiteBG mx-0">

            <div class="lh-padding-fluid">
                <div class="lh-container-xl">
                    <div class="whiteBGinner mx-0">
                        <h2 class="nhsuk-heading-l">Do you have an OpenAthens account?</h2>

                        <img src="/images/openathenslogo.png" class="align-middle mb-4 mt-4" alt="OpenAthens logo" />

                        <p>You can use your OpenAthens account to sign in to the Learning Hub.</p>

                        <button class="btn btn-custom mt-5" @click="launchOpenAthens">
                            Sign in with OpenAthens
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import AxiosWrapper from '../axiosWrapper';
    import { required, email, sameAs, maxLength } from "vuelidate/lib/validators";

    export default Vue.extend({
        data() {
            return {
                emailAddress: "",
                confirmEmailAddress: ""
            };
        },
        props: {
            authServiceUrl: String,
            whoCanAccessUrl: String
        },
        methods: {
            onSubmit() {
                if (this.$v.$invalid) {
                    this.$v.emailAddress.$touch();
                    this.$v.confirmEmailAddress.$touch();
                } else {
                    AxiosWrapper.axios.get('/api/registration/validate-email', {
                        params: {
                            emailAddress: this.emailAddress
                        }
                    })
                        .then(response => {
                            // 1 = NewUserNotEligible (screen 2)
                            // 2 = ExistingUserNotEligible (screen 3)
                            // 3 = ExistingUserIsEligible (screen 4)
                            // 4 = PersonalInformation (screen 5)
                            if (response.data == 'ExistingUserIsEligible') {
                                window.location.href = "/Account/RegistrationNotRequired";
                            } else if (response.data == 'NewUserIsEligible') {
                                this.$store.commit('setEmailAddress', this.emailAddress);
                                this.$router.push({ name: 'PersonalInformation' });
                            } else {
                                this.$router.push({ name: response.data });
                            }
                        })
                        .catch(e => {
                            console.log(e);
                        });
                }
            },
            launchOpenAthens() {
                var returnUrl = encodeURIComponent(location.protocol + '//' + location.host + '/Account/AuthorisationRequired/?originalUrl=%2F');
                window.location.href = '/openathens/login?returnUrl=' + returnUrl;
            },
            returnError(ctrl: string) {
                var errorMessage = '';
                switch (ctrl) {
                    case 'emailAddress':
                        if (this.$v.emailAddress.$invalid) {
                            if (!this.$v.emailAddress.required) {
                                errorMessage = "Enter an email address."
                            } else if (!this.$v.emailAddress.email) {
                                errorMessage = "Enter a valid email address."
                            } else if (!this.$v.emailAddress.maxLength) {
                                errorMessage = "Email address must be less than 100 characters."
                            }
                        }
                        break;
                    case 'confirmEmailAddress':
                        if (this.$v.confirmEmailAddress.$invalid) {
                            if (!this.$v.confirmEmailAddress.required) {
                                errorMessage = "Confirm your email address."
                            } else if (!this.$v.confirmEmailAddress.sameAsEmail) {
                                errorMessage = "Email addresses do not match."
                            }
                        }
                        break;
                }
                return errorMessage;
            }
        },
        validations: {
            emailAddress: {
                required,
                email,
                maxLength: maxLength(100)
            },
            confirmEmailAddress: {
                required,
                sameAsEmail: sameAs('emailAddress')
            }
        }
    })

</script>
