-- =============================================
-- Author:	Swapnamol Abraham
-- Create date: 17-12-2024
-- Description:	To get the details of completion percentage for an assessment with questionBlock
-- =============================================
CREATE PROCEDURE [activity].[GetAssessmentActivityCompletionPercentage]
	@userId INT,
	@ResourceVersionId INT,
	@activityId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT @activityId as ResourceActivityId,
    TotalQuestions,
    CompletedQuestions,
	CASE WHEN TotalQuestions = 0 THEN 0 ELSE (CompletedQuestions/TotalQuestions)*100 END AS CompletionPercentage
FROM 
    (select count(ARV.Id) as TotalQuestions from resources.AssessmentResourceVersion ARV
              INNER JOIN [resources].[BlockCollection] AS BC ON BC.Id = ARV.AssessmentContentId
			  LEFT JOIN [resources].[Block] AS B ON B.BlockCollectionId = BC.Id
			  WHERE B.BlockType = 4 AND arv.ResourceVersionId  = @ResourceVersionId AND BC.Deleted = 0 ) AS Total,

    (select count(ARA.Id) As completedQuestions from [activity].[AssessmentResourceActivity] ARA
LEFT JOIN [activity].[AssessmentResourceActivityInteraction] ARAI ON ARA.Id = ARAI.AssessmentResourceActivityId
Where ResourceActivityId = @activityId AND ARAI.Deleted = 0 ) AS Completed;

END