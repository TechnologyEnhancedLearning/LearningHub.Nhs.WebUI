<template>
    <div class="">
        <div class="">
            <div class="object-container">
                <h3>SCORM Object View</h3>
                <pre>{{scormDisplay}}</pre>
            </div>
        </div>
        <div>
            <div class="trace-container">
                <h3>Trace</h3> <button @click="showTextLog">Text log</button>
                <div class="trace-row" v-for="logItem in traceDisplay">
                    <div class="statement">{{logItem.statement}}</div>
                    <div class="col-result">{{logItem.result}}</div>
                </div>
            </div>
        </div>
        <div class="text-popup" v-show="textLogVisible">
            <button @click="textLogVisible=false">close</button>
            <textarea v-model="textLog" id="log"></textarea>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { ScormApiModel } from "../models/learningsessions/scormApiModel";
    import { ScormLogItem } from '../models/learningsessions/scormLogItem';

    export default Vue.extend({
        components: {
        },
        data() {
            return {
                scormApi: null as ScormApiModel,
                textLog: '',
                textLogVisible: false
            };
        },
        computed: {
            scormDisplay(): string {
                return JSON.stringify(this.scormApi.sco, null, ' ');
            },
            traceDisplay(): ScormLogItem[] {
                return this.scormApi.trace;
            }
        },
        created() {
            this.scormApi = (window as any).opener.API;
        },
        methods: {
            showTextLog() {
                this.textLog = '';
                this.traceDisplay.forEach((value) => {
                    this.textLog += value.statement + '\n';
                })
                this.textLogVisible = true;
            }
        }
    })
</script>
<style scoped>
    .object-container {
        /*width: 100%;*/
        overflow: auto;
        margin-bottom: 250px;
        height: calc(100vh - 300px);
        padding: 10px;
    }

    .trace-container {
        width: calc(100vw - 45px);
        overflow: auto;
        position: fixed;
        border: 4px solid blue;
        height: 250px;
        background-color: lightblue;
        color: black;
        bottom: 0;
        padding: 10px;
    }
        .trace-container .trace-row {
            display: flex;
        }
        .trace-container .statement {
            flex: 50%;
            min-width: 350px;
        }
        .trace-container .result {
            flex: 50%;
            min-width: 100px;
        }
    .text-popup {
        position: fixed;
        height: 75%;
        width: 75%;
        top: 12.5%;
        left: 12.5%;
    }
        .text-popup textarea {
            height: 100%;
            width: 100%;
        }
</style>
