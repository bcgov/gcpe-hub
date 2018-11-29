CREATE TABLE [dbo].[NewsReleaseService] (
    [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
    [ServiceId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_NewsReleaseService] PRIMARY KEY CLUSTERED ([ReleaseId] ASC, [ServiceId] ASC),
    CONSTRAINT [FK_NewsReleaseService_NewsRelease] FOREIGN KEY ([ReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_NewsReleaseService_Service] FOREIGN KEY ([ServiceId]) REFERENCES [dbo].[Service] ([Id])
);

