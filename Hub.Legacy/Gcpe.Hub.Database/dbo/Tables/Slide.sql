CREATE TABLE [dbo].[Slide] (
    [Id]              UNIQUEIDENTIFIER   NOT NULL,
    [Headline]        NVARCHAR (255)     NOT NULL,
    [Summary]         NVARCHAR (MAX)     NOT NULL,
    [ActionUrl]       NVARCHAR (255)     NOT NULL,
    [Image]           IMAGE              NOT NULL,
    [FacebookPostUrl] NVARCHAR (255)     NULL,
    [Justify]         INT                NOT NULL,
    [Timestamp]       DATETIMEOFFSET (7) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

