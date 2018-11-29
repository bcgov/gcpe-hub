CREATE TABLE [media].[CompanyWebAddress] (
    [Id]     UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]        UNIQUEIDENTIFIER NOT NULL,
    [WebAddress]         VARCHAR (250)    NOT NULL,
    [WebAddressTypeId] UNIQUEIDENTIFIER NOT NULL,
    [CreationDate]       DATETIME         NOT NULL,
    [ModifiedDate]       DATETIME         NOT NULL,
    CONSTRAINT [PK_CompanyWebAddress] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanyWebAddress_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyWebAddress_WebAddressType] FOREIGN KEY ([WebAddressTypeId]) REFERENCES [media].[WebAddressType] ([Id])
);

