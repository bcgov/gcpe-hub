CREATE TABLE [dbo].[Theme] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_Theme_Id] DEFAULT (newid()) NOT NULL,	
	[Key] VARCHAR (255)	 NOT NULL ,
    [DisplayName] VARCHAR(255) NULL,
	[Timestamp] DATETIME   NOT NULL,
	[TopReleaseId]  UNIQUEIDENTIFIER NULL,
	[FeatureReleaseId]  UNIQUEIDENTIFIER NULL, 
    [SortOrder] INT CONSTRAINT [DF_Theme_SortOrder] DEFAULT (0) NOT NULL, 
    [IsActive] BIT  NOT NULL,
    CONSTRAINT [PK_Theme] PRIMARY KEY NONCLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Theme_TopRelease] FOREIGN KEY ([TopReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]),
	CONSTRAINT [FK_Theme_FeatureRelease] FOREIGN KEY ([FeatureReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]),
);