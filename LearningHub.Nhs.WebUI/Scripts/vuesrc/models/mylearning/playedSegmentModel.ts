export class PlayedSegmentModel {
    resourceId: number;
    userId: number;
    majorVersion: number;
    segmentStartTime: number;
    segmentEndTime: number;
    segmentDuration: number;
    played: boolean;
    percentage: number;

    public constructor(init?: Partial<PlayedSegmentModel>) {
        Object.assign(this, init);
    }
}