<template>
    <div class="tab-panel">
        <form id="additionalSecurityForm" @submit.prevent="onSubmit">
            <div class="tab-panel-content">
                <div class="lh-container-xl">
                    <div class="user-entry  mx-0">
                        <p>
                            These security questions can be used to help you to sign in if you forget your password. Please be assured that anything you enter on this page is encrypted and will not be visible to anyone, including system administrators.
                        </p>
                        <p>
                            Health Education England will only use this information to help you sign in to the Learning Hub.
                        </p>
                        <div class="row" v-if="userSecurityQuestionAnswerModel">
                            <div class="form-group col-md-6">
                                <div class="form-group" v-bind:class="{ 'input-validation-error': $v.selectedQuestionOneId.$error}">
                                    <label for="selectedQuestionOneId" class="pt-10">Please select your first question</label>
                                    <div class="error-text" v-if="$v.selectedQuestionOneId.$invalid && $v.selectedQuestionOneId.$dirty">
                                        <span class="text-danger">{{returnError('selectedQuestionOneId')}}</span>
                                    </div>
                                    <select class="form-control" id="questionOne" aria-describedby="question"
                                            v-model="selectedQuestionOneId"
                                            @change="onQuestionOneChange($event)"
                                            placeholder="Select a question..."
                                            @blur="$v.selectedQuestionOneId.$touch()"
                                            v-bind:class="{ 'input-validation-error': $v.selectedQuestionOneId.$error}">
                                        <option v-for="question in userSecurityQuestionAnswerModel.securityQuestions" v-bind:value="question.value">{{ question.text }}</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.questionOneAnswer.$error}">
                                <label for="questionOneAnswer" class="pt-10">Please provide your answer</label>
                                <div class="error-text" v-if="$v.questionOneAnswer.$invalid && $v.questionOneAnswer.$dirty">
                                    <span class="text-danger">{{returnError('questionOneAnswer')}}</span>
                                </div>
                                <input type="password" class="form-control" id="questionOneAnswer" aria-describedby="questionOneAnswer" autocomplete="off"
                                       v-model.trim="questionOneAnswer"
                                       @blur="$v.questionOneAnswer.$touch()"
                                       v-bind:class="{ 'input-validation-error': $v.questionOneAnswer.$error}">
                            </div>
                        </div>
                        <div class="row" v-if="userSecurityQuestionAnswerModel">
                            <div class="form-group col-md-6">
                                <div class="form-group" v-bind:class="{ 'input-validation-error': $v.selectedQuestionTwoId.$error}">
                                    <label for="selectedQuestionTwoId" class="pt-10">Please select your second question</label>
                                    <div class="error-text" v-if="$v.selectedQuestionTwoId.$invalid && $v.selectedQuestionTwoId.$dirty">
                                        <span class="text-danger">{{returnError('selectedQuestionTwoId')}}</span>
                                    </div>
                                    <select class="form-control"
                                            id="questionTwo"
                                            @change="onQuestionTwoChange($event)"
                                            aria-describedby="question"
                                            v-model="selectedQuestionTwoId"
                                            placeholder="Select a question..."
                                            @blur="$v.selectedQuestionTwoId.$touch();"
                                            v-bind:class="{ 'input-validation-error': $v.selectedQuestionTwoId.$error}">
                                        <option v-for="question in userSecurityQuestionAnswerModel.securityQuestions" v-bind:value="question.value">{{ question.text }}</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.questionTwoAnswer.$error}">
                                <label for="questionTwoAnswer" class="pt-10">Please provide your answer</label>
                                <div class="error-text" v-if="$v.questionTwoAnswer.$invalid && $v.questionTwoAnswer.$dirty">
                                    <span class="text-danger">{{returnError('questionTwoAnswer')}}</span>
                                </div>
                                <input type="password" class="form-control" id="questionTwoAnswer" aria-describedby="questionTwoAnswer" autocomplete="off"
                                       v-model.trim="questionTwoAnswer"
                                       @blur="$v.questionTwoAnswer.$touch()"
                                       v-bind:class="{ 'input-validation-error': $v.questionTwoAnswer.$error}">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="white-background footerButtons lh-container-xl ">
                <div class="col-md-6 success-container">
                    <div class="save-success" v-if="updated">
                        <i class="fa-solid fa-check"></i><p class="ml-3">Your changes have been saved</p>
                    </div>
                </div>
                <button type="submit" @click.prevent="onSubmit" :disabled="$v.$anyError" class="btn btn-green  mr-25">Save security questions</button>
                <!--<button type="reset" class="btn btn-outline-custom btn-cancel">Cancel</button>-->
            </div>
        </form>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Vuelidate from "vuelidate";
    import { required, maxLength, sameAs, not, minValue } from "vuelidate/lib/validators";
    Vue.use(Vuelidate as any);
    import { userData } from '../data/user';
    import { UserSecurityQuestionAnswerModel, SecurityQuestion, UserSecurityQuestion } from '../models/userBasicModel';
    export default Vue.extend({
        components: {
        },
        props: {

        },
        data() {
            return {
                userName: this.$store.state.user.userName,
                userSecurityQuestionAnswerModel: null,
                selectedQuestionOneId: 0,
                questionOneAnswer: '',
                selectedQuestionTwoId: 0,
                questionTwoAnswer: '',
                updated: false,
            }
        },
        computed: {
        },
        mounted() {
            this.updated = false;
        },
        async created() {
            await userData.getSecurityQuestionAnswers().then(response => {
                this.userSecurityQuestionAnswerModel = response;
                if (this.userSecurityQuestionAnswerModel.userSecurityQuestions &&
                    this.userSecurityQuestionAnswerModel.userSecurityQuestions.length == 2) {
                    if (this.userSecurityQuestionAnswerModel.userSecurityQuestions[0]) {
                        this.selectedQuestionOneId = this.userSecurityQuestionAnswerModel.userSecurityQuestions[0].securityQuestionId;
                        this.questionOneAnswer = this.userSecurityQuestionAnswerModel.userSecurityQuestions[0].securityQuestionAnswerHash;
                    }
                    if (this.userSecurityQuestionAnswerModel.userSecurityQuestions[1]) {
                        this.selectedQuestionTwoId = this.userSecurityQuestionAnswerModel.userSecurityQuestions[1].securityQuestionId;
                        this.questionTwoAnswer = this.userSecurityQuestionAnswerModel.userSecurityQuestions[1].securityQuestionAnswerHash;
                    }
                }
                if (!this.userSecurityQuestionAnswerModel.userSecurityQuestions || this.userSecurityQuestionAnswerModel.userSecurityQuestions.length == 0) {
                    this.userSecurityQuestionAnswerModel.userSecurityQuestions = [];
                    this.userSecurityQuestionAnswerModel.userSecurityQuestions.push(new UserSecurityQuestion(), new UserSecurityQuestion());
                }
            });
        },
        methods: {
            onQuestionOneChange(event: any) {
                let questionText = event.target.options[event.target.selectedIndex].text;
                this.userSecurityQuestionAnswerModel.userSecurityQuestions[0].securityQuestionId = this.selectedQuestionOneId;
                this.userSecurityQuestionAnswerModel.userSecurityQuestions[0].questionText = questionText;
            },
            onQuestionTwoChange(event: any) {
                let questionText = event.target.options[event.target.selectedIndex].text;
                this.userSecurityQuestionAnswerModel.userSecurityQuestions[1].securityQuestionId = this.selectedQuestionTwoId;
                this.userSecurityQuestionAnswerModel.userSecurityQuestions[1].questionText = questionText;
            },
            onSubmit() {
                if (this.$v.$invalid) {
                    this.$v.selectedQuestionOneId.$touch();
                    this.$v.questionOneAnswer.$touch();
                    this.$v.selectedQuestionTwoId.$touch();
                    this.$v.questionTwoAnswer.$touch();
                    return;
                }
                this.userSecurityQuestionAnswerModel.userSecurityQuestions[0].securityQuestionAnswerHash = this.questionOneAnswer;
                this.userSecurityQuestionAnswerModel.userSecurityQuestions[1].securityQuestionAnswerHash = this.questionTwoAnswer;
                userData.updateSecurityQuestionAnswers(this.userSecurityQuestionAnswerModel.userSecurityQuestions).then(response => {
                    this.updated = true;
                });
            },
            returnError(ctrl: string) {
                var errorMessage = '';
                switch (ctrl) {
                    case 'selectedQuestionOneId':
                        if (this.$v.selectedQuestionOneId.$invalid) {
                            if (!this.$v.selectedQuestionOneId.minValue) {
                                errorMessage = "Please Select your question."
                            }
                            else if (!this.$v.selectedQuestionOneId.selectedQuestionsMatch) {
                                errorMessage = "Security questions selected must be diferent"
                            }
                            console.log(this.$v.selectedQuestionOneId.selectedQuestionsMatch);
                        }
                        break;
                    case 'questionOneAnswer':
                        if (this.$v.questionOneAnswer.$invalid) {
                            if (!this.$v.questionOneAnswer.required) {
                                errorMessage = "Please provide your answer."
                            } else if (!this.$v.questionOneAnswer.maxLength) {
                                errorMessage = "answer must be less than 100 characters."
                            }
                        }
                        break;
                    case 'selectedQuestionTwoId':
                        if (this.$v.selectedQuestionTwoId.$invalid) {
                            if (!this.$v.selectedQuestionTwoId.minValue) {
                                errorMessage = "Please Select your question."
                            } else if (!this.$v.selectedQuestionTwoId.selectedQuestionsMatch) {
                                errorMessage = "Security questions selected must be diferent"
                            }
                        }
                        break;
                    case 'questionTwoAnswer':
                        if (this.$v.questionTwoAnswer.$invalid) {
                            if (!this.$v.questionTwoAnswer.required) {
                                errorMessage = "Please provide your answer."
                            } else if (!this.$v.questionTwoAnswer.maxLength) {
                                errorMessage = "answer must be less than 100 characters."
                            }
                        }
                        break;
                }
                return errorMessage;
            },
        },
        validations: {
            selectedQuestionOneId: {
                minValue: minValue(1),
                selectedQuestionsMatch: not(sameAs('selectedQuestionTwoId')),
            },
            questionOneAnswer: {
                required,
                maxLength: maxLength(100)
            },
            selectedQuestionTwoId: {
                minValue: minValue(1),
                selectedQuestionsMatch: not(sameAs('selectedQuestionOneId')),
            },
            questionTwoAnswer: {
                required,
                maxLength: maxLength(100)
            }
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