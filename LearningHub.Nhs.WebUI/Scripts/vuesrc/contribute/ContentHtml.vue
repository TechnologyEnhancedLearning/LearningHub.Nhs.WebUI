<template>
    <div>
        <div class="row">
            <div class="form-group col-12 mb-0">
                <h3 id="content-label">Content<i v-if="!fileExists" class="warningTriangle fas fa-exclamation-triangle"></i></h3>

                <p>You can upload a zip file that contains a root index.html, from your computer or other storage drive you are connected to. Maximum file size 2GB.</p>
            </div>
        </div>

        <div v-show="!fileExists">
            <div class="mb-4 uploadBox">
                <div class="p-4 uploadInnerBox mt-4">
                    <div class="upload-btn-wrapper nhsuk-u-font-size-16">
                        <label for="fileUpload" class="nhsuk-button nhsuk-button--secondary" tabindex="0">Choose file</label> No file chosen
                        <input hidden type="file" id="fileUpload" accept=".zip,.rar,.7zip" aria-label="Choose file" ref="fileUpload" v-on:change="onResourceFileChange" />
                    </div>
                </div>
            </div>
        </div>

        <div v-show="fileExists">
            <div class="row">
                <file-panel :file-id="localHtmlDetail.file.fileId" :file-description="localHtmlDetail.file.fileName" :file-size="localHtmlDetail.file.fileSizeKb" @changefile="changeFile"></file-panel>
            </div>
        </div>

        <div class="col-12 mt-4 file-details">
            <div>
                <h3>HTML window size</h3>
                <p>Select an option below.</p>
                <div class="row my-2">
                    <div class="accordion col-12" id="accordion">
                        <div class="pt-0 pb-4">
                            <div class="heading" id="htmlWindowInfo">
                                <div class="mb-0">
                                    <a href="#" class="collapsed text-decoration-skip" style="color:#005EB8;" data-toggle="collapse" data-target="#collapseHtmlWindowInfo" aria-expanded="false" aria-controls="collapseHtmlWindowInfo">
                                        <div class="accordion-arrow">More information on HTML window</div>
                                    </a>
                                </div>
                            </div>
                            <div id="collapseHtmlWindowInfo" class="collapse" aria-labelledby="htmlWindowInfo" data-parent="#accordion">
                                <div class="content col-12">
                                    <p>
                                        HTML content will open in a new window. You can use the default window
                                        dimensions or you can set the height and width of the HTML window by
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
                        <input type="radio" name="htmlWindowType" class="nhsuk-radios__input" v-model="localHtmlDetail.useDefaultPopupWindowSize" v-bind:value="true" @click="selectPopupWindowSize($event.target.value)" data-toggle="collapse" data-target="#advancedHtmlWindowSizePanel" checked />
                        <span class="radioButton"></span>
                    </label>
                </div>
                <div class="row">
                    <label class="checkContainer mr-0" style="margin-left:10px;">
                        <span>Advanced</span>
                        <input type="radio" name="htmlWindowType"  class="nhsuk-radios__input" v-model="localHtmlDetail.useDefaultPopupWindowSize" v-bind:value="false" @click="selectPopupWindowSize($event.target.value)" data-toggle="collapse" data-target="#advancedHtmlWindowSizePanel" />
                        <span class="radioButton"></span>
                    </label>
                </div>
                <div class="row" id="advancedHtmlWindowSize">
                    <div id="advancedHtmlWindowSizePanel" class="panel-collapse collapseAdvHtmlWindowSize" v-bind:class="{ 'collapse': localHtmlDetail.useDefaultPopupWindowSize}" style="margin-left:32px;">
                        <div id="popupDimensions" v-bind:class="{ 'input-validation-error': ($v.popupWidth.$error || $v.popupHeight.$error)}">
                            <div>
                                Input the HTML window dimensions below in pixels (px) using numbers only. Maximum
                                width and height is 4096px by 2160px. Minimum width and height is 640px by 480px.
                            </div>
                            <div class="form-group mt-3">
                                <div class="error-text pb-0 mb-3" v-if="($v.popupWidth.$invalid && $v.popupWidth.$dirty) || ($v.popupHeight.$invalid && $v.popupHeight.$dirty)">
                                    <span class="text-danger">{{returnError()}}</span>
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
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localHtmlDetail.esrLinkType" value="1" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)" checked>
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer ml-3" v-if="resourceCatalogueCount<=1">
                        <span>Display only to me</span>
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localHtmlDetail.esrLinkType" value="2" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)">
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer ml-3" v-if="resourceCatalogueCount>1">
                        <span>Display only to me and other catalogue editors</span>
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localHtmlDetail.esrLinkType" value="3" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)">
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer ml-3">
                        <span>Display to everyone</span>
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localHtmlDetail.esrLinkType" value="4" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)">
                        <span class="radioButton"></span>
                    </label>
                </div>
            </div>
        </div>

        <div id="esrLinkModal" v-if="showEsrModal" class="modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
            <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                <div class="modal-content" v-if="showEsrModal">
                    <div class="modal-header mb-15">
                        <h2><i class="warningTriangle fas fa-exclamation-triangle mr-3"></i>Removal of ESR learning object link</h2>
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
    import * as _ from "lodash";
    import { required, between } from "vuelidate/lib/validators";
    import { HtmlResourceModel, ResourceFileModel } from '../models/contribute/contributeResourceModel';
    import { FileTypeModel } from "../models/contribute/fileTypeModel";
    import FilePanel from './FilePanel.vue';

    export default Vue.extend({
        props: {
            onResourceFileChange: Function
        },
        components: {
            FilePanel
        },
        data() {
            return {
                localHtmlDetail: null as HtmlResourceModel,
                popupWidth: 0 as number,
                popupHeight: 0 as number,
                previousEsrLinkType: 1 as number,
                showEsrModal: false as Boolean,
                fieldName: null as string,
                changedValue: null as string
            };
        },
        computed: {
            htmlDetail(): HtmlResourceModel {
                return this.$store.state.htmlDetail;
            },
            htmlResourceVersionId(): number {
                return this.$store.state.htmlDetail.resourceVersionId;
            },
            fileUpdated(): ResourceFileModel {
                return this.$store.state.fileUpdated;
            },
            fileExists(): boolean {
                return this.$store.state.htmlDetail.file != null && this.$store.state.htmlDetail.file.fileName !== ''
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
            }
        },
        created() {
            this.localHtmlDetail = _.cloneDeep(this.htmlDetail);
            this.popupWidth = this.localHtmlDetail.popupWidth;
            this.popupHeight = this.localHtmlDetail.popupHeight;
            this.previousEsrLinkType = this.localHtmlDetail.esrLinkType;
        },
        methods: {
            changeFile() {
                this.$emit('filechanged');
            },
            getFileType(fileName: string) {
                let fileExtension = fileName.split(".").pop();
                let fileTypes: FileTypeModel[] = this.$store.state.fileTypes;
                let fileType: FileTypeModel = fileTypes.find(ft => ft.extension == fileExtension);
                return fileType;
            },
            setValidStatus() {
                this.$emit('isvalid', !(this.$v.popupWidth.$invalid || this.$v.popupHeight.$invalid));
            },
            updatePopupWidth() {
                if (this.$v.popupWidth.$invalid) {
                    this.$v.popupWidth.$touch();
                }
                else {
                    let field: string = 'popupWidth';
                    let value: number = this.popupWidth;
                    if (this.$store.state.htmlDetail.popupWidth != value) {
                        this.$store.commit("saveHtmlDetail", { field, value });
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
                    if (this.$store.state.htmlDetail.popupHeight != value) {
                        this.$store.commit("saveHtmlDetail", { field, value });
                    }
                }
                this.setValidStatus();
            },
            selectPopupWindowSize(useDefault: boolean) {
                this.popupWidth = 1024;
                this.popupHeight = 768;

                let field: string = 'useDefaultPopupWindowSize';
                let value: boolean = useDefault;
                if (this.$store.state.htmlDetail.useDefaultPopupWindowSize != value) {
                    this.$store.state.htmlDetail.popupWidth = this.popupWidth;
                    this.$store.state.htmlDetail.popupHeight = this.popupHeight;
                    this.$store.commit("saveHtmlDetail", { field, value });
                    this.setValidStatus();
                }
            },
            returnError() {
                var errorMessage = '';
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
                return errorMessage;
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
            hideWarningModal() {
                this.localHtmlDetail.esrLinkType = this.previousEsrLinkType;
                this.showEsrModal = false;
            },
            processChangeEsrLinkType() {
                let preventSave = false;
                // "this.htmlDetail[field as keyof HtmlResourceModel]" equivalent to "this.htmlDetail[field]"
                // TypeScript syntax is needed because noImplicitAny is set to true in the tsconfig.json file
                let storedValue: string = '';
                if (this.htmlDetail[this.fieldName as keyof HtmlResourceModel] != null) {
                    storedValue = this.htmlDetail[this.fieldName as keyof HtmlResourceModel].toString();
                }
                if (!preventSave && storedValue != this.changedValue) {
                    this.$store.commit("saveHtmlDetail", { field: this.fieldName, value: this.changedValue });
                }
                this.showEsrModal = false;
            },
        },
        validations: {
            popupWidth: { required, between: between(640, 4096) },
            popupHeight: { required, between: between(480, 2160) }
        },
        watch: {
            htmlResourceVersionId(value) {
                this.localHtmlDetail = _.cloneDeep(this.htmlDetail);
            },
            fileUpdated(value) {
                this.localHtmlDetail.file = this.htmlDetail.file;
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
</style>
