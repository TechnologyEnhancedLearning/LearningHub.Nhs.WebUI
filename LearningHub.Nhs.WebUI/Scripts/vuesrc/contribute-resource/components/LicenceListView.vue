<template>
    <div>
        <div class="licence-list-view--licence" v-for="licence in licences">
            <div class="py-15 px-20">
                <h3 class="licence-list-view--licence-name">
                    <a v-bind:href="licence.url" target="_blank">{{licence.name}}</a>
                </h3>
            </div>
            <div class="licence-list-view--divider"></div>
            <div class="licence-list-view--info-block-container py-15 px-20 d-flex flex-wrap">
                <div class="licence-list-view--info-block d-flex flex-wrap flex-column">
                    <h4>Users can:</h4>
                    <ul>
                        <LicenceInfoDatum v-bind:is-ticked="licence.usersCanDetails.canView"
                                          description="View the material"
                                          true-value="can"
                                          false-value="cannot"/>
                        <LicenceInfoDatum v-bind:is-ticked="licence.usersCanDetails.canCopy"
                                          description="Copy the material"
                                          true-value="can"
                                          false-value="cannot"/>
                        <LicenceInfoDatum v-bind:is-ticked="licence.usersCanDetails.canDistribute"
                                          description="Distribute the material"
                                          true-value="can"
                                          false-value="cannot"/>
                        <LicenceInfoDatum v-bind:is-ticked="licence.usersCanDetails.canAdapt"
                                          description="Adapt the material"
                                          true-value="can"
                                          false-value="cannot"/>
                        <LicenceInfoDatum v-bind:is-ticked="licence.usersCanDetails.canDistributeAdaptations"
                                          description="Distribute the adaptation"
                                          true-value="can"
                                          false-value="cannot"/>
                        <LicenceInfoDatum v-bind:is-ticked="licence.usersCanDetails.canUseCommercially"
                                          description="Use commercially"
                                          true-value="can"
                                          false-value="cannot"/>
                    </ul>
                </div>
                <div class="licence-list-view--info-block d-flex flex-wrap flex-column">
                    <h4>Users must:</h4>
                    <ul>
                        <LicenceInfoDatum v-bind:is-ticked="licence.usersMustDetails.mustCreditOwner"
                                          description="Credit the owner of the material"
                                          true-value="are required to"
                                          false-value="are not required to"/>
                        <LicenceInfoDatum v-bind:is-ticked="licence.usersMustDetails.mustApplyOriginalLicence"
                                          description="Apply original licence to adaptations"
                                          true-value="are required to"
                                          false-value="are not required to"/>
                    </ul>
                </div>
            </div>
            <div class="licence-list-view--divider"></div>
            <div class="d-flex align-items-center justify-content-center py-15 px-25">
                <LicenceSelectButton v-bind:licence="licence"
                                     v-bind:selected-licence-id="selectedLicenceId"
                                     v-on:choose-licence="onChooseLicence"/>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import { LicenceDetails } from "../../models/contribute-resource/licenceDetails";

    import LicenceInfoDatum from "./LicenceInfoDatum.vue";
    import LicenceSelectButton from "./LicenceSelectButton.vue";

    export default Vue.extend({
        props: {
            licences: { type: Array } as PropOptions<LicenceDetails[]>,
            selectedLicenceId: Number,
        },
        components: {
            LicenceInfoDatum,
            LicenceSelectButton,
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

    .licence-list-view {
        &--licence-name {
            font-size: inherit;
            font-family: inherit;
            margin-bottom: 0;
        }

        &--info-block-container {
            margin: -5px 0;

            @media screen and (max-width: 540px) {
                flex-direction: column;
            }
        }

        &--info-block {
            width: 50%;
            margin: 5px 0;

            @media screen and (max-width: 540px) {
                width: 100%;
            }
        }

        &--divider {
            border-top: 1px solid $nhsuk-grey-light;
        }

        &--licence {
            background-color: $nhsuk-white;
            border: 1px solid $nhsuk-grey-lighter;
            margin: 25px 0;
            box-shadow: 0 4px $nhsuk-grey-lighter;
        }
        
        ul {
            list-style-type: none;
            margin: 0;
            padding: 0;
        }
        
        h4 {
            font-size: inherit;
        }
    }
</style>