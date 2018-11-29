CREATE TABLE [calendar].[ActivityInitiatives] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [ActivityId]          INT              NOT NULL,
    [InitiativeId]        INT              NOT NULL,
    [IsActive]            BIT              CONSTRAINT [DF_ActivityInitiatives_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_ActivityInitiatives_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT              NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_ActivityInitiatives_LastUpdatedDateTime] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]       INT              NULL,
    [TimeStamp]           ROWVERSION       NOT NULL,
    [RowGuid]             UNIQUEIDENTIFIER CONSTRAINT [DF_ActivityInitiatives_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_ActivityInitiatives] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActivityInitiatives_Initiative] FOREIGN KEY ([InitiativeId]) REFERENCES [calendar].[Initiative] ([Id]),
    CONSTRAINT [FK_ActivityInitiatives_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id]),
    CONSTRAINT [FK_ActivityInitiatives_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_ActivityInitiatives_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ActivityId]
    ON [calendar].[ActivityInitiatives]([ActivityId] ASC);

