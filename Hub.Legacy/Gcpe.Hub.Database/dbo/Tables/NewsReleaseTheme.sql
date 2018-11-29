CREATE TABLE [dbo].[NewsReleaseTheme] (
    [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
    [ThemeId]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_NewsReleaseTheme] PRIMARY KEY CLUSTERED ([ReleaseId] ASC, [ThemeId] ASC),
    CONSTRAINT [FK_NewsReleaseTheme_NewsRelease] FOREIGN KEY ([ReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_NewsReleaseTheme_Theme] FOREIGN KEY ([ThemeId]) REFERENCES [dbo].[Theme] ([Id])
);



