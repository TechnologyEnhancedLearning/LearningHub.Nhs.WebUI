<template>
    <div class="licence-table-header-icons d-flex flex-wrap">
        <img src='/images/licenceicons/creative-commons.svg' alt="Creative Commons" v-if="licenceTerms.creativeCommons">
        <img src='/images/licenceicons/attribution.svg' alt="Attribution" v-if="licenceTerms.attribution">
        <img src='/images/licenceicons/non-commercial.svg' alt="Non Commercial" v-if="licenceTerms.nonCommercial">
        <img src='/images/licenceicons/share-alike.svg' alt="Share Alike" v-if="licenceTerms.shareAlike">
        <img src='/images/licenceicons/no-derivatives.svg' alt="No Derivatives" v-if="licenceTerms.noDerivatives">
        <img src='/images/licenceicons/all-rights-reserved.svg' alt="All Rights Reserved" v-if="licenceTerms.allRightsReserved">
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { LicenceTerms } from "../../models/contribute-resource/licenceDetails";
    
    export default Vue.extend({
        props: {
            licenceTerms: { type: Object } as PropOptions<LicenceTerms>
        },
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .licence-table-header-icons {
        margin: -4 -4px;

        img {
            margin: 4px;
            width: 32px;
            height: 32px;
        }
    }    
</style>