CREATE TABLE [calendar].[ActivityFilter] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [QueryString]         NVARCHAR (300)   NULL,
    [Name]                NVARCHAR (200)   NULL,
    [SortOrder]           INT              NULL,
    [IsActive]            BIT              CONSTRAINT [DF_ActivityFilter_IsActive] DEFAULT ((1)) NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_ActivityFilter_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT              NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_ActivityFilter_LastUpdatedDateTime] DEFAULT (getdate()) NULL,
    [LastUpdatedBy]       INT              NULL,
    [TimeStamp]           ROWVERSION       NULL,
    [RowGuid]             UNIQUEIDENTIFIER CONSTRAINT [DF_ActivityFilter_RowGuid] DEFAULT (newid()) NULL,
    CONSTRAINT [PK_ActivityFilter] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActivityFilter_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_ActivityFilter_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id])
);









