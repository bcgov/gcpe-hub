SET NOCOUNT ON
GO

--
-- Example usage (must pass in a path variable
-- sqlcmd -S localhost -i _sqlcmd.master.sql -v path="C:\gcpe-hub\db-scripts"
--

PRINT 'Load Gcpe.Hub initial data...'
:On Error exit

PRINT 'Create objects...'
:r $(path)\gcpe.hub-03-create-objects.sql
PRINT 'Load dbo schema data...'
:r $(path)\gcpe.hub-data-01-dbo.sql
PRINT 'Load calendar schema data...'
:r $(path)\gcpe.hub-data-02-calendar.sql
PRINT 'Load media schema data...'
:r $(path)\gcpe.hub-data-03-media.sql
PRINT 'Load system users...'
:r $(path)\gcpe.hub-data-04-systemuser.sql
--PRINT 'Load calendar sample data...'
--:r $(path)\gcpe.hub-sample-data-01-calendar.sql
--PRINT 'Load media sample data...'
--:r $(path)\gcpe.hub-sample-data-02-media.sql


PRINT '... complete Gcpe.Hub initial data load'