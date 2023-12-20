<template>
    <div class="panel-card-container">

        <div tabindex="0" class="my-contributions-panel-card h-100">

            <div class="h-100 d-flex flex-column">
                <div class="d-flex flex-row justify-content-start my-4">
                    <div class="d-flex ml-4">
                        <span :class="headerVersionStatusClass">{{ headerVersionStatusDescription }}</span>
                        <a class="ml-2 mt-1" href="#" v-if="this.carditem.versionStatusEnum === this.CardVersionStatus.FAILED" @click="contactSupport()">Contact support</a>
                    </div>
                    <div v-if="isPublishedAndBeingEdited" class="ml-4 header-status-info edits-pending">Editing...</div>
                </div>

                <span class="nhsuk-u-font-weight-bold mx-4">{{ carditem.title | truncate(50, '...') }}</span>
                <div class="mx-4 my-2 nhsuk-u-font-size-14"><i :class="getResourceTypeIconClass()"></i> {{carditem.resourceTypeDescription}} | {{carditem.contributedOn}}</div>

                <div class="d-flex flex-row align-self-start mt-auto mb-4 mx-4">
                    <Button v-if="canView" size="medium" v-on:click="view()">View</Button>
                    <Button v-if="carditem.hasValidationErrors" size="medium" v-on:click="viewValidationErrors()" class="nhsuk-u-margin-right-3">View Errors</Button>
                    <Button v-if="canDuplicate && !isDuplicatingResource"
                            size="medium"
                            v-on:click="duplicate()"
                            :class="getDuplicateButtonClass"
                            :color="getDuplicationButtonColor"
                            :disabled="!isResourceToDuplicate && isResourceDuplicationSuccess">
                        {{ this.getDuplicateButtonText }}
                    </Button>
                    <Button v-if="isDuplicateButtonDisabled"
                            size="medium"
                            :class="getDuplicateButtonClass"
                            disabled>
                        Duplicate
                    </Button>
                    <Button v-if="isDuplicateButtonActive" size="medium" :class="getDuplicateButtonClass">
                        <Spinner />
                    </Button>
                    <Button v-if="canEdit" :class="getEditButtonClass" size="medium" v-on:click="edit()">Edit</Button>
                    <Button v-if="canDelete" size="medium" v-on:click="confirmDeleteResource()">Delete</Button>
                </div>

            </div>

        </div>

        <!-- Resource deletion modal -->
        <div v-if="showDeleteConfirm" id="deleteResourceVersion" ref="deleteResourceVersionModal" class="modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
            <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                <div class="modal-content">
                    <div class="modal-header text-center">
                        <h2 v-if="this.carditem.resourceReferenceId == 0">Delete this draft</h2>
                        <h2 v-else>Delete changed draft</h2>
                    </div>

                    <div class="modal-body">
                        <p>This will no longer be available.</p>
                        <p v-if="this.carditem.resourceReferenceId != 0">If a previously published copy of this resource exists, this will continue to be available.</p>
                    </div>

                    <div class="modal-footer">
                        <button class="nhsuk-button nhsuk-button--secondary" data-dismiss="modal">Cancel</button>
                        <button class="nhsuk-button nhsuk-button--red" @click="onDeleteResourceVersion">Continue</button>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    
    import { MyContributionsCardModel } from '../models/contribute/mycontributionsCardModel';
    import Button from '../globalcomponents/Button.vue';
    import Spinner from '../globalcomponents/Spinner.vue';

    import { VersionStatus, ResourceType } from '../constants';
    import { commonlib } from '../common';

    import SupportUrls from '../data/supportUrls';

    export default Vue.extend({
        name: 'cardcomp',
        props: {
            carditem: { type: Object } as PropOptions<MyContributionsCardModel>,
            isDuplicatingResource: Boolean,
            isResourceDuplicationSuccess: Boolean,
        },
        data() {
            return {
                CardVersionStatus: VersionStatus,
                duplicateResourceId: 0,
                showDeleteConfirm: false,
            };
        },
        components: {
            Button,
            Spinner,
        },
        computed: {
            isPublishedAndBeingEdited(): boolean {
                return this.carditem.inEdit && !this.carditem.actionRequired && this.carditem.versionStatusEnum !== this.CardVersionStatus.DRAFT
            },
            canView(): boolean {
                return (this.carditem.versionStatusEnum === this.CardVersionStatus.PUBLISHED
                    || this.carditem.versionStatusEnum === this.CardVersionStatus.UNPUBLISHED);
            },
            canDuplicate(): boolean {
                return this.isCaseOrAssessmentResourceType
                    && (this.carditem.versionStatusEnum === this.CardVersionStatus.DRAFT
                        || this.carditem.versionStatusEnum === this.CardVersionStatus.PUBLISHED
                        || this.carditem.versionStatusEnum === this.CardVersionStatus.UNPUBLISHED)
                    && !this.isPublishedAndBeingEdited;
            },
            canEdit(): boolean {
                return !this.$store.state.myContributionsState.isReadonly
                    && !this.carditem.readOnly
                    && !(this.carditem.versionStatusEnum === this.CardVersionStatus.PUBLISHING)
                    && !(this.carditem.versionStatusEnum === this.CardVersionStatus.SUBMITTED)
                    && !(this.carditem.versionStatusEnum === this.CardVersionStatus.FAILED);
            },
            canDelete(): boolean {
                return !this.$store.state.myContributionsState.isReadonly
                    && !this.carditem.readOnly
                    && this.carditem.versionStatusEnum === this.CardVersionStatus.FAILED;
            },
            headerVersionStatusClass: function (): any {
                var classes = ["header-status-info",
                    (this.carditem.draftCategory && !(this.carditem.versionStatusEnum === this.CardVersionStatus.PUBLISHING)) ? " header-draft" : "",
                    (this.carditem.publishedCategory && !(this.carditem.versionStatusEnum === this.CardVersionStatus.PUBLISHING || this.carditem.versionStatusEnum === this.CardVersionStatus.SUBMITTED)) ? " header-published" : "",
                    (this.carditem.versionStatusEnum === this.CardVersionStatus.PUBLISHING || this.carditem.versionStatusEnum === this.CardVersionStatus.SUBMITTED) ? " header-publishing" : "",
                    (this.carditem.unpublishedCategory && !this.carditem.unpublishedByAdmin) ? " header-unpublished" : "",
                    (this.carditem.unpublishedCategory && this.carditem.unpublishedByAdmin) ? " header-unpublished-admin" : "",
                    (this.carditem.actionRequired && this.CardVersionStatus.FAILED) ? " header-failed" : "",
                ];
                return classes;
            },
            headerVersionStatusDescription: function (): string {
                var retVal;

                if (this.carditem.draftCategory && !(this.carditem.versionStatusEnum === this.CardVersionStatus.PUBLISHING)) {
                    retVal = 'Draft';
                }
                else if (this.carditem.publishedCategory && !(this.carditem.versionStatusEnum === this.CardVersionStatus.PUBLISHING || this.carditem.versionStatusEnum === this.CardVersionStatus.SUBMITTED)) {
                    retVal = 'Published';
                }
                else if (this.carditem.versionStatusEnum === this.CardVersionStatus.PUBLISHING || this.carditem.versionStatusEnum === this.CardVersionStatus.SUBMITTED) {
                    retVal = "Publishing ...";
                }
                else if (this.carditem.unpublishedCategory && !this.carditem.unpublishedByAdmin) {
                    retVal = 'Unpublished';
                }
                else if (this.carditem.unpublishedCategory && this.carditem.unpublishedByAdmin) {
                    retVal = 'Unpublished by Admin';
                }
                else if (this.carditem.versionStatusEnum === this.CardVersionStatus.FAILED) {
                    retVal = 'Failed to publish';
                }
                else {
                    retVal = "Invalid State"
                };
                return retVal;
            },
            actionRequiredIconClass: function (): any {
                if (this.carditem.versionStatusEnum === this.CardVersionStatus.UNPUBLISHED && this.carditem.unpublishedByAdmin)
                    return ["fas", "fa-exclamation-circle", "resource-important-icon", "m-2"];
                else if (this.carditem.flaggedByAdmin)
                    return ["fas", "fa-flag", "resource-important-icon", "m-2"];
                else if (this.carditem.isFlagged)
                    return ["fas", "fa-flag", "resource-important-icon", "m-2"];
                else
                    return "";
            },
            actionRequiredDescription: function (): string {
                if (this.carditem.versionStatusEnum === this.CardVersionStatus.UNPUBLISHED && this.carditem.unpublishedByAdmin)
                    return "Unpublished by Admin";
                else if (this.carditem.flaggedByAdmin)
                    return "Flagged by Admin";
                else if (this.carditem.isFlagged)
                    return "Flagged by learner";
                else if (this.carditem.isFlagged)
                    return "Flagged by learner";
                else
                    return "";
            },
            isResourceToDuplicate: function(): boolean {
                if (this.carditem.versionStatusEnum === VersionStatus.PUBLISHED || this.carditem.versionStatusEnum === VersionStatus.UNPUBLISHED) {
                    return this.duplicateResourceId === this.carditem.resourceReferenceId
                } 
                
                if (this.carditem.versionStatusEnum === VersionStatus.DRAFT) {
                    return this.duplicateResourceId === this.carditem.draftResourceVersionId
                }
                
                return false;
            },
            getDuplicateButtonText: function(): string {
                return this.isResourceToDuplicate && this.isResourceDuplicationSuccess === false ? 'Retry' : 'Duplicate'; 
            },
            getDuplicationButtonColor: function(): string {
                if (this.isResourceToDuplicate && this.isResourceDuplicationSuccess === false) {
                    return 'red';
                }
                
                if (this.isResourceToDuplicate && this.isResourceDuplicationSuccess) {
                    return 'green';
                }
                
                return '';
            },
            isDuplicateButtonDisabled: function(): boolean {
                return this.canDuplicate && !this.isResourceToDuplicate && this.isDuplicatingResource;
            },
            isDuplicateButtonActive: function(): boolean {
                return this.canDuplicate && this.isResourceToDuplicate && this.isDuplicatingResource;
            },
            isCaseOrAssessmentResourceType: function(): boolean {
                return this.carditem.resourceTypeEnum === ResourceType.CASE
                    || this.carditem.resourceTypeEnum === ResourceType.ASSESSMENT;
            },
            isDraftCaseOrAssessmentResourceType: function(): boolean {
                return this.isCaseOrAssessmentResourceType 
                    && this.carditem.versionStatusEnum === VersionStatus.DRAFT;
            },
            isPublishedCaseOrAssessmentResourceType: function(): boolean {
                return this.isCaseOrAssessmentResourceType
                    && this.carditem.versionStatusEnum === VersionStatus.PUBLISHED;
            },
            isUnpublishedCaseOrAssessmentResourceType: function(): boolean {
                return this.isCaseOrAssessmentResourceType
                    && this.carditem.versionStatusEnum === VersionStatus.UNPUBLISHED;
            },
            getEditButtonClass: function(): string {
                if (this.carditem.versionStatusEnum === VersionStatus.PUBLISHED
                    || this.carditem.versionStatusEnum === VersionStatus.UNPUBLISHED
                    || this.isDraftCaseOrAssessmentResourceType) {
                    return 'nhsuk-u-margin-left-3';
                }
                
                return '';
            },
            getDuplicateButtonClass: function(): string {
                if (this.isDraftCaseOrAssessmentResourceType) {
                    return '';
                }
                
                if (this.isPublishedCaseOrAssessmentResourceType || this.isUnpublishedCaseOrAssessmentResourceType) {
                    return 'nhsuk-u-margin-left-3';
                }
                
                return '';
            },
        },
        methods: {
            getResourceTypeIconClass(): any {
                return commonlib.getResourceTypeIconClass(this.carditem.resourceTypeEnum);
            },
            edit() {
                if (this.carditem.inEdit) {
                    var resourceVersionIdString = this.carditem.draftResourceVersionId.toString();

                    if (this.carditem.resourceTypeEnum === ResourceType.CASE || this.carditem.resourceTypeEnum === ResourceType.ASSESSMENT) {
                        window.location.pathname = './contribute-resource/edit/' + resourceVersionIdString;
                    }
                    else {
                        window.location.pathname = './Contribute/contribute-a-resource/' + resourceVersionIdString;
                    }
                }
                else {
                    this.$emit('createedit', this.carditem.resourceId);
                }
            },
            view() {
                window.location.pathname = './Resource/' + this.carditem.resourceReferenceId.toString() + '/Item';
            },
            viewValidationErrors(){
                window.location.pathname = `./Resource/${this.carditem.resourceVersionId.toString()}/ValidationErrors`;
            },
            duplicate() {
                if (this.carditem.versionStatusEnum === VersionStatus.PUBLISHED || this.carditem.versionStatusEnum === VersionStatus.UNPUBLISHED) {
                    this.duplicateResourceId = this.carditem.resourceReferenceId;
                }

                if (this.carditem.versionStatusEnum === VersionStatus.DRAFT) {
                    this.duplicateResourceId = this.carditem.draftResourceVersionId;
                }

                this.$emit('createDuplicate', this.carditem);
            },
            confirmDeleteResource() {
                // Using v-if to add popup to DOM on request, otherwise it would be repeated for every catalogue item. 
                // nextTick - Need to wait for popup to be added to DOM before showing it.
                this.showDeleteConfirm = true;
                Vue.nextTick(() => {
                    $("#deleteResourceVersion").modal("show");
                });
            },
            async onDeleteResourceVersion() {
                $("#deleteResourceVersion").modal("hide");
                this.showDeleteConfirm = false;

                this.$emit('deleteResourceVersion', this.carditem);
            },
            contactSupport() {
                window.open(SupportUrls.contributingAResource, "_blank");
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .header-status-info {
        vertical-align: top;
        padding-left: 10px;
        padding-right: 10px;
        padding-bottom: 4px;
        padding-top: 4px;
        min-width: 100px;
        font-family: FrutigerLight;
        color: $nhsuk-white;
        text-align: center;
    }

    .header-draft {
        background-color: $nhsuk-grey;
    }

    .header-published {
        background-color: $nhsuk-blue;
    }

    .header-publishing {
        background-color: $nhsuk-warm-yellow;
        color: $nhsuk-black;
    }

    .header-unpublished {
        background-color: $nhsuk-grey;
    }

    .header-unpublished-admin {
        background-color: $nhsuk-white;
        color: $nhsuk-red;
        border: 1px solid $nhsuk-red;
    }

    .header-failed {
        background-color: $nhsuk-red;
        color: $nhsuk-white;
    }

    .heading-xs {
        font-size: 1.8rem;
    }

    .backgroundImageText {
        color: $nhsuk-white !important;
    }

    .backgroundImageRule {
        border-color: $nhsuk-white !important;
    }

    .resource-important-icon {
        color: #DA291C;
    }

    .edits-pending-icon {
        color: $nhsuk-blue;
    }

    .action-required {
        color: $nhsuk-red;
        border: 1px solid $nhsuk-red;
    }

    .edits-pending {
        background-color: $nhsuk-warm-yellow;
        color: $nhsuk-black;
    }

    .btn-action {
        background-color: $nhsuk-white;
        color: $nhsuk-blue;
        border: 1px solid $nhsuk-blue;
        width: 80px;
        height: 40px;
        font-size: 1.7rem !important;
        text-align: center !important;
        border-radius: 5px;
    }
</style>