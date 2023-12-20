-- Script to create a small folder tree within an existing catalogue:
--
-- * Folder name 1
--		* Subfolder name 1.1
--			* Subfolder name 1.1.1
--			* Subfolder name 1.1.2
--		* Subfolder name 1.2
--		* Subfolder name 1.3
-- * Folder name 2
--		* Subfolder name 2.1
--		* Subfolder name 2.1
--		* Subfolder name 2.1
-- * Folder name 3
--		* Subfolder name 3.1
--		* Subfolder name 3.1
--		* Subfolder name 3.1

DECLARE @CatalogueNodeId INT = <XXX> -- Enter the Id of an existing catalogue Node record.

DECLARE @Folder1NodeId INT
EXEC hierarchy.FolderCreate @CatalogueNodeId, 'Folder name 1', 'Folder 1 description.', @Folder1NodeId output

DECLARE @Subfolder1NodeId INT
EXEC hierarchy.FolderCreate @Folder1NodeId, 'Subfolder name 1.1', 'Subfolder 1.1 description', @Subfolder1NodeId output

DECLARE @NodeId INT
EXEC hierarchy.FolderCreate @Subfolder1NodeId, 'Subfolder name 1.1.1', 'Subfolder 1.1.1 description', @NodeId output
EXEC hierarchy.FolderCreate @Subfolder1NodeId, 'Subfolder name 1.1.2', 'Subfolder 1.1.1 description', @NodeId output

EXEC hierarchy.FolderCreate @Folder1NodeId, 'Subfolder name 1.2', 'Subfolder 1.2 description', @NodeId output
EXEC hierarchy.FolderCreate @Folder1NodeId, 'Subfolder name 1.3', 'Subfolder 1.3 description', @NodeId output

DECLARE @Folder2NodeId INT
EXEC hierarchy.FolderCreate @CatalogueNodeId, 'Folder name 2', 'Folder 2 description.', @NodeId output
EXEC hierarchy.FolderCreate @Folder2NodeId, 'Subfolder name 2.1', 'Subfolder 2.1 description', @NodeId output
EXEC hierarchy.FolderCreate @Folder2NodeId, 'Subfolder name 2.2', 'Subfolder 2.2 description', @NodeId output
EXEC hierarchy.FolderCreate @Folder2NodeId, 'Subfolder name 2.3', 'Subfolder 2.3 description', @NodeId output

DECLARE @Folder3NodeId INT
EXEC hierarchy.FolderCreate @CatalogueNodeId, 'Folder name 3', 'Folder 3 description.', @Folder3NodeId output
EXEC hierarchy.FolderCreate @Folder3NodeId, 'Subfolder name 3.1', 'Subfolder 3.1 description', @NodeId output
EXEC hierarchy.FolderCreate @Folder3NodeId, 'Subfolder name 3.2', 'Subfolder 3.2 description', @NodeId output
EXEC hierarchy.FolderCreate @Folder3NodeId, 'Subfolder name 3.3', 'Subfolder 3.3 description', @NodeId output