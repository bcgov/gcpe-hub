CREATE TABLE [media].[MediaRequestSharedMinistry] (
    [MediaRequestId]     UNIQUEIDENTIFIER NOT NULL,
    [MinistryId]     UNIQUEIDENTIFIER NOT NULL
    PRIMARY KEY CLUSTERED ([MediaRequestId], [MinistryId]), 
    CONSTRAINT [FK_MediaRequestSharedMinistry_MediaRequest] FOREIGN KEY ([MediaRequestId]) REFERENCES [media].[MediaRequest] ([Id]),
    CONSTRAINT [FK_MediaRequestSharedMinistry_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id])
);

