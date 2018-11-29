CREATE TABLE [dashboard].[SocialMediaPost]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [EmbedCode] TEXT NOT NULL, 
    [Timestamp] DATETIME NOT NULL DEFAULT (getdate())
)
