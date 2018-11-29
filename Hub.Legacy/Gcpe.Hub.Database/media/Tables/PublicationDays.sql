CREATE TABLE [media].[PublicationDays] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PublicationDaysName] VARCHAR (250)    NOT NULL,
    [CreationDate]        DATETIME         NOT NULL,
    [ModifiedDate]        DATETIME         NOT NULL,
    [SortOrder]           INT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PublicationDays] PRIMARY KEY CLUSTERED ([Id] ASC)
);

