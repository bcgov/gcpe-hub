CREATE TABLE [media].[PhoneType] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PhoneTypeName] VARCHAR (50)     NOT NULL,
    [SortOrder]     INT              CONSTRAINT [DF_PhoneType_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PhoneType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

