﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gcpe.Hub.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SignificanceIsRequired {
            get {
                return ((bool)(this["SignificanceIsRequired"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SchedulingIsRequired {
            get {
                return ((bool)(this["SchedulingIsRequired"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool StrategyIsRequired {
            get {
                return ((bool)(this["StrategyIsRequired"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowScheduleField {
            get {
                return ((bool)(this["ShowScheduleField"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowSignificanceField {
            get {
                return ((bool)(this["ShowSignificanceField"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowHqCommentsField {
            get {
                return ((bool)(this["ShowHqCommentsField"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowRecordsSection {
            get {
                return ((bool)(this["ShowRecordsSection"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DisableEditTable {
            get {
                return ((bool)(this["DisableEditTable"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DoExceptionLogging {
            get {
                return ((bool)(this["DoExceptionLogging"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public int TypedownItemLimit {
            get {
                return ((int)(this["TypedownItemLimit"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DisableEmail {
            get {
                return ((bool)(this["DisableEmail"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int MaxBccEmails {
            get {
                return ((int)(this["MaxBccEmails"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool EnableForecastTab {
            get {
                return ((bool)(this["EnableForecastTab"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("dev.sql.sdlc.gcpe.bcgov")]
        public string DbServer {
            get {
                return ((string)(this["DbServer"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("~/Resources/BC/favicon.ico")]
        public string FaviconImg {
            get {
                return ((string)(this["FaviconImg"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("~/Resources/BC/MediaRelations@2x.png")]
        public string ContactsHeaderImg {
            get {
                return ((string)(this["ContactsHeaderImg"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\BC\\LookAheadCover.jpg")]
        public string CalendarLookAheadCoverImg {
            get {
                return ((string)(this["CalendarLookAheadCoverImg"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://intranet.gov.bc.ca/assets/download/9AE6AC6DB97B4F2D93C0A6CECC6B0CED")]
        public string NewsHelpUrl {
            get {
                return ((string)(this["NewsHelpUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("LDAP://idir.bcgov/dc=idir,dc=bcgov")]
        public string LdapUrl {
            get {
                return ((string)(this["LdapUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("idir.bcgov")]
        public string ActiveDirectoryDomain {
            get {
                return ((string)(this["ActiveDirectoryDomain"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("apps.smtp.gov.bc.ca")]
        public string SMTPServer {
            get {
                return ((string)(this["SMTPServer"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("GCPECorporateCalendar@gov.bc.ca")]
        public string LogMailFrom {
            get {
                return ((string)(this["LogMailFrom"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Craig.Shutko@gov.bc.ca")]
        public string LogMailTo {
            get {
                return ((string)(this["LogMailTo"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("GCPEHQ,GCPEMEDIA,PREM")]
        public string ApplicationOwnerOrganizations {
            get {
                return ((string)(this["ApplicationOwnerOrganizations"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("GCPEHQ")]
        public string HQAdmin {
            get {
                return ((string)(this["HQAdmin"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("GCPEHQ,GCPEMEDIA")]
        public string SharedWithExcludes {
            get {
                return ((string)(this["SharedWithExcludes"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://intranet.gov.bc.ca/assets/download/91430BC4194F488E83A77D0574603EBB")]
        public global::System.Uri HelpFileUri {
            get {
                return ((global::System.Uri)(this["HelpFileUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://dev.newsondemand.gcpe.gov.bc.ca/services/")]
        public global::System.Uri SubscribeBaseUri {
            get {
                return ((global::System.Uri)(this["SubscribeBaseUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("FIN_AP_GCPE_MDRLTNS_SDLC_TSTRS")]
        public string ContributorGroups {
            get {
                return ((string)(this["ContributorGroups"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("FIN_AP_GCPE_MDRLTNS_SDLC_DVLPRS")]
        public string AdminGroups {
            get {
                return ((string)(this["AdminGroups"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("no-reply@gov.bc.ca")]
        public string FromEmailAddress {
            get {
                return ((string)(this["FromEmailAddress"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsCompany/FIN_AP_GCPE_MDRLTNS_SDLC_TSTRS/Read|Create|Update")]
        public string permissions_1 {
            get {
                return ((string)(this["permissions_1"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsContact/FIN_AP_GCPE_MDRLTNS_SDLC_TSTRS/Read|Create|Update")]
        public string permissions_2 {
            get {
                return ((string)(this["permissions_2"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsUserReports/FIN_AP_GCPE_MDRLTNS_SDLC_TSTRS/Read|Create|Delete")]
        public string permissions_4 {
            get {
                return ((string)(this["permissions_4"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsCommonReports/FIN_AP_GCPE_MDRLTNS_SDLC_TSTRS/Read")]
        public string permissions_5 {
            get {
                return ((string)(this["permissions_5"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsApprovals/FIN_AP_GCPE_MDRLTNS_SDLC_TSTRS/Read")]
        public string permissions_6 {
            get {
                return ((string)(this["permissions_6"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsSearch/FIN_AP_GCPE_MDRLTNS_SDLC_TSTRS/Read")]
        public string permissions_7 {
            get {
                return ((string)(this["permissions_7"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsCompany/FIN_AP_GCPE_MDRLTNS_SDLC_DVLPRS/Read|Create|Update|Delete")]
        public string permissions_8 {
            get {
                return ((string)(this["permissions_8"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsContact/FIN_AP_GCPE_MDRLTNS_SDLC_DVLPRS/Read|Create|Update|Delete")]
        public string permissions_9 {
            get {
                return ((string)(this["permissions_9"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsDataLists/FIN_AP_GCPE_MDRLTNS_SDLC_DVLPRS/Read|Create|Update|Delete" +
            "")]
        public string permissions_10 {
            get {
                return ((string)(this["permissions_10"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsUserReports/FIN_AP_GCPE_MDRLTNS_SDLC_DVLPRS/Read|Create|Delete")]
        public string permissions_11 {
            get {
                return ((string)(this["permissions_11"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsCommonReports/FIN_AP_GCPE_MDRLTNS_SDLC_DVLPRS/Read|Create|Delete")]
        public string permissions_12 {
            get {
                return ((string)(this["permissions_12"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsApprovals/FIN_AP_GCPE_MDRLTNS_SDLC_DVLPRS/Read|Update|Delete")]
        public string permissions_13 {
            get {
                return ((string)(this["permissions_13"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsSearch/FIN_AP_GCPE_MDRLTNS_SDLC_DVLPRS/Read")]
        public string permissions_14 {
            get {
                return ((string)(this["permissions_14"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsCompany/FIN_AP_GCPE_MDRLTNS_SDLC_CLNTS/Read")]
        public string permissions_105 {
            get {
                return ((string)(this["permissions_105"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsContact/FIN_AP_GCPE_MDRLTNS_SDLC_CLNTS/Read")]
        public string permissions_106 {
            get {
                return ((string)(this["permissions_106"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsUserReports/FIN_AP_GCPE_MDRLTNS_SDLC_CLNTS/Read|Create|Delete")]
        public string permissions_107 {
            get {
                return ((string)(this["permissions_107"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsCommonReports/FIN_AP_GCPE_MDRLTNS_SDLC_CLNTS/Read")]
        public string permissions_108 {
            get {
                return ((string)(this["permissions_108"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaRelationsSearch/FIN_AP_GCPE_MDRLTNS_SDLC_CLNTS/Read")]
        public string permissions_109 {
            get {
                return ((string)(this["permissions_109"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https")]
        public string CloudEndpointsProtocol {
            get {
                return ((string)(this["CloudEndpointsProtocol"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("hubassetsdev")]
        public string CloudAccountName {
            get {
                return ((string)(this["CloudAccountName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("05Ae8YR1lXe/hDyIemcNoFy7WXAvONlCVk2l5pMg23DNYaf44EACol40Fu6659Ig/O7soERVixaUAlrKm" +
            "VZlug==")]
        public string CloudAccountKey {
            get {
                return ((string)(this["CloudAccountKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("core.windows.net")]
        public string CloudEndpointSuffix {
            get {
                return ((string)(this["CloudEndpointSuffix"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://dev.news.gov.bc.ca/")]
        public global::System.Uri NewsHostUri {
            get {
                return ((global::System.Uri)(this["NewsHostUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://dev.news.gov.bc.ca/assets/")]
        public global::System.Uri MediaAssetsUri {
            get {
                return ((global::System.Uri)(this["MediaAssetsUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://hubassetsdev.azureedge.net/translations/")]
        public global::System.Uri ContentDeliveryUri {
            get {
                return ((global::System.Uri)(this["ContentDeliveryUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://dev.news.gov.bc.ca/translations/")]
        public global::System.Uri TranslationsUri {
            get {
                return ((global::System.Uri)(this["TranslationsUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("E:\\Applications\\Gcpe.Hub\\Publish\\")]
        public string PublishLocation {
            get {
                return ((string)(this["PublishLocation"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("E:\\Applications\\Gov.News.Archive\\Web Site - Releases\\")]
        public string DeployLocations {
            get {
                return ((string)(this["DeployLocations"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\dev.www.sdlc.gcpe.bcgov\\Applications\\Gov.News.Media\\Web Site - Assets\\")]
        public string MediaAssetsUnc {
            get {
                return ((string)(this["MediaAssetsUnc"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>\\dev.www.sdlc.gcpe.bcgov\Applications\Gov.News.Media\Web Site - Assets\Mirror\</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection DeployFolders {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["DeployFolders"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\dev.hub.gcpe.gov.bc.ca\\NewsFiles\\")]
        public string NewsFileFolder {
            get {
                return ((string)(this["NewsFileFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2336824156b149983ef2e7dd535f56e3")]
        public string FlickrApiKey {
            get {
                return ((string)(this["FlickrApiKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("14336a4436d5617b")]
        public string FlickrApiSecret {
            get {
                return ((string)(this["FlickrApiSecret"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("72157697185772144-b4c8283bc1de20cb")]
        public string FlickrApiToken {
            get {
                return ((string)(this["FlickrApiToken"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ea7a13fe637837a0")]
        public string FlickrApiTokenSecret {
            get {
                return ((string)(this["FlickrApiTokenSecret"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("86a36ae0048e8b8b")]
        public string FlickrApiVerifier {
            get {
                return ((string)(this["FlickrApiVerifier"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("carolynn@shaw.ca")]
        public string FlickrErrorNotificationContactEmail {
            get {
                return ((string)(this["FlickrErrorNotificationContactEmail"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("anne.krutzmann@gov.bc.ca")]
        public string FlickrErrorNotificationContactEmailCC {
            get {
                return ((string)(this["FlickrErrorNotificationContactEmailCC"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("carolynn.hunter@gov.bc.ca")]
        public string FlickrErrorNotificationContactEmailBCC {
            get {
                return ((string)(this["FlickrErrorNotificationContactEmailBCC"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://www.flickr.com/photos/bcgovphotos/")]
        public global::System.Uri FlickrPrivateBaseUri {
            get {
                return ((global::System.Uri)(this["FlickrPrivateBaseUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>142.34.223.240</string>
  <string>142.34.223.241</string>
  <string>142.34.223.242</string>
  <string>142.34.223.243</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection TrustedReverseProxyServers {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["TrustedReverseProxyServers"]));
            }
        }
    }
}
