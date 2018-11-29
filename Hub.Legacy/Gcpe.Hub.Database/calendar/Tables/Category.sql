CREATE TABLE [calendar].[Category] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50)    NULL,
    [SortOrder] INT              NULL,
    [IsActive]  BIT              CONSTRAINT [DF_Category_IsActive] DEFAULT ((1)) NOT NULL,
    [TimeStamp] ROWVERSION       NOT NULL,
    [RowGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_Category_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([Id] ASC)
);










GO

