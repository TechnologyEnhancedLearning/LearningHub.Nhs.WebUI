import store from './contributeState';

export const file_size_validation = (value: any) => {
    if (!value) { return true; }
    let file = value; return file.size < store.state.contributeSettings.fileUploadSettings.fileUploadSizeLimit;
};
export const file_no_extension = (value: any) => {
    if (!value) { return true; }
    if (value.name.indexOf(".") == -1 || value.name.split(".")[value.name.split(".").length - 1] == undefined) { return false;}
    return true;
};
export const file_extension_validation = (value: any) => {

    if (!value) { return true; }
    let fileExtension = value.name.split(".").pop();
    let fileType = store.state.fileTypes.find(ft => ft.extension == fileExtension);
    return !fileType ? true : !fileType.notAllowed;
};
export const file_name_length_validation = (value: any) => {
    if (!value) { return true; }
    if (value.name.length > 255) { return false; }
    return true;
};
export const transcriptfile_extension_validation = (value: any) => {
    if (!value) { return true; }
    let fileExtension = value.name.split(".").pop();
    let fileType = ['txt', 'doc', 'docx', 'pdf'].find(ext => ext == fileExtension.toLowerCase());
    return fileType!=undefined;
};
export const closedcaptionsfile_extension_validation = (value: any) => {
    if (!value) { return true; }
    let fileExtension = value.name.split(".").pop();
    let fileType = ['vtt'].find(ext => ext == fileExtension.toLowerCase());
    return fileType != undefined;
};
