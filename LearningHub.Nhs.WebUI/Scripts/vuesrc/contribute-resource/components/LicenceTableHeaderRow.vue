<template>
    <tr>
        <th></th>
        <th v-for="licence in licences" v-bind:id="`licence-table--licence-${licence.id}`">
            <LicenceTableHeaderIcons class="pb-12" v-bind:licence-terms="licence.terms"/>
            <a v-bind:href="licence.url" target="_blank">
                {{licence.name}}
            </a>
        </th>
    </tr>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    
    import LicenceTableHeaderIcons from "./LicenceTableHeaderIcons.vue";
    
    import { LicenceDetails } from "../../models/contribute-resource/licenceDetails";
    
    export default Vue.extend({
        props: {
            licences: { type: Array } as PropOptions<LicenceDetails[]>,
        },
        components: {
            LicenceTableHeaderIcons,
        },
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    tr {
        vertical-align: top;
    }

    th {
        padding: 20px;
        border: 1px solid $nhsuk-grey-placeholder;
        background-color: $nhsuk-white;
        font-size: 1.6rem;
        font-weight: normal;
     }
</style>