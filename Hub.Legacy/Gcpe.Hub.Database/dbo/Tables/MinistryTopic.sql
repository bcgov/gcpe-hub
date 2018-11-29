CREATE TABLE [dbo].[MinistryTopic]
(
	[MinistryId] UNIQUEIDENTIFIER NOT NULL , 
    [SortIndex] INT NOT NULL, 
    [LinkText] NVARCHAR(MAX) NOT NULL, 
    [LinkUrl] VARCHAR(255) NOT NULL, 
    PRIMARY KEY ([MinistryId], [SortIndex]), 
    CONSTRAINT [FK_MinistryTopic_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [Ministry]([Id])
)
