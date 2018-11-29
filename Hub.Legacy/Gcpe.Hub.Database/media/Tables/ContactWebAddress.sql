CREATE TABLE [media].[ContactWebAddress] (
    [Id]     UNIQUEIDENTIFIER NOT NULL,
    [ContactId]        UNIQUEIDENTIFIER NOT NULL,
    [WebAddress]         VARCHAR (250)    NOT NULL,
    [WebAddressTypeId] UNIQUEIDENTIFIER NOT NULL,
    [CreationDate]       DATETIME         NOT NULL,
    [ModifiedDate]       DATETIME         NOT NULL,
    CONSTRAINT [PK_ContactWebAddress] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ContactWebAddress_Contact] FOREIGN KEY ([ContactId]) REFERENCES [media].[Contact] ([Id]),
    CONSTRAINT [FK_ContactWebAddress_WebAddressType] FOREIGN KEY ([WebAddressTypeId]) REFERENCES [media].[WebAddressType] ([Id])
);

