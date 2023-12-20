<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl">
            <div class="user-entry mx-0">
                <form @submit.prevent="onSubmit">
                    <h1 class="nhsuk-heading-l">Current role</h1>
                    <div class="row">
                        <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.selectedJobRoleId.$error}">
                            <label id="currentJobRoleLb" for="currentJobRole">Current role</label>
                            <div class="error-text" v-if="$v.selectedJobRoleId.$invalid && $v.selectedJobRoleId.$dirty">
                                <span class="text-danger">{{returnError('selectedJobRoleId')}}</span>
                            </div>
                            <typeahead v-model="jobRoleSearch"
                                       v-bind:data="jobRoles"
                                       display-property="nameWithStaffGroup"
                                       v-bind:internalFilter="true"
                                       placeholder-text="Type 2 or more characters to search..."
                                       label-text="currentJobRole"
                                       v-bind:loading="jobRolesLoading"
                                       @hit="selectJobRole($event)"
                                       @input="jobRoleValueChanged"
                                       :inputClass="errorClass($v.selectedJobRoleId.$error)">
                            </typeahead>
                            <span class="text-hint">Results may take a moment to load</span>
                        </div>
                    </div>

                    <div class="row" v-if="medicalCouncilId>0">
                        <div class="form-group col-md-6 mb-0">
                            <div class="child-form-group">
                                <label id="professionalBodyLb">Professional body</label>
                                <div>{{medicalCouncilName}}</div>
                            </div>
                        </div>
                    </div>
                    <div class="row" v-if="medicalCouncilId>0">
                        <div class="form-group col-md-6 mb-0">
                            <div class="child-form-group pb-4" v-bind:class="{ 'input-validation-error': medicalCouncilError || $v.medicalCouncilNumber.$error}">
                                <label id="mcNumberLb" for="mcNumber">
                                    {{medicalCouncilCode}} number
                                </label>
                                <div class="error-text" v-if="medicalCouncilError || ($v.medicalCouncilNumber.$invalid && $v.medicalCouncilNumber.$dirty)">
                                    <span class="text-danger">{{returnError('medicalCouncilNumber')}}</span>
                                </div>
                                <input type="text" class="form-control" id="mcNumber" aria-describedby="mcNumber" aria-labelledby="mcNumberLb"
                                       autocomplete="off"
                                       @input="medicalCouncilError=''"
                                       v-model.trim="medicalCouncilNumber"
                                       v-bind:class="{ 'input-validation-error': medicalCouncilError || $v.medicalCouncilNumber.$error}">
                            </div>
                        </div>
                    </div>
                    <div class="row" v-if="grades.length>0">
                        <div class="form-group col-md-6">
                            <div class="child-form-group" v-bind:class="{ 'input-validation-error': $v.selectedGradeId.$error}">
                                <label id="gradeLb" for="grade">Grade</label>
                                <div>
                                    You can find this on your payslip.
                                </div>
                                <div class="error-text" v-if="$v.selectedGradeId.$invalid && $v.selectedGradeId.$dirty">
                                    <span class="text-danger">{{returnError('selectedGradeId')}}</span>
                                </div>
                                <select class="form-control" id="grade" aria-describedby="grade" aria-labelledby="gradeLb" v-model="selectedGradeId" placeholder="Select a grade..."
                                        v-bind:class="{ 'input-validation-error': $v.selectedGradeId.$error}">
                                    <option v-for="grade in grades" v-bind:value="grade.id">{{grade.name}}</option>
                                </select>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.selectedSpecialtyId.$error}">
                            <label id="primarySpecialtyLb" for="PrimarySpecialty">Primary specialty</label>
                            <div class="error-text" v-if="$v.selectedSpecialtyId.$invalid && $v.selectedSpecialtyId.$dirty">
                                <span class="text-danger">{{returnError('selectedSpecialtyId')}}</span>
                            </div>
                            <select class="form-control" id="PrimarySpecialty" aria-describedby="PrimarySpecialty" aria-labelledby="primarySpecialtyLb" v-model="selectedSpecialtyId"
                                    v-bind:class="{ 'input-validation-error': $v.selectedSpecialtyId.$error}">
                                <option v-for="specialty in specialties" v-bind:value="specialty.id">{{specialty.name}}</option>
                            </select>
                        </div>
                    </div>

                    <div class="row">
                        <div class="accordion col-md-12" id="accordion">
                            <div class="pt-0 pb-5">
                                <div class="heading" id="headingOne">
                                    <div class="mb-0">
                                        <a href="#" class="collapsed" data-toggle="collapse" data-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                                            <div class="accordion-arrow">Why do I need to provide details about my role?</div>
                                        </a>
                                    </div>
                                </div>
                                <div id="collapseOne" class="collapse" aria-labelledby="headingOne" data-parent="#accordion">
                                    <div class="content">
                                        In the future, resources will be recommended to you based on your role.
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="mt-2">
                        <router-link to="/personal-information" tag="button" class="btn btn-outline-custom mr-5 my-2" v-on:click.native="recordState">
                            Previous
                        </router-link>

                        <button type="submit" class="btn btn-custom my-2">Next</button>
                    </div>
                </form>
            </div>
        </div>
    </div>

