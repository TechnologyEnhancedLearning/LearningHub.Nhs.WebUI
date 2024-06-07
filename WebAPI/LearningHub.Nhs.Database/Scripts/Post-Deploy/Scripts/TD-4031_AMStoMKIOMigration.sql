/* 
	TD-4031
	Progress MediaKind production implementation, including migration of content and functionality.
*/

/* 	https://hee-tis.atlassian.net/browse/TD-4032.
Remove WebM from the list of Learning Hub supported filetypes (it is not supported by MediaKind)
*/

Update [resources].[FileType] Set  [NotAllowed] = 1 Where Extension = 'webm';


/* 	https://hee-tis.atlassian.net/browse/TD-4102
Update AMS streaming locators to MediaKind streaming locators in tables
*/

Update [resources].[VideoFile]
SET LocatorUri = REPLACE(LocatorUri, 'https://ukslearninghubdevcontent-ukso1.streaming.media.azure.net', 'https://ep-defaultlhdev-mediakind02-dev-by-am-sl.uksouth.streaming.mediakind.com')
WHERE LocatorUri LIKE '%https://ukslearninghubdevcontent-ukso1.streaming.media.azure.net%';



