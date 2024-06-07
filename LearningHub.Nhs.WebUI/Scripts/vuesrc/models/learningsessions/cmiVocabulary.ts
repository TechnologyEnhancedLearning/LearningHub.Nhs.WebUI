export enum CMIDataType {
    CMIBlank = 0,
    CMIBoolean = 1,
    CMIDecimal = 2,
    CMIDecimalOrBlank = 3,
    CMIFeedback = 4,
    CMIIdentifier = 5,
    CMIInteger = 6,
    CMISInteger = 7,
    CMIString255 = 8,
    CMIString4096 = 9,
    CMITime = 10,
    CMITimeSpan = 11,
    Mode = 12,
    Status = 13,
    Exit = 14,
    Credit = 15,
    Entry = 16,
    Interaction = 17,
    Result = 18,
    TimeLimitAction = 19,
    CMIString64000 = 20
};

export enum CMIVocabularyMode {
    Normal = "normal",
    Review = "review",
    Browse = "browse"
};

export enum CMIVocabularyExit {
    TimeOut = "time-out",
    Suspend = "suspend",
    Logout = "logout",
    Empty = ""
};

export enum CMIVocabularyStatus {
    Passed = "passed",
    Completed = "completed",
    Failed = "failed",
    Incomplete = "incomplete",
    Browsed = "browsed",
    NotAttempted = "not attempted"
};

export enum CMIVocabularyCredit {
    Credit = "credit",
    NoCredit = "no-credit"
};

export enum CMIVocabularyEntry {
    AbInitio = "ab-initio",
    Resume = "resume",
    Empty = ""
};

export enum CMIVocabularyInteraction {
    TrueFalse = "true-false",
    Choice = "choice",
    FillIn = "fill-in",
    Matching = "matching",
    Performance = "performance",
    Likert = "likert",
    Sequencing = "sequencing",
    Numeric = "numeric",
};

export enum CMIVocabularyResult {
    Correct = "correct",
    Wrong = "wrong",
    Unanticipated = "unanticipated",
    Neutral = "neutral",
    CMIDecimal = "CMIDecimal"
};

export enum CMIVocabularyTimeLimitAction {
    ExitMessage = "exit,message",
    ExitNoMessage = "exit,no message",
    ContinueMessage = "continue,message",
    ContinueNoMessage = "continue,no message"
};