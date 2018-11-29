CREATE TABLE [media].[MediaRequestContact] (
    [MediaRequestId]     UNIQUEIDENTIFIER NOT NULL,
    [ContactId]     UNIQUEIDENTIFIER NOT NULL,
    [CompanyId] UNIQUEIDENTIFIER NOT NULL
    CONSTRAINT [PK_MediaRequestContact] PRIMARY KEY ([MediaRequestId], [ContactId]),
    CONSTRAINT [FK_MediaRequestContact_MediaRequest] FOREIGN KEY ([MediaRequestId]) REFERENCES [media].[MediaRequest] ([Id]),
    CONSTRAINT [FK_MediaRequestContact_Contact] FOREIGN KEY ([ContactId]) REFERENCES [media].[Contact] ([Id]),
    CONSTRAINT [FK_MediaRequestContact_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id])
);