</template>

<script lang="ts">
    import Vue from 'vue';
    import Typeahead from '../typeahead.vue';
    import { required, minValue, requiredIf } from "vuelidate/lib/validators";
    import { jobRoleData } from '../data/jobRole'
    import { gradeData } from '../data/grade'
    import { specialtyData } from '../data/specialty'
    import { JobRoleBasicModel } from '../models/jobRoleBasicModel';
    import { GenericListItemModel } from '../models/genericListItemModel';

    export default Vue.extend({
        data() {
            return {
                selectedJobRoleId: 0,
                selectedJobRoleName: '',
                jobRoles: [],
                jobRoleSearch: '',
                jobRolesLoading: false,
                lastJobRoleSearch: '',
                selectedGradeId: 0,
                selectedGradeName: '',
                grades: [],
                medicalCouncilId: 0,
                medicalCouncilName: '',
                medicalCouncilCode: '',
                medicalCouncilNumber: '',
                medicalCouncilError: '',
                selectedSpecialtyId: 0,
                specialties: []
            }
        },
        components: {
            Typeahead
        },
        async created() {
            if (!this.$store.state.userObj.firstName) {
                this.$router.push({ name: 'PersonalInformation' });
            }
            this.selectedJobRoleId = this.$store.state.userObj.jobRoleId;
            this.selectedJobRoleName = this.$store.state.userObj.jobRoleName;
            this.jobRoleSearch = this.selectedJobRoleName;
            this.loadSpecialties();
            if (this.selectedJobRoleId) {
                await this.loadGrades(this.selectedJobRoleId);
            }
            if (this.$store.state.userObj.gradeId > 0) {
                this.selectedGradeId = this.$store.state.userObj.gradeId;
            }
            this.selectedGradeId = this.$store.state.userObj.gradeId;
            this.selectedGradeName = this.$store.state.userObj.gradeName;
            this.medicalCouncilId = this.$store.state.userObj.medicalCouncilId;
            this.medicalCouncilName = this.$store.state.userObj.medicalCouncilName;
            this.medicalCouncilCode = this.$store.state.userObj.medicalCouncilCode;
            this.medicalCouncilNumber = this.$store.state.userObj.medicalCouncilNumber;
            this.selectedSpecialtyId = this.$store.state.userObj.specialtyId;
        },
        methods: {
            async loadJobRoles(filter: string) {
                this.$data.jobRolesLoading = true;
                this.jobRoles = await jobRoleData.getFilteredJobRoles(filter);
                this.jobRolesLoading = false;
            },
            jobRoleValueChanged(newValue: string) {
                if (newValue.length < 2) {
                    this.medicalCouncilId = 0;
                    this.medicalCouncilCode = '';
                    this.medicalCouncilNumber = '';
                    this.medicalCouncilError = '';
                    this.selectedJobRoleId = 0;
                    this.selectedJobRoleName = '';
                    this.jobRoles = [];
                    this.lastJobRoleSearch = '';
                    this.grades = [];
                } else {
                    if (!this.lastJobRoleSearch || newValue.indexOf(this.lastJobRoleSearch) != 0) {
                        this.loadJobRoles(newValue);
                        this.lastJobRoleSearch = newValue;
                    }
                }
            },
            selectJobRole(jobRole: JobRoleBasicModel) {
                if (jobRole) {
                    this.selectedJobRoleId = jobRole.id;
                    this.selectedJobRoleName = jobRole.name;
                    this.medicalCouncilId = jobRole.medicalCouncilId;
                    this.medicalCouncilName = jobRole.medicalCouncilName;
                    this.medicalCouncilCode = jobRole.medicalCouncilCode;
                    this.loadGrades(this.selectedJobRoleId);
                } else {
                    this.selectedJobRoleId = 0
                    this.selectedJobRoleName = '';
                    this.medicalCouncilId = 0;
                    this.medicalCouncilName = '';
                    this.medicalCouncilCode = '';
                    this.grades = [];
                }
            },
            async loadGrades(jobRoleId: number) {
                this.grades = await gradeData.getGradesForJobRole(jobRoleId);
                if (this.grades.length > 1) {
                    this.grades.unshift({ id: '', name: 'Please choose' });
                    this.selectedGradeId = this.grades[0].id;
                }
                if (this.grades.length == 1) {
                    this.selectedGradeId = this.grades[0].id;
                }
            },
            async loadSpecialties() {
                await specialtyData.getSpecialties()
                    .then(response => {
                        this.specialties = response;
                        if (this.specialties.length > 1) {
                            this.specialties.unshift({ id: '', name: 'Please choose' });
                            this.selectedSpecialtyId = this.specialties[0].id;
                        }
                        if (this.specialties.length == 1) {
                            this.selectedSpecialtyId = this.specialties[0].id;
                        }
                    })
                    .catch(e => {
                        console.log(e);
                    });
            },
            returnError(ctrl: string) {
                var errorMessage = '';
                switch (ctrl) {
                    case 'selectedJobRoleId':
                        if (this.$v.selectedJobRoleId.$invalid) {
                            if (!this.$v.selectedJobRoleId.minValue) {
                                errorMessage = "Select a job role.";
                            }
                        }
                        break;
                    case 'medicalCouncilNumber':
                        if (this.$v.medicalCouncilNumber.$invalid) {
                            if (!this.$v.medicalCouncilNumber.required) {
                                errorMessage = "Enter a " + this.medicalCouncilCode + " number.";
                            }
                        }
                        if (this.medicalCouncilError != '') {
                            errorMessage = this.medicalCouncilError;
                        }
                        break;
                    case 'selectedGradeId':
                        if (this.$v.selectedGradeId.$invalid) {
                            if (!this.$v.selectedGradeId.required) {
                                errorMessage = "Select a grade.";
                            }
                        }
                        break;
                    case 'selectedSpecialtyId':
                        if (this.$v.selectedSpecialtyId.$invalid) {
                            if (!this.$v.selectedSpecialtyId.required) {
                                errorMessage = "Select a specialty.";
                            }
                        }
                        break;
                }
                return errorMessage;
            },
            async onSubmit() {
                if (this.$v.$invalid) {
                    this.$v.selectedJobRoleId.$touch();
                    this.$v.medicalCouncilNumber.$touch();
                    this.$v.selectedGradeId.$touch();
                    this.$v.selectedSpecialtyId.$touch();
                } else {
                    if (this.medicalCouncilId > 0 && this.medicalCouncilId < 4) {
                        this.medicalCouncilError = await jobRoleData.validateMedicalCouncilNumber(
                            this.$store.state.userObj.lastName,
                            this.medicalCouncilId,
                            this.medicalCouncilNumber
                        );
                    }
                    if (!this.medicalCouncilError) {
                        let selectedGradeName = '';
                        if (this.selectedGradeId) {
                            selectedGradeName = this.grades.find((g: GenericListItemModel) => g.id === this.selectedGradeId).name;
                        }
                        let selectedSpecialtyName = '';
                        if (this.selectedSpecialtyId) {
                            selectedSpecialtyName = this.specialties.find((s: GenericListItemModel) => s.id === this.selectedSpecialtyId).name;
                        }
                        this.$store.commit('setJobRoleInfo', {
                            jobRoleId: this.selectedJobRoleId,
                            jobRoleName: this.selectedJobRoleName,
                            gradeId: this.selectedGradeId,
                            gradeName: selectedGradeName,
                            medicalCouncilId: this.medicalCouncilId,
                            medicalCouncilName: this.medicalCouncilName,
                            medicalCouncilCode: this.medicalCouncilCode,
                            medicalCouncilNumber: this.medicalCouncilNumber,
                            specialtyId: this.selectedSpecialtyId,
                            specialtyName: selectedSpecialtyName
                        });
                        this.$router.push({ name: 'PlaceOfWork' });
                    }
                }
            },
            recordState() {
                this.$store.commit('setJobRoleInfo', {
                    jobRoleId: this.selectedJobRoleId,
                    jobRoleName: this.selectedJobRoleName,
                    gradeId: this.selectedGradeId,
                    medicalCouncilId: this.medicalCouncilId,
                    medicalCouncilName: this.medicalCouncilName,
                    medicalCouncilCode: this.medicalCouncilCode,
                    medicalCouncilNumber: this.medicalCouncilNumber,
                    specialtyId: this.selectedSpecialtyId,
                });
            },
            errorClass(isError: boolean) {
                if (isError) {
                    return 'input-validation-error';
                } else {
                    return '';
                }
            }
        },
        validations: {
            selectedJobRoleId: {
                minValue: minValue(1)
            },
            medicalCouncilNumber: {
                required: requiredIf((vueInstance) => {
                    return vueInstance.medicalCouncilId > 0 && vueInstance.medicalCouncilId < 4;
                })
            },
            selectedGradeId: {
                required: requiredIf((vueInstance) => {
                    return vueInstance.grades.length > 0;
                })
            },
            selectedSpecialtyId: {
                required
            }
        }

    })

</script>
