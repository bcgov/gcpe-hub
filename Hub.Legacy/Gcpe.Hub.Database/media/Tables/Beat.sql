CREATE TABLE [media].[Beat] (
    [Id]     UNIQUEIDENTIFIER NOT NULL,
    [BeatName]     NVARCHAR (255)   NOT NULL,
    [CreationDate] DATETIME         NOT NULL,
    [ModifiedDate] DATETIME         NOT NULL,
    [SortOrder]    INT              CONSTRAINT [DF_Beat_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Beat] PRIMARY KEY CLUSTERED ([Id] ASC)
);

