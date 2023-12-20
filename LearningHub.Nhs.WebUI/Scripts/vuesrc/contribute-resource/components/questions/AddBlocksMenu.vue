<template>
    <div class="block-menu-container">
        <Button @click="addMediaBlock(FileUploadType.Video)" class="nhsuk-u-margin-bottom-3">+ Add video</Button>
        <Button @click="addMediaBlock(FileUploadType.Image)">+ Add image</Button>
        <input type="file"
               ref="addMediaInput"
               multiple
               @change="uploadNewMediaFiles"
               class="visually-hidden" />
    </div>
</template>
<script lang="ts">
    import Vue from 'vue';
    import Button from '../../../globalcomponents/Button.vue';
    import { FileUploadType, startUploadsFromFileElement, getFileExtensionAllowedList } from '../../../helpers/fileUpload';

    export default Vue.extend({
        components: {
            Button,
        },

        data() {
            return { FileUploadType };
        },

        methods: {
            addMediaBlock(fileUploadType: FileUploadType) {
                let inputElement = this.$refs.addMediaInput as any;
                inputElement.accept = getFileExtensionAllowedList(fileUploadType).join(',');
                inputElement.click();
            },
            async uploadNewMediaFiles(event: Event): Promise<void> {
                startUploadsFromFileElement(
                    event.target as HTMLInputElement,
                    (fileId, mediaType) => this.$emit('add-media-block', fileId, mediaType)
                );
            },
        }
    });
</script>

<style lang="scss" scoped>
    .block-menu-container > button {
        margin-right: 20px;
    }
</style>
