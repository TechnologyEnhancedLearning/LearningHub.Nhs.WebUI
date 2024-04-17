<template>
    <div class="pageVideoSection">
        <a :href="backUrl()">
            <i class="fa-solid fa-chevron-left">&nbsp;</i>
            Back
        </a>

        <div class="pt-5 pb-4">
            <h1>Video and text</h1>
        </div>

        <div class="pb-5" id="rowTitleDiv">
            <label class="control-label">Section Title (optional)</label><br />
            <input class="form-control w-75" type="text" name="rowTitle" v-model="pageSectionDetail.sectionTitle" maxlength="128" />
            <small>You can enter a maximum of 128 characters.</small><br />
        </div>
        <div class="nhsuk-form-group">
            <fieldset class="nhsuk-fieldset">
                <legend class="nhsuk-fieldset__legend nhsuk-fieldset__legend--l">
                    <h1 class="nhsuk-fieldset__heading">
                        Section Title style
                    </h1>
                </legend>

                <div class="nhsuk-radios nhsuk-radios--inline">

                    <div class="nhsuk-radios__item">
                        <input class="nhsuk-radios__input" id="radio-h1" name="sectionTitleElement" type="radio" value="h1" v-model="pageSectionDetail.sectionTitleElement">
                        <label class="nhsuk-label nhsuk-radios__label" for="radio-h1">
                            H1
                        </label>
                    </div>

                    <div class="nhsuk-radios__item">
                        <input class="nhsuk-radios__input" id="radio-h2" name="sectionTitleElement" type="radio" value="h2" v-model="pageSectionDetail.sectionTitleElement">
                        <label class="nhsuk-label nhsuk-radios__label" for="radio-h2">
                            H2
                        </label>
                    </div>

                    <div class="nhsuk-radios__item">
                        <input class="nhsuk-radios__input" id="radio-h3" name="sectionTitleElement" type="radio" value="h3" v-model="pageSectionDetail.sectionTitleElement">
                        <label class="nhsuk-label nhsuk-radios__label" for="radio-h3">
                            H3
                        </label>
                    </div>

                    <div class="nhsuk-radios__item">
                        <input class="nhsuk-radios__input" id="radio-h4" name="sectionTitleElement" type="radio" value="h4" v-model="pageSectionDetail.sectionTitleElement">
                        <label class="nhsuk-label nhsuk-radios__label" for="h4">
                            H4
                        </label>
                    </div>
                </div>
            </fieldset>
        </div>

        <div class="pb-5 pt-3">
            <div class="d-flex justify-content-start">
                <input type="checkbox" v-model="pageSectionDetail.topMargin" name="chkTopMargin" id="chkTopMargin" class="checkbox-large" />
                <label for="chkTopMargin" class="ml-2 mt-2">Top margin</label>
            </div>
            <div class="d-flex justify-content-start">
                <input type="checkbox" v-model="pageSectionDetail.bottomMargin" name="chkBottomMargin" id="chkBottomMargin" class="checkbox-large" />
                <label for="chkBottomMargin" class="ml-2 mt-2">Bottom margin</label>
            </div>
        </div>

        <div v-if="!addAVFlag">
            <label class="control-label">Feature Video</label>
            <div v-html="audioVideoUnavailableView"></div>
        </div>

        <div v-else>
            <label class="control-label">Feature Video</label>
            <div>
                <span class="text-secondary mb-5">
                    Video must be no larger than 2Gb.
                </span>
                <div class="p-4 uploadBox" v-show="pageSectionDetail.videoAsset && !pageSectionDetail.videoAsset.videoFile">
                    <div class="p-4">
                        <div class="upload-btn-wrapper">
                            <button class="btn btn-nhs-common btn-outline-secondary bg-white mr-4" type="button" @click="$refs.videoFileUpload.click()">Browse</button>
                            No file selected
                            <input class="d-none" type="file" id="videoFileUpload" ref="videoFileUpload"
                                   accept="video/*" @change="onVideoFileChange" />
                        </div>
                    </div>
                </div>
            </div>
            <span v-show="videoErrorMessage !== '' " class="text-danger field-validation-error col-12 pt-3" v-text="videoErrorMessage"></span>
            <div v-if="pageSectionDetail.videoAsset">
                <content-video ref="contentVideoPanel"
                               @filechanged="fileChanged"
                               @childfileuploaderror="childFileUploadError"
                               @confirmdeletefile="confirmDeleteFile">
                </content-video>
            </div>

            <file-upload ref="fileUploader"
                         @fileuploadcomplete="fileUploadComplete"
                         @fileuploadcancelled="fileUploadCancelled"
                         @childfileuploaderror="childFileUploadError">
            </file-upload>

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
                                            <p class="text-center">This file will be removed.</p>
                                        </div>
                                    </div>
                                    <div class="row modal-footer two-buttons">
                                        <div class="col-md-6 mx-0 mb-4">
                                            <button type="button" class="btn btn-outline-custom" @click="cancelPopup">Cancel</button>
                                        </div>
                                        <div class="col-md-6 mx-0 mb-4">
                                            <button type="button" class="btn btn-danger-custom" @click="processDeleteFile">Delete</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </transition>
            </div>

        </div>

        <div class="pb-5 pt-3">
            <label class="control-label">Background colour</label><br />
            <small>Pick a colour from the colour palette below or add your own hex value.</small>

            <div class="d-flex align-items-center mt-4">
                <i :class="[`section-bg section-bg-white ${pageSectionDetail.backgroundColour == white ? 'selected' : ''}`]" @click="setBgColor(white)" />
                <i :class="[`section-bg section-bg-grey ${pageSectionDetail.backgroundColour == grey3 ? 'selected' : ''}`]" @click="setBgColor(grey3)" />
                <i :class="[`section-bg section-bg-yellow ${pageSectionDetail.backgroundColour == yellow ? 'selected' : ''}`]" @click="setBgColor(yellow)" />
                <i :class="[`section-bg section-bg-blue ${pageSectionDetail.backgroundColour == blue ? 'selected' : ''}`]" @click="setBgColor(blue)" />
                <i :class="[`section-bg section-bg-grey-1 ${pageSectionDetail.backgroundColour == grey1 ? 'selected' : ''}`]" @click="setBgColor(grey1)" />
                <div class="input-color">
                    <i class="fa-solid fa-hashtag input-color-hash"></i>
                    <input class="form-control input-color-field" v-model="customBgColor" type="text" @change="setBgColor(`#${$event.target.value}`, true)" />
                </div>
            </div>
            <span v-if="invalidBgColor" class="text-danger field-validation-error">Enter a valid hex value</span>
        </div>

        <div class="pb-5 pt-3">
            <label class="control-label">Text colour</label><br />
            <small>Pick a colour from the colour palette below or add your own hex value.</small>

            <div class="d-flex align-items-center mt-4">
                <i :class="[`section-bg section-bg-black ${pageSectionDetail.textColour == black ? 'selected' : ''}`]" @click="setColor(black)" />
                <i :class="[`section-bg section-bg-white ${pageSectionDetail.textColour == white ? 'selected' : ''}`]" @click="setColor(white)" />
                <div class="input-color">
                    <i class="fa-solid fa-hashtag input-color-hash"></i>
                    <input class="form-control input-color-field" type="text" v-model="customTextColor" @change="setColor(`#${$event.target.value}`, true)" />
                </div>
            </div>
            <span v-if="invalidColor" class="text-danger field-validation-error">Enter a valid hex value</span>
        </div>

        <div class="pb-5 pt-3">
            <label class="control-label">Text background colour</label><br />
            <small>Pick a colour from the colour palette below or add your own hex value.</small>

            <div class="d-flex align-items-center mt-4">
                <i :class="[`section-bg section-bg-transparent ${pageSectionDetail.textBackgroundColour == transparent ? 'selected' : ''}`]" @click="setTxtBgColor(transparent)" />
                <i :class="[`section-bg section-bg-white ${pageSectionDetail.textBackgroundColour == white ? 'selected' : ''}`]" @click="setTxtBgColor(white)" />
                <i :class="[`section-bg section-bg-grey ${pageSectionDetail.textBackgroundColour == grey3 ? 'selected' : ''}`]" @click="setTxtBgColor(grey3)" />
                <i :class="[`section-bg section-bg-yellow ${pageSectionDetail.textBackgroundColour == yellow ? 'selected' : ''}`]" @click="setTxtBgColor(yellow)" />
                <i :class="[`section-bg section-bg-blue ${pageSectionDetail.textBackgroundColour == blue ? 'selected' : ''}`]" @click="setTxtBgColor(blue)" />
                <i :class="[`section-bg section-bg-grey-1 ${pageSectionDetail.textBackgroundColour == grey1 ? 'selected' : ''}`]" @click="setTxtBgColor(grey1)" />
                <div class="input-color">
                    <i class="fa-solid fa-hashtag input-color-hash"></i>
                    <input class="form-control input-color-field" v-model="customTxtBgColor" type="text" @change="setTxtBgColor(`#${$event.target.value}`, true)" />
                </div>
            </div>
            <span v-if="invalidTxtBgColor" class="text-danger field-validation-error">Enter a valid hex value</span>
        </div>

        <div class="pb-5 pt-3">
            <label class="control-label">Hyperlink colour</label><br />
            <small>Pick a colour from the colour palette below or add your own hex value.</small>

            <div class="d-flex align-items-center mt-4">
                <i :class="[`section-bg section-bg-blue ${pageSectionDetail.hyperLinkColour == blue ? 'selected' : ''}`]" @click="setHyperlinkColor(blue)" />
                <i :class="[`section-bg section-bg-white ${pageSectionDetail.hyperLinkColour == white ? 'selected' : ''}`]" @click="setHyperlinkColor(white)" />
                <div class="input-color">
                    <i class="fa-solid fa-hashtag input-color-hash"></i>
                    <input class="form-control input-color-field" type="text" v-model="customHyperlinkColor" @change="setHyperlinkColor(`#${$event.target.value}`, true)" />
                </div>
            </div>
            <span v-if="invalidHyperlinkColor" class="text-danger field-validation-error">Enter a valid hex value</span>
        </div>

        <div class="pb-5 pt-3 w-75" id="descriptionDiv">
            <label class="control-label">Description</label>

            <ckeditor v-model="pageSectionDetail.description" :config="editorConfig" @ready="onEditorReady" @blur="validateDescription"></ckeditor>

            <span v-if="invalidDescription" class="text-danger field-validation-error">Enter a description</span>
        </div>

        <div class="d-flex flex-row justify-content-between align-items-center my-5 w-75">
            <div>
                <button :class="[`btn btn-nhs-common ${valid ? 'btn-success' : 'btn-secondary'}`]" @click="save" type="button">Save</button>
                <button class="btn btn-nhs-common btn-outline-primary ml-5" type="button" @click="cancel">Cancel</button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';

