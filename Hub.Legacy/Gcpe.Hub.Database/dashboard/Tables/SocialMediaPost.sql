CREATE TABLE [dashboard].[SocialMediaPost]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Url] VARCHAR(255) NOT NULL,
	[SortOrder] INT NOT NULL DEFAULT ((0)),
    [Timestamp] DATETIME NOT NULL DEFAULT (getdate()),
    [IsActive] BIT NOT NULL 
)
