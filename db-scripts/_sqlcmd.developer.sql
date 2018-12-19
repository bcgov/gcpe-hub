SET NOCOUNT ON
GO

--
-- Example usage (must pass in a path variable
-- sqlcmd -S localhost -i _sqlcmd.developer.sql -v path="C:\gcpe-hub\db-scripts"
--

PRINT 'Start Gcpe.Hub installation...'
:On Error exit

PRINT 'Drop login...'
:r $(path)\drop-01-login.sql
PRINT 'Drop db...'
:r $(path)\drop-02-db.sql
PRINT 'Create db...'
:r $(path)\gcpe.hub-01-create-db.sql
PRINT 'Create login...'
:r $(path)\gcpe.hub-02-create_login_hb_sdlc_dvlpr.sql
PRINT 'Create objects...'
:r $(path)\gcpe.hub-03-create-objects.sql
PRINT 'Load dbo schema data...'
:r $(path)\gcpe.hub-data-01-dbo.sql
PRINT 'Load calendar schema data...'
:r $(path)\gcpe.hub-data-02-calendar.sql
PRINT 'Load media schema data...'
:r $(path)\gcpe.hub-data-03-media.sql
PRINT 'Load system user...'
:r $(path)\gcpe.hub-data-04-systemuser.sql
PRINT 'Load calendar sample data...'
:r $(path)\gcpe.hub-sample-data-01-calendar.sql
PRINT 'Load media sample data...'
:r $(path)\gcpe.hub-sample-data-02-media.sql


PRINT '... complete Gcpe.Hub installation'