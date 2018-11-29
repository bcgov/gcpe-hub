CREATE TABLE [dbo].[MinistryNewsletter]
(
	[MinistryId] UNIQUEIDENTIFIER NOT NULL ,
    [NewsletterId] INT NOT NULL, 
    PRIMARY KEY ([MinistryId], [NewsletterId]), 
    CONSTRAINT [FK_MinistryNewsletter_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [Ministry]([Id])
)

GO

CREATE UNIQUE INDEX [IX_MinistryNewsletter_MinistryNewsletter] ON [dbo].[MinistryNewsletter] ([MinistryId], [NewsletterId])
