<template>

    <div class="lh-padding-fluid">
        <div class="lh-container-xl">
            <div class="user-entry mx-0">
                <form @submit.prevent="onSubmit">
                    <h1 class="nhsuk-heading-l">Where you work</h1>

                    <div class="row">
                        <div class="form-group col-12 mb-0" v-bind:class="{ 'input-validation-error': $v.startDate.$error}">
                            <label id="startDateLb" for="startDate" class="control-label">Start date</label>
                            <div>Enter the date you started your current role, for example, 27/08/2019</div>
                        </div>
                        <div class="form-group col-md-3 col-lg-6" v-bind:class="{ 'input-validation-error': $v.startDate.$error}">
                            <div class="error-text" v-if="$v.startDate.$invalid && $v.startDate.$dirty">
                                <span class="text-danger">{{returnError('startDate')}}</span>
                            </div>
                            <lh-date-picker label-text="startDateLb" display-style="1" v-model="startDate"
                                            unique-name="startDate" @dateValid="setDateValid($event)" v-bind:class="{ 'input-validation-error': $v.startDate.$error}"></lh-date-picker>
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-12 mb-0" v-bind:class="{ 'input-validation-error': $v.selectedLocationId.$error}">
                            <label id="placeOfWorkLb" for="placeOfWork">Place of work</label>
                            <div>Enter the name, postcode or Organisation Data Service (ODS) code of your place of work. Select the correct location from the list.</div>
                        </div>
                        <div class="form-group col-md-12" v-bind:class="{ 'input-validation-error': $v.selectedLocationId.$error}">
                            <div class="error-text" v-if="$v.selectedLocationId.$invalid && $v.selectedLocationId.$dirty">
                                <span class="text-danger">{{returnError('selectedLocationId')}}</span>
                            </div>
                            <typeahead v-model="locationSearch"
                                       v-bind:data="locations"
                                       display-property="expandedName"
                                       selection-display-property="name"
                                       v-bind:internalFilter="internalLocationFilter"
                                       placeholder-text="Type 2 or more characters to search..."
                                       label-text="placeOfWork"
                                       v-bind:loading="locationsLoading"
                                       @hit="selectLocation($event)"
                                       @input="locationValueChanged"
                                       :inputClass="errorClass($v.selectedLocationId.$error)">
                            </typeahead>
                            <span class="text-hint mt-0">Results may take a moment to load</span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="accordion col-md-12" id="accordion">
                            <div class="pt-0 pb-5">
                                <div class="heading" id="headingOne">
                                    <div class="mb-0">
                                        <a href="#" class="collapsed" data-toggle="collapse" data-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                                            <div class="accordion-arrow">Why do I need to provide details about where I work?</div>
                                        </a>
                                    </div>
                                </div>
                                <div id="collapseOne" class="collapse" aria-labelledby="headingOne" data-parent="#accordion">
                                    <div class="content">
                                        Information on your place of work will help us provide activity reports to evidence your learning.
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row" v-if="generalErrorMessage!=''">
                        <div class="col-md-12 error-text">
                            <span class="text-danger">{{generalErrorMessage}}</span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12 mt-2">
                            <router-link to="/current-job-role" tag="button" class="btn btn-outline-custom mr-5 my-2" v-on:click.native="recordState">
                                Previous
                            </router-link>

                            <button type="submit" class="btn btn-custom my-2" :disabled=formSubmitting v-bind:class="{ 'button-processing': formSubmitting }">
                                {{submitCaption}}
                                <i class="fa fa-spinner fa-spin" v-if="formSubmitting"></i>
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>

</template>

