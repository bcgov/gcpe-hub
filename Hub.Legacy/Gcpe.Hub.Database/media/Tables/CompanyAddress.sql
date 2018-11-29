CREATE TABLE [media].[CompanyAddress] (
    [Id]   UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [StreetAddress] VARCHAR (250)    NOT NULL,
    [CityId]      UNIQUEIDENTIFIER NULL,
    [CityName]      VARCHAR (150)    NULL,
    [ProvStateId] UNIQUEIDENTIFIER NULL,
    [ProvStateName] VARCHAR (150)    NULL,
    [PostalZipCode] VARCHAR (50)     NOT NULL,
    [CountryId]   UNIQUEIDENTIFIER NOT NULL,
    [AddressType]   INT              NOT NULL,
    [CreationDate]  DATETIME         NOT NULL,
    [ModifiedDate]  DATETIME         NOT NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Address_City] FOREIGN KEY ([CityId]) REFERENCES [media].[ContactCity] ([Id]),
    CONSTRAINT [FK_Address_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_Address_Country] FOREIGN KEY ([CountryId]) REFERENCES [media].[Country] ([Id]),
    CONSTRAINT [FK_Address_ProvState] FOREIGN KEY ([ProvStateId]) REFERENCES [media].[ProvState] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_AddressType]
    ON [media].[CompanyAddress]([AddressType] ASC);
