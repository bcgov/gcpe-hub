CREATE TABLE [dbo].[SectorLanguage] (
    [SectorId]   UNIQUEIDENTIFIER NOT NULL,
    [LanguageId] INT              NOT NULL,
    [Name]       NVARCHAR (50)    NOT NULL,
    CONSTRAINT [PK_SectorLanguage] PRIMARY KEY CLUSTERED ([SectorId] ASC, [LanguageId] ASC),
    CONSTRAINT [FK_SectorLanguage_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([Id]),
    CONSTRAINT [FK_SectorLanguage_Sector] FOREIGN KEY ([SectorId]) REFERENCES [dbo].[Sector] ([Id]),
    CONSTRAINT [UK_SectorLanguage_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);






GO


