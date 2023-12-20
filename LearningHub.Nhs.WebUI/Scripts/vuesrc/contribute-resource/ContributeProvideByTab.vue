<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl py-15">
            <div class="contribute contribute-providedby-tab">
                <h2 id="licence-heading" class="nhsuk-heading-l pt-15">Content provided by</h2>
                <p class="nhsuk-u-font-weight-normal">When applicable pease select the provider of this content. This will enable users to search for content produced by specific organisations.</p>
                <div class="my-3">
                    <input class="radio-button" name="resourceProviderId" type="radio" :value="0" v-model="resourceProviderId" @click="setResourceProvider($event.target.value)" />
                    <span class="span-provider nhsuk-u-font-weight-normal">Not applicable</span>
                    <div v-for="provider in userProviders">
                        <label class="my-0 span-provider nhsuk-u-font-weight-normal">
                            <input class="radio-button" name="resourceProviderId" type="radio" :value="provider.id" v-model="resourceProviderId" @click="setResourceProvider($event.target.value)" />
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
                                    <a href="#" class="collapsed" data-toggle="collapse" data-target="#collapseProvidedByInfo" aria-expanded="false" aria-controls="collapseProvidedByInfo">
                                        <div class="accordion-arrow">Why should I flag a resource as 'Provided by'</div>
                                    </a>
                                </div>
                            </div>
                            <div id="collapseProvidedByInfo" class="collapse" aria-labelledby="headingProvidedBy" data-parent="#provided-by-info-accordion">
                                <div class="content col-12">
                                    <p>
                                        <b>
                                            When publishing a resource it is important to mark a resource as 'Provided by' as it helps;
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
</style>