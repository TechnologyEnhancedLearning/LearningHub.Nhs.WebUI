<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl">
            <div class="user-entry mx-0">
                <form method="post" v-bind:action="authLoginUrl">

                    <h1 class="heading-xl">You already have an account</h1>
                    <p>
                        You already have an elearning for Healthcare Hub account. You can use your username and password for this to access the Learning Hub.
                    </p>
                    <div class="row">
                        <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.username.$error}">
                            <label for="username">Username</label>
                            <div class="error-text" v-if="$v.username.$invalid && $v.username.$dirty">
                                <span class="text-danger">Enter your username.</span>
                            </div>
                            <input type="text" class="form-control" id="Username" name="Username" aria-describedby="Username" autocomplete="off"
                                   v-model.trim="username"
                                   @blur="$v.username.$touch()"
                                   v-bind:class="{ 'input-validation-error': $v.username.$error}">
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.password.$error}">
                            <label for="emailAddress">Password</label>
                            <div class="error-text" v-if="$v.password.$invalid && $v.password.$dirty">
                                <span class="text-danger">Enter your password.</span>
                            </div>
                            <input type="password" class="form-control" id="Password" name="Password" aria-describedby="Password" autocomplete="off"
                                   v-model.trim="password"
                                   @blur="$v.password.$touch()"
                                   v-bind:class="{ 'input-validation-error': $v.password.$error}">
                        </div>
                    </div>

                    <router-link to="/create-an-account" tag="button" class="btn btn-outline-custom">
                        Previous
                    </router-link>

                    <button class="btn btn-custom ml-5" type="submit">
                        Sign in
                    </button>

                    <p>
                        <a href="/forgotten-password">Forgotten your username or password?</a>
                    </p>

                </form>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { required } from "vuelidate/lib/validators";

    export default Vue.extend({
        data() {
          return {
            username: "",
            password: ""
          };
        },
        props: {
            authService: String
        },
        validations: {
            username: {
                required
            },
            password: {
                required
            }
        },
        computed: {
            authLoginUrl() {
                //return this.$props.authService + '/account/login';
                return '/Account/RegistrationLogon';
            }
        },

    })

</script>
