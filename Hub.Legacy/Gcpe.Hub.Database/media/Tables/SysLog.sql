CREATE TABLE [media].[SysLog] (
    [LogID]      INT              IDENTITY (1, 1) NOT NULL,
    [Action]     VARCHAR (50)     NOT NULL,
    [EntityType] VARCHAR (50)     NOT NULL,
    [EntityId] UNIQUEIDENTIFIER NULL,
    [EntityData] VARCHAR (150)    NOT NULL,
    [EventId]  UNIQUEIDENTIFIER NULL,
    [EventData]  VARCHAR (MAX)    NULL,
    [EventUser]  VARCHAR (150)    NOT NULL,
    [EventDate]  DATETIME         NOT NULL,
    CONSTRAINT [PK_sys_log] PRIMARY KEY CLUSTERED ([LogID] ASC)
);

