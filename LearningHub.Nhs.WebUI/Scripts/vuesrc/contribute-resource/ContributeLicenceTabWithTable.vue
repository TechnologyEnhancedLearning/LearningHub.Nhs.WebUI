<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl py-15">
            <div class="contribute-licence-tab">
                <h2 id="licence-heading" class="nhsuk-heading-l pt-15">Licence</h2>
                <div class="mb-40">
                    Before a resource can be published a licence must be selected.
                    This summary table is aimed at helping you decide which licence is the best type for the content.
                    It is not a replacement for the full licence detail which can be found by selecting the licence
                    links at the top of the table.
                </div>

                <LicenceTable class="mb-40 licence-table-view"
                              v-on:choose-licence="onChooseLicence"
                              v-bind:licences="licences"
                              v-bind:selected-licence-id="selectedLicenceId"/>
                <LicenceListView class="mb-40 licence-list-view"
                                 v-on:choose-licence="onChooseLicence"
                                 v-bind:licences="licences"
                                 v-bind:selected-licence-id="selectedLicenceId"/>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    import LicenceTable from "./components/LicenceTable.vue";
    import LicenceListView from "./components/LicenceListView.vue";
    import {
        LicenceDetails,
        LicenceTerms,
        UsersCanDetails,
        UsersMustDetails
    } from "../models/contribute-resource/licenceDetails";
    import SupportUrls from '../data/supportUrls';
    
    export default Vue.extend({
        props: {
            selectedLicenceId: Number,
        },
        components: {
            LicenceListView,
            LicenceTable,
        },
        data() {
            return {
                licences: getLicences(),
            }
        },
        methods: {
            onChooseLicence(id: number) {
                this.$emit('choose-licence', id);
            },
        }
    })

    function getLicences(): LicenceDetails[] {
        return [
            new LicenceDetails({
                id: 1,
                name: "Attribution-NonCommercial 4.0 International",
                url: "https://creativecommons.org/licenses/by-nc/4.0/",
                terms: new LicenceTerms({
                    attribution: true,
                    creativeCommons: true,
                    nonCommercial: true,
                }),
                usersCanDetails: new UsersCanDetails({
                    canView: true,
                    canCopy: true,
                    canDistribute: true,
                    canAdapt: true,
                    canDistributeAdaptations: true
                }),
                usersMustDetails: new UsersMustDetails({
                    mustCreditOwner: true
                }),
            }),
            new LicenceDetails({
                id: 2,
                name: "Attribution-NonCommercial-ShareAlike 4.0 International",
                url: "https://creativecommons.org/licenses/by-nc-sa/4.0/",
                terms: new LicenceTerms({
                    attribution: true,
                    creativeCommons: true,
                    nonCommercial: true,
                    shareAlike: true,
                }),
                usersCanDetails: new UsersCanDetails({
                    canView: true,
                    canCopy: true,
                    canDistribute: true,
                    canAdapt: true,
                    canDistributeAdaptations: true
                }),
                usersMustDetails: new UsersMustDetails({
                    mustCreditOwner: true,
                    mustApplyOriginalLicence: true,
                }),
            }),
            new LicenceDetails({
                id: 3,
                name: "Attribution-NonCommercial-NoDerivatives 4.0 International (CC BY-NC-ND 4.0)",
                url: "https://creativecommons.org/licenses/by-nc-nd/4.0/",
                terms: new LicenceTerms({
                    attribution: true,
                    creativeCommons: true,
                    nonCommercial: true,
                    noDerivatives: true,
                }),
                usersCanDetails: new UsersCanDetails({
                    canView: true,
                    canCopy: true,
                    canDistribute: true,
                }),
                usersMustDetails: new UsersMustDetails({
                    mustCreditOwner: true
                }),
            }),
            new LicenceDetails({
                id: 4,
                name: "All rights reserved",
                url: SupportUrls.resourceLicenceUrl,
                terms: new LicenceTerms({
                    allRightsReserved: true
                }),
                usersCanDetails: new UsersCanDetails({
                    canView: true,
                }),
                usersMustDetails: new UsersMustDetails(),
            }),
        ]
    }
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .licence-table-view {
        display: none;
    }

    .licence-list-view {
        display: block;
    }

    @media screen and (min-width: 900px) {
        .licence-table-view {
            display: table;
        }

        .licence-list-view {
            display: none;
        }
    }

    .contribute-licence-tab {
        max-width: 931px;
        padding-bottom: 60px;
    }
</style>