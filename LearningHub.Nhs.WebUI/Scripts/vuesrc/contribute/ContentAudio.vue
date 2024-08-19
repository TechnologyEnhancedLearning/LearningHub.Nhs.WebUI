<template>
    <div>
        <div class="row">
            <div class="form-group col-12">
                <h3>Uploaded file</h3>
            </div>
        </div>
        <div v-if="!contributeResourceAVFlag">
            <div v-html="audioVideoUnavailableView"></div>
        </div>
        <div v-else class="row">
            <file-panel :file-id="localAudioDetail.file.fileId" :file-description="localAudioDetail.file.fileName" :file-size="localAudioDetail.file.fileSizeKb" @changefile="changeFile"></file-panel>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12">
                <h3>Transcript <span class="optional">(optional)</span></h3>
                <p>Please upload a transcript file to support learners that need to use an alternative format of this resource. This must be either a Word (.doc or .docx), PDF (.pdf) or Text (.txt) file.</p>
            </div>
        </div>
        <div v-if="localAudioDetail.transcriptFile" class="row">
            <file-panel :file-id="localAudioDetail.transcriptFile.fileId" :file-description="localAudioDetail.transcriptFile.fileName" :file-size="localAudioDetail.transcriptFile.fileSizeKb" @changefile="changeTranscriptFile" :showDelete="true" @deletefile="deleteTranscriptFile"></file-panel>
        </div>
        <div class="row" v-show="!localAudioDetail.transcriptFile">
            <div class="form-group col-12">
                <div class="p-4 uploadInnerBox">
                    <div class="upload-btn-wrapper nhsuk-u-font-size-16">
                        <label for="transcriptFileUpload" class="nhsuk-button nhsuk-button--secondary">Choose file</label> No file chosen
                        <input type="file" id="transcriptFileUpload" aria-label="Choose transcript file" ref="transcriptFileUpload" @change="onTranscriptFileChange" hidden />
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12">
                <h3 id="additionalinfo-label">Additional information <span class="optional">(optional)</span></h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                Add any further information that is relevant to this resource or will help learners to use it,
                for example, how it was developed or what is required for it to be used.
            </div>
            <div class="col-12 mt-3">
                <textarea class="form-control" aria-labelledby="additionalinfo-label" rows="4" maxlength="250" v-model="additionalInformation" @change="setAdditionalInformation($event.target.value)"></textarea>
            </div>
            <div class="col-12 footer-text">
                You can enter a maximum of 250 characters
            </div>
        </div>

        <file-upload :resource-version-id="resourceVersionId"
                     ref="attachedFileUploader"
                     @fileuploadcomplete="fileUploadComplete"
                     @fileuploadcancelled="fileUploadCancelled"
                     @childfileuploaderror="childFileUploadError">
        </file-upload>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import * as _ from "lodash";
    import { resourceData } from '../data/resource';
    import { AudioResourceModel, ResourceFileModel } from '../models/contribute/contributeResourceModel';
    import { FileUploadResult } from "../models/contribute/fileUploadResult";
    import { FileErrorTypeEnum } from './fileErrors';
    import { file_extension_validation, file_no_extension, file_size_validation, transcriptfile_extension_validation } from './fileValidation';
    import { EventBus } from './contributeEvents';
    import { ResourceType } from '../constants';
    import FilePanel from './FilePanel.vue';
    import FileUpload from './fileUpload.vue';

    export default Vue.extend({
        components: {
            FilePanel,
            FileUpload
        },
        data() {
            return {
                localAudioDetail: { resourceVersionId: 0 } as AudioResourceModel,
                additionalInformation: '' as string,
                uploadingFile: null as File,
                uploadingTranscriptFile: null as File,
                contributeResourceAVFlag: true
            };
        },
        computed: {
            resourceVersionId(): number {
                return this.$store.state.resourceDetail.resourceVersionId;
            },
            audioDetail(): AudioResourceModel {
                return this.$store.state.audioDetail;
            },
            audioResourceVersionId(): number {
                return this.$store.state.audioDetail.resourceVersionId;
            },
            fileUpdated(): ResourceFileModel {
                return this.$store.state.fileUpdated;
            },
            audioVideoUnavailableView(): string {
                return this.$store.state.getAVUnavailableView;
            }
        },
        created() {
            this.setInitialValues();
            this.getContributeResAVResourceFlag();
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
            changeFile() {
                this.$emit('filechanged');
            },
            getContributeResAVResourceFlag() {
                resourceData.getContributeAVResourceFlag().then(response => {
                    this.contributeResourceAVFlag = response;
                });
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
                    this.uploadingFile = this.uploadingTranscriptFile;
                    this.onFileChange(1);
                } else {
                    $('#transcriptFileUpload').val(null);
                    this.$emit('childfileuploaderror', FileErrorTypeEnum.InvalidTranscriptType, '');
                }
            },
            onFileChange(fileType: number) {
                if (!this.$v.uploadingFile.$invalid) {
                    (this.$refs.attachedFileUploader as any).uploadResourceAttachedFile(this.uploadingFile, fileType, ResourceType.AUDIO);
                } else {
                    $('#transcriptFileUpload').val(null);
                    $('#closedCaptionsFileUpload').val(null);
                    if (!this.$v.uploadingFile.file_size_validation) {
                        this.$emit('childfileuploaderror', FileErrorTypeEnum.TooLarge, '');
                    } else if (!this.$v.uploadingFile.file_extension_validation) {
                        this.$emit('childfileuploaderror', FileErrorTypeEnum.InvalidType, '');
                    } else if (!this.$v.uploadingFile.file_no_extension) {
                        this.$emit('childfileuploaderror', FileErrorTypeEnum.NoExtension, '');
                    }
                }
            },
            fileUploadComplete(uploadResult: FileUploadResult) {
                if (!uploadResult.invalid) {
                    this.$store.commit('setSaveStatus', 'Saved');
                    let file: ResourceFileModel = new ResourceFileModel();
                    file.resourceVersionId = this.resourceVersionId;
                    file.fileId = uploadResult.fileId;
                    file.fileTypeId = uploadResult.fileTypeId;
                    file.fileName = uploadResult.fileName;
                    file.fileSizeKb = uploadResult.fileSizeKb;
                    if (uploadResult.attachedFileTypeId == 1) {
                        this.localAudioDetail.transcriptFile = file;
                        this.$store.commit('setAudioTranscriptFile', file);
                    }
                } else {
                    this.$emit('childfileuploaderror', FileErrorTypeEnum.Custom, 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.');
                    this.$store.commit('setSaveStatus', '');
                }
                $('#transcriptFileUpload').val(null);
            },
            fileUploadCancelled() {
                console.log('File upload cancelled');
            },
            childFileUploadError(errorType: FileErrorTypeEnum, customError: string) {
                this.$emit('childfileuploaderror', errorType, customError);
            },
            async processDeleteFile(fileTypeId: number) {
                let response = await resourceData.deleteResourceAttributeFile(ResourceType.AUDIO, this.resourceVersionId, fileTypeId);
                if (response) {
                    this.$store.commit('removeAudioAttributeFile', fileTypeId);
                    switch (fileTypeId) {
                        case 1:
                            this.localAudioDetail.transcriptFile = null;
                            break;
                    }
                }
            },
            setInitialValues() {
                this.localAudioDetail = _.cloneDeep(this.audioDetail);
                this.additionalInformation = this.$store.state.resourceDetail.additionalInformation;
            },
            setProperty(field: string, value: string) {
                // "this.imageDetail[field as keyof ImageResourceModel]" equivalent to "this.imageDetail[field]"
                // TypeScript syntax is needed because noImplicitAny is set to true in the tsconfig.json file
                let storedValue: string = '';
                if (this.audioDetail[field as keyof AudioResourceModel] != null) {
                    storedValue = this.audioDetail[field as keyof AudioResourceModel].toString();
                }
                if (storedValue != value) {
                    this.$store.commit("saveAudioDetail", { field, value });
                }
            },
            setAdditionalInformation(value: string) {
                let field: string = 'additionalInformation';
                if (this.$store.state.resourceDetail.title != value) {
                    this.$store.commit("saveResourceDetail", { field, value });
                }
            }
        },
        watch: {
            audioResourceVersionId(value) {
                this.setInitialValues();
            },
            fileUpdated(value) {
                this.localAudioDetail.file = this.audioDetail.file;
            }
        },
        validations: {
            uploadingFile: {
                file_size_validation,
                file_extension_validation,
                file_no_extension
            },
            uploadingTranscriptFile: {
                transcriptfile_extension_validation
            }
        }
    })

</script>
