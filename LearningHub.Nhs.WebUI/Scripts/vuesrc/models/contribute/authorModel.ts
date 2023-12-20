export class AuthorModel {
    id: number;
    authorName: string;
    organisation: string;
    role: string;
    isContributor: boolean;
    resourceVersionId: number;

    public constructor(init?: Partial<AuthorModel>) {
        Object.assign(this, init);
    }
    get description() {
        var displayText = '';

        if (this.authorName != '' && this.organisation !== '') {
            displayText = this.authorName + ", " + this.organisation;
        }

        if (this.authorName != '' && this.organisation == '') {
            displayText = this.authorName;
        }

        if (this.authorName == '' && this.organisation != '') {
            displayText = this.organisation;
        }

        if (this.role != '') {
            displayText += ', ' + this.role;
        }

        return displayText;
    }
}