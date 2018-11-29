CREATE TABLE [dbo].[Service] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Service_Id] DEFAULT (newid()) NOT NULL,
    [Key]         VARCHAR (255)    NOT NULL,
    [DisplayName] VARCHAR (255)    NULL,
    [SortOrder]   INT              CONSTRAINT [DF_Service_SortOrder] DEFAULT ((0)) NOT NULL,
    [IsActive]    BIT              NOT NULL,
    CONSTRAINT [PK_Service] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);

