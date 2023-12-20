import { PageStatus } from '../models/content/pageModel';
import moment from 'moment';
import { PageSectionStatus } from '../models/content/pageSectionModel';
import { PageSectionDetailModel } from '../models/content/pageSectionDetailModel';

const getPageStatusText = function getPageStatus(pageStatus: PageStatus): string {
    switch (pageStatus) {
        case PageStatus.Live:
            return 'Live';
        case PageStatus.EditsPending:
        default:
            return `Edits Pending`;
    }
}

const getPageHeaderStatusText = function getPageStatus(pageStatus: PageStatus): string {
    switch (pageStatus) {
        case PageStatus.Live:
            return 'live';
        case PageStatus.EditsPending:
        default:
            return `new draft`;
    }
}

const getPageStatusIcon = function getPageStatusIcon(pageStatus: PageStatus): string {
    switch (pageStatus) {
        case PageStatus.Live:
            return 'liveCircle fas fa-check-circle';
        case PageStatus.EditsPending:
        default:
            return 'warningTriangle fas fa-exclamation-triangle';
    }
}

const getPageSectionStatusText = function getPageStatus(pageSectionDetail: PageSectionDetailModel): string {
    switch (pageSectionDetail.pageSectionStatus) {
        case PageSectionStatus.Live:
            return 'Live';
        case PageSectionStatus.Draft:
            if (pageSectionDetail.deletePending) {
                return 'Delete Pending';
            }
            return `Edits Pending`;
        case PageSectionStatus.Processing:
            return `Video Processing`;
        case PageSectionStatus.ProcessingFailed:
            return `Processing Failed!`;
        case PageSectionStatus.Processed:
        default:
            return `Edits Pending`;
    }
}

const getPageSectionStatusIcon = function getPageStatusIcon(pageSectionDetail: PageSectionDetailModel): string {
    switch (pageSectionDetail.pageSectionStatus) {
        case PageSectionStatus.Live:
            return 'liveCircle fas fa-check-circle';
        case PageSectionStatus.Draft:
            if (pageSectionDetail.deletePending) {
                return 'delete-pending fas fa-trash';
            }
        case PageSectionStatus.Processing:
        case PageSectionStatus.ProcessingFailed:
        case PageSectionStatus.Processed:
        default:
            return 'warningTriangleGrey fas fa-exclamation-triangle';
    }
}
export const contentLib = {
    getPageStatusText,
    getPageHeaderStatusText,
    getPageStatusIcon,
    getPageSectionStatusText,
    getPageSectionStatusIcon    
};