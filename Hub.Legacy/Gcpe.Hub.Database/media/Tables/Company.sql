CREATE TABLE [media].[Company] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [ParentCompanyId]        UNIQUEIDENTIFIER NULL,
    [CompanyName]            VARCHAR (250)    NOT NULL,
    [CompanyDescription]     VARCHAR (2500)   NOT NULL,
    [CirculationDescription] VARCHAR (500)    NULL,
    [Deadlines]              VARCHAR (500)    NULL,
    [KeyPrograms]            VARCHAR (500)    NULL,
    [PrintCategoryId]        UNIQUEIDENTIFIER NULL,
    [PublicationFrequencyId] UNIQUEIDENTIFIER NULL,
    [IsMajorMedia]           BIT              NULL,
    [IsEthnicMedia]          BIT              NULL,
    [IsLiveMedia]            BIT              NULL,
    [IsOutlet]               BIT              CONSTRAINT [DF_Company_IsOutlet] DEFAULT ((0)) NOT NULL,
    [IsActive]               BIT              CONSTRAINT [DF_Company_IsActive] DEFAULT ((1)) NOT NULL,
    [RecordEditedBy]         VARCHAR (50)     NULL,
    [CreationDate]           DATETIME         NOT NULL,
    [ModifiedDate]           DATETIME         NOT NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Company_Company] FOREIGN KEY ([ParentCompanyId]) REFERENCES [media].[Company] ([Id]),
    CONSTRAINT [FK_Company_PrintCategory] FOREIGN KEY ([PrintCategoryId]) REFERENCES [media].[PrintCategory] ([Id]),
    CONSTRAINT [FK_Company_PublicationFrequency] FOREIGN KEY ([PublicationFrequencyId]) REFERENCES [media].[PublicationFrequency] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [FK_Company_UniqueActiveOutletName]
    ON [media].[Company]([CompanyName] ASC) WHERE ([IsActive]=(1));

