CREATE TABLE [media].[CompanyLanguage] (
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [LanguageId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanyLanguage] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [LanguageId] ASC),
    CONSTRAINT [FK_CompanyLanguage_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyLanguage_Language] FOREIGN KEY ([LanguageId]) REFERENCES [media].[MediaLanguage] ([Id])
);

