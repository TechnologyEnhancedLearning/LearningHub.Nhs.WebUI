<template>
    <div>

        <div class="">
            <div class="">
                <div class="object-container">
                    <h3>SCORM Object View</h3>
                    <pre>{{scormDisplay}}</pre>
                </div>
            </div>

            <div>
                <div class="trace-container">
                    <h3>Trace</h3>
                    <button @click="showTextLog">Text log</button>

                    <div class="trace-row" v-for="logItem in traceDisplay" :key="logItem.statement">
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

        <!-- LMS Error Modal -->
        <div id="lmsErrorModal" class="error-modal" v-show="showLmsErrorModal">
            <div class="modal-content">
                <h2>We’re having trouble connecting to the learning system</h2>

                <p>We couldn’t save your learning details.</p>

                <ul>
                    <li>Refresh your browser and try again</li>
                    <li>
                        If the error continues, email
                        <a href="mailto:support@learninghub.nhs.uk">
                            support@learninghub.nhs.uk
                        </a>
                        for help
                    </li>
                </ul>

                <button @click="closeLmsError">Close</button>
            </div>
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
                textLogVisible: false,
                showLmsErrorModal: false
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
            },
            closeLmsError() {
                this.showLmsErrorModal = false;
            }
        },
        mounted() {
            window.addEventListener('show-lms-error', () => {
                this.showLmsErrorModal = true;
            });
        },
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

    .error-modal {
        position: fixed;
        inset: 0;
        background: rgba(0,0,0,0.5);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 9999;
    }

    .modal-content {
        background: #fff;
        padding: 24px;
        border-radius: 8px;
        width: 500px;
        max-width: 90%;
        font-family: Arial, sans-serif;
    }

        .modal-content h2 {
            font-size: 28px;
            font-weight: bold;
            margin-bottom: 20px;
        }

        .modal-content p,
        .modal-content li {
            font-size: 18px;
            line-height: 1.6;
        }

        .modal-content button {
            margin-top: 20px;
            padding: 10px 18px;
        }
</style>
