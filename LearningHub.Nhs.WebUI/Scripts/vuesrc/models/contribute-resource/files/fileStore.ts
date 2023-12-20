import _ from 'lodash';
import { resourceData } from '../../../data/resource';
import { FileModel } from './fileModel';

const files = [] as FileModel[];
let pollingIntervalHandle: number = undefined;

async function pollForChanges() {
    const fileIds = files
        .filter(file => file.fileId !== undefined)
        .filter(file => file.shouldPollForChanges())
        .map(file => file.fileId);

    if (fileIds.length > 0) {
        const newFiles: FileModel[] = await resourceData.getFileStatusDetails(fileIds);

        newFiles.forEach((newFile: FileModel) => {
            const oldFile: FileModel = _.find(files, (file) => file.fileId === newFile.fileId);
            oldFile.updateFromPolling(newFile);
        })
    }
};

export const FileStore = {
    _doNotUse: files /* The "files" variable must be publicly accessible (even if it's not used) to allow Vue to replace the list with it's reactive list */,

    getFile(fileId: number): FileModel {
        if (!fileId) {
            return undefined;
        }
        return _.find(files, (file) => file.fileId === fileId);
    },

    addFile(fileModel: FileModel): void {
        if (!this.getFile(fileModel.fileId)) {
            files.push(fileModel);
        }
    },

    enablePolling(): void {
        if (pollingIntervalHandle === undefined) {
            pollingIntervalHandle = window.setInterval(pollForChanges, 10 * 1000 /* poll once every 10 seconds */);
        }
    },

    disablePolling(): void {
        if (pollingIntervalHandle !== undefined) {
            window.clearInterval(pollingIntervalHandle);
            pollingIntervalHandle = undefined;
        }
    },

    getFilesWithOngoingUploads(): FileModel[] {
        return files.filter(file => file.hasOngoingFileUpload());
    }
};
