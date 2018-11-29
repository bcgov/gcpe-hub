CREATE TABLE [calendar].[Log] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [ActivityId]          INT              NOT NULL,
    [LogType]             INT              NOT NULL,
    [TableName]           NVARCHAR (50)    NULL,
    [FieldName]           NVARCHAR (1000)  NULL,
    [OldValue]            NVARCHAR (1000)  NULL,
    [NewValue]            NVARCHAR (1000)  NULL,
    [Operation]           NVARCHAR (50)    NOT NULL,
    [IsActive]            BIT              CONSTRAINT [DF_Log_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_Log_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT              NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_Log_LastUpdatedDateTime] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]       INT              NULL,
    [TimeStamp]           ROWVERSION       NOT NULL,
    [RowGuid]             UNIQUEIDENTIFIER CONSTRAINT [DF_Log_RowGuid] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Log_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id]),
    CONSTRAINT [FK_Log_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_Log_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id])
);





