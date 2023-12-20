<template>
    <tr class="licence-table-divider-row">
        <th v-bind:colspan="(licences.length + 1)" v-bind:id="rowId">{{rowDescription}}</th>
    </tr>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import { LicenceDetails } from "../../models/contribute-resource/licenceDetails";

    export default Vue.extend({
        props: {
            rowId: String,
            rowDescription: String,
            licences: { type: Array } as PropOptions<LicenceDetails[]>,
        },
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .licence-table-divider-row {
        font-family: $font-stack-bold;
        background-color: $nhsuk-grey-white;
    }

    th {
        padding: 20px;
        border: 1px solid $nhsuk-grey-placeholder;
    }
</style>