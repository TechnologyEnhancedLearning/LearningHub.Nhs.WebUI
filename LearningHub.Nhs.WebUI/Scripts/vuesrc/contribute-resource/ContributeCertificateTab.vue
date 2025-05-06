<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl py-15">
            <div class="contribute contribute-licence-tab">
                <h2 id="licence-heading" class="nhsuk-heading-l pt-15">Certificate <i v-if="certificateEnabled===null" class="warningTriangle fas fa-exclamation-triangle"></i></h2>
                <p class="nhsuk-u-font-weight-bold">Should a learner receive a certificate after completing the learning?</p>
                <div class="mb-10">
                    <label class="checkContainer mr-0">
                        <span>Yes</span>
                        <input type="radio" name="resourceCertificateRadio" class="nhsuk-radios__input" value="true" v-model="resourceDetails.certificateEnabled" @click="certificateEnabledChange($event.target.value)" />
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer mr-0">
                        <span>No</span>
                        <input type="radio" name="resourceCertificateRadio" class="nhsuk-radios__input" value="false" v-model="resourceDetails.certificateEnabled" @click="certificateEnabledChange($event.target.value)" checked/>
                        <span class="radioButton"></span>
                    </label>
                </div>
                <div class="mb-10">
                    <a v-bind:href="moreInfoLink" class="mb-10 accessible-link" target="_blank">More information on certificates</a>
                </div>


            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
    import SupportUrls from '../data/supportUrls';
    
    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>
        },
        data() {
            return {
                moreInfoLink: SupportUrls.resourceCertificateUrl,
                certificateEnabled: null
            }
        },
        created() {
            if (this.resourceDetails.certificateEnabled !== null) {
                this.certificateEnabled = this.resourceDetails.certificateEnabled;
            }
        },
        methods: {
            certificateEnabledChange(useDefault: boolean) {
                this.certificateEnabled = useDefault;
                if (this.resourceDetails.certificateEnabled !== useDefault)
                this.resourceDetails.certificateEnabled = useDefault;
                this.$emit('save');              
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