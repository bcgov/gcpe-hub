CREATE TABLE [calendar].[Priority] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50)    NULL,
    [IsActive]  BIT              CONSTRAINT [DF_Priority_IsActive] DEFAULT ((1)) NOT NULL,
    [TimeStamp] ROWVERSION       NOT NULL,
    [RowGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_Priority_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_Priority] PRIMARY KEY CLUSTERED ([Id] ASC)
);









