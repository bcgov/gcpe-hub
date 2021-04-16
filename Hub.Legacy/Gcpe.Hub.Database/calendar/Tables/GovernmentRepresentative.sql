CREATE TABLE [calendar].[GovernmentRepresentative] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)    NULL,
    [Description] NVARCHAR (84)    NULL,
    [IsActive]    BIT              CONSTRAINT [DF_GovernmentRepresentative_IsActive] DEFAULT ((1)) NULL,
    [SortOrder]   INT              NULL,
    [TimeStamp]   ROWVERSION       NULL,
    [RowGuid]     UNIQUEIDENTIFIER CONSTRAINT [DF_GovernmentRepresentative_RowGuid] DEFAULT (newid()) NULL,
    CONSTRAINT [PK_GovernmentRepresentative] PRIMARY KEY CLUSTERED ([Id] ASC)
);








GO

