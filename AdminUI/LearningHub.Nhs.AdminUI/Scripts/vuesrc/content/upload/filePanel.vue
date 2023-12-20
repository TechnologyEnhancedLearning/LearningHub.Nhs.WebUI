<template>
    <div class="col-12">
        <div class="file-details" v-if="fileDetails && fileDetails.fileId">
            <div>{{fileDetails.fileName}} {{ fileSizeDisplay }}</div>
            <div>
                <button class="btn btn-link" @click="changeFile()">Change</button>
                <span v-if="showDelete"> | </span>
                <button v-if="showDelete" class="btn btn-link" @click="deleteFile()">Delete</button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { FileModel } from '../../models/content/fileModel';
    export default Vue.extend({
        props: {
            file: Object,
            showDelete: Boolean
        },
        data() {
            return {
                fileDetails: null as FileModel
            }
        },
        components: {
        },
        created() {
            this.fileDetails = this.file as FileModel;
        },
        computed: {            
            fileSizeDisplay(): string {
                if (!this.file)
                    return "";
                if (this.fileDetails.fileSizeKb < 1000) {
                    return '(' + this.fileDetails.fileSizeKb.toString() + ' KB)';
                } else if (this.fileDetails.fileSizeKb < 1000000) {
                    return '(' + (this.fileDetails.fileSizeKb / 1000).toFixed(1) + ' MB)';
                } else {
                    return '(' + (this.fileDetails.fileSizeKb / 1000000).toFixed(1) + ' GB)';
                }
            }
        },
        methods: {
            updateFileDetails(file: FileModel) {
                this.fileDetails = file;
            },
            changeFile() {
                this.$emit('changefile', this.fileDetails.fileId);
            },
            deleteFile(fileId: number) {
                this.$emit('deletefile', this.fileDetails.fileId);
            }
        }
    })

</script>
<style lang="scss">
    .btn-link {
        font-family: Frutiger LT Std;
        font-style: normal;
        font-weight: normal;
        font-size: 16px;
        line-height: 28px;
        text-decoration-line: underline;
        font-family: FrutigerLT55Roman, Arial, sans-serif; /*$font-stack*/
        color: #DA291C;
    }
    .file-details {
        display: flex;
        justify-content: space-between;
        background-color: #fff;
        border-radius: .5rem;
        padding: 2.5rem;
    }
</style>