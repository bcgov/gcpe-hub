CREATE TABLE [calendar].[ActivityServices]
(
    [ActivityId] INT NOT NULL ,
    [ServiceId] UNIQUEIDENTIFIER NOT NULL,
    [IsActive]            BIT              CONSTRAINT [DF_ActivityServices_IsActive] DEFAULT 1 NOT NULL,
    [CreatedDateTime]     DATETIME         CONSTRAINT [DF_ActivityServices_CreatedDateTime] DEFAULT GETDATE() NOT NULL,
    [CreatedBy]           INT              NOT NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_ActivityServices_LastUpdatedDateTime] DEFAULT GETDATE() NOT NULL,
    [LastUpdatedBy]       INT              NOT NULL,

    CONSTRAINT [PK_ActivityService] PRIMARY KEY ([ActivityId], [ServiceId]), 
    CONSTRAINT [FK_ActivityServices_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity]([Id]), 
    CONSTRAINT [FK_ActivityServices_Service] FOREIGN KEY ([ServiceId]) REFERENCES [dbo].[Service]([Id])
)