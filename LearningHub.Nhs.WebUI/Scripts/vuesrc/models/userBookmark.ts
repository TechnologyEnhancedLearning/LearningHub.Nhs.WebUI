import { ResourceType } from '../constants';
export class UserBookmarkModel {
    id: number;
    parentId?: number = null;
    userId?: number;   
    bookmarkTypeId: BookmarkType;   
    resourceReferenceId?: number = null;   
    resourceTypeId?: ResourceType = ResourceType.UNDEFINED;
    nodeId?: number = null;   
    title: string;
    link: string;
    position?:number = 1;
    deleted?: boolean = false;
    childrenCount? : number = 0;
    expanded?: boolean = false;
    isEditMode?: boolean = false;
    moveInitiated?: boolean = false;   
    bookmarks? :UserBookmarkModel[];
    public constructor(init?: Partial<UserBookmarkModel>) {
        Object.assign(this, init);
         this.bookmarks = new Array<UserBookmarkModel>();         
    }
}

export class ToggleBookmarkModel {
    id: number;
    bookmarkType: BookmarkType;
    //actionType: BookmarkActionType;  
    title: string;
    link: string;
    resourceReferenceId?: number = null;
    nodeId?: number = null;
    isBookmarked: boolean = false;
    public constructor(init?: Partial<ToggleBookmarkModel>) {
        Object.assign(this, init);        
    }
}

export enum BookmarkType {
    FOLDER = 1,
    NODE = 2,
    RESOURCE = 3,
}

export enum BookmarkActionType {
    ADD = 1,    
    DELETE = 2
}