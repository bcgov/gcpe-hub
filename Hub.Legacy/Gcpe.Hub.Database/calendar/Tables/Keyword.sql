CREATE TABLE [calendar].[Keyword] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [Name]                VARCHAR (255) NOT NULL,
    [SortOrder]           INT           DEFAULT ((0)) NOT NULL,
    [IsActive]            BIT           CONSTRAINT [DF_Keyword_IsActive] DEFAULT ((1)) NOT NULL,
    [LastUpdatedDateTime] DATETIME      CONSTRAINT [DF_Keyword_LastUpdatedDateTime] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]       INT           NOT NULL,
    CONSTRAINT [PK_Keyword] PRIMARY KEY CLUSTERED ([Id] ASC)
);

