CREATE TABLE [calendar].[NRDistribution] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50)    NULL,
    [SortOrder] INT              CONSTRAINT [DF_NRDistribution_SortOrder] DEFAULT ((1)) NULL,
    [IsActive]  BIT              CONSTRAINT [DF_NRDistribution_IsActive] DEFAULT ((1)) NOT NULL,
    [TimeStamp] ROWVERSION       NOT NULL,
    [RowGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_NRDistribution_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_NRDistribution] PRIMARY KEY CLUSTERED ([Id] ASC)
);









