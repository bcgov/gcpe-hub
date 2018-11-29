CREATE TABLE [calendar].[CommunicationContact] (
    [Id]                INT              IDENTITY (1, 1) NOT NULL,
    [SystemUserId]      INT              NOT NULL,
    [Name]              NVARCHAR (100)   NULL,
    [MinistryShortName] VARCHAR (50)     NULL,
    [MinistryId]        UNIQUEIDENTIFIER NULL,
    [SortOrder]         INT              NULL,
    [IsActive]          BIT              CONSTRAINT [DF_CommunicationContact_IsActive] DEFAULT ((1)) NULL,
    [TimeStamp]         ROWVERSION       NULL,
    [RowGuid]           UNIQUEIDENTIFIER CONSTRAINT [DF_CommunicationContact_RowGuid] DEFAULT (newid()) NULL,
    CONSTRAINT [PK_CommunicationContact] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CommunicationContact_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id]),
    CONSTRAINT [FK_CommunicationContact_SystemUser_SystemUser] FOREIGN KEY ([SystemUserId]) REFERENCES [calendar].[SystemUser] ([Id])
);














GO

