<template>
    <div>
        <div class="row mt-2">
            <div class="form-group col-12 mb-1">
                <h3 id="web-link-label">Add the address (URL) of the web link<i v-if="$v.localWeblinkDetail.url.$invalid || !urlIsAccessible" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                This can be found in the address bar of your browser, for example, https://www.england.nhs.uk
            </div>
            <div class="col-12 mt-3">
                <input type="text" aria-labelledby="web-link-label" placeholder="https://" class="form-control" v-bind:class="{ 'input-validation-error': (!urlIsAccessible || $v.localWeblinkDetail.url.$invalid) && $v.localWeblinkDetail.url.$dirty }" v-model="localWeblinkDetail.url" @change="setProperty('url', $event.target.value)" @input="urlKeyup" />
            </div>
        </div>
        <div class="error-text pt-3" v-if="!$v.localWeblinkDetail.url.url && $v.localWeblinkDetail.url.$dirty">
            <span class="text-danger">Enter a valid web link with http:// or https:// at the beginning.</span>
        </div>
        <div class="error-text pt-3" v-if="!urlIsAccessible && $v.localWeblinkDetail.url.$dirty">
            <span class="text-danger">The web link you have entered is not working. Please check the web page and try again.</span>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12 mb-1">
                <h3 id="text-to-display-label">Text to display <span class="optional">(optional)</span></h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                As a web link can sometimes be long and confusing,
                you can change the way it is shown to learners so that they understand what it is for, for example, NHS England.
            </div>
            <div class="col-12 mt-3">
                <input type="text" aria-labelledby="text-to-display-label" class="form-control" maxlength="50" v-model="localWeblinkDetail.displayText" @change="setProperty('displayText', $event.target.value)" />
            </div>
            <div class="col-12 footer-text">
                You can enter a maximum of 50 characters
            </div>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12">
                <h3 id="additionalinfo-label">Additional information <span class="optional">(optional)</span></h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                Add any further information that is relevant to this resource or will help learners to use it,
                for example, how it was developed or what is required for it to be used.
            </div>
            <div class="col-12 mt-3">
                <textarea class="form-control" aria-labelledby="additionalinfo-label" rows="4" maxlength="250" v-model="additionalInformation" @change="setAdditionalInformation($event.target.value)"></textarea>
            </div>
            <div class="col-12 footer-text">
                You can enter a maximum of 250 characters
            </div>
        </div>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { required, url } from "vuelidate/lib/validators";
    import * as _ from "lodash";
    import { WeblinkResourceModel } from '../models/contribute/contributeResourceModel';
    import { url_is_accessible } from './urlValidation';

    export default Vue.extend({
        components: {
        },
        data() {
            return {
                weblinkDescription: '',
                localWeblinkDetail: { resourceVersionId: 0, url: '', displayText: '' } as WeblinkResourceModel,
                additionalInformation: '' as string,
                urlIsAccessible: true
            };
        },
        computed: {
            weblinkDetail(): WeblinkResourceModel {
                return this.$store.state.weblinkDetail;
            },
            weblinkResourceVersionId(): number {
                return this.$store.state.weblinkDetail.resourceVersionId;
            }
        },
        created() {
            this.setInitialValues();
        },
        methods: {
            setInitialValues() {
                this.localWeblinkDetail = _.cloneDeep(this.weblinkDetail);
                this.additionalInformation = this.$store.state.resourceDetail.additionalInformation;
                this.setValidStatus();
            },
            async setProperty(field: string, value: string) {
                let preventSave = false;
                switch (field) {
                    case 'url':
                        this.$v.localWeblinkDetail.url.$touch();
                        preventSave = this.$v.localWeblinkDetail.url.$invalid;
                        if (!preventSave) {
                            this.urlIsAccessible = await url_is_accessible(value);
                            preventSave = !this.urlIsAccessible;
                            this.setValidStatus();
                        }
                        break;
                }

                // "this.imageDetail[field as keyof ImageResourceModel]" equivalent to "this.imageDetail[field]"
                // TypeScript syntax is needed because noImplicitAny is set to true in the tsconfig.json file
                let storedValue: string = '';
                if (this.weblinkDetail[field as keyof WeblinkResourceModel] != null) {
                    storedValue = this.weblinkDetail[field as keyof WeblinkResourceModel].toString();
                }
                if (!preventSave && storedValue != value) {
                    this.$store.commit("saveWeblinkDetail", { field, value });
                    
                }
            },
            setAdditionalInformation(value: string) {
                let field: string = 'additionalInformation';
                if (this.$store.state.resourceDetail.title != value) {
                    this.$store.commit("saveResourceDetail", { field, value });
                }
            },
            urlKeyup() {
                this.$store.commit("setSpecificContentDirty", this.localWeblinkDetail.url != this.$store.state.weblinkDetail.url);
                this.setValidStatus();
            },
            setValidStatus() {                
                this.$emit('isvalid', (this.$v.localWeblinkDetail.url.url) && this.urlIsAccessible && this.localWeblinkDetail.url != '');
            }
        },
        watch: {
            weblinkResourceVersionId(value) {
                this.setInitialValues();
            }
        },
        validations: {
            localWeblinkDetail: {
                url: {
                    required,
                    url
                }
            }
        }
    })

</script>
