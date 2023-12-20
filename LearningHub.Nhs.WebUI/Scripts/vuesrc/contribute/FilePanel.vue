<template>
    <div class="col-12">
        <div class="file-details">
            <div class="text-break">
                {{fileDescription}} {{fileSizeDisplay}}
            </div>
            <div style="display:flex;">
                <button class="btn btn-link" @click="changeFile()">Change</button>
                <span v-if="showDelete"> | </span>
                <button v-if="showDelete" class="btn btn-link" @click="deleteFile()">Delete</button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    export default Vue.extend({
        props: {
            fileId: Number as PropOptions<number>,
            fileDescription: String as PropOptions<string>,
            fileSize: Number as PropOptions<number>,
            showDelete: Boolean as PropOptions<boolean>
        },
        components: {
        },
        computed: {
            fileSizeDisplay(): string {
                if (this.fileSize < 1000) {
                    return '(' + this.fileSize.toString() + ' KB)';
                } else if (this.fileSize < 1000000) {
                    return '(' + (this.fileSize / 1000).toFixed(1) + ' MB)';
                } else {
                    return '(' + (this.fileSize / 1000000).toFixed(1) + ' GB)';
                }
            }
        },
        methods: {
            changeFile() {
                this.$emit('changefile', this.fileId);
            },
            deleteFile(fileId: number) {
                this.$emit('deletefile', this.fileId);
            }
        }
    })

</script>


