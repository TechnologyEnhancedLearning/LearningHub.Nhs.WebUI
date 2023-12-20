/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/* 
Card 9399 has added a LastShownDate column to the CatalogueNodeVersion table 
This is used in the dashboard to correctly order the catalogues for the "Latest" cards.

However, existing visible catalogues need to be updated to have this column populated.
Existing behavior is that the catalogues are ordered based on the createdate of the node,
so we will use that for our default.
*/

/*
For each catalogue node version with a non-hidden node, we want to take the node create date and set the catalogue node version LastShownDate with it.
*/

UPDATE cnv
SET cnv.LastShownDate = n.CreateDate
FROM 
    hierarchy.CatalogueNodeVersion cnv
    JOIN hierarchy.NodeVersion nv on cnv.NodeVersionId = nv.Id
    JOIN hierarchy.Node n on nv.NodeId = n.Id
WHERE 
    n.Hidden = 0 AND cnv.LastShownDate IS NULL