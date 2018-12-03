USE [master]
GO

IF EXISTS 
    (SELECT name  
     FROM master.sys.server_principals
     WHERE name = 'HB_SDLC_DVLPR')
BEGIN
    DROP LOGIN [HB_SDLC_DVLPR]
END

