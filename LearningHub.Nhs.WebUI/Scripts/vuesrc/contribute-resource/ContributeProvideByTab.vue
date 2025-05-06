<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl py-15">
            <div class="contribute contribute-providedby-tab">
                <h2 id="licence-heading" class="nhsuk-heading-l pt-15">Content developed with</h2>
                <p class="nhsuk-u-font-weight-normal">When applicable pease select the provider of this content. This will enable users to search for content produced by specific organisations.</p>
                <div class="my-3">
                    <input class="radio-button" name="resourceProviderId" type="radio" :value="0" v-model="resourceProviderId" @click="setResourceProvider($event.target.value)" />
                    <span class="span-provider nhsuk-u-font-weight-normal">Not applicable</span>
                    <div v-for="provider in userProviders">
                        <label class="my-0 span-provider nhsuk-u-font-weight-normal">
                            <input class="radio-button" name="resourceProviderId"  type="radio" :value="provider.id" v-model="resourceProviderId" @click="setResourceProvider($event.target.value)" />
                            {{provider.name}}
                        </label>
                        <br />
                    </div>
                </div>

                <div class="row my-2">
                    <div class="accordion col-12" id="provided-by-info-accordion">
                        <div class="pt-0 pb-4">

                            <div class="heading" id="headingProvidedBy">
                                <div class="mb-0">
                                    <a href="#" class="collapsed text-decoration-skip" style="color:#005EB8;" data-toggle="collapse" data-target="#collapseProvidedByInfo" aria-expanded="false" aria-controls="collapseProvidedByInfo">
                                        <div class="accordion-arrow"></div>
                                        <span class="pl-3">
                                            Why should I flag a resource as 'Developed with''
                                        </span>
                                    </a>
                                </div>
                            </div>
                            <div id="collapseProvidedByInfo" class="collapse" aria-labelledby="headingProvidedBy" data-parent="#provided-by-info-accordion">
                                <div class="content col-12">
                                    <p>
                                        <b>
                                            When publishing a resource it is important to mark a resource as 'Developed with' as it helps;
                                        </b>
                                    </p>
                                    <ul>
                                        <li>Users search and filter content by specific providers.</li>
                                        <li>Separate learning resources from community contributions.</li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
    import { ProviderModel } from '../models/providerModel';

    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>,
            userProviders: { type: Array } as PropOptions<ProviderModel[]>
        },
        data() {
            return {
                resourceProviderId: null,
            }
        },
        created() {
            if (this.resourceDetails.resourceProviderId > 0) {
                this.resourceProviderId = this.resourceDetails.resourceProviderId;
            }
            else {
                if (Boolean(this.$route.query.initialCreate)) {
                    this.resourceProviderId = null;
                }
                else {

                    this.resourceProviderId = 0;
                }
            }

        },
        methods: {
            setResourceProvider(value: number) {
                if (this.resourceDetails.resourceProviderId != value) {
                    this.resourceDetails.resourceProviderId = value;
                    this.$emit('save');
                }
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-providedby-tab {
        max-width: 825px;
        padding-bottom: 60px;
    }

    select.form-control {
        border: 2px solid $nhsuk-grey;
        height: 40px !important;
    }

    .span-provider {
        font-family: $font-stack !important;
        font-size: 19px !important;
    }
    /*Add a background color to the radio button when focused */
    .radio-button:focus {
        box-shadow: 0 0 0 4px $nhsuk-yellow !important;
        outline: 0;
        //box-shadow: 0 0 0 4px rgba(255, 255, 0, 0.5);
    }

    .radio-button {
        appearance: none;
        -webkit-appearance: none;
        width: 24px;
        height: 24px;
        border: 2px solid black;
        border-radius: 50%;
        position: relative;
        cursor: pointer;
        transition: box-shadow 0.3s;
        outline: none;
        filter: grayscale(0) !important;
    }

    /* Yellow glow on focus */
    .radio-button:focus {
        box-shadow: 0 0 0 4px $nhsuk-yellow; /* yellow circle */
    }

    /* Inner black dot when selected */
    .radio-button:checked::before {
        content: "";
        position: absolute;
        top: 4px;
        left: 4px;
        width: 12px;
        height: 12px;
        background-color: black;
        border-radius: 50%;
    }

    label {
        display: inline-flex;
        align-items: center;
        gap: 5px; /* space between radio and text */
        margin: 7px;
    }
</style>