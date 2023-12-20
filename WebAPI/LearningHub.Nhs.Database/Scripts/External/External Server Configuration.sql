---- ** Development server configuration for External tables

------------------------------------------------------------
---- 'Master' database - ** Run on Master db
---- Create new Login
--CREATE LOGIN QueryUserProfile WITH PASSWORD = 'UserProfile12#45Six'
------------------------------------------------------------


------------------------------------------------------------
---- User Profile database - ** Run on UserProfile db
---- Create new user in the external db for the newly created Login
--CREATE USER QueryUserProfile FOR LOGIN QueryUserProfile
--GRANT SELECT ON dbo.[User] TO QueryUserProfile
------------------------------------------------------------


------------------------------------------------------------
---- LH database - ** Run on LH db
---- Master Key is required in database that uses External data source:
---- protects private keys
--CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'Master12#498Seven'; -- create master key

---- credential maps to a login or contained user used to connect to remote database 
--CREATE DATABASE SCOPED CREDENTIAL UserProfileCredential -- credential name
--WITH IDENTITY = 'QueryUserProfile',						-- login or contained user name
--SECRET = 'UserProfile12#45Six';							-- login or contained user password
--GO
 
---- data source to remote Azure SQL Database server and database
--CREATE EXTERNAL DATA SOURCE UserProfileExternal
--WITH
--(
--    TYPE=RDBMS,                           -- data source type
--    LOCATION='uks-learninghubnhsuk-dev-dbs.database.windows.net', -- Azure SQL Database server name
--    DATABASE_NAME='learninghubnhsuk-dev-user-db',         -- database name
--    CREDENTIAL=UserProfileCredential                -- credential used to connect to server / database  
--);
--GO
