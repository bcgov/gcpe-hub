CREATE TABLE [calendar].[ActivitySharedWith] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [ActivityId]          INT              NOT NULL,
    [MinistryId]          UNIQUEIDENTIFIER NOT NULL,
    [IsActive]            BIT              CONSTRAINT [DF_ActivitySharedWith_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_ActivitySharedWith_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT              NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_ActivitySharedWith_LastUpdatedDateTime] DEFAULT (getdate()) NULL,
    [LastUpdatedBy]       INT              NULL,
    [TimeStamp]           ROWVERSION       NOT NULL,
    [RowGuid]             UNIQUEIDENTIFIER CONSTRAINT [DF_ActivitySharedWith_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_ActivitySharedWith] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActivitySharedWith_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id]),
    CONSTRAINT [FK_ActivitySharedWith_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id]),
    CONSTRAINT [FK_ActivitySharedWith_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_ActivitySharedWith_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_ActivityId]
    ON [calendar].[ActivitySharedWith]([ActivityId] ASC);

