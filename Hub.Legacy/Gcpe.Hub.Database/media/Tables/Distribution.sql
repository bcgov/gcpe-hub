CREATE TABLE [media].[Distribution] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [DistributionName] VARCHAR (250)    NOT NULL,
    [CreationDate]     DATETIME         NOT NULL,
    [ModifiedDate]     DATETIME         NOT NULL,
    [SortOrder]        INT              CONSTRAINT [DF_Distribution_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Distribution] PRIMARY KEY CLUSTERED ([Id] ASC)
);

