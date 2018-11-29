CREATE TABLE [media].[SpecialtyPublication] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [SpecialtyPublicationName] VARCHAR (250)    NOT NULL,
    [CreationDate]             DATETIME         NOT NULL,
    [ModifiedDate]             DATETIME         NOT NULL,
    [SortOrder]                INT              CONSTRAINT [DF_SpecialtyPublication_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SpecialtyPublication] PRIMARY KEY CLUSTERED ([Id] ASC)
);

