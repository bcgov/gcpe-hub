CREATE TABLE [dbo].[MinistryService]
(
	[MinistryId] UNIQUEIDENTIFIER NOT NULL , 
    [SortIndex] INT NOT NULL, 
    [LinkText] NVARCHAR(MAX) NOT NULL, 
    [LinkUrl] VARCHAR(255) NOT NULL, 
    PRIMARY KEY ([MinistryId], [SortIndex]),
    CONSTRAINT [FK_MinistryService_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [Ministry]([Id])
)