CREATE TABLE [media].[CompanyEthnicity] (
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [EthnicityId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanyEthnicity] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [EthnicityId] ASC),
    CONSTRAINT [FK_CompanyEthnicity_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyEthnicity_Ethnicity] FOREIGN KEY ([EthnicityId]) REFERENCES [media].[Ethnicity] ([Id])
);

