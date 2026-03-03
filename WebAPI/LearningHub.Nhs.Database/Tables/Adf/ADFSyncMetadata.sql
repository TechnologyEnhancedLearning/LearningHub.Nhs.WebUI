CREATE TABLE ADFSyncMetadata (
    SyncDirection VARCHAR(50),   -- e.g., 'ELFHtoLH' or 'LHtoELFH'
    TableName     VARCHAR(100),  -- e.g., 'userTBL_Test', 'departmentTBL'
    LastSyncTime  DATETIME2,
    PRIMARY KEY (SyncDirection, TableName)
);
GO
