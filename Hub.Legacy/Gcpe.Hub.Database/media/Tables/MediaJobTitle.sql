CREATE TABLE [media].[MediaJobTitle] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [MediaJobTitleName] VARCHAR (250)    NOT NULL,
    [CreationDate]      DATETIME         NOT NULL,
    [ModifiedDate]      DATETIME         NOT NULL,
    [SortOrder]         INT              CONSTRAINT [DF_MediaJobTitle_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_MediaJobTitle] PRIMARY KEY CLUSTERED ([Id] ASC)
);

