import $ from 'jquery';
import { ResourceType } from './constants';
import moment from 'moment';

const monthNames = ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];

 const scrollTo = function ScrollTo(id: string) : void {
        var top = $(id).first().offset().top;
        window.scroll({ top: top, left: 0, behavior: 'smooth' });
 }

const getIcon = function getIcon(extension: string): string {
    var iconUrl: string;

    switch (extension) {
        case "pdf":
            iconUrl = "pdf.svg";
        case "Image":
            iconUrl = "jpeg.svg";
        case "Word":
            iconUrl = "ms-word-doc.svg";
        default:
            iconUrl = "pdf.svg";
    }

    return iconUrl;
}

const getResourceTypeIconClass = function getResourceTypeIconClass(resourceType: ResourceType): any {
    var resourceTypeEnum = ResourceType[resourceType];
    switch (resourceTypeEnum) {
        case ResourceType[ResourceType.ARTICLE]:
            return ["fa-regular", "fa-file-alt"];
        case ResourceType[ResourceType.AUDIO]:
            return ["fa-solid", "fa-volume-up"];
        case ResourceType[ResourceType.EQUIPMENT]:
            return ["fa-solid", "fa-map-marker-alt"];
        case ResourceType[ResourceType.GENERICFILE]:
            return ["fa-regular", "fa-file"];
        case ResourceType[ResourceType.IMAGE]:
            return ["fa-regular", "fa-image"];
        case ResourceType[ResourceType.SCORM]:
            return ["fa-solid", "fa-cube"];
        case ResourceType[ResourceType.VIDEO]:
            return ["fa-solid", "fa-video"];
        case ResourceType[ResourceType.WEBLINK]:
            return ["fa-solid", "fa-globe"];
        case ResourceType[ResourceType.CASE]:
            return ["fa-solid", "fa-microscope"];
        default:
            return ["fa-regular", "fa-file"];
    }
}

const getDurationText = function (durationInMilliseconds: number): string{

    if (durationInMilliseconds) {
        // Azure media player rounds duration to nearest second. e.g. 8:59.88 becomes 9:00. LH needs to match.
        // Moment.js can't round to the nearest second, so do that first with the raw millisecond value.
        var nearestSecond = Math.round(durationInMilliseconds / 1000) * 1000;

        let duration = "";

        // If duration greater than an hour, don't return the seconds part.
        if (nearestSecond >= 60 * 60 * 1000) {
            duration = moment.utc(nearestSecond).format("h[ hr ]m[ min ]");

            //Exclude "0 min" from the return value.
            if (duration.endsWith(" 0 min ")) {
                duration = duration.replace("0 min ", "");
            }
        }
        else {
            duration = moment.utc(nearestSecond).format("m[ min ]s[ sec ]");

            //Exclude "0 min" and "0 sec" from the return value.
            if (duration.startsWith("0 min ")) {
                duration = duration.replace("0 min ", "");
            }

            if (duration.endsWith(" 0 sec ")) {
                duration = duration.replace("0 sec ", "");
            }
        }

        if (duration == " ") {
            return ""; 
        }

        return duration;
    }
    else {
        return "";
    }
}
const getPrettifiedResourceTypeName = function getPrettifiedResourceTypeName(resourceType: ResourceType, durationInMilliSeconds: number) {
    var resourceTypeEnum = ResourceType[resourceType];

    switch (resourceTypeEnum) {
        case ResourceType[ResourceType.ARTICLE]:
            return "Article";
        case ResourceType[ResourceType.AUDIO]:
            return getDurationText(durationInMilliSeconds) + " audio";
        case ResourceType[ResourceType.EQUIPMENT]:
            return "Equipment";
        case ResourceType[ResourceType.IMAGE]:
            return "Image";
        case ResourceType[ResourceType.SCORM]:
            return "e-learning package";
        case ResourceType[ResourceType.VIDEO]:
            return getDurationText(durationInMilliSeconds) + " video";
        case ResourceType[ResourceType.WEBLINK]:
            return "Web link";
        case ResourceType[ResourceType.GENERICFILE]:
            return "File";
        default:
            return "File";
    }
}
const getAuthoredDate = function getAuthoredDate(day: number, month: number, year : number): string {

    let monthName;

    if (month)
        monthName = monthNames[month - 1];

    let authdate = [day, monthName, year].filter(Boolean).join(' ');    

    return authdate;
}


const getDurationHhmmss = function (seconds: number): string {
    var minutes = Math.floor(seconds / 60);
    seconds = seconds % 60;
    var hours = Math.floor(minutes / 60)
    minutes = minutes % 60;

    let duration = "";
    if (hours > 0) {
        duration = pad(hours) + ":";
    }

    duration += pad(minutes) + ":" + pad(seconds);
    return duration
}

const pad = function (num: number): string {
    return ("0" + num).slice(-2);
}

const capitalize = (title : string) => {
    if (typeof title !== 'string') return ''
    return title.charAt(0).toUpperCase() + title.slice(1)
}

const getGenericFileButtonTitle = function getGenericFileButtonText(extension: string): string {

    var title = "View this file";

    if (extension.toLowerCase() == "zip") {
        title = "Download this file";
    }

    return title;
}

const getResourceTypeText = function (resourceType: ResourceType): string {
    switch (resourceType) {
        case ResourceType.ARTICLE:
            return "Article"
        case ResourceType.AUDIO:
            return "Audio";
        case ResourceType.EMBEDDED:
            return "Embedded";
        case ResourceType.EQUIPMENT:
            return "Equipment";
        case ResourceType.GENERICFILE:
            return "File";
        case ResourceType.IMAGE:
            return "Image";
        case ResourceType.SCORM:
            return "e-learning package";
        case ResourceType.VIDEO:
            return "Video";
        case ResourceType.WEBLINK:
            return "Web link";
        default:
            return "";
    }
}

const getResourceTypeVerb = function (resourceType: ResourceType): string {
    switch (resourceType) {
        case ResourceType.ARTICLE:
            return "Read"
        case ResourceType.AUDIO:
            return "Played";
        case ResourceType.EMBEDDED:
            return "";
        case ResourceType.EQUIPMENT:
            return "Used equipment/visited facility";
        case ResourceType.GENERICFILE:
            return "Downloaded";
        case ResourceType.IMAGE:
            return "Viewed";
        case ResourceType.SCORM:
            return "Downloaded";
        case ResourceType.VIDEO:
            return "Played";
        case ResourceType.WEBLINK:
            return "Visited";
        default:
            return "";
    }
}

const stripHTMLFromString = function stripHTMLFromString(originalString: string): string {
    if (!originalString)
        return '';

    let txtField = document.createElement('textarea');
    txtField.innerHTML = originalString.replace(/(<([^>]+)>)/gi, "");
    return txtField.textContent;
}

export const commonlib = {
    scrollTo,
    getIcon,
    getAuthoredDate,
    getDurationText,
    getDurationHhmmss,
    getGenericFileButtonTitle,
    getResourceTypeText,
    getResourceTypeVerb,
    getResourceTypeIconClass,
    getPrettifiedResourceTypeName,
    stripHTMLFromString
};

