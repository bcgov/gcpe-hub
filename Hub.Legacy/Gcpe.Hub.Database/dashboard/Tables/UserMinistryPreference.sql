CREATE TABLE [dashboard].[UserMinistryPreference]
(
    [Email] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [MinistryId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [FK_UserMinistryPreference_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id])
)
