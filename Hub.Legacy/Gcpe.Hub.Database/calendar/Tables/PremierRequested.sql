CREATE TABLE [calendar].[PremierRequested] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50)    NULL,
    [IsActive]  BIT              CONSTRAINT [DF_PremierRequested_IsActive] DEFAULT ((1)) NOT NULL,
    [SortOrder] INT              NULL,
    [TimeStamp] ROWVERSION       NOT NULL,
    [RowGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_PremierRequested_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_PremierRequested] PRIMARY KEY CLUSTERED ([Id] ASC)
);









