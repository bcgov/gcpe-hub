CREATE TABLE [media].[ContactPhoneNumber] (
    [Id]      UNIQUEIDENTIFIER NOT NULL,
    [ContactId]          UNIQUEIDENTIFIER NOT NULL,
    [PhoneNumber]          VARCHAR (50)     NOT NULL,
    [PhoneTypeId]        UNIQUEIDENTIFIER NOT NULL,
    [CreationDate]         DATETIME         NOT NULL,
    [ModifiedDate]         DATETIME         NOT NULL,
    [PhoneNumberExtension] NVARCHAR (15)    NULL,
    CONSTRAINT [PK_ContactPhoneNumber] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ContactPhoneNumber_Contact] FOREIGN KEY ([ContactId]) REFERENCES [media].[Contact] ([Id]),
    CONSTRAINT [FK_ContactPhoneNumber_PhoneType] FOREIGN KEY ([PhoneTypeId]) REFERENCES [media].[PhoneType] ([Id])
);

