CREATE TABLE [dbo].[NewsReleaseImageLanguage] (
    [ImageId]       UNIQUEIDENTIFIER NOT NULL,
    [LanguageId]    INT              NOT NULL,
    [AlternateName] NVARCHAR (100)   NOT NULL,
    CONSTRAINT [PK_NewsReleaseImageLanguage_1] PRIMARY KEY CLUSTERED ([ImageId] ASC, [LanguageId] ASC),
    CONSTRAINT [FK_NewsReleaseImageLanguage_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([Id]),
    CONSTRAINT [FK_NewsReleaseImageLanguage_NewsReleaseImage] FOREIGN KEY ([ImageId]) REFERENCES [dbo].[NewsReleaseImage] ([Id])
);

