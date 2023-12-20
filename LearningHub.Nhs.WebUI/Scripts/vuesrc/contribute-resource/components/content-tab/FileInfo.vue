<template>
    <span>
        {{ fileModel.fileName }} |
        <span class="no-wrap">
            File size: {{ formattedFileSize }}
        </span>
    </span>
</template>

<script lang="ts">
    import Vue, {PropOptions} from 'vue';

    import { FileSizeHelper } from "../../../helpers/fileSizeHelper";

    import { FileModel } from "../../../models/contribute-resource/files/fileModel";

    export default Vue.extend({
        props: {
            fileModel: { type: Object } as PropOptions<FileModel>,
            allowDownload: Boolean,
        },
        computed: {
            formattedFileSize(): string {
                if(this.fileModel?.partialFile?.totalFileSize){
                    return FileSizeHelper.getFormattedFileSize(this.fileModel.partialFile.totalFileSize);
                }
                return FileSizeHelper.getFormattedFileSize(this.fileModel.fileSizeKb * 1000);
            },
            downloadPath(): string {
                return this.fileModel.getDownloadResourceLink();
            }
        },
    })
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;
</style>