import { contentData } from '../data/content';
import { PageSectionDetailModel, SectionLayoutType } from '../models/content/pageSectionDetailModel';
import { VideoAssetModel } from '../models/content/videoAssetModel';
import CKEditorToolbar from '../models/ckeditorToolbar';
import CKEditor from 'ckeditor4-vue/dist/legacy.js';

import FileUpload from './upload/fileUpload.vue';
import ProgressBar from 'vue-simple-progress';
import { FileUploadResult } from "../models/content/fileUploadResult";
import { FileModel } from '../models/content/fileModel';
import contentVideo from './upload/contentVideo.vue';
import { UploadSettingsModel } from '../models/content/uploadSettingsModel';
import { file_extension_validation, file_size_validation } from './upload/fileValidation';
import { FileErrorTypeEnum } from './upload/fileErrors';
import { EventBus } from './upload/contentEvents';
import store from './contentState';
import Vuelidate from "vuelidate";

Vue.use(Vuelidate as any);
    export default Vue.extend({
        store,
        components: {
            ckeditor: CKEditor.component,            
            ProgressBar,
            contentVideo,
            FileUpload,
        },
        props: {
        },
        data() {
            return {
                pageSectionDetail: new PageSectionDetailModel(),
                transparent: '',
                white: '#FFFFFF',
                yellow: '#FFED00',
                blue: '#005EB8',
                black: '#212B32',
                grey1: '#425563',
                grey3: '#F0F4F5',
                SectionLayoutType: SectionLayoutType,
                editorConfig: {
                    toolbar: CKEditorToolbar.landingPages,
                    stylesSet: 'landing-pages-video-text'
                },

                invalidBgColor: false,
                invalidColor: false,
                invalidHyperlinkColor: false,
                invalidTxtBgColor: false,
                invalidImageAlt: false,
                invalidDescription: false,

                showImage: false,
                imageErrorMessage: '',
                customBgColor: '',
                customTextColor: '',
                customHyperlinkColor: '',
                customTxtBgColor: '',

                file: null as File,
                uploadingFile: null as File,
                fileTypeChangeWarning: false,
                invalidFileTypeError: false,
                fileUploadServerError: '',
                fileErrorType: FileErrorTypeEnum.NoError,
                deleteWarning: false,
                fileDeleteWarning: false,
                fileOrTypeToBeDeleted: 0,
                videoErrorMessage: '',
                addAVFlag: false
            }
        },
        validations: {
            uploadingFile: {
                file_extension_validation,
                file_size_validation,                
            }
        },
        async created() {
            this.$store.commit('populateUploadSettings');            
            this.$store.commit('populateAVUnavailableView'); 
            this.getAddAudioVideoFlag();

            const pageSectionId = this.$route.params.sectionId;

            if (!isNaN(parseInt(pageSectionId))) {                
                await contentData.getPageSectionDetailForEdit(parseInt(pageSectionId)).then(response => {
                    this.pageSectionDetail = response;
                    this.$store.commit('setPageSectionDetail', this.pageSectionDetail);
                });;

                if (this.pageSectionDetail) {
                    
                    if (this.pageSectionDetail.backgroundColour != this.white
                        && this.pageSectionDetail.backgroundColour != this.grey3
                        && this.pageSectionDetail.backgroundColour != this.yellow
                        && this.pageSectionDetail.backgroundColour != this.blue
                        && this.pageSectionDetail.backgroundColour != this.grey1) {
                        this.customBgColor = this.pageSectionDetail.backgroundColour.substring(1);
                    }

                    if (this.pageSectionDetail.textColour != this.black
                        && this.pageSectionDetail.textColour != this.white) {
                        this.customTextColor = this.pageSectionDetail.textColour.substring(1);
                    }

                    if (this.pageSectionDetail.hyperLinkColour != this.blue
                        && this.pageSectionDetail.hyperLinkColour != this.white) {
                        this.customHyperlinkColor = this.pageSectionDetail.hyperLinkColour.substring(1);
                    }

                    if (this.pageSectionDetail.textBackgroundColour != this.white
                        && this.pageSectionDetail.textBackgroundColour != this.grey3
                        && this.pageSectionDetail.textBackgroundColour != this.yellow
                        && this.pageSectionDetail.textBackgroundColour != this.blue
                        && this.pageSectionDetail.textBackgroundColour != this.grey1) {
                        this.customTxtBgColor = !this.pageSectionDetail.textBackgroundColour ? '' :  this.pageSectionDetail.textBackgroundColour.substring(1);
                    }
                }
            }
        },
        computed: {
            valid: function () {
                const vm = this as any;
                return !vm.invalidBgColor &&
                    !vm.invalidColor &&
                    !vm.invalidHyperlinkColor &&
                    !vm.invalidDescription &&
                    (vm.videoAsset && vm.videoAsset.videoFile);
            },
            uploadSettings(): UploadSettingsModel {
                return this.$store.state.contributeSettings;
            },
            videoAsset(): VideoAssetModel {
                return this.$store.state.pageSectionDetail.videoAsset;
            },            
            audioVideoUnavailableView(): string {
                return this.$store.state.getAVUnavailableView;
            },
        },
        methods: {
            setSectionLayoutType(sectionLayoutType: SectionLayoutType) {
                this.pageSectionDetail.sectionLayoutType = sectionLayoutType;
            },
            setBgColor(color: string, isCustom: boolean = false) {
                this.invalidBgColor = this.validateColorHex(color);
                this.pageSectionDetail.backgroundColour = color;
                if (!isCustom) this.customBgColor = '';
            },
            setColor(color: string, isCustom: boolean = false) {
                this.invalidColor = this.validateColorHex(color);
                this.pageSectionDetail.textColour = color;
                if (!isCustom) this.customTextColor = '';
            },
            setHyperlinkColor(color: string, isCustom: boolean = false) {
                this.invalidHyperlinkColor = this.validateColorHex(color);
                this.pageSectionDetail.hyperLinkColour = color;
                if (!isCustom) this.customHyperlinkColor = '';
            },
            setTxtBgColor(color: string, isCustom: boolean = false) {
                this.invalidTxtBgColor = color != '' && this.validateColorHex(color);
                this.pageSectionDetail.textBackgroundColour = color;
                if (!isCustom) this.customTxtBgColor = '';
            },
            validateColorHex(color: string) {              
                return !(/^#([0-9A-F]{3}){1,2}$/i.test(color));
            },
            validateDescription() {           
                this.invalidDescription = this.isEmptyOrSpaces(CKEDITOR.instances.editor1.document.getBody().getText());               
            },            
            isEmptyOrSpaces(str: string) {
                if (str) {
                    str = str.trim();
                }
                return str === null || str === undefined || str.match(/^ *$/) !== null;                
            },
            onEditorReady(editor: any) {
                var current = this;
                editor.on('change', function () {
                    current.validateDescription();
                });
            },
            scrollTo(elem: string) {
                document.getElementById(elem).scrollIntoView();
            },
            backUrl() {
                return `/cms/page/${this.$route.params.pageId}`;
            },
            async save() {
                this.validateDescription();
                
                if (this.invalidDescription) this.scrollTo("descriptionDiv");

                if (this.valid) {
                    contentData.updatePageSectionDetail(this.pageSectionDetail).then(response => {
                        if (response) {
                            window.location.replace(`/cms/page/${this.$route.params.pageId}`);
                        }
                    });;
                }
            },
            cancel() {
                window.location.replace(`/cms/page/${this.$route.params.pageId}`);
            },           
            onVideoFileChange() {
                this.fileUploadServerError = '';
                this.uploadingFile = (this.$refs.videoFileUpload as any).files[0];
                this.videoErrorMessage = '';
                if (this.$v.uploadingFile.$invalid) {
                    if (!this.$v.uploadingFile.file_extension_validation) {
                        this.videoErrorMessage = "This type of file cannot be uploaded.";
                        $('#videoFileUpload').val(null);
                        return;
                    }
                    else if (!this.$v.uploadingFile.file_size_validation) {
                        this.videoErrorMessage = "Your video file size is too large. Please upload a video no larger than 2.GB";
                        $('#videoFileUpload').val(null);
                        return;
                    }                    
                }
                else {
                    this.videoErrorMessage = '';
                    this.acceptUploadedFile();
                }               
            },
            acceptUploadedFile() {
                this.file = this.uploadingFile;
                this.uploadingFile = null;
                $('#videoFileUpload').val(null);
                (this.$refs.fileUploader as any).uploadFile(this.file);
            },
            fileUploadComplete(uploadResult: FileUploadResult) {
                if (!uploadResult.invalid) {
                    let file: FileModel = new FileModel();
                    file.fileId = uploadResult.fileId;
                    file.fileTypeId = uploadResult.fileTypeId;
                    file.fileName = uploadResult.fileName;
                    file.fileSizeKb = uploadResult.fileSizeKb;
                    contentData.getPageSectionDetailVideo(this.pageSectionDetail.id).then(response => {
                        this.pageSectionDetail = response;
                        let panel = (this.$refs.contentVideoPanel as any);
                        if (panel) { panel.videoFileUploaded(file); }
                        this.$store.commit('setPageSectionDetail', this.pageSectionDetail);
                    });                                      
                    this.$store.commit('setVideoFile', file);
                    
                } else {
                    this.fileUploadServerError = 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.';                    
                }
            },
            fileUploadCancelled() {
                console.log('File upload cancelled');
            },
            fileChanged() {
                $('#videoFileUpload').val(null);
                $('#videoFileUpload').click();
            },
            childFileUploadError(errorType: FileErrorTypeEnum, customError: string) {
                this.fileErrorType = errorType;
                if (errorType == FileErrorTypeEnum.Custom) {
                    this.fileUploadServerError = customError;
                }
            },
            cancelPopup() {
                this.deleteWarning = false;
                this.fileDeleteWarning = false;
                this.fileOrTypeToBeDeleted = 0;
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
                $('#fileUpload').val(null);
            }, 
            getAddAudioVideoFlag() {
                contentData.getAddAVFlag().then(response => {
                    this.addAVFlag = response;
               });
            },
        },
    });
</script>
<style lang="scss"  scoped>
    @use "../../../Styles/Abstracts/all" as *;
    
    div.pageVideoSection {
        select.form-control,
        input.form-control,
        textarea.form-control {
            border: 2px solid $nhsuk-grey;
            box-shadow: none;
        }
    
        hr {
            margin: 3.5rem 0 0 0;
            border-color: $nhsuk-grey-light;
        }
    
        .modal-dialog {
            max-width: 625px;
        }
    
        .modal-mask {
            position: fixed;
            z-index: 9998;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, .5);
            display: table;
            transition: opacity .3s ease;
        }
    
        .modal-wrapper {
            display: table-cell;
            vertical-align: middle;
        }
    
        div.modal-content div.modal-body {
            padding: 1rem 1.2rem;
    
            div.modal-section-header {
                font-family: $font-stack-bold;
                font-weight: bold;
                font-size: 2.4rem;
                margin-top: 1rem;
            }
        }
    
        .model-body-container {
            background-color: $nhsuk-grey-white;
            min-height: 7rem;
            border-radius: .5rem;
            padding: 2.5rem;
    
            p {
                padding: 0;
                margin: 0;
            }
        }
    
        div.modal-content {
    
            div.modal-header {
                justify-content: center;
                padding: 1rem;
    
                i.fa-exclamation-circle {
                    color: $nhsuk-red;
                    font-size: 3.6rem
                }
            }
    
            h4.modal-title i {
                font-size: 2.5rem;
            }
    
            div.modal-footer {
                padding-left: 0;
                padding-right: 0;
                padding-bottom: 0;
                justify-content: center;
            }
    
            div.modal-footer.two-buttons {
                padding-left: 1rem;
                padding-right: 1rem;
    
                button {
                    padding-left: 1rem;
                    padding-right: 1rem;
                    width: 100%;
                }
            }
        }
    
        progress {
            width: 400px;
            margin: auto;
            display: block;
            margin-top: 20px;
            margin-bottom: 20px;
            -webkit-appearance: none;
            appearance: none;
            background-color: $nhsuk-white;
            color: $nhsuk-blue
        }
    
        .uploadBox {
            background-color: $nhsuk-grey-white;
        }
    
        .uploadInnerBox {
            background-color: $nhsuk-white;
            font-size: 1.6rem;
            height: 7rem;
    
            .upload-btn-wrapper {
                position: relative;
                overflow: hidden;
                display: inline-block;
    
                input[type=file] {
                    font-size: 100px;
                    position: absolute;
                    left: 0;
                    top: 0;
                    opacity: 0;
                }
            }
    
            button {
                border: 1px solid #979797;
                background-color: white;
                padding: 8px 15px;
                border-radius: 10px;
                margin-right: 1rem;
            }
        }
    
        .nhsWhiteBackground {
            background-color: $nhsuk-white;
        }
    
        .input-with-button {
            flex-flow: row;
            display: flex;
            align-items: center;
        }
    }
