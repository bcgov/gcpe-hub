CREATE TABLE [dbo].[NewsReleaseMediaDistribution] (
    [ReleaseId]               UNIQUEIDENTIFIER NOT NULL,
    [MediaDistributionListId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_NewsReleaseMediaDistribution] PRIMARY KEY CLUSTERED ([ReleaseId] ASC, [MediaDistributionListId] ASC),
    CONSTRAINT [FK_NewsReleaseMediaDistribution_NewsRelease] FOREIGN KEY ([ReleaseId]) REFERENCES [dbo].[NewsRelease] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_NewsReleaseMediaDistribution_List] FOREIGN KEY ([MediaDistributionListId]) REFERENCES [dbo].[MediaDistributionList] ([Id])
);

