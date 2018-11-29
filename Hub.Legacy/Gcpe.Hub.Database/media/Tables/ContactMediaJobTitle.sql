CREATE TABLE [media].[ContactMediaJobTitle] (
    [Id] UNIQUEIDENTIFIER CONSTRAINT [DF_ContactMediaJobTitle_ContactMediaJobTitleId] DEFAULT (newid()) NOT NULL,
    [ContactId]              UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]              UNIQUEIDENTIFIER NOT NULL,
    [MediaJobTitleId]        UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ContactMediaJobTitle_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ContactMediaJobTitle_1JobPerOutlet] UNIQUE NONCLUSTERED ([ContactId], [CompanyId]),
    CONSTRAINT [FK_ContactMediaJobTitle_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_ContactMediaJobTitle_Contact] FOREIGN KEY ([ContactId]) REFERENCES [media].[Contact] ([Id]),
    CONSTRAINT [FK_ContactMediaJobTitle_MediaJobTitle] FOREIGN KEY ([MediaJobTitleId]) REFERENCES [media].[MediaJobTitle] ([Id])
);

