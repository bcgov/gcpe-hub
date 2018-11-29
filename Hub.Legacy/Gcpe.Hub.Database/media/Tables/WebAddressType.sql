CREATE TABLE [media].[WebAddressType] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [WebAddressTypeName] VARCHAR (50)     NOT NULL,
    [SortOrder]          INT              CONSTRAINT [DF_WebAddressType_SortOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_WebAddressType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

