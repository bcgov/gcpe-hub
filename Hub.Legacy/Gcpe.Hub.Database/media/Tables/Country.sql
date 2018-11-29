CREATE TABLE [media].[Country] (
    [Id]   UNIQUEIDENTIFIER NOT NULL,
    [CountryName]   VARCHAR (250)    NOT NULL,
    [CountryAbbrev] VARCHAR (15)     NOT NULL,
    [CreationDate]  DATETIME         NOT NULL,
    [ModifiedDate]  DATETIME         NOT NULL,
    [SortOrder]     INT              CONSTRAINT [DF_Country_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([Id] ASC)
);

