CREATE TABLE [dbo].[NewsReleaseCollection] (
    [Id]   UNIQUEIDENTIFIER NOT NULL,
    [Name] VARCHAR (50)     NOT NULL,
    CONSTRAINT [PK_NewsCollection] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_NewsCollection_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

