CREATE TABLE [dbo].[NewsReleaseMinistry] (
    [ReleaseId]  UNIQUEIDENTIFIER NOT NULL,
    [MinistryId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_NewsReleaseMinistry] PRIMARY KEY CLUSTERED ([ReleaseId] ASC, [MinistryId] ASC),
    CONSTRAINT [FK_NewsReleaseMinistry_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id]),
    CONSTRAINT [FK_NewsReleaseMinistry_NewsRelease] FOREIGN KEY ([ReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]) ON DELETE CASCADE
);



