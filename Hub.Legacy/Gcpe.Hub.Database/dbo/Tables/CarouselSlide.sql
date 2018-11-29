CREATE TABLE [dbo].[CarouselSlide] (
    [CarouselId] UNIQUEIDENTIFIER NOT NULL,
    [SlideId]    UNIQUEIDENTIFIER NOT NULL,
    [SortIndex]  INT              NOT NULL,
    CONSTRAINT [PK_CarouselSlide] PRIMARY KEY CLUSTERED ([CarouselId] ASC, [SlideId] ASC),
    CONSTRAINT [FK_CarouselSlide_Carousel] FOREIGN KEY ([CarouselId]) REFERENCES [dbo].[Carousel] ([Id]),
    CONSTRAINT [FK_CarouselSlide_Slide] FOREIGN KEY ([SlideId]) REFERENCES [dbo].[Slide] ([Id])
);








GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_CarouselSlide_SortIndex]
    ON [dbo].[CarouselSlide]([CarouselId] ASC, [SlideId] ASC, [SortIndex] ASC);

