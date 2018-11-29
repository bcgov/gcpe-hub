CREATE TABLE [calendar].[ActivityCategories] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [ActivityId]          INT              NOT NULL,
    [CategoryId]          INT              NOT NULL,
    [IsActive]            BIT              CONSTRAINT [DF_ActivityCategories_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_ActivityCategories_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT              NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_ActivityCategories_LastUpdatedDateTime] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]       INT              NULL,
    [TimeStamp]           ROWVERSION       NOT NULL,
    [RowGuid]             UNIQUEIDENTIFIER CONSTRAINT [DF_ActivityCategories_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_ActivityCategories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActivityCategories_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id]),
    CONSTRAINT [FK_ActivityCategories_Category] FOREIGN KEY ([CategoryId]) REFERENCES [calendar].[Category] ([Id]),
    CONSTRAINT [FK_ActivityCategories_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_ActivityCategories_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id])
);








GO
CREATE NONCLUSTERED INDEX [IX_ActivityId]
    ON [calendar].[ActivityCategories]([ActivityId] ASC);

