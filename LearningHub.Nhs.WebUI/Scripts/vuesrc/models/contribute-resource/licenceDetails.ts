export class LicenceDetails {
    id: number;
    name: string;
    url: string;
    terms: LicenceTerms;
    usersCanDetails: UsersCanDetails;
    usersMustDetails: UsersMustDetails;

    constructor(init?: Partial<LicenceDetails>) {
        Object.assign(this, init);
    }
}

export class LicenceTerms {
    allRightsReserved: boolean = false;
    attribution: boolean = false;
    creativeCommons: boolean = false;
    noDerivatives: boolean = false;
    nonCommercial: boolean = false;
    shareAlike: boolean = false;

    constructor(init?: Partial<LicenceTerms>) {
        Object.assign(this, init);
    }
}

export class UsersCanDetails {
    canView: boolean = false;
    canCopy: boolean = false;
    canDistribute: boolean = false;
    canAdapt: boolean = false;
    canDistributeAdaptations: boolean = false;
    canUseCommercially: boolean = false;

    constructor(init?: Partial<UsersCanDetails>) {
        Object.assign(this, init);
    }
}

export class UsersMustDetails {
    mustCreditOwner: boolean = false;
    mustApplyOriginalLicence: boolean = false;

    constructor(init?: Partial<UsersMustDetails>) {
        Object.assign(this, init);
    }
}