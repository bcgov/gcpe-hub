CREATE TABLE [calendar].[ActivityKeywords] (
    [ActivityId]          INT      NOT NULL,
    [KeywordId]           INT      NOT NULL,
    [IsActive]            BIT      CONSTRAINT [DF_ActivityKeywords_IsActive] DEFAULT ((1)) NOT NULL,
    [LastUpdatedDateTime] DATETIME CONSTRAINT [DF_ActivityKeywords_LastUpdatedDateTime] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]       INT      NOT NULL,
    CONSTRAINT [PK_ActivityKeyword] PRIMARY KEY CLUSTERED ([ActivityId] ASC, [KeywordId] ASC),
    CONSTRAINT [FK_ActivityKeywords_Keyword] FOREIGN KEY ([KeywordId]) REFERENCES [calendar].[Keyword] ([Id]),
    CONSTRAINT [FK_ActivityKeywords_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id])
);

