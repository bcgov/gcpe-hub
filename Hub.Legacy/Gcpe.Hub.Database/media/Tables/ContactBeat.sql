CREATE TABLE [media].[ContactBeat] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [ContactId]     UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]     UNIQUEIDENTIFIER NOT NULL,
    [BeatId]        UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ContactBeat_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ContactBeat_Beat] FOREIGN KEY ([BeatId]) REFERENCES [media].[Beat] ([Id]),
    CONSTRAINT [FK_ContactBeat_Company] FOREIGN KEY ([CompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_ContactBeat_ContactBeat] FOREIGN KEY ([ContactId]) REFERENCES [media].[Contact] ([Id])
);

