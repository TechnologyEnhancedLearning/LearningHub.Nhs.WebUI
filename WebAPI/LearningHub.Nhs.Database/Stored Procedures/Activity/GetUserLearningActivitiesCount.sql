
-------------------------------------------------------------------------------
-- Author       Jignesh Jethwani
-- Created      27-10-2023
-- Purpose      Get total count learning activity details of user
--
-- Modification History

-- Sarathlal	18-12-2023
-- Sarathlal	08-03-2024
-- Sarathlal	23-04-2024	TD-2954: Audio/Video/Assessment issue resolved and duplicate issue also resolved
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[GetUserLearningActivitiesCount] (
	 @userId INT	
	,@searchText nvarchar(255) = NULL
	,@activityStatuses varchar(50) = NULL
	,@resourceTypes varchar(50) = NULL
	,@activityStartDate datetimeoffset(7) = NULL
	,@activityEndDate datetimeoffset(7) = NULL
	,@mediaActivityRecordingStartDate datetimeoffset(7) = NULL
	,@certificateEnabled bit = NULL,
	@totalcount int OUTPUT
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

	SELECT @totalcount = (SELECT COUNT(*) FROM ( SELECT [t2].Id
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
					(
					   -- resource type is not video/audio and launch resource activity doesn't exists
					   NOT (EXISTS
								(
									SELECT 1 FROM   [activity].[ResourceActivity] AS [ResAct1]
									WHERE  [ResAct1].[Deleted] = 0 AND [ResourceActivity].[Id] = [ResAct1].[LaunchResourceActivityId] 
								))
					)
				OR  
					-- or launch resource activity completed
					EXISTS
					(
							SELECT 1	FROM   [activity].[ResourceActivity] AS [ResAct2]
							WHERE  [ResAct2].[Deleted] = 0 AND  [ResourceActivity].[Id] = [ResAct2].[LaunchResourceActivityId] AND  [ResAct2].[ActivityStatusId] in (3,7)
					)
					
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
												 OR	([Res].[ResourceTypeId]  IN (11) AND  [ResourceActivity].[ActivityStatusId] = 3 AND [AssessResVer].[AssessmentType]=1)

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
									[Res].[ResourceTypeId] = 11 AND  [AssessResVer].[AssessmentType]=2
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
													[AssessmentResourceActivity3].[Score]
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
									[Res].[ResourceTypeId] = 11 and [AssessResVer].[AssessmentType]=2
									AND 
										EXISTS
											(
											  SELECT 1  FROM   [activity].[AssessmentResourceActivity] AS [AssessmentResourceActivity4]
												WHERE  ([AssessmentResourceActivity4].[Deleted] = 0 AND [ResourceActivity].[Id] = [AssessmentResourceActivity4].[ResourceActivityId])
											)
									
									AND        
										(
										   (SELECT TOP(1) [AssessmentResourceActivity5].[Score]
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
GROUP BY [t2].[Id]	
) tbl)
END
