CREATE TABLE [media].[Contact] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [FirstName]               NVARCHAR (150)   NOT NULL,
    [LastName]                NVARCHAR (150)   NOT NULL,
    [MinistryId]              UNIQUEIDENTIFIER NULL,
    [MinisterialJobTitleId]   UNIQUEIDENTIFIER NULL,
    [MLAAssignmentId]         UNIQUEIDENTIFIER NULL,
    [IsPressGallery]          BIT              NOT NULL,
    [HasMinisterAssignment]   BIT              NOT NULL,
    [IsPrimaryMediaContact]   BIT              NOT NULL,
    [IsSecondaryMediaContact] BIT              NOT NULL,
    [IsActive]                BIT              CONSTRAINT [DF_Contact_IsActive] DEFAULT ((1)) NOT NULL,
    [RecordEditedBy]          NVARCHAR (50)    NULL,
    [CreationDate]            DATETIME         NOT NULL,
    [ModifiedDate]            DATETIME         NOT NULL,
    [ShowNotes]               NVARCHAR (100)   NULL,
    CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Contact_ElectoralDistrict] FOREIGN KEY ([MLAAssignmentId]) REFERENCES [media].[ElectoralDistrict] ([Id]),
    CONSTRAINT [FK_Contact_MinisterialJobTitle] FOREIGN KEY ([MinisterialJobTitleId]) REFERENCES [media].[MinisterialJobTitle] ([Id]),
    CONSTRAINT [FK_Contact_Ministry] FOREIGN KEY ([MinistryId]) REFERENCES [dbo].[Ministry] ([Id])
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [FK_Contact_UniqueActiveName]
    ON [media].[Contact]([FirstName] ASC, [LastName] ASC) WHERE ([IsActive]=(1));

