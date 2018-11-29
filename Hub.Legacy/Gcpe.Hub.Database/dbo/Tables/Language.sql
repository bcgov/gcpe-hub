CREATE TABLE [dbo].[Language] (
    [Id]        INT          NOT NULL,
    [SortOrder] INT          CONSTRAINT [DF_Language_SortOrder] DEFAULT ((0)) NOT NULL,
    [Name]      VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_Language_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);






GO


