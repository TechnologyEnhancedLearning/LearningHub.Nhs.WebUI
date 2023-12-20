<template>
    <div class="contribute-attachment d-flex align-items-center py-20 px-24">
        <div class="contribute-attachment--thumbnail">
            <AttachmentIcon :file-extension="attachment.getFileModel().getFileExtension()"/>
        </div>
        <div class="contribute-attachment--details">
            <FileUploader v-if="showFileUploader"
                          v-bind:file="attachmentFileModel"
                          v-bind:fileCategory="FileUploadType.Media"
                          v-on:newFileId="newFileId"/>
            <FileInfo v-else v-bind:file-model="attachmentFileModel"/>
        </div>
        <div class="contribute-attachment--download">
            <a v-bind:href="downloadPath" v-bind:download="attachmentFileModel.fileName">
                <IconButton iconClasses="fa-solid fa-download"
                            ariaLabel="Download attachment"
                            size="large"/>
            </a>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    
    import AttachmentIcon from "../../../globalcomponents/AttachmentIcon.vue";
    import FileInfo from "./FileInfo.vue";
    import { FileStore } from "../../../models/contribute-resource/files/fileStore";
    import FileUploader from "../../../globalcomponents/FileUploader.vue";
    import IconButton from "../../../globalcomponents/IconButton.vue";
    
    import { AttachmentModel } from "../../../models/contribute-resource/blocks/attachmentModel";
    import { FileModel } from "../../../models/contribute-resource/files/fileModel";
    import { FileUploadType } from '../../../helpers/fileUpload';
    
    export default Vue.extend({
        components: {
            AttachmentIcon,
            FileInfo,
            FileUploader,
            IconButton,
        },
        props: {
            attachment: { type: Object } as PropOptions<AttachmentModel>,
        },
        data(){
            return {
                FileUploadType: FileUploadType,
                FileStore: FileStore /* It looks like FileStore isn't used, but we need it to be exposed here to allow Vue to make the files list reactive */,
            }
        },
        computed: {
            attachmentFileModel(): FileModel {
                return this.attachment.getFileModel();
            },
            downloadPath(): string {
                return this.attachmentFileModel.getDownloadResourceLink();
            },
            showFileUploader(): boolean {
                const fileUploadComplete = this.attachment
                    && this.attachment.getFileModel()
                    && this.attachment.getFileModel().isUploadComplete();
                this.$emit("updatePublishingStatus");
                return !fileUploadComplete;
            },
        },
        methods: {
            newFileId(fileId: number) {
                // A fileId is available from the FileUploader
                // Set the fileId on this AttachmentModel
                this.attachment.setFileId(fileId);
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;
    
    .contribute-attachment {
        margin: 20px;
        border: 1px solid $nhsuk-grey-light;
        border-radius: 6px;
    
        &--thumbnail {
            flex-shrink: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 30px;
            height: 30px;
        }
    
        &--details {
            max-width: 756px;
            font-size: 16px;
            padding: 0 16px;
        }
        
        &--download {
            margin-left: auto;
    
            a i {
                font-size: 32px;
                font-weight: 200;
                color: $nhsuk-grey;
            }
        }
    }
</style>