<template>
    <table>
        <LicenceTableHeaderRow v-bind:licences="licences"/>

        <LicenceTableDividerRow row-description="Users can" row-id="licence-table--users-can" v-bind:licences="licences" />
        <LicenceTableRow row-description="View the material"
                         v-bind:licences="licences"
                         v-bind:property="(l) => l.usersCanDetails.canView"
                         true-value="Allowed"
                         false-value="Not allowed"
                         row-id="licence-table--can-view"
                         header-ids="licence-table--users-can" />
        <LicenceTableRow row-description="Copy the material"
                         v-bind:licences="licences"
                         v-bind:property="(l) => l.usersCanDetails.canCopy"
                         true-value="Allowed"
                         false-value="Not allowed"
                         row-id="licence-table--can-copy"
                         header-ids="licence-table--users-can" />
        <LicenceTableRow row-description="Distribute the material"
                         v-bind:licences="licences"
                         v-bind:property="(l) => l.usersCanDetails.canDistribute"
                         true-value="Allowed"
                         false-value="Not allowed"
                         row-id="licence-table--can-distribute"
                         header-ids="licence-table--users-can" />
        <LicenceTableRow row-description="Adapt the material"
                         v-bind:licences="licences"
                         v-bind:property="(l) => l.usersCanDetails.canAdapt"
                         true-value="Allowed"
                         false-value="Not allowed"
                         row-id="licence-table--can-adapt"
                         header-ids="licence-table--users-can" />
        <LicenceTableRow row-description="Distribute the adaptation"
                         v-bind:licences="licences"
                         v-bind:property="(l) => l.usersCanDetails.canDistributeAdaptations"
                         true-value="Allowed"
                         false-value="Not allowed"
                         row-id="licence-table--can-distribute-adaptations"
                         header-ids="licence-table--users-can" />
        <LicenceTableRow row-description="Use commercially"
                         v-bind:licences="licences"
                         v-bind:property="(l) => l.usersCanDetails.canUseCommercially"
                         true-value="Allowed"
                         false-value="Not allowed"
                         row-id="licence-table--can-use-commercially"
                         header-ids="licence-table--users-can" />

        <LicenceTableDividerRow row-description="Users must" row-id="licence-table--users-must" v-bind:licences="licences" />
        <LicenceTableRow row-description="Credit the owner of the material"
                         v-bind:licences="licences"
                         v-bind:property="(l) => l.usersMustDetails.mustCreditOwner"
                         true-value="Required"
                         false-value="Not required"
                         row-id="licence-table--must-credit-owner"
                         header-ids="licence-table--users-must" />
        <LicenceTableRow row-description="Apply original licence to adaptations"
                         v-bind:licences="licences"
                         v-bind:property="(l) => l.usersMustDetails.mustApplyOriginalLicence"
                         true-value="Required"
                         false-value="Not required"
                         row-id="licence-table--must-apply-original-licence"
                         header-ids="licence-table--users-must" />

        <LicenceTableSelectionRow v-on:choose-licence="onChooseLicence"
                                  v-bind:licences="licences"
                                  v-bind:selected-licence-id="selectedLicenceId" />
    </table>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import LicenceTableDividerRow from "./LicenceTableDividerRow.vue";
    import LicenceTableHeaderRow from "./LicenceTableHeaderRow.vue";
    import LicenceTableRow from "./LicenceTableRow.vue";
    import LicenceTableSelectionRow from "./LicenceTableSelectionRow.vue";
    import { LicenceDetails } from "../../models/contribute-resource/licenceDetails";

    export default Vue.extend({
        components: {
            LicenceTableDividerRow,
            LicenceTableHeaderRow,
            LicenceTableRow,
            LicenceTableSelectionRow,
        },
        props: {
          selectedLicenceId: Number,
          licences: { type: Array } as PropOptions<LicenceDetails[]>,
        },
        methods: {
            onChooseLicence(id: number) {
                this.$emit('choose-licence', id);
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    table {
        table-layout: fixed;
        width: 100%;
        border: 1px solid $nhsuk-grey-placeholder;
        background-color: $nhsuk-white;
        font-size: 1.6rem;
    }
</style>