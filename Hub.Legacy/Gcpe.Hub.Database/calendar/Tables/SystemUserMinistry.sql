CREATE TABLE [calendar].[SystemUserMinistry] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [SystemUserId]        INT              NULL,
    [MinistryId]          UNIQUEIDENTIFIER NULL,
    [IsActive]            BIT              CONSTRAINT [DF_SystemUserMinistry_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_SystemUserMinistry_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT              NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_SystemUserMinistry_LastUpdatedDateTime] DEFAULT (getdate()) NULL,
    [LastUpdatedBy]       INT              NULL,
    [TimeStamp]           ROWVERSION       NOT NULL,
    [RowGuid]             UNIQUEIDENTIFIER CONSTRAINT [DF_SystemUserMinistry_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_SystemUserMinistry] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SystemUserMinistry_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id]),
    CONSTRAINT [FK_SystemUserMinistry_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_SystemUserMinistry_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_SystemUserMinistry_SystemUserId] FOREIGN KEY ([SystemUserId]) REFERENCES [calendar].[SystemUser] ([Id])
);