<script lang="ts">
    import Vue from 'vue';
    import Typeahead from  '../typeahead.vue';
    import LhDatePicker from '../datepicker.vue';
    import { minValue, required } from "vuelidate/lib/validators";
    import { locationData } from '../data/location';
    import { LocationModel } from '../models/locationModel';

    const isValidStartDate = (value: any, vm: any) => {
        if (value !== undefined && vm.dateValid !== undefined) {
            return vm.dateValid;
        }
        return true;
    };
    export default Vue.extend({
        data() {
            return {
                startDate: '' as string,
                dateValid: false,
                selectedLocationId: 0,
                selectedLocationName: '',
                locations: [],
                locationSearch: '',
                lastLocationSearch: '',
                locationsLoading: false,
                internalLocationFilter: false,
                generalErrorMessage: '',
                formSubmitting: false
            }
        },
        components: {
            LhDatePicker,
            Typeahead
        },
        async created() {
            if (this.$store.state.userObj.specialtyId==0) {
                this.$router.push({ name: 'CurrentJobRole' });
            }
            this.startDate = this.$store.state.userObj.locationStartDate;
            this.selectedLocationId = this.$store.state.userObj.locationId;
            this.selectedLocationName = this.$store.state.userObj.locationName;
            this.locationSearch = this.selectedLocationName;
        },
        computed: {
            submitCaption: function submitCaption() {
                if (this.formSubmitting) {
                    return 'Processing...';
                } else {
                    return 'Complete';
                }
            }
        },
        methods: {
            async loadLocations(criteria: string) {
                this.locationsLoading = true;
                //this.locations = await locationData.getFilteredLocations(criteria);
                await locationData.getFilteredLocations(criteria)
                    .then(response => {
                        this.locations = response.map(location => ({
                            id: location.id,
                            name: location.name,
                            expandedName: location.name + '<br />Address: ' + location.address //+ '<br />Org Code: ' + location.nhsCode
                        }));
                    });

                this.locationsLoading = false;
            },
            async locationValueChanged(newValue: string) {
                if (newValue.length < 2) {
                    this.locations = [];
                    this.lastLocationSearch = '';
                    this.selectedLocationId = 0;
                    this.selectedLocationName = '';
                } else {
                    //if (this.lastLocationSearch != newValue && newValue.length < 20) {
                    //    await this.loadLocations(newValue);
                    //    this.lastLocationSearch = newValue;
                    //}
                    if (!this.lastLocationSearch || newValue.indexOf(this.lastLocationSearch) != 0 || this.locations.length > 149 ) {
                        await this.loadLocations(newValue);
                        this.lastLocationSearch = newValue;
                    }
                    this.internalLocationFilter = this.locations.length < 149; // if less then 150 then all matching locations have been returned from the database, allow internal filtering
                }
            },
            selectLocation(location: LocationModel) {
                if (location) {
                    this.selectedLocationId = location.id;
                    this.selectedLocationName = location.expandedName;
                } else {
                    this.selectedLocationId = 0;
                    this.selectedLocationName = '';
                }
            },
            setDateValid(dateValid: boolean) {
                this.dateValid = dateValid;
            },
            returnError(ctrl: string) {
                let errorMessage = '';
                switch (ctrl) {
                    case 'startDate':
                        if (this.$v.startDate.$invalid) {
                            if (!this.$v.startDate.required) {
                                errorMessage = "Enter a valid start date.";
                            }
                            if (!this.$v.startDate.$isValidStartDate) {
                                errorMessage = "Enter a valid start date.";
                            }
                        }                       
                        break;
                    case 'selectedLocationId':
                        if (this.$v.selectedLocationId.$invalid) {
                            if (!this.$v.selectedLocationId.minValue) {
                                errorMessage = "Select a place of work.";
                            }
                        }
                        break;
                }
                return errorMessage;
            },
            async onSubmit() {
                this.generalErrorMessage = '';
                if (this.$v.$invalid) {
                    this.$v.startDate.$touch();
                    this.$v.selectedLocationId.$touch();
                } else {
                    this.$store.commit('setPlaceOfWorkInfo', {
                        startDate: this.startDate,
                        locationId: this.selectedLocationId,
                        locationName: this.selectedLocationName
                    });
                    this.formSubmitting = true;
                    this.generalErrorMessage = await this.$store.dispatch('processRegistrationDetails');
                    if (this.generalErrorMessage == '') {
                        this.$router.push({ name: 'Complete' });
                    }
                    this.formSubmitting = false;
                }
            },
            recordState() {
                this.$store.commit('setPlaceOfWorkInfo', {
                    startDate: this.startDate,
                    locationId: this.selectedLocationId,
                    locationName: this.selectedLocationName
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
            selectedLocationId: {
                minValue: minValue(1)
            },
            startDate: {
                required,
                isValidStartDate
            }
        }
    })

</script>
