CREATE TABLE [calendar].[NewsFeed] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [ActivityId]          INT              NULL,
    [MinistryId]          UNIQUEIDENTIFIER NOT NULL,
    [Text]                NVARCHAR (1000)  NULL,
    [Description]         NVARCHAR (50)    NULL,
    [IsActive]            BIT              CONSTRAINT [DF_NewsFeed_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_NewsFeed_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT              NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_NewsFeed_LastUpdatedDateTime] DEFAULT (getdate()) NULL,
    [LastUpdatedBy]       INT              NULL,
    [TimeStamp]           ROWVERSION       NOT NULL,
    [RowGuid]             UNIQUEIDENTIFIER CONSTRAINT [DF_NewsFeed_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_NewsFeed] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NewsFeed_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id]),
    CONSTRAINT [FK_NewsFeed_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id]),
    CONSTRAINT [FK_NewsFeed_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_NewsFeed_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id])
);









