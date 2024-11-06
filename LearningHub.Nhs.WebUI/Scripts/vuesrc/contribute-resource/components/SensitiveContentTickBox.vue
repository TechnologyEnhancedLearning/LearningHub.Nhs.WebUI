<template>
    <div>

        <h2 class="nhsuk-heading-l">
            Sensitive content
        </h2>

        <div>
            Do you want learners to be notified that this resource contains sensitive content, which they
            may find offensive or disturbing, before they access it?
        </div>

        <div class="mt-3">
            <label class="checkContainer mb-0" for="sensitivecontent">
                Yes
                <input type="checkbox" id="sensitivecontent" v-model="resourceDetails.sensitiveContent">
                <span class="checkmark"></span>
            </label>
        </div>

    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { ContributeResourceDetailModel } from "../../models/contribute/contributeResourceModel";
    
    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>
        },
    }) 
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .checkContainer {
        font-family: $font-stack;
        padding-left: 40px;
    }
</style>