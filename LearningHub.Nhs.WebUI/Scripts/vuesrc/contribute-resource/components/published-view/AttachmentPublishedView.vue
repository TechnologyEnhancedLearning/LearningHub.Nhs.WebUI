<template>
    <div class="contribute-attachment d-flex align-items-center py-20 px-24">
        <div class="contribute-attachment--thumbnail">
            <AttachmentIcon :file-extension="attachment.getFileModel().getFileExtension()"/>
        </div>
        <div class="contribute-attachment--details">
            <FileInfo :file-model="attachmentFileModel"/>
        </div>
        <div class="contribute-attachment--download">
            <a :href="downloadPath" :download="attachmentFileModel.fileName">
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
    import FileInfo from "../content-tab/FileInfo.vue";
    import IconButton from "../../../globalcomponents/IconButton.vue";

    import { AttachmentModel } from "../../../models/contribute-resource/blocks/attachmentModel";
    import { FileModel } from "../../../models/contribute-resource/files/fileModel";
    
    export default Vue.extend({
        components: {
            AttachmentIcon,
            FileInfo,
            IconButton,
        },
        props: {
            attachment: { type: Object } as PropOptions<AttachmentModel>,
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