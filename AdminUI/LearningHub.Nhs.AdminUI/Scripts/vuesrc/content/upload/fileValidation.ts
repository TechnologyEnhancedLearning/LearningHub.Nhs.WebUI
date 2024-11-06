import store from '../contentState';

export const file_size_validation = (value: any) => {
    if (!value) { return true; }
    let file = value;
    return file.size < store.state.uploadSettings.fileUploadSettings.fileUploadSizeLimit;
};
export const file_extension_validation = (value: any) => {
    if (!value) { return true; }
    let fileExtension = value.name.split(".").pop();
    let fileType = ['mp4', 'avi', 'm4v', 'mov', 'mkv', 'mpg', 'm2v', 'vob','wmv'].find(ext => ext == fileExtension);
    return fileType != undefined;
};
export const transcriptfile_extension_validation = (value: any) => {
    if (!value) { return true; }
    let fileExtension = value.name.split(".").pop();
    let fileType = ['txt', 'doc', 'docx', 'pdf'].find(ext => ext == fileExtension.toLowerCase());
    return fileType != undefined;
};
export const closedcaptionsfile_extension_validation = (value: any) => {
    if (!value) { return true; }
    let fileExtension = value.name.split(".").pop();
    let fileType = ['vtt'].find(ext => ext == fileExtension.toLowerCase());
    return fileType != undefined;
};
export const thumbnailimagefile_extension_validation = (value: any) => {
    if (!value) { return true; }
    let fileExtension = value.name.split(".").pop();
    let fileType = ['jpg', 'jpeg', 'gif', 'png'].find(ext => ext == fileExtension.toLowerCase());
    return fileType != undefined;
};

export const thumbnailimagefile_size_validation = (value: any)  => {
    if (!value) { return false; }
    let reader = new FileReader();    
    reader.readAsDataURL(value);
    let img = new Image();
    
    store.state.isThumbnailFileValid = false;
    reader.onload = function (e) {        
        let src = e.target.result as string;
        img.onload = () => {            
            let isValid = (img.naturalWidth === 540 && img.naturalHeight === 280) || (img.width === 540 && img.height === 280);
            store.state.isThumbnailFileValid = isValid;
            return isValid;
        }
        img.src = src;
    }    
    reader.onerror = evt => {     
        return false;
    }
    return true;
};
