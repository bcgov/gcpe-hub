CREATE TABLE [media].[CompanyPhoneNumber] (
    [Id]      UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]          UNIQUEIDENTIFIER NOT NULL,
    [PhoneNumber]          VARCHAR (50)     NOT NULL,
    [PhoneTypeId]        UNIQUEIDENTIFIER NOT NULL,
    [CreationDate]         DATETIME         NOT NULL,
    [ModifiedDate]         DATETIME         NOT NULL,
    [PhoneNumberExtension] NVARCHAR (15)    NULL,
    CONSTRAINT [PK_CompanyPhoneNumber] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanyPhoneNumber_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyPhoneNumber_PhoneType] FOREIGN KEY ([PhoneTypeId]) REFERENCES [media].[PhoneType] ([Id])
);

