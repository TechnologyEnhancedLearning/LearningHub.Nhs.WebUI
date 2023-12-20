import { ScormApiModel } from "../models/learningsessions/scormApiModel";
const resourceReferenceId = Number((document.getElementById("ResourceReferenceId") as HTMLInputElement).value);
(window as any).API = new ScormApiModel(resourceReferenceId);
//console.log((window as any).API);
