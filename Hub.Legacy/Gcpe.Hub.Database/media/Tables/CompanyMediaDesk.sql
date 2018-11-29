CREATE TABLE [media].[CompanyMediaDesk] (
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [MediaDeskId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompanyMediaDesk] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [MediaDeskId] ASC),
    CONSTRAINT [FK_CompanyMediaDesk_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_CompanyMediaDesk_MediaDesk] FOREIGN KEY ([MediaDeskId]) REFERENCES [media].[MediaDesk] ([Id])
);

