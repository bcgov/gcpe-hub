CREATE TABLE [dbo].[NewsReleaseType] (
    [PageTitle]   NVARCHAR (50)    NOT NULL,
    [LanguageId]  INT              NOT NULL,
    [ReleaseType] INT              NOT NULL,
    [SortOrder]   INT              NOT NULL,
    [PageLayout]  INT              NOT NULL,
    [PageImageId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_NewsReleaseType] PRIMARY KEY CLUSTERED ([PageTitle] ASC, [LanguageId] ASC),
    CONSTRAINT [CK_NewsReleaseType_ReleaseType] CHECK ([ReleaseType]<=(5) AND [ReleaseType]>=(1)),
    CONSTRAINT [FK_NewsReleaseType_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([Id]),
    CONSTRAINT [FK_NewsReleaseType_NewsReleaseImage] FOREIGN KEY ([PageImageId]) REFERENCES [dbo].[NewsReleaseImage] ([Id])
);



