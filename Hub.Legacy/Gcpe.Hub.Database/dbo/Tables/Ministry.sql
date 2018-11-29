CREATE TABLE [dbo].[Ministry] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [Key]               NVARCHAR (100)    NOT NULL,
    [SortOrder]         INT              CONSTRAINT [DF_Ministry_SortOrder] DEFAULT ((0)) NOT NULL,
    [DisplayName]       NVARCHAR (255)    NOT NULL,
    [Abbreviation]      NVARCHAR (10)     NOT NULL,
    [IsActive]          BIT              NOT NULL,
    [MinisterEmail]     NVARCHAR (255)    NOT NULL,
    [MinisterPhotoUrl]  NVARCHAR (255)    NULL,
    [MinisterPageHtml]  NVARCHAR (MAX)   NOT NULL,
    [Timestamp]         DATETIME         NOT NULL,
    [MiscHtml]          NVARCHAR (MAX)   NOT NULL,
    [MiscRightHtml]     NVARCHAR (MAX)   NOT NULL,
    [TwitterUsername]   NVARCHAR (255)    NOT NULL,
    [FlickrUrl]         NVARCHAR (255)    NOT NULL,
    [YoutubeUrl]        NVARCHAR (255)    NOT NULL,
    [AudioUrl]          NVARCHAR (255)    NOT NULL,
    [FacebookEmbedHtml] NVARCHAR (MAX)   NOT NULL,
    [YoutubeEmbedHtml]  NVARCHAR (MAX)   NOT NULL,
    [AudioEmbedHtml]    NVARCHAR (MAX)   NOT NULL,
    [TopReleaseId]      UNIQUEIDENTIFIER NULL,
    [FeatureReleaseId]  UNIQUEIDENTIFIER NULL,
    [MinisterAddress]   NVARCHAR (255)   NOT NULL,
    [MinisterName]      NVARCHAR (255)   NOT NULL,
    [MinisterSummary]   NVARCHAR (MAX)   NOT NULL,
    [MinistryUrl] NVARCHAR(255) NULL, 
    [ParentId] UNIQUEIDENTIFIER NULL, 
    [ContactUserId] INT NULL, 
    [SecondContactUserId] INT NULL,
    [WeekendContactNumber] VARCHAR(20) NULL, 
    [EodFinalizedDateTime] DATETIMEOFFSET NULL, 
    [EodLastRunUserId] INT NULL, 
    [EodLastRunDateTime] DATETIMEOFFSET NULL, 
    [DisplayAdditionalName] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_Ministry] PRIMARY KEY NONCLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Ministry_FeatureRelease] FOREIGN KEY ([FeatureReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]),
    CONSTRAINT [FK_Ministry_TopRelease] FOREIGN KEY ([TopReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]),
    CONSTRAINT [UK_Ministry_Abbreviation] UNIQUE NONCLUSTERED ([Abbreviation] ASC),
    CONSTRAINT [UK_MinistryLanguage_DisplayName] UNIQUE NONCLUSTERED ([DisplayName] ASC),
    CONSTRAINT [FK_Ministry_Parent] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Ministry] ([Id]),
    CONSTRAINT [FK_Ministry_ContactSystemUser] FOREIGN KEY ([ContactUserId]) REFERENCES [calendar].[SystemUser] ([Id]), 
    CONSTRAINT [FK_Ministry_SecondContactSystemUser] FOREIGN KEY ([SecondContactUserId]) REFERENCES [calendar].[SystemUser]([Id]), 
    CONSTRAINT [FK_Ministry_EodLastRunSystemUser] FOREIGN KEY ([EodLastRunUserId]) REFERENCES [calendar].[SystemUser]([Id])

);










GO


CREATE UNIQUE CLUSTERED INDEX [IX_Ministry_Key] ON [dbo].[Ministry] ([Key])
