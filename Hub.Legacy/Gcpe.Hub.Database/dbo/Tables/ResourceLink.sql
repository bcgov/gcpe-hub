CREATE TABLE [dbo].[ResourceLink]
(
	[SortIndex] INT NOT NULL, 
    [LinkText] NVARCHAR(MAX) NOT NULL, 
    [LinkUrl] VARCHAR(255) NOT NULL, 
    CONSTRAINT [PK_ResourceLink] PRIMARY KEY ([SortIndex]),
)