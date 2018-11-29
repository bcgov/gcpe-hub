CREATE TABLE [media].[Ethnicity] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [EthnicityName] VARCHAR (250)    NOT NULL,
    [CreationDate]  DATETIME         NOT NULL,
    [ModifiedDate]  DATETIME         NOT NULL,
    [SortOrder]     INT              CONSTRAINT [DF_Ethnicity_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_EthnicityLanguage] PRIMARY KEY CLUSTERED ([Id] ASC)
);

