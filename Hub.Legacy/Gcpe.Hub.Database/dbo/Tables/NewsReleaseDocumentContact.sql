CREATE TABLE [dbo].[NewsReleaseDocumentContact] (
    [DocumentId]  UNIQUEIDENTIFIER NOT NULL,
    [LanguageId]  INT              NOT NULL,
    [SortIndex]   INT              NOT NULL,
    [Information] NVARCHAR (250)   NOT NULL,
    CONSTRAINT [PK_DocumentLanguageContact] PRIMARY KEY CLUSTERED ([DocumentId] ASC, [LanguageId] ASC, [SortIndex] ASC),
    CONSTRAINT [FK_DocumentLanguageContact_Document] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[NewsReleaseDocument] ([Id]),
    CONSTRAINT [FK_DocumentLanguageContact_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([Id]),
    CONSTRAINT [FK_NewsDocumentLanguageContact_NewsDocumentLanguage] FOREIGN KEY ([DocumentId], [LanguageId]) REFERENCES [dbo].[NewsReleaseDocumentLanguage] ([DocumentId], [LanguageId]) ON DELETE CASCADE
);

