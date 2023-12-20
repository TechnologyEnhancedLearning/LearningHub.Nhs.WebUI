//import { ScormApiModel } from "./models/scormApiModel";
import Vue from 'vue';
import ScormViewer from './ScormViewer.vue';

//const resourceReferenceId = Number((document.getElementById("ResourceReferenceId") as HTMLInputElement).value);
//(window as any).API = new ScormApiModel(resourceReferenceId);
////console.log((window as any).API);

new Vue({
    el: '#scormtracecontainer',
    //data() {
    //    return {
    //        //scormApi: null as ScormApiModel
    //    }
    //},
    components: {
        ScormViewer
    },
    created() {
        //this.scormApi = (window as any).API;
    }
});

