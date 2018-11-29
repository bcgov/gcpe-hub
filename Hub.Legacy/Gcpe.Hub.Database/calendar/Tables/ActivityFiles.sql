CREATE TABLE [calendar].[ActivityFiles] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [ActivityId]          INT              NOT NULL,
    [Data]				  VARBINARY(MAX)   NOT NULL,
    [FileName]            VARCHAR(MAX)     NOT NULL,
    [FileType]			  VARCHAR(MAX)     NOT NULL,
    [FileLength]          INT              NOT NULL,
	[Md5]				  VARCHAR(32)      NOT NULL,
    [LastUpdatedDateTime] DATETIME         CONSTRAINT [DF_ActivityFiles_LastUpdatedDateTime] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]       INT              NULL    
    CONSTRAINT [PK_ActivityFiless] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActivityFiles_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [calendar].[Activity] ([Id])    
);