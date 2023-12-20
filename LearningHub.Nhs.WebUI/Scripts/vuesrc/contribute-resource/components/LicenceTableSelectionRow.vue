<template>
    <tr>
        <td></td>
        <td v-for="licence in licences">
            <LicenceSelectButton v-bind:licence="licence"
                                 v-bind:selected-licence-id="selectedLicenceId"
                                 v-on:choose-licence="onChooseLicence"/>
        </td>
    </tr>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import { LicenceDetails } from "../../models/contribute-resource/licenceDetails";

    import LicenceSelectButton from "./LicenceSelectButton.vue";

    export default Vue.extend({
        props: {
            licences: { type: Array } as PropOptions<LicenceDetails[]>,
            selectedLicenceId: Number,
        },
        components: {
            LicenceSelectButton,
        },
        methods: {
            isCurrentlySelectedLicence(id: number): boolean {
                return this.selectedLicenceId === id;
            },
            onChooseLicence(id: number) {
                this.$emit('choose-licence', id);
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;
    
    td {
        padding: 20px;
        border: 1px solid $nhsuk-grey-placeholder;
        text-align: center;
    }
    
    i.fa-check-circle {
        font-size: 4rem;
        vertical-align: middle;
        color: $nhsuk-green;
    }
</style>