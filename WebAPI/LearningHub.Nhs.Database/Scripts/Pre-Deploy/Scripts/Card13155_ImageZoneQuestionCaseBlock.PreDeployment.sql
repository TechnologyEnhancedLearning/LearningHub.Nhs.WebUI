/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
	Nat Dean-Lewis - 26 Jan 2022
	Card 13155 - Image Zone Question Case Block
--------------------------------------------------------------------------------------
*/

-- Rename TABLES containing 'WholeSlideImageAnnotation' to contain just 'ImageAnnotation' instead
EXEC sp_rename 'resources.WholeSlideImageAnnotation', 'ImageAnnotation';
EXEC sp_rename 'resources.WholeSlideImageAnnotationMark', 'ImageAnnotationMark';

-- Rename COLUMNS containing 'WholeSlideImageAnnotation' to contain just 'ImageAnnotation' instead
EXEC sp_rename 'resources.ImageAnnotationMark.WholeSlideImageAnnotationId', 'ImageAnnotationId', 'COLUMN';