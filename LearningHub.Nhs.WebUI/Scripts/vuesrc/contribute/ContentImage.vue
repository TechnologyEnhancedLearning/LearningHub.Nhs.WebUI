<template>
    <div>
        <div class="row">
            <div class="form-group col-12">
                <h3>Uploaded file</h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                <file-panel :file-id="localImageDetail.file.fileId" :file-description="localImageDetail.file.fileName" :file-size="localImageDetail.file.fileSizeKb" @changefile="changeFile"></file-panel>
            </div>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12">
                <h3 id="alttag-label"><label for="alttag">Alt tag</label> <i v-if="$v.localImageDetail.altTag.$invalid" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                Provide a short description of the image.
                This will not be shown on screen but it will help those using screen readers.
            </div>
            <div class="col-12 mt-3">
                <textarea class="form-control" id="alttag" name="alttag" aria-labelledby="alttag-label" rows="4" maxlength="125" v-model="localImageDetail.altTag" @change="setProperty('altTag', $event.target.value)" @input="altTextKeyup"></textarea>
            </div>
            <div class="col-12 footer-text">
                You can enter a maximum of 125 characters
            </div>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12">
                <h3 id="additionalinfo-label"><label for="additionalinfo">Additional information <span class="optional">(optional)</span></label></h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                Add any further information that is relevant to this resource or will help learners to use it, 
                for example, how it was developed or what is required for it to be used.
            </div>
            <div class="col-12 mt-3">
                <textarea class="form-control" id="additionalinfo" aria-labelledby="additionalinfo-label" rows="4" maxlength="250" v-model="additionalInformation" @change="setAdditionalInformation($event.target.value)"></textarea>
            </div>
            <div class="col-12 footer-text">
                You can enter a maximum of 250 characters
            </div>
        </div>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { required } from "vuelidate/lib/validators";
    import * as _ from "lodash";
    import { ImageResourceModel, ResourceFileModel } from '../models/contribute/contributeResourceModel';
    import FilePanel from './FilePanel.vue';

    export default Vue.extend({
        components: {
            FilePanel
        },
        data() {
            return {
                localImageDetail: { resourceVersionId: 0,  altTag: '' } as ImageResourceModel,
                additionalInformation: '' as string
            };
        },
        computed: {
            imageDetail(): ImageResourceModel {
                return this.$store.state.imageDetail;
            },
            imageResourceVersionId(): number {
                return this.$store.state.imageDetail.resourceVersionId;
            },
            fileUpdated(): ResourceFileModel {
                return this.$store.state.fileUpdated;
            }
        },
        created() {
            this.setInitialValues();
        },
        methods: {
            changeFile() {
                this.$emit('filechanged');
            },
            setInitialValues() {
                this.localImageDetail = _.cloneDeep(this.imageDetail);
                this.additionalInformation = this.$store.state.resourceDetail.additionalInformation;
                this.setValidStatus();
            },
            setProperty(field: string, value: string) {
                // "this.imageDetail[field as keyof ImageResourceModel]" equivalent to "this.imageDetail[field]"
                // TypeScript syntax is needed because noImplicitAny is set to true in the tsconfig.json file
                let storedValue: string = '';
                if (this.imageDetail[field as keyof ImageResourceModel] != null) {
                    storedValue = this.imageDetail[field as keyof ImageResourceModel].toString();
                }    
                if (storedValue != value) {
                    this.$store.commit("saveImageDetail", { field, value });
                }
                this.setValidStatus();
            },
            setAdditionalInformation(value: string) {
                let field: string = 'additionalInformation';
                if (this.$store.state.resourceDetail.title != value) {
                    this.$store.commit("saveResourceDetail", { field, value });
                }
            },
            altTextKeyup() {
                this.$store.commit("setSpecificContentDirty", this.localImageDetail.altTag != this.$store.state.imageDetail.altTag);
                this.setValidStatus();
            },
            setValidStatus() {
                this.$emit('isvalid', this.localImageDetail.altTag !== '');
            }
        },
        watch:{
            imageResourceVersionId(value) {
                this.setInitialValues();
            },
            fileUpdated(value) {
                this.localImageDetail.file = this.imageDetail.file;
            }
        },
        validations: {
            localImageDetail: {
                altTag: {
                    required
                }
            }
        }
    })

</script>
