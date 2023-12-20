<template>
    
    <div class="manage-catalogue-container">

        <div v-if="isCatalogueLocalAdmin" >
            <div v-if="isLoading" class="d-flex flex-column align-items-center">
                <div class="loading-title">Loading...</div>
                <div class="loading-spinner">
                    <i class="fa fa-spinner fa-spin fa-3x"></i>
                </div>
            </div>

            <div v-if="catalogue && !isLoading">
                <div class="lh-padding-fluid">
                    <div class="lh-container-xl">
                        <div class="nhsuk-back-link">
                            <router-link :to="'/Catalogue/' + reference" class="nhsuk-back-link__link" id="goBackLink"><svg class="nhsuk-icon nhsuk-icon__chevron-left" 
                                xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" aria-hidden="true">
                                <path d="M8.5 12c0-.3.1-.5.3-.7l5-5c.4-.4 1-.4 1.4 0s.4 1 0 1.4L10.9 12l4.3 4.3c.4.4.4 1 0 1.4s-1 .4-1.4 0l-5-5c-.2-.2-.3-.4-.3-.7z"></path>
                                </svg> Go back</router-link>
                        </div>
                        <h1>Catalogue Management</h1>
                        <div class="nhsuk-grid-row">
                            <div class="nhsuk-grid-column-full">
                                <p>You are managing the following catalogue and can invite users to request access.</p>
                                <input type="button" class="nhsuk-button" @click="inviteUserModal()" value="Invite Users" v-if="catalogue.restrictedAccess" />  

                            </div>
                        </div>
                        <div class="row pt-4">
                            <div class="col">
                                <h2>{{catalogue.name}}</h2>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="lh-padding-fluid-md">
                    <div class="lh-container-xl">
                        <div class="row mx-0 w-100 my-4">
                            <div class="col-12 px-md-0">
                                <nav class="subnavbarwhite navbar-expand-md navbar-toggleable-md">
                                    <div class="bar">
                                        <div class="navbar-toggler navbar-toggler-left">{{ activeTabName }}</div>
                                        <button class="navbar-toggler navbar-toggler-right" type="button" aria-controls="collapsingSubNavbar" aria-expanded="false" aria-label="Toggle navigation" id="subnavbar-toggler" data-toggle="collapse" data-target="#collapsingSubNavbar">
                                            <i class="fa-solid fa-chevron-down"></i>
                                            <i class="fa-solid fa-xmark"></i>
                                        </button>
                                    </div>
                                    <div class="navbar-collapse collapse" id="collapsingSubNavbar">
                                        <ul class="navbar-nav">
                                            <li v-if="catalogue.restrictedAccess" class="subnavwhite-item" v-bind:class="[isActive('accessRequestsTab') && 'active']" v-on:click="activateTab('accessRequestsTab')">
                                                <a ref="accessRequestsLink" tabindex="0" class="subnavwhite-link text-nowrap" data-toggle="collapse" data-target=".navbar-collapse.show">{{ tabs[0].name }}</a>
                                            </li>
                                            <li v-if="catalogue.restrictedAccess" class="subnavwhite-item" v-bind:class="[isActive('usersTab') && 'active']" v-on:click="activateTab('usersTab')">
                                                <a ref="usersLink" tabindex="0" class="subnavwhite-link text-nowrap" data-toggle="collapse" data-target=".navbar-collapse.show">{{ tabs[1].name }}</a>
                                            </li>
                                            <li class="subnavwhite-item" v-bind:class="[isActive('foldersTab') && 'active']" v-on:click="activateTab('foldersTab')">
                                                <a ref="foldersLink" tabindex="0" class="subnavwhite-link text-nowrap" data-toggle="collapse" data-target=".navbar-collapse.show">{{ tabs[2].name }}</a>
                                            </li>
                                        </ul>
                                    </div>
                                </nav>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="lh-padding-fluid">
                    <div class="lh-container-xl">
                        <div id="usersTab" v-if="catalogue.restrictedAccess" v-show="isActive('usersTab')" class="pb-md-50">
                            <div class="row">
                                <div class="col" style="min-width:400px; width:400px;">
                                    <div class="input-group pt-4" id="input-group-searchbar-md">
                                        <input class="form-control small pl-4" v-model="emailAddressFilter" type="search" placeholder="Find a user by their email address" aria-label="Search your learning activity" id="input-search-md" @change="loadUsers();" v-on:keyup="searchUsersKeyUp($event.keyCode)">
                                        <span class="input-group-append">
                                            <button class="btn btn-outline-secondary btn-search" type="button" name="button-search" aria-label="search" v-on:click="loadUsers()" style="margin-top: 0;">
                                                <svg class="nhsuk-icon nhsuk-icon__search" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" aria-hidden="true" focusable="false" width="27" height="27">
                                                    <path d="M19.71 18.29l-4.11-4.1a7 7 0 1 0-1.41 1.41l4.1 4.11a1 1 0 0 0 1.42 0 1 1 0 0 0 0-1.42zM5 10a5 5 0 1 1 5 5 5 5 0 0 1-5-5z"></path>
                                                </svg>
                                            </button>
                                        </span>
                                    </div>
                                </div>
                                <div class="d-flex flex-row flex-wrap">
                                    <label class="checkContainer-medium mt-25 mb-0 ml-4">
                                        Added by Catalogue admins
                                        <input type="checkbox" v-model="includeCatalogueAdmins" @change="loadUsers()" />
                                        <span class="checkmark"></span>
                                    </label>
                                    <label class="checkContainer-medium mt-25 mb-0 ml-4">
                                        Added by Platform admins
                                        <input type="checkbox" v-model="includePlatformAdmins" @change="loadUsers()" />
                                        <span class="checkmark"></span>
                                    </label>
                                </div>
                            </div>

                            <div class="row pt-4" v-if="showLoadingUsers">
                                <div class="col">
                                    <div class="d-flex flex-column align-items-left">
                                        <div class="loading-spinner">
                                            <i class="fa fa-spinner fa-spin fa-2x"></i>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row pt-4">
                                <div class="col">
                                    <p class="small mt-2 mb-2" v-if="userCount !== 0">Showing 1 - {{(skip + take) < userCount ? (skip + take) : userCount}} of {{userCount}}</p>
                                    <p class="small mt-2 mb-2" v-if="userCount === 0">There are no results to display</p>
                                </div>
                            </div>

                            <div class="row pt-4 d-none d-md-block">
                                <div class="col">
                                    <table class="table small mb-0">
                                        <thead>
                                            <tr>
                                                <th style="width:20%">Name</th>
                                                <th style="width:32%">Email address</th>
                                                <th style="width:18%">Added by</th>
                                                <th style="width:20%">Time / date added</th>
                                                <th style="width:10%; text-align: center;">Remove</th>
                                            </tr>
                                        </thead>
                                        <tbody v-if="users.length > 0">
                                            <tr v-for="(user, index) in users" :key="user.userUserGroupId">
                                                <td>{{user.fullName}}</td>
                                                <td>{{user.emailAddress}}</td>
                                                <td>{{user.addedByUsername}}</td>
                                                <td v-if="user.canRemove">{{user.addedDatetime | formatDate('HH:mm DD MMM YYYY')}}</td>
                                                <td v-if="!user.canRemove">Not available</td>
                                                <td style="text-align: center;" v-if="user.canRemove"><span><a @click="removeUserModal(user)" class="remove-user"><i class="fa fa-times"></i></a></span></td>
                                                <td style="text-align: center;" v-if="!user.canRemove"><span class="lock-user"><i class="fa fa-lock"></i></span></td>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <div class="d-flex justify-content-center mt-4" v-if="users.length === 0">There are no results to display</div>

                                </div>
                            </div>

                            <div class="row d-md-none">
                                <div class="col">

                                    <div class="mobile-user mt-3" v-for="(user, index) in users" :key="user.userUserGroupId">
                                        <div @click="toggleUserDetails(user)" class="mobile-user-section-header p-10">
                                            <span style="cursor:pointer">
                                                <i v-if="!user.showExpandedDetails" class="fa fa-plus-circle toggle-circle" aria-hidden="true"></i>
                                                <i v-if="user.showExpandedDetails" class="fa fa-minus-circle toggle-circle" aria-hidden="true"></i>
                                                <span>{{user.fullName}}</span>
                                            </span>
                                        </div>
                                        <div v-if="user.showExpandedDetails" class="mobile-user-details h-100 d-flex flex-column p-10">
                                            <div class="mobile-user-detail-header mb-2"><span>Email address</span></div>
                                            <div class="mb-2"><span>{{user.emailAddress}}</span></div>
                                            <div class="mobile-user-detail-header mb-2"><span>Added by</span></div>
                                            <div class="mb-2"><span>{{user.addedByUsername}}</span></div>
                                            <div class="mobile-user-detail-header  mb-2"><span>Time / date added</span></div>
                                            <div v-if="user.canRemove" class="mb-2"><span>{{user.addedDatetime | formatDate('hh:mm a DD MMM YYYY')}}</span></div>
                                            <div v-if="!user.canRemove" class="mb-2"><span>Not available</span></div>
                                            <div v-if="user.canRemove"><span class="mb-2"><a class="admin-link" @click.prevent="removeUserModal(user);" href="#"><i class="fa fa-times remove-user mr-2"></i>Remove</a></span></div>
                                            <div v-if="!user.canRemove"><span class="lock-user mb-2"><i class="fa fa-lock mr-2"></i>Locked</span></div>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="row">
                                <div class="col mb-20 my-20">
                                    <button class="btn btn-outline-custom" @click="loadMoreUsers()" v-if="this.users && (this.users.length < this.userCount)" :disabled="disableLoadMore">
                                        {{ loadMoreUsersButtonText }}
                                        <i class="fa fa-spinner fa-spin ml-3" v-if="disableLoadMore"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div id="accessRequestsTab" v-if="catalogue.restrictedAccess" v-show="isActive('accessRequestsTab')">
                            <div class="row">
                                <div class="d-flex flex-row flex-wrap">
                                    <label class="checkContainer-medium mt-10 mb-0 mx-4">
                                        Pending ({{accessRequests.filter((item) => item.catalogueAccessRequestStatus === AccessRequestStatus.Pending).length}})
                                        <input type="checkbox" v-model="accessRequestsFilter.includePending" />
                                        <span class="checkmark"></span>
                                    </label>
                                    <label class="checkContainer-medium mt-10 mb-0 mx-4">
                                        Approved ({{ accessRequests.filter((item) => item.catalogueAccessRequestStatus === AccessRequestStatus.Approved).length }})
                                        <input type="checkbox" v-model="accessRequestsFilter.includeApproved" />
                                        <span class="checkmark"></span>
                                    </label>
                                    <label class="checkContainer-medium mt-10 mb-0 ml-4">
                                        Denied ({{ accessRequests.filter((item) => item.catalogueAccessRequestStatus === AccessRequestStatus.Denied).length }})
                                        <input type="checkbox" v-model="accessRequestsFilter.includeDenied" />
                                        <span class="checkmark"></span>
                                    </label>
                                </div>
                            </div>
                            <div class="row pt-4 d-none d-md-block">
                                <div class="col pb-md-50">
                                    <table class="table small mb-0">
                                        <thead>
                                            <tr>
                                                <th style="width:22%">Name</th>
                                                <th style="width:35%">Email address</th>
                                                <th style="width: 23%">Time / date requested</th>
                                                <th style="text-align: right; width:20%">Status</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr v-for="accessRequest in filteredAccessRequests">
                                                <td><router-link :to="'/Catalogue/Manage/' + reference + '/AccessRequest/' + accessRequest.catalogueAccessRequestId" class="my-auto admin-link">{{accessRequest.fullName}}</router-link></td>
                                                <td>{{accessRequest.emailAddress}}</td>
                                                <td>{{accessRequest.requestedDatetime | formatDate('h:mm a DD MMM YYYY')}}</td>
                                                <td style="text-align: right;" class="pr-0">
                                                    <span v-if="accessRequest.catalogueAccessRequestStatus === AccessRequestStatus.Pending">
                                                        Pending<span class="fa-stack fa-1x">
                                                            <i class="fas fa-circle fa-stack-1x pending-circle"></i><i class="fas fa-ellipsis-h fa-stack-1x pending-ellipsis"></i>
                                                        </span>
                                                    </span>
                                                    <span v-if="accessRequest.catalogueAccessRequestStatus === AccessRequestStatus.Approved">Approved<span class="fa-stack fa-1x"><i class="fa-solid fa-circle-check fa-stack-1x approved-circle"></i></span></span>
                                                    <span v-if="accessRequest.catalogueAccessRequestStatus === AccessRequestStatus.Denied">
                                                        Denied<span class="fa-stack fa-1x">
                                                            <i class="fas fa-circle fa-stack-1x denied-circle"></i><i class="fas fa-times fa-stack-1x denied-times"></i>
                                                        </span>
                                                    </span>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="row pt-4 mt-2 d-md-none">
                                <div class="col">
                                    <div class="mobile-access-request mt-3 mb-3" v-for="(accessRequest, index) in filteredAccessRequests" :key="accessRequest.catalogueAccessRequestId">
                                        <div @click="toggleAccessRequest(accessRequest)" class="mobile-access-request p-10">
                                            <span style="cursor:pointer">
                                                <i v-if="!accessRequest.showExpandedDetails" class="fa fa-plus-circle toggle-circle mt-auto" aria-hidden="true"></i>
                                                <i v-if="accessRequest.showExpandedDetails" class="fa fa-minus-circle toggle-circle mt-auto" aria-hidden="true"></i>
                                                <span class="pt-1"><router-link :to="'/Catalogue/Manage/' + reference + '/AccessRequest/' + accessRequest.catalogueAccessRequestId" class="my-auto">{{accessRequest.fullName}}</router-link></span>
                                                <span class="float-right">
                                                    <span v-if="accessRequest.catalogueAccessRequestStatus === AccessRequestStatus.Pending">
                                                        Pending<span class="fa-stack fa-1x">
                                                            <i class="fas fa-circle fa-stack-1x pending-circle"></i><i class="fas fa-ellipsis-h fa-stack-1x pending-ellipsis"></i>
                                                        </span>
                                                    </span>
                                                    <span v-if="accessRequest.catalogueAccessRequestStatus === AccessRequestStatus.Approved">Approved<span class="fa-stack fa-1x"><i class="fa-solid fa-circle-check fa-stack-1x approved-circle"></i></span></span>
                                                    <span v-if="accessRequest.catalogueAccessRequestStatus === AccessRequestStatus.Denied">
                                                        Denied<span class="fa-stack fa-1x">
                                                            <i class="fas fa-circle fa-stack-1x denied-circle"></i><i class="fas fa-times fa-stack-1x denied-times"></i>
                                                        </span>
                                                    </span>
                                                </span>
                                            </span>
                                        </div>
                                        <div v-if="accessRequest.showExpandedDetails" class="mobile-user-details h-100 d-flex flex-column p-10">
                                            <div class="mobile-user-detail-header mb-2"><span>Email address</span></div>
                                            <div class="mb-2"><span>{{accessRequest.emailAddress}}</span></div>
                                            <div class="mobile-user-detail-header  mb-2"><span>Time / date requested</span></div>
                                            <div class="mb-2"><span>{{accessRequest.requestedDatetime | formatDate('hh:mm a DD MMM YYYY')}}</span></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="foldersTab" v-show="isActive('foldersTab')" class="node-contents-treeview">
                            <content-structure class="pb-5" />
                        </div>
                    </div>
                </div>
            </div>

            <div id="removeUserModal" class="modal remove-user-modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
                <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                    <div class="modal-content">
                        <div class="modal-header alert-modal-header text-center">
                            <h2 class="nhsuk-heading-l w-100"><i class="warningTriangle fa-solid fa-triangle-exclamation pr-3"></i>Remove user</h2>
                        </div>

                        <div class="modal-body alert-modal-body">
                            <div class="mt-3">The user will no longer have access to the resources in this catalogue with immediate effect.</div>
                        </div>

                        <div class="modal-footer alert-modal-footer">
                            <div class="form-group col-12 p-0 m-0">
                                <div class="d-flex button-pair">
                                    <input type="button" class="nhsuk-button nhsuk-button--secondary" data-dismiss="modal" value="Cancel" />
                                    <input type="button" class="nhsuk-button" @click="removeUser()" value="Continue" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="inviteUserModal" class="modal remove-user-modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
                <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                    <div class="modal-content">
                        <div class="modal-header alert-modal-header text-center">
                            <h2 class="nhsuk-heading-l w-100">Invite</h2>
                        </div>

                        <div class="modal-body alert-modal-body">
                            <div class="mt-3 mb-4">You can invite other users to request access to this Catalogue. They will receive an invite and instructions on how to access the Learning Hub and how to request access to this catalogue.</div>

                            <div class="form-group">
                                <div class="user-entry ml-0 pb-0" v-bind:class="{ 'input-validation-error': $v.inviteEmailAddress.$error}">
                                    <label class="mb-0" for="inviteEmailAddress">Email address</label>
                                    <div class="error-text pb-0 my-3" v-if="$v.inviteEmailAddress.$invalid && $v.inviteEmailAddress.$dirty">
                                        <span class="text-danger">{{returnError('inviteEmailAddress')}}</span>
                                    </div>
                                    <input type="email" id="inviteEmailAddress" class="form-control my-4" :class="{ 'input-validation-error': $v.inviteEmailAddress.$error}" maxlength="100" v-model="inviteEmailAddress" />
                                </div>
                            </div>

                        </div>

                        <div class="modal-footer alert-modal-footer">
                            <div class="form-group col-12 p-0 m-0">
                                <div class="d-flex button-pair">
                                    <input type="button" :class="{disabled: !inviteEmailAddress}" class="nhsuk-button" @click="inviteUser()" value="Send invite" />
                                    <input type="button" class="nhsuk-button nhsuk-button--secondary" data-dismiss="modal" value="Cancel" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="inviteUserSuccessModal" class="modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
                <div class="modal-dialog modal-dialog-centered modal-sm" role="document">
                    <div class="modal-content px-0">
                        <div class="modal-header alert-modal-header text-center">
                            <h2 class="nhsuk-heading-l"><i class="fa-solid fa-circle-check approved-circle pr-3"></i>Invitation sent</h2>
                        </div>
                        <div class="d-flex justify-content-center">
                            <input type="button" class="nhsuk-button" data-dismiss="modal" value="Close" />
                        </div>
                    </div>
                </div>
            </div>

        </div>

    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Vuelidate from "vuelidate";
    import { required, email } from "vuelidate/lib/validators";
    import { catalogueData } from '../data/catalogue';
    import { userData } from '../data/user';
    import contentStructure from '../content-structure-admin/contentStructure.vue';
    import { RoleUserGroupModel } from '../models/roleUserGroupModel';
    import { RestrictedCatalogueUsersRequestModel } from '../models/catalogue/restrictedCatalogueUsersRequestModel';
    import { RestrictedCatalogueUserModel } from '../models/catalogue/restrictedCatalogueUserModel';
    import { RestrictedCatalogueAccessRequestsRequestModel } from '../models/catalogue/restrictedCatalogueAccessRequestsRequestModel';
    import { RestrictedCatalogueAccessRequestModel } from '../models/catalogue/restrictedCatalogueAccessRequestModel';
    import { RestrictedCatalogueAccessRequestFilterModel } from '../models/catalogue/restrictedCatalogueAccessRequestFilterModel';
    import { RestrictedCatalogueSummaryModel } from '../models/catalogue/RestrictedCatalogueSummaryModel';
    import { RestrictedCatalogueInviteUserRequestModel } from '../models/catalogue/restrictedCatalogueInviteUserRequestModel';
    import '../filters';
    import { CatalogueModel } from '../models/catalogueModel';
    import { AccessRequestStatus, RoleEnum } from "../constants";

    Vue.use(Vuelidate as any);

    export default Vue.extend({
        components: {
            'contentStructure': contentStructure
        },
        data() {
            return {
                roleUserGroups: [] as RoleUserGroupModel[],
                AccessRequestStatus: AccessRequestStatus,
                isLoading: true,
                isLoadingUsers: false,
                contentStructureLoaded: false,
                showLoadingUsers: false,
                disableLoadMore: false,
                loadMoreUsersButtonText: '',
                activeTab: 'accessRequestsTab',
                catalogue: new CatalogueModel(),
                summary: new RestrictedCatalogueSummaryModel(),
                users: [] as RestrictedCatalogueUserModel[],
                selectedUser: null as RestrictedCatalogueUserModel,
                accessRequests: [] as RestrictedCatalogueAccessRequestModel[],
                emailAddressFilter: '',
                includeCatalogueAdmins: true,
                includePlatformAdmins: false,
                accessRequestsFilter: { includePending: true, includeApproved: false, includeDenied: false } as RestrictedCatalogueAccessRequestFilterModel,
                inviteEmailAddress: '',
                skip: 0,
                take: 10,
                userCount: 0
            };
        },
        created: async function () {
            await this.loadCatalogue()
            await this.loadRoleUserGroups();
            if (this.isCatalogueLocalAdmin && this.catalogue.restrictedAccess) {
                await this.loadSummary();
                await this.loadAccessRequests();
                await this.loadUsers();
            }
            this.isLoading = false;
        },
        computed: {
            reference: function (): string {
                return this.$route.params.reference;
            },
            tabs: function(): Array<any>{
                return [
                    { id: "accessRequestsTab", name: "Access requests (" + this.summary.accessRequestCount + ")" },
                    { id: "usersTab", name: "Users (" + this.summary.userCount + ")" },
                    { id: "foldersTab", name: "Folders" },
                ];
            },
            activeTabName: function (): string {
                return this.tabs.filter(item => item.id === this.activeTab)[0].name;
            },
            filteredAccessRequests: function (): RestrictedCatalogueAccessRequestModel[] {
                return this.accessRequests.filter(item => (item.catalogueAccessRequestStatus === AccessRequestStatus.Pending && this.accessRequestsFilter.includePending)
                    || (item.catalogueAccessRequestStatus === AccessRequestStatus.Approved && this.accessRequestsFilter.includeApproved)
                    || (item.catalogueAccessRequestStatus === AccessRequestStatus.Denied && this.accessRequestsFilter.includeDenied)
                );
            },
            accessRequestCount: function (): number {
                return this.summary.accessRequestCount;
            },
            url: function (): String {
                return '/Catalogue/Manage/' + this.reference;
            },
            isCatalogueLocalAdmin(): boolean {
                return (this.roleUserGroups.some(rug => rug.catalogueNodeId === this.catalogue.nodeId
                                                          && rug.roleEnum === RoleEnum.LocalAdmin));
            }
        },
        validations: {
            inviteEmailAddress: { required, email }
        },
        methods: {
            toggleUserDetails(user: RestrictedCatalogueUserModel) {
                user.showExpandedDetails = !user.showExpandedDetails;
            },
            toggleAccessRequest(accessRequest: RestrictedCatalogueAccessRequestModel) {
                accessRequest.showExpandedDetails = !accessRequest.showExpandedDetails;
            },
            activateTab: function (val: string) {
                this.activeTab = val;

                // Only load content structure data when tab first selected.
                if (val === "foldersTab" && !this.contentStructureLoaded) {
                    this.loadContentStructure()
                        .then(response => this.contentStructureLoaded = true);
                }
            },
            isActive: function (val: string): Boolean {
                return this.activeTab === val
            },
            async loadRoleUserGroups() {
                this.roleUserGroups = await userData.getRoleUserGroups();
            },
            async loadSummary() {
                await catalogueData.getSummary(this.catalogue.nodeId)
                    .then(response => { this.summary = response; }).catch(e => console.log(e))
            },
            async loadAccessRequests() {
                this.skip = 0;
                await catalogueData.getAccessRequests(this.getAccessRequestsRequestModel())
                    .then(response => {
                        this.accessRequests = response;
                        this.accessRequests.forEach(function (u) {
                            Vue.set(u, 'showExpandedDetails', false);
                        });
                    }).catch(e => console.log(e))
            },
            getAccessRequestsRequestModel() {
                let requestModel = new RestrictedCatalogueAccessRequestsRequestModel();

                requestModel.catalogueNodeId = this.catalogue.nodeId;
                requestModel.includeNew = true;
                requestModel.includeApproved = true;
                requestModel.includeDenied = true;

                return requestModel;
            },
            searchUsersKeyUp(keycode: number) {
                if (keycode === 13 /* enter */ || !this.emailAddressFilter) {
                    this.loadUsers();
                }
            },
            async loadCatalogue() {
                await catalogueData.getCatalogue(this.reference)
                    .then(response => {
                        this.catalogue = response;

                        if (!this.catalogue.restrictedAccess) {
                            this.activateTab("foldersTab");
                        }
                    })
                    .catch(e => console.log(e))
            },
            async loadContentStructure() {
                this.$store.dispatch('contentStructureState/populateCatalogue', this.catalogue.id);
            },
            async loadUsers() {
                setTimeout(() => { if (this.isLoadingUsers) this.showLoadingUsers = true }, 1000);
                this.isLoadingUsers = true;
                this.skip = 0;
                await catalogueData.getUsers(this.getUsersRequestModel())
                    .then(response => {
                        this.isLoadingUsers = false;
                        this.showLoadingUsers = false;
                        this.users = response.restrictedCatalogueUsers;
                        this.users.forEach(function (u) {
                            Vue.set(u, 'showExpandedDetails', false);
                        });
                        this.userCount = response.userCount;
                        this.updateLoadMoreUsersButtonText();
                    }).catch(e => console.log(e))
            },
            async loadMoreUsers() {
                this.disableLoadMore = true;
                this.skip += this.take;
                await catalogueData.getUsers(this.getUsersRequestModel())
                    .then(response => {
                        this.users = this.users.concat(response.restrictedCatalogueUsers);
                        this.userCount = response.userCount;
                        this.updateLoadMoreUsersButtonText();
                    }).catch(e => console.log(e))
                this.disableLoadMore = false;
            },
            updateLoadMoreUsersButtonText () {
                let remaining = this.userCount - (this.skip + 10);
                remaining = Math.min(10, remaining);
                this.loadMoreUsersButtonText = `Load ${remaining} more`;
            },
            getUsersRequestModel() {
                let requestModel = new RestrictedCatalogueUsersRequestModel();

                requestModel.catalogueNodeId = this.catalogue.nodeId;
                requestModel.emailAddressFilter = this.emailAddressFilter;
                requestModel.includeCatalogueAdmins = this.includeCatalogueAdmins;
                requestModel.includePlatformAdmins = this.includePlatformAdmins;

                requestModel.skip = this.skip;
                requestModel.take = this.take;

                return requestModel;
            },
            removeUserModal(user: RestrictedCatalogueUserModel) {
                this.selectedUser = user;
                $('#removeUserModal').modal('show');
            },
            async removeUser() {
                await catalogueData.removeUserFromRestrictedAccess(this.selectedUser.userUserGroupId)
                    .then(response => {
                        this.users.splice(this.users.indexOf(this.selectedUser), 1);
                        this.userCount--;
                        this.summary.userCount--;
                        this.skip--;
                        $('#removeUserModal').modal('hide');
                    }).catch(e => console.log(e))
            },
            inviteUserModal(user: RestrictedCatalogueUserModel) {               
                this.inviteEmailAddress = '';
                $('#inviteUserModal').modal('show');
            },
            async inviteUser() {
                if (this.$v.inviteEmailAddress.$invalid) {
                    this.$v.inviteEmailAddress.$touch();
                }
                else {
                    let requestModel = new RestrictedCatalogueInviteUserRequestModel();
                    requestModel.catalogueNodeId = this.catalogue.nodeId;
                    requestModel.emailAddress = this.inviteEmailAddress;

                    await catalogueData.inviteUser(requestModel)
                        .then(response => {
                            $('#inviteUserModal').modal('hide');
                            $('#inviteUserSuccessModal').modal('show');
                        }).catch(e => console.log(e))
                }
            },
            returnError(ctrl: string) {
                var errorMessage = '';
                switch (ctrl) {
                    case 'inviteEmailAddress':
                        if (this.$v.inviteEmailAddress.$invalid) {
                            if (!this.$v.inviteEmailAddress.required) {
                                errorMessage = "Enter an email address";
                            }
                            if (!this.$v.inviteEmailAddress.email) {
                                errorMessage = "Enter an email address in the correct format";
                            }
                        }
                        break;
                }
                return errorMessage;
            },
            errorClass(isError: boolean) {
                if (isError) {
                    return 'input-validation-error';
                } else {
                    return '';
                }
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .d-flex.button-pair {
        justify-content:space-around;
    }
    
    .btn-red{
        background-color: $nhsuk-red !important;
        border-color: $nhsuk-red !important;

        &:hover{
            background-color: $nhsuk-red-hover !important;
            border-color: $nhsuk-red-hover !important;
        }
    }

    .btn.btn-outline-secondary.btn-search {
        line-height:1.5;
        width:auto;
        padding:0 8px;
        font-size:16px;
    }

    .mobile-access-request {
        border: 1px solid $nhsuk-grey-light !important;
    }

    .mobile-user {
        border: 1px solid $nhsuk-grey-light !important;
    }

    .mobile-user-details {
        border-top: 1px solid $nhsuk-grey-light !important;
    }

    .mobile-user-detail-header {
        font-weight: bold;
    }

    .manage-catalogue-container {
        background: #fff;
    }

    .remove-user {
        cursor: pointer;
        color: $nhsuk-red;
    }

    .lock-user {
        color: $nhsuk-grey;
    }

    .alert-modal-header {
        padding-bottom: 0px !important;
    }

    .btn-admin-action.disabled{
        background-color: #425563 !important;
        border-color: #425563 !important;
    }

    .alert-modal-body {
        background-color: $nhsuk-grey-white;
        /*font-size: 1.9rem;*/
        min-height: 7rem;
        border-radius: .5rem;
        padding: 2.5rem !important;
        p

    {
        padding: 0;
        margin: 0;
    }

    margin-top: 2rem;
    margin-bottom: 2rem;
    }

    .alert-modal-footer {
        padding: 0px !important;
    }

    .alert-modal-footer .form-group .nhsuk-button--reverse {
        border-color:#384853;
    }

    .fa-exclamation-triangle {
        color: $nhsuk-warm-yellow;
    }

    #tabs {
        .a {
            color: $nhsuk-black;
            text-decoration: none;
        }

        li.active {
            font-family: $font-stack-bold !important;
            border-bottom: 3px solid $nhsuk-blue;
        }

        ul {
            border-bottom: 1px solid $nhsuk-grey-white;
            display: flex;
            flex-direction: row;
            padding-left: 0;
        }

        li {
            list-style: none;
            margin-right: 24px;
            p {
                margin-bottom: 0;
            }
        }
    }

    .checkContainer-medium {
        font-family: FrutigerLTW01-65, Arial, sans-serif;
        font-size: 16px;
    }

    .approved-circle {
        color: $nhsuk-green;
        font-size: 2.5rem;
    }

    .pending-circle {
        color: $nhsuk-warm-yellow;
        font-size: 2.5rem;
    }

    .pending-ellipsis {
        color: $nhsuk-white;
    }

    .denied-circle {
        color: $nhsuk-red;
        font-size: 2.5rem;
    }

    .denied-times {
        color: $nhsuk-white;
    }

    .toggle-circle {
        color: $nhsuk-blue;
        font-size: 2rem;
    }

    .subnavwhite-link:focus {
        outline: none;
    }

    .admin-link {
        text-decoration: none;
    }

    .admin-link:hover {
        text-decoration: underline;
    }

    .user-entry.input-validation-error {
        border-left: 3px solid $nhsuk-red;
        padding-left: 15px;
    }

    @media(max-width: 576px){
        .modal-footer > *{margin: 0}
        .modal-footer{
        padding-left: 0 !important;
        padding-right: 0 !important;
    }
        .modal-dialog{
            min-height: 100%;
            margin: 0;
            .modal-content{
                padding: 16px;
                border-radius: 0;
                height: 100%;
                .modal-body{
                    padding: 0;
                }
                .modal-header{
                    
                    padding: 0;
                    padding-bottom: 1rem;
                }
            }
        }
        .modal{
            padding-right: 0!important;
        }
    }

    .node-contents-treeview {
        overflow-x: auto;
        display: block;
        max-width: 1145px;
        margin-left: auto;
        margin-right: auto;
    }

    @media (max-width: 768px) {
        .mobile-user {
            font-size: 14px;
        }
        .mobile-access-request {
            font-size: 14px;
        }
        label.checkContainer-medium {
            padding-top: 2px;
            font-size: 14px;
        }
    }
</style>