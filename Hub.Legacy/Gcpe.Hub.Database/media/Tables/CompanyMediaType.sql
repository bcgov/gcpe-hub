CREATE TABLE [media].[CompanyMediaType] (
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [MediaTypeId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanyMediaType] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [MediaTypeId] ASC),
    CONSTRAINT [FK_CompanyMediaType_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyMediaType_MediaType] FOREIGN KEY ([MediaTypeId]) REFERENCES [media].[MediaType] ([Id])
);

