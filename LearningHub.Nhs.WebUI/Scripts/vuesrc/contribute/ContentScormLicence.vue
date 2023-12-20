<template>
    <div class="mt-4" v-if="localScormDetail.file && localScormDetail.file.fileSizeKb > 0">        
        <h3>Scorm download</h3>
        <span>Select an option below.</span>
        <div class="row mt-4">
            <label class="checkContainer" style="margin-right:20px; margin-left:10px;">
                Unavailable
                <input type="radio" name="canDownload" v-model="localScormDetail.canDownload" v-bind:value="false" @click="setProperty($event.target.name, $event.target.value)" checked>
                <span class="radioButton"></span>
            </label>
            <label class="checkContainer">
                Available
                <input type="radio" name="canDownload" v-model="localScormDetail.canDownload" v-bind:value="true" @click="setProperty($event.target.name, $event.target.value)">
                <span class="radioButton"></span>
            </label>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import * as _ from "lodash";
    import { ScormResourceModel } from '../models/contribute/contributeResourceModel';
    import store from './contributeState';

    export default Vue.extend({
        components: {

        },
        data() {
            return {
                localScormDetail: null as ScormResourceModel,
            };
        },
        computed: {
            scormDetail(): ScormResourceModel {
                return store.state.scormDetail;
            },
            scormFileResourceVersionId(): number {
                return store.state.scormDetail.id;
            },
        },
        created() {
            this.localScormDetail = _.cloneDeep(this.scormDetail);
        },
        methods: {
            setProperty(field: string, value: string) {                       

                let preventSave = false;

                // "this.scormDetail[field as keyof ScormResourceModel]" equivalent to "this.scormDetail[field]"
                // TypeScript syntax is needed because noImplicitAny is set to true in the tsconfig.json file
                let storedValue: string = '';
                if (this.scormDetail[field as keyof ScormResourceModel] != null) {
                    storedValue = this.scormDetail[field as keyof ScormResourceModel].toString();
                }
                if (!preventSave && storedValue != value) {
                    this.$store.commit("saveScormDetail", { field, value });
                }
            },
        },
        validations: {

        },
        watch: {
            scormFileResourceVersionId(value) {
                this.localScormDetail = _.cloneDeep(this.scormDetail);
            },
        }
    })

</script>
