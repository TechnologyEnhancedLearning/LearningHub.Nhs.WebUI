-------------------------------------------------------------------------------
-- Author       Julian Ng (Softwire)
-- Created      12-08-2021
-- Purpose      Duplicates a resource
--
-- Modification History
--
-- 12-08-2021  Julian Ng	    Initial Revision
-- 13-08-2021  Julian Ng	    Extract Case Resource duplication to stored proc
-- 20-09-2021  Malina Slevoaca  Allow duplicating Assessment resource type
-------------------------------------------------------------------------------
CREATE TYPE [resources].[CurrentBlockIdsType] 
AS TABLE (
     BlockId             INT PRIMARY KEY,
     BlockCollectionId   INT NULL
)
GO

CREATE TYPE [resources].[CopiedBlocksType]
AS TABLE (
    OriginalBlockId             INT PRIMARY KEY,
    BlockId                     INT NULL,
    OriginalBlockCollectionId   INT NOT NULL,
    BlockCollectionId           INT NOT NULL,
    [Order]                     INT NOT NULL,
    Title                       NVARCHAR(200) NULL,
    BlockType                   INT NOT NULL,
    Deleted                     BIT NOT NULL,
    CreateUserId                INT NOT NULL,
    CreateDate                  DATETIMEOFFSET(7) NOT NULL,
    AmendUserId                 INT NOT NULL,
    AmendDate                   DATETIMEOFFSET(7) NOT NULL
)
GO

CREATE PROCEDURE [resources].[ResourceVersionCreateDuplicate](
    @ResourceId int,
    @UserId int,
	@UserTimezoneOffset int = NULL,
    @ResourceVersionId int OUTPUT
) AS

BEGIN
    
    BEGIN TRY
		-- Check if the catalogue is currently locked for editing by admin.
		IF EXISTS (SELECT 1 FROM hierarchy.NodeResource nr
					INNER JOIN hierarchy.NodePath np ON np.NodeId = nr.NodeId
					INNER JOIN hierarchy.NodePath rnp ON np.CatalogueNodeId = rnp.NodeId
					INNER JOIN hierarchy.HierarchyEdit he ON he.RootNodePathId = rnp.Id
					WHERE nr.ResourceId = @ResourceId AND nr.Deleted = 0 AND np.Deleted = 0 AND np.IsActive = 1 AND he.Deleted = 0 AND
					HierarchyEditStatusId IN (1 /* Draft */, 4 /* Publishing */, 5 /* Submitted */))
		BEGIN
			RAISERROR ('Error - Cannot duplicate the ResourceVersion because the catalogue is currently locked for editing',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
        
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @NewResourceId int
        DECLARE @CurrentResourceTypeId int
        DECLARE @CurrentResourceVersionId int
        DECLARE @CurrentVersionStatusId int
    
        -- Get the Version Status of the most recently amended ResourceVersion      
        SELECT @CurrentVersionStatusId = VersionStatusId
        FROM resources.ResourceVersion as rv
            INNER JOIN (
                SELECT ResourceId, MAX(AmendDate) AS MaxAmendDate
                FROM resources.ResourceVersion
                GROUP BY ResourceId
            ) AS rv_joined 
                ON rv.ResourceId = rv_joined.ResourceId
                AND rv.AmendDate = rv_joined.MaxAmendDate 
        WHERE rv.ResourceId = @ResourceId
        AND Deleted = 0
    
        IF @CurrentVersionStatusId != 1 /* Draft */
        AND @CurrentVersionStatusId != 2 /* Published */
        AND @CurrentVersionStatusId != 3 /* Unpublished */
            BEGIN
                SET @ErrorMessage = 'Error - Cannot duplicate a resource with a current status of ' + CONVERT(NVARCHAR(50), @CurrentVersionStatusId)
                RAISERROR (@ErrorMessage, -- Message text.  
                    16, -- Severity.  
                    1 -- State.  
                );
            END
        
        -- Draft Resource Versions do not have a CurrentResourceVersionId value in the Resource table 
        IF @CurrentVersionStatusId = 1
        BEGIN
            SELECT @CurrentResourceVersionId = rv.Id,
                   @CurrentResourceTypeId = r.ResourceTypeId
            FROM resources.Resource AS r
                INNER JOIN resources.ResourceVersion AS rv ON r.Id = rv.ResourceId
            WHERE r.Id = @ResourceId
        END
        ELSE
        BEGIN
            SELECT @CurrentResourceVersionId = CurrentResourceVersionId,
                   @CurrentResourceTypeId = ResourceTypeId
            FROM resources.Resource
            WHERE Id = @ResourceId
        END
    
        BEGIN TRAN
            -- We need to create a new Resource entry for the duplicate
            INSERT INTO resources.Resource (ResourceTypeId, DuplicatedFromResourceVersionId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT ResourceTypeId, @CurrentResourceVersionId, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM resources.Resource
            WHERE Id = @ResourceId
            SELECT @NewResourceId = SCOPE_IDENTITY()
    
            -- Populate the ResourceVersions common details
            INSERT INTO resources.ResourceVersion (ResourceId, VersionStatusId,ResourceAccessibilityId, PublicationId, MajorVersion, MinorVersion, Title, Description, AdditionalInformation, ReviewDate, HasCost, Cost, ResourceLicenceId, SensitiveContent, CertificateEnabled, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT	@NewResourceId, 1,ResourceAccessibilityId, null, null, null, 'Copy of ' + Title, Description, AdditionalInformation, null, 0, null, ResourceLicenceId, SensitiveContent, CertificateEnabled, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM	resources.ResourceVersion
            WHERE	id = @CurrentResourceVersionId
            SELECT  @ResourceVersionId = SCOPE_IDENTITY()
            
            INSERT INTO resources.ResourceVersionAuthor (ResourceVersionId, AuthorUserId, AuthorName, Organisation, Role, IsContributor, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT	@ResourceVersionId, AuthorUserId, AuthorName, Organisation, Role, IsContributor, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM	resources.ResourceVersionAuthor
            WHERE	ResourceVersionId = @CurrentResourceVersionId
            AND     Deleted = 0
            
            INSERT INTO resources.ResourceVersionKeyword (ResourceVersionId, Keyword, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT	@ResourceVersionId, Keyword, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM	resources.ResourceVersionKeyword
            WHERE	ResourceVersionId = @CurrentResourceVersionId
            AND     Deleted = 0
              
            -- For now, we can only duplicate Case and Assessment resource types
            IF @CurrentResourceTypeId != 10 and @CurrentResourceTypeId != 11
                BEGIN
                    SET @ErrorMessage = 'Error - Cannot duplicate a resource of type' + CONVERT(NVARCHAR(50), @CurrentResourceTypeId)
                    RAISERROR (@ErrorMessage, -- Message text.  
                        16, -- Severity.  
                        1 -- State.  
                    );
                END
        
            IF @CurrentResourceTypeId = 10
                BEGIN
                    EXECUTE resources.CaseResourceVersionCreateDuplicate @ResourceVersionId, @CurrentResourceVersionId, @UserId, @UserTimezoneOffset
                END
            
            IF @CurrentResourceTypeId = 11
                BEGIN
                    EXECUTE resources.AssessmentResourceVersionCreateDuplicate @ResourceVersionId, @CurrentResourceVersionId, @UserId, @UserTimezoneOffset
                END
            
            EXECUTE resources.ResourceVersionDraftEventCreate @ResourceVersionId, @UserId
        COMMIT
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT
                @ErrorMessage = ERROR_MESSAGE(),
                @ErrorSeverity = ERROR_SEVERITY(),
                @ErrorState = ERROR_STATE();

        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH
END
