CREATE TABLE [dashboard].[Message]
(
	[Id]					UNIQUEIDENTIFIER	NOT NULL,
	[Title]					VARCHAR (255)		NOT NULL,
	[Description]			NVARCHAR (1000)		NULL,
	[SortOrder]				INT					CONSTRAINT [DF_Message_SortOrder] DEFAULT ((0)) NOT NULL,
	[IsHighlighted]			BIT					CONSTRAINT [DF_Message_IsHighlighted] DEFAULT ((0)) NOT NULL,
	[IsPublished]			BIT					CONSTRAINT [DF_Message_IsPublished] DEFAULT ((0)) NOT NULL,
	[Timestamp]				DATETIME			CONSTRAINT [DF_Message_Timestamp] DEFAULT (getdate()) NOT NULL,
	PRIMARY KEY ([Id])
)
