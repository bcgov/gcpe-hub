CREATE TABLE [calendar].[NROrigin] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50)    NULL,
    [IsActive]  BIT              CONSTRAINT [DF_NROrigin_IsActive] DEFAULT ((1)) NOT NULL,
    [SortOrder] INT              NULL,
    [TimeStamp] ROWVERSION       NOT NULL,
    [RowGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_NROrigin_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_NROrigin] PRIMARY KEY CLUSTERED ([Id] ASC)
);









