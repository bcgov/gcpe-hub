USE [master]
GO

IF EXISTS(select * from sys.databases where name='Gcpe.Hub')
  DROP DATABASE [Gcpe.Hub]
GO


