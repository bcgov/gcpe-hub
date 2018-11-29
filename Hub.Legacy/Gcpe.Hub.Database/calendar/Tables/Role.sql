CREATE TABLE [calendar].[Role] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)    NULL,
    [Description] NVARCHAR (150)   NULL,
    [IsActive]    BIT              CONSTRAINT [DF_RoleTemp_IsActive] DEFAULT ((1)) NOT NULL,
    [TimeStamp]   ROWVERSION       NOT NULL,
    [RowGuid]     UNIQUEIDENTIFIER CONSTRAINT [DF_RoleTemp_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_RoleTemp] PRIMARY KEY CLUSTERED ([Id] ASC)
);







