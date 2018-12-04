# gcpe-hub
Repository containing the Open Sourced code for the Hub and Hub Legacy projects - including Corporate Calendar, Media Request Manager and others. 

### Developer Setup

Important Note for developers:  
You will need to access your Windows Active Directory. 
Please gather information for all your AD domains (dev, test, prod, etc).  You will need the url and domain name.  


##### Developers NOT in Domain
And of more importance, for developers that do **NOT** log their machines into the domain, credentials will be required to query the AD.
Environment variables will be required for these developers to mimic Authorization/Authentication.

###### Environment Variables

  * **DebugLpadUser**: Domain\Username of account that can access the AD
  * **DebugLpadPwd**: password for that account
  * **DebugUsername**: Domain\Username of the developer's AD account.


#### Downloads
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

#### Installation
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

#### SQL Server Configuration
By default, the local SQL Server accepts only Windows Authorization, we need to allow for SQL Server Authorization.

1. Open SQL Server Management Studio, connect to local SQL Server using Windows Authentication
2. Right-click the database server in Object Explorer, select Properties
3. Under Security, change Server authentication from Windows Authentication mode to SQL Server and Windows authentication mode.
4. The SQL Server will have to be restarted.
5. Open SQL Server Configuration Manager (MMC snap-in)
6. Select SQL Server Services 
7. Right-click SQL Server, select Restart

#### Install local Database
Create a local version of the Gcpe.Hub database.  This will create the database, a database user, and seed the tables.

1. Review the files under /db-scripts directory.
2. Open gcpe.hub-01-create-db.sql, ensure file path is correct for location of db mdf and ldf files.
3. Edit gcpe.hub-02-create-login_hb_sdlc_dvlpr.sql, set desired password.  This password will be used in appSettings.
4. Edit gcpe.hub-data-04-systemuser.sql, edit the @systemUsername variable - your Active Directory User Account name (without the DOMAIN\)..  Also see: DebugUsername
5. In the same file, edit the @fullName variable - Display name for the Active Directory User Account 
6. Open a command prompt at /db-scripts
7. Run the following command:

 `sqlcmd -S localhost -i _sqlcmd.master.sql -v path="path to /db-scripts"`

#### Configure Applications
Under the /Config directory, there are 2 template files: Hub.Legacy.Template.appSettings.config and Hub.WebApp.Template.appsettings.json.  Copy these files and rename as: Hub.Legacy.Debug.appSettings.config and Hub.WebApp.Debug.appsettings.json.  This should be done for each build/runtime configuration and should remain in /Config directory.  **DO NOT CHECK THESE INTO SOURCE CONTROL**

Note that at build time, the configuration specific files will be copied to /Config as Hub.Legacy.appSettings.config and Hub.WebApp.appSettings.json.  In this way, we mimic the production layout and file naming for configuration.

##### Hub.Legacy
This is the collection of applications (Calendar, Contacts/Media Relations, News Release Management) that are bundled together.  These will be deployed as a sub-site of the Hub.WebApp.  Configuration for Hub.Legacy is considerable.  All configuration keys must in Hub.Legacy.\*.appSettings.config must be populated, but for immediate development purposes, set the following values to match your system:

###### Database
  * DbServer (localhost)
  * DbName (Gcpe.Hub)
  * DbUserID (see gcpe.hub-02-create-login_hb_sdlc_dvlpr.sql: HB_SDLC_DVLPR)
  * DbPassword (password you set)

These values will be used to build up various connection strings in the applications at runtime.

###### Active Directory

  * LdapUrl: url for your Active Directory (ex. LDAP://dmn.mygov/dc=dmn,dc=mygov)
  * ActiveDirectoryDomain: Domain Context or Domain name. (ex. dmn.mygov)

##### Hub.WebApp
This is the HUB proper.  It is a .NET Core application.  Configuration is significantly easier.

  * CommOffice: GCPE
  * HQMinistry: GCPEMedia
  * ConnectionStrings/HubDbConnect: connection string for your Gcpe.Hub database.  This should align with the DB configuration in Hub.Legacy.

### Solutions
There are two solutions: hub.web.opensource.sln (Hub.WebApp) and hub.legacy.opensource.sln (Hub.Legacy). 

##### Hub.WebApp

1. Open hub.web.opensource.sln   
2. Right-click Gcpe.Hub.WebApp in Solution Explorer, click Set as Startup Project
3. Clean Solution
4. Right-click solution in Solution Explorer, click Restore Nuget Packages
5. Rebuild solution
6. Debug 

##### Hub.Legacy

1. Open hub.legacy.opensource.sln
2. Right-click Gcpe.Hub.Legacy.Website in Solution Explorer, click Set as Startup Project
3. Clean Solution
4. Right-click solution in Solution Explorer, click Restore Nuget Packages
5. Rebuild solution
6. Debug 