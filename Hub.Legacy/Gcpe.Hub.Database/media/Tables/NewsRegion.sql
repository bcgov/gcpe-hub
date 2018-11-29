CREATE TABLE [media].[NewsRegion] (
    [Id]   UNIQUEIDENTIFIER NOT NULL,
    [RegionName]   VARCHAR (250)    NOT NULL,
    [CreationDate] DATETIME         NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [SortOrder]    INT              CONSTRAINT [DF_Region_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED ([Id] ASC)
);

