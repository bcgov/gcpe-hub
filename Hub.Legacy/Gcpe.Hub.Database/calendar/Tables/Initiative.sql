CREATE TABLE [calendar].[Initiative] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50)    NULL,
    [IsActive]  BIT              CONSTRAINT [DF_Initiative_IsActive] DEFAULT ((1)) NOT NULL,
    [SortOrder] INT              NULL,
    [TimeStamp] ROWVERSION       NOT NULL,
    [RowGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_Initiative_RowGuid] DEFAULT (newid()) NOT NULL,
    [ShortName] NVARCHAR (40)    NULL,
    CONSTRAINT [PK_Initiative] PRIMARY KEY CLUSTERED ([Id] ASC)
);



