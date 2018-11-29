CREATE TABLE [media].[MediaType] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [MediaTypeName] VARCHAR (250)    NOT NULL,
    [CreationDate]  DATETIME         NOT NULL,
    [ModifiedDate]  DATETIME         NOT NULL,
    [SortOrder]     INT              CONSTRAINT [DF_MediaType_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_MediaType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

