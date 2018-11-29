CREATE TABLE [dbo].[NewsReleaseTag] (
    [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
    [TagId]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_NewsReleaseTag] PRIMARY KEY CLUSTERED ([ReleaseId] ASC, [TagId] ASC),
    CONSTRAINT [FK_NewsReleaseTag_NewsRelease] FOREIGN KEY ([ReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_NewsReleaseTag_Tag] FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tag] ([Id])
);


