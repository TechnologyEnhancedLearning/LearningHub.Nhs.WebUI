import axios from 'axios';
import { MyLearningRequestModel } from '../models/mylearning/myLearningRequestModel';
import { ActivityDetailedModel } from '../models/mylearning/activityDetailedModel';
import { PlayedSegmentModel } from '../models/mylearning/playedSegmentModel';


const getActivitiesDetailed = async function (filters: MyLearningRequestModel): Promise<ActivityDetailedModel> {
    return await axios.post<ActivityDetailedModel>('/api/MyLearning/GetActivityDetailed', filters)
        .then(response => {
            return response.data;
        })
        .catch(e => {
            console.log('getActivitiesDetailed:' + e);
            throw e;
        });
};

const getPlayedSegments = async function (resourceId: number, majorVersion: number, mediaLengthInSeconds: number): Promise<PlayedSegmentModel[]> {
    // Adding a timestamp as a query string param to bust cache on IE11.
    return await axios.get<PlayedSegmentModel[]>('/api/MyLearning/GetPlayedSegments/' + resourceId + '/' + majorVersion + '?timestamp=' + new Date().getTime())
        .then(response => {

            let playedSegments = response.data;
            let allSegments = new Array<PlayedSegmentModel>()
            let currentTime = 0;

            for (var i = 0; i < playedSegments.length; i++) {
                let thisSegment = playedSegments[i];
                thisSegment.played = true;

                if (thisSegment.segmentStartTime > currentTime) {
                    allSegments.push(new PlayedSegmentModel({
                        segmentStartTime: currentTime,
                        segmentEndTime: thisSegment.segmentStartTime,
                        played: false
                        }));
                }

                allSegments.push(thisSegment);
                currentTime = thisSegment.segmentEndTime;
            }

            // Check if we need a not played segment at the end.
            if (currentTime < mediaLengthInSeconds) {
                allSegments.push(new PlayedSegmentModel({
                    segmentStartTime: currentTime,
                    segmentEndTime: mediaLengthInSeconds,
                    played: false
                }));
            }

            // Set percentage for div width setting in page.
            let total = 0;
            let totalSeconds = 0;
            allSegments.forEach(segment => {
                if (!segment.segmentDuration) { segment.segmentDuration = segment.segmentEndTime - segment.segmentStartTime; }
                segment.percentage = segment.segmentDuration / mediaLengthInSeconds * 100;
                total += segment.percentage;
                totalSeconds += segment.segmentDuration;
            });

            return allSegments;
        })
        .catch(e => {
            console.log('getPlayedSegments:' + e);
            throw e;
        });
};

export const myLearningData = {
    getActivitiesDetailed,
    getPlayedSegments
}
