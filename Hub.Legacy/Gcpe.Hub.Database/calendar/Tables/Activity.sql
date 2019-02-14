CREATE TABLE [calendar].[Activity] (
    [Id]                          INT              IDENTITY (1, 1) NOT NULL,
    [StartDateTime]               DATETIME         NULL,
    [EndDateTime]                 DATETIME         NULL,
    [PotentialDates]              NVARCHAR (70)    NULL,
    [Title]                       NVARCHAR (500)   CONSTRAINT [DF_Activity_Title] DEFAULT ('') NOT NULL,
    [Details]                     NVARCHAR (500)   CONSTRAINT [DF_Activity_Details] DEFAULT ('') NOT NULL,
    [Schedule]                    NVARCHAR (500)   CONSTRAINT [DF_Activity_Schedule] DEFAULT ('') NOT NULL,
    [Significance]                NVARCHAR (500)   CONSTRAINT [DF_Activity_Significance] DEFAULT ('') NOT NULL,
    [Strategy]                    NVARCHAR (500)   NULL,
    [Comments]                    NVARCHAR (4000)  NULL,
    [HqComments]                  NVARCHAR (2000)   NULL,
    [LeadOrganization]            NVARCHAR (100)   NULL,
    [Venue]                       NVARCHAR (150)   NULL,
    [Translations]                NVARCHAR (150)   NULL,
    [StatusId]                    INT              NULL,
    [HqStatusId]                  INT              NULL,
    [HqSection]                   INT              CONSTRAINT [DF_Activity_HqSection] DEFAULT ((2)) NOT NULL,
    [NRDistributionId]            INT              NULL,
    [PremierRequestedId]          INT              NULL,
    [ContactMinistryId]           UNIQUEIDENTIFIER NULL,
    [GovernmentRepresentativeId]  INT              NULL,
    [CommunicationContactId]      INT              NULL,
    [EventPlannerId]              INT              NULL,
    [VideographerId]              INT              NULL,
    [CityId]                      INT              NULL,
    [OtherCity]                   NVARCHAR (150)   NULL,
    [IsActive]                    BIT              CONSTRAINT [DF_Activity_IsActive] DEFAULT ((0)) NOT NULL,
    [IsConfirmed]                 BIT              CONSTRAINT [DF_Activity_IsConfirmed] DEFAULT ((0)) NOT NULL,
    [IsIssue]                     BIT              CONSTRAINT [DF_Activity_IsIssue] DEFAULT ((0)) NOT NULL,
    [IsAllDay]                    BIT              CONSTRAINT [DF_Activity_IsAllDay] DEFAULT ((0)) NOT NULL,
    [IsAtLegislature]             BIT              CONSTRAINT [DF_Activity_IsAtLegislature] DEFAULT ((0)) NOT NULL,
    [IsConfidential]              BIT              CONSTRAINT [DF_Activity_IsConfidential] DEFAULT ((0)) NOT NULL,
    [IsCrossGovernment]           BIT              CONSTRAINT [DF_Activity_IsCrossGovernment] DEFAULT ((0)) NOT NULL,
    [IsMilestone]                 BIT              CONSTRAINT [DF_Activity_IsMilestone] DEFAULT ((0)) NOT NULL,
    [IsTitleNeedsReview]          BIT              CONSTRAINT [DF_Activity_IsTitleNeedsReview] DEFAULT ((0)) NOT NULL,
    [IsDetailsNeedsReview]        BIT              CONSTRAINT [DF_Activity_IsDetailsNeedsReview] DEFAULT ((0)) NOT NULL,
    [IsRepresentativeNeedsReview] BIT              CONSTRAINT [DF_Activity_IsRepresentativeNeedsReview] DEFAULT ((0)) NOT NULL,
    [IsCityNeedsReview]           BIT              CONSTRAINT [DF_Activity_IsCityNeedsReview] DEFAULT ((0)) NOT NULL,
    [IsStartDateNeedsReview]      BIT              CONSTRAINT [DF_Activity_IsStartDateNeedsReview] DEFAULT ((0)) NOT NULL,
    [IsEndDateNeedsReview]        BIT              CONSTRAINT [DF_Activity_IsEndDateNeedsReview] DEFAULT ((0)) NOT NULL,
    [IsCategoriesNeedsReview]     BIT              CONSTRAINT [DF_Activity_IsCategoriesNeedsReview] DEFAULT ((0)) NOT NULL,
    [IsCommMaterialsNeedsReview]  BIT              CONSTRAINT [DF_Activity_IsCommMaterialsNeedsReview] DEFAULT ((0)) NOT NULL,
    [IsActiveNeedsReview]         BIT              CONSTRAINT [DF_Activity_IsStatusNeedsReview] DEFAULT ((0)) NOT NULL,
    [NRDateTime]                  DATETIME         NULL,
    [CreatedDateTime]             DATETIME         CONSTRAINT [DF_Activity_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]                   INT              NULL,
    [LastUpdatedDateTime]         DATETIME         CONSTRAINT [DF_Activity_LastUpdatedDateTime] DEFAULT (getdate()) NULL,
    [LastUpdatedBy]               INT              NULL,
    [TimeStamp]                   ROWVERSION       NULL,
    [RowGuid]                     UNIQUEIDENTIFIER CONSTRAINT [DF_Activity_RowGuid] DEFAULT (newid()) NULL,
    CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Activity_City] FOREIGN KEY ([CityId]) REFERENCES [calendar].[City] ([Id]),
    CONSTRAINT [FK_Activity_CommunicationContact] FOREIGN KEY ([CommunicationContactId]) REFERENCES [calendar].[CommunicationContact] ([Id]),
    CONSTRAINT [FK_Activity_EventPlanner] FOREIGN KEY ([EventPlannerId]) REFERENCES [calendar].[EventPlanner] ([Id]),
    CONSTRAINT [FK_Activity_GovernmentRepresentative] FOREIGN KEY ([GovernmentRepresentativeId]) REFERENCES [calendar].[GovernmentRepresentative] ([Id]),
    CONSTRAINT [FK_Activity_HqStatus] FOREIGN KEY ([HqStatusId]) REFERENCES [calendar].[Status] ([Id]),
    CONSTRAINT [FK_Activity_Ministry] FOREIGN KEY ([ContactMinistryId]) REFERENCES [dbo].[Ministry] ([Id]),
    CONSTRAINT [FK_Activity_NRDistribution] FOREIGN KEY ([NRDistributionId]) REFERENCES [calendar].[NRDistribution] ([Id]),
    CONSTRAINT [FK_Activity_PremierRequested] FOREIGN KEY ([PremierRequestedId]) REFERENCES [calendar].[PremierRequested] ([Id]),
    CONSTRAINT [FK_Activity_Status] FOREIGN KEY ([StatusId]) REFERENCES [calendar].[Status] ([Id]),
    CONSTRAINT [FK_Activity_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_Activity_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_Activity_Videographer] FOREIGN KEY ([VideographerId]) REFERENCES [calendar].[Videographer] ([Id])
);






















GO
CREATE NONCLUSTERED INDEX [IX_StartDateTime]
    ON [calendar].[Activity]([StartDateTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EndDateTime]
    ON [calendar].[Activity]([EndDateTime] ASC);

