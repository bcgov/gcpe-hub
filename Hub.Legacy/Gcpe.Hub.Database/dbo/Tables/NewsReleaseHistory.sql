CREATE TABLE [dbo].[NewsReleaseHistory] (
    [ReleaseId]       UNIQUEIDENTIFIER   NOT NULL,
    [PublishDateTime] DATETIMEOFFSET (7) NOT NULL,
    [MimeType]        VARCHAR (100)      NOT NULL,
    [BlobId]          UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_NewsReleaseHistory] PRIMARY KEY CLUSTERED ([ReleaseId] ASC, [PublishDateTime] ASC, [MimeType] ASC),
    CONSTRAINT [FK_NewsReleaseHistory_Blob] FOREIGN KEY ([BlobId]) REFERENCES [dbo].[Blob] ([Id]),
    CONSTRAINT [FK_NewsReleaseHistory_NewsRelease] FOREIGN KEY ([ReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]) ON DELETE CASCADE
);





