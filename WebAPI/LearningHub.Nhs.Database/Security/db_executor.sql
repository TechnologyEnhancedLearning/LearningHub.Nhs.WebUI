CREATE ROLE [db_executor]
    AUTHORIZATION [dbo];
GO

-- Grant execute rights to the new role 
GRANT EXECUTE TO db_executor
GO