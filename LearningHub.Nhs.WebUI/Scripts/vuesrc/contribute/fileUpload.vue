<template>
    <div class="col-12">
        <div v-if="uploading">
            <transition name="modal">
                <div class="modal-mask">
                    <div class="modal-wrapper">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header text-center">
                                    <h4 class="modal-title">File upload</h4>
                                </div>
                                <div class="modal-body">
                                    <div class="p-4 model-body-container">
                                        <progress-bar bg-color="#ffffff" bar-color="#005EB8" :val="uploadPercentage" size="huge"></progress-bar>
                                        <p v-if="!processing">{{ uploadPercentage }}% uploaded</p>
                                    </div>
                                </div>
                                <div class="modal-footer" v-if="chunks>1">
                                    <button v-if="!processing && !canceling" type="button" class="btn btn-custom my-2" @click="cancelUpload">Cancel</button>
                                    <button v-if="processing" type="button" class="btn btn-custom my-2 disabled" disabled>Finalising upload</button>
                                    <button v-if="canceling" type="button" class="btn btn-custom my-2 disabled" disabled>Canceling</button>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </transition>
        </div>
    </div>
</template>

<script lang="ts">
    import axios from 'axios';
    import AxiosWrapper from '../axiosWrapper';
    import Vue from 'vue';
    import { FileChunkResponseModel } from '../models/contribute/fileChunkResponceModel';
    import { FileUploadSettingsModel } from '../models/contribute/contributeSettingsModel';
    import { FileUploadResult } from "../models/contribute/fileUploadResult";
    import { FileErrorTypeEnum } from './fileErrors';
    import { FileUploadTypeEnum } from './fileUploadTypes';
    import ProgressBar from 'vue-simple-progress';
    
    export default Vue.extend({
        props: {
            resourceVersionId: Number
        },
        components: {
            ProgressBar
        },
        data() {
            return {
                allowedThreads: 3,
                cancelToken: axios.CancelToken,
                source: axios.CancelToken.source(),
                activeThreads: 0,
                progressStore: new Map(),
                totalBytesUploaded: 0,
                fileChunkDetailId: 0,
                chunks: 0,
                remainingChunks: [] as Number[],
                file: null as File,
                fileName: '',
                fileUploadType: FileUploadTypeEnum.Unspecified,
                changeingFileId: 0,
                resourceType: 0,
                fileType: 0,
                uploadPercentage: 0,
                uploading: false,
                processing: false,
                canceling: false
            }
        },
        computed: {
            fileUploadSettings(): FileUploadSettingsModel {
                return this.$store.state.contributeSettings.fileUploadSettings;
            }
        },
        methods: {
            uploadResourceFile(file: File) {
                this.fileUploadType = FileUploadTypeEnum.Resource;
                this.uploadFile(file);
            },
            uploadArticleFile(file: File, changeingFileId: number) {
                this.fileUploadType = FileUploadTypeEnum.Article;
                this.changeingFileId = changeingFileId;
                this.uploadFile(file);
            },
            uploadResourceAttachedFile(file: File, fileType: number, resourceType: number) {
                this.fileUploadType = FileUploadTypeEnum.ResourceAttached;
                this.fileType = fileType;
                this.resourceType = resourceType;
                this.uploadFile(file);
            },
            uploadFile(file: File) {
                this.allowedThreads = this.fileUploadSettings.allowedThreads;
                this.activeThreads = 0;
                this.uploadPercentage = 0;
                this.totalBytesUploaded = 0;
                this.progressStore.clear(); // = new Map();
                this.processing = false;
                this.canceling = false;
                this.file = file;
                this.fileName = this.file.name;
                this.fileChunkDetailId = 0;
                this.chunks = Math.ceil(this.file.size / this.fileUploadSettings.chunkSize);
                this.remainingChunks = Array.from(Array(this.chunks).keys()).reverse();
                this.uploading = true;
                this.source = axios.CancelToken.source();

                if (this.chunks > 1) {
                    this.sendFileChunk();
                } else {
                    this.sendFile();
                }
            },
            async sendFileChunk() {
                if (this.activeThreads >= this.allowedThreads
                    ||
                    this.activeThreads > 0 && this.fileChunkDetailId == 0) {
                    return;
                }
                if (this.canceling) {
                    //console.log("Chunk upload cancelled");
                    return;
                }
                if (!this.remainingChunks.length) {
                    if (!this.activeThreads) {
                        //console.log("All chunks uploaded");
                        await this.RegisterChunkedFile();
                    }
                    return;
                }

                const chunkIndex: number = this.remainingChunks.pop() as number;
                const begin: number = chunkIndex * this.fileUploadSettings.chunkSize;
                const chunk = this.file.slice(begin, begin + this.fileUploadSettings.chunkSize);
                this.activeThreads += 1;

                let startTime = Date.now();
                //console.log("startTime: " + startTime);
                this.uploadChunk(chunk, chunkIndex)
                    .then(() => {
                        if (chunkIndex == 0) {
                            let endTime = Date.now();
                            let uploadTime = Math.ceil((endTime - startTime) / 1000);
                            let maxThreads = Math.floor((this.fileUploadSettings.timeoutSec / uploadTime));
                            if (maxThreads < this.allowedThreads) {
                                this.allowedThreads = (maxThreads == 0) ? 1 : maxThreads;
                            }
                            //console.log('Number of threads: ' + this.allowedThreads.toString());
                        }
                        this.activeThreads -= 1;
                        this.sendFileChunk();
                    })
                    .catch(() => {
                        this.activeThreads -= 1;
                        this.remainingChunks.push(chunkIndex);
                    });
                this.sendFileChunk();
            },
            async uploadChunk(fileChunk: Blob, chunkIndex: number): Promise<boolean> {
                const formData = new FormData();
                formData.append('fileChunkDetailId', this.fileChunkDetailId.toString());
                formData.append('chunkIndex', chunkIndex.toString());
                formData.append('chunkCount', this.chunks.toString());
                formData.append('resourceVersionId', this.resourceVersionId.toString());
                formData.append('fileChunk', fileChunk);
                formData.append('allowedThreads', this.allowedThreads.toString());
                formData.append('fileName', this.fileName);
                formData.append('fileSize', this.file.size.toString());
                //console.log('Post of file chunk ' + chunkIndex.toString() + ' started.');
                const self = this;
                return await AxiosWrapper.axios.post<FileChunkResponseModel>('/Contribute/UploadFileChunk', formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        },
                        onUploadProgress: function (progressEvent: ProgressEvent) {
                            self.progressStore.set(chunkIndex.toString(), progressEvent.loaded);
                            self.updateProgress();
                        }.bind(this),
                        cancelToken: this.source.token
                    }
                )
                    .then(response => {
                        //console.log('Post of file chunk ' + chunkIndex.toString() + ' complete.' + response);
                        this.fileChunkDetailId = response.data.fileChunkDetailId;
                        this.progressStore.delete(chunkIndex.toString());
                        this.totalBytesUploaded += fileChunk.size;
                        this.updateProgress()
                        return response.data.success;
                    })
                    .catch(e => {
                        if (axios.isCancel(e)) {
                            //console.log('Request canceled at chunk ' + chunkIndex.toString(), e.message);
                        } else {
                            this.$emit('childfileuploaderror', FileErrorTypeEnum.Custom, 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.');
                            //console.log('Error: uploadFileChunk:' + e);
                        }
                        this.progressStore.delete(chunkIndex.toString());
                        this.updateProgress()
                        return false;
                    });
            },
            async sendFile() {
                let formData = new FormData();
                this.uploading = true;
                this.uploadPercentage = 0;
                formData.append('file', this.file);
                formData.append('resourceVersionId', this.resourceVersionId.toString());                
                let self = this;

                let url = '';
                switch (this.fileUploadType) {
                    case FileUploadTypeEnum.Resource:
                        url = '/Contribute/UploadFile';
                        break;
                    case FileUploadTypeEnum.Article:
                        url = '/Contribute/UploadArticleFile';
                        formData.append('changeingFileId', this.changeingFileId.toString());
                        break;
                    case FileUploadTypeEnum.ResourceAttached:
                        url = '/Contribute/UploadResourceAttachedFile';
                        formData.append('resourceType', this.resourceType.toString());
                        formData.append('attachedFileType', this.fileType.toString());
                        break;
                    default:
                        return;
                }
                AxiosWrapper.axios.post(url,
                    formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        },
                        onUploadProgress: function (progressEvent: ProgressEvent) {
                            self.uploadPercentage = Math.round((progressEvent.loaded * 100) / progressEvent.total);
                        }.bind(this)
                    }
                ).then(response => {
                    if (!response.data.invalid) {
                        this.$emit('fileuploadcomplete', response.data);
                        this.processing = false;
                        this.uploading = false;
                    } else {                       
                        if (this.file.size > 0) {
                            if (response.data.invalid) {
                                this.$emit('childfileuploaderror', FileErrorTypeEnum.InvalidType, 'We do not support this file type on the Learning Hub.');
                            }
                            else {
                                this.$emit('childfileuploaderror', FileErrorTypeEnum.Custom, 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.');
                            }
                        }
                        else {
                            this.$emit('childfileuploaderror', FileErrorTypeEnum.InvalidType, 'We do not support this file type on the Learning Hub.');
                        }
                    }
                    this.uploading = false;
                })
                    .catch(e => {
                        if (this.file.size > 0) {
                            this.$emit('childfileuploaderror', FileErrorTypeEnum.Custom, 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.');
                        }
                        else {
                            this.$emit('childfileuploaderror', FileErrorTypeEnum.InvalidType, 'We do not support this file type on the Learning Hub.');
                        }                        
                        self.processing = false;
                        self.uploading = false;
                        throw e;
                    });
            },
            updateProgress() {
                let bytesInProgress = 0;
                this.progressStore.forEach((value, key) => {
                    bytesInProgress += value;
                });
                //console.log('this.totalBytesUploaded: ' + this.totalBytesUploaded.toString() + '. bytesInProgress: ' + bytesInProgress.toString() + '. this.file: ' + this.file.size.toString() + '.');
                this.uploadPercentage = Math.round((this.totalBytesUploaded + bytesInProgress) / this.file.size * 100);
            },
            async RegisterChunkedFile() {
                this.processing = true;
                var data = {
                    fileUploadType: this.fileUploadType,
                    fileChunkDetailId: this.fileChunkDetailId,
                    changeingFileId: this.changeingFileId,
                    resourceType: this.resourceType,
                    attachedFileType: this.fileType
                };
                return await AxiosWrapper.axios.post<FileUploadResult>('/api/Contribute/RegisterChunkedFile', data)
                    .then(response => {
                        this.$emit('fileuploadcomplete', response.data);
                        this.processing = false;
                        this.uploading = false;
                    })
                    .catch(e => {
                        this.processing = false;
                        this.uploading = false;
                        this.$emit('childfileuploaderror', FileErrorTypeEnum.Custom, 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.');
                        throw e;
                    });
            },
            async ProcessChunkedFile() {
                this.processing = true;
                var data = {
                    fileUploadType: this.fileUploadType,
                    fileChunkDetailId: this.fileChunkDetailId,
                    changeingFileId: this.changeingFileId,
                    resourceType: this.resourceType,
                    attachedFileType: this.fileType
                };
                return await AxiosWrapper.axios.post<FileUploadResult>('/api/Contribute/ProcessChunkedFile', data)
                    .then(response => {
                        this.$emit('fileuploadcomplete', response.data);
                        this.processing = false;
                        this.uploading = false;
                    })
                    .catch(e => {
                        this.processing = false;
                        this.uploading = false;
                        this.$emit('childfileuploaderror', FileErrorTypeEnum.Custom, 'There was a problem uploading this file to the Learning Hub. Please try again and if it still does not upload, contact the support team.');
                        throw e;
                    });
            },
            async cancelUpload() {
                this.canceling = true;
                this.processing = false;
                this.uploading = false;
                this.source.cancel('Operation canceled by the user.');

                if (this.fileChunkDetailId != 0) {
                    var data = {
                        fileChunkDetailId: this.fileChunkDetailId
                    };
                    return await AxiosWrapper.axios.post<FileUploadResult>('/api/Contribute/CancelChunkedFile', data)
                        .then(response => {
                            this.$emit('fileuploadcancelled');
                        })
                        .catch(e => {
                            //console.log('ProcessChunkedFile:' + e);
                            throw e;
                        });
                } else {
                    this.$emit('fileuploadcancelled');
                }
            }
        }
    })

</script>