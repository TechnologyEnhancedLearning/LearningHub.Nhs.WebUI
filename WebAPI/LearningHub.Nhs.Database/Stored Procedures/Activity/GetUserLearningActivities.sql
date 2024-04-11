
-------------------------------------------------------------------------------
-- Author       Jignesh Jethwani
-- Created      27-10-2023
-- Purpose      Get learning activity details of user
--
-- Modification History

-- Sarathlal	14-12-2023
-- Sarathlal	08-03-2023
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[GetUserLearningActivities] (
	 @userId INT	
	,@searchText nvarchar(255) = NULL
	,@activityStatuses varchar(50) = NULL
	,@resourceTypes varchar(50) = NULL
	,@activityStartDate datetimeoffset(7) = NULL
	,@activityEndDate datetimeoffset(7) = NULL
	,@mediaActivityRecordingStartDate datetimeoffset(7) = NULL
	,@certificateEnabled bit = NULL
	,@offSet int
	,@fetchRows int
	)
AS
BEGIN

	DECLARE @tmpActivityStatus Table(ActivityStatusId int PRIMARY KEY)
	DECLARE @tmpResourceTypes Table(ResourceTypeId int PRIMARY KEY)
	DECLARE @filterActivityStatus bit = 0;
	DECLARE @filterResourceType bit = 0;
	IF NULLIF(LTRIM(RTRIM(@activityStatuses)), '') IS NOT NULL	
	BEGIN
	
		INSERT INTO @tmpActivityStatus

		SELECT			
			[Value]				
		FROM
			STRING_SPLIT(@ActivityStatuses, ',')
			
		SET @filterActivityStatus = 1;	
	END
	
	IF NULLIF(LTRIM(RTRIM(@resourceTypes)), '') IS NOT NULL	
	BEGIN
	
		INSERT INTO @tmpResourceTypes

		SELECT			
			[Value]			
		FROM
			STRING_SPLIT(@resourceTypes, ',')
			
		SET @filterResourceType = 1;	
	END

	SELECT 
	--Resource activity starts here
	[t2].[Id] as Id,[t2].[UserId] as UserId,[t7].[ResourceId] as ResourceId,[t2].[ResourceVersionId] as ResourceVersionId,[t7].[NodePathId],[t2].[AmendDate],[t2].[Deleted],[t2].[CreateUserId],[t2].[CreateDate],[t2].[AmendUserId]
	,[t2].[LaunchResourceActivityId],[t2].[MajorVersion],[t2].[MinorVersion],[t2].[ActivityStatusId],[t2].[ActivityStart],[t2].[ActivityEnd],[t9].[DurationSeconds],[t2].[Score]
	--Node path clumns starts here
	,[t6].[AmendDate] as NodePath_AmendDate,[t6].[AmendUserId] as NodePath_AmendUserId,[t6].[CatalogueNodeId] as NodePath_CatalogueNode, [t6].[CreateDate] as NodePath_CreateDate, [t6].[CreateUserId] as NodePath_CreateUserId, [t6].[Deleted] as NodePath_Deleted, [t6].[IsActive] as NodePath_IsActive
	,[t6].[NodeId] as NodePath_NodeId,[t6].[NodePath] as NodePath_NodePathString
	--Node path columns ends here
	--Resource columns startrs here
	,[t2].[AmendDate0] as Resource_AmendDate, [t2].[AmendUserId0] as Resource_AmendUserId,[t2].[CreateDate0] as Resource_CreateDate, [t2].[CreateUserId0] as Resource_CreateUserId,[t2].[CurrentResourceVersionId] as Resource_CurrentResourceVersionId
	,[t2].[Deleted] as Resource_Deleted,[t2].[DuplicatedFromResourceVersionId] as Resource_DuplicatedFromResourceVersionId,[t2].[Title] as Resource_ResourceTypeEnum,[t2].ResourceTypeId as Resource_ResourceTypeId,
	[t7].[OriginalResourceReferenceId] as Resource_OriginalResourceReferenceId
	--Resource columns ends here
	--Resource version columns startrs here
	,[t2].[AdditionalInformation] as ResourceVersion_AdditionalInformation, [t2].[AmendDate1] ResourceVersion_AmendDate, [t2].[AmendUserId1] as ResourceVersion_AmendUserId,
	[t2].[CertificateEnabled] as ResourceVersion_CertificateEnabled,  [t2].[Cost] as ResourceVersion_Cost,[t2].[CreateUserId] as ResourceVersion_CreateUser, [t2].[CreateDate] as ResourceVersion_CreateDate
	,[t2].[Deleted0] as ResourceVersion_Deleted,[t2].[Description] as ResourceVersion_Description, [t2].[HasCost] as ResourceVersion_HasCost, [t2].[MajorVersion0] as ResourceVersion_MajorVersion,
	[t2].[MinorVersion0] as ResourceVersion_MinorVersion, [t2].[PublicationId] as ResourceVersion_PublicationId ,[t2].[ResourceLicenceId] as ResourceVersion_ResourceLicenceId,[t2].[ReviewDate] ResourceVersion_ReviewDate
	,[t2].[SensitiveContent] as ResourceVersion_SensitiveContent, [t2].[Title] as ResourceVersion_Title,[t2].[ResourceId0] as ResourceVersion_ResourceId, [t2].[VersionStatusId] as ResourceVersion_VersionStatusId
	,[t2].[MaximumAttempts] AS ResourceVersion_MaximumAttempts, [t2].[PassMark] AS ResourceVersion_PassMark, [t2].[AssessmentType] as ResourceVersion_AssessmentResourceVersion_AssessmentType
	--Resource version  columns ends here

	--Block columns starts here
	  ,[t10].Id as Block_BlockId,[t10].[BlockCollectionId] as Block_BlockCollectionId, [t10].[BlockType] as Block_BlockType,[t10].[Title] as Block_Title, [t10].[Order] as Block_Order
	--Block columns ends here
	--Video resource version starts here
	,[VideoResourceVersion].[AmendDate] as VideoResourcVersion_AmendDate, [VideoResourceVersion].[AmendUserId] AS VideoResourcVersion_AmendUserId , [VideoResourceVersion].[ClosedCaptionsFileId] AS VideoResourcVersion_ClosedCaptionsFileId, [VideoResourceVersion].[CreateDate] AS VideoResourcVersion_CreateDate, [VideoResourceVersion].[CreateUserId] AS VideoResourcVersion_CreateUserId,
	[VideoResourceVersion].[Deleted] AS VideoResourcVersion_Deleted, [VideoResourceVersion].[DurationInMilliseconds] AS VideoResourcVersion_DurationInMilliseconds, [VideoResourceVersion].[ResourceAzureMediaAssetId] AS VideoResourcVersion_ResourceAzureMediaAssetId, [VideoResourceVersion].[ResourceVersionId]  AS VideoResourcVersion_ResourceVersionId,
	[VideoResourceVersion].[TranscriptFileId] AS VideoResourcVersion_TranscriptFileId, [VideoResourceVersion].[VideoFileId] AS VideoResourcVersion_VideoFileId
	--video resource version ends here

	--Audio resource version starts here
	,[AudeoResourceVersion].[Id] as AudioResourceVersion_Id, [AudeoResourceVersion].[AmendDate] as AudioResourceVersion_AmendDate,[AudeoResourceVersion].[AmendUserId] as AudioResourceVersion_AmendUserId, [AudeoResourceVersion].[AudioFileId] as AudioResourceVersion_AudioFileId,
	[AudeoResourceVersion].[CreateDate] as AudioResourceVersion_CreateDate, [AudeoResourceVersion].[CreateUserId] as AudioResourceVersion_CreateUserId, [AudeoResourceVersion].[Deleted] as AudioResourceVersion_Deleted, [AudeoResourceVersion].[DurationInMilliseconds] as AudioResourceVersion_DurationInMilliseconds, 
	[AudeoResourceVersion].[ResourceAzureMediaAssetId] as AudioResourceVersion_ResourceAzureMediaAssetId, [AudeoResourceVersion].[ResourceVersionId] as AudioResourceVersion_ResourceVersionId, [AudeoResourceVersion].[TranscriptFileId] as AudioResourceVersion_TranscriptFileId
	--Audio resource version ends here

	 --ResourceReference   starts here
	 ,[t7].[NodePathId] AS ResourceReference_NodePathId, [t7].[OriginalResourceReferenceId] AS ResourceReference_OriginalResourceReferenceId , [t7].[ResourceId] AS ResourceReference_ResourceId
	  --ResourceReference   ends here


	--Scorm activity starts here
	 ,[t9].[Id] as ScormActivity_Id, [t9].[AmendDate] as ScormActivity_AmendDate, [t9].[AmendUserID] as ScormActivity_AmendUserID, [t9].[CmiCoreExit] as ScormActivity_CmiCoreExit, [t9].[CmiCoreLesson_location] as ScormActivity_CmiCoreLessonLocation
	 , [t9].[CmiCoreLesson_status] as ScormActivity_CmiCoreLessonStatus, [t9].[CmiCoreScoreMax] as ScormActivity_CmiCoreScoreMax, [t9].[CmiCoreScoreMin] as ScormActivity_CmiCoreScoreMin, [t9].[CmiCoreScoreRaw] as ScormActivity_CmiCoreScoreRaw,
	 [t9].[CmiCoreSession_time] as ScormActivity_CmiCoreSessionTime, [t9].[CmiSuspend_data] as ScormActivity_CmiSuspendData, [t9].[CreateDate] as ScormActivity_CreateDate, [t9].[CreateUserID] as ScormActivity_CreateUserID,
	 [t9].[Deleted] as ScormActivity_Deleted, [t9].[DurationSeconds] as ScormActivity_DurationSeconds, [t9].[ResourceActivityId] as ScormActivity_ResourceActivityId
	--Scorm activity ends here

	-- ScormResourceVersion starts here
	 ,[t3].[AmendDate] as ScormResourceVersion_AmendDate, [t3].[AmendUserId] as ScormResourceVersion_AmendUserId, [t3].[CanDownload] as ScormResourceVersion_CanDownload, [t3].[ClearSuspendData] as ScormResourceVersion_ClearSuspendData, 
	 [t3].[ContentFilePath] as ScormResourceVersion_ContentFilePath, [t3].[CreateDate] as ScormResourceVersion_CreateDate, [t3].[CreateUserId] as ScormResourceVersion_CreateUserId, [t3].[Deleted] as ScormResourceVersion_Deleted,
	 [t3].[DevelopmentId] as ScormResourceVersion_DevelopmentId, [t3].[EsrLinkTypeId] as ScormResourceVersion_EsrLinkTypeId, [t3].[FileId] as  ScormResourceVersion_FileId, [t3].[PopupHeight] as  ScormResourceVersion_PopupHeight, 
	 [t3].[PopupWidth] as ScormResourceVersion_PopupWidth, [t3].[ResourceVersionId] as ScormResourceVersion_ResourceVersionId
	 -- ScormResourceVersion starts here

	 --Assessment Resource activity startes here
	
	,[t11].[Reason] as AssessmentResourceActivity_Reason, [t11].[ResourceActivityId] as AssessmentResourceActivity_ResourceActivityId
	,[t11].[Score] as AssessmentResourceActivity_Score
	,[t11].[Id0] as AssessmentResourceActivity_AssessmentResourceActivityInteraction_Id,[t11].[QuestionBlockId] as AssessmentResourceActivity_AssessmentResourceActivityInteraction_QuestionBlockId,
	 [t11].[AssessmentResourceActivityId] as AssessmentResourceActivity_AssessmentResourceActivityInteraction_AssessmentResourceActivityId
	,[t11].[AssessmentResourceActivityId] as AssessmentResourceActivity_AssessmentResourceActivityId
	,[t11].[Id] as  AssessmentResourceActivity_Id

	--Assessment resource activity ends here

	--InverseLaunchResourceActivity starts here
	--	no data available in resilt set
	--InverseLaunchResourceActivity ends here
	 --MediaResourceActivity   starts here
	 ,[t8].[Id] as MediaResourceActivity_Id, [t2].[ActivityStart] as MediaResourceActivity_ActivityStart, [t8].[AmendDate] as MediaResourceActivity_AmendDate, [t8].[AmendUserID]  as MediaResourceActivity_AmendUserID,
	 [t8].[CreateDate] as MediaResourceActivity_CreateDate, [t8].[CreateUserID] as MediaResourceActivity_CreateUserID, [t8].[Deleted] as MediaResourceActivity_Deleted, [t8].[PercentComplete] as MediaResourceActivity_PercentComplete,
	 [t8].[ResourceActivityId] as MediaResourceActivity_ResourceActivityId, [t8].[SecondsPlayed] as MediaResourceActivity_SecondsPlayed
	 --MediaResourceActivity   ends here

	 --ScormResourceVersionManifest starts here
	   ,[t4].[ItemIdentifier] as ScormResourceVersionManifest_ItemIdentifier, [t4].[Keywords] as ScormResourceVersionManifest_Keywords, [t4].[LaunchData] as ScormResourceVersionManifest_LaunchData,
	   [t4].[ManifestURL] as ScormResourceVersionManifest_ManifestUrl,	   [t4].[MasteryScore] as ScormResourceVersionManifest_MasteryScore,
	   [t4].[MaxTimeAllowed] as ScormResourceVersionManifest_MaxTimeAllowed, [t4].[QuicklinkId] as ScormResourceVersionManifest_QuicklinkId, 
	   [t4].[ResourceIdentifier] as ScormResourceVersionManifest_ResourceIdentifier, [t4].[ScormResourceVersionId] as ScormResourceVersionManifest_ScormResourceVersionId, 
	   [t4].[TemplateVersion] as ScormResourceVersionManifest_TemplateVersion , [t4].[TimeLimitAction] as ScormResourceVersionManifest_TimeLimitAction, [t4].[Title] as ScormResourceVersionManifest_Title

	 --ScormResourceVersionManifest ends here
