CREATE TABLE [calendar].[Videographer] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (100)   NULL,
    [JobTitle]  NVARCHAR (150)   NULL,
    [SortOrder] INT              NULL,
    [IsActive]  BIT              CONSTRAINT [DF_Videographer_IsActive] DEFAULT ((1)) NULL,
    [TimeStamp] ROWVERSION       NULL,
    [RowGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_Videographer_RowGuid] DEFAULT (newid()) NULL,
    CONSTRAINT [PK_Videographer] PRIMARY KEY CLUSTERED ([Id] ASC)
);









