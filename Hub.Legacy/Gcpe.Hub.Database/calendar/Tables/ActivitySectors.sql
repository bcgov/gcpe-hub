CREATE TABLE [calendar].[ActivitySectors] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [ActivityId]          INT              NOT NULL,
    [SectorId]            UNIQUEIDENTIFIER NOT NULL,
    [IsActive]            BIT              CONSTRAINT [DF_ActivitySectors_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_ActivitySectors_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT              NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_ActivitySectors_LastUpdatedDateTime] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]       INT              NULL,
    [TimeStamp]           ROWVERSION       NOT NULL,
    [RowGuid]             UNIQUEIDENTIFIER CONSTRAINT [DF_ActivitySectors_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_ActivitySectors] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActivitySectors_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id]),
    CONSTRAINT [FK_ActivitySectors_Sector] FOREIGN KEY ([SectorId]) REFERENCES [dbo].[Sector] ([Id]),
    CONSTRAINT [FK_ActivitySectors_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_ActivitySectors_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id])
);








GO
CREATE NONCLUSTERED INDEX [IX_ActivityId]
    ON [calendar].[ActivitySectors]([ActivityId] ASC);

