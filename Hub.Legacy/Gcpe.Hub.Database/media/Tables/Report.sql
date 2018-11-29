CREATE TABLE [media].[Report] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [ReportName]        VARCHAR (150)    NOT NULL,
    [ReportOwner]       VARCHAR (150)    NOT NULL,
    [ReportQueryString] VARCHAR (MAX)    NOT NULL,
    [IsPublic]          BIT              CONSTRAINT [DF_Report_IsPublic] DEFAULT ((0)) NOT NULL,
    [SortOrder]         INT              CONSTRAINT [DF_Report_SortOrder] DEFAULT ((0)) NOT NULL,
    [CreationDate]      DATETIME         NOT NULL,
    CONSTRAINT [PK_Report] PRIMARY KEY CLUSTERED ([Id] ASC)
);

