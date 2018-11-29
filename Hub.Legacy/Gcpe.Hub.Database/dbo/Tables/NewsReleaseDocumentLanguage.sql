CREATE TABLE [dbo].[NewsReleaseDocumentLanguage] (
    [DocumentId]    UNIQUEIDENTIFIER NOT NULL,
    [LanguageId]    INT              NOT NULL,
    [PageImageId]   UNIQUEIDENTIFIER NULL,
    [PageTitle]     NVARCHAR (50)    NOT NULL,
    [Organizations] NVARCHAR (250)   NOT NULL,
    [Headline]      NVARCHAR (255)   NOT NULL,
    [Subheadline]   NVARCHAR (100)   NOT NULL,
    [Byline]        NVARCHAR (250)   NOT NULL,
    [BodyHtml]      NVARCHAR (MAX)   NOT NULL,  
    CONSTRAINT [PK_DocumentLanguage] PRIMARY KEY CLUSTERED ([DocumentId] ASC, [LanguageId] ASC),
    CONSTRAINT [FK_DocumentLanguage_Document] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[NewsReleaseDocument] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DocumentLanguage_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([Id]),
    CONSTRAINT [FK_DocumentLanguage_Image] FOREIGN KEY ([PageImageId], [LanguageId]) REFERENCES [dbo].[NewsReleaseImageLanguage] ([ImageId], [LanguageId])
);



