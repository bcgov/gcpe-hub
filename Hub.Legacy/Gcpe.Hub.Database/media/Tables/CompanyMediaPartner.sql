CREATE TABLE [media].[CompanyMediaPartner] (
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [MediaPartnerId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanyMediaPartner] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [MediaPartnerId] ASC),
    CONSTRAINT [FK_CompanyMediaPartner_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyMediaPartner_MediaPartner] FOREIGN KEY ([MediaPartnerId]) REFERENCES [media].[Company] ([Id])
);

