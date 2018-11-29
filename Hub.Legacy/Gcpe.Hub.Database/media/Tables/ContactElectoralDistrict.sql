CREATE TABLE [media].[ContactElectoralDistrict] (
    [ContactId]  UNIQUEIDENTIFIER NOT NULL,
    [DistrictId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ContactElectoralDistrict] PRIMARY KEY CLUSTERED ([ContactId] ASC, [DistrictId] ASC),
    CONSTRAINT [FK_ContactElectoralDistrict_Contact] FOREIGN KEY ([ContactId]) REFERENCES [media].[Contact] ([Id]),
    CONSTRAINT [FK_ContactElectoralDistrict_ElectoralDistrict] FOREIGN KEY ([DistrictId]) REFERENCES [media].[ElectoralDistrict] ([Id])
);

