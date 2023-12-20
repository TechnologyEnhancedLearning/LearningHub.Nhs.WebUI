import { PageSectionModel } from "./pageSectionModel";

export class PageModel {
    id: number;
    name: string;
    pageStatus: PageStatus;
    canDiscard: boolean;
    canPreview: boolean;
    canPublish: boolean;
    hasHiddenSections: boolean;
    previewUrl: string;
    amendUserId: number;
    pageSections: PageSectionModel[];

    public constructor(init?: Partial<PageModel>) {
        Object.assign(this, init);
    }
}
export enum PageStatus {
    Live = 1,
    EditsPending = 2
}