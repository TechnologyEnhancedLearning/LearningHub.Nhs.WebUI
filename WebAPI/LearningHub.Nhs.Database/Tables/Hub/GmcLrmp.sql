CREATE TABLE hub.GmcLrmp
(
    GMC_Ref_No             nvarchar(50)  NOT NULL,
    Surname                nvarchar(255) NULL,
    Given_Name             nvarchar(255) NULL,
    Year_Of_Qualification  int           NULL,
    GP_Register_Date       nvarchar(255) NULL,
    Registration_Status    nvarchar(255) NULL,
    Other_Names            nvarchar(255) NULL,
    dateProcessed          datetime      NULL,
    action                 nchar(1)      NULL,

    CONSTRAINT PK_GmcLrmp 
        PRIMARY KEY CLUSTERED (GMC_Ref_No ASC)
);
GO