CREATE TABLE [media].[CompanyDistribution] (
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [DistributionId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanyDistribution] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [DistributionId] ASC),
    CONSTRAINT [FK_CompanyDistribution_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyDistribution_Distribution] FOREIGN KEY ([DistributionId]) REFERENCES [media].[Distribution] ([Id])
);

