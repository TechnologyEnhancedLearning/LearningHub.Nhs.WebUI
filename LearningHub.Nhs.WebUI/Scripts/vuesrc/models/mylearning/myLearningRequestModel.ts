export class MyLearningRequestModel {
    searchText: string;

    weblink: boolean;
    file: boolean;
    video: boolean;
    article: boolean;
    image: boolean;
    audio: boolean;
    elearning: boolean;
    assessment: boolean;
    case: boolean;

    complete: boolean;
    incomplete: boolean;
    passed: boolean;
    failed: boolean;
    downloaded: boolean;

    timePeriod: string;
    startDate: Date;
    endDate: Date;

    skip: number;
    take: number;

    public constructor(init?: Partial<MyLearningRequestModel>) {
        Object.assign(this, init);
    }
}