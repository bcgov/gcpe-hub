CREATE TABLE [calendar].[EventPlanner] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100)   NULL,
    [PhoneNumber] NVARCHAR (50)    NULL,
    [JobTitle]    NVARCHAR (150)   NULL,
    [SortOrder]   INT              NULL,
    [IsActive]    BIT              CONSTRAINT [DF_EventPlanner_IsActive] DEFAULT ((1)) NULL,
    [TimeStamp]   ROWVERSION       NULL,
    [RowGuid]     UNIQUEIDENTIFIER CONSTRAINT [DF_EventPlanner_RowGuid] DEFAULT (newid()) NULL,
    CONSTRAINT [PK_EventPlanner] PRIMARY KEY CLUSTERED ([Id] ASC)
);












GO

