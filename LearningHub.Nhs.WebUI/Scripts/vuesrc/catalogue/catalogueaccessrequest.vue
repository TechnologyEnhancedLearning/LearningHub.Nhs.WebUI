<template>
    <div class="catalogue-access-request">
        <div class="lh-container-xl">
            <div>
                <div>
                    <div class="nhsuk-u-padding-bottom-5">
                        <router-link :to="goBackUrl" class="back-link"><i class="fa fa-chevron-left"></i>&nbsp; Go back</router-link>
                    </div>
                </div>
                <div>
                    <div>
                        <h1 class="nhsuk-heading-xl">Access request</h1>
                    </div>
                </div>
                <div v-if="loaded">
                    <div>
                        <div>
                            <h3>{{accessRequest.userFullName}}</h3>
                        </div>
                    </div>
                    <div>
                        <div>
                            <p><b>Email address</b></p>
                        </div>
                    </div>
                    <div>
                        <div>
                            <p>{{accessRequest.emailAddress}}</p>
                        </div>
                    </div>
                    <div>
                        <div>
                            <p><b>Message</b></p>
                        </div>
                    </div>
                    <div>
                        <div>
                            <p>{{accessRequest.message}}</p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <div class="accept-deny-request">
                                <div class="row" v-if="accessRequest.status == 0">
                                    <div class="col-12">
                                        <p>What would you like to do?</p>
                                    </div>
                                    <div v-if="accessRequestResponseMessage != null" class="col-12 responsemessage">
                                        <p>{{accessRequestResponseMessage}} </p>
                                    </div>
                                    <div class="col-12 d-flex justify-content-start mt-5">
                                        <button class="nhsuk-button mr-5" @click="acceptAccessRequest()">{{acceptString}}</button>
                                        <button class="nhsuk-button nhsuk-button--secondary" @click="openDenyModal()">Deny</button>
                                    </div>
                                </div>
                                <div class="row" v-if="accessRequest.status == 1">
                                    <div class="col-12  d-flex flex-row align-items-center">
                                        <i class="fa-solid fa-circle-check"></i>
                                        <p class="mb-0 ml-2">Accepted at {{this.accessRequest.dateApproved | formatDate('h:mm a DD MMM YYYY')}}</p>
                                    </div>
                                </div>
                                <div class="row" v-if="accessRequest.status == 2">
                                    <div class="col-12 d-flex flex-row align-items-center">
                                        <i class="fas fa-times-circle"></i>
                                        <p class="mb-0 ml-2"> Declined at {{this.accessRequest.dateRejected | formatDate('h:mm a DD MMM YYYY')}}</p>
                                    </div>
                                    <div class="col-12 mt-5">
                                        <p>The following reason was sent to the user:</p>
                                    </div>
                                    <div class="col-12 mt-2">
                                        <p>{{accessRequest.responseMessage}}</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mt-20" v-if="accessRequest.status == 1 && catalogue && roleUserGroups && notInRole">
                        <div class="col-12">
                            <p><i class="fa-solid fa-triangle-exclamation"></i> This user no longer has access to this catalogue. If you need further assistance please <a :href="supportUrl" target="_blank">contact support</a></p>
                        </div>
                    </div>
                </div>
                <div class="spacer"></div>
            </div>
        </div>
        <div id="rejectAccessModal" class="modal request-user-modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header text-center">
                        <h4 class="modal-title"><i class="fa-solid fa-triangle-exclamation"></i> Access denied</h4>
                    </div>
                    <div class="modal-body">
                        <div class="model-body-container">
                            <div class="row">
                                <div class="col">
                                    <p>You have not approved this request for the user to access this catalogue. Please provide a reason why the user cannot have access. This will be sent to them to inform them why access to the catalogue.</p>
                                </div>
                            </div>
                            <div class="row mt-5">
                                <div class="col">
                                    <label class="nhsuk-u-visually-hidden" for="rejectionMessage"></label>
                                    <textarea v-model="rejectionMessage" id="rejectionMessage" class="form-control rejection-message-input nhsuk-textarea" rows="6"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer d-flex justify-content-between">
                        <button class="nhsuk-button nhsuk-button--secondary" @click="cancelRejection()">Cancel</button>
                        <button class="nhsuk-button" @click="submitRejection()" v-bind:disabled="!rejectionMessage" :class="{disabled: !rejectionMessage }">{{submitString}}</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Vuelidate from "vuelidate";
    import TabbedGroupHeader from '../globalcomponents/tabbedgroupheader.vue';
    import { catalogueData } from '../data/catalogue';
    import { userData } from '../data/user';
    import { RoleUserGroupModel } from '../models/roleUserGroupModel';
    import moment from 'moment';

    import '../filters';

    Vue.use(Vuelidate as any);

    export default Vue.extend({
        components: {
            TabbedGroupHeader
        },
        props: {
            supportUrl: { Type: String } as PropOptions<string>
        },
        data() {
            return {
                rejectionMessage: '',
                accessRequest: <any>{},
                loaded: false,
                acceptPending: false,
                rejectPending: false,
                roleUserGroups: [] as RoleUserGroupModel[],
                catalogue: null,              
                accessRequestResponseMessage: null,
            };
        },
        created: async function () {
            await this.getAccessRequest();
            if (this.accessRequest.lastResponseMessage) {
                this.rejectionMessage = this.accessRequest.lastResponseMessage;
            }
            await Promise.all([
                await this.loadRoleUserGroups(),
                await this.loadCatalogue()
            ]);
        },
        computed: {
            acceptString(): string {
                return this.acceptPending ? "Processing..." : "Accept";
            },
            submitString(): string {
                return this.rejectPending ? "Processing..." : "Submit";
            },
            goBackUrl(): string {
                return `/Catalogue/Manage/${this.reference}`;
            },
            reference: function (): string {
                return this.$route.params.reference;
            },
            dateCompleted: function (): string {
                var mDateCompleted = moment(this.accessRequest.dateApproved);
                return mDateCompleted.format("h:mma") + " on " + mDateCompleted.format("d MMMM YYYY")
            },
            dateRejected: function (): string {
                var mDateRejected = moment(this.accessRequest.dateRejected);
                return mDateRejected.format("h:mma") + " on " + mDateRejected.format("d MMMM YYYY")
            },
            notInRole: function (): boolean {
                return !this.roleUserGroups.some(rug => rug.catalogueNodeId === this.catalogue.nodeId)
            }
        },
        methods: {
            openDenyModal() {
                $("#rejectAccessModal").modal("show");
            },
            async loadCatalogue(): Promise<void> {
                this.catalogue = await catalogueData.getCatalogue(this.reference);
            },
            async loadRoleUserGroups(): Promise<void> {
                this.roleUserGroups = await userData.getRoleUserGroups(this.accessRequest.userId);
            },
            getAccessRequest: async function (): Promise<void> {
                this.loaded = false;
                this.accessRequest = await catalogueData.getAccessRequest(+this.$route.params.accessRequestId);
                this.loaded = true;
            },
            cancelRejection(): void {
                this.rejectionMessage = '';
                $("#rejectAccessModal").modal("hide");
            },
            async acceptAccessRequest(): Promise<void> {           
                await catalogueData.acceptAccessRequest(this.accessRequest)
                    .then(async x => {
                        this.accessRequestResponseMessage = x.data.details[0];
                        await this.getAccessRequest();
                        await this.loadRoleUserGroups();
                    });
            },
            async submitRejection(): Promise<void> {
                await catalogueData.rejectAccessRequest(+this.$route.params.accessRequestId, this.rejectionMessage)
                    .then(x => {
                        $("#rejectAccessModal").modal("hide");
                        this.rejectionMessage = '';
                        this.getAccessRequest();
                    });
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .btn-green.disabled {
        background-color: #425563 !important;
        border-color: #425563 !important;
    }

    .mt-30 {
        p {
            margin-bottom: 4px;
        }
    }

    .spacer {
        height: 80px;
    }

    h1 {
        font-size: 3.2rem;
    }

    h3 {
        font-size: 2.4rem;
    }

    .rejection-message-input {
        resize: none;
    }

    .catalogue-access-request {
        background-color: $nhsuk-white;
    }

    .back-link {
        text-decoration: none;
    }

    .accept-deny-request {
        background-color: $nhsuk-grey-white;
        border: 1px solid $nhsuk-grey-light;
        padding: 40px;
    }

    .modal-footer {
        justify-content: space-between !important;
    }

    .modal-dialog {
        max-width: 1000px !important;
    }

    .fa-check-circle {
        color: $nhsuk-green;
        font-size: 32px;
    }

    .fa-times-circle {
        color: $nhsuk-red;
        font-size: 32px;
    }

    .fa-exclamation-triangle {
        color: $nhsuk-warm-yellow;
    }

    .manage-catalogue-container {
        background: #fff;
    }

    .column-layout {
        display: flex;
    }

    .main-column {
        flex: 2;
        order: 1;
    }

    .sub-column {
        flex: 1;
        order: 2;
        display: none;
    }
    .responsemessage {
        color: $nhsuk-red;
    }

    @media screen and (min-width: 769px) {
        .sub-column {
            display: block;
        }
    }

    @media (min-width: 414px) and (max-width: 479px) {
        .sub-column {
            margin-left: auto;
            margin-right: auto;
            width: 42%;
        }
    }

    @media (min-width: 374px) and (max-width: 413px) {
        .sub-column {
            margin-left: auto;
            margin-right: auto;
            width: 36%;
        }
    }

    @media (min-width: 320px) and (max-width: 373px) {
        .sub-column {
            margin-left: auto;
            margin-right: auto;
            width: 42%;
        }
    }

    .modal-footer {
        padding-left: 9px !important;
        padding-right: 9px !important;
    }

    @media(max-width: 576px) {
        .modal-footer > * {
            margin: 0
        }

        .modal-footer {
            padding-left: 0 !important;
            padding-right: 0 !important;
        }

        .modal-dialog {
            min-height: 100%;
            margin: 0;

            .modal-content {
                padding: 16px;
                border-radius: 0;
                height: 100%;

                .modal-body {
                    padding: 0;
                }

                .modal-header {
                    padding: 0;
                    padding-bottom: 1rem;
                }
            }
        }

        .modal {
            padding-right: 0 !important;
        }
    }
</style>