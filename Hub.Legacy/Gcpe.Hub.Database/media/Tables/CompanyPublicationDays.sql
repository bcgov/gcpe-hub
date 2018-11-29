CREATE TABLE [media].[CompanyPublicationDays] (
    [CompanyId]         UNIQUEIDENTIFIER NOT NULL,
    [PublicationDaysId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanyPublicationDays] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [PublicationDaysId] ASC),
    CONSTRAINT [FK_CompanyPublicationDays_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyPublicationDays_PublicationDays] FOREIGN KEY ([PublicationDaysId]) REFERENCES [media].[PublicationDays] ([Id])
);

