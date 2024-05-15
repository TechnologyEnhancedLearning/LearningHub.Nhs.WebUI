<template>
    <div v-bind:class="{'form-group col-12': (isInlineWithMainContentBody)}">
        <div class="uploadBox" v-bind:class="{'p-4': (isInlineWithMainContentBody)}">
            <p class="genericFileUploaderText">
                You can upload a file from your computer or other storage drive you are connected to.
                Maximum file size {{ contributeSettings.fileUploadSettings.fileUploadSizeLimitText }}
            </p>
            <div v-if="!contributeResourceAVFlag">
                <div v-html="audioVideoUnavailableView"></div>
            </div>

            <div class="p-4 uploadInnerBox">
                <div class="upload-btn-wrapper nhsuk-u-font-size-16">
                    <label for="fileUpload" class="nhsuk-button nhsuk-button--secondary">Choose file</label> No file chosen
                    <input type="file" id="fileUpload" :accept="fileAccept" aria-label="Choose file" ref="fileUpload" v-on:change="onResourceFileChange" hidden />
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { resourceData } from '../data/resource';
    import { ResourceType, UploadResourceType } from '../constants';
    import { ContributeSettingsModel } from '../models/contribute/contributeSettingsModel';

    export default Vue.extend({
        props: {
            selectedUploadResourceType: { type: Number } as PropOptions<UploadResourceType>,
            onResourceFileChange: Function,
            isInlineWithMainContentBody: Boolean,
        },
        data() {
            return {
                uploadResourceType: UploadResourceType,
                contributeResourceAVFlag: true
            }
        },
        created() {
            this.getContributeResAVResourceFlag();
        },
        computed: {
            contributeSettings(): ContributeSettingsModel {
                return this.$store.state.contributeSettings;
            },
            fileAccept(): string {
                return this.$store.state.resourceDetail.resourceType == ResourceType.HTML ? '.zip,.rar,.7zip' : ''
            },
            audioVideoUnavailableView(): string {
                return this.$store.state.getAVUnavailableView;
            },
        },
        methods: {
            getContributeResAVResourceFlag() {
                resourceData.getContributeAVResourceFlag().then(response => {
                    this.contributeResourceAVFlag = response;
                });
            },
        }
    });
</script>

<style lang="scss" scoped>
    .genericFileUploaderText {
        margin-top: 0;
    }
</style>