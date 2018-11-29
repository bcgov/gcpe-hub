CREATE TABLE [dbo].[User] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [DisplayName]  NVARCHAR (256)   CONSTRAINT [DF_User_DisplayName] DEFAULT ('') NOT NULL,
    [EmailAddress] NVARCHAR (256)   CONSTRAINT [DF_User_EmailAddress] DEFAULT ('') NOT NULL,
    [IsActive]     BIT              CONSTRAINT [DF_User_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_User_DisplayName] UNIQUE NONCLUSTERED ([DisplayName] ASC),
    CONSTRAINT [UK_User_EmailAddress] UNIQUE NONCLUSTERED ([EmailAddress] ASC)
);








GO



GO


