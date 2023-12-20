CREATE TABLE [Migration].[FileStaging] (
    [Resource_Unique_Ref]    NVARCHAR (50)   NULL,
    [Resource_Title]         NVARCHAR (255)  NULL,
    [Description]            NVARCHAR (1024) NULL,
    [Sensitive_content_flag] NVARCHAR (10)   NULL,
    [Keywords]               NVARCHAR (2048) NULL,
    [Catalogue]              NVARCHAR (255)  NULL,
    [Licence]                NVARCHAR (100)  NULL,
    [File_name]              NVARCHAR (1024) NULL,
    [Year_Authored]          SMALLINT        NULL,
    [Month_Authored]         TINYINT         NULL,
    [Day_Authored]           TINYINT         NULL,
    [Additional_information] NVARCHAR (100)  NULL,
    [Author_Name_1]          NVARCHAR (100)  NULL,
    [Role_1]                 NVARCHAR (100)  NULL,
    [Organisation_1]         NVARCHAR (100)  NULL,
    [Author_Name_2]          NVARCHAR (100)  NULL,
    [Role_2]                 NVARCHAR (100)  NULL,
    [Organisation_2]         NVARCHAR (100)  NULL,
    [Author_Name_3]          NVARCHAR (100)  NULL,
    [Role_3]                 NVARCHAR (100)  NULL,
    [Organisation_3]         NVARCHAR (100)  NULL,
    [Published_Date]         DATE            NULL,
    [I_Am_The_Author]        NVARCHAR (10)   NULL
);



