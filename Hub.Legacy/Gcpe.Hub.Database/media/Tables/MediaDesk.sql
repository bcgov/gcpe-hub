CREATE TABLE [media].[MediaDesk] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [MediaDeskName] VARCHAR (250)    NOT NULL,
    [CreationDate]  DATETIME         NOT NULL,
    [ModifiedDate]  DATETIME         NOT NULL,
    [SortOrder]     INT              CONSTRAINT [DF_MediaDesk_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_MediaDesk] PRIMARY KEY CLUSTERED ([Id] ASC)
);

