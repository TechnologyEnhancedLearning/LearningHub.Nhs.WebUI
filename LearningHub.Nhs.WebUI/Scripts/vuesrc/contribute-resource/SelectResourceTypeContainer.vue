<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl">
            <h2 id="title-label" class="nhsuk-heading-l">Select a resource type</h2>
            <SelectResourceType :resourceType="resourceType.GENERICFILE" v-bind:resourceDetails="resourceDetails"></SelectResourceType>
            <SelectResourceType :resourceType="resourceType.WEBLINK" v-bind:resourceDetails="resourceDetails"></SelectResourceType>
            <SelectResourceType :resourceType="resourceType.ARTICLE" v-bind:resourceDetails="resourceDetails"></SelectResourceType>
            <SelectResourceType :resourceType="resourceType.ASSESSMENT" v-bind:resourceDetails="resourceDetails"></SelectResourceType>
            <SelectResourceType :resourceType="resourceType.CASE" v-bind:resourceDetails="resourceDetails"></SelectResourceType>
            <SelectResourceType :resourceType="resourceType.SCORM" v-bind:resourceDetails="resourceDetails"></SelectResourceType>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import SelectResourceType from './SelectResourceType.vue';
    import { ResourceType } from '../constants';
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';

    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>
        },
        components: {
            SelectResourceType
        },
        data() {
            return {
                resourceType: ResourceType
            }
        }
    });
</script>
