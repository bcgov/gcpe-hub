CREATE TABLE [calendar].[ActivityNROrigins] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [ActivityId]          INT              NOT NULL,
    [NROriginId]          INT              NOT NULL,
    [IsActive]            BIT              CONSTRAINT [DF_ActivityNewsReleaseOrigins_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_ActivityNewsReleaseOrigins_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT              NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_ActivityNewsReleaseOrigins_LastUpdatedDateTime] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]       INT              NULL,
    [TimeStamp]           ROWVERSION       NOT NULL,
    [RowGuid]             UNIQUEIDENTIFIER CONSTRAINT [DF_ActivityNewsReleaseOrigins_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_ActivityNROrigins] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActivityNROrigins_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id]),
    CONSTRAINT [FK_ActivityNROrigins_NROrigin] FOREIGN KEY ([NROriginId]) REFERENCES [calendar].[NROrigin] ([Id]),
    CONSTRAINT [FK_ActivityNROrigins_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_ActivityNROrigins_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_ActivityId]
    ON [calendar].[ActivityNROrigins]([ActivityId] ASC);

