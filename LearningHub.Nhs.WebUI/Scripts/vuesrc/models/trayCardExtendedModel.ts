import { ArticleModel } from './articleModel';
import { AudioModel } from './audioModel';
import { EmbedCodeModel } from './embedCodeModel';
import { EquipmentModel } from './equipmentModel';
import { ImageModel } from './imageModel';
import { GenericFileModel } from './genericFileModel';
import { WebLinkModel } from './webLinkModel';
import { VideoModel } from './videoModel';
import { TrayCardModel } from './trayCardModel';

export class TrayCardExtendedModel extends TrayCardModel {
    description: string;
    additionalInformation: string;  
    lastVersionUpdateDate: Date;
    versionDescription: string;
    publishedDate: Date;
    authorList: string[];
    authoredDate: Date; 
    resourceFree: boolean;
    cost: number;
    showFlagResourceLink: boolean;
    showActionPanel: boolean;    
    articleDetails: ArticleModel;
    audioDetails: AudioModel;
    videoDetails: VideoModel;
    embedCodeDetails: EmbedCodeModel;
    equipmentDetails: EquipmentModel;
    imageDetails: ImageModel;
    genericFileDetails: GenericFileModel;  
    webLinkDetails: WebLinkModel;  

    constructor(init?: Partial<TrayCardExtendedModel>) {
        super(init);
        Object.assign(this, init);
    }
}