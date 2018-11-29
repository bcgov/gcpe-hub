CREATE TABLE [calendar].[FavoriteActivity] (
    [SystemUserId] INT NOT NULL,
    [ActivityId]   INT NOT NULL,
    CONSTRAINT [PK_FavoriteActivity] PRIMARY KEY CLUSTERED ([ActivityId] ASC, [SystemUserId] ASC),
    CONSTRAINT [FK_FavoriteActivity_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id]),
    CONSTRAINT [FK_FavoriteActivity_SystemUser] FOREIGN KEY ([SystemUserId]) REFERENCES [calendar].[SystemUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_FavoriteActivity]
    ON [calendar].[FavoriteActivity]([ActivityId] ASC);

