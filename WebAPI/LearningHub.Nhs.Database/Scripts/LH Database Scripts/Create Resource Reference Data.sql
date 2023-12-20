

IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 0)
BEGIN
	INSERT INTO [resources].[ResourceType]([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
	VALUES (0, 'Undefined', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 1)
BEGIN
	INSERT INTO [resources].[ResourceType]([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
	VALUES (1, 'Article', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 2)
BEGIN
	INSERT INTO [resources].[ResourceType]([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
	VALUES (2, 'Audio', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 3)
BEGIN
	INSERT INTO [resources].[ResourceType]([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
	VALUES (3, 'Embedded', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 4)
BEGIN
	INSERT INTO [resources].[ResourceType]([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
	VALUES (4, 'Equipment', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 5)
BEGIN
	INSERT INTO [resources].[ResourceType]([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
	VALUES (5, 'Image', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 6)
BEGIN
	INSERT INTO [resources].[ResourceType]([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (6, 'SCORM/AICC resource', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 7)
BEGIN
	INSERT INTO [resources].[ResourceType]([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (7, 'Video', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 8)
BEGIN
	INSERT INTO [resources].[ResourceType]([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (8, 'Web link', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 9)
BEGIN
	INSERT INTO [resources].[ResourceType]([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (9, 'File', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
GO

IF NOT EXISTS(SELECT 1 FROM resources.FileType)
BEGIN
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 0, 9, 'Unknown','unknown','', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 1, 9, 'Office Word','Office Word','doc', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 2, 9, 'Office Word - Newer versions','Office Word - Newer versions','docx', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 3, 9, 'Adobe Acrobat','Adobe Acrobat','pdf', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 4, 9, 'Open Office Writer','Open Office Writer','odt', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 5, 9, 'Office Excel','Office Excel','xls', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 6, 9, 'Office Excel - Newer versions','Office Excel - Newer versions','xlsx', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 7, 9, 'Office Excel with Macros','Office Excel with Macros','xlm', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 8, 9, 'Office Excel with Macros - Newer versions','Office Excel with Macros - Newer versions','xlsm', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 9, 9, 'Open Office Calc','Open Office Calc','ods', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 10, 9, 'Office PowerPoint','Office PowerPoint','ppt', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 11, 9, 'Office PowerPoint','Office PowerPoint','pptx', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 12, 9, 'Open Office Impress','Open Office Impress','odp', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 13, 9, 'Apple Keynote','Apple Keynote','key', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 14, 2, 'Audio','Audio','mp3', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 15, 2, 'Waveform Audio File','Waveform Audio File','wav', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 16, 2, 'Windows Media Audio','Windows Media Audio','wma', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 17, 7, 'Video','Video','mp4', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 18, 7, 'Audio Video Interleave','Audio Video Interleave','avi', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 19, 7, 'Apple','Apple','m4v', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 20, 7, 'Quicktime','Quicktime','mov', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 21, 7, 'Commonly used for larger disc rips','Commonly used for larger disc rips','mkv', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 22, 7, 'MPEG','MPEG','mpg', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 23, 7, 'MPEG','MPEG','mpeg', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 24, 7, 'Windows Media Video','Windows Media Video','wmv', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 25, 5, 'Bitmap','Bitmap','bmp', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 26, 5, 'Graphics Interchange Format','Graphics Interchange Format','gif', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 27, 5, 'Commonly used graphic file','Commonly used graphic file','jpeg', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 28, 5, 'Commonly used graphic file','Commonly used graphic file','jpg', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 29, 5, 'Portable Network Graphics','Portable Network Graphics','png', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 30, 5, 'Photoshop file','Photoshop file','psd', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 31, 5, 'Tagged Image File Format','Tagged Image File Format','tiff', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 32, 9, 'Apple word processor','Apple word processor','pages', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 33, 9, 'Apple spreadsheet application','Apple spreadsheet application','numbers', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 34, 9, 'Hypertext Markup Language File','Hypertext Markup Language File','html', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 35, 9, 'Hypertext Markup Language File','Hypertext Markup Language File','htm', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 36, 9, 'Dynamic HTML','Dynamic HTML','dhtml', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 37, 9, 'Compressed (ZIP) file','Compressed (ZIP) file', 'zip', 0, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())

	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 38, 7, 'Adobe Flash','Adobe Flash', 'flv', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 39, 7, 'Adobe Flash','Adobe Flash', 'f4v', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 40, 7, 'Adobe Shockwave Flash','Adobe Shockwave Flash', 'swf', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 41, 7, 'Active Server Page','Active Server Page', 'asp', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 42, 9, 'Active Server Page Extended File','Active Server Page Extended File', 'aspx', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 43, 9, 'JavaScript File','JavaScript File', 'js', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 44, 9, 'JavaScript Page','JavaScript Page', 'jsp', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 45, 9, 'PHP Source Code File','PHP Source Code File', 'php', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 46, 9, 'HTML Server Side Include File','HTML Server Side Include File', 'shtm', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
	INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate) 
	Values ( 47, 9, 'Server Side Include HTML File','Server Side Include HTML File', 'shtml', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
    INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    Values ( 48, 9, 'Executable File','Executable File', 'exe', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
    INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    Values ( 49, 9, 'Dynamic-Link Library File','Dynamic-Link Library File', 'dll', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
    INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    Values ( 50, 9, 'Apple Mac OS X Application File','Apple Mac OS X Application File', 'app', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
    INSERT INTO resources.FileType (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    Values ( 51, 9, 'Apple Mac OS X Disk Image File','Apple Mac OS X Disk Image File', 'dmg', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
END
GO

IF NOT EXISTS(SELECT 'X' FROM resources.[VersionStatus] WHERE Id = 1)
BEGIN
	INSERT INTO resources.[VersionStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (1, 'Draft', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM resources.[VersionStatus] WHERE Id = 2)
BEGIN
	INSERT INTO resources.[VersionStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (2, 'Published', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM resources.[VersionStatus] WHERE Id = 3)
BEGIN
	INSERT INTO resources.[VersionStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (3, 'Unpublished', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM resources.[VersionStatus] WHERE Id = 4)
BEGIN
	INSERT INTO resources.[VersionStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (4, 'Publishing', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM resources.[VersionStatus] WHERE Id = 5)
BEGIN
	INSERT INTO resources.[VersionStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (5, 'Submitted to Publishing Queue', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM resources.[VersionStatus] WHERE Id = 6)
BEGIN
	INSERT INTO resources.[VersionStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (6, 'Failed to Publish', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

GO


UPDATE [resources].[FileType] SET [Icon]=N'a-mword-icon.svg' WHERE [Id]=1
UPDATE [resources].[FileType] SET [Icon]=N'a-mword-icon.svg' WHERE [Id]=2
UPDATE [resources].[FileType] SET [Icon]=N'a-pdf-icon.svg' WHERE [Id]=3
UPDATE [resources].[FileType] SET [Icon]=N'a-document-icon.svg' WHERE [Id]=4
UPDATE [resources].[FileType] SET [Icon]=N'a-mexcel-icon.svg' WHERE [Id]=5
UPDATE [resources].[FileType] SET [Icon]=N'a-mexcel-icon.svg' WHERE [Id]=6
UPDATE [resources].[FileType] SET [Icon]=N'a-mexcel-icon.svg' WHERE [Id]=7
UPDATE [resources].[FileType] SET [Icon]=N'a-mexcel-icon.svg' WHERE [Id]=8
UPDATE [resources].[FileType] SET [Icon]=N'a-spreadsheet-icon.svg' WHERE [Id]=9
UPDATE [resources].[FileType] SET [Icon]=N'a-mppoint-icon.svg' WHERE [Id]=10
UPDATE [resources].[FileType] SET [Icon]=N'a-mppoint-icon.svg' WHERE [Id]=11
UPDATE [resources].[FileType] SET [Icon]=N'a-presentation-icon.svg' WHERE [Id]=12
UPDATE [resources].[FileType] SET [Icon]=N'a-presentation-icon.svg' WHERE [Id]=13
UPDATE [resources].[FileType] SET [Icon]=N'a-audio-icon.svg' WHERE [Id]=14
UPDATE [resources].[FileType] SET [Icon]=N'a-audio-icon.svg' WHERE [Id]=15
UPDATE [resources].[FileType] SET [Icon]=N'a-audio-icon.svg' WHERE [Id]=16
UPDATE [resources].[FileType] SET [Icon]=N'a-video-icon.svg' WHERE [Id]=17
UPDATE [resources].[FileType] SET [Icon]=N'a-video-icon.svg' WHERE [Id]=18
UPDATE [resources].[FileType] SET [Icon]=N'a-video-icon.svg' WHERE [Id]=19
UPDATE [resources].[FileType] SET [Icon]=N'a-video-icon.svg' WHERE [Id]=20
UPDATE [resources].[FileType] SET [Icon]=N'a-video-icon.svg' WHERE [Id]=21
UPDATE [resources].[FileType] SET [Icon]=N'a-video-icon.svg' WHERE [Id]=22
UPDATE [resources].[FileType] SET [Icon]=N'a-video-icon.svg' WHERE [Id]=23
UPDATE [resources].[FileType] SET [Icon]=N'a-video-icon.svg' WHERE [Id]=24
UPDATE [resources].[FileType] SET [Icon]=N'a-image-icon.svg' WHERE [Id]=25
UPDATE [resources].[FileType] SET [Icon]=N'a-image-icon.svg' WHERE [Id]=26
UPDATE [resources].[FileType] SET [Icon]=N'a-image-icon.svg' WHERE [Id]=27
UPDATE [resources].[FileType] SET [Icon]=N'a-image-icon.svg' WHERE [Id]=28
UPDATE [resources].[FileType] SET [Icon]=N'a-image-icon.svg' WHERE [Id]=29
UPDATE [resources].[FileType] SET [Icon]=N'a-image-icon.svg' WHERE [Id]=30
UPDATE [resources].[FileType] SET [Icon]=N'a-image-icon.svg' WHERE [Id]=31
UPDATE [resources].[FileType] SET [Icon]=N'a-document-icon.svg' WHERE [Id]=32
UPDATE [resources].[FileType] SET [Icon]=N'a-spreadsheet-icon.svg' WHERE [Id]=33
UPDATE [resources].[FileType] SET [Icon]=N'a-default-icon.svg' WHERE [Id]=34
UPDATE [resources].[FileType] SET [Icon]=N'a-default-icon.svg' WHERE [Id]=35
UPDATE [resources].[FileType] SET [Icon]=N'a-default-icon.svg' WHERE [Id]=36
UPDATE [resources].[FileType] SET [Icon]=N'a-zip-icon.svg' WHERE [Id]=37

GO

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceVersionEventType]  WHERE Id = 1)
BEGIN
	INSERT INTO [resources].[ResourceVersionEventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES(1, 'Created Draft', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceVersionEventType]  WHERE Id = 2)
BEGIN
	INSERT INTO [resources].[ResourceVersionEventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES(2, 'Published', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceVersionEventType]  WHERE Id = 3)
BEGIN
	INSERT INTO [resources].[ResourceVersionEventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES(3, 'Unpublished', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceVersionEventType]  WHERE Id = 4)
BEGIN
	INSERT INTO [resources].[ResourceVersionEventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES(4, 'Publishing', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
GO

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceVersionEventType]  WHERE Id = 5)
BEGIN
	INSERT INTO [resources].[ResourceVersionEventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES(5, 'Admin', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
GO

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceVersionEventType]  WHERE Id = 6)
BEGIN
	INSERT INTO [resources].[ResourceVersionEventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES(6, 'Unpublished by Admin', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
GO
GO

-- [resources].[ResourceTypeValidationRule]
IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule]  WHERE Id = 1)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule] ([Id],[ResourceTypeId],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (1,6,'Scorm_ManifestPresent',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule]  WHERE Id = 2)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule] ([Id],[ResourceTypeId],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (2,6,'Scorm_QuickLinkIdValid',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule]  WHERE Id = 3)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule] ([Id],[ResourceTypeId],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (3,6,'Scorm_CatalogEntryValid',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END		

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule]  WHERE Id = 4)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule] ([Id],[ResourceTypeId],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (4,6,'Scorm_SchemaVersionValid',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END	

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule]  WHERE Id = 5)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule] ([Id],[ResourceTypeId],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (5,6,'Scorm_ManifestFluentValidatorValid',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END	