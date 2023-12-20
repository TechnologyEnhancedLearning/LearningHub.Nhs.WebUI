import { PageModel } from "./pageModel";

export class PageResultModel {
    pages: PageModel[];
    totalCount: number;

    public constructor(init?: Partial<PageResultModel>) {
        Object.assign(this, init);
    }
}
