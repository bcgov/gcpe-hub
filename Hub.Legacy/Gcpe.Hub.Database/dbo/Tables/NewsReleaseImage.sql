CREATE TABLE [dbo].[NewsReleaseImage] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [SortOrder] INT              NOT NULL,
    [Name]      VARCHAR (50)     NOT NULL,
    [MimeType]  VARCHAR (100)    NOT NULL,
    [BlobId]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_NewsReleaseImage] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NewsReleaseImage_Blob] FOREIGN KEY ([BlobId]) REFERENCES [dbo].[Blob] ([Id]),
    CONSTRAINT [UK_NewsReleaseImage_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);












GO


