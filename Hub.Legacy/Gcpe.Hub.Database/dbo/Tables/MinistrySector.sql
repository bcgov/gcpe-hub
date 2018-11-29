CREATE TABLE [dbo].[MinistrySector] (
    [MinistryId] UNIQUEIDENTIFIER NOT NULL,
    [SectorId]   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_MinistrySector] PRIMARY KEY CLUSTERED ([MinistryId] ASC, [SectorId] ASC),
    CONSTRAINT [FK_MinistrySector_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id]),
    CONSTRAINT [FK_MinistrySector_Sector] FOREIGN KEY ([SectorId]) REFERENCES [dbo].[Sector] ([Id])
);

