<template>
    <div>
        <div class="row">
            <div class="form-group col-12">
                <h3>Uploaded file</h3>
            </div>
        </div>
        <div class="row">
            <file-panel :file-id="localGenericFileDetail.file.fileId" :file-description="localGenericFileDetail.file.fileName" :file-size="localGenericFileDetail.file.fileSizeKb" @changefile="changeFile"></file-panel>
        </div>
        <div v-if="scormOption" class="row mt-5">
            <div class="col-12 text-danger">
                <div>
                    Please choose the resource type as Elearning/HTML for the SCORM/AICC elearning or HTML resources.
                </div>
            </div>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12 mb-2">
                <h3>Date authored <span class="optional">(optional)</span></h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12 mb-2">
                Enter only the year, month and year or full date that this resource was authored.<br />
                For example, 2018 03 28
            </div>
        </div>
        <div class="row authored-date">
            <div class="col-4 col-sm-3 col-md-2">
                <label for="year" class="mb-0">Year</label><br />
                <input type="text" id="year" name="year" aria-describedby="yearError" maxlength="4" v-bind:class="{ 'input-validation-error': $v.authoredYear.$invalid && $v.authoredYear.$dirty }" class="form-control" :placeholder="currentYear" v-model="authoredYear" @blur="setProperty('authoredYear', $event.target.value)" />
            </div>
            <div class="col-4 col-sm-3 col-md-2">
                <label for="month" v-bind:class="{ disabled: $v.authoredYear.$invalid || !authoredYear }" class="mb-0">Month</label><br />
                <input type="text" id="month" name="month" aria-describedby="monthError" maxlength="2" v-bind:class="{ 'input-validation-error': $v.authoredMonth.$invalid && $v.authoredMonth.$dirty }" class="form-control" :placeholder="currentMonth" v-model="authoredMonth" v-bind:disabled="$v.authoredYear.$invalid || !authoredYear" @blur="setProperty('authoredMonth', $event.target.value)" />
            </div>
            <div class="col-4 col-sm-3 col-md-2">
                <label for="day-of-month" v-bind:class="{ disabled: $v.authoredYear.$invalid || !authoredYear || $v.authoredMonth.$invalid || !authoredMonth }" class="mb-0">Day</label><br />
                <input type="text" id="day-of-month" aria-describedby="day-of-monthError" name="day-of-month" maxlength="2" v-bind:class="{ 'input-validation-error': $v.authoredDayOfMonth.$invalid && $v.authoredDayOfMonth.$dirty }" class="form-control" :placeholder="currentDayOfMonth" v-model="authoredDayOfMonth" v-bind:disabled="$v.authoredYear.$invalid || authoredYear == '' || $v.authoredMonth.$invalid || !authoredMonth" @blur="setProperty('authoredDayOfMonth', $event.target.value)" />
            </div>
        </div>
        <div class="error-text pt-3" v-if="!$v.authoredYear.between && $v.authoredYear.$dirty">
            <span class="text-danger" id="yearError" aria-live="polite">Use four numbers to enter the year.</span>
        </div>
        <div class="error-text pt-3" v-if="$v.authoredYear.between && !$v.authoredYear.maxValue && $v.authoredYear.$dirty">
            <span class="text-danger" id="yearError" aria-live="polite">The year this file was authored cannot be in the future.</span>
        </div>
        <div class="error-text pt-3" v-if="!$v.authoredYear.$invalid && (!$v.authoredMonth.between || !$v.authoredMonth.minLength) && $v.authoredMonth.$dirty">
            <span class="text-danger" id="monthError" aria-live="polite">Use two numbers to enter a valid month.</span>
        </div>
        <div class="error-text pt-3" v-if="!$v.authoredYear.$invalid && !$v.authoredMonth.month_in_past && $v.authoredMonth.$dirty">
            <span class="text-danger" id="monthError" aria-live="polite">The month this file was authored cannot be in the future.</span>
        </div>
        <div class="error-text pt-3" v-if="!$v.authoredMonth.$invalid && (!$v.authoredDayOfMonth.between || !$v.authoredDayOfMonth.minLength) && $v.authoredDayOfMonth.$dirty">
            <span class="text-danger" id="day-of-monthError" aria-live="polite">Use two numbers to enter a valid day.</span>
        </div>
        <div class="error-text pt-3" v-if="!$v.authoredMonth.$invalid && !$v.authoredDayOfMonth.dayOfMonth_in_past && $v.authoredDayOfMonth.$dirty">
            <span class="text-danger" id="day-of-monthError" aria-live="polite">The day this file was authored cannot be in the future.</span>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12 my-2">
                <h3 id="additionalinfo-label"><label for="additionalinfo">Additional information <span class="optional">(optional)</span></label></h3>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                Add any further information that is relevant to this resource or will help learners to use it,
                for example, how it was developed or what is required for it to be used.
            </div>
            <div class="col-12 mt-3">
                <textarea class="form-control nhsuk-textarea" id="additionalinfo" aria-labelledby="additionalinfo-label" rows="4" maxlength="250" v-model="additionalInformation" @change="setAdditionalInformation($event.target.value)"></textarea>
            </div>
            <div class="col-12 footer-text">
                You can enter a maximum of 250 characters
            </div>
        </div>

        <div class="col-12 mt-4 file-details">
            <div>
                <h3>ESR learning object link</h3>
                <p>Select an option below.</p>
                <div class="row my-2">
                    <div class="accordion col-12" id="accordion">
                        <div class="pt-0 pb-4">
                            <div class="heading" id="esrLinkInfo">
                                <div class="mb-0">
                                    <a href="#" class="collapsed text-decoration-skip" style="color:#005EB8;" data-toggle="collapse" data-target="#collapseEsrLinkInfo" aria-expanded="false" aria-controls="collapseEsrLinkInfo">
                                        <div class="accordion-arrow">More information on ESR learning object links</div>
                                    </a>
                                </div>
                            </div>
                            <div id="collapseEsrLinkInfo" class="collapse" aria-labelledby="esrLinkInfo" data-parent="#accordion">
                                <div class="content col-12">
                                    <p>
                                        Electronic Staff Record (ESR) users with the Learning Administration User Responsibility Profile (URP)
                                        can set up elearning content to play via ESR. Providing a link enables users with that URP to add this resource as a
                                        learning object in ESR. Select one of the three options to indicate if / how you would like the ESR link to be displayed
                                        to other users in the Learning Hub.
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <label class="checkContainer mr-0" style="margin-left:10px;">
                        <span>Don't display</span>
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localGenericFileDetail.esrLinkType" value="1" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)" checked>
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer ml-3" v-if="resourceCatalogueCount<=1">
                        <span>Display only to me</span>
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localGenericFileDetail.esrLinkType" value="2" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)">
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer ml-3" v-if="resourceCatalogueCount>1">
                        <span>Display only to me and other catalogue editors</span>
                        <input type="radio" name="esrLinkType" class="nhsuk-radios__input" v-model="localGenericFileDetail.esrLinkType" value="3" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)">
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer ml-3">
                        <span>Display to everyone</span>
                        <input type="radio" name="esrLinkType"class="nhsuk-radios__input"  v-model="localGenericFileDetail.esrLinkType" value="4" data-toggle="modal" data-target="#esrLinkModal" @click="showWarningModal($event.target.name, $event.target.value)">
                        <span class="radioButton"></span>
                    </label>
                </div>
            </div>
        </div>

        <div id="esrLinkModal" v-if="showEsrModal" class="modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
            <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                <div class="modal-content" v-if="showEsrModal">
                    <div class="modal-header mb-15">
                        <h2><i class="warningTriangle fas fa-exclamation-triangle mr-3"></i>Removal of ESR learning object link</h2>
                    </div>
                    <div class="modal-body">
                        <div class="model-body-container mb-25">
                            <div class="mb-3">
                                This resource has an ESR learning object link that is visible to other users. This means that the link may have been used to add this resource as a learning object so others can access it in ESR. By removing this:
                            </div>
                            <div>
                                <ul v-if="changedValue==='1'">
                                    <li class="mb-3">The link will no longer be available for others to copy and use.</li>
                                    <li>The resource will no longer be available to access via ESR if this link has been used to set it up as a learning object. The resource will only be available to access in the Learning Hub.</li>
                                </ul>
                                <ul v-if="changedValue!=='1'">
                                    <li>The link will no longer be available for everyone to copy and use.</li>
                                </ul>
                            </div>
                        </div>
                    </div>

                    <div class="modal-footer modal-footer--buttons" style="padding-left: 0px;padding-right: 0px;">
                        <button type="button" class="nhsuk-button nhsuk-button--secondary" data-dismiss="modal" @click="hideWarningModal()">Cancel</button>
                        <button type="button" class="nhsuk-button" data-dismiss="modal" @click="processChangeEsrLinkType">Continue</button>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import * as _ from "lodash";
    import { between, maxValue, minLength } from "vuelidate/lib/validators";
    import { GenericFileResourceModel, ResourceFileModel } from '../models/contribute/contributeResourceModel';
    import { FileTypeModel } from "../models/contribute/fileTypeModel";
    import LhDatePicker from '../datepicker.vue';
    import store from './contributeState';
    import FilePanel from './FilePanel.vue';
    import { ResourceType } from '../constants';

    const month_in_past = (value: number) => {
        if (!value) { return true; }
        let dt = new Date();
        let yearCurrent = store.state.genericFileDetail.authoredYear == dt.getFullYear();
        if (yearCurrent && value > (dt.getMonth() + 1)) {
            return false;
        } else {
            return true;
        }
    };

    const dayOfMonth_in_past = (value: number) => {
        if (!value) { return true; }
        let dt = new Date();
        let yearCurrent = store.state.genericFileDetail.authoredYear == dt.getFullYear();
        let monthCurrent = store.state.genericFileDetail.authoredMonth == (dt.getMonth() + 1);
        if (yearCurrent && monthCurrent && value > dt.getDate()) {
            return false;
        } else {
            return true;
        }
    };

    export default Vue.extend({
        components: {
            FilePanel,
            LhDatePicker
        },
        data() {
            return {
                file: null as File,
                localGenericFileDetail: null as GenericFileResourceModel,
                authoredYear: '' as string,
                authoredMonth: '' as string,
                authoredDayOfMonth: '' as string,
                additionalInformation: '' as string,
                previousEsrLinkType: 1 as number,
                showEsrModal: false as Boolean,
                fieldName: null as string,
                changedValue: null as string
            };
        },
        computed: {
            genericFileDetail(): GenericFileResourceModel {
                return this.$store.state.genericFileDetail;
            },
            genericFileResourceVersionId(): number {
                return this.$store.state.genericFileDetail.resourceVersionId;
            },
            fileUpdated(): ResourceFileModel {
                return this.$store.state.fileUpdated;
            },
            scormOption(): boolean {
                return this.localGenericFileDetail.file.fileName.endsWith('.zip')
                    && store.state.resourceDetail.resourceType != ResourceType.HTML;
            },
            currentYear(): number {
                let dt = new Date();
                return dt.getFullYear();
            },
            currentMonth(): string {
                if (this.$v.authoredYear.$invalid || !this.authoredYear) {
                    return '';
                } else {
                    let dt = new Date();
                    let month = '0' + (dt.getMonth() + 1).toString();
                    return month.substring(month.length - 2);
                }
            },
            currentDayOfMonth(): string {
                if (this.$v.authoredMonth.$invalid || !this.authoredMonth) {
                    return '';
                } else {
                    let dt = new Date();
                    let day = '0' + dt.getDate().toString();
                    return day.substring(day.length - 2);
                }
            },
            resourceCatalogueCount(): number {
                if (!this.$store.state.userCatalogues) {
                    return 0;
                } else {
                    return this.$store.state.userCatalogues.length;
                }
            },
            previousVersionExists(): boolean {
                return this.$store.state.previousVersionExists;
            }
        },
        created() {
            this.localGenericFileDetail = _.cloneDeep(this.genericFileDetail);
            this.previousEsrLinkType = this.localGenericFileDetail.esrLinkType;
            if (this.localGenericFileDetail.authoredYear) {
                this.authoredYear = this.localGenericFileDetail.authoredYear.toString();
            }
            if (this.localGenericFileDetail.authoredMonth) {
                this.authoredMonth = '0' + this.localGenericFileDetail.authoredMonth.toString();
                this.authoredMonth = this.authoredMonth.substring(this.authoredMonth.length - 2);
            }
            if (this.localGenericFileDetail.authoredDayOfMonth) {
                this.authoredDayOfMonth = '0' + this.localGenericFileDetail.authoredDayOfMonth.toString();
                this.authoredDayOfMonth = this.authoredDayOfMonth.substring(this.authoredDayOfMonth.length - 2);
            }
            this.additionalInformation = this.$store.state.resourceDetail.additionalInformation;
        },
        methods: {
            changeFile() {
                this.$emit('filechanged');
            },
            showWarningModal(field: string, value: string) {
                this.fieldName = field;
                this.changedValue = value
                if (!this.previousVersionExists || this.previousEsrLinkType <= (+value)) {
                    this.processChangeEsrLinkType();
                }
                else {
                    this.showEsrModal = true
                }
            },
            hideWarningModal() {
                this.localGenericFileDetail.esrLinkType = this.previousEsrLinkType;
                this.showEsrModal = false;
            },
            processChangeEsrLinkType() {
                let preventSave = false;
                // "this.genericFileDetail[field as keyof GenericFileResourceModel]" equivalent to "this.genericFileDetail[field]"
                // TypeScript syntax is needed because noImplicitAny is set to true in the tsconfig.json file
                let storedValue: string = '';
                if (this.genericFileDetail[this.fieldName as keyof GenericFileResourceModel] != null) {
                    storedValue = this.genericFileDetail[this.fieldName as keyof GenericFileResourceModel].toString();
                }
                if (!preventSave && storedValue != this.changedValue) {
                    this.$store.commit("saveGenericFileDetail", { field: this.fieldName, value: this.changedValue });
                }
                this.showEsrModal = false;
            },
            setProperty(field: string, value: string) {
                let preventSave = false;
                switch (field) {
                    case 'authoredYear':
                        if (this.localGenericFileDetail.authoredYear == 0) { this.localGenericFileDetail.authoredYear = null; }
                        this.$v.authoredYear.$touch();
                        preventSave = this.$v.authoredYear.$invalid;
                        break;
                    case 'authoredMonth':
                        //if (this.authoredMonth == '') { this.localGenericFileDetail.authoredMonth = null; }
                        this.$v.authoredMonth.$touch();
                        preventSave = this.$v.authoredMonth.$invalid;
                        break;
                    case 'authoredDayOfMonth':
                        //if (this.authoredDayOfMonth == 0) { this.localGenericFileDetail.authoredDayOfMonth = null; }
                        this.$v.authoredDayOfMonth.$touch();
                        preventSave = this.$v.authoredDayOfMonth.$invalid;
                        break;
                    default:
                }

                // "this.genericFileDetail[field as keyof GenericFileResourceModel]" equivalent to "this.genericFileDetail[field]"
                // TypeScript syntax is needed because noImplicitAny is set to true in the tsconfig.json file
                let storedValue: string = '';
                if (this.genericFileDetail[field as keyof GenericFileResourceModel] != null) {
                    storedValue = this.genericFileDetail[field as keyof GenericFileResourceModel].toString();
                }
                if (!preventSave && storedValue != value) {
                    this.$store.commit("saveGenericFileDetail", { field, value });
                }
            },
            setAdditionalInformation(value: string) {
                let field: string = 'additionalInformation';
                if (this.$store.state.resourceDetail.title != value) {
                    this.$store.commit("saveResourceDetail", { field, value });
                }
            },
            getFileType(fileName: string) {
                let fileExtension = fileName.split(".").pop();
                let fileTypes: FileTypeModel[] = this.$store.state.fileTypes;
                let fileType: FileTypeModel = fileTypes.find(ft => ft.extension == fileExtension);
                return fileType;
            }
        },
        validations: {
            authoredYear: {
                between: between(1000, 9999),
                maxValue: maxValue(new Date().getFullYear())
            },
            authoredMonth: {
                between: between(1, 12),
                minLength: minLength(2),
                month_in_past
            },
            authoredDayOfMonth: {
                between: between(1, 31),
                minLength: minLength(2),
                dayOfMonth_in_past
            }
        },
        watch: {
            genericFileResourceVersionId(value) {
                this.localGenericFileDetail = _.cloneDeep(this.genericFileDetail);
                this.additionalInformation = this.$store.state.resourceDetail.additionalInformation;
            },
            fileUpdated(value) {
                this.localGenericFileDetail.file = this.genericFileDetail.file;
            }
        }
    })

</script>
<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .checkContainer {
        padding-left: 28px;
        span{
                font-size:16px;
            }
    }

    .radioButton {
        height: 24px;
        width: 24px;
    }

    .radioButton:after {
        top: 4px;
        left: 4px;
        width: 12px;
        height: 12px;
    }

</style>