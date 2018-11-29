CREATE TABLE [dbo].[NewsReleaseDocument] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [ReleaseId]  UNIQUEIDENTIFIER NOT NULL,
    [SortIndex]  INT              NOT NULL,
    [PageLayout] INT              NOT NULL,
    CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Document_Release] FOREIGN KEY ([ReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]) ON DELETE CASCADE
);



