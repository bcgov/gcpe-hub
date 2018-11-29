CREATE TABLE [media].[ProvState] (
    [Id]   UNIQUEIDENTIFIER NOT NULL,
    [ProvStateName]   VARCHAR (150)    NOT NULL,
    [ProvStateAbbrev] VARCHAR (15)     NOT NULL,
    [CreationDate]    DATETIME         NOT NULL,
    [ModifiedDate]    DATETIME         NOT NULL,
    [SortOrder]       INT              CONSTRAINT [DF_ProvState_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ProvState] PRIMARY KEY CLUSTERED ([Id] ASC)
);

