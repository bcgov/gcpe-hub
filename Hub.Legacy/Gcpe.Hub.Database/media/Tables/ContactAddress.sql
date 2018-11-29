CREATE TABLE [media].[ContactAddress] (
    [Id]   UNIQUEIDENTIFIER NOT NULL,
    [ContactId]   UNIQUEIDENTIFIER NOT NULL,
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
    CONSTRAINT [PK_ContactAddress] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Address_Contact] FOREIGN KEY ([ContactId]) REFERENCES [media].[Contact] ([Id]),
    CONSTRAINT [FK_ContactAddress_City] FOREIGN KEY ([CityId]) REFERENCES [media].[ContactCity] ([Id]),
    CONSTRAINT [FK_ContactAddress_Country] FOREIGN KEY ([CountryId]) REFERENCES [media].[Country] ([Id]),
    CONSTRAINT [FK_ContactAddress_ProvState] FOREIGN KEY ([ProvStateId]) REFERENCES [media].[ProvState] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_AddressType]
    ON [media].[ContactAddress]([AddressType] ASC);