</style>
<style>
    .selected-check-circle {
        position: absolute;
        top: -15px;
        right: -15px;
    }

    .section-bg {
        width: 40px;
        height: 40px;
        border-radius: 50%;
        margin-right: 24px;
        position: relative;
        cursor: pointer;
    }

        .section-bg.selected:before {
            content: '';
            position: absolute;
            width: 50px;
            height: 50px;
            border-radius: 50%;
            border: 3px solid #005EB8;
            top: -5px;
            left: -5px;
        }

    .section-bg-white {
        border: 1px solid #768692;
        background: #FFFFFF;
    }

    .section-bg-grey {
        border: 1px solid #AEB7BD;
        background: #F0F4F5;
    }

    .section-bg-yellow {
        background: #FFED00;
    }

    .section-bg-blue {
        background: #005EB8;
    }

    .section-bg-grey-1 {
        background: #425563;
    }

    .section-bg-black {
        background: #212B32;
    }

    .section-bg-white.selected:before, .section-bg-grey.selected:before {
        top: -6px;
        left: -6px;
    }

    .input-color-hash {
        position: absolute;
        padding: 13px 5px;
        min-width: 30px;
    }

    .input-color-field {
        padding-left: 24px;
        max-width: 170px;
    }

    .btn-outline-secondary:hover {
        color: #6c757d;
    }

    .btn-outline-custom, .btn-custom, .btn-danger-custom, .btn-green {
        background-color: #005eb8 !important;
        color: #fff !important;
        font-size: 1.9rem !important;
        text-align: center !important;
        border: 1px solid #005eb8 !important;
        min-height: 50px;
        min-width: 115px;
        padding: 0 25px 0 25px;
        border-radius: 5px;
    }
    .btn-outline-custom:hover {
        background-color: #e4f2ff !important;
    }
    .btn-danger-custom {
        background-color: #d5281b !important;
        color: #fff !important;
        border: 1px solid #d5281b !important;
    }
</style>