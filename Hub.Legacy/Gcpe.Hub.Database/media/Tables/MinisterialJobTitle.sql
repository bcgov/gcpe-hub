CREATE TABLE [media].[MinisterialJobTitle] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [MinisterialJobTitleName] VARCHAR (250)    NOT NULL,
    [CreationDate]            DATETIME         NOT NULL,
    [ModifiedDate]            DATETIME         NOT NULL,
    [SortOrder]               INT              CONSTRAINT [DF_MinisterialJobTitle_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_MinisterialJobTitle] PRIMARY KEY CLUSTERED ([Id] ASC)
);

