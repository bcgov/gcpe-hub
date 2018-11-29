CREATE TABLE [media].[CompanySpecialtyPublication] (
    [CompanyId]              UNIQUEIDENTIFIER NOT NULL,
    [SpecialtyPublicationId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanySpecialtyPublication] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [SpecialtyPublicationId] ASC),
    CONSTRAINT [FK_CompanySpecialtyPublication_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanySpecialtyPublication_SpecialtyPublication] FOREIGN KEY ([SpecialtyPublicationId]) REFERENCES [media].[SpecialtyPublication] ([Id])
);

