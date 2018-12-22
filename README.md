# gcpe-hub
Repository containing the Open Sourced code for the Hub and Hub Legacy projects - including Corporate Calendar, Media Request Manager and others. 

# Overview
This readme has information for Infrastructure / DevOps teams, Application Developers, and Configuration of the applications.  All team members should read this to gain familiarity with the various pieces and how they all fit together.

# Table of Contents
1. [Infrastructure](#infrastructure)
2. [Developer Setup](#developer-setup)
3. [Configuration](#configuration)

# Infrastructure

## IIS, SQL Server, Active Directory Setup

The following is an example setup of the IIS web site, SQL Server database, and Active Directory groups.  This is not the only way to implement, as each team will have its own policies to follow; however, consider the following as a reference of issues to be addressed.

### Active Directory Groups and Users

Active Directory Groups play a large part in securing access to the site and filesystems. Groups are used to provide NTFS access and certain permissions within the application code.  Keep in mind, that not ALL the privileges within the application code are provided through Active Directory.   Within the Corporate Calendar (Legacy) application, there is a user management panel (/Legacy/Calendar/Admin/UserList.aspx), and within that are roles: Read Only, Editor, Advanced, Administrator, System Administrator.  See (link) for more; in this section we will only discuss securing the site, the database and granting initial access to the application through Active Directory.

For the following will use **DIR** as the Active Directory domain.

#### Service Account

An Active Directory (AD) &quot;service&quot; account is used as the identity of the Application Pools in order access the database. The following are some best practices:

- It should have a non-expiring password, so a password expiry doesn&#39;t inadvertently bring down the application
- A plan should be made to change the password manually and updated on the App Pools as per corporate policy
- A different service account should be used in production and pre-production, in order to ensure a test application can never access the production database
- For further references to this account in this document we will use HUBSVC (generally), HUBSVCP (Prod), HUBSVCT (Test)

#### Active Directory User Groups

These groups are used to authenticate to websites, and require NTFS READ permission on application folders. Names are just examples, and corporate policy / team standards should be followed

- HUB \_SDLC\_DVLPRS: Used to provide access required by developers to both the database and file systems. Contains all developers who require this access. An organization may want to use a group specific to the Hub application, or there may be an existing group of developers they prefer to use.
- HUB\_SDLC\_TSTRS: Used to authenticate to the IIS TEST Website. All users who will access the Test website for any purpose need to be members either directly or of nested security groups.
- HUB\_SDLC\_????: Other pre-production websites may need their own group depending on how the environment is managed
- HUB\_LOB\_USERS: Used to authenticate to the IIS Production Website. All users who will access the Prod website for any purpose need to be members either directly or of nested security groups.

In addition to these groups, others are identified in the Legacy configuration file to authorise certain users for certain roles in the Legacy application. These groups can be nested in the TSTRS/USERS group

### IIS and Folder Structure

It is best to first create the folder, which will be required when the Website is created, and finally set permissions which need to IIS App Pools to exist.

#### Overview of folders

##### Directory Example

    \\env.iis\applications\Gcpe.Hub
        IIS Application Root
        
    \\env.iis\applications\Gcpe.Hub\Configuration
        Location of configuration files for the Hub application and Legacy Applications.
        Code expects this relative path and name.
        
    \\env.iis\applications\Gcpe.Hub\Log Files
        IIS Logging and Hub application logging (if enabled).  
        This location and name can be changed.  
        See Hub.WebApp.appSettings.json Logging section to enable and configure logging.
        
    \\env.iis\applications\Gcpe.Hub\Web Site
        Main Web Site – the Hub..NET Core 2.1.4 application.
        Name of the directory can be changed.
        
    \\env.iis\applications\Gcpe.Hub\Web Site - Legacy 
        Hub Legacy applications.ASP .NET 4 application
        Name of directory can be changed.
        

#### IIS App Pools

Two application pools are required.

- Gcpe.Hub: With .NET CLR Version set to &quot;No Managed Code&quot;
- Gcpe.Hub\_Legacy: With .NET CLR Version set 4.0
- Note: Both should be using the identity of the AD service account HUBSVC, and an Idle Timeout of 0. All other defaults can be left as-is, or set as per applicable standards.

##### IIS Website &amp; Virtual Application

One website is created, named Gcpe.Hub, using the &quot;Web Site&quot; folder and _Gcpe.Hub_ App Pool, and Logging configured as desired. W3C logs can be placed in the _Log Files_ folder used by the application, or in a location as per applicable standards.

In this website, create an Application named &quot;Legacy&quot; using the &quot;Web Site - Legacy&quot; folder and _Gcpe.Hub\_Legacy_ App Pool.

A valid SSL certificate is highly recommended for the website, especially if it will be accessible form the internet.

**Additional Components for Reporting:**
* MS Report Viewer 2015 Runtime (also known as v12.0.0.0)
* MS CLR Types for SQL 2014

##### NTFS Permissions

The folders used by the application require the following permissions in addition to what ever is required by the system. AppPools can be identified with the local reference &quot;IIS AppPool\_\&lt;appPoolName\&gt;&quot;_

- Configuration: Both Application Pools need READ access
- Web Site &amp; Website – Legacy: Both Application Pools, and a group containing all users (HUB\_SDLC\_TSTRS in Test) requires READ access
- Log Files: Both Application Pools need WRITE access
- All: Developers and system operators also require READ and/or WRITE access as appropriate

#### Additional Details of Folders

In order to keep the above table as brief as possible, here is a further explanation of how each folder is used.

##### \\env.iis\applications\Gcpe.Hub\Configuration

Under this directory there will be two files:

1. Hub.Legacy.appSettings.config

   This is the configuration for the legacy applications.
   It is pulled into the web.config via the appSettings section file attribute.  
   Please note that the connection strings for the Legacy applications are in the Web.Config file.

2. Hub.WebApp.appSettings.json

   This is the configuration for the hub (.NET Core) application.
   This file will contain the connection string for the Hub.

##### \\env.iis\applications\Gcpe.Hub\Web Site

Contains application code for the Hub, the main web site.  This is a .NET Core 2.1.4 application.

##### \\env.iis\applications\Gcpe.Hub\Web Site - Legacy

Contains the application code to all the Legacy components: Corporate Calendar, Media Relations / Contacts, News Release Management and News Release File Management.  All the Legacy components exist under a single ASP .NET 4.0 web application.

##### \\env.iis\applications\Gcpe.Hub\Log Files

The application(s) can optionally write log files, and require a location to do so. This can optionally be used for the IIS W3C log files as well

### SQL Server

A database administrator should create the database in advance according to their corporate/team standards and provide required access for analysts, developers and the service account.

#### Application Access

All connections to SQL Server will be with Integrated Security = true in the connection strings, so the service account used for their identity will require access to the database (Gcpe.Hub).

This example will create a SQL Login for the Test Service account **DIR\HUBSVCT** , create a user in the Gcpe.Hub database, create a role to allow execution of Table and Scalar functions, and grant required roles to the service account.

  > USE [master]
  >
  > GO
  >
  > CREATE LOGIN [DIR\HUBSVCT] FROM WINDOWS WITH DEFAULT_DATABASE=[Gcpe.Hub]
  >
  > GO
  >
  > USE [Gcpe.Hub]
  >
  > GO
  >
  > CREATE ROLE db_executor
  >
  > GO
  > 
  > GRANT EXECUTE TO db_executor
  >
  > GO
  >
  > CREATE USER [DIR\HUBSVCT] FOR LOGIN [DIR\HUBSVCT]
  >
  > GO
  > 
  > USE [Gcpe.Hub]
  >
  > GO
  >
  > ALTER ROLE [db_datareader] ADD MEMBER [DIR\HUBSVCT]
  > 
  > ALTER ROLE [db_datawriter] ADD MEMBER [DIR\HUBSVCT]
  >
  > ALTER ROLE [db_executor] ADD MEMBER [DIR\HUBSVCT]
  >
  > GO

### SQL Server Groups

Developers will require db\_owner access to pre-production databases to create/manage the database schemas. It&#39;s best practice that the same accounts not have db\_owner in production to prevent mistakes targeting the wrong database. Often developers only want db\_datareader access to production database, but they may use a different account in their operator role.

Similar T-SQL used to create the Login/User for the service account can be used to provide the developer groups access, using the desired roles.

#### Groups used within Application Code

Within the Media Relations / Contacts application (/Legacy/Contacts), Active Directory groups are used for additional logic and security.

In the Hub.Legacy.appSettings.config, there are two group settings: ContributorGroups and AdminGroups.
See [Configuration / Groups Configuration and Permissions Matrix](#groups-configuration-and-permissions-matrix)

### Seed Gcpe.Hub Database
The assumption here is that the DBAs have created a Gcpe.Hub database, assigned the service accounts and Active Directory access. The following is to load the objects (tables, views, etc), and load an initial dataset. The initial data set contains records that are required for certain (current) system logic to work correctly; ids and names that are expected to trigger additional logic.

1. Edit gcpe.hub-data-04-systemuser.sql, edit the @list variable.  This should be a list of Active Directory accounts and names to be seeded as System Admins.  The @list variable requires that it end with the separator: "|", and that each Account Name have a Full name.  (ex. "ACCOUNT1 - Acct One|ACCOUNT2 - AcctTwo|ACCOUNTN - Acct N|")
2. Determine if you want some non-required sample data (these tables can be populated via the Admin screens).  Open \_sqlcmd.master.sql and uncomment the lines to call the sample data scripts.
3. Review the 3 data scripts (ex. gcpe.hub-data-01-dbo.sql) and add any additional data you would like seeded.  For example, seed more Ministries or Cities.
5. Open a command prompt at /db-scripts
6. Run the following command:

   `sqlcmd -S localhost -i _sqlcmd.master.sql -v path="path to /db-scripts"`


# Developer Setup

Important Note for developers:  
You will need to access your Windows Active Directory. 
Please gather information for all your AD domains (dev, test, prod, etc).  You will need the url and domain name.  

## Developers NOT in Domain
And of more importance, for developers that do **NOT** log their machines into the domain, credentials will be required to query the AD.
Environment variables will be required for these developers to mimic Authorization/Authentication.

### Environment Variables

  * **DebugLdapUser**: Domain\Username of account that can access the AD
  * **DebugLdapPwd**: password for that account
  * **DebugUsername**: Domain\Username of the developer's AD account.
  * **GcpeHubDbUserID**: Database User ID used for developer only connections.  Set to HB_SDLC_DVLPR to match db-scripts/gcpe.hub-02-create-login_hb_sdlc_dvlpr.sql. Adjust as necessary.
  * **GcpeHubDbPassword**: Database User Password used for developer only connections.  Same as password entered in HB_SDLC_DVLPR to match db-scripts/gcpe.hub-02-create-login_hb_sdlc_dvlpr.sql

  **Note**: if GcpeHubDbUserID and GcpeHubDbPassword environment variables are set, connection strings will be changed from Integrated Security=true to Integrated Security=false. These values are added as User ID and Password.

## Downloads
1. .NET Framework 4.6.2 Developer Pack [link](https://www.microsoft.com/en-us/download/details.aspx?id=53321)
2. TypeScript 2.3 SDK for Visual Studio 2017 [link](https://www.microsoft.com/en-us/download/details.aspx?id=55258)
3. Open XML SDK 2.5 & Open XML SDK Tool 2.5 [link](https://www.microsoft.com/en-us/download/details.aspx?id=30425)
4. Visual Studio 2017 Community Edition [link](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=15)

   This code base can be developed and run using Visual Studio 2017 Community Edition, however; 2017 Professional Edition is preferred.
  
5. SQL Server Developer Edition [link](https://go.microsoft.com/fwlink/?linkid=853016)
6. SQL Server Management Studio [link](https://go.microsoft.com/fwlink/?linkid=2043154)
7. Web Essentials 2017 VS extension [link](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebExtensionPack2017)

   This is required to generate/identify the minified version of our site.css file in the Gcpe.Hub.WebApp project and the Gov.News.WebApp projects. This projects will not build locally without site.min.cs, so it must be generated locally, by installing web essentials

8. TypeScript Definition Generator VS extension [link](https://github.com/madskristensen/TypeScriptDefinitionGenerator)

   This used to be included in Web Essentials, but it is no longer. It is a tool for generating typescript .d.ts files from C# classes. See Gcpe.Hub.WebApp in the models folder there are C# objects with generated .d.ts files (click the arrow to see).
   **Note: this only works on Windows 10**

9. Node.js with NPM (version 8.11 or later, this written using 10.4.1) [link](https://nodejs.org/en/download/)


## Installation
Assumes administrative privileges on the machine.

1. .NET Framework, TypeScript SDK, Open XML SDK - just double-click and use defaults.
2. Visual Studio installation, you will be presented with options for adding Workloads and Components.  
 
    At a minimum, you must install:
    * .NET Development Workload
    * ASP.NET Development Workload
    * .NET Compiler Platform SDK Component.    

3. SQL Server Developer Edition install using the Basic option (defaults), then Customize.
  
  Customizations
  * Installation Type = Add Features to existing
  * Feature Selection / Instance Features / Full-Text and Semantic Extractions

4. SQL Server Management Studio, double-click and use defaults
5. Web Essentials and TypeScript (if applicable) extensions, just double-click.
6. Node.js, follow installation instuctions.  If using the Windows Installer, double-click and accept defaults.  Ensure node is in the path.
7. Install bower and gulp globally, from a command prompt:

   `npm install -g bower`

   `npm install -g gulp`

8.  Ensure MSBUILD is on your PATH, if it is not already there (Community Edition does not add VS tools to PATH).
    
    `set PATH=%PATH%;C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\bin;`


## Developer Local SQL Server Configuration
By default, the local SQL Server accepts only Windows Authorization, we need to allow for SQL Server Authorization.

1. Open SQL Server Management Studio, connect to local SQL Server using Windows Authentication
2. Right-click the database server in Object Explorer, select Properties
3. Under Security, change Server authentication from Windows Authentication mode to SQL Server and Windows authentication mode.
4. The SQL Server will have to be restarted.
5. Open SQL Server Configuration Manager (MMC snap-in)
6. Select SQL Server Services 
7. Right-click SQL Server, select Restart

### Install local Database
Create a local version of the Gcpe.Hub database.  This will create the database, a database user, and seed the tables.

1. Review the files under /db-scripts directory.
2. Open gcpe.hub-01-create-db.sql, ensure file path is correct for location of db mdf and ldf files.
3. Edit gcpe.hub-02-create-login_hb_sdlc_dvlpr.sql, set desired password.  User ID and passwords should be set as see [environment variables](#environment-variables): GcpeHubDbUserID and GcpeHubDbPassword.  This should only be used on Debug/Development environments.
4. Edit gcpe.hub-data-04-systemuser.sql, edit the @list variable - enter your account name (without DOMAIN\) followed by a dash and a Full name (ex. "ADEVELOPER - Adam Developer|").  Ensure the list is terminated with the separator "|".   Also see: DebugUsername
5. Open a command prompt at /db-scripts
6. Run the following command:

   `sqlcmd -S localhost -i _sqlcmd.developer.sql -v path="path to /db-scripts"`

## Configure Applications
Under the /Configuration directory, there are 2 template files: Hub.Legacy.Template.appSettings.config and Hub.WebApp.Template.appsettings.json.  Copy these files and rename as: Hub.Legacy.Debug.appSettings.config and Hub.WebApp.Debug.appsettings.json.  This should be done for each build/runtime configuration and should remain in /Configuration directory.  **DO NOT CHECK THESE INTO SOURCE CONTROL**

Note that at build time, the configuration specific files will be copied to /Configuration as Hub.Legacy.appSettings.config and Hub.WebApp.appSettings.json.  In this way, we mimic the production layout and file naming for configuration.

### Hub.Legacy
This is the collection of applications (Calendar, Contacts/Media Relations, News Release Management) that are bundled together.  These will be deployed as a sub-site of the Hub.WebApp.  Configuration for Hub.Legacy is considerable.  All configuration keys must in Hub.Legacy.\*.appSettings.config must be populated, but for immediate development purposes, set the following values to match your system:

#### Hub.Legacy.appSettings.config - Database
  * DbServer (localhost)
  * DbName (Gcpe.Hub)

These values will be used to build up various connection strings in the applications at runtime.  For development scenarios, see [environment variables](#environment-variables). 

#### Hub.Legacy.appSettings.config - Active Directory

  * LdapUrl: url for your Active Directory (ex. LDAP://dmn.mygov/dc=dmn,dc=mygov)
  * ActiveDirectoryDomain: Domain Context or Domain name. (ex. dmn.mygov)

### Hub.WebApp
This is the HUB proper.  It is a .NET Core application.  Configuration is significantly easier.

#### Hub.WebApp.appSettings.json
  * CommOffice: GCPE
  * HQMinistry: GCPEMedia
  * ConnectionStrings/HubDbConnect: connection string for your Gcpe.Hub database.  This should align with the DB configuration in Hub.Legacy.

### Solutions
There are two solutions: hub.web.opensource.sln (Hub.WebApp) and hub.legacy.opensource.sln (Hub.Legacy). 

#### Hub.WebApp

1. Open hub.web.opensource.sln   
2. Right-click Gcpe.Hub.WebApp in Solution Explorer, click Set as Startup Project
3. Clean Solution
4. Right-click solution in Solution Explorer, click Restore Nuget Packages
5. Rebuild solution
6. Debug 

#### Hub.Legacy

1. Open hub.legacy.opensource.sln
2. Right-click Gcpe.Hub.Legacy.Website in Solution Explorer, click Set as Startup Project
3. Clean Solution
4. Right-click solution in Solution Explorer, click Restore Nuget Packages
5. Rebuild solution
6. Debug 

##### LOCAL_MEDIA_STORAGE Preprocessor Symbol
There is a [define preprocessor symbol](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-define) (LOCAL_MEDIA_STORAGE).  This controls whether the application uses local storage (filesystem and database) or Azure Cloud based storage.  By default, this is _NOT_ defined and will use Azure Cloud Storage.  

This can be added to the [Project](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/define-compiler-option) (Gcpe.Hub.Legacy.Website) Properties... Build -> Conditional compilation symbols. 

Or this can be added as a parameter to msbuild (note that you must specify ALL DefineConstants in this parameter - it overrides the value in the csproj file):

    `msbuild Hub.Legacy\Gcpe.Hub.Legacy.Website\Gcpe.Hub.Legacy.Website.csproj /t:Clean,Build /p:SolutionDir=C:\gcpe-hub\ /p:Configuration=Debug  /p:DefineConstants="DEBUG;TRACE;LOCAL_MEDIA_STORAGE"`

**Note:** If you encounter an issue when running through the IDE like _"Could not find a part of the path … bin\roslyn\csc.exe"_.  Try the following in the Package Manager console:
  
  `PM> Install-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform`

## Publication and Deployment

### Publish Applications
We will build and publish to a local filesystem first, then copy the output to the IIS server.  The /p:DeleteExistingFiles=True switch will not only delete files, but will delete the base folder.  We need to leave those intact on IIS directories due to specific permissions and site configuration.

1. Ensure msbuild, bower, gulp are on your path
2. Open command prompt at root of code (ex. C:\gcpe-hub)
3. Publish to a local filesystem mirroring IIS layout (ex. C:\pub)   
4. Build and Publish the Release version of Hub.Web (see [LOCAL_MEDIA_STORAGE Preprocessor Symbol](#local_media_storage-preprocessor-symbol))

    `msbuild Gcpe.Hub.WebApp\Gcpe.Hub.WebApp.csproj /t:Clean,Build,Publish /p:SolutionDir=C:\gcpe-hub\ /p:Configuration=Release /p:DeployOnBuild=true /p:DeployDefaultTarget=WebPublish /p:WebPublishMethod=FileSystem /p:PublishProvider=FileSystem /p:publishUrl="C:\Pub\Web Site" /p:DeleteExistingFiles=True`

5. Build and Publish the Release version of Gcpe.Hub.Legacy.Website

    `msbuild Hub.Legacy\Gcpe.Hub.Legacy.Website\Gcpe.Hub.Legacy.Website.csproj /t:Clean,Build,Publish /p:SolutionDir=C:\gcpe-hub\ /p:Configuration=Release /p:DeployOnBuild=true /p:DeployDefaultTarget=WebPublish /p:WebPublishMethod=FileSystem /p:PublishProvider=FileSystem /p:publishUrl="C:\Pub\Web Site - Legacy" /p:DeleteExistingFiles=True`

6.  Take applications offline in IIS.  Under \\IIS Site\Web Site and \\IIS Site\Web Site - Legacy, add files app_offline.htm (http://appoffline.appspot.com/).
7.  Copying over of files will be up to the team.  You may want to delete all current files from IIS or just cherrypick files, or copy over the build output.  That would assume that no resource files were removed between releases, only changes to the dlls and configuration.  Also, the tools and scripts can vary, for example, you may wish to use ROBOCOPY.

    `xcopy "C:\Pub\Web Site\*" "\\IIS Site\Web Site" /s /i /y`
    
    `xcopy "C:\Pub\Web Site - Legacy\*" "\\IIS Site\Web Site - Legacy" /s /i /y`

8.  Copy the environment specific configuration files to the IIS Site Configuration folder (Hub.Legacy.appSettings.config and Hub.WebApp.appSettings.json).  Ensure files are named correctly and are configured for the intended environment.
9.  Remove (or rename) the 2 app_offline.htm files

# Configuration

## Hub.Legacy.appSettings.config

  **Notes**
  - Configuration for all of the legacy applications.
  - A template file with all the keys and comments is provided (/Configuration/Hub.Legacy.Template.config).
  - Create a file for each build configuration (ex. Hub.Legacy.Debug.appSettings.config) with appropriate values.
  - Create a file for each environment, and ensure that it is copied to the /Configuration IIS directory on publication (see [Additional Details of Folders](#additional-details-of-folders))
  - This is an xml file, used to populate the Hub.Legacy\Gcpe.Hub.Legacy.Website\Web.Config appSettings section.
  - Connection strings are in the Hub.Legacy\Gcpe.Hub.Legacy.Website\Web.Config connectionStrings section, Database Name and Server Name are replaced with values in the appSettings.config file.
   
  **TODO: fill in each key ** 




### Groups Configuration and Permissions Matrix

Within the Media Relations / Contacts application (/Legacy/Contacts), Active Directory groups are used for additional logic and security.

See [Active Directory User Groups](#active-directory-user-groups)

In the Hub.Legacy.appSettings.config, there are two group settings: ContributorGroups and AdminGroups.

These settings are comma separated lists of Active Directory Groups and are give special privileges for adding/editing Companies and Contacts, history of records.  Admins are always considered Contributors.  Admins can also edit things like web addresses and Media Distribution lists.

#### Media Relations Permissions Matrix

Active Directory groups are also used to build a permissions matrix for Media Relations / Contacts.  In the Hub.Legacy.appSettings.config, there are a series of keys named permissions\_NNN.  The numbers are irrelevant, all keys beginning with permissions\_ will be read to build out the matrix.

The matrix provides a mapping for groups to specific sections of the Media Relations / Contacts application and specific functionality.

The permissions\_ key values are parsed into Site Sections, Active Directory Group, permissions.

Example:

  `<add key="permissions_1" value="MediaRelationsCompany/GCPE_CONTRIB/Read|Create|Update" />`

The 3 components are separated with &quot;/&quot;.

The permissions (or site actions) are separated with &quot;|&quot;.

In the above:

    Site Section = MediaRelationsCompany
    Active Directory Group = GCPE_CONTRIB
    Site Actions = Read, Create, Update.

This means, that members of GCPE\_CONTRIB Active Directory Group can read, create new, and update existing companies.

##### Site Sections

Enumeration used to define certain pages/screens within Media Relations (see Hub.Legacy/Gcpe.Contacts.Library/Permissions.cs).

- None
- MediaRelationsCompany
- MediaRelationsContact
- MediaRelationsDataLists
- MediaRelationsCommonReports
- MediaRelationsUserReports
- MediaRelationsSearch
- MediaRelationsApprovals

##### Site Actions

Flags used to determine allowed functionality within the application; basically permissions (see Hub.Legacy/Gcpe.Contacts.Library/Permissions.cs).

- None
- Read
- Create
- Update
- Delete

## Hub.WebApp.appSettings.json

  **Notes**
  - Configuration for the .NET Core Hub application.
  - This is a json file, and thus has no notes or comments within.
  - A template file with all the keys is provided (/Configuration/Hub.WebApp.Template.json).
  - Create a file for each build configuration (ex. Hub.WebApp.Debug.appSettings.json) with appropriate values.
  - Create a file for each environment, and ensure that it is copied to the /Configuration IIS directory on publication (see [Additional Details of Folders](#additional-details-of-folders))
  - The connection string is in this file, so we can fully create it for each environment.
   
  **TODO: fill in each key ** 

