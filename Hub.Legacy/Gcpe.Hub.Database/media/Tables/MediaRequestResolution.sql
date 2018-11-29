CREATE TABLE [dbo].[MediaRequestResolution]
(
	[Id] UNIQUEIDENTIFIER CONSTRAINT [DF_MediaRequestResolution_Id] DEFAULT (newid()) NOT NULL, 
    [DisplayAs] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_MediaRequestResolution] PRIMARY KEY ([Id]),
)