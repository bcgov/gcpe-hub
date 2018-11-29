CREATE TABLE [dbo].[Sector] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_Sector_Id] DEFAULT (newid()) NOT NULL,
	[Key]			  VARCHAR (100)	 NOT NULL ,
    [SortOrder] INT              CONSTRAINT [DF_Sector_SortOrder] DEFAULT ((0)) NOT NULL,
    [IsActive]  BIT              NOT NULL,
    [DisplayName] VARCHAR(255) NULL, 
    [Timestamp] DATETIME   NOT NULL, 
    [MiscHtml] NVARCHAR(MAX)   NOT NULL, 
    [MiscRightHtml] NVARCHAR(MAX)   NOT NULL, 
    [TwitterUsername] VARCHAR(255)   NOT NULL, 
    [FlickrUrl] VARCHAR(255)   NOT NULL, 
    [YoutubeUrl] VARCHAR(255)   NOT NULL, 
    [AudioUrl] VARCHAR(255)   NOT NULL, 
    [FacebookEmbedHtml] NVARCHAR(MAX)   NOT NULL, 
    [YoutubeEmbedHtml] NVARCHAR(MAX)   NOT NULL, 
    [AudioEmbedHtml] NVARCHAR(MAX)   NOT NULL,
	[TopReleaseId]  UNIQUEIDENTIFIER NULL,
	[FeatureReleaseId]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Sector] PRIMARY KEY NONCLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Sector_TopRelease] FOREIGN KEY ([TopReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]),
	CONSTRAINT [FK_Sector_FeatureRelease] FOREIGN KEY ([FeatureReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]),
);


GO

CREATE UNIQUE CLUSTERED INDEX [IX_Sector_Key] ON [dbo].[Sector] ([Key])
