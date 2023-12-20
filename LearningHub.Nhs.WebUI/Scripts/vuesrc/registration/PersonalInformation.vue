<template>

    <div class="lh-padding-fluid">
        <div class="lh-container-xl">
            <div class="user-entry mx-0">
                <form @submit.prevent="onSubmit">
                    <h1 class="nhsuk-heading-l">Personal details</h1>

                    <div class="row">
                        <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.firstName.$error}">
                            <label for="firstName">First name</label>
                            <div class="error-text" v-if="$v.firstName.$invalid && $v.firstName.$dirty">
                                <span class="text-danger">{{returnError('firstName')}}</span>
                            </div>
                            <input type="text" class="form-control" id="firstName" aria-describedby="firstName" autocomplete="off"
                                   v-model.trim="firstName"
                                   @blur="$v.firstName.$touch()"
                                   v-bind:class="{ 'input-validation-error': $v.firstName.$error}">
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.lastName.$error}">
                            <label for="lastName">Last name</label>
                            <div class="error-text" v-if="$v.lastName.$invalid && $v.lastName.$dirty">
                                <span class="text-danger">{{returnError('lastName')}}</span>
                            </div>
                            <input type="text" class="form-control" id="lastName" aria-describedby="lastName" autocomplete="off"
                                   v-model.trim="lastName"
                                   @blur="$v.lastName.$touch()"
                                   v-bind:class="{ 'input-validation-error': $v.lastName.$error}">
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.selectedCountryId.$error}">
                            <label class="mb-2" for="country">Country you live in</label>
                            <div class="mb-2">
                                For example, England
                            </div>
                            <div class="error-text" v-if="$v.selectedCountryId.$invalid && $v.selectedCountryId.$dirty">
                                <span class="text-danger">{{returnError('selectedCountryId')}}</span>
                            </div>
                            <typeahead v-model="countrySearch"
                                       v-bind:data="countries"
                                       v-bind:internalFilter="true"
                                       label-text="country"
                                       placeholder-text="Type a country..."
                                       v-bind:loading="countriesLoading"
                                       @hit="selectCountry($event)"
                                       @input="countryValueChanged"
                                       :inputClass="errorClass($v.selectedCountryId.$error)">
                            </typeahead>
                        </div>
                    </div>
                    <div class="row" v-if="regions.length>0">
                        <div class="form-group col-md-6">
                            <div class="child-form-group" v-bind:class="{ 'input-validation-error': $v.selectedRegionId.$error}">
                                <label for="region">Region</label>
                                <div class="error-text" v-if="$v.selectedRegionId.$invalid && $v.selectedRegionId.$dirty">
                                    <span class="text-danger">{{returnError('selectedRegionId')}}</span>
                                </div>
                                <select class="form-control" id="region" aria-describedby="region" v-model="selectedRegionId" placeholder="Select a region..." v-bind:class="{ 'input-validation-error': $v.selectedRegionId.$error}">
                                    <option v-for="region in regions" v-bind:value="region.id">{{ region.name }}</option>
                                </select>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.secondaryEmailAddress.$error}">
                            <label for="secondaryEmailAddress">Secondary email address <span style="font-weight:normal;">(optional)</span></label>
                            <div class="error-text" v-if="$v.secondaryEmailAddress.$invalid && $v.secondaryEmailAddress.$dirty">
                                <span class="text-danger">{{returnError('secondaryEmailAddress')}}</span>
                            </div>
                            <input type="text" class="form-control" id="secondaryEmailAddress" aria-describedby="secondaryEmailAddress" autocomplete="off"
                                   v-model.trim="secondaryEmailAddress"
                                   @blur="$v.secondaryEmailAddress.$touch()"
                                   v-bind:class="{ 'input-validation-error': $v.secondaryEmailAddress.$error}">
                        </div>
                    </div>

                    <div class="row">
                        <div class="accordion col-md-12" id="accordion">
                            <div class="pt-0 pb-5">
                                <div class="heading" id="headingTwo">
                                    <div class="mb-0">
                                        <a href="#" class="collapsed" data-toggle="collapse" data-target="#collapseTwo" aria-expanded="false" aria-controls="collapseTwo">
                                            <div class="accordion-arrow">Why should I provide a secondary email address?</div>
                                        </a>
                                    </div>
                                </div>
                                <div id="collapseTwo" class="collapse" aria-labelledby="headingTwo" data-parent="#accordion">
                                    <div class="content">
                                        If your work email address changes, our support team can contact you using your secondary email address. This can be a personal email address.
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="mt-2">
                        <router-link to="/create-an-account" tag="button" class="btn btn-outline-custom mr-5 my-2">
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
    import Typeahead from  '../typeahead.vue';
    import { required, email, maxLength, minValue, requiredIf } from "vuelidate/lib/validators";
    import { regionData } from '../data/region'
    import { countryData } from '../data/country'
    import { GenericListItemModel } from '../models/genericListItemModel'

    export default Vue.extend({
        data() {
            return {
                firstName: '',
                lastName: '',
                selectedCountryId: 0,
                selectedCountryName: '',
                selectedRegionId: null,
                preferredName: '',
                secondaryEmailAddress: '',
                countries: [],
                countrySearch: '',
                lastCountrySearch: '',
                countriesLoading: false,
                regions: []
            }
        },
        components: {
            Typeahead
        },
        async created() {
            if (!this.$store.state.userObj.emailAddress) {
                this.$router.push({ name: 'CreateAccount' });
            }
            this.firstName = this.$store.state.userObj.firstName;
            this.lastName = this.$store.state.userObj.lastName;
            this.selectedCountryId = this.$store.state.userObj.countryId;
            this.selectedCountryName = this.$store.state.userObj.countryName;
            this.countrySearch = this.selectedCountryName;
            if (this.selectedCountryId == 1) {
                await this.loadCountries(this.countrySearch);
                await this.loadRegions();
            }

            this.selectedRegionId = this.$store.state.userObj.regionId;
            this.preferredName = this.$store.state.userObj.preferredName;
            this.secondaryEmailAddress = this.$store.state.userObj.secondaryEmailAddress;
        },
        methods: {
            async loadRegions() {
                this.regions = await regionData.getAllRegions();
                if (this.regions.length > 1) {
                    this.regions.unshift({ id:null, name: 'Please choose' });
                    this.selectedRegionId = null;
                }
                if (this.regions.length == 1) {
                    this.selectedRegionId = this.regions[0].id;
                }
            },
            async loadCountries(filter: string) {
                this.countriesLoading = true;
                this.countries = await countryData.getFilteredCountries(filter);
                this.countriesLoading = false;
            },
            countryValueChanged(newValue: string) {
                if (newValue.length < 2) {
                    this.selectedCountryId = 0;
                    this.countries = [];
                    this.lastCountrySearch = '';
                    this.selectedRegionId = null;
                    this.regions = [];
                } else {
                    if (!this.lastCountrySearch || newValue.indexOf(this.lastCountrySearch) != 0) {
                        this.loadCountries(newValue);
                        this.lastCountrySearch = newValue;
                    }
                }
            },
            selectCountry(country: GenericListItemModel) {
                if (country) {
                    this.selectedCountryId = country.id;
                    this.selectedCountryName = country.name;
                } else {
                    this.selectedCountryId = 0
                    this.selectedCountryName = '';
                }
                if (this.selectedCountryId == 1) {
                    this.loadRegions();
                    this.selectedRegionId = null;
                } else {
                    this.regions = [];
                    this.selectedRegionId = null;
                }
            },
            returnError(ctrl: string) {
                var errorMessage = '';
                switch (ctrl) {
                    case 'firstName':
                        if (this.$v.firstName.$invalid) {
                            if (!this.$v.firstName.required) {
                                errorMessage = "Enter a first name."
                            } else if (!this.$v.firstName.maxLength) {
                                errorMessage = "First name must be less than 100 characters."
                            }
                        }
                        break;
                    case 'lastName':
                        if (this.$v.lastName.$invalid) {
                            if (!this.$v.lastName.required) {
                                errorMessage = "Enter a last name."
                            } else if (!this.$v.lastName.maxLength) {
                                errorMessage = "Last name must be less than 100 characters."
                            }
                        }
                        break;
                    case 'selectedCountryId':
                        if (this.$v.selectedCountryId.$invalid) {
                            if (!this.$v.selectedCountryId.minValue) {
                                errorMessage = "Select a country."
                            }
                        }
                        break;
                    case 'selectedRegionId':
                        if (this.$v.selectedRegionId.$invalid) {
                            if (!this.$v.selectedRegionId.requiredIfEngland) {
                                errorMessage = "Select a region."
                            }
                        }
                        break;
                    case 'preferredName':
                        if (this.$v.preferredName.$invalid) {
                            if (!this.$v.preferredName.maxLength) {
                                errorMessage = "Preferred name must be less than 100 characters."
                            }
                        }
                        break;
                    case 'secondaryEmailAddress':
                        if (this.$v.secondaryEmailAddress.$invalid) {
                            if (!this.$v.secondaryEmailAddress.email) {
                                errorMessage = "Enter a valid email address."
                            } else if (!this.$v.secondaryEmailAddress.maxLength) {
                                errorMessage = "Secondary email address must be less than 100 characters."
                            }
                        }
                        break;
                }
                return errorMessage;
            },
            onSubmit() {
                if (this.$v.$invalid) {
                    this.$v.firstName.$touch();
                    this.$v.lastName.$touch();
                    this.$v.selectedCountryId.$touch();
                    this.$v.selectedRegionId.$touch();
                    //this.$v.preferredName.$touch();
                    this.$v.secondaryEmailAddress.$touch();
                } else {
                    let selectedRegionName = '';
                    if (this.selectedRegionId) {
                        selectedRegionName = this.regions.find((r: GenericListItemModel) => r.id === this.selectedRegionId).name;
                    }
                    this.$store.commit('setPersonalInfo', {
                        firstName: this.firstName,
                        lastName: this.lastName,
                        countryId: this.selectedCountryId,
                        countryName: this.selectedCountryName,
                        regionId: this.selectedRegionId,
                        regionName: selectedRegionName,
                        preferredName: this.preferredName,
                        secondaryEmailAddress: this.secondaryEmailAddress,
                    });
                    this.$router.push({ name: 'CurrentJobRole' });
                }
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
            firstName: {
                required,
                maxLength: maxLength(100)
            },
            lastName: {
                required,
                maxLength: maxLength(100)
            },
            selectedCountryId: {
                minValue: minValue(1)
            },
            selectedRegionId: {
                requiredIfEngland: requiredIf((vueInstance) => { 
                    return vueInstance.selectedCountryId == 1
                })
            },
            //preferredName: {
            //    maxLength: maxLength(100)
            //},
            secondaryEmailAddress: {
                email,
                maxLength: maxLength(100)
            }
        }
    })

</script>
