CREATE TABLE [media].[CompanyElectoralDistrict] (
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [DistrictId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanyElectoralDistrict] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [DistrictId] ASC),
    CONSTRAINT [FK_CompanyElectoralDistrict_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyElectoralDistrict_ElectoralDistrict] FOREIGN KEY ([DistrictId]) REFERENCES [media].[ElectoralDistrict] ([Id])
);

