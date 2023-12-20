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
            <div class="col-12">
                <div>
                    <label class="checkContainer">
                        This is a SCORM/AICC elearning resource
                        <input type="checkbox" v-model="localGenericFileDetail.scormAiccContent" @click="setProperty('scormAiccContent', $event.target.checked)">
                        <span class="checkmark"></span>
                    </label>
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
                <input type="text" id="year" maxlength="4" v-bind:class="{ 'input-validation-error': $v.authoredYear.$invalid && $v.authoredYear.$dirty }" class="form-control" :placeholder="currentYear" v-model="authoredYear" @blur="setProperty('authoredYear', $event.target.value)" />
            </div>
            <div class="col-4 col-sm-3 col-md-2">
                <label for="month" v-bind:class="{ disabled: $v.authoredYear.$invalid || !authoredYear }" class="mb-0">Month</label><br />
                <input type="text" id="month" maxlength="2" v-bind:class="{ 'input-validation-error': $v.authoredMonth.$invalid && $v.authoredMonth.$dirty }" class="form-control" :placeholder="currentMonth" v-model="authoredMonth" v-bind:disabled="$v.authoredYear.$invalid || !authoredYear" @blur="setProperty('authoredMonth', $event.target.value)" />
            </div>
            <div class="col-4 col-sm-3 col-md-2">
                <label for="day-of-month" v-bind:class="{ disabled: $v.authoredYear.$invalid || !authoredYear || $v.authoredMonth.$invalid || !authoredMonth }" class="mb-0">Day</label><br />
                <input type="text" id="day-of-month" maxlength="2" v-bind:class="{ 'input-validation-error': $v.authoredDayOfMonth.$invalid && $v.authoredDayOfMonth.$dirty }" class="form-control" :placeholder="currentDayOfMonth" v-model="authoredDayOfMonth" v-bind:disabled="$v.authoredYear.$invalid || authoredYear == '' || $v.authoredMonth.$invalid || !authoredMonth" @blur="setProperty('authoredDayOfMonth', $event.target.value)" />
            </div>
        </div>
        <div class="error-text pt-3" v-if="!$v.authoredYear.between && $v.authoredYear.$dirty">
            <span class="text-danger">Use four numbers to enter the year.</span>
        </div>
        <div class="error-text pt-3" v-if="$v.authoredYear.between && !$v.authoredYear.maxValue && $v.authoredYear.$dirty">
            <span class="text-danger">The year this file was authored cannot be in the future.</span>
        </div>
        <div class="error-text pt-3" v-if="!$v.authoredYear.$invalid && (!$v.authoredMonth.between || !$v.authoredMonth.minLength) && $v.authoredMonth.$dirty">
            <span class="text-danger">Use two numbers to enter a valid month.</span>
        </div>
        <div class="error-text pt-3" v-if="!$v.authoredYear.$invalid && !$v.authoredMonth.month_in_past && $v.authoredMonth.$dirty">
            <span class="text-danger">The month this file was authored cannot be in the future.</span>
        </div>
        <div class="error-text pt-3" v-if="!$v.authoredMonth.$invalid && (!$v.authoredDayOfMonth.between || !$v.authoredDayOfMonth.minLength) && $v.authoredDayOfMonth.$dirty">
            <span class="text-danger">Use two numbers to enter a valid day.</span>
        </div>
        <div class="error-text pt-3" v-if="!$v.authoredMonth.$invalid && !$v.authoredDayOfMonth.dayOfMonth_in_past && $v.authoredDayOfMonth.$dirty">
            <span class="text-danger">The day this file was authored cannot be in the future.</span>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12 my-2">
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
    import * as _ from "lodash";
    import { between, maxValue, minLength } from "vuelidate/lib/validators";
    import { GenericFileResourceModel, ResourceFileModel } from '../models/contribute/contributeResourceModel';
    import { FileTypeModel } from "../models/contribute/fileTypeModel";
    import LhDatePicker from '../datepicker.vue';
    import store from './contributeState';
    import FilePanel from './FilePanel.vue';

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
                additionalInformation: '' as string
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
                return this.localGenericFileDetail.file.fileName.endsWith('.zip');
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
            }
        },
        created() {
            this.localGenericFileDetail = _.cloneDeep(this.genericFileDetail);
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
