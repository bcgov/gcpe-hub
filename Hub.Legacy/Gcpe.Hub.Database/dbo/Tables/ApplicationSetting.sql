CREATE TABLE [dbo].[ApplicationSetting] (
    [SettingName]  VARCHAR (100)  NOT NULL,
    [SettingValue] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Setting] PRIMARY KEY CLUSTERED ([SettingName] ASC)
);

