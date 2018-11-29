CREATE TABLE [dbo].[Carousel] (
    [Id]              UNIQUEIDENTIFIER   NOT NULL,
    [PublishDateTime] DATETIMEOFFSET (7) NULL,
    [Timestamp]       DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Carousel] PRIMARY KEY CLUSTERED ([Id] ASC)
);

