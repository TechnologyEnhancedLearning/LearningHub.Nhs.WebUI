export class FileTypeModel {
    id: number;
    defaultResourceTypeId: number;
    name: string;
    extension: string;
    notAllowed: boolean;
    icon: string;

    public constructor(init?: Partial<FileTypeModel>) {
        Object.assign(this, init);
    }
}