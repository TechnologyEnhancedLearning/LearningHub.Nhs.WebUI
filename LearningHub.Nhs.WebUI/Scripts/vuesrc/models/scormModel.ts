import { FileModel } from './fileModel';
import { EsrLinkTypeEnum } from '../mycontributions/mycontributionsEnum';

export class ScormFileModel {
    resourceVersionId: number;
    fileId: number;   
    esrLinkType: EsrLinkTypeEnum;
    canDownload: boolean = false;
    useDefaultPopupWindowSize: boolean = true;
    popupHeight: number = 768;
    popupWidth: number = 1024;
    file: FileModel;

    public constructor(init?: Partial<ScormFileModel>) {
        Object.assign(this, init);
    }
}

export class ScormContentDetailsModel {
    externalReferenceId: number;
    esrLinkType: EsrLinkTypeEnum;    
    contentFilePath: string;
    externalReference: string;
    manifestUrl: string;    
    hostedContentUrl: string;
    isOwnerOrEditor: boolean;
    public constructor(init?: Partial<ScormContentDetailsModel>) {
        Object.assign(this, init);
    }
}