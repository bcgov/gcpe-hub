CREATE TABLE [media].[PrintCategory] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PrintCategoryName] VARCHAR (250)    NOT NULL,
    [CreationDate]      DATETIME         NOT NULL,
    [ModifiedDate]      DATETIME         NOT NULL,
    [SortOrder]         INT              CONSTRAINT [DF_PrintCategory_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PrintCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
);

