CREATE TABLE [calendar].[SystemUser] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [Username]            NVARCHAR (20)    NOT NULL,
    [RoleId]              INT              NOT NULL,
    [Description]         NVARCHAR (1000)    NULL,
    [FullName]            NVARCHAR (50)    NULL,
    [DisplayName]         NVARCHAR (50)    NULL,
    [JobTitle]            NVARCHAR (100)   NULL,
    [PhoneNumber]         NVARCHAR (20)    NULL,
    [MobileNumber]        NVARCHAR(15)     CONSTRAINT [DF_SystemUser_MobileNumber] CHECK (LEN(MobileNumber)=0 OR MobileNumber LIKE REPLICATE('[0-9-]', 12)),
    [EmailAddress]        NVARCHAR (50)    NULL,
    [FilterDisplayValue]  INT              NULL,
    [IsActive]            BIT              CONSTRAINT [DF_SystemUser_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_SystemUser_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]           INT              NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_SystemUser_LastUpdatedDateTime] DEFAULT (getdate()) NULL,
    [LastUpdatedBy]       INT              NULL,
    [TimeStamp]           ROWVERSION       NOT NULL,
    [RowGuid]             UNIQUEIDENTIFIER CONSTRAINT [DF_SystemUser_RowGuid] DEFAULT (newid()) NOT NULL,
    [HiddenColumns]       NVARCHAR (50)    NULL,
    CONSTRAINT [PK_SystemUser] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SystemUser_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_SystemUser_SystemUser_Role] FOREIGN KEY ([RoleId]) REFERENCES [calendar].[Role] ([Id]),
    CONSTRAINT [FK_SystemUser_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id])
);












GO

CREATE UNIQUE INDEX [UX_SystemUser_RowGuid] ON [calendar].[SystemUser] ([RowGuid])
