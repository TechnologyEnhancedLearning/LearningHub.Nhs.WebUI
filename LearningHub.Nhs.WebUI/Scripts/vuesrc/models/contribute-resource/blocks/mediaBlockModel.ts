import { AttachmentModel } from './attachmentModel';
import { MediaTypeEnum } from './mediaTypeEnum';
import { ImageModel } from './imageModel';
import { VideoMediaModel } from './videoMediaModel';

export class MediaBlockModel {
    mediaType: MediaTypeEnum = undefined;
    attachment: AttachmentModel = undefined;
    image: ImageModel = undefined;
    video: VideoMediaModel = undefined;
    // Media blocks need this bool in order to update upon the media uploading.
    readyToPublish: boolean = undefined;

    constructor(init?: Partial<MediaBlockModel>) {
        if(init) {
            this.mediaType = init.mediaType as MediaTypeEnum;
            this.attachment = init.attachment ? new AttachmentModel(init.attachment) : undefined;
            this.image = init.image ? new ImageModel(init.image) : undefined;
            this.video = init.video ? new VideoMediaModel(init.video) : undefined;
        }
    }

    getIsReadyToPublish(): boolean {
        if (this.mediaType === MediaTypeEnum.Attachment) {
            return this.attachment && this.attachment.isReadyToPublish();
        }
        if (this.mediaType === MediaTypeEnum.Image) {
            return this.image && this.image.isReadyToPublish();
        }
        if (this.mediaType === MediaTypeEnum.Video) {
            return this.video && this.video.isReadyToPublish();
        }
        return false;
    }

    updatePublishingStatus(): void {
        this.readyToPublish = this.getIsReadyToPublish();
    }

    isReadyToPublish(): boolean {
        this.readyToPublish = this.getIsReadyToPublish();
        return this.readyToPublish;
    }
    
    dispose(): void {
        switch (this.mediaType) {
            case MediaTypeEnum.Attachment:
                this.attachment.dispose();
                break;
            case MediaTypeEnum.Image:
                this.image.dispose();
                break;
            case MediaTypeEnum.Video:
                this.video.dispose();
                break;
        }
    }
}
