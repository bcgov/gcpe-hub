CREATE TABLE [dashboard].[UserMinistryPreference]
(
    [Email] NVARCHAR(50) NOT NULL , 
    [MinistryId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [FK_UserMinistryPreference_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id]), 
    PRIMARY KEY ([MinistryId], [Email])
)
