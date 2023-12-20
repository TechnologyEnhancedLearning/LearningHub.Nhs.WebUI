<template>
    <div class="tab-panel active">
        <form @submit.prevent="onSubmit">
            <div class="tab-panel-content">
                <div class="lh-container-xl">
                    <div class="user-entry mx-0" v-if="userName">
                        <div class="row">
                            <div class="form-group col-md-6">
                                <label for="userName">Username</label>
                                <input type="text" class="form-control" aria-describedby="userName" disabled
                                        v-model.trim="userName">
                            </div>
                        </div>
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
                            <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.preferredName.$error}">
                                <label for="preferredName">Preferred name</label>
                                <div class="error-text" v-if="$v.preferredName.$invalid && $v.preferredName.$dirty">
                                    <span class="text-danger">{{returnError('preferredName')}}</span>
                                </div>
                                <input type="text" class="form-control" id="preferredName" aria-describedby="preferredName" autocomplete="off"
                                        v-model.trim="preferredName"
                                        @blur="$v.preferredName.$touch()"
                                        v-bind:class="{ 'input-validation-error': $v.preferredName.$error}">
                            </div>
                        </div>
                        <div class="row">
                            <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.selectedCountryId.$error}">
                                <label class="mb-6" for="country">Country</label>
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
                            <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.primaryEmailAddress.$error}">
                                <label for="primaryEmailAddress">Primary email address</label>
                                <div class="error-text" v-if="$v.primaryEmailAddress.$invalid && $v.primaryEmailAddress.$dirty">
                                    <span class="text-danger">{{returnError('primaryEmailAddress')}}</span>
                                </div>
                                <input type="text" class="form-control" id="primaryEmailAddress" aria-describedby="primaryEmailAddress" autocomplete="off"
                                        v-model.trim="primaryEmailAddress"
                                        @blur="$v.primaryEmailAddress.$touch()"
                                        v-bind:class="{ 'input-validation-error': $v.primaryEmailAddress.$error}">
                            </div>
                        </div>
                        <div class="row">
                            <div class="form-group col-md-6" v-bind:class="{ 'input-validation-error': $v.secondaryEmailAddress.$error}">
                                <label for="secondaryEmailAddress">Secondary email address</label>
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
                    </div>
                </div>
            </div>
            <div class="white-background footerButtons lh-container-xl ">
                <div class="col-md-6 success-container">
                    <div class="save-success" v-if="editState">
                        <i class="fa-solid fa-check"></i><p class="ml-3">Your changes have been saved</p>
                    </div>
                </div>
                <button type="submit" @click.prevent="onSubmit" :disabled="$v.$anyError" class="btn btn-green mr-25">Save changes</button>
                <!--<button type="reset" class="btn btn-outline-custom btn-cancel">Cancel</button>-->
            </div>
        </form>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Typeahead from '../typeahead.vue';

    import { regionData } from '../data/region'
    import { countryData } from '../data/country'
    import { GenericListItemModel } from '../models/genericListItemModel'
    import Vuelidate from "vuelidate";
    import { required, email, maxLength, minValue, requiredIf } from "vuelidate/lib/validators";
    Vue.use(Vuelidate as any);
    import { UserPersonalDetailsModel } from '../models/userBasicModel';
    import { userData } from '../data/user';
    import { mapActions, mapGetters } from 'vuex';
    export default Vue.extend({
        data() {
            return {
                userId: 0,
                userName: '',
                firstName: '',
                lastName: '',
                preferredName: '',
                primaryEmailAddress: '',
                secondaryEmailAddress: '',
                selectedCountryId: 0,
                selectedCountryName: '',
                selectedRegionId: null,
                countries: [],
                countrySearch: '',
                lastCountrySearch: '',
                countriesLoading: false,
                regions: [],
                editState: 0,
            }
        },
        components: {
            Typeahead
        },
        async created() {
            await this.loadUserPersonalDetails();
        },
        methods: {
            async loadUserPersonalDetails() {
                await userData.getCurrentUserPersonalDetails().then(response => {
                    this.userId = response.userId;
                    this.userName = response.userName;
                    this.firstName = response.firstName;
                    this.lastName = response.lastName;
                    this.preferredName = response.preferredName;
                    this.primaryEmailAddress = response.primaryEmailAddress;
                    this.secondaryEmailAddress = response.secondaryEmailAddress;
                    this.selectedCountryId = response.countryId;
                    let regionId = response.regionId;
                    return countryData.getCountry(this.selectedCountryId).then(response => {
                        this.countrySearch = response.name;
                        this.loadCountries(this.countrySearch).then(response => {
                            if (this.selectedCountryId !== 1) {
                                // only england has regions
                                return;
                            }
                            this.loadRegions().then(response => {
                                this.selectedRegionId = regionId;
                            });
                        });
                    });
                });
            },
            async loadRegions() {
                this.regions = await regionData.getAllRegions();
                if (this.regions.length > 1) {
                    this.regions.unshift({ id: null, name: 'Please choose' });
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
                                errorMessage = "Preferred name must be less than 50 characters."
                            }
                        }
                        break;
                    case 'primaryEmailAddress':
                        if (this.$v.primaryEmailAddress.$invalid) {
                            if (!this.$v.primaryEmailAddress || !this.$v.primaryEmailAddress.email) {
                                errorMessage = "Enter a valid email address.";
                                break;
                            } else if (!this.$v.primaryEmailAddress.maxLength) {
                                errorMessage = "Primary email address must be less than 100 characters."
                                break;
                            }
                        }
                    case 'secondaryEmailAddress':
                        if (this.$v.secondaryEmailAddress.$invalid) {
                            if (!this.$v.secondaryEmailAddress || !this.$v.secondaryEmailAddress.email) {
                                errorMessage = "Enter a valid email address."
                                break;
                            } else if (!this.$v.secondaryEmailAddress.maxLength) {
                                errorMessage = "Secondary email address must be less than 100 characters."
                                break;
                            }
                        }
                }
                return errorMessage;
            },
            onSubmit() {
                if (this.$v.$invalid) {
                    this.$v.firstName.$touch();
                    this.$v.lastName.$touch();
                    this.$v.selectedCountryId.$touch();
                    this.$v.selectedRegionId.$touch();
                    this.$v.primaryEmailAddress.$touch();
                    return;
                }
                let userPersonalDetails = new UserPersonalDetailsModel({
                    userId: this.userId,
                    firstName: this.firstName,
                    lastName: this.lastName,
                    preferredName: this.preferredName,
                    primaryEmailAddress: this.primaryEmailAddress,
                    secondaryEmailAddress: this.secondaryEmailAddress,
                    countryId: this.selectedCountryId,
                    regionId: this.selectedRegionId,
                });
                userData.updatePersonalDetails(userPersonalDetails).then(response => {
                    this.$store.commit('setUser', {
                        id: userPersonalDetails.userId,
                        userName: userPersonalDetails.userName,
                        firstName: userPersonalDetails.firstName,
                        lastName: userPersonalDetails.lastName,
                        lastUpdated: new Date(),
                    });
                    this.editState = this.editState + 1;
                    this.$router.push(`personaldetails?${this.editState}`).catch(e => { });
                });
            },
            errorClass(isError: boolean) {
                if (isError) {
                    return 'input-validation-error';
                } else {
                    return '';
                }
            },
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
            preferredName: {
                maxLength: maxLength(50)
            },
            primaryEmailAddress: {
                required,
                email,
                maxLength: maxLength(100)
            },
            secondaryEmailAddress: {
                email,
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

    .tab-panel-content {
        padding-top: 40px;
        background-color: #F0F4F5;
        padding-left: 4rem;
        padding-right: 4rem;
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
    @media(max-width: 1200px){
        .footerButtons{
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