CREATE TABLE [media].[ContactRegion] (
    [ContactId] UNIQUEIDENTIFIER NOT NULL,
    [RegionId]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ContactRegion] PRIMARY KEY CLUSTERED ([ContactId] ASC, [RegionId] ASC),
    CONSTRAINT [FK_ContactRegion_Contact] FOREIGN KEY ([ContactId]) REFERENCES [media].[Contact] ([Id]),
    CONSTRAINT [FK_ContactRegion_Region] FOREIGN KEY ([RegionId]) REFERENCES [media].[NewsRegion] ([Id])
);

