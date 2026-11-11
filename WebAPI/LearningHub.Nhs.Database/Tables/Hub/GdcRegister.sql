CREATE TABLE hub.GdcRegister
(
    reg_number      nvarchar(50)  NOT NULL,
    Dentist         bit           NOT NULL 
                        CONSTRAINT DF_GdcRegister_Dentist DEFAULT (0),

    Title           nvarchar(50)  NULL,
    Surname         nvarchar(255) NULL,
    Forenames       nvarchar(255) NULL,
    honorifics      nvarchar(50)  NULL,
    house_name      nvarchar(255) NULL,
    address_line1   nvarchar(255) NULL,
    address_line2   nvarchar(255) NULL,
    address_line3   nvarchar(255) NULL,
    address_line4   nvarchar(255) NULL,
    Town            nvarchar(50)  NULL,
    County          nvarchar(50)  NULL,
    PostCode        nvarchar(50)  NULL,
    Country         nvarchar(50)  NULL,
    regdate         nvarchar(50)  NULL,
    qualifications  nvarchar(1000) NULL,
    dcp_titles      nvarchar(100) NULL,
    specialties     nvarchar(100) NULL,
    condition       nvarchar(50)  NULL,
    suspension      nvarchar(50)  NULL,

    dateProcessed   datetimeoffset(7) NOT NULL 
                        CONSTRAINT DF_GdcRegister_dateProcessed DEFAULT (sysdatetimeoffset()),

    action          nvarchar(1)   NULL,

    CONSTRAINT PK_GdcRegister 
        PRIMARY KEY CLUSTERED (reg_number ASC)
);
GO