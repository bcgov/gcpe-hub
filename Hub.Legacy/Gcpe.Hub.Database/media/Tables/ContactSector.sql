CREATE TABLE [media].[ContactSector] (
    [ContactId] UNIQUEIDENTIFIER NOT NULL,
    [SectorId]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ContactSector] PRIMARY KEY CLUSTERED ([ContactId] ASC, [SectorId] ASC),
    CONSTRAINT [FK_ContactSector_Contact] FOREIGN KEY ([ContactId]) REFERENCES [media].[Contact] ([Id]),
    CONSTRAINT [FK_ContactSector_Sector] FOREIGN KEY ([SectorId]) REFERENCES [dbo].[Sector] ([Id])
);

