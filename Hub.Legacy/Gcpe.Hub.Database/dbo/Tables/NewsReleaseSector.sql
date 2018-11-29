CREATE TABLE [dbo].[NewsReleaseSector] (
    [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
    [SectorId]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_NewsReleaseSector] PRIMARY KEY CLUSTERED ([ReleaseId] ASC, [SectorId] ASC),
    CONSTRAINT [FK_NewsReleaseSector_NewsRelease] FOREIGN KEY ([ReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_NewsReleaseSector_Sector] FOREIGN KEY ([SectorId]) REFERENCES [dbo].[Sector] ([Id])
);



