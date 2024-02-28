import AxiosWrapper from './axiosWrapper';
import { ActivityStatus, MediaResourceActivityType, ResourceType } from './constants';
import { LearningHubValidationResultModel } from './models/learningHubValidationResultModel';
import 'navigator.sendbeacon';
import { AssessmentModel } from './models/contribute-resource/assessmentModel';
import { MatchQuestionState } from "./models/mylearning/matchQuestionState";

const recordActivityLaunched = async function (
    resourceType: ResourceType,
    resourceVersionId: number,
    nodePathId: number,
    activityDatetime: Date,
    extraAttemptReason?: string): Promise<LearningHubValidationResultModel> {

    var data = {
        resourceVersionId: resourceVersionId,
        nodePathId: nodePathId,
        activityStatus: (resourceType == ResourceType.ASSESSMENT || resourceType == ResourceType.VIDEO || resourceType == ResourceType.AUDIO ||
            resourceType == ResourceType.SCORM) ? ActivityStatus.Incomplete : ActivityStatus.Completed,
        activityStart: activityDatetime,
        extraAttemptReason
    };

    return await AxiosWrapper.axios.post<LearningHubValidationResultModel>('/api/activity/CreateResourceActivity', data)
        .then(response => {
            if (!response.data.isValid) {
                window.location.pathname = './Home/Error';
            }
            else if (!!response.data.details && response.data.details.includes("TooManyAttempts")) {
                window.location.pathname = './Resource/TooManyAttempts';
            }
            return response.data;
        })
        .catch(e => {
            console.log('recordActivityLaunched:' + e);
            throw e;
        });

};
const recordActivity = async function (
    resourceVersionId: number,
    nodePathId: number,
    activityStart: Date,
    activityEnd: Date,
    activityStatus: ActivityStatus,
    launchResourceActivityId?: number,
    mediaResourceActivityId?: number, // Must be populated if completing a media resource activity.
    sendBeacon: boolean = false): Promise<LearningHubValidationResultModel> {

    var data = {
        resourceVersionId: resourceVersionId,
        nodePathId: nodePathId,
        activityStatus: activityStatus,
        activityStart: activityStart,
        activityEnd: activityEnd,
        launchResourceActivityId: launchResourceActivityId,
        mediaResourceActivityId: mediaResourceActivityId
    };
    if (sendBeacon) {
        var blob = new Blob([JSON.stringify(data)], { type: 'application/json; charset=UTF-8' });
        navigator.sendBeacon('/api/activity/CreateResourceActivity', blob);
        return new Promise(() => { });
    }
    return await AxiosWrapper.axios.post<LearningHubValidationResultModel>('/api/activity/CreateResourceActivity', data)
        .then(response => {
            if (!response.data.isValid) {
                window.location.pathname = './Home/Error';
            }
            return response.data;
        })
        .catch(e => {
            console.log('recordActivityLaunched:' + e);
            throw e;
        });

};

const recordMediaResourceActivity = async function (resourceActivityId: number, activityDatetime: Date): Promise<LearningHubValidationResultModel> {

    var data = {
        resourceActivityId: resourceActivityId,        
        activityStart: activityDatetime        
    };

    return await AxiosWrapper.axios.post<LearningHubValidationResultModel>('/api/activity/CreateMediaResourceActivity', data)
        .then(response => {
            if (!response.data.isValid) {
                window.location.pathname = './Home/Error';
            }
            return response.data;
        })
        .catch(e => {
            console.log('recordMediaResourceActivity:' + e);
            throw e;
        });

};

