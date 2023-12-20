export class UserBasicModel {
    id: number;
    userName: string;
    firstName: string;
    lastName: string;
    emailAddress: string;
    lastUpdated: string;

    public constructor(init?: Partial<UserBasicModel>) {
        Object.assign(this, init);
    }
}

export class UserPersonalDetailsModel {
    userId: number;
    userName: string;
    firstName: string;
    lastName: string;
    preferredName: string;
    countryId: number;
    regionId: number;
    primaryEmailAddress: string;
    secondaryEmailAddress: string;

    public constructor(init?: Partial<UserPersonalDetailsModel>) {
        Object.assign(this, init);
    }
}

export class SecurityQuestion {
    disabled: boolean;
    group?: any;
    selected: boolean;
    text: string;
    value: string;
}

export class UserSecurityQuestion {
    id: number;
    userId: number;
    securityQuestionId: number;
    questionText: string;
    securityQuestionAnswerHash: string;
}

export class UserSecurityQuestionAnswerModel {
    securityQuestions: SecurityQuestion[];
    userSecurityQuestions: UserSecurityQuestion[];
}