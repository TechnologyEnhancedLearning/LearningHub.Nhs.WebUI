<template>
    <div>
        <div v-if="localScormDetail.file && localScormDetail.file.fileId > 0">
            <div class="row">
                <div class="col-12">                    
                    <span>
                        You can upload a .ZIP file that contains a SCORM 1.2 elearning package from your
                        computer or other storage drive you are connected to. Maximum file size {{contributeSettings.fileUploadSettings.fileUploadSizeLimitText}}
                    </span>
                </div>
            </div>
            <div class="row mt-3">
                <file-panel :file-id="localScormDetail.file.fileId" :file-description="localScormDetail.file.fileName" :file-size="localScormDetail.file.fileSizeKb" @changefile="changeFile"></file-panel>
            </div>
        </div>
        <div class="col-12 mt-4 file-details">
            <div>
                <h3>SCORM window size</h3>
                <p>Select an option below.</p>
                <div class="row my-2">
                    <div class="accordion col-12" id="accordion">
                        <div class="pt-0 pb-4">
                            <div class="heading" id="scormWindowInfo">
                                <div class="mb-0">
                                    <a href="#" class="collapsed text-decoration-skip" style="color:#005EB8;" data-toggle="collapse" data-target="#collapseScormWindowInfo" aria-expanded="false" aria-controls="collapseScormWindowInfo">
                                        <div class="accordion-arrow">More information on SCORM window</div>
                                    </a>
                                </div>
                            </div>
                            <div id="collapseScormWindowInfo" class="collapse" aria-labelledby="scormWindowInfo" data-parent="#accordion">
                                <div class="content col-12">
                                    <p>
                                        SCORM content will open in a new window. You can use the default window
                                        dimensions or you can set the height and width of the SCORM window by
                                        selecting the Advanced option below.
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <label class="checkContainer mr-0" style="margin-left:10px;">
                        <span>Default (1024 x 768)</span>
                        <input type="radio" name="scormWindowType" class="nhsuk-radios__input" v-model="localScormDetail.useDefaultPopupWindowSize" v-bind:value="true" @click="selectPopupWindowSize($event.target.value)" data-toggle="collapse" data-target="#advancedScormWindowSizePanel" checked />
                        <span class="radioButton"></span>
                    </label>
                </div>
                <div class="row">
                    <label class="checkContainer mr-0" style="margin-left:10px;">
                        <span>Advanced</span>
                        <input type="radio" name="scormWindowType" class="nhsuk-radios__input" v-model="localScormDetail.useDefaultPopupWindowSize" v-bind:value="false" @click="selectPopupWindowSize($event.target.value)" data-toggle="collapse" data-target="#advancedScormWindowSizePanel" />
                        <span class="radioButton"></span>
                    </label>
                </div>
                <div class="row" id="advancedScormWindowSize">
                    <div id="advancedScormWindowSizePanel" class="panel-collapse collapseAdvScormWindowSize" v-bind:class="{ 'collapse': localScormDetail.useDefaultPopupWindowSize}" style="margin-left:32px;">
                        <div id="popupDimensions" v-bind:class="{ 'input-validation-error': ($v.popupWidth.$error || $v.popupHeight.$error)}">
                            <div>
                                Input the SCORM window dimensions below in pixels (px) using numbers only. Maximum
                                width and height is 4096px by 2160px. Minimum width and height is 640px by 480px.
                            </div>
                            <div class="form-group mt-3">
                                <div class="error-text pb-0 mb-3" v-if="($v.popupWidth.$invalid && $v.popupWidth.$dirty) || ($v.popupHeight.$invalid && $v.popupHeight.$dirty)">
                                    <span class="text-danger">{{returnError('popupDimensions')}}</span>
                                </div>
                                <div class="user-entry d-inline-block ml-0 pb-0" v-bind:class="{ 'input-validation-error': $v.popupWidth.$error}">
                                    <input type="text" id="popupWidth" class="form-control d-inline-block popupDimension" :class="{ 'input-validation-error': $v.popupWidth.$error}" v-model="popupWidth" @blur="updatePopupWidth" @input="setValidStatus" /><span class="ml-2 mr-32">px</span>
                                </div>
                                <div class="user-entry d-inline-block ml-0 pb-0" v-bind:class="{ 'input-validation-error': $v.popupHeight.$error}">
                                    <input type="text" id="popupHeight" class="form-control d-inline-block popupDimension" :class="{ 'input-validation-error': $v.popupHeight.$error}" v-model="popupHeight" @blur="updatePopupHeight" @input="setValidStatus" /><span class="ml-2">px</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-12 mt-4 file-details">
            <div>
                <h3>ESR learning object link</h3>
                <p>Select an option below.</p>
                <div class="row my-2">
                    <div class="accordion col-12" id="accordion">
                        <div class="pt-0 pb-4">
                            <div class="heading" id="esrLinkInfo">
                                <div class="mb-0">
                                    <a href="#" class="collapsed text-decoration-skip" style="color:#005EB8;" data-toggle="collapse" data-target="#collapseEsrLinkInfo" aria-expanded="false" aria-controls="collapseEsrLinkInfo">
                                        <div class="accordion-arrow">More information on ESR learning object links</div>
                                    </a>
                                </div>
                            </div>
                            <div id="collapseEsrLinkInfo" class="collapse" aria-labelledby="esrLinkInfo" data-parent="#accordion">
                                <div class="content col-12">
                                    <p>
                                        Electronic Staff Record (ESR) users with the Learning Administration User Responsibility Profile (URP)
                                        can set up elearning content to play via ESR. Providing a link enables users with that URP to add this resource as a
                                        learning object in ESR. Select one of the three options to indicate if / how you would like the ESR link to be displayed
                                        to other users in the Learning Hub.
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <label class="checkContainer mr-0" style="margin-left:10px;">
                        <span>Don't display</span>
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localScormDetail.esrLinkType" value="1" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)" checked>
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer ml-3" v-if="resourceCatalogueCount<=1">
                        <span>Display only to me</span>
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localScormDetail.esrLinkType" value="2" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)">
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer ml-3" v-if="resourceCatalogueCount>1">
                        <span>Display only to me and other catalogue editors</span>
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localScormDetail.esrLinkType" value="3" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)">
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer ml-3">
                        <span>Display to everyone</span>
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localScormDetail.esrLinkType" value="4" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)">
                        <span class="radioButton"></span>
                    </label>
                </div>
            </div>
        </div>

        <div class="col-12 mt-4 file-details">
            <div>
                <h3>
                    Allow this resource to be downloaded
                </h3>
                <p>By default this resource is not available to download until permission is given. If you would like learners to be able to download this resource, please select the checkbox below.</p>
                <div class="mt-3">
                    <label class="checkContainer mb-0">
                        <p class="pl-3 mb-0">Yes, make this resource available to download.</p>
                        <input type="checkbox" name="canDownload"  class= "nhsuk-checkboxes__input" v-model="localScormDetail.canDownload" @click="resourceDownloadable($event.target.name, $event.target.checked)" />
                        <span class="checkmark"></span>
                    </label>
                </div>
            </div>
        </div>

        <div v-if="previousVersionExists" class="col-12 mt-4 file-details">
            <div>
                <h3>
                    Force clearing of user suspend data.
                </h3>
                <p>By default suspend data is usually persisted between user launches, but a change in the content can cause this data to become corrupted. If you know the content has changed, or if users are seeing issues when trying to launch a new version, selecting this checkbox will force a clearing of the users suspend data when they next launch this version of the content.</p>
                <div class="mt-3">
                    <label class="checkContainer mb-0">
                        <p class="pl-3 mb-0">Yes, clear the suspend data.</p>
                        <input type="checkbox" name="clearSuspendData" class= "nhsuk-checkboxes__input" v-model="localScormDetail.clearSuspendData" @click="resourceClearSuspendData($event.target.name, $event.target.checked)" />
                        <span class="checkmark"></span>
                    </label>
                </div>
            </div>
        </div>

        <div id="esrLinkModal" v-if="showEsrModal" class="modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
            <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                <div class="modal-content" v-if="showEsrModal">
                    <div class="modal-header mb-15">
                        <h2><i class="warningTriangle fa-solid fa-triangle-exclamation mr-3"></i>Removal of ESR learning object link</h2>
                    </div>
                    <div class="modal-body">
                        <div class="model-body-container mb-25">
                            <div class="mb-3">
                                This resource has an ESR learning object link that is visible to other users. This means that the link may have been used to add this resource as a learning object so others can access it in ESR. By removing this:
                            </div>
                            <div>
                                <ul v-if="changedValue==='1'">
                                    <li class="mb-3">The link will no longer be available for others to copy and use.</li>
                                    <li>The resource will no longer be available to access via ESR if this link has been used to set it up as a learning object. The resource will only be available to access in the Learning Hub.</li>
                                </ul>
                                <ul v-if="changedValue!=='1'">
                                    <li>The link will no longer be available for everyone to copy and use.</li>
                                </ul>
                            </div>
                        </div>
                    </div>

                    <div class="modal-footer modal-footer--buttons" style="padding-left: 0px;padding-right: 0px;">
                        <button type="button" class="nhsuk-button nhsuk-button--secondary" data-dismiss="modal" @click="hideWarningModal()">Cancel</button>
                        <button type="button" class="nhsuk-button" data-dismiss="modal" @click="processChangeEsrLinkType">Continue</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';    
    import { required, between } from "vuelidate/lib/validators";
    import * as _ from "lodash";
    import { ScormResourceModel, ResourceFileModel } from '../models/contribute/contributeResourceModel';
    import store from './contributeState';
    import FilePanel from './FilePanel.vue';
    import { ContributeSettingsModel } from '../models/contribute/contributeSettingsModel';

    export default Vue.extend({
        components: {
            FilePanel,
        },
        data() {
            return {
                file: null as File,
                localScormDetail: null as ScormResourceModel,
                showEsrModal: false as Boolean,
                previousEsrLinkType: 1 as number,
                popupWidth: 0 as number,
                popupHeight: 0 as number,
                fieldName: null as string,
                changedValue: null as string
            };
        },
        computed: {
            contributeSettings(): ContributeSettingsModel {
                return this.$store.state.contributeSettings;
            },
            scormDetail(): ScormResourceModel {
                return store.state.scormDetail;
            },
            scormFileResourceVersionId(): number {
                return store.state.scormDetail.id;
            },
            fileUpdated(): ResourceFileModel {
                return this.$store.state.fileUpdated;
            },
            resourceCatalogueCount(): number {
                if (!this.$store.state.userCatalogues) {
                    return 0;
                } else {
                    return this.$store.state.userCatalogues.length;
                }
            },
            previousVersionExists(): boolean {
                return this.$store.state.previousVersionExists;
            },
        },
        created() {
            this.localScormDetail = _.cloneDeep(this.scormDetail);
            this.previousEsrLinkType = this.localScormDetail.esrLinkType;
            this.popupWidth = this.localScormDetail.popupWidth;
            this.popupHeight = this.localScormDetail.popupHeight;
            this.setValidStatus();
        },
        methods: {
            changeFile() {
                this.$emit('filechanged');
            },
            showWarningModal(field: string, value: string) {
                this.fieldName = field;
                this.changedValue = value
                if (!this.previousVersionExists || this.previousEsrLinkType <= (+value)) {
                    this.processChangeEsrLinkType();                    
                }
                else {
                    this.showEsrModal = true
                }
            },
            selectPopupWindowSize(useDefault: boolean) {
                this.popupWidth = 1024;
                this.popupHeight = 768;

                let field: string = 'useDefaultPopupWindowSize';
                let value: boolean = useDefault;
                if (this.$store.state.scormDetail.useDefaultPopupWindowSize != value) {
                    this.$store.state.scormDetail.popupWidth = this.popupWidth;
                    this.$store.state.scormDetail.popupHeight = this.popupHeight;
                    this.$store.commit("saveScormDetail", { field, value });
                    this.setValidStatus();
                }
            },
            hideWarningModal() {
                this.localScormDetail.esrLinkType = this.previousEsrLinkType;
                this.showEsrModal = false;
            },
            processChangeEsrLinkType() {
                let preventSave = false;
                // "this.scormDetail[field as keyof ScormResourceModel]" equivalent to "this.scormDetail[field]"
                // TypeScript syntax is needed because noImplicitAny is set to true in the tsconfig.json file
                let storedValue: string = '';
                if (this.scormDetail[this.fieldName as keyof ScormResourceModel] != null) {
                    storedValue = this.scormDetail[this.fieldName as keyof ScormResourceModel].toString();
                }
                if (!preventSave && storedValue != this.changedValue) {
                    this.$store.commit("saveScormDetail", { field: this.fieldName, value: this.changedValue });
                }
                //this.previousEsrLinkType = parseInt(this.changedValue);
                this.showEsrModal = false;
            },
            resourceDownloadable(field: string, value: boolean) {
                this.$store.commit("saveScormDetail", { field: field, value: value });
            },
            resourceClearSuspendData(field: string, value: boolean) {
                this.$store.commit("saveScormDetail", { field: field, value: value });
            },
            setValidStatus() {
                this.$emit('isvalid', !(this.$v.popupWidth.$invalid || this.$v.popupHeight.$invalid));
            },
            updatePopupWidth()
            {
                if (this.$v.popupWidth.$invalid) {
                    this.$v.popupWidth.$touch();
                }
                else {
                    let field: string = 'popupWidth';
                    let value: number = this.popupWidth;
                    if (this.$store.state.scormDetail.popupWidth != value) {
                        this.$store.commit("saveScormDetail", { field, value });
                    }
                }
                this.setValidStatus();
            },
            updatePopupHeight() {
                if (this.$v.popupHeight.$invalid) {
                    this.$v.popupHeight.$touch();
                }
                else {
                    let field: string = 'popupHeight';
                    let value: number = this.popupHeight;
                    if (this.$store.state.scormDetail.popupHeight != value) {
                        this.$store.commit("saveScormDetail", { field, value });
                    }
                }
                this.setValidStatus();
            },
            returnError(ctrl: string) {
                var errorMessage = '';
                switch (ctrl) {
                    case 'popupDimensions':
                        if (this.$v.popupWidth.$invalid || this.$v.popupHeight.$invalid) {
                            if (!this.$v.popupHeight.required && !this.$v.popupWidth.required) {
                                errorMessage = "A width and height size in pixels is required";
                            }
                            else if (!this.$v.popupWidth.required) {
                                errorMessage = "A width size in pixels is required";
                            }
                            else if (!this.$v.popupHeight.required) {
                                errorMessage = "A height size in pixels is required";
                            }
                            if (!this.$v.popupWidth.between) {
                                if (this.popupWidth < 640) {
                                    errorMessage = "Popup width must be no less than 640 pixels";
                                }
                                if (this.popupWidth > 4096) {
                                    errorMessage = "Popup width must be no more than 4096 pixels";
                                }
                            }
                            else if (!this.$v.popupHeight.between) {
                                if (this.popupHeight < 480) {
                                    errorMessage = "Popup height must be no less than 480 pixels";
                                }
                                if (this.popupHeight > 2160) {
                                    errorMessage = "Popup height must be no more than 2160 pixels";
                                }
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
        },
        validations: {
            popupWidth: { required, between: between(640, 4096) },
            popupHeight: { required, between: between(480, 2160) }
        },
        watch: {
            scormFileResourceVersionId(value) {
                this.localScormDetail = _.cloneDeep(this.scormDetail);
            },
            fileUpdated(value) {
                this.localScormDetail.file = this.scormDetail.file;
            }
        }
    })

</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    #popupDimensions.input-validation-error {
        border-left: 3px solid $nhsuk-red;
        padding-left: 15px;
    }

    .popupDimension {
        width: 200px;
    }

    .checkContainer {
        padding-left: 28px;
        span{
                font-size:16px;
            }
    }

    .radioButton {
        height: 24px;
        width: 24px;
    }

        .radioButton:after {
            top: 4px;
            left: 4px;
            width: 12px;
            height: 12px;
        }

    .modal-body {
        background: #F0F4F5;
        width: auto;
        max-width: 642px;
    }

    .btn-cancel {
        margin-left: 0px !important;
        max-width: 99px !important;
        max-height: 48px !important;
    }

    .btn-continue {
        margin-right: 0px !important;
        color: #ffffff;
        max-width: 122px !important;
        max-height: 48px !important;
    }

    .modal-footer {        
        justify-content: space-between !important;
    }
</style>