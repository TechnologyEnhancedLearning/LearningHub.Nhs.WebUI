
<template>
    <div>
        <div class="input-group lh-datepicker">
            <input type="text" class="nhsuk-input" v-bind:class="{ invalid: !dateValid }" :aria-labelledby="labelText" v-model="displayedDate" @keyup="manualChange">
            <input type="hidden" :name="uniqueName" v-model="outputDate" />
            <div class="input-group-append" style="margin-top: -40px;margin-left:200px;">
                <vue-ctk-date-time-picker v-model="selectedDate"
                                          :no-value-to-custom-elem="true"
                                          :right="true"
                                          :format="dateTimeFormat"
                                          :onlyDate="displayStyle==1"
                                          :onlyTime="displayStyle==2"
                                          :no-header="true"
                                          :button-now-translation="buttonCaption"
                                          :button-color="'#005EB8'"
                                          :color="lhColor"
                                          @input="valueChanged">
                    <button type="button" class="date-select-button dateButton" aria-label="Choose Date"><i class="fas fa-calendar-alt" v-bind:style="{ color: lhColor, fontSize: '2.3rem' }"></i></button>
                </vue-ctk-date-time-picker>
            </div>
        </div>
    </div>
</template>
<style type="text/css">
    .dateButton{
        border : none;
        background-color : white;
    }
</style>
<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import VueCtkDateTimePicker from 'vue-ctk-date-time-picker';
    import 'vue-ctk-date-time-picker/dist/vue-ctk-date-time-picker.css';
    import moment from 'moment';

    export default Vue.extend({
        components: {
            VueCtkDateTimePicker
        },
        props: {
            displayStyle: String as PropOptions<string>,
            value: String as PropOptions<string>,
            uniqueName: String as PropOptions<string>,
            labelText: String as PropOptions<string>,
            useLocalTimezone: Boolean as PropOptions<boolean>
        },
        data() {
            return {
                selectedDate: null,
                outputDate: '',
                dateTimeFormat: 'DD/MM/YYYY',
                displayedDate: '',
                dateValid: false,
                allowedFormats: [],
                buttonCaption: 'Today',
                lhColor: '#005EB8',
                allowedDateFormats: [
                    moment.ISO_8601,
                    "D/MM/YYYY", "DD/MM/YYYY", "DD/MMM/YYYY", "D/MMM/YYYY", "DD/MMMM/YYYY", "D/MMMM/YYYY",
                    "DD MM YYYY", "D MM YYYY", "DD MMM YYYY", "D MMM YYYY", "DD MMMM YYYY", "D MMMM YYYY",
                    "DD/MM/YYYY HH:mm", "D/MM/YYYY HH:mm", "DD/MMM/YYYY HH:mm", "D/MMM/YYYY HH:mm", "DD/MMMM/YYYY HH:mm", "D/MMMM/YYYY HH:mm",
                    "DD MM YYYY HH:mm", "D MM YYYY HH:mm", "DD MMM YYYY HH:mm", "D MMM YYYY HH:mm", "DD MMMM YYYY HH:mm", "D MMMM YYYY HH:mm",
                    "DD/MM/YYYY HH:mm:ss", "D/MM/YYYY HH:mm:ss", "DD/MMM/YYYY HH:mm:ss", "D/MMM/YYYY HH:mm:ss", "DD/MMMM/YYYY HH:mm:ss", "D/MMMM/YYYY HH:mm:ss",
                    "DD/MM/YYYY HH:mm:s", "D/MM/YYYY HH:mm:s", "DD/MMM/YYYY HH:mm:s", "D/MMM/YYYY HH:mm:s", "DD/MMMM/YYYY HH:mm:s", "D/MMMM/YYYY HH:mm:s",
                    "DD/MM/YYYY HH:m:ss", "D/MM/YYYY HH:m:ss", "DD/MMM/YYYY HH:m:ss", "D/MMM/YYYY HH:m:ss", "DD/MMMM/YYYY HH:m:ss", "D/MMMM/YYYY HH:m:ss",
                    "DD/MM/YYYY HH:m:s", "D/MM/YYYY HH:m:s", "DD/MMM/YYYY HH:m:s", "D/MMM/YYYY HH:m:s", "DD/MMMM/YYYY HH:m:s", "D/MMMM/YYYY HH:m:s",
                    "DD MM YYYY HH:mm:ss", "D MM YYYY HH:mm:ss", "DD MMM YYYY HH:mm:ss", "D MMM YYYY HH:mm:ss", "DD MMMM YYYY HH:mm:ss", "D MMMM YYYY HH:mm:ss",
                    "DD MM YYYY HH:mm:s", "D MM YYYY HH:mm:s", "DD MMM YYYY HH:mm:s", "D MMM YYYY HH:mm:s", "DD MMMM YYYY HH:mm:s", "D MMMM YYYY HH:mm:s",
                    "DD MM YYYY HH:m:ss", "D MM YYYY HH:m:ss", "DD MMM YYYY HH:m:ss", "D MMM YYYY HH:m:ss", "DD MMMM YYYY HH:m:ss", "D MMMM YYYY HH:m:ss",
                    "DD MM YYYY HH:m:s", "D MM YYYY HH:m:s", "DD MMM YYYY HH:m:s", "D MMM YYYY HH:m:s", "DD MMMM YYYY HH:m:s", "D MMMM YYYY HH:m:s"
                ],
                allowedTimeFormats: [
                    "HH:mm", "HH:m", "H:mm", "H:m"
                ]
            };
        },
        methods: {
            valueChanged: function (dateValue: string) {
                this.displayedDate = dateValue;
                this.dateValid = moment(this.displayedDate, this.allowedFormats, true).isValid();
                if (this.dateValid || this.displayedDate == '') {
                    this.setOutputDate(this.displayedDate);
                }
            },
            manualChange: function () {
                this.dateValid = moment(this.displayedDate, this.allowedFormats, true).isValid();
                if (this.dateValid) {
                    this.selectedDate = moment(this.displayedDate, this.allowedFormats, true).format(this.dateTimeFormat);
                }
                if (this.dateValid || this.displayedDate == '') {
                    this.setOutputDate(this.displayedDate);
                }
                this.$emit('dateValid', this.dateValid);
            },
            setOutputDate: function (dateValue: string) {
                if (dateValue == '') {
                    this.outputDate = '';
                    this.$emit('input', '');
                } else {
                    if (this.useLocalTimezone) {
                        // Datepicker returns a date in local timezone of user.
                        var outputDate = moment(dateValue, this.allowedFormats).format();
                        this.$emit('input', outputDate);
                        this.$emit('dateValid', this.dateValid);
                    }
                    else {
                        // Datepicker returns a date in UTC timezone.
                        var outputDate = moment.utc(dateValue, this.allowedFormats).toISOString();
                        this.$emit('input', outputDate);
                        this.$emit('dateValid', this.dateValid);
                    }
                }
            },
            setInitialValue: function () {
                var initialDate = '';
                if (this.value != undefined && this.value != '' && this.value.indexOf('01/01/0001') == -1) {
                    initialDate = this.value;
                    // Remove any time offset
                    if (this.value.indexOf(' +') > -1) { initialDate = initialDate.substring(0, initialDate.indexOf(' +')); }
                    if (this.value.indexOf(' -') > -1) { initialDate = initialDate.substring(0, initialDate.indexOf(' -')); }
                    this.displayedDate = moment(initialDate, this.allowedDateFormats, true).format(this.dateTimeFormat);
                    this.setOutputDate(this.displayedDate);
                }
                else {
                    // Clear textbox if date value externally updated to null/undefined.
                    this.displayedDate = '';
                }
                this.selectedDate = this.displayedDate;
                this.dateValid = moment(this.displayedDate, this.allowedFormats, true).isValid();
            }
        },
        created() {
            switch (this.displayStyle) {
                case "0":
                    this.dateTimeFormat = 'DD/MM/YYYY HH:mm';
                    this.allowedFormats = this.allowedDateFormats;
                    this.buttonCaption = 'Now';
                    break;
                case "1":
                    this.dateTimeFormat = 'DD/MM/YYYY';
                    this.allowedFormats = this.allowedDateFormats;
                    this.buttonCaption = 'Today';
                    break;
                case "2":
                    this.dateTimeFormat = 'HH:mm';
                    this.allowedFormats = this.allowedTimeFormats;
                    break;
            }

            this.setInitialValue();
        },
        mounted() {
            this.$emit('dateValid', this.dateValid);
        },
        watch: {
            value: function (newVal) {
                let displayedDateValue = moment(this.displayedDate, this.allowedFormats, true).format(this.dateTimeFormat);
                let newValue = moment(newVal, this.allowedFormats, true).format(this.dateTimeFormat);
                if (displayedDateValue != newValue) { // Externally updated
                    this.setInitialValue();
                }
            }
        }
    })
</script>
