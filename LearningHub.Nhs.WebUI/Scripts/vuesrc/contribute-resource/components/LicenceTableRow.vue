<template>
    <tr>
        <th v-bind:id="rowId">{{rowDescription}}</th>
        <td class="licence-tab-icon"
            v-bind:headers="`licence-table--licence-${licence.id} ${headerIds} ${rowId}`"
            v-for="licence in licences">
            <i v-bind:class="getIconClass(licence)" v-bind:aria-label="getIsTicked(licence) ? trueValue : falseValue"></i>
        </td>
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
            property: Function,
            trueValue: String,
            falseValue: String,
            headerIds: String,
        },
        methods: {
            getIconClass(licence: LicenceDetails): string {
                return this.getIsTicked(licence) ? "fas fa-check" : "fas fa-times";
            },
            getIsTicked(licence: LicenceDetails): boolean {
                return this.property(licence);
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    td, th {
        padding: 20px;
        border: 1px solid $nhsuk-grey-placeholder;
        font-weight: normal;

        &.licence-tab-icon {
            text-align: center;
        }
    }

    i {
        font-size: 2.4rem;
        vertical-align: middle;

        &.fa-check {
            color: $nhsuk-green;
        }
        &.fa-times{
            color: $nhsuk-red;
        }
    }
</style>