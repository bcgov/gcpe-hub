CREATE TABLE [media].[ElectoralDistrict] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [DistrictName] VARCHAR (250)    NOT NULL,
    [CreationDate] DATETIME         NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [SortOrder]    INT              CONSTRAINT [DF_ElectoralDistrict_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ElectoralDistrict] PRIMARY KEY CLUSTERED ([Id] ASC)
);

