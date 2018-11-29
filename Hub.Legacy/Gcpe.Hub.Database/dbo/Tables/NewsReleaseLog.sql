CREATE TABLE [dbo].[NewsReleaseLog] (
    [Id]          INT                IDENTITY (1, 1) NOT NULL,
    [ReleaseId]   UNIQUEIDENTIFIER   NOT NULL,
    [DateTime]    DATETIMEOFFSET (7) NOT NULL,
    [UserId]      UNIQUEIDENTIFIER   NULL,
    [Description] NVARCHAR (200)     NOT NULL,
    CONSTRAINT [PK_NewsReleaseLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NewsReleaseLog_NewsRelease] FOREIGN KEY ([ReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_NewsReleaseLog_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
);





