
function getFormattedFileSize(fileSize: number): string {
    let prefix: number;
    let suffix: string;

    const KILOBYTE_SIZE = 1000;
    const MEGABYTE_SIZE = 1000 * 1000;
    const GIGABYTE_SIZE = 1000 * 1000 * 1000;

    if (fileSize < KILOBYTE_SIZE) {
        prefix = fileSize;
        suffix = 'bytes';
    }
    else if (fileSize < MEGABYTE_SIZE) {
        prefix = fileSize / KILOBYTE_SIZE;
        suffix = 'KB';
    }
    else if (fileSize < GIGABYTE_SIZE) {
        prefix = fileSize / MEGABYTE_SIZE;
        suffix = 'MB';
    }
    else {
        prefix = fileSize / GIGABYTE_SIZE;
        suffix = 'GB';
    }

    return `${prefix.toPrecision(3)} ${suffix}`;
}

export const FileSizeHelper = {
    getFormattedFileSize,
}
