CREATE TABLE [media].[CompanySector] (
    [CompanyId] UNIQUEIDENTIFIER NOT NULL,
    [SectorId]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanySector] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [SectorId] ASC),
    CONSTRAINT [FK_CompanySector_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanySector_Sector] FOREIGN KEY ([SectorId]) REFERENCES [dbo].[Sector] ([Id])
);

