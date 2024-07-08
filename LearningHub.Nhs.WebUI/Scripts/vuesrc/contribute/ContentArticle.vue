<template>
    <div>

        <div class="row">
            <div class="form-group col-12 mb-0">
                <h3 id="content-label">Content<i v-if="$v.articleContent.$invalid" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                Add the article content below.
            </div>
            <div class="col-12 mt-3 article-content">
                <ckeditor aria-labelledby="content-label" v-model="articleContent" :config="editorConfig" @blur="onContentBlur" @input="contentKeyup"></ckeditor>
            </div>
        </div>

        <div class="row mt-4">
            <div class="form-group col-12">
                <h3>Files <span class="optional">(optional) You can upload a maximum of 10 files.</span></h3>
                <p class="">
                    You can upload a file from your computer or other storage drive you are connected to.
                    Maximum file size {{contributeSettings.fileUploadSettings.fileUploadSizeLimitText}}
                </p>
            </div>
        </div>

        <div class="row" v-if="localArticleDetail.files.length<10">
            <div class="form-group col-12">
                <div class="p-4 uploadInnerBox">
                    <div class="upload-btn-wrapper nhsuk-u-font-size-16">
                        <label for="articleFileUpload" class="nhsuk-button nhsuk-button--secondary">Choose file</label> No file chosen
                        <input type="file" id="articleFileUpload" aria-label="Choose file" ref="articleFileUpload" @change="onArticleFileChange" hidden />
                    </div>
                </div>
            </div>
        </div>

        <div v-for="file in localArticleDetail.files" class="row mb-4">
            <file-panel :file-id="file.fileId" :file-description="file.fileName" :file-size="file.fileSizeKb" @changefile="changeFile" :showDelete="true" @deletefile="deleteFile"></file-panel>
        </div>

        <file-upload :resource-version-id="resourceVersionId"
                     ref="articleFileUploader"
                     @fileuploadcomplete="fileUploadComplete"
                     @fileuploadcancelled="fileUploadCancelled"
                     @childfileuploaderror="childFileUploadError">
        </file-upload>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { required } from "vuelidate/lib/validators";
    import * as _ from "lodash";
    import { ArticleResourceModel, AttachedFileModel } from '../models/contribute/contributeResourceModel';
    import CKEditorToolbar from '../models/ckeditorToolbar';
    import CKEditor from 'ckeditor4-vue/dist/legacy.js';
    import { resourceData } from '../data/resource';
    import { file_extension_validation, file_no_extension, file_size_validation } from './fileValidation';
    import { FileErrorTypeEnum } from './fileErrors';
    import { FileUploadResult } from "../models/contribute/fileUploadResult";
    import { ContributeSettingsModel } from '../models/contribute/contributeSettingsModel';
    import { EventBus } from './contributeEvents';
    import FilePanel from './FilePanel.vue';
    import FileUpload from './fileUpload.vue';

    export default Vue.extend({
        components: {
            FilePanel,
            FileUpload,
            ckeditor: CKEditor.component
        },
        data() {
            return {
                articleContent: '',
                editorConfig: {                
                    toolbar: CKEditorToolbar.default,
                     versionCheck: false
                },
                localArticleDetail: new ArticleResourceModel(),
                uploadingFile: null as File,
                changeingFileId: 0
            };
        },
        computed: {
            contributeSettings(): ContributeSettingsModel {
                return this.$store.state.contributeSettings;
            },
            resourceVersionId(): number {
                return this.$store.state.resourceDetail.resourceVersionId;
            },
            articleDetail(): ArticleResourceModel {
                return this.$store.state.articleDetail;
            },
            articleResourceVersionId(): number {
                return this.$store.state.articleDetail.resourceVersionId;
            }
        },
        created() {
            this.setInitialValues();
            EventBus.$on('deleteFile', (fileIdToBeDeleted: number) => {
                this.processDeleteFile(fileIdToBeDeleted);
            });
        },
        beforeDestroy() {
            EventBus.$off('deleteFile', (fileIdToBeDeleted: number) => {
                this.processDeleteFile(fileIdToBeDeleted);
            });
        },
        methods: {
            setInitialValues() {
                this.localArticleDetail = _.cloneDeep(this.articleDetail);
                if (this.localArticleDetail.description) {
                    this.articleContent = this.localArticleDetail.description;
                } else {
                    this.articleContent = '';
                }
                this.setValidStatus();
            },
            setProperty(field: string, value: string) {
                // "this.imageDetail[field as keyof ImageResourceModel]" equivalent to "this.imageDetail[field]"
                // TypeScript syntax is needed because noImplicitAny is set to true in the tsconfig.json file
                let storedValue: string = '';
                if (this.articleDetail[field as keyof ArticleResourceModel] != null) {
                    storedValue = this.articleDetail[field as keyof ArticleResourceModel].toString();
                }
                if (storedValue != value) {
                    this.$store.commit("saveArticleDetail", { field, value });
                }
            },
            onContentBlur() {
                this.setProperty('description', this.articleContent);
            },
            contentKeyup() {
                this.$store.commit("setSpecificContentDirty", this.articleContent != this.articleDetail.description);
                this.setValidStatus();
            },
            onArticleFileChange() {
                this.uploadingFile = (this.$refs.articleFileUpload as any).files[0];
                if (!this.$v.uploadingFile.$invalid) {
                    (this.$refs.articleFileUploader as any).uploadArticleFile(this.uploadingFile, this.changeingFileId);
                } else {
                    $('#articleFileUpload').val(null);
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
                    let file: AttachedFileModel = new AttachedFileModel();
                    file.fileId = uploadResult.fileId;
                    file.fileTypeId = uploadResult.fileTypeId;
                    file.fileName = uploadResult.fileName;
                    file.fileSizeKb = uploadResult.fileSizeKb;
                    if (this.changeingFileId == 0) {
                        this.localArticleDetail.files.push(file);
                        this.$store.commit('addArticleFile', file);
                    } else {
                        let existingFileId = this.changeingFileId;
                        let index = _.findIndex(this.localArticleDetail.files, { fileId: existingFileId });
                        this.localArticleDetail.files.splice(index, 1, file);
                        this.$store.commit('replaceArticleFile', { existingFileId, file });
                    }
                } else {
                    this.$emit('childfileuploaderror', FileErrorTypeEnum.Custom, 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.');
                    this.$store.commit('setSaveStatus', '');
                }
                this.changeingFileId = 0;
                $('#articleFileUpload').val(null);
            },
            fileUploadCancelled() {
                console.log('File upload cancelled');
            },
            childFileUploadError(errorType: FileErrorTypeEnum, customError: string) {
                this.$emit('childfileuploaderror', errorType, customError);
            },
            changeFile(fileId: number) {
                this.changeingFileId = fileId;
                $('#articleFileUpload').val(null);
                $('#articleFileUpload').click();
            },
            deleteFile(fileId: number) {
                this.$emit('confirmdeletefile', fileId);
            },
            async processDeleteFile(fileId: number) {
                let response = await resourceData.deleteArticleFile(this.resourceVersionId, fileId);
                if (response) {
                    this.$store.commit('removeArticleFile', fileId);
                    this.localArticleDetail.files = _.filter(this.localArticleDetail.files, function(f) {
                        return f.fileId != fileId;
                    });
                }
            },
            setValidStatus() {
                this.$emit('isvalid', this.articleContent !== '');
            }
        },
        watch: {
            articleResourceVersionId(value) {
                this.setInitialValues();
            },
            imageResourceVersionId(value) {
                this.setInitialValues();
            }
        },
        validations: {
            articleContent: {
                required
            },
            uploadingFile: {
                file_size_validation,
                file_extension_validation,
                file_no_extension
            }
        }
    })

</script>
