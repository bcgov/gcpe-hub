CREATE TABLE [dbo].[MinistryLanguage] (
    [MinistryId] UNIQUEIDENTIFIER NOT NULL,
    [LanguageId] INT              NOT NULL,
    [Name]       NVARCHAR (100)   NOT NULL,
    CONSTRAINT [PK_OrganizationLanguage] PRIMARY KEY CLUSTERED ([MinistryId] ASC, [LanguageId] ASC),
    CONSTRAINT [FK_MinistryLanguage_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([Id]),
    CONSTRAINT [FK_MinistryLanguage_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id]),
    CONSTRAINT [UK_MinistryLanguage_LanguageName] UNIQUE NONCLUSTERED ([LanguageId] ASC, [Name] ASC)
);








GO


