CREATE TABLE [media].[CompanyRegion] (
    [CompanyId] UNIQUEIDENTIFIER NOT NULL,
    [RegionId]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanyRegion] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [RegionId] ASC),
    CONSTRAINT [FK_CompanyRegion_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyRegion_Region] FOREIGN KEY ([RegionId]) REFERENCES [media].[NewsRegion] ([Id])
);

