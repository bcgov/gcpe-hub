CREATE TABLE [media].[SysConfig] (
    [ConfigKey]         VARCHAR (250) NOT NULL,
    [ConfigValue]       VARCHAR (250) NOT NULL,
    [ConfigDataType]    INT           NOT NULL,
    [ConfigDescription] VARCHAR (500) NULL,
    CONSTRAINT [PK_SysConfig] PRIMARY KEY CLUSTERED ([ConfigKey] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_sys_config]
    ON [media].[SysConfig]([ConfigKey] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_sys_config_1]
    ON [media].[SysConfig]([ConfigValue] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_sys_config_2]
    ON [media].[SysConfig]([ConfigDataType] ASC);

