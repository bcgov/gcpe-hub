CREATE TABLE [media].[MediaLanguage] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [LanguageName] VARCHAR (150)    NOT NULL,
    [CreationDate] DATETIME         NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [SortOrder]    INT              CONSTRAINT [DF_Language_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Lang] PRIMARY KEY CLUSTERED ([Id] ASC)
);

