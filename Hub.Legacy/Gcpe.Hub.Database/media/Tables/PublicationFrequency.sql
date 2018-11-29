CREATE TABLE [media].[PublicationFrequency] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PublicationFrequencyName] VARCHAR (250)    NOT NULL,
    [CreationDate]             DATETIME         NOT NULL,
    [ModifiedDate]             DATETIME         NOT NULL,
    [SortOrder]                INT              CONSTRAINT [DF_PublicationFrequency_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PublicationFrequency] PRIMARY KEY CLUSTERED ([Id] ASC)
);

