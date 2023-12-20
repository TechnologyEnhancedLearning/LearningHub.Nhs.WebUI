<template>
    <div class="tab-panel">
        <form id="changePasswordForm" @submit.prevent="onSubmit">
            <div class="tab-panel-content">
                <div class="lh-container-xl">
                    <div class="user-entry  mx-0">
                        <p>
                            Your password must be a minimum of 8 characters and must contain at
                            least one numeric character, one upper case character and one lower case
                            character. Your username and password cannot match.
                        </p>
                        <div class="pl-0">
                            <div class="row">
                                <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.newPassword.$error}">
                                    <label for="newPassword" class="pt-10">New password</label>
                                    <div class="error-text" v-if="$v.newPassword.$invalid && $v.newPassword.$dirty">
                                        <span class="text-danger">{{returnError('newPassword')}}</span>
                                    </div>
                                    <input type="password" class="form-control" id="newPassword" aria-describedby="newPassword" autocomplete="off"
                                           v-model.trim="newPassword"
                                           placeholder="New password"
                                           @blur="$v.newPassword.$touch()"
                                           v-bind:class="{ 'input-validation-error': $v.newPassword.$error}">
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.confirmPassword.$error}">
                                    <label for="confirmPassword" class="pt-10">Confirm new password</label>
                                    <div class="error-text" v-if="$v.confirmPassword.$invalid && $v.confirmPassword.$dirty">
                                        <span class="text-danger">{{returnError('confirmPassword')}}</span>
                                    </div>
                                    <input type="password" class="form-control" id="confirmPassword" aria-describedby="confirmPassword" autocomplete="off"
                                           v-model.trim="confirmPassword"
                                           placeholder="Confirm new password"
                                           @blur="$v.confirmPassword.$touch()"
                                           v-bind:class="{ 'input-validation-error': $v.confirmPassword.$error}">
                                </div>
                            </div>
                            <input type="hidden" id="userName" v-model.trim="userName" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="white-background footerButtons lh-container-xl">
                <div class="col-md-6 success-container">
                    <div class="save-success" v-if="passwordUpdated">
                        <i class="fa-solid fa-check"></i><p class="ml-3">Your changes have been saved</p>
                    </div>
                </div>
                <button type="submit" @click.prevent="onSubmit" :disabled="$v.$anyError" class="btn btn-green  mr-25">Change password</button>
                <!--<button type="reset" class="btn btn-outline-custom btn-cancel">Cancel</button>-->
            </div>
        </form>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Vuelidate from "vuelidate";
    import { required, maxLength, minLength, sameAs, not } from "vuelidate/lib/validators";
    Vue.use(Vuelidate as any);
    import { userData } from '../data/user';

    // Password list taken from here https://github.com/danielmiessler/SecLists/blob/master/Passwords/Common-Credentials/top-passwords-shortlist.txt
    const commonPasswords: string[] = [
        'password',
        '123456',
        '12345678',
        'abc123',
        'querty',
        'monkey',
        'letmein',
        'dragon',
        '111111',
        'baseball',
        'iloveyou',
        'trustno1',
        '1234567',
        'sunshine',
        'master',
        '123123',
        'welcome',
        'shadow',
        'ashley',
        'footbal',
        'jesus',
        'michael',
        'ninja',
        'mustang',
        'password1'];

    const isValidPassword = (value: string) => {
        if (typeof value === 'undefined' || value === null || value === '') {
            return true
        }
        return /(?=.*?[^\w\s])(?=.*?[0-9])(?=.*?[A-Z])(?=.*?[a-z]).*/.test(value);
    }

    const isNotCommon = (value: string) => {
        if (typeof value === 'undefined' || value === null || value === '') {
            return true
        }
        const password = value.toLowerCase();
        for (let i = 0; i > commonPasswords.length; i++) {
            if (commonPasswords[i] == password || commonPasswords[i].includes(password) || password.includes(commonPasswords[i])) {
                return false;
            }
        }
        return true;
    }

    export default Vue.extend({
        components: {
        },
        props: {

        },
        data() {
            return {
                userName: this.$store.state.user.userName,
                newPassword: '',
                confirmPassword: '',
                passwordUpdated: false,
            }
        },
        computed: {

        },
        async created() {
        },
        mounted() {
            this.passwordUpdated = false;
        },
        methods: {
            onSubmit() {
                if (this.$v.$invalid) {
                    this.$v.newPassword.$touch();
                    this.$v.confirmPassword.$touch();
                    return;
                }
                userData.changePassword(this.newPassword, this.confirmPassword).then(response => {
                    this.passwordUpdated = true;
                });
            },
            returnError(ctrl: string) {
                var errorMessage = '';
                switch (ctrl) {
                    case 'newPassword':
                        if (this.$v.newPassword.$invalid) {
                            if (!this.$v.newPassword.required) {
                                errorMessage = "Enter new password."
                            }
                            else if (!this.$v.newPassword.maxLength) {
                                errorMessage = "Password must be less than 50 characters."
                            }
                            else if (!this.$v.newPassword.minLength) {
                                errorMessage = "Password must be at least 8 characters."
                            }
                            else if (!this.$v.newPassword.userNamePasswordMatch) {
                                errorMessage = "Your username and password cannot match."
                            }
                            else if (!this.$v.newPassword.isValidPassword) {
                                errorMessage = "Password does not match the rules"
                            }
                        }
                        break;
                    case 'confirmPassword':
                        if (this.$v.confirmPassword.$invalid) {
                            if (!this.$v.confirmPassword.required) {
                                errorMessage = "Confirm new Password."
                            }
                            else if (!this.$v.confirmPassword.maxLength) {
                                errorMessage = "Password must be less than 50 characters."
                            }
                            else if (!this.$v.confirmPassword.minLength) {
                                errorMessage = "Password must be at least 8 characters."
                            }
                            else if (!this.$v.confirmPassword.passwordMatch) {
                                errorMessage = "Password values does not match."
                            }
                            else if (!this.$v.confirmPassword.isValidPassword) {
                                errorMessage = "Password does not match the rules"
                            }
                        }
                        break;
                }
                return errorMessage;
            },
        },
        validations: {
            userName: {
            },
            newPassword: {
                required,
                maxLength: maxLength(50),
                minLength: minLength(8),
                isValidPassword,
                userNamePasswordMatch: not(sameAs('userName')),
                isNotCommon,
            },
            confirmPassword: {
                required,
                maxLength: maxLength(50),
                minLength: minLength(8),
                isValidPassword,
                userNamePasswordMatch: not(sameAs('userName')),
                passwordMatch: sameAs('newPassword'),
                isNotCommon,
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .white-background {
        background-color: #ffffff;
    }

    .success-container {
        padding-left: 0;
        padding-right: 0;

        @media(max-width: 1200px) {
            padding-right: 15px;
        }

        @media(max-width: 768px) {
            padding-right: 0;
        }
    }

    .save-success {
        color: $nhsuk-green;
        background-color: #CCE5D8;
        border: 1px solid $nhsuk-green;
        display: flex;
        justify-content: center;
        align-items: center;
        padding: 6px;
        margin-bottom: 20px;

        p {
            margin: 0;
        }
    }

    .tab-panel-content {
        padding-top: 40px;
        background-color: #F0F4F5;
        padding-left: 4rem;
        padding-right: 4rem;
    }

    @media (max-width: 768px) {
        .tab-panel-content {
            padding-left: 15px !important;
            padding-right: 5px !important;
        }
    }

    .footerButtons {
        padding-top: 30px;
        padding-bottom: 80px;
        padding-right: 30px !important;
        padding-left: 0 !important;
    }

    @media(max-width: 1200px) {
        .footerButtons {
            padding-left: 4rem !important;
        }
    }

    @media (max-width: 768px) {
        .footerButtons {
            padding-left: 15px !important;
            padding-right: 5px !important;
        }
    }

    .btn:disabled {
        background-color: #AEB7BD !important;
        border: 1px solid #AEB7BD !important;
    }
</style>