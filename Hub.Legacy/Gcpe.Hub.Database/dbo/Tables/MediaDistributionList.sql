CREATE TABLE [dbo].[MediaDistributionList] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_MediaDistributionList_Id] DEFAULT (newid()) NOT NULL,
    [Key]         NVARCHAR (50)    NOT NULL,
    [DisplayName] NVARCHAR (50)    NOT NULL,
    [SortOrder]   INT              CONSTRAINT [DF_MediaDistributionList_SortOrder] DEFAULT ((0)) NOT NULL,
    [IsActive]    BIT              NOT NULL,
    CONSTRAINT [PK_MediaDistributionList] PRIMARY KEY CLUSTERED ([Id] ASC)
);

