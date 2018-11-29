CREATE TABLE [calendar].[Status] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50)    NULL,
    [IsActive]  BIT              CONSTRAINT [DF_Status_IsActive] DEFAULT ((1)) NOT NULL,
    [TimeStamp] ROWVERSION       NOT NULL,
    [RowGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_Status_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([Id] ASC)
);









