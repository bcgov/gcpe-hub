CREATE TABLE [dbo].[NewsReleaseLanguage] (
    [ReleaseId]  UNIQUEIDENTIFIER NOT NULL,
    [LanguageId] INT              NOT NULL,
    [Location]   NVARCHAR (50)    NOT NULL,
    [Summary]    NVARCHAR(MAX)   NOT NULL,
    [SocialMediaHeadline] NVARCHAR(MAX) NULL, 
    [SocialMediaSummary] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_ReleaseLanguage] PRIMARY KEY CLUSTERED ([ReleaseId] ASC, [LanguageId] ASC),
    CONSTRAINT [FK_ReleaseLanguage_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([Id]),
    CONSTRAINT [FK_ReleaseLanguage_Release] FOREIGN KEY ([ReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]) ON DELETE CASCADE
);











