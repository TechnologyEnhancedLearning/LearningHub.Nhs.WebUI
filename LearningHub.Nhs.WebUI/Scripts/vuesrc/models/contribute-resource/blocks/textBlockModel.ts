import { getRemainingCharactersFromHtml } from "../../../helpers/ckeditorValidationHelper";

export class TextBlockModel {
    content: string = "";
    maxLength: number = 0;
    richText: boolean = false;

    constructor(init?: Partial<TextBlockModel>) {
        if (init) {
            this.content = init.content;
        }
    }

    isReadyToPublish(): boolean {
        return !!this.content && // Returns true if the content has a value (i.e. not undefined, null or "")
            this.isLengthValid();
    }

    isLengthValid(): boolean {
        return this.maxLength == 0 ||
            (!this.richText && this.content.length <= this.maxLength) ||
            (this.richText && getRemainingCharactersFromHtml(this.maxLength, this.content) >= 0);
    }
};
