CREATE TABLE [media].[ContactCity] (
    [Id]     UNIQUEIDENTIFIER NOT NULL,
    [CityName]     VARCHAR (250)    NOT NULL,
    [CreationDate] DATETIME         NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [SortOrder]    INT              CONSTRAINT [DF_City_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED ([Id] ASC)
);

