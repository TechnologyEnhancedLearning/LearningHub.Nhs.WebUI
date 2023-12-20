---- Insert new migration source for first South Central Content Server trust.

--IF NOT EXISTS(SELECT 1 FROM [migration].[MigrationSource] WHERE Description = 'FirstStagingTableTrust-TBC')
--BEGIN
--	INSERT INTO [migrations].[MigrationSource] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
--		VALUES (100, 'FirstStagingTableTrust-TBC',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
--END

--GO