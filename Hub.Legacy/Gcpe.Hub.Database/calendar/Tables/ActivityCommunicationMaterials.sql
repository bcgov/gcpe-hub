CREATE TABLE [calendar].[ActivityCommunicationMaterials] (
    [Id]                      INT              IDENTITY (1, 1) NOT NULL,
    [ActivityId]              INT              NOT NULL,
    [CommunicationMaterialId] INT              NOT NULL,
    [IsActive]                BIT              CONSTRAINT [DF_ActivityCommunicationMaterials_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDateTime]         DATETIME         CONSTRAINT [DF_ActivityCommunicationMaterials_CreatedDateTime] DEFAULT (getdate()) NULL,
    [CreatedBy]               INT              NULL,
    [LastUpdatedDateTime]     DATETIME         CONSTRAINT [DF_ActivityCommunicationMaterials_LastUpdatedDateTime] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]           INT              NULL,
    [TimeStamp]               ROWVERSION       NOT NULL,
    [RowGuid]                 UNIQUEIDENTIFIER CONSTRAINT [DF_ActivityCommunicationMaterials_RowGuid] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_ActivityCommunicationMaterials] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActivityCommunicationMaterials_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id]),
    CONSTRAINT [FK_ActivityCommunicationMaterials_CommunicationMaterial] FOREIGN KEY ([CommunicationMaterialId]) REFERENCES [calendar].[CommunicationMaterial] ([Id]),
    CONSTRAINT [FK_ActivityCommunicationMaterials_SystemUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [calendar].[SystemUser] ([Id]),
    CONSTRAINT [FK_ActivityCommunicationMaterials_SystemUser_UpdatedBy] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [calendar].[SystemUser] ([Id])
);








GO
CREATE NONCLUSTERED INDEX [IX_ActivityId]
    ON [calendar].[ActivityCommunicationMaterials]([ActivityId] ASC);

