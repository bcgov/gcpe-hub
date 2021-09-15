CREATE TABLE [calendar].[ActivityTags]
(
	 [ActivityId] INT NOT NULL , 
    [TagId] UNIQUEIDENTIFIER NOT NULL, 
    [IsActive]            BIT              CONSTRAINT [DF_ActivityTags_IsActive] DEFAULT 1 NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_ActivityTags_CreatedDateTime] DEFAULT GETDATE() NOT NULL,
    [CreatedBy]           INT              NOT NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_ActivityTags_LastUpdatedDateTime] DEFAULT GETDATE() NOT NULL,
    [LastUpdatedBy]       INT              NOT NULL,

    CONSTRAINT [PK_ActivityTag] PRIMARY KEY ([ActivityId], [TagId]), 
    CONSTRAINT [FK_ActivityTags_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity]([Id]), 
    CONSTRAINT [FK_ActivityTags_Tag] FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tag]([Id])
)
