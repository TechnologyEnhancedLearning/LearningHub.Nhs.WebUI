<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl py-15">
            <div class="contribute-licence-tab">
                <h2 id="licence-heading" class="nhsuk-heading-l pt-15">Licence</h2>
                <div class="mb-10">
                    It is your responsibility to select the most appropriate licence type for this resource.<br>
                    <a v-bind:href="moreInfoLink" class="mb-10 accessible-link" target="_blank">More information on licences</a>
                </div>

                <select class="form-control" aria-labelledby="licence-heading" id="licence" v-model="resourceDetails.resourceLicenceId">
                    <option disabled v-bind:value="null" id="select">Please choose...</option>
                    <option v-for="licence in licences" :value="licence.id" :id="licence.id">
                        {{licence.title}}
                    </option>
                </select>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
    import { LicenceModel } from "../models/contribute/licenceModel";
    import SupportUrls from '../data/supportUrls';
    
    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>,
            licences: { type: Array } as PropOptions<LicenceModel[]>,
        },
        data() {
            return {
                moreInfoLink: SupportUrls.resourceLicenceUrl,
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;
    
    .contribute-licence-tab {
        max-width: 825px;
        padding-bottom: 60px;
    }
    
    select.form-control {
        border: 2px solid $nhsuk-grey;
        height: 40px !important;
    }
</style>