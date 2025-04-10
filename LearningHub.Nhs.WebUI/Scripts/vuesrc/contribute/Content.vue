<template>

    <div class="">
        <div v-if="hierarchyEditLoaded && catalogueLockedForEdit" class="mt-4">
            <div class="lh-padding-fluid bg-grey-white">
                <div class="lh-container-xl">
                    <div class="d-flex flex-row">
                        <i class="fa-solid fa-triangle-exclamation mr-4 mt-2 text-warning" aria-hidden="true"></i>
                        <div class="small">A Learning Hub system administrator is currently making changes to this catalogue. You can browse the catalogue but cannot add / edit or move resources at this time.</div>
                    </div>
                </div>
            </div>
        </div>

        <div v-if="hierarchyEditLoaded && !catalogueLockedForEdit">
            <div class="lh-padding-fluid white-background nhsuk-u-font-size-19">
                <div class="lh-container-xl">
                    <contributeHeaderComponent></contributeHeaderComponent>
                    <div class="limit-width px-xl-0 mx-xl-0">
                        <div class="row">
                            <div class="form-group col-12 mt-5">
                                <h2 id="title-label" class="nhsuk-heading-l">Title<i v-if="$v.resourceDetailTitle.$invalid" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h2>
                                <div class="mb-3"><label for="resourceDetail_title">Give your resource a concise, useful title that will make sense to learners.</label></div>
                                <input type="text" class="form-control" aria-labelledby="title-label" maxlength="255" id="resourceDetail_title" name="resourceDetail_title" v-model="resourceDetailTitle" @change="setTitle($event.target.value)" autocomplete="off" v-bind:disabled="resourceLoading">
                            </div>
                        </div>
                        <div class="row mt-4">
                            <div class="col-12">
                                <h2 v-if="!resourceLoading" id="type-label" class="nhsuk-heading-l">Type<i v-if="selectedResourceType == resourceType.UNDEFINED" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h2>
                                <h2 v-if="resourceLoading" class="nhsuk-heading-l"><i class="fa fa-spinner fa-spin"></i> Loading, please wait.</h2>
                            </div>
                        </div>
                        <div v-show="!resourceLoading && (selectedResourceType == resourceType.UNDEFINED)">
                            <div class="row my-2">
                                <div class="accordion col-12" id="accordion">
                                    <div class="pt-0 pb-4">
                                        <div class="heading" id="headingOne">
                                            <div class="mb-0">
                                                <a href="#" class="collapsed text-decoration-skip" data-toggle="collapse" data-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                                                    <div class="accordion-arrow">What type of resource should I select?</div>
                                                </a>
                                            </div>
                                        </div>
                                        <div id="collapseOne" class="collapse" aria-labelledby="headingOne" data-parent="#accordion">
                                            <div class="content col-12">
                                                <p>
                                                    <span class="title">elearning package (SCORM 1.2)</span><br />
                                                    If you want to add an elearning package in a zip file that is
                                                    in SCORM format, version 1.2.
                                                </p>
                                                <p>
                                                    <span class="title">File upload</span><br />
                                                    If you have a file stored on your computer or storage device that you want to add, for example a Word document, image, video or audio file.
                                                </p>
                                                <p>
                                                    <span class="title">Article</span><br />
                                                    If you want to publish text on a topic and attach associated files.
                                                </p>
                                                <p>
                                                    <span class="title">Web link</span><br />
                                                    If you have a link to a website that you want to share, for example https://www.nhs.uk
                                                </p>
                                                <p>
                                                    <span class="title">HTML</span><br />
                                                    If you want to add an HTML resource package in a zip file.
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group col-12">
                                    <select class="form-control" aria-labelledby="type-label" id="uploadResourceTypes" v-model="selectUploadResourceType" @change="onUploadResourceTypeChange">
                                        <option disabled v-bind:value="0">Please choose...</option>
                                        <option v-for="option in uploadResourceTypes" :value="option.id">
                                            {{ option.description }}
                                        </option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div v-if="!resourceLoading && resourceDetail && selectedResourceType != resourceType.UNDEFINED" class="row pb-5">
                            <div class="col-12 resource-area-container">
                                <div class="resource-area-header">
                                    <div class="nhsuk-u-font-size-24">{{resourceTypeDescription}}</div>
                                    <div>
                                        <button class="btn btn-link" @click="clearResourceType" v-if="!previousVersionExists">Change resource type <i class="fa-regular fa-pen-to-square"></i></button>
                                    </div>
                                </div>
                                <div class="resource-area-body"
                                     v-bind:class="{ 'resource-area-body-file-upload': (isGenericFileUploaderVisible)}"
                                     v-if="!resourceLoading">
                                    <GenericFileUploader ref="genericFileUpload"
                                                         v-show="isGenericFileUploaderVisible"
                                                         v-bind:onResourceFileChange="onResourceFileChange">

                                    </GenericFileUploader>
                                    <content-article v-if="selectedResourceType == resourceType.ARTICLE"
                                                     @childfileuploaderror="childFileUploadError"
                                                     @confirmdeletefile="confirmDeleteFile"
                                                     @isvalid="setSpecificContentLocalValid">
                                    </content-article>
                                    <content-audio v-if="selectedResourceType == resourceType.AUDIO && !isGenericFileUploaderVisible"
                                                   @filechanged="fileChanged"
                                                   @childfileuploaderror="childFileUploadError"
                                                   @confirmdeletefile="confirmDeleteFile">
                                    </content-audio>
                                    <content-embedded v-if="selectedResourceType == resourceType.EMBEDDED"></content-embedded>
                                    <content-equipment v-if="selectedResourceType == resourceType.EQUIPMENT"></content-equipment>
                                    <content-generic-file v-if="selectedResourceType == resourceType.GENERICFILE && !isGenericFileUploaderVisible"
                                                          @filechanged="fileChanged">
                                    </content-generic-file>
                                    <div v-if="selectUploadResourceType == uploadResourceType.SCORM">
                                        <div v-show="this.scormDetail.file === null || (this.scormDetail.file && this.scormDetail.file.fileId === 0)">
                                            <span>
                                                You can upload a zip file that contains a SCORM 1.2 elearning package from your
                                                computer or other storage drive you are connected to. Maximum file size {{contributeSettings.fileUploadSettings.fileUploadSizeLimitText}}
                                            </span>
                                            <div class="mb-4 uploadBox">
                                                <div class="row">
                                                    <div class="form-group col-12 mb-0">
                                                        <!--<h3 id="content-label">Content<i v-if="$v.articleContent.$invalid" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h3>-->
                                                    </div>
                                                </div>
                                                <div class="p-4 uploadInnerBox mt-4">
                                                    <div class="upload-btn-wrapper nhsuk-u-font-size-16">
                                                        <label for="fileUpload" class="nhsuk-button nhsuk-button--secondary">Choose file</label> No file chosen
                                                        <input hidden type="file" id="fileUpload" aria-label="Choose file" ref="fileUpload" @change="onScormPackageFileChange($event)" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <input type="file" style="display:none" id="fileUploadScorm" aria-label="Choose file" ref="fileUploadScorm" @change="onScormPackageFileChange($event)" />
                                        <content-scorm @filechanged="fileChangedScorm" @isvalid="setSpecificContentLocalValid">
                                        </content-scorm>
                                    </div>
                                    <content-image v-if="selectedResourceType == resourceType.IMAGE && !isGenericFileUploaderVisible"
                                                   @filechanged="fileChanged"
                                                   @isvalid="setSpecificContentLocalValid">
                                    </content-image>
                                    <content-video v-if="selectedResourceType == resourceType.VIDEO && !isGenericFileUploaderVisible"
                                                   @filechanged="fileChanged"
                                                   @childfileuploaderror="childFileUploadError"
                                                   @confirmdeletefile="confirmDeleteFile">
                                    </content-video>
                                    <content-weblink v-if="selectedResourceType == resourceType.WEBLINK"
                                                     @isvalid="setSpecificContentLocalValid">
                                    </content-weblink>
                                    <content-html v-if="selectedResourceType == resourceType.HTML"
                                                  v-bind:onResourceFileChange="onResourceFileChange"
                                                  @filechanged="fileChanged"
                                                  @isvalid="setSpecificContentLocalValid">
                                    </content-html>
                                </div>
                            </div>
                        </div>
                        <content-access v-if="!resourceLoading && resourceDetail" @isvalid="commonContentValid"></content-access>
                        <content-common :key="commonContentKey" v-if="!resourceLoading && resourceDetail" ref="contentCommon" @isvalid="commonContentValid"></content-common>

                        <file-upload :resource-version-id="resourceVersionId"
                                     :resource-type="selectedResourceType"
                                     ref="fileUploader"
                                     @fileuploadcomplete="fileUploadComplete"
                                     @fileuploadcancelled="fileUploadCancelled"
                                     @childfileuploaderror="childFileUploadError">
                        </file-upload>

                        <div v-if="$v.uploadingFile.$invalid || fileUploadServerError != '' || fileErrorType != 0">
                            <transition name="modal">
                                <div class="modal-mask">
                                    <div class="modal-wrapper">
                                        <div class="modal-dialog">
                                            <div class="modal-content">
                                                <div class="modal-header text-center">
                                                    <h4 class="modal-title"><i class="fas fa-exclamation-circle"></i> {{returnFileErrorTitle()}}</h4>
                                                </div>
                                                <div class="modal-body">
                                                    <div class="model-body-container">
                                                        <p>{{returnFileError()}}</p>
                                                        <p v-if="fileErrorType==2 || fileErrorType==7">Please see our support article which describes the <a :href="supportUrlExcludedFiles" target="_blank">file types that can be uploaded to the Learning Hub.</a></p>
                                                    </div>
                                                </div>
                                                <div class="modal-footer">
                                                    <button type="button" class="nhsuk-button nhsuk-button--secondary my-2" @click="clearFile">Close</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </transition>
                        </div>

                        <div v-if="changeResourceTypeWarning">
                            <transition name="modal">
                                <div class="modal-mask">
                                    <div class="modal-wrapper">
                                        <div class="modal-dialog">
                                            <div class="modal-content">
                                                <div class="modal-header text-center">
                                                    <h4 class="modal-title"><i class="warningTriangle fa-solid fa-triangle-exclamation"></i> Change resource type</h4>
                                                </div>
                                                <div class="modal-body">
                                                    <div class="model-body-container">
                                                        <p>
                                                            Changing the type of resource will remove some of the information you may have entered.
                                                        </p>
                                                    </div>
                                                </div>
                                                <div class="modal-footer modal-footer--buttons">
                                                    <button type="button" class="nhsuk-button nhsuk-button--secondary" @click="cancelPopup">Cancel</button>
                                                    <button type="button" class="nhsuk-button" @click="processChangeResourceType">Continue</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </transition>
                        </div>

                        <div v-if="deleteWarning">
                            <transition name="modal">
                                <div class="modal-mask">
                                    <div class="modal-wrapper">
                                        <div class="modal-dialog">
                                            <div class="modal-content">
                                                <div class="modal-header text-center">
                                                    <h4 class="modal-title">Delete this {{this.previousVersionExists ? 'draft version' : 'draft'}}</h4>
                                                </div>
                                                <div class="modal-body">
                                                    <div class="model-body-container text-center">
                                                        <p class="text-center">This {{this.previousVersionExists ? 'draft version' : 'draft'}} will no longer be available. This action cannot be undone.</p>
                                                    </div>
                                                </div>
                                                <div class="modal-footer modal-footer--buttons">
                                                    <button type="button" class="nhsuk-button nhsuk-button--secondary" @click="cancelPopup">Cancel</button>
                                                    <button type="button" class="nhsuk-button" @click="processDelete">Continue</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </transition>
                        </div>

                        <div v-if="fileDeleteWarning">
                            <transition name="modal">
                                <div class="modal-mask">
                                    <div class="modal-wrapper">
                                        <div class="modal-dialog">
                                            <div class="modal-content">
                                                <div class="modal-header text-center">
                                                    <h4 class="modal-title">Delete this file</h4>
                                                </div>
                                                <div class="modal-body">
                                                    <div class="model-body-container text-center">
                                                        <p class="text-center">This file will be removed from this resource.</p>
                                                    </div>
                                                </div>
                                                <div class="modal-footer modal-footer--buttons">
                                                    <button type="button" class="nhsuk-button nhsuk-button--secondary" @click="cancelPopup">Cancel</button>
                                                    <button type="button" class="nhsuk-button" @click="processDeleteFile">Delete</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </transition>
                        </div>

                        <div v-if="fileTypeChangeWarning">
                            <transition name="modal">
                                <div class="modal-mask">
                                    <div class="modal-wrapper">
                                        <div class="modal-dialog">
                                            <div class="modal-content">
                                                <div class="modal-header text-center">
                                                    <h4 class="modal-title"><i class="warningTriangle fa-solid fa-triangle-exclamation"></i> Different file type selected</h4>
                                                </div>
                                                <div class="modal-body">
                                                    <div class="model-body-container">
                                                        <p>
                                                            Changing the type of file will remove some of the information you may have entered.
                                                        </p>
                                                    </div>
                                                </div>
                                                <div class="modal-footer modal-footer--buttons">
                                                    <button type="button" class="nhsuk-button nhsuk-button--secondary" @click="cancelChangeFile">Cancel</button>
                                                    <button type="button" class="nhsuk-button" @click="processChangeFile">Continue</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </transition>
                        </div>

                        <div v-if="avUnavailableMessage">
                            <transition name="modal">
                                <div class="modal-mask">
                                    <div class="modal-wrapper">
                                        <div class="modal-dialog">
                                            <div class="modal-content">
                                                <div v-html="audioVideoUnavailableView"></div>
                                                <div class="modal-footer modal-footer--buttons">
                                                    <button type="button" class="nhsuk-button nhsuk-button--secondary" @click="cancelAVUnavailModal">Cancel</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </transition>
                        </div>

                        <div v-if="invalidFileTypeError">
                            <transition name="modal">
                                <div class="modal-mask">
                                    <div class="modal-wrapper">
                                        <div class="modal-dialog">
                                            <div class="modal-content">
                                                <div class="modal-header text-center">
                                                    <h4 class="modal-title"><i class="fas fa-exclamation-circle"></i> Different file type selected</h4>
                                                </div>
                                                <div class="modal-body">
                                                    <div class="model-body-container">
                                                        <p class="mb-3">
                                                            The file you have selected is a different type to the one that is published. As it is already published you cannot change this.
                                                        </p>
                                                        <p>
                                                            If you want to upload this instead, you will need to unpublish this resource and then create a new resource. You can do this in the My Contributions area.
                                                        </p>
                                                    </div>
                                                </div>
                                                <div class="modal-footer">
                                                    <button type="button" class="nhsuk-button nhsuk-button--secondary" @click="invalidFileTypeError=false">Close</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </transition>
                        </div>

                        <div v-if="publishWarning">
                            <transition name="modal">
                                <div class="modal-mask">
                                    <div class="modal-wrapper">
                                        <div class="modal-dialog">
                                            <div class="modal-content modal-content--scroll">
                                                <div class="modal-header text-center">
                                                    <h4 class="modal-title">Publish this version</h4>
                                                </div>
                                                <div class="modal-body">
                                                    <div class="model-body-containerContent">
                                                        <p>
                                                            This version will replace the published resource. This means that learners will no longer have access to the previous version. This action cannot be undone.
                                                        </p>
                                                    </div>
                                                    <div>
                                                        <div class="modal-section-header"><label for="notes">Notes</label></div>
                                                        <p class="mt-1">Provide information to help learners understand why this new version has been created.</p>
                                                        <textarea class="form-control" id="notes" v-bind:class="{ 'input-validation-error': $v.publishNotes.$invalid && $v.publishNotes.$dirty }" rows="4" maxlength="4000" v-model="publishNotes"></textarea>
                                                        <div class="error-text pt-3" v-if="$v.publishNotes.$invalid && $v.publishNotes.$dirty">
                                                            <span class="text-danger">Enter notes.</span>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="modal-footer modal-footer--buttons">
                                                    <button type="button" class="nhsuk-button nhsuk-button--secondary" @click="publishWarning=false">Cancel</button>
                                                    <button type="button" class="nhsuk-button" @click="publishConfirm">Publish</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </transition>
                        </div>

                        <div v-if="passwordVerification">
                            <transition name="modal">
                                <div class="modal-mask">
                                    <div class="modal-wrapper">
                                        <div class="modal-dialog">
                                            <div class="modal-content">
                                                <div class="modal-header text-center">
                                                    <h4 class="modal-title"><i class="warningTriangle fa-solid fa-triangle-exclamation"></i> Confirm your password</h4>
                                                </div>
                                                <div class="modal-body">
                                                    <div class="model-body-container">
                                                        <p>
                                                            To continue with the upload please verify your identity by entering your password.
                                                        </p>
                                                    </div>
                                                    <div>
                                                        <div class="form-group password-div-width" v-bind:class="{ 'input-validation-error': $v.currentPassword.$error}">
                                                            <label for="confirmPassword" class="pt-10">Current password</label>
                                                            <div class="error-text" v-if="$v.currentPassword.$invalid && $v.currentPassword.$dirty">
                                                                <span class="text-danger">Enter a valid password.</span>
                                                            </div>
                                                            <input type="password" class="form-control" id="currentPassword" aria-describedby="currentPassword" autocomplete="off" maxlength="1000"
                                                                   v-model.trim="currentPassword"
                                                                   placeholder="Current password"
                                                                   @blur="$v.currentPassword.$touch()"
                                                                   v-bind:class="{ 'input-validation-error': $v.currentPassword.$error}">
                                                        </div>

                                                        <div class="row" v-if="showError">
                                                            <div class="form-group col-12 text-danger" v-html="errorMessage" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="modal-footer modal-footer--buttons">
                                                    <button type="button" class="nhsuk-button nhsuk-button--secondary" @click="passwordVerification=false">Cancel</button>
                                                    <button type="button" class="nhsuk-button" @click="submitPassword">Continue</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </transition>
                        </div>

                    </div>

                    <div v-if="!resourceLoading" class="limit-width px-xl-0 mx-xl-0 pb-5">
                        <div class="row">
                            <div class="form-group col-12">
                                <hr />
                                <div class="pb-3 publish-warning" v-if="!canPublish">
                                    <div class="triangle">
                                        <i class="warningTriangle fa-solid fa-triangle-exclamation"></i>
                                    </div>
                                    <div>
                                        <p>You can only publish a resource when you have entered all of the mandatory information.</p>
                                    </div>
                                </div>
                                <TermsAndConditionsReminder v-if="canPublish" />
                                <button type="button" class="nhsuk-button mr-5 my-2" v-bind:class="{ 'disabled': !canPublish }" @click="publishResource" v-bind:disabled="!canPublish">Publish {{this.previousVersionExists ? 'new version' : 'draft'}}</button>
                                <button type="button" class="nhsuk-button nhsuk-button--secondary mr-5 my-2" @click="saveForLater" v-bind:class="{ 'disabled': resourceVersionId==0 }" v-bind:disabled="resourceVersionId==0">Save draft {{this.previousVersionExists ? 'version' : ''}}</button>
                                <button type="button" class="nhsuk-button nhsuk-button--secondary my-2" @click="deleteResource" v-bind:class="{ 'disabled': resourceVersionId==0 }" v-if="resourceVersionId!=0">Delete draft {{this.previousVersionExists ? 'version' : ''}}</button>
                            </div>
                        </div>
                        <div class="row" v-if="showError">
                            <div class="form-group col-12 text-danger" v-html="errorMessage" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { required } from "vuelidate/lib/validators";
    import contributeHeaderComponent from './HeaderComponent.vue';
    import ContentAccess from './ContentAccess.vue';
    import ContentCommon from './ContentCommon.vue';
    import ContentGenericFile from './ContentGenericFile.vue';
    import ContentScorm from './ContentScorm.vue';
    import ContentArticle from './ContentArticle.vue';
    import ContentAudio from './ContentAudio.vue';
    import ContentEmbedded from './ContentEmbedded.vue';
    import ContentEquipment from './ContentEquipment.vue';
    import ContentImage from './ContentImage.vue';
    import ContentVideo from './ContentVideo.vue';
    import ContentWeblink from './ContentWeblink.vue';
    import ContentHtml from './ContentHtml.vue';
    import FileUpload from './fileUpload.vue';
    import ProgressBar from 'vue-simple-progress';
    import GenericFileUploader from './GenericFileUploader.vue';
    import { UploadResourceType, ResourceType, VersionStatus } from '../constants';
    import { resourceData } from '../data/resource';
    import { userData } from '../data/user';
    import { FileTypeModel } from "../models/contribute/fileTypeModel";
    import { FileUploadResult } from "../models/contribute/FileUploadResult";
    import { FlagModel } from '../models/flagModel';
    import { ContributeResourceDetailModel, ResourceFileModel, ScormResourceModel } from '../models/contribute/contributeResourceModel';
    import { ContributeSettingsModel } from '../models/contribute/contributeSettingsModel';
    import { file_extension_validation, file_no_extension, file_size_validation, file_name_length_validation } from './fileValidation';
    import { FileErrorTypeEnum } from './fileErrors';
    import { EventBus } from './contributeEvents';
    import * as _ from "lodash";
    import { CatalogueBasicModel } from '../models/catalogueModel';
    import TermsAndConditionsReminder from './TermsAndConditionsReminder.vue';
    import { HierarchyEditStatusEnum } from '../models/content-structure/hierarchyEditModel';

    export default Vue.extend({
        props: {
            currentUserName: String,
            resourceLicenseUrl: String,
            resourceCertificateUrl: String,
            supportUrlExcludedFiles: String,
            resourceTypesSupported: String
        },
        components: {
            TermsAndConditionsReminder,
            'contributeHeaderComponent': contributeHeaderComponent,
            ProgressBar,
            ContentAccess,
            ContentCommon,
            ContentGenericFile,
            ContentScorm,
            ContentArticle,
            ContentAudio,
            ContentEmbedded,
            ContentEquipment,
            ContentImage,
            ContentVideo,
            ContentWeblink,
            ContentHtml,
            FileUpload,
            GenericFileUploader,
        },
        data() {
            return {
                resourceDetailTitle: '' as string,
                file: null as File,
                uploadingFile: null as File,
                uploadResourceTypes: [] as { id: number, description: string }[],
                uploadResourceType: UploadResourceType,
                resourceType: ResourceType,
                selectUploadResourceType: 0,
                fileUploadServerError: '',
                deleteWarning: false,
                publishWarning: false,
                publishNotes: '',
                fileTypeChangeWarning: false,
                changeResourceTypeWarning: false,
                invalidFileTypeError: false,
                fileErrorType: FileErrorTypeEnum.NoError,
                fileDeleteWarning: false,
                fileOrTypeToBeDeleted: 0,
                flags: [] as FlagModel[],
                displayType: '' as string,
                commonContentKey: 0,
                avUnavailableMessage: false,
                passwordVerification: false,
                currentPassword: '',
                // Some of the Content components have local state
                // which isn't in the vuex store.
                // This means those fields are validated using an
                // 'isvalid' $emit which sets specificContentLocalValid,
                // while other parts of the specific content are validated
                // in the Vuex store.
                // Ideally all validation would happen in one place, Vuex.
                specificContentLocalValid: null as boolean,
                localScormDetail: null as ScormResourceModel,
                showError: false,
                errorMessage: '',
                contributeResourceAVFlag: true,
                filePathBeforeFileChange: [] as string[],
                filePathAfterFileChange: [] as string[]
            }
        },
        computed: {
            commonContentValid(): boolean {
                return this.$store.state.commonContentValid;
            },
            specificContentValid(): boolean {
                return this.$store.state.specificContentValid;
            },
            contributeSettings(): ContributeSettingsModel {
                return this.$store.state.contributeSettings;
            },
            previousVersionExists(): boolean {
                return this.$store.state.previousVersionExists;
            },
            resourceLoading(): boolean {
                return this.$store.state.resourceLoading;
            },
            resourceVersionId(): number {
                return this.$store.state.resourceDetail.resourceVersionId;
            },
            resourceDetail(): ContributeResourceDetailModel {
                return this.$store.state.resourceDetail;
            },
            selectedResourceType(): ResourceType {
                return this.$store.state.resourceDetail.resourceType;
            },
            fileTypes(): FileTypeModel[] {
                return this.$store.state.fileTypes;
            },
            resourceTypeDescription(): string {
                let resourceTypeDescription = '';
                switch (this.selectUploadResourceType) {
                    case this.uploadResourceType.ARTICLE:
                        resourceTypeDescription = 'Article';
                        break;
                    case this.uploadResourceType.EMBEDRESOURCE:
                        resourceTypeDescription = 'Audio or video embed code';
                        break;
                    case this.uploadResourceType.EQUIPMENT:
                        resourceTypeDescription = 'Equipment or facilities';
                        break;
                    case this.uploadResourceType.FILEUPLOAD:
                        resourceTypeDescription = 'File upload';
                        break;
                    case this.uploadResourceType.WEBLINK:
                        resourceTypeDescription = 'Web link';
                        break;
                    case this.uploadResourceType.SCORM:
                        resourceTypeDescription = 'elearning package (SCORM 1.2)';
                        break;
                    case this.uploadResourceType.HTML:
                        resourceTypeDescription = 'HTML package';
                        break;
                }
                return resourceTypeDescription;
            },
            canPublish(): boolean {
                return !this.$v.resourceDetailTitle.$invalid
                    && (this.specificContentLocalValid == null || this.specificContentLocalValid)
                    && this.commonContentValid
                    && this.specificContentValid;
            },
            publishAfterSave(): boolean {
                return this.$store.state.publishAfterSave;
            },
            closeAfterSave(): boolean {
                return this.$store.state.closeAfterSave;
            },
            isFileAlreadyUploaded(): boolean {         
                switch (this.selectedResourceType) {
                    case this.resourceType.GENERICFILE:
                        return this.$store.state.genericFileDetail.file.fileName !== '';
                    case this.resourceType.IMAGE:
                        return this.$store.state.imageDetail.file.fileName !== '';
                    case this.resourceType.VIDEO:
                        return this.$store.state.videoDetail.file.fileName !== '';
                    case this.resourceType.AUDIO:
                        return this.$store.state.audioDetail.file.fileName !== '';
                    case this.resourceType.SCORM:
                        return this.$store.state.scormDetail.file && this.$store.state.scormDetail.file.fileName !== '';
                    case this.resourceType.HTML:
                        return this.$store.state.htmlDetail.file.fileName !== '';
                    default:
                        return false;
                }
            },
            isGenericFileUploaderVisible(): boolean {
                return this.selectUploadResourceType == this.uploadResourceType.FILEUPLOAD && !this.isFileAlreadyUploaded;
            },
            fileUploadRef(): any {
                const genericFileUploadRef: any = this.$refs.genericFileUpload;
                if (genericFileUploadRef === undefined) {
                    return undefined;
                }
                return genericFileUploadRef.$refs.fileUpload;
            },
            scormDetail(): ScormResourceModel {
                return this.$store.state.scormDetail;
            },
            catalogueLockedForEdit(): boolean {
                return !(this.$store.state.hierarchyEdit === null)
                    && this.$store.state.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Draft;
            },
            hierarchyEditLoaded(): boolean {
                return this.$store.state.hierarchyEditLoaded;
            },
            audioVideoUnavailableView(): string {
                return this.$store.state.getAVUnavailableView;
            },
        },
        async created() {
            if (this.$store.state.currentUserName == '') {
                this.$store.commit('setCurrentUserName', this.currentUserName);
            }
            if (this.$store.state.resourceLicenseUrl == '') {
                this.$store.commit('setResourceLicenseUrl', this.resourceLicenseUrl);
            }
            if (this.$store.state.resourceCertificateUrl == '') {
                this.$store.commit('setResourceCertificateUrl', this.resourceCertificateUrl);
            }
            if (this.$store.state.supportUrlExcludedFiles == '') {
                this.$store.commit('setSupportUrlExcludedFiles', this.supportUrlExcludedFiles);
            }
            if (!this.$store.state.fileTypes) {
                this.$store.commit('populateFileTypes');
            }
            this.getContributeResAVResourceFlag();
            this.uploadResourceTypes = await resourceData.getUploadResourceTypes();
            const allResourceTypes = this.uploadResourceTypes.slice();
            const allowedTypes = this.resourceTypesSupported.split(',');
            for (var i = 0; i < allResourceTypes.length; i++) {
                if (allowedTypes.indexOf(allResourceTypes[i].id.toString()) == -1) {
                    this.uploadResourceTypes = this.uploadResourceTypes.filter((elem) => elem.id !== allResourceTypes[i].id);
                }
            }

            if (this.selectedResourceType != this.resourceType.UNDEFINED) {
                this.setUploadResourceType();
            }

            this.localScormDetail = _.cloneDeep(this.scormDetail);
        },
        methods: {
            getContributeResAVResourceFlag() {
                resourceData.getContributeAVResourceFlag().then(response => {
                    this.contributeResourceAVFlag = response;
                });
            },
            setSpecificContentLocalValid(val: boolean) {
                this.specificContentLocalValid = val;
            },
            setTitle(value: string) {
                let field: string = 'title';
                if (this.$store.state.resourceDetail.title != value) {
                    this.$store.commit("saveResourceDetail", { field, value });
                }
            },
            setUploadResourceType() {
                switch (this.selectedResourceType) {
                    case this.resourceType.UNDEFINED:
                        this.selectUploadResourceType = UploadResourceType.NONE;
                        break
                    case this.resourceType.ARTICLE:
                        this.selectUploadResourceType = UploadResourceType.ARTICLE;
                        break
                    case this.resourceType.EMBEDDED:
                        this.selectUploadResourceType = UploadResourceType.EMBEDRESOURCE;
                        break
                    case this.resourceType.EQUIPMENT:
                        this.selectUploadResourceType = UploadResourceType.EQUIPMENT;
                        break
                    case this.resourceType.WEBLINK:
                        this.selectUploadResourceType = UploadResourceType.WEBLINK;
                        break
                    case this.resourceType.SCORM:
                        this.selectUploadResourceType = UploadResourceType.SCORM;
                        break
                    case this.resourceType.HTML:
                        this.selectUploadResourceType = UploadResourceType.HTML;
                        break
                    default:
                        this.selectUploadResourceType = UploadResourceType.FILEUPLOAD;
                        break
                }
            },
            publishResource() {
                if (this.$store.state.isSaving || this.$store.state.commonContentDirty || this.$store.state.specificContentDirty) {
                    this.$store.commit("publishAfterSave");
                } else {
                    if (this.previousVersionExists) {
                        this.publishNotes = '';
                        this.publishWarning = true;
                    } else {
                        this.publishNotes = 'Publish of draft resource.';
                        this.processPublish();
                    }
                }
            },
            saveForLater() {
                if (this.$store.state.isSaving || this.$store.state.commonContentDirty || this.$store.state.specificContentDirty) {
                    this.$store.commit("closeAfterSave");
                } else {

                    // IT1: Return to the 'All Content' tab with the Node expanded except when working in the Community Contributions catalogue.
                    // IT1: Previous behaviour wrt cards retained for Community Contributions catalogue.
                    if (this.$store.state.resourceDetail.nodeId > 1) {
                        const catalogue: CatalogueBasicModel = (<CatalogueBasicModel[]>this.$store.state.userCatalogues).find(c => c.nodeId == this.$store.state.resourceDetail.resourceCatalogueId);
                        window.location.href = '/my-contributions/allcontent/' + catalogue.url + '/' + this.$store.state.resourceDetail.nodeId;
                    }
                    else {
                        var targetUrl = '/my-contributions/draft';
                        if (this.previousVersionExists) {
                            if (this.resourceDetail.currentResourceVersionStatusEnum == VersionStatus.UNPUBLISHED) {
                                targetUrl = '/my-contributions/unpublished';
                            } else {
                                targetUrl = '/my-contributions/published';
                            }
                            if (this.resourceDetail.publishedResourceCatalogueId && this.resourceDetail.publishedResourceCatalogueId != this.resourceDetail.resourceCatalogueId) {
                                this.redirectToUrl(targetUrl, this.resourceDetail.publishedResourceCatalogueId);
                            } else {
                                this.redirectToUrl(targetUrl);
                            }
                        } else {
                            this.redirectToUrl(targetUrl);
                        }
                    }
                }
            },
            publishConfirm() {
                this.$v.publishNotes.$touch();
                if (!this.$v.publishNotes.$invalid) {
                    this.processPublish();
                }
            },
            submitPassword() {
                this.$v.currentPassword.$touch();
                if (!this.$v.currentPassword.$invalid) {
                    this.validatePassword();
                }              
            },
            async processPublish() {
                this.showError = false;
                let publishSuccess = await resourceData.publishResource(this.resourceVersionId, this.publishNotes);
                if (publishSuccess) {

                    // IT1: Return to the 'All Content' tab with the Node expanded except when Publishing into the Community Contributions catalogue.
                    if (this.$store.state.resourceDetail.nodeId > 1) {
                        const catalogue: CatalogueBasicModel = (<CatalogueBasicModel[]>this.$store.state.userCatalogues).find(c => c.nodeId == this.$store.state.resourceDetail.resourceCatalogueId);
                        window.location.href = '/my-contributions/allcontent/' + catalogue.url + '/' + this.$store.state.resourceDetail.nodeId;
                    }
                    else {
                        this.redirectToUrl('/my-contributions/published');
                    }
                }
                else {
                    this.showError = true;
                    this.errorMessage = "An error occurred whilst trying to publish the resource.";
                }
            },
            async validatePassword() {
                this.showError = false;
                let isValidUser = await userData.IsValidUser(this.currentPassword);
                if (isValidUser) {
                    this.acceptUploadedFile();
                    this.passwordVerification = false;
                }
                else {
                    this.showError = true;
                    this.errorMessage = "Enter a valid password.";
                }
            },
            deleteResource() {
                this.deleteWarning = true;
                this.showError = false;
            },
            async processDelete() {
                let deleteResult = await resourceData.deleteResourceVersion(this.resourceVersionId);
                if (deleteResult.isValid) {
                    // IT1: Return to the 'All Content' tab with the Node expanded except when working int the Community Contributions catalogue.
                    if (this.$store.state.resourceDetail.nodeId > 1) {
                        const catalogue: CatalogueBasicModel = (<CatalogueBasicModel[]>this.$store.state.userCatalogues).find(c => c.nodeId == this.$store.state.resourceDetail.resourceCatalogueId);
                        window.location.href = '/my-contributions/allcontent/' + catalogue.url + '/' + this.$store.state.resourceDetail.nodeId;
                    }
                    else {
                        this.redirectToUrl('/my-contributions/draft');
                    }
                }
                else {
                    this.deleteWarning = false;
                    this.showError = true;
                    this.errorMessage = "An error occurred whilst trying to delete the resource.";
                }
            },
            redirectToUrl(targetUrl: string, catalogueId?: number) {
                if (!catalogueId) {
                    catalogueId = this.resourceDetail.resourceCatalogueId;
                }
                if (catalogueId > 1) {
                    const catalogue: CatalogueBasicModel = (<CatalogueBasicModel[]>this.$store.state.userCatalogues).find(c => c.nodeId == catalogueId);
                    if (catalogue !== undefined) {
                        targetUrl += '/' + catalogue.url;
                    }
                }
                window.location.href = targetUrl;
            },
            clearResourceType() {
                this.changeResourceTypeWarning = true;
            },
            cancelPopup() {
                this.deleteWarning = false;
                this.changeResourceTypeWarning = false;
                this.fileDeleteWarning = false;
                this.fileOrTypeToBeDeleted = 0;
            },
            processChangeResourceType() {
                this.$store.commit("saveResourceType", this.resourceType.UNDEFINED);
                this.selectUploadResourceType = this.uploadResourceType.NONE;
                this.changeResourceTypeWarning = false;
            },
            cancelChangeFile() {
                this.fileTypeChangeWarning = false;
            },
            cancelAVUnavailModal() {
                this.avUnavailableMessage = false;
            },
            processChangeFile() {
                this.fileTypeChangeWarning = false;
                this.acceptUploadedFile();
            },
            acceptUploadedFile() {
                this.file = this.uploadingFile;
                this.uploadingFile = null;
                this.fileUploadRef.value = null;
                (this.$refs.fileUploader as any).uploadResourceFile(this.file);
            },
            confirmPassword() {
                this.currentPassword = '';
                this.passwordVerification = true;
            },
            async fileUploadComplete(uploadResult: FileUploadResult) {
                if (!uploadResult.invalid) {
                    if (uploadResult.resourceType != ResourceType.SCORM) {
                        this.$store.commit("setResourceType", uploadResult.resourceType);
                        this.$store.commit('setSaveStatus', 'Saved');
                        this.$store.commit('setResourceVersionId', uploadResult.resourceVersionId)
                    }
                    let file: ResourceFileModel = new ResourceFileModel();
                    file.resourceVersionId = uploadResult.resourceVersionId;
                    file.fileTypeId = uploadResult.fileTypeId;
                    file.fileName = uploadResult.fileName;
                    file.fileLocation = uploadResult.fileLocation;
                    file.fileSizeKb = uploadResult.fileSizeKb;
                    this.$store.commit('setResourceFile', file);
                    if (uploadResult.resourceType === ResourceType.SCORM) {
                        this.$store.commit('populateScormDetails', uploadResult.resourceVersionId);
                    }

                    if (this.filePathBeforeFileChange.length > 0) {
                        await this.getResourceFilePath('completed');
                        if (this.filePathBeforeFileChange.length > 0 && this.filePathAfterFileChange.length > 0) {
                            let filePaths = this.filePathBeforeFileChange.filter(item => !this.filePathAfterFileChange.includes(item));
                            if (filePaths.length > 0) {
                                resourceData.archiveResourceFile(filePaths);
                                this.filePathBeforeFileChange.length = 0;
                                this.filePathAfterFileChange.length = 0;
                            }
                        }
                    }
                   
                } else {
                    this.fileUploadServerError = 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.';
                    this.$store.commit('setSaveStatus', '');
                }
            },
            fileUploadCancelled() {
                console.log('File upload cancelled');
            },
            onUploadResourceTypeChange() {
                switch (this.selectUploadResourceType) {
                    case this.uploadResourceType.ARTICLE:
                        this.$store.commit("saveResourceType", this.resourceType.ARTICLE);
                        break;
                    case this.uploadResourceType.WEBLINK:
                        this.$store.state.resourceDetail.resourceLicenceId = 0;
                        (this.$refs.contentCommon as any).resetSelectedLicence();
                        this.$store.commit("saveResourceType", this.resourceType.WEBLINK);
                        break;
                    case this.uploadResourceType.EMBEDRESOURCE:
                        this.$store.commit("saveResourceType", this.resourceType.EMBEDDED);
                        break;
                    case this.uploadResourceType.EQUIPMENT:
                        this.$store.commit("saveResourceType", this.resourceType.EQUIPMENT);
                        break;
                    case this.uploadResourceType.FILEUPLOAD:
                        this.$store.commit("saveResourceType", this.resourceType.GENERICFILE);
                        break;
                    case this.uploadResourceType.SCORM:
                        this.$store.commit("saveResourceType", this.resourceType.SCORM);
                        break;
                    case this.uploadResourceType.HTML:
                        this.$store.commit("saveResourceType", this.resourceType.HTML);
                        break;
                    default:
                }
            },
            onScormPackageFileChange(event: any) {
                this.fileUploadServerError = '';
                this.uploadingFile = event.target.files[0];
                if (this.uploadingFile != null) {
                    if (!this.$v.uploadingFile.$invalid) {
                        if (this.selectedResourceType != ResourceType.UNDEFINED) {
                            let fileExtension = this.uploadingFile.name.split(".").pop();

                            if (fileExtension !== 'zip') {
                                this.fileErrorType = FileErrorTypeEnum.InvalidScormType;
                                return;
                            }
                            this.confirmPassword();
                        } else {
                            this.confirmPassword();
                        }
                    }
                }
            },
            fileChangedScorm() {
                (this.$refs.fileUploadScorm as any).click();
                this.getResourceFilePath('initialised');
            },
            childResourceFileChanged(newFile: File) {
                this.uploadingFile = newFile;
                this.processResourceFile();
            },
            onResourceFileChange() {
                this.uploadingFile = (this.fileUploadRef).files[0]
                this.processResourceFile();
            },
            processResourceFile() {
                this.fileUploadServerError = '';
                if (this.uploadingFile != null) {
                    if (!this.$v.uploadingFile.$invalid) {
                        if (this.selectedResourceType != ResourceType.UNDEFINED) {
                            let fileExtension = this.uploadingFile.name.split(".").pop();
                            let resourceType: ResourceType = ResourceType.GENERICFILE;
                            if (fileExtension.toLowerCase() == 'zip' && this.selectedResourceType == ResourceType.SCORM) {
                                resourceType = ResourceType.SCORM;
                            } else if (fileExtension.toLowerCase() == 'zip' && this.selectedResourceType == ResourceType.HTML) {
                                resourceType = ResourceType.HTML;
                            } else {
                                let fileTypes: FileTypeModel[] = this.$store.state.fileTypes;
                                let fileType = fileTypes.find(ft => ft.extension == fileExtension);
                                if (fileType) {
                                    resourceType = fileType.defaultResourceTypeId;
                                }
                            }
                            if ((resourceType == 2 || resourceType == 7) && !this.contributeResourceAVFlag) {
                                this.avUnavailableMessage = true;
                                return;
                            }
                            if (resourceType != this.selectedResourceType && this.isFileAlreadyUploaded) {
                                if (this.previousVersionExists) {
                                    this.invalidFileTypeError = true;
                                } else {
                                    this.fileTypeChangeWarning = true;
                                }
                            } else {
                                this.confirmPassword();
                            }
                        } else {
                            this.confirmPassword();  
                        }
                    }
                }
            },
            fileChanged() {
                this.fileUploadRef.value = null;
                this.fileUploadRef.click();
                this.getResourceFilePath('initialised');
            },
            childFileUploadError(errorType: FileErrorTypeEnum, customError: string) {
                this.fileErrorType = errorType;
                if (errorType == FileErrorTypeEnum.Custom) {
                    this.fileUploadServerError = customError;
                }
            },
            confirmDeleteFile(fileOrTypeId: number) {
                this.fileOrTypeToBeDeleted = fileOrTypeId;
                this.fileDeleteWarning = true;
            },
            processDeleteFile() {
                EventBus.$emit('deleteFile', this.fileOrTypeToBeDeleted);
                this.fileDeleteWarning = false;
                this.fileOrTypeToBeDeleted = 0;
            },
            clearFile() {
                this.uploadingFile = null;
                this.fileErrorType = FileErrorTypeEnum.NoError
                this.fileUploadServerError = '';
                this.fileUploadRef.value = null;
            },
            returnFileError() {
                let errorMessage = '';
                if (this.$v.uploadingFile.$invalid) {
                    if (!this.$v.uploadingFile.file_extension_validation) {
                        this.fileErrorType = FileErrorTypeEnum.InvalidType;
                    }
                    else if (!this.$v.uploadingFile.file_no_extension) {
                        this.fileErrorType = FileErrorTypeEnum.NoExtension;
                    } else if (!this.$v.uploadingFile.file_size_validation) {
                        this.fileErrorType = FileErrorTypeEnum.TooLarge;
                    } else if (!this.$v.uploadingFile.file_name_length_validation) {
                        this.fileErrorType = FileErrorTypeEnum.FilenameTooLong;
                    }
                } else if (this.fileUploadServerError != '') {
                    errorMessage = this.fileUploadServerError;
                }
                switch (this.fileErrorType) {
                    case FileErrorTypeEnum.NoExtension:
                        errorMessage = "A file without an extension cannot be uploaded.";
                        break;
                    case FileErrorTypeEnum.InvalidType:
                        errorMessage = "We do not support this file type on the Learning Hub.";
                        break;
                    case FileErrorTypeEnum.InvalidScormType:
                        errorMessage = "Only zip file can be uploaded as a SCORM package.";
                        break;
                    case FileErrorTypeEnum.TooLarge:
                        errorMessage = "This file cannot be uploaded as it exceeds the maximum file size limit.";
                        break;
                    case FileErrorTypeEnum.FilenameTooLong:
                        errorMessage = "This file cannot be uploaded as the file name is too long. The maximum is 255 characters.";
                        break;
                    case FileErrorTypeEnum.InvalidTranscriptType:
                        errorMessage = "This type of file cannot be uploaded for a transcript.";
                        break;
                    case FileErrorTypeEnum.InvalidCaptionsType:
                        errorMessage = "This type of file cannot be uploaded for closed captions.";
                        break;
                }
                return errorMessage;
            },
            returnFileErrorTitle() {
                let errorTitle = 'File upload';
                switch (this.fileErrorType) {
                    case FileErrorTypeEnum.InvalidTranscriptType:
                    case FileErrorTypeEnum.InvalidCaptionsType:
                        errorTitle = 'File not allowed';
                        break;
                }
                return errorTitle;
            },
            async getResourceFilePath(fileChangeStatus: string) {
                let resource = this.$store.state.resourceDetail;
                if (resource != null && resource.resourceVersionId > 0 &&(resource.resourceType != this.resourceType.CASE || resource.resourceType != this.resourceType.ASSESSMENT))
                {
                    await resourceData.getObsoleteResourceFile(resource.resourceVersionId).then(response => {
                        if (fileChangeStatus == 'initialised') {
                            this.filePathBeforeFileChange = response;
                            this.filePathAfterFileChange.length = 0;
                        }
                        else if (fileChangeStatus == 'completed') {
                            this.filePathAfterFileChange = response;
                        }
                    });
                }

            }
        },
        validations: {
            resourceDetailTitle: {
                required
            },
            uploadingFile: {
                file_size_validation,
                file_extension_validation,
                file_no_extension,
                file_name_length_validation
            },
            publishNotes: {
                required
            },
            currentPassword: {
                required
            }
        },
        watch: {
            resourceVersionId(value) {
                if (!this.$route.params.rvId && value != 0) {
                    this.$router.push({ name: 'ContributeAResource', params: { rvId: value.toString() } });
                } else {
                    this.resourceDetailTitle = this.resourceDetail.title;
                    this.flags = this.resourceDetail.flags;
                    this.setUploadResourceType();
                    this.displayType = this.previousVersionExists ? 'version' : 'draft';
                }
            },
            publishAfterSave(value) {
                if (!value) {
                    this.publishResource();
                }
            },
            closeAfterSave(value) {
                if (!value) {
                    this.saveForLater();
                }
            },
            selectedResourceType(value) {
                this.commonContentKey += 1;
            },
            scormDetail(value) {
                this.localScormDetail = _.cloneDeep(this.scormDetail);
            },
        }
    })

</script>

<style lang="scss" scoped>
    .resource-area-body-file-upload {
        min-height: auto !important;
    }

    .modal-content--scroll {
        max-height: 90vh;
        overflow-y: auto;
    }
    .password-div-width{
        max-width:70% !important;
        }
</style>
