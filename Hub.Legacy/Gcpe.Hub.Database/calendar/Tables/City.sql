CREATE TABLE [calendar].[City] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (255)    NULL,
    [IsActive]  BIT              CONSTRAINT [DF_Location_IsActive] DEFAULT ((1)) NULL,
    [TimeStamp] ROWVERSION       NULL,
    [RowGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_Location_RowGuid] DEFAULT (newid()) NULL,
    [SortOrder] INT              CONSTRAINT [DF_Location_SortOrder] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED ([Id] ASC)
);












GO