const recordMediaResourceActivityInteraction = async function (mediaResourceActivityId: number, durationInSeconds: number, mediaResourceActivityType: MediaResourceActivityType, clientDateTime: Date): Promise<LearningHubValidationResultModel> {

    var data = {
        mediaResourceActivityId: mediaResourceActivityId,
        durationInSeconds: durationInSeconds,
        mediaResourceActivityType: mediaResourceActivityType    ,
        clientDateTime: clientDateTime
    };

    return await AxiosWrapper.axios.post<LearningHubValidationResultModel>('/api/activity/CreateMediaResourceActivityInteraction', data)
        .then(response => {
            if (!response.data.isValid) {
                window.location.pathname = './Home/Error';
            }
            return response.data;
        })
        .catch(e => {
            console.log('recordMediaResourceActivityInteraction:' + e);
            throw e;
        });

};

const recordActivityAndInteractionTogether = async function (
    resourceVersionId: number,
    nodePathId: number,
    activityStart: Date,
    activityEnd: Date,
    activityStatus: ActivityStatus,
    launchResourceActivityId: number,
    sendBeacon: boolean,
    mediaResourceActivityId: number,
    durationInSeconds: number,
    mediaResourceActivityType: MediaResourceActivityType,
    clientDateTime: Date): Promise<LearningHubValidationResultModel> {

    var data = {
        createResourceActivityViewModel: {
            resourceVersionId: resourceVersionId,
            nodePathId: nodePathId,
            activityStatus: activityStatus,
            activityStart: activityStart,
            activityEnd: activityEnd,
            launchResourceActivityId: launchResourceActivityId,
            mediaResourceActivityId: mediaResourceActivityId
        },
        createMediaResourceActivityInteractionViewModel: {
            mediaResourceActivityId: mediaResourceActivityId,
            durationInSeconds: durationInSeconds,
            mediaResourceActivityType: mediaResourceActivityType,
            clientDateTime: clientDateTime
        }
    };

    if (sendBeacon) {
        var blob = new Blob([JSON.stringify(data)], { type: 'application/json; charset=UTF-8' });
        navigator.sendBeacon('/api/activity/CreateResourceActivityAndMediaInteraction', blob);
        return new Promise(() => { });
    }
    return await AxiosWrapper.axios.post<LearningHubValidationResultModel>('/api/activity/CreateResourceActivityAndMediaInteraction', data)
        .then(response => {
            if (!response.data.isValid) {
                window.location.pathname = './Home/Error';
            }
            return response.data;
        })
        .catch(e => {
            console.log('recordActivityAndInteractionTogether:' + e);
            throw e;
        });

};

const recordAssessmentResourceActivity = async function (
    resourceActivityId: number,
    matchQuestions: MatchQuestionState[],
    extraAttemptReason?: string): Promise<LearningHubValidationResultModel> {
    const data = {
        resourceActivityId,
        extraAttemptReason,
        matchQuestions
    };

    return await AxiosWrapper.axios.post<LearningHubValidationResultModel>('/api/activity/CreateAssessmentResourceActivity', data)
        .then(response => {
            if (!response.data.isValid) {
                window.location.pathname = './Home/Error';
            }
            return response.data;
        })
        .catch(e => {
            console.log('recordAssessmentResourceActivity:' + e);
            throw e;
        });
}

const recordAssessmentResourceActivityInteraction = async function (
    assessmentResourceActivityId: number,
    questionNumber: number,
    answers: number[]): Promise<AssessmentModel> {
    const data = {
        assessmentResourceActivityId: assessmentResourceActivityId,
        questionNumber: questionNumber,
        answers: answers
    };

    return await AxiosWrapper.axios.post<AssessmentModel>('/api/activity/CreateAssessmentResourceActivityInteraction', data)
        .then(response => {
            if (!response.data) {
                window.location.pathname = './Home/Error';
            }
            return response.data;
        })
        .catch(e => {
            console.log('recordAssessmentResourceActivityInteraction:' + e);
            throw e;
        });
}

export const activityRecorder = {
    recordActivityLaunched,
    recordActivity,
    recordMediaResourceActivity,
    recordMediaResourceActivityInteraction,
    recordActivityAndInteractionTogether,
    recordAssessmentResourceActivity,
    recordAssessmentResourceActivityInteraction
};