FROM (
    SELECT [ResourceActivity].[Id], [ResourceActivity].[ActivityEnd], [ResourceActivity].[ActivityStart], [ResourceActivity].[ActivityStatusId], [ResourceActivity].[AmendDate], [ResourceActivity].[AmendUserId], [ResourceActivity].[CreateDate], [ResourceActivity].[CreateUserId], [ResourceActivity].[Deleted], [ResourceActivity].[DurationSeconds], [ResourceActivity].[LaunchResourceActivityId], [ResourceActivity].[MajorVersion], [ResourceActivity].[MinorVersion], [ResourceActivity].[NodePathId], [ResourceActivity].[ResourceId], [ResourceActivity].[ResourceVersionId], [ResourceActivity].[Score], [ResourceActivity].[UserId], [Res].[Id] AS [Id0], [Res].[AmendDate] AS [AmendDate0], [Res].[AmendUserId] AS [AmendUserId0], [Res].[CreateDate] AS [CreateDate0], [Res].[CreateUserId] AS [CreateUserId0], [Res].[CurrentResourceVersionId], [Res].[Deleted] AS [Deleted0], [Res].[DuplicatedFromResourceVersionId], [Res].[ResourceTypeId], [ResVer].[Id] AS [Id1], [ResVer].[AdditionalInformation], [ResVer].[AmendDate] AS [AmendDate1], [ResVer].[AmendUserId] AS [AmendUserId1], [ResVer].[CertificateEnabled], [ResVer].[Cost], [ResVer].[CreateDate] AS [CreateDate1], [ResVer].[CreateUserId] AS [CreateUserId1], [ResVer].[Deleted] AS [Deleted1], [ResVer].[Description], [ResVer].[HasCost], [ResVer].[MajorVersion] AS [MajorVersion0], [ResVer].[MinorVersion] AS [MinorVersion0], [ResVer].[PublicationId], [ResVer].[ResourceId] AS [ResourceId0], [ResVer].[ResourceLicenceId], [ResVer].[ReviewDate], [ResVer].[SensitiveContent], [ResVer].[Title], [ResVer].[VersionStatusId], [AssessResVer].[Id] AS [Id2], [AssessResVer].[AmendDate] AS [AmendDate2], [AssessResVer].[AmendUserId] AS [AmendUserId2], [AssessResVer].[AnswerInOrder], [AssessResVer].[AssessmentContentId], [AssessResVer].[AssessmentType], [AssessResVer].[CreateDate] AS [CreateDate2], [AssessResVer].[CreateUserId] AS [CreateUserId2], [AssessResVer].[Deleted] AS [Deleted2], [AssessResVer].[EndGuidanceId], [AssessResVer].[MaximumAttempts], [AssessResVer].[PassMark], [AssessResVer].[ResourceVersionId] AS [ResourceVersionId0]
    FROM [activity].[ResourceActivity] AS [ResourceActivity]
    INNER JOIN (
        SELECT [Resource].[Id], [Resource].[AmendDate], [Resource].[AmendUserId], [Resource].[CreateDate], [Resource].[CreateUserId], [Resource].[CurrentResourceVersionId], [Resource].[Deleted], [Resource].[DuplicatedFromResourceVersionId], [Resource].[ResourceTypeId]
        FROM [resources].[Resource] AS [Resource]
		LEFT JOIN @tmpResourceTypes AS [ResourceTypes]
		ON
			[Resource].[ResourceTypeId] = [ResourceTypes].ResourceTypeId
        WHERE 
			[Resource].[Deleted] = 0 AND (@filterResourceType = 0 OR [ResourceTypes].ResourceTypeId IS NOT NULL)
    ) AS [Res] ON [ResourceActivity].[ResourceId] = [Res].[Id]
    INNER JOIN (
        SELECT [ResourceVersion].[Id], [ResourceVersion].[AdditionalInformation], [ResourceVersion].[AmendDate], [ResourceVersion].[AmendUserId], [ResourceVersion].[CertificateEnabled], [ResourceVersion].[Cost], [ResourceVersion].[CreateDate], [ResourceVersion].[CreateUserId], [ResourceVersion].[Deleted], [ResourceVersion].[Description], [ResourceVersion].[HasCost], [ResourceVersion].[MajorVersion], [ResourceVersion].[MinorVersion], [ResourceVersion].[PublicationId], [ResourceVersion].[ResourceId], [ResourceVersion].[ResourceLicenceId], [ResourceVersion].[ReviewDate], [ResourceVersion].[SensitiveContent], [ResourceVersion].[Title], [ResourceVersion].[VersionStatusId]
        FROM [resources].[ResourceVersion] AS [ResourceVersion]
        WHERE [ResourceVersion].[Deleted] = 0
    ) AS [ResVer] ON [ResourceActivity].[ResourceVersionId] = [ResVer].[Id]	
    LEFT JOIN (
        SELECT [AssessmentResourceVersion].[Id], [AssessmentResourceVersion].[AmendDate], [AssessmentResourceVersion].[AmendUserId], [AssessmentResourceVersion].[AnswerInOrder], [AssessmentResourceVersion].[AssessmentContentId], [AssessmentResourceVersion].[AssessmentType], [AssessmentResourceVersion].[CreateDate], [AssessmentResourceVersion].[CreateUserId], [AssessmentResourceVersion].[Deleted], [AssessmentResourceVersion].[EndGuidanceId], [AssessmentResourceVersion].[MaximumAttempts], [AssessmentResourceVersion].[PassMark], [AssessmentResourceVersion].[ResourceVersionId]
        FROM [resources].[AssessmentResourceVersion] AS [AssessmentResourceVersion]
        WHERE [AssessmentResourceVersion].[Deleted] = 0
    ) AS [AssessResVer] ON [ResVer].[Id] = [AssessResVer].[ResourceVersionId]
    WHERE 	
			[ResourceActivity].[Deleted] = 0 
		AND 
			[ResourceActivity].[UserId] = @userId
		AND 
			(@activityStartDate IS NULL OR [ResourceActivity].[ActivityStart] >= @activityStartDate) 
		AND 
			(@activityEndDate IS NULL OR [ResourceActivity].[ActivityStart] < @activityEndDate)
		 AND  
		 (
			@searchText IS NULL
            OR 
				(
						Charindex(@searchText, [ResVer].[Title]) > 0
					OR                          
						Charindex(@searchText, [ResVer].[Description]) > 0
					OR  
					EXISTS
                    (
                        SELECT 1
                        FROM   [resources].[ResourceVersionKeyword] AS [ResourceVersionKeyword]
                        WHERE  (
									[ResourceVersionKeyword].[Deleted] = 0
                                AND  
									[ResVer].[Id] = [ResourceVersionKeyword].[ResourceVersionId]
								AND   
									Charindex(@searchText, [ResourceVersionKeyword].[Keyword]) > 0
								)
					)
				)
		)
		AND
			(
				(
					-- first scorm activity cmi core status is not complete
					SELECT 
						TOP(1) [ScormActivity].[CmiCoreLesson_status]
					FROM 
						[activity].[ScormActivity] AS [ScormActivity]
					WHERE
						([ScormActivity].[Deleted] = 0 AND [ResourceActivity].[Id] = [ScormActivity].[ResourceActivityId])
				) 
					<> 3
				OR
				-- first scorm activity cmi core status is null
				(
					SELECT TOP(1)
						[ScormActivity].[CmiCoreLesson_status]
                     FROM   [activity].[ScormActivity] AS [ScormActivity]
                     WHERE  ([ScormActivity].[Deleted] = 0 AND [ResourceActivity].[Id] = [ScormActivity].[ResourceActivityId])										 
				) 
					IS NULL
			)		
		
		AND
				(
						@filterActivityStatus = 0			
						OR
						(
								EXISTS (SELECT 1 FROM @tmpActivityStatus WHERE ActivityStatusId = 3)
								AND	
									(
										(
											[Res].[ResourceTypeId] IN (2,7) 
											AND 
												(
													[ResourceActivity].[ActivityStart] < @mediaActivityRecordingStartDate

													OR         
														(
																		EXISTS
																		(
																			SELECT 1
																			FROM   [activity].[MediaResourceActivity] AS [MediaResourceActivity]
																			WHERE  ([MediaResourceActivity].[Deleted] = 0  AND  [ResourceActivity].[Id] = [MediaResourceActivity].[ResourceActivityId])
																		)
															AND        (
																			SELECT TOP(1)  [m0].[PercentComplete]
																			FROM   [activity].[MediaResourceActivity] AS [m0]
																			WHERE  [m0].[Deleted] = 0  AND [ResourceActivity].[Id] = [m0].[ResourceActivityId]
																		) = 100.0
							   
														)	
												)
										  )
										  OR
												([Res].[ResourceTypeId]  IN (6,11) AND  [ResourceActivity].[ActivityStatusId] = 3)
										--OR         
										--(
										--		([Res].[ResourceTypeId] IN (1,5,10,12) AND  [ResourceActivity].[ActivityStatusId] = 3)
										--	  AND
										--		([Res].[ResourceTypeId] NOT IN (2,7,8) AND  [ResourceActivity].[ActivityStatusId] = 3)
										--)
									)
						)
						OR	
						(
								EXISTS (SELECT 1 FROM @tmpActivityStatus WHERE ActivityStatusId = 2)
							AND

								(
										(
											  [Res].[ResourceTypeId] IN (2,7)
											AND   
												([ResourceActivity].[ActivityStart] >= @mediaActivityRecordingStartDate)
											AND 
												(
													NOT EXISTS
														 (
																SELECT 1
																FROM   [activity].[MediaResourceActivity] AS [MediaResourceActivity1]
																WHERE  [MediaResourceActivity1].[Deleted] = 0
																AND   [ResourceActivity].[Id] = [MediaResourceActivity1].[ResourceActivityId]
														 )
													OR 
														(
																(SELECT TOP(1)
																	   [MediaResourceActivity2].[PercentComplete]
																FROM   [activity].[MediaResourceActivity] AS [MediaResourceActivity2]
																WHERE  [MediaResourceActivity2].[Deleted] = 0
																AND    [ResourceActivity].[Id] = [MediaResourceActivity2].[ResourceActivityId]) < 100.0

																OR 
																(SELECT TOP(1)
																	   [MediaResourceActivity2].[PercentComplete]
																FROM   [activity].[MediaResourceActivity] AS [MediaResourceActivity2]
																WHERE  [MediaResourceActivity2].[Deleted] = 0
																AND    [ResourceActivity].[Id] = [MediaResourceActivity2].[ResourceActivityId]) IS NULL
														)
										)
					
									OR  
										(
						 
												[Res].[ResourceTypeId] = 6
											AND 
												EXISTS
												(
													SELECT 1 FROM   [activity].[ScormActivity] AS [ScormActivity0]
													WHERE  [ScormActivity0].[Deleted] = 0	 AND [ResourceActivity].[Id] = [ScormActivity0].[ResourceActivityId]
												)
											AND 
												(
													(SELECT TOP(1) [ScormActivity1].[CmiCoreLesson_status]
													FROM   [activity].[ScormActivity] AS [ScormActivity1]
													WHERE [ScormActivity1].[Deleted] = 0 AND [ResourceActivity].[Id] = [ScormActivity1].[ResourceActivityId]) = 2
												)
										)
			
									OR

										(
											[Res].[ResourceTypeId] = 11
											AND   
												EXISTS
												(
														  SELECT 1
														  FROM   [activity].[AssessmentResourceActivity] AS [AssessmentResourceActivity0]
														  WHERE  [AssessmentResourceActivity0].[Deleted] = 0
														  AND  [ResourceActivity].[Id] = [AssessmentResourceActivity0].[ResourceActivityId]
												)
							 
											AND     
												(
													(
														SELECT TOP(1)
														CASE
															WHEN [AssessmentResourceActivity1].[Score] IS NOT NULL THEN 1
														ELSE 0
														END
														FROM   [activity].[AssessmentResourceActivity] AS [AssessmentResourceActivity1]
														WHERE [AssessmentResourceActivity1].[Deleted] = 0 AND    [ResourceActivity].[Id] = [AssessmentResourceActivity1].[ResourceActivityId]
													) = 0
												)
										)
								)
							)				
							OR	
							(
								-- select passed activities if requested
								EXISTS (SELECT 1 FROM @tmpActivityStatus WHERE ActivityStatusId = 5) AND [ResourceActivity].[ActivityStatusId] = 5 
							)
							OR	
							(
								EXISTS (SELECT 1 FROM @tmpActivityStatus WHERE ActivityStatusId = 5) 
								AND
								(
									[Res].[ResourceTypeId] = 11
									AND
										EXISTS
										(
											 SELECT 1
											 FROM   [activity].[AssessmentResourceActivity] AS [AssessmentResourceActivity2]
											 WHERE  
												[AssessmentResourceActivity2].[Deleted] = 0
											 AND    
												[ResourceActivity].[Id] = [AssessmentResourceActivity2].[ResourceActivityId]
										)
									AND 
										(
											 (SELECT TOP(1)
													ISNULL([AssessmentResourceActivity3].[Score],0)
											 FROM   [activity].[AssessmentResourceActivity] AS [AssessmentResourceActivity3]
											 WHERE  
													[AssessmentResourceActivity3].[Deleted] = 0
											 AND    [ResourceActivity].[Id] = [AssessmentResourceActivity3].[ResourceActivityId]) >= Cast([AssessResVer].[PassMark] AS DECIMAL(18,2))
										)						
								)
							)
							OR	
							(  -- select failed activities if requested
								EXISTS (SELECT 1 FROM @tmpActivityStatus WHERE ActivityStatusId = 4) AND [ResourceActivity].[ActivityStatusId] = 4 
							)	
							OR	
							(
								-- select failed activities if requested
								EXISTS (SELECT 1 FROM @tmpActivityStatus WHERE ActivityStatusId = 4) 
				
								AND
								(
									[Res].[ResourceTypeId] = 11
									AND 
										EXISTS
											(
											  SELECT 1  FROM   [activity].[AssessmentResourceActivity] AS [AssessmentResourceActivity4]
												WHERE  ([AssessmentResourceActivity4].[Deleted] = 0 AND [ResourceActivity].[Id] = [AssessmentResourceActivity4].[ResourceActivityId])
											)
									
									AND        
										(
										   (SELECT TOP(1) ISNULL([AssessmentResourceActivity5].[Score],0)
											   FROM   [activity].[AssessmentResourceActivity] AS [AssessmentResourceActivity5]
											   WHERE  [AssessmentResourceActivity5].[Deleted] = 0
											   AND  [ResourceActivity].[Id] = [AssessmentResourceActivity5].[ResourceActivityId]) < Cast([AssessResVer].[PassMark] AS DECIMAL(18,2))
										)

								)
							)	
							OR	
							(
								-- select download activities if requested
								EXISTS (SELECT 1 FROM @tmpActivityStatus WHERE ActivityStatusId = 6) AND [ResourceActivity].[ActivityStatusId]  = 3 
								AND ([Res].[ResourceTypeId] = 9)
							)
							OR	
							(
								-- select Launched activities if requested
								EXISTS (SELECT 1 FROM @tmpActivityStatus WHERE ActivityStatusId = 1) AND [ResourceActivity].[ActivityStatusId]  = 3
								AND [Res].[ResourceTypeId] = 8
								
							)
							OR	
							(
								-- select Viewed activities if requested
								EXISTS (SELECT 1 FROM @tmpActivityStatus WHERE ActivityStatusId = 8) AND [ResourceActivity].[ActivityStatusId]  = 3
								AND ([Res].[ResourceTypeId] = 1 OR [Res].[ResourceTypeId] = 5 OR [Res].[ResourceTypeId] = 10 OR [Res].[ResourceTypeId] = 12)
								
							)
						)
			  )
		AND (@certificateEnabled IS NULL OR [ResVer].[CertificateEnabled]=@certificateEnabled)
    ORDER BY 
		[ResourceActivity].[ActivityStart] DESC
    OFFSET @offSet 
		ROWS FETCH NEXT @fetchRows ROWS ONLY
) AS [t2]
LEFT JOIN [resources].[VideoResourceVersion] AS [VideoResourceVersion] ON [t2].[Id1] = [VideoResourceVersion].[ResourceVersionId]
LEFT JOIN [resources].[AudioResourceVersion] AS [AudeoResourceVersion] ON [t2].[Id1] = [AudeoResourceVersion].[ResourceVersionId]
LEFT JOIN (
    SELECT [ScormResourceVersion2].[Id], [ScormResourceVersion2].[AmendDate], [ScormResourceVersion2].[AmendUserId], [ScormResourceVersion2].[CanDownload], [ScormResourceVersion2].[ClearSuspendData], [ScormResourceVersion2].[ContentFilePath], [ScormResourceVersion2].[CreateDate], [ScormResourceVersion2].[CreateUserId], [ScormResourceVersion2].[Deleted], [ScormResourceVersion2].[DevelopmentId], [ScormResourceVersion2].[EsrLinkTypeId], [ScormResourceVersion2].[FileId], [ScormResourceVersion2].[PopupHeight], [ScormResourceVersion2].[PopupWidth], [ScormResourceVersion2].[ResourceVersionId]
    FROM [resources].[ScormResourceVersion] AS [ScormResourceVersion2]
    WHERE [ScormResourceVersion2].[Deleted] = 0
) AS [t3] ON [t2].[Id1] = [t3].[ResourceVersionId]
LEFT JOIN (
    SELECT [ScormResourceVersionManifest3].[Id], [ScormResourceVersionManifest3].[AmendDate], [ScormResourceVersionManifest3].[AmendUserId], [ScormResourceVersionManifest3].[Author], [ScormResourceVersionManifest3].[CatalogEntry], [ScormResourceVersionManifest3].[Copyright], [ScormResourceVersionManifest3].[CreateDate], [ScormResourceVersionManifest3].[CreateUserId], [ScormResourceVersionManifest3].[Deleted], [ScormResourceVersionManifest3].[Description], [ScormResourceVersionManifest3].[Duration], [ScormResourceVersionManifest3].[ItemIdentifier], [ScormResourceVersionManifest3].[Keywords], [ScormResourceVersionManifest3].[LaunchData], [ScormResourceVersionManifest3].[ManifestURL], [ScormResourceVersionManifest3].[MasteryScore], [ScormResourceVersionManifest3].[MaxTimeAllowed], [ScormResourceVersionManifest3].[QuicklinkId], [ScormResourceVersionManifest3].[ResourceIdentifier], [ScormResourceVersionManifest3].[ScormResourceVersionId], [ScormResourceVersionManifest3].[TemplateVersion], [ScormResourceVersionManifest3].[TimeLimitAction], [ScormResourceVersionManifest3].[Title]
    FROM [resources].[ScormResourceVersionManifest] AS [ScormResourceVersionManifest3]
    WHERE [ScormResourceVersionManifest3].[Deleted] = 0
) AS [t4] ON [t3].[Id] = [t4].[ScormResourceVersionId]
LEFT JOIN (
    SELECT [BlockCollection].[Id], [BlockCollection].[AmendDate], [BlockCollection].[AmendUserId], [BlockCollection].[CreateDate], [BlockCollection].[CreateUserId], [BlockCollection].[Deleted]
    FROM [resources].[BlockCollection] AS [BlockCollection]
    WHERE [BlockCollection].[Deleted] = 0
) AS [t5] ON [t2].[AssessmentContentId] = [t5].[Id]
INNER JOIN (
    SELECT [NodePath].[Id], [NodePath].[AmendDate], [NodePath].[AmendUserId], [NodePath].[CatalogueNodeId], [NodePath].[CreateDate], [NodePath].[CreateUserId], [NodePath].[Deleted], [NodePath].[IsActive], [NodePath].[NodeId], [NodePath].[NodePath]
    FROM [hierarchy].[NodePath] AS [NodePath]
    WHERE [NodePath].[Deleted] = 0
) AS [t6] ON [t2].[NodePathId] = [t6].[Id]
LEFT JOIN (
    SELECT [ResourceReference5].[Id], [ResourceReference5].[AmendDate], [ResourceReference5].[AmendUserId], [ResourceReference5].[CreateDate], [ResourceReference5].[CreateUserId], [ResourceReference5].[Deleted], [ResourceReference5].[NodePathId], [ResourceReference5].[OriginalResourceReferenceId], [ResourceReference5].[ResourceId]
    FROM [resources].[ResourceReference] AS [ResourceReference5]
    WHERE [ResourceReference5].[Deleted] = 0
) AS [t7] ON [t2].[Id0] = [t7].[ResourceId]
LEFT JOIN (
    SELECT [MediaResourceActivity3].[Id], [MediaResourceActivity3].[ActivityStart], [MediaResourceActivity3].[AmendDate], [MediaResourceActivity3].[AmendUserID], [MediaResourceActivity3].[CreateDate], [MediaResourceActivity3].[CreateUserID], [MediaResourceActivity3].[Deleted], [MediaResourceActivity3].[PercentComplete], [MediaResourceActivity3].[ResourceActivityId], [MediaResourceActivity3].[SecondsPlayed]
    FROM [activity].[MediaResourceActivity] AS [MediaResourceActivity3]
    WHERE [MediaResourceActivity3].[Deleted] = 0
) AS [t8] ON [t2].[Id] = [t8].[ResourceActivityId]
LEFT JOIN (
    SELECT [ScormActivity4].[Id], [ScormActivity4].[AmendDate], [ScormActivity4].[AmendUserID], [ScormActivity4].[CmiCoreExit], [ScormActivity4].[CmiCoreLesson_location], [ScormActivity4].[CmiCoreLesson_status], [ScormActivity4].[CmiCoreScoreMax], [ScormActivity4].[CmiCoreScoreMin], [ScormActivity4].[CmiCoreScoreRaw], [ScormActivity4].[CmiCoreSession_time], [ScormActivity4].[CmiSuspend_data], [ScormActivity4].[CreateDate], [ScormActivity4].[CreateUserID], [ScormActivity4].[Deleted], [ScormActivity4].[DurationSeconds], [ScormActivity4].[ResourceActivityId]
    FROM [activity].[ScormActivity] AS [ScormActivity4]
    WHERE [ScormActivity4].[Deleted] = 0
) AS [t9] ON [t2].[Id] = [t9].[ResourceActivityId]
LEFT JOIN (
    SELECT [Block0].[Id], [Block0].[AmendDate], [Block0].[AmendUserId], [Block0].[BlockCollectionId], [Block0].[BlockType], [Block0].[CreateDate], [Block0].[CreateUserId], [Block0].[Deleted], [Block0].[Order], [Block0].[Title]
    FROM [resources].[Block] AS [Block0]
    WHERE [Block0].[Deleted] = 0
) AS [t10] ON [t5].[Id] = [t10].[BlockCollectionId]
LEFT JOIN (
    SELECT [AssessmentResourceActivity5].[Id], [AssessmentResourceActivity5].[AmendDate], [AssessmentResourceActivity5].[AmendUserID], [AssessmentResourceActivity5].[CreateDate], [AssessmentResourceActivity5].[CreateUserID], [AssessmentResourceActivity5].[Deleted], [AssessmentResourceActivity5].[Reason], [AssessmentResourceActivity5].[ResourceActivityId], [AssessmentResourceActivity5].[Score], [t12].[Id] AS [Id0], [t12].[AmendDate] AS [AmendDate0], [t12].[AmendUserID] AS [AmendUserID0], [t12].[AssessmentResourceActivityId], [t12].[CreateDate] AS [CreateDate0], [t12].[CreateUserID] AS [CreateUserID0], [t12].[Deleted] AS [Deleted0], [t12].[QuestionBlockId]
    FROM [activity].[AssessmentResourceActivity] AS [AssessmentResourceActivity5]
    LEFT JOIN (
        SELECT [AssessmentResourceActivityInteraction6].[Id], [AssessmentResourceActivityInteraction6].[AmendDate], [AssessmentResourceActivityInteraction6].[AmendUserID], [AssessmentResourceActivityInteraction6].[AssessmentResourceActivityId], [AssessmentResourceActivityInteraction6].[CreateDate], [AssessmentResourceActivityInteraction6].[CreateUserID], [AssessmentResourceActivityInteraction6].[Deleted], [AssessmentResourceActivityInteraction6].[QuestionBlockId]
        FROM [activity].[AssessmentResourceActivityInteraction] AS [AssessmentResourceActivityInteraction6]
        WHERE [AssessmentResourceActivityInteraction6].[Deleted] = 0
    ) AS [t12] ON [AssessmentResourceActivity5].[Id] = [t12].[AssessmentResourceActivityId]
    WHERE [AssessmentResourceActivity5].[Deleted] = 0
) AS [t11] ON [t2].[Id] = [t11].[ResourceActivityId]
ORDER BY [t2].[ActivityStart] DESC, [t2].[Id], [t2].[Id0], [t2].[Id1], [t2].[Id2], [VideoResourceVersion].[Id], [AudeoResourceVersion].[Id], [t3].[Id], [t4].[Id], [t5].[Id], [t6].[Id], [t7].[Id], [t8].[Id], [t9].[Id], [t10].[Id], [t11].[Id]
		
END


