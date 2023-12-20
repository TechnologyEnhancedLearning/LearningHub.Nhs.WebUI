<template>
    <div>
        <div class="contentVideoContainer" v-show="videoAsset.videoFile">
            <file-panel ref="videoFilePanel" :file="localVideoAsset.videoFile" @changefile="changeFile"></file-panel>
            <div>
                <div class="mt-5">
                    <div class="form-group col-12">
                        <label class="control-label">Transcript <span class="optional">(optional)</span></label>
                        <div class="text-secondary">Please upload a transcript file to support learners that need to use an alternative format of this resource. This must be either a Word (.doc or .docx), PDF (.pdf) or Text (.txt) file.</div>
                    </div>
                </div>
                <div v-if="localVideoAsset.transcriptFile">
                    <file-panel ref="transcriptPanel" :file="localVideoAsset.transcriptFile" @changefile="changeTranscriptFile" :showDelete="true" @deletefile="deleteTranscriptFile"></file-panel>
                </div>
                <div v-show="!localVideoAsset.transcriptFile">
                    <div class="form-group col-12">
                        <div class="p-4 uploadInnerBox">
                            <div class="upload-btn-wrapper">
                                <button class="btn btn-nhs-common btn-outline-secondary bg-white mr-4" type="button" @click="$refs.transcriptFileUpload.click()">Browse</button>
                                No file selected
                                <input type="file" accept=".txt, .pdf, .doc, .docx" id="transcriptFileUpload" aria-label="Choose transcript file" ref="transcriptFileUpload" @change="onTranscriptFileChange" />
                            </div>
                        </div>
                    </div>
                </div>
                <span v-show="transcriptErrorMessage !== '' " class="text-danger field-validation-error  col-12 pt-3" v-text="transcriptErrorMessage"></span>
                <div class="mt-5">
                    <div class="form-group col-12">
                        <label class="control-label">Closed captions <span class="optional">(optional)</span></label>
                        <div class="text-secondary">Please upload a closed captions file to support learners that need the audio displayed as text on this video. This must be a file that has a .VTT file extension.</div>
                    </div>
                </div>
                <div v-if="localVideoAsset.closedCaptionsFile">
                    <file-panel ref="closedCaptionPanel" :file="localVideoAsset.closedCaptionsFile" @changefile="changeCaptionsFile" :showDelete="true" @deletefile="deleteCaptionsFile"></file-panel>
                </div>
                <div v-show="!localVideoAsset.closedCaptionsFile">
                    <div class="form-group col-12">
                        <div class="p-4 uploadInnerBox">
                            <div class="upload-btn-wrapper">
                                <button class="btn btn-nhs-common btn-outline-secondary bg-white mr-4" type="button" @click="$refs.closedCaptionsFileUpload.click()">Browse</button>
                                No file selected
                                <input type="file" accept=".vtt" id="closedCaptionsFileUpload" aria-label="Choose closed captions file" ref="closedCaptionsFileUpload" @change="onClosedCaptionsFileChange" />
                            </div>
                        </div>
                    </div>
                </div>
                <span v-show="captionErrorMessage !== '' " class="text-danger field-validation-error col-12 pt-3" v-text="captionErrorMessage"></span>
            </div>
        </div>
        <div class="mt-5">
            <label class="control-label">Feature video thumbnail<span class="optional">(optional)</span></label>
            <div class="text-secondary">Image must be in either .jpg, .png or .gif format (suggested size 640px x 360px).</div>
        </div>
        <div class="contentVideoContainer" v-if="localVideoAsset && localVideoAsset.thumbnailImageFile">
            <file-panel ref="thumbnailPanel" :file="localVideoAsset.thumbnailImageFile" @changefile="changeThumbnailImageFile" :showDelete="true" @deletefile="deleteThumbnailImageFile"></file-panel>
        </div>
        <div class="p-4 uploadBox" v-show="localVideoAsset && !localVideoAsset.thumbnailImageFile">
            <div class="p-4">
                <div class="upload-btn-wrapper">
                    <button class="btn btn-nhs-common btn-outline-secondary bg-white mr-4" type="button" @click="$refs.thumbnailImageFileUpload.click()">Browse</button>
                    No file selected
                    <input class="d-none" type="file" id="thumbnailImageFileUpload" ref="thumbnailImageFileUpload"
                           accept="image/*" @change="onThumbnailImageFileChange" />
                </div>
            </div>
        </div>
        <span v-show="thumbnailErrorMessage !== ''" class="text-danger field-validation-error  col-12 pt-3" v-text="thumbnailErrorMessage"></span>

        <file-upload ref="attachedFileUploader"
                     @fileuploadcomplete="fileUploadComplete"
                     @fileuploadcancelled="fileUploadCancelled"
                     @childfileuploaderror="childFileUploadError">
        </file-upload>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import * as _ from "lodash";
    import { FileUploadResult } from "../../models/content/fileUploadResult";
    import { FileErrorTypeEnum } from './fileErrors';
    import { file_extension_validation, file_size_validation, transcriptfile_extension_validation, closedcaptionsfile_extension_validation, thumbnailimagefile_extension_validation, thumbnailimagefile_size_validation } from './fileValidation';
    import { EventBus } from './contentEvents';
    import filePanel from './filePanel.vue';
    import fileUpload from './fileUpload.vue';
    import { VideoAssetModel } from '../../models/content/videoAssetModel';
    import { FileModel } from '../../models/content/fileModel';
    import Vuelidate from "vuelidate";
    import { contentData } from '../../data/content';
    import store from '../contentState';
    Vue.use(Vuelidate as any);
    export default Vue.extend({
        components: {
            filePanel,
            fileUpload
        },
        data() {
            return {
                localVideoAsset: {} as VideoAssetModel,
                uploadingFile: null as File,
                uploadingTranscriptFile: null as File,
                uploadingCaptionsFile: null as File,
                uploadingThumbnailImageFile: null as File,
                transcriptErrorMessage: '' as string,
                captionErrorMessage: '' as string,
                thumbnailErrorMessage: '' as string,
            };
        },
        watch: {
           
        },
        computed: {
            pageSectionDetailId(): number {
                return this.$store.state.pageSectionDetailId;
            },
            videoAsset(): VideoAssetModel {
                return this.$store.state.pageSectionDetail.videoAsset;
            }
        },
        created() {
            let videoAsset = this.videoAsset;
            this.localVideoAsset = _.cloneDeep(videoAsset);

            EventBus.$on('deleteFile', (fileTypeToBeDeleted: number) => {
                this.processDeleteFile(fileTypeToBeDeleted);
            });
        },
        beforeDestroy() {
            EventBus.$off('deleteFile', (fileTypeToBeDeleted: number) => {
                this.processDeleteFile(fileTypeToBeDeleted);
            });
        },
        methods: {
            setInitialValues() {
                var videoAsset = this.$store.state.pageSectionDetail.videoAsset;
                this.localVideoAsset = _.cloneDeep(videoAsset);
            },
            videoFileUploaded(file: FileModel) {                
                this.localVideoAsset.videoFile = file;
                (this.$refs.videoFilePanel as any).updateFileDetails(file);
            },
            changeFile() {
                this.$emit('filechanged');
            },
            changeTranscriptFile() {
                $('#transcriptFileUpload').val(null);
                $('#transcriptFileUpload').click();
            },
            deleteTranscriptFile() {
                this.$emit('confirmdeletefile', 1);
            },
            onTranscriptFileChange() {
                this.uploadingTranscriptFile = (this.$refs.transcriptFileUpload as any).files[0];
                if (!this.$v.uploadingTranscriptFile.$invalid) {
                    this.transcriptErrorMessage = "";
                    this.uploadingFile = this.uploadingTranscriptFile;
                    this.onFileChange(1);
                } else {
                    $('#transcriptFileUpload').val(null);
                    this.transcriptErrorMessage = "Your transcript file must be either a Word (.doc or .docx), PDF (.pdf) or Text (.txt file)";
                }
            },
            changeCaptionsFile() {
                $('#closedCaptionsFileUpload').val(null);
                $('#closedCaptionsFileUpload').click();
            },
            deleteCaptionsFile() {
                this.$emit('confirmdeletefile', 2);
                console.log('deleteCaptionsFile');
            },
            onClosedCaptionsFileChange() {
                this.uploadingCaptionsFile = (this.$refs.closedCaptionsFileUpload as any).files[0];

                if (!this.$v.uploadingCaptionsFile.$invalid) {
                    this.captionErrorMessage = "";
                    this.uploadingFile = this.uploadingCaptionsFile;
                    this.onFileChange(2);
                } else {
                    $('#closedCaptionsFileUpload').val(null);
                    this.captionErrorMessage = "Your caption file must be .VTT format";
                }
            },
            changeThumbnailImageFile() {
                $('#thumbnailImageFileUpload').val(null);
                $('#thumbnailImageFileUpload').click();
            },
            deleteThumbnailImageFile() {
                this.$emit('confirmdeletefile', 3);
                console.log('deleteThumbnailImageFile');
            },
            onThumbnailImageFileChange() {
                this.uploadingThumbnailImageFile = (this.$refs.thumbnailImageFileUpload as any).files[0];
                this.thumbnailErrorMessage = "";

                if (this.$v.uploadingThumbnailImageFile.$invalid) {
                    $('#thumbnailImageFileUpload').val(null);
                    this.thumbnailErrorMessage = "Image must be in either .jpg, .png or .gif format.";
                    return;
                }

                this.uploadingFile = this.uploadingThumbnailImageFile;
                this.onFileChange(3);

                //var v = this;
                //setTimeout(function () {
                //    if (store.state.isThumbnailFileValid) {
                //        v.thumbnailErrorMessage = "";
                //        v.uploadingFile = v.uploadingThumbnailImageFile;
                //        v.onFileChange(3);
                //    }
                //    else {
                //        $('#thumbnailImageFileUpload').val(null);
                //        v.thumbnailErrorMessage = "Image must be 540 X 280px size and in either .jpg, .png or .gif format.";
                //    }
                //}, 1000);                
            },
            onFileChange(fileType: number) {
                (this.$refs.attachedFileUploader as any).uploadVideoAttachedFile(this.uploadingFile, fileType);
            },
            fileUploadComplete(uploadResult: FileUploadResult) {
                if (!uploadResult.invalid) {
                    let file: FileModel = new FileModel();
                    file.fileId = uploadResult.fileId;
                    file.fileTypeId = uploadResult.fileTypeId;
                    file.fileName = uploadResult.fileName;
                    file.fileSizeKb = uploadResult.fileSizeKb;
                    
                    if (uploadResult.attachedFileTypeId == 1) {
                        this.localVideoAsset.transcriptFile = file;
                        let panel = (this.$refs.transcriptPanel as any);
                        if (panel) { panel.updateFileDetails(file); }
                        this.$store.commit('setVideoTranscriptFile', file);
                    } else if (uploadResult.attachedFileTypeId == 2) {
                        this.localVideoAsset.closedCaptionsFile = file;
                        let panel = (this.$refs.closedCaptionPanel as any);
                        if (panel) { panel.updateFileDetails(file); }                        
                        this.$store.commit('setVideoclosedCaptionsFile', file);
                    }
                    else if (uploadResult.attachedFileTypeId == 3) {
                        this.localVideoAsset.thumbnailImageFile = file;
                        let panel = (this.$refs.thumbnailPanel as any);
                        if (panel) { panel.updateFileDetails(file); }
                        this.$store.commit('setVideoThumbnailImageFile', file);
                    }
                    
                } else {
                    this.$emit('childfileuploaderror', FileErrorTypeEnum.Custom, 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.');                    
                }
                $('#transcriptFileUpload').val(null);
                $('#closedCaptionsFileUpload').val(null);
                $('#thumbnailImageFileUpload').val(null);
            },
            fileUploadCancelled() {
                console.log('File upload cancelled');
            },
            childFileUploadError(errorType: FileErrorTypeEnum, customError: string) {
                this.$emit('childfileuploaderror', errorType, customError);
            },
            async processDeleteFile(fileTypeId: number) {
                console.log('processDeleteFile ' + fileTypeId);
                switch (fileTypeId) {
                    case 1:
                        this.localVideoAsset.transcriptFileId = null;
                        this.localVideoAsset.transcriptFile = null;
                        break;
                    case 2:
                        this.localVideoAsset.closedCaptionsFileId = null;
                        this.localVideoAsset.closedCaptionsFile = null;
                        break;
                    case 3:
                        this.localVideoAsset.thumbnailImageFileId = null;
                        this.localVideoAsset.thumbnailImageFile = null;
                        break;
                }
                contentData.updateVideoAsset(this.localVideoAsset);
                this.$store.commit('removeVideoAttributeFile', fileTypeId);
            },
            setProperty(field: string, value: string) {
                let storedValue: string = '';
                if (this.videoAsset[field as keyof VideoAssetModel] != null) {
                    storedValue = this.videoAsset[field as keyof VideoAssetModel].toString();
                }
                if (storedValue != value) {
                    this.$store.commit("updateVideoAsset", { field, value });
                }
                console.log("storedValue" + storedValue);
            },
        },
        validations: {
            uploadingFile: {
                file_size_validation,
                file_extension_validation
            },
            uploadingTranscriptFile: {
                transcriptfile_extension_validation
            },
            uploadingCaptionsFile: {
                closedcaptionsfile_extension_validation
            },
            uploadingThumbnailImageFile: {
                thumbnailimagefile_extension_validation,
                thumbnailimagefile_size_validation
            },
        }
    })

</script>
<style lang="scss"  scoped>
    .contentVideoContainer {
        background-color: #F0F4F5;
        min-height: 110px;
        margin-top: 10px;
        padding: 20px 10px 20px 10px;
        margin-bottom: 15px;
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
</style>