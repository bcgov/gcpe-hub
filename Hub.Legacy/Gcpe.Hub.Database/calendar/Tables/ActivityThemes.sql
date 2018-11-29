CREATE TABLE [calendar].[ActivityThemes]
(
	[ActivityId] INT NOT NULL , 
    [ThemeId] UNIQUEIDENTIFIER NOT NULL, 
	[IsActive]            BIT              CONSTRAINT [DF_ActivityThemes_IsActive] DEFAULT 1 NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_ActivityThemes_CreatedDateTime] DEFAULT GETDATE() NOT NULL,
    [CreatedBy]           INT              NOT NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_ActivityThemes_LastUpdatedDateTime] DEFAULT GETDATE() NOT NULL,
    [LastUpdatedBy]       INT              NOT NULL,

    CONSTRAINT [PK_ActivityTheme] PRIMARY KEY ([ActivityId], [ThemeId]), 
    CONSTRAINT [FK_ActivityThemes_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity]([Id]), 
    CONSTRAINT [FK_ActivityThemes_Theme] FOREIGN KEY ([ThemeId]) REFERENCES [dbo].[Theme]([Id])
)