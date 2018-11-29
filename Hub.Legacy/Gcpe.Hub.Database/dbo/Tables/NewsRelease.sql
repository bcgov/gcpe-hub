CREATE TABLE [dbo].[NewsRelease] (
    [Id]              UNIQUEIDENTIFIER   NOT NULL,
    [Timestamp]       DATETIMEOFFSET (7) CONSTRAINT [DF_NewsRelease_Timestamp] DEFAULT ('1900-01-01') NOT NULL,
    [ReleaseType]     INT                NOT NULL,
    [Key]             VARCHAR (255)      NOT NULL,
    [CollectionId]    UNIQUEIDENTIFIER   NOT NULL,
    [Year]            INT                NULL,
    [YearRelease]     INT                NULL,
    [MinistryId]      UNIQUEIDENTIFIER   NULL,
    [MinistryRelease] INT                NULL,
    [Reference]       VARCHAR (50)       NOT NULL,
    [ActivityId]      INT                NULL,
    [ReleaseDateTime] DATETIME           NULL,
    [PublishDateTime] DATETIMEOFFSET (7) NULL,
    [IsCommitted]     BIT                NOT NULL,
    [IsPublished]     BIT                NOT NULL,
    [PublishOptions]  INT                NOT NULL,
    [IsActive]        BIT                NOT NULL,
    [HasMediaAssets]  BIT                NOT NULL,
    [NodSubscribers]  INT                NULL,
    [MediaSubscribers] INT               NULL,
    [AtomId]          VARCHAR (255)      NOT NULL,
    [Keywords]        NVARCHAR (MAX)     NOT NULL,
    [AssetUrl]        VARCHAR (MAX)      NOT NULL,
    [RedirectUrl]     VARCHAR (MAX)      DEFAULT ('') NOT NULL,
    CONSTRAINT [PK_Releases] PRIMARY KEY NONCLUSTERED ([Id] ASC),
    CONSTRAINT [CK_NewsRelease_CommittedPublishDateTime] CHECK ([IsCommitted]=(0) OR [IsCommitted]=(1) AND [PublishDateTime] IS NOT NULL),
    CONSTRAINT [CK_NewsRelease_ReleaseDateTime] CHECK ([IsPublished]=(0) OR [IsPublished]=(1) AND [ReleaseDateTime] IS NOT NULL),
    CONSTRAINT [CK_NewsRelease_ReleaseType] CHECK ([ReleaseType]<=(5) AND [ReleaseType]>=(1)),
    CONSTRAINT [FK_NewsRelease_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id]),
    CONSTRAINT [FK_NewsRelease_NewsCollection] FOREIGN KEY ([CollectionId]) REFERENCES [dbo].[NewsReleaseCollection] ([Id])
);
































GO
CREATE NONCLUSTERED INDEX [IX_NewsRelease_YearRelease]
    ON [dbo].[NewsRelease]([Year] ASC, [YearRelease] ASC) WHERE ([Year] IS NOT NULL);


GO

CREATE UNIQUE CLUSTERED INDEX [IX_NewsRelease_Key]
    ON [dbo].[NewsRelease]([ReleaseType] ASC, [Key] ASC);


GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_NewsRelease_Reference] 
	ON [dbo].[NewsRelease] ([Reference] ASC)
	WHERE [Reference] <> '';



