CREATE TABLE [calendar].[CommunicationMaterial] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (100)   NULL,
    [IsActive]  BIT              CONSTRAINT [DF_CommunicationMaterials_IsActive] DEFAULT ((1)) NULL,
    [SortOrder] INT              NULL,
    [TimeStamp] ROWVERSION       NOT NULL,
    [RowGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_CommunicationMaterials_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_CommunicationMaterials] PRIMARY KEY CLUSTERED ([Id] ASC)
);










GO

