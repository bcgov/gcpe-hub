using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Text;
using MediaRelationsDatabase;
using MediaRelationsLibrary;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using static MediaRelationsLibrary.CommonEventLogging;
using Gcpe.Hub.Properties;

public partial class Search : System.Web.UI.Page
{
    private string tabSpacing = "&nbsp;&nbsp;&nbsp;";

    private enum Tabs
    {
        Contacts,
        Outlets,
        Companies
    }

    private List<ContactSearchResult> contactResults = null;
    private List<CompanySearchResult> companyResults = null;
    private List<CompanySearchResult> outletResults = null;

    private bool canDeleteContacts = false;
    private bool canDeleteCompany = false;

    private Tabs currentTab = Tabs.Contacts;

    StringBuilder shareSb;
    StringBuilder emailSb;
    int emailCount;

    string headingStyle = "font-weight:bold;font-size:14px;";

    protected void Page_Load(object sender, EventArgs e)
    {
        Permissions.SiteAction siteActions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsContact);
        if ((siteActions & Permissions.SiteAction.Delete) != 0) canDeleteContacts = true;
        siteActions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsCompany);
        if ((siteActions & Permissions.SiteAction.Delete) != 0) canDeleteCompany = true;

        if (!Enum.TryParse(Request.QueryString["tab"], out currentTab))
            currentTab = Tabs.Contacts;

        if (IsPostBack) return;

        ReportsLib rLib = new ReportsLib();
        if (rLib.CanCreatePublicReports) reportTypeContainer.Visible = true;

        DateTime startDt = DateTime.Now;

        if (!canDeleteCompany)
        {
            companyDisplayCbTop.Visible = false;
            companyDisplayCbBottom.Visible = false;
            outletDisplayCbBottom.Visible = false;
            outletDisplayCbTop.Visible = false;

            TopOutletsPaginator.BulkActions = BottomOutletsPaginator.BulkActions = false;
            topCompanyPaginator.BulkActions = bottomCompanyPaginator.BulkActions = false;

        }
        else
        {
            List<string> actions = new List<string>();
            actions.Add("Bulk Actions");
            actions.Add("Delete");
            TopOutletsPaginator.BulkActionItems = BottomOutletsPaginator.BulkActionItems = actions;
            topCompanyPaginator.BulkActionItems = bottomCompanyPaginator.BulkActionItems = actions;

            topCompanyPaginator.BulkActionsEventHandler += TopCompanyBulkActionsHandler;
            bottomCompanyPaginator.BulkActionsEventHandler += BottomCompanyBulkActionsHandler;
            TopOutletsPaginator.BulkActionsEventHandler += TopOutletsBulkActionHandler;
            BottomOutletsPaginator.BulkActionsEventHandler += BottomOutletsBulkActionHandler;
        }


        if (!canDeleteContacts)
        {
            contactDisplayCbTop.Visible = false;
            contactDisplayCbBottom.Visible = false;

            topContactsPaginator.BulkActions = bottomContactsPaginator.BulkActions = false;
        }
        else
        {
            List<string> actions = new List<string>();
            actions.Add("Bulk Actions");
            actions.Add("Delete");
            topContactsPaginator.BulkActionItems = bottomContactsPaginator.BulkActionItems = actions;

            topContactsPaginator.BulkActionsEventHandler += TopContactsBulkActionHandler;
            bottomContactsPaginator.BulkActionsEventHandler += BottomContactsBulkActionHandler;
        }

        int maxEmailCount = Settings.Default.MaxBccEmails;
        if (maxEmailCount > 50) maxEmailCount = 50; //hard-coded limit

        using (var ctx = new MediaRelationsEntities())
        {
            DoSearch(ctx);
        }

        int contactCount = contactResults.Count;
        int outletCount = outletResults.Count;
        int companyCount = companyResults.Count;

        #region tabs setup

        List<KeyValuePair<string, string>> qs = CommonMethods.GetEditableQueryStringMultiple();
        CommonMethods.RemoveItemFromQueryString(qs, "page");
        CommonMethods.RemoveItemFromQueryString(qs, "perpage");
        CommonMethods.RemoveItemFromQueryString(qs, "message");
        CommonMethods.RemoveItemFromQueryString(qs, "tab");

        var tabs = new Dictionary<string, string>();
        CommonMethods.RemoveItemFromQueryString(qs, "tab");
        qs.Add(new KeyValuePair<string, string>("tab", Tabs.Contacts.ToString()));
        tabs.Add(contactCount + tabSpacing + "Contact" + (contactCount == 1 ? "" : "s"), Request.Path + "?" + CommonMethods.GetQueryStringMultiple(qs));
        CommonMethods.RemoveItemFromQueryString(qs, "tab");
        qs.Add(new KeyValuePair<string, string>("tab", Tabs.Outlets.ToString()));
        tabs.Add(outletCount + tabSpacing + "Outlet" + (outletCount == 1 ? "" : "s"), Request.Path + "?" + CommonMethods.GetQueryStringMultiple(qs));
        CommonMethods.RemoveItemFromQueryString(qs, "tab");
        qs.Add(new KeyValuePair<string, string>("tab", Tabs.Companies.ToString()));
        tabs.Add(companyCount + tabSpacing + "Compan" + (companyCount == 1 ? "y" : "ies"), Request.Path + "?" + CommonMethods.GetQueryStringMultiple(qs));


        tabControl.Tabs = tabs;

        #endregion

        // setup search criteria display
        int totalCount = contactCount + outletCount + companyCount;

        // desktop search setup
        searchCount.InnerText = totalCount.ToString();
        searchCriteria.InnerHtml = " Results for \"" + advancedSearchControlDesktop.SearchCriteriaString + "\"";

        // mobile advanced search setup
        advancedSearchHref.HRef = "RefineResults.aspx?" + advancedSearchControlDesktop.SearchCriteriaQueryUrl;
        searchCriteriaLit.Text = totalCount + " Results for \"" + advancedSearchControlDesktop.SearchCriteriaString + "\"";

        ShareCountLit.Text = "share: " + shareSb.Length;


        if (!Settings.Default.DisableEmail)
        {
            emailHrefLink.NavigateUrl = mobileEmailHref.NavigateUrl = (emailCount > maxEmailCount ? "javascript:alert(tooManyEmailError.replace('###emailcount###', " + emailCount + ").replace('###maxemails###'," + maxEmailCount + "));" : emailSb.ToString());
        }
        else
        {
            emailHrefLink.NavigateUrl = mobileEmailHref.NavigateUrl = "javascript:alert('This function has been disabled');";
        }

        if (emailCount <= maxEmailCount)
        {
            emailHrefLink.Attributes.Add("onclick", "return emailLinkClick();");
            mobileEmailHref.Attributes.Add("onclick", "return emailLinkClick();");
        }
        shareSb.Append("</div>");
    }

    private void DoSearch(MediaRelationsEntities ctx)
    {
        shareSb = new StringBuilder();
        emailSb = new StringBuilder();
        shareSb.Append("<div style='font-family:Verdana;font-size:14px;'><p>Search results for: " + advancedSearchControlDesktop.SearchCriteriaString + "</p>\n\n");
        emailSb.Append("mailto:?bcc=");

        SearchLib lib = new SearchLib();

        IQueryable<CompanySearchResult> companies = lib.CompanySearch(advancedSearchControlDesktop.SearchCriteria, ctx, advancedSearchControlDesktop.MatchAll);
        SetupCompanyTab(ctx, companies, advancedSearchControlDesktop.SearchCriteria, (currentTab == Tabs.Companies ? true : false));

        IQueryable<CompanySearchResult> outlets = lib.OutletSearch(advancedSearchControlDesktop.SearchCriteria, ctx, advancedSearchControlDesktop.MatchAll, companyResults);
        SetupOutletsTab(ctx, outlets, advancedSearchControlDesktop.SearchCriteria, (currentTab == Tabs.Outlets ? true : false), companyResults);

        IQueryable<ContactSearchResult> contacts = lib.ContactSearch(advancedSearchControlDesktop.SearchCriteria, ctx, advancedSearchControlDesktop.MatchAll, outletResults);
        SetupContactTab(ctx, contacts, advancedSearchControlDesktop.SearchCriteria, (currentTab == Tabs.Contacts ? true : false), outletResults);

        if (lib.IsNestedContactSearch(advancedSearchControlDesktop.SearchCriteria, advancedSearchControlDesktop.MatchAll))
        {
            outlets = (from o in contacts
                       from x in o.Contact.ContactMediaJobTitles
                       select new CompanySearchResult
                       {
                           Company = x.Company,
                           CompanyName = x.Company.CompanyName,
                           ParentCompanyName = (x.Company.ParentCompany == null ? null : x.Company.ParentCompany.CompanyName),

                           City = x.Company.CompanyAddresses.Where(t => t.City != null).OrderBy(t => t.AddressType).Select(t => t.City.CityName).FirstOrDefault(),
                           Email = (x.Company.CompanyWebAddresses.Any(t => t.WebAddressType.WebAddressTypeName.Contains("email")) ? x.Company.CompanyWebAddresses.Where(t => t.WebAddressType.WebAddressTypeName.Contains("email")).FirstOrDefault().WebAddress : null),
                           Twitter = (x.Company.CompanyWebAddresses.Any(t => t.WebAddressType.WebAddressTypeName.Contains("twitter")) ? x.Company.CompanyWebAddresses.Where(t => t.WebAddressType.WebAddressTypeName.Contains("twitter")).FirstOrDefault().WebAddress : null),
                           PrimaryPhone = (x.Company.CompanyPhoneNumbers.Any(t => t.PhoneType.PhoneTypeName.Contains("primary")) ? x.Company.CompanyPhoneNumbers.Where(t => t.PhoneType.PhoneTypeName.Contains("primary")).FirstOrDefault().PhoneNumber : null),
                           PrimaryPhoneExtension = (x.Company.CompanyPhoneNumbers.Any(t => t.PhoneType.PhoneTypeName.Contains("primary")) ? x.Company.CompanyPhoneNumbers.Where(t => t.PhoneType.PhoneTypeName.Contains("primary")).FirstOrDefault().PhoneNumberExtension : null),
                           FirstMediaType = (x.Company.MediaTypes.Count > 0 ? x.Company.MediaTypes.FirstOrDefault().MediaTypeName : null)
                       }).Distinct();

            companies = Enumerable.Empty<CompanySearchResult>().AsQueryable(); // setup a blank list of companies
            SetupCompanyTab(ctx, companies, advancedSearchControlDesktop.SearchCriteria, (currentTab == Tabs.Companies ? true : false));
            SetupOutletsTab(ctx, outlets, advancedSearchControlDesktop.SearchCriteria, (currentTab == Tabs.Outlets ? true : false), companyResults);
        }
    }

    public void SaveReport_Click(object sender, EventArgs e)
    {
        onLoadJsLit.Text = "";

        ReportsLib lib = new ReportsLib();

        string url = advancedSearchControlDesktop.SearchCriteriaQueryUrl;

        bool isPublic = false;

        if (lib.CanCreatePublicReports)
        {
            isPublic = publicReportRb.Checked;
        }

        string reportName = reportTitleNameTb.Text.Trim();

        int errors;
        if (isPublic)
        {
            errors = lib.CreatePublicReport(reportName, url);
        }
        else
        {
            errors = lib.CreatePrivateReport(reportName, url);
        }

        if (errors != 0)
        {
            string errorMessage = "";
            if ((errors & 1) != 0) errorMessage += "Error!\\nReport name must not be empty.\\n";
            if ((errors & 2) != 0) errorMessage += "Error!\\nThe report title you entered is already in use. Please enter a different report title.\\n";
            if ((errors & 4) != 0) errorMessage += "Error!\\nYou do not have permission to create the report.\\n";

            onLoadJsLit.Text = "<script type='text/javascript'>alert(\"" + errorMessage + "\");</script>\n";
        }
        else
        {
            onLoadJsLit.Text = "<script type='text/javascript'>alert(reportSavedSuccess.replace('###reportName###', \"" + reportName.Replace("\"", "\\\"") + "\"));</script>\n";
        }
    }

    public void ExportButton_Click(object sender, EventArgs e)
    {
        byte[] buffer;

        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
        {
            using (var ctx = new MediaRelationsEntities())
            {
                DoSearch(ctx);

                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                    spreadsheetDocument.WorkbookPart.Workbook = new Workbook();

                    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                    //Contacts
                    {
                        WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

                        Worksheet worksheet = new Worksheet();
                        worksheetPart.Worksheet = worksheet;

                        SheetData sheetData = new SheetData();

                        Sheet sheet = new Sheet()
                        {
                            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                            SheetId = 1,
                            Name = "Contacts"
                        };

                        sheets.Append(sheet);

                        Columns columns = new Columns();
                        columns.Append(new Column() { Min = 1, Max = 1, Width = 25, CustomWidth = true });
                        columns.Append(new Column() { Min = 2, Max = 2, Width = 60, CustomWidth = true });
                        columns.Append(new Column() { Min = 3, Max = 3, Width = 25, CustomWidth = true });
                        columns.Append(new Column() { Min = 4, Max = 4, Width = 25, CustomWidth = true });
                        columns.Append(new Column() { Min = 5, Max = 5, Width = 40, CustomWidth = true });
                        columns.Append(new Column() { Min = 6, Max = 6, Width = 40, CustomWidth = true });
                        worksheetPart.Worksheet.Append(columns);

                        Row headerRow = new Row();
                        headerRow.Append(new Cell() { CellValue = new CellValue("Name"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("Outlet"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("City"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("Primary"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("Email"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("Ministerial Job Title"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        sheetData.Append(headerRow);

                        var contacts = (from c in ctx.Contacts.Include("ContactMediaJobTitles") select c).ToArray();

                        foreach (ContactSearchResult contact in contactResults)
                        {
                            Row row = new Row();

                            row.Append(new Cell() { CellValue = new CellValue(contact.FirstName + " " + contact.LastName), DataType = new EnumValue<CellValues>(CellValues.String) });

                            var dbContact = contacts.Single(c => c.Id == contact.Contact.Id);
                            //ctx.Contacts.Attach(contact.Contact);

                            //bool first = true;
                            string companyString = "";
                            foreach (ContactMediaJobTitle contactMediaJob in dbContact.ContactMediaJobTitles)
                            {
                                //if (!first) companyString += "<br style=\"mso-data-placement:same-cell;\"/>";
                                companyString += contactMediaJob.Company.CompanyName;

                                //if (dbContact.ContactMediaJobTitles.Count > 0)
                                {
                                    //List<ContactMediaJobTitle> mediaJobTitles = dbContact.ContactMediaJobTitles.Where(x => x.CompanyId == mediaJobTitle.CompanyId).ToList();
                                    //if (mediaJobTitles.Count > 0)
                                    {
                                        companyString += " (";

                                        bool frst = true;
                                        //foreach (ContactMediaJobTitle title in mediaJobTitles)
                                        {
                                            if (!frst) companyString += ", ";
                                            companyString += contactMediaJob.MediaJobTitle.MediaJobTitleName;
                                            frst = false;
                                        }

                                        companyString += ")";
                                    }
                                }

                                //first = false;
                            }

                            row.Append(new Cell() { CellValue = new CellValue(companyString), DataType = new EnumValue<CellValues>(CellValues.String) });

                            row.Append(new Cell() { CellValue = new CellValue(contact.City), DataType = new EnumValue<CellValues>(CellValues.String) });
                            row.Append(new Cell() { CellValue = new CellValue(contact.PrimaryPhone + (!string.IsNullOrEmpty(contact.PrimaryPhoneExtension) ? " ext. " + contact.PrimaryPhoneExtension : "")), DataType = new EnumValue<CellValues>(CellValues.String) });

                            row.Append(new Cell() { CellValue = new CellValue(contact.Email), DataType = new EnumValue<CellValues>(CellValues.String) });
                            row.Append(new Cell() { CellValue = new CellValue(contact.MinisterialJobTitle), DataType = new EnumValue<CellValues>(CellValues.String) });

                            sheetData.Append(row);
                        }

                        worksheet.Append(sheetData);
                    }

                    //Outlets
                    {
                        WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

                        Worksheet worksheet = new Worksheet();
                        worksheetPart.Worksheet = worksheet;

                        SheetData sheetData = new SheetData();

                        Sheet sheet = new Sheet()
                        {
                            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                            SheetId = 2,
                            Name = "Outlets"
                        };

                        sheets.Append(sheet);

                        Columns columns = new Columns();
                        columns.Append(new Column() { Min = 1, Max = 1, Width = 60, CustomWidth = true });
                        columns.Append(new Column() { Min = 2, Max = 2, Width = 40, CustomWidth = true });
                        columns.Append(new Column() { Min = 3, Max = 3, Width = 25, CustomWidth = true });
                        columns.Append(new Column() { Min = 4, Max = 4, Width = 20, CustomWidth = true });
                        columns.Append(new Column() { Min = 5, Max = 5, Width = 40, CustomWidth = true });
                        columns.Append(new Column() { Min = 6, Max = 6, Width = 40, CustomWidth = true });
                        worksheetPart.Worksheet.Append(columns);

                        Row headerRow = new Row();
                        headerRow.Append(new Cell() { CellValue = new CellValue("Name"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("Company"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("City"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("News Desk"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("Email"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("Media Type"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        sheetData.Append(headerRow);

                        var companies = (from c in ctx.Companies.Include("CompanyPhoneNumbers") select c).ToArray();

                        foreach (CompanySearchResult outlet in outletResults)
                        {
                            Row row = new Row();

                            var company = companies.Single(c => c.Id == outlet.Company.Id);

                            row.Append(new Cell() { CellValue = new CellValue(outlet.CompanyName), DataType = new EnumValue<CellValues>(CellValues.String) });
                            row.Append(new Cell() { CellValue = new CellValue(outlet.ParentCompanyName), DataType = new EnumValue<CellValues>(CellValues.String) });
                            row.Append(new Cell() { CellValue = new CellValue(outlet.City), DataType = new EnumValue<CellValues>(CellValues.String) });

                            CompanyPhoneNumber newsDeskPhone = company.NewsDeskPhone;
                            if (newsDeskPhone == null)
                                row.Append(new Cell() { CellValue = new CellValue(string.Empty), DataType = new EnumValue<CellValues>(CellValues.String) });
                            else
                                row.Append(new Cell() { CellValue = new CellValue(newsDeskPhone.PhoneNumber + (!string.IsNullOrEmpty(newsDeskPhone.PhoneNumberExtension) ? " ext. " + newsDeskPhone.PhoneNumberExtension : "")), DataType = new EnumValue<CellValues>(CellValues.String) });

                            row.Append(new Cell() { CellValue = new CellValue(outlet.Email), DataType = new EnumValue<CellValues>(CellValues.String) });

                            row.Append(new Cell() { CellValue = new CellValue(outlet.FirstMediaType), DataType = new EnumValue<CellValues>(CellValues.String) });
                            //row.Append(new Cell() { CellValue = new CellValue(outlet.Twitter), DataType = new EnumValue<CellValues>(CellValues.String) });

                            sheetData.Append(row);
                        }

                        worksheet.Append(sheetData);
                    }

                    //Company
                    {
                        WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

                        Worksheet worksheet = new Worksheet();
                        worksheetPart.Worksheet = worksheet;

                        SheetData sheetData = new SheetData();

                        Sheet sheet = new Sheet()
                        {
                            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                            SheetId = 3,
                            Name = "Company"
                        };

                        sheets.Append(sheet);

                        Columns columns = new Columns();
                        columns.Append(new Column() { Min = 1, Max = 1, Width = 60, CustomWidth = true });
                        columns.Append(new Column() { Min = 2, Max = 2, Width = 25, CustomWidth = true });
                        columns.Append(new Column() { Min = 3, Max = 3, Width = 20, CustomWidth = true });
                        columns.Append(new Column() { Min = 4, Max = 4, Width = 40, CustomWidth = true });
                        columns.Append(new Column() { Min = 5, Max = 5, Width = 40, CustomWidth = true });
                        worksheetPart.Worksheet.Append(columns);

                        Row headerRow = new Row();
                        headerRow.Append(new Cell() { CellValue = new CellValue("Name"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("City"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("Primary"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("Email"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        headerRow.Append(new Cell() { CellValue = new CellValue("Twitter"), DataType = new EnumValue<CellValues>(CellValues.String) });
                        sheetData.Append(headerRow);

                        foreach (CompanySearchResult company in companyResults)
                        {
                            Row row = new Row();

                            row.Append(new Cell() { CellValue = new CellValue(company.CompanyName), DataType = new EnumValue<CellValues>(CellValues.String) });
                            //row.Append(new Cell() { CellValue = new CellValue(company.ParentCompanyName), DataType = new EnumValue<CellValues>(CellValues.String) });
                            row.Append(new Cell() { CellValue = new CellValue(company.City), DataType = new EnumValue<CellValues>(CellValues.String) });
                            row.Append(new Cell() { CellValue = new CellValue(company.PrimaryPhone + (!string.IsNullOrEmpty(company.PrimaryPhoneExtension) ? " ext. " + company.PrimaryPhoneExtension : "")), DataType = new EnumValue<CellValues>(CellValues.String) });
                            row.Append(new Cell() { CellValue = new CellValue(company.Email), DataType = new EnumValue<CellValues>(CellValues.String) });
                            row.Append(new Cell() { CellValue = new CellValue(company.Twitter), DataType = new EnumValue<CellValues>(CellValues.String) });

                            sheetData.Append(row);
                        }
                        worksheet.Append(sheetData);
                    }
                }
            }
            buffer = memoryStream.ToArray();

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"Media Relations Contacts " + DateTime.Now.ToString("MMMM d, yyyy") + ".xlsx\"");
            Response.Clear();
            Response.BinaryWrite(buffer);
            Response.End();

            WriteActivityLogEntry(CommonEventLogging.ActivityType.Export, CommonEventLogging.EntityType.Search,
                Guid.Empty, "", Guid.Empty, advancedSearchControlDesktop.SearchCriteriaQueryUrl, CommonMethods.GetLoggedInUser());
        }
    }

    public void ShareButton_Click(object sender, EventArgs e)
    {
        shareSuccessLit.Text = "";

        using (var ctx = new MediaRelationsEntities())
        {
            DoSearch(ctx);
        }

        string email = Permissions.GetEmailForUser(CommonMethods.GetLoggedInUser());
        if (email != null)
        {
            WriteActivityLogEntry(CommonEventLogging.ActivityType.Share, CommonEventLogging.EntityType.Search,
                Guid.Empty, "", Guid.Empty, advancedSearchControlDesktop.SearchCriteriaQueryUrl, CommonMethods.GetLoggedInUser());

            CommonMethods.SendEmail(Settings.Default.FromEmailAddress, email, "Media Relations Search Results Share", shareSb.ToString(), true);
            shareSuccessLit.Text = "<script type='text/javascript'>alert(sentShareEmailText.replace(\"###email###\", \"" + email.Replace("\"", "\\\"") + "\"));</script>\n";
        }
    }

    private void SetupContactTab(MediaRelationsEntities ctx, IQueryable<ContactSearchResult> contacts, List<KeyValuePair<SearchLib.CriteriaType, string>> criteria, bool doDisplay, List<CompanySearchResult> outlets = null)
    {
        using (MediaRelationsEntities db = new MediaRelationsEntities())
        {
            DateTime startDt = DateTime.Now;

            string sort = null;
            string sortDir = null;
            sort = Request.QueryString["sort"];
            sortDir = Request.QueryString["sortDir"];
            if (sort == null) sort = "Name";
            switch (sort)
            {
                case "Name":
                    sort = "ContactName";
                    if (sortDir == null) sortDir = "asc";
                    if (sortDir.Equals("asc")) contacts = contacts.OrderBy(t => t.ContactName);
                    else contacts = contacts.OrderByDescending(t => t.ContactName);
                    break;
                case "City":
                    sort = "City";
                    if (sortDir == null) sortDir = "asc";
                    if (sortDir.Equals("asc")) contacts = contacts.OrderBy(t => t.City);
                    else contacts = contacts.OrderByDescending(t => t.City);
                    break;
                case "Email":
                    sort = "Email";
                    if (sortDir == null) sortDir = "asc";
                    if (sortDir.Equals("asc")) contacts = contacts.OrderBy(t => t.Email);
                    else contacts = contacts.OrderByDescending(t => t.Email);
                    break;
                case "Ministry":
                    sort = "Ministry";
                    if (sortDir == null) sortDir = "asc";
                    if (sortDir.Equals("asc"))
                    {
                        contacts = contacts.OrderBy(t => t.Ministry == null).ThenBy(t => t.Ministry);
                    }
                    else
                    {
                        contacts = contacts.OrderByDescending(t => t.Ministry).ThenBy(t => t.Ministry == null);
                    }
                    break;
                case "Ministerial Job Title":
                    sort = "MinisterialJobTitle";
                    if (sortDir == null) sortDir = "asc";
                    if (sortDir.Equals("asc")) contacts = contacts.OrderBy(t => t.MinisterialJobTitle);
                    else contacts = contacts.OrderByDescending(t => t.MinisterialJobTitle);
                    break;
                default:
                    sort = "ContactName";
                    sortDir = "asc";
                    if (sortDir.Equals("asc")) contacts = contacts.OrderBy(t => t.ContactName);
                    else contacts = contacts.OrderByDescending(t => t.ContactName);
                    break;
            }
            //contacts = LinqDataMethods.OrderBy(contacts, sort, (sortDir!=null&&sortDir.Equals("desc")), false);

            contactResults = contacts.ToList();

            int contactCount = contactResults.Count;

            if (currentTab == Tabs.Contacts)
            {
                //contactsDisplayPanel.CssClass = "view-panel-show";
                tabControl.SelectedTab = contactCount + tabSpacing + "Contact" + (contactCount == 1 ? "" : "s");
            }

            Guid guid;
            if (Guid.TryParse(Request.QueryString["deleteContact"], out guid))
            {
                string message = ContactAdminLib.DeleteContact(guid);
                if (message == null) message = "Contact deleted";

                List<KeyValuePair<string, string>> queryString = CommonMethods.GetEditableQueryStringMultiple();
                CommonMethods.RemoveItemFromQueryString(queryString, "message");
                CommonMethods.RemoveItemFromQueryString(queryString, "deleteContact");
                queryString.Add(new KeyValuePair<string, string>("message", message));
                Response.Redirect(Request.Path + "?" + CommonMethods.GetQueryStringMultiple(queryString));
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder psb = new StringBuilder();

            topContactsPaginator.Count = bottomContactsPaginator.Count = contactResults.Count;

            if (!doDisplay) return;
            contactsDisplayPanel.Visible = true;

            int start = topContactsPaginator.Skip;
            int end = start + topContactsPaginator.PerPage;

            SearchLib slib = new SearchLib();

            shareSb.Append("<h2 style='" + headingStyle + "'>" + contactResults.Count + " Contact" + (contactResults.Count == 1 ? "" : "s") + "</h2>\n\n");

            if (contactResults.Count > 0)
            {
                var Contacts = db.Contacts.Include("Ministry").Include("ContactMediaJobTitles").Include("ContactPhoneNumbers").Include("ContactWebAddresses").ToArray();
                var ContactMediaJobTitles = db.ContactMediaJobTitles.ToArray();
                //var Companies = db.Companies.Include("Contacts").ToArray();
                string externalUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                string contactRoot = ResolveUrl("~/Contacts/");

                bool odd = false;
                for (int i = 0; i < contactResults.Count; i++)
                {
                    ContactSearchResult result = contactResults[i];

                    var contact = Contacts.Single(c => c.Id == result.Contact.Id);

                    string matchStr = null;
                    if (!advancedSearchControlDesktop.MatchAll) matchStr = slib.GetMatchString(result, criteria, outlets);

                    //List<ContactMediaJobTitle> titles = contact.ContactMediaJobTitles.ToList();
                    //List<Company> contactCompanies = contact.Companies.ToList();

                    //var titles = ContactMediaJobTitles.Where(e => e.ContactId == contact.Id);
                    //var contactCompanies = Companies.Where(e => e.Contacts.Any(c => c.Id == contact.Id));

                    // desktop display
                    if (i >= start && i < end)
                    {
                        sb.Append("<tr class='" + (advancedSearchControlDesktop.MatchAll || advancedSearchControlDesktop.SearchCriteria.Count == 0 ? "" : "result-top ") + (i % 2 == 0 ? "even" : "odd") + "'>\n");
                        if (canDeleteContacts) sb.Append("<td><input type=\"checkbox\" name=\"categoryAction\" value=\"" + contact.Id + "\"></td>\n");
                        sb.Append("<td>" + result.FirstName + " " + result.LastName + "</td>\n");

                        sb.Append("<td>");
                        if (contact.Ministry == null && contact.ContactMediaJobTitles.Count() > 0)
                        {
                            bool first = true;
                            foreach (ContactMediaJobTitle jobTitle in contact.ContactMediaJobTitles)
                            {
                                if (!first) sb.Append("<br/>");
                                sb.Append("<a href='" + contactRoot + "MediaOutlet/ViewOutlet.aspx?guid=" + jobTitle.CompanyId + "&from=search'>" + jobTitle.Company.CompanyName + "</a>");

                                List<ContactMediaJobTitle> mJobTitles = contact.ContactMediaJobTitles.Where(x => x.CompanyId == jobTitle.CompanyId).ToList();
                                string mediaJobTitles = "";

                                foreach (ContactMediaJobTitle title in mJobTitles)
                                {
                                    if (!string.IsNullOrEmpty(mediaJobTitles)) mediaJobTitles += ", ";
                                    mediaJobTitles += title.MediaJobTitle.MediaJobTitleName;
                                }

                                if (!string.IsNullOrEmpty(mediaJobTitles))
                                    sb.Append(" (" + mediaJobTitles + ")");

                                first = false;
                            }
                        }
                        else
                        {
                            sb.Append(contact.Ministry == null ? "&mdash;" : contact.Ministry.DisplayName);
                        }
                        sb.Append("</td>\n");

                        sb.Append("<td>" + (string.IsNullOrWhiteSpace(result.City) ? "&mdash;" : result.City) + "</td>\n");

                        sb.Append("<td>");

                        string phoneString = "";

                        if (result.PrimaryPhone != null)
                        {
                            phoneString += "p: " + MultiSelectorItem.GetPhoneNumberLink(result.PrimaryPhone);
                            if (!string.IsNullOrEmpty(result.PrimaryPhoneExtension)) phoneString += " ext. " + result.PrimaryPhoneExtension;
                        }
                        ContactPhoneNumber cellPhone = contact.CellPhone;
                        if (cellPhone != null)
                        {
                            phoneString += (!string.IsNullOrWhiteSpace(phoneString) ? "<br/>" : "") + "c: " + MultiSelectorItem.GetPhoneNumberLink(cellPhone.PhoneNumber);
                            if (!string.IsNullOrEmpty(cellPhone.PhoneNumberExtension)) phoneString += " ext. " + cellPhone.PhoneNumberExtension;
                        }

                        sb.Append(string.IsNullOrWhiteSpace(phoneString) ? "&mdash;" : phoneString);

                        sb.Append("</td>\n");


                        sb.Append("<td>" + (result.Email != null ? MultiSelectorItem.GetEmailLink(result.Email) : "&mdash;") + "</td>\n");
                        //sb.Append("<td>" + (result.Twitter != null ? MultiSelectorItem.GetTwitterLink(result.Twitter) : "&mdash;") + "</td>\n");
                        sb.Append("<td>" + (string.IsNullOrWhiteSpace(result.MinisterialJobTitle) ? "&mdash;" : result.MinisterialJobTitle) + "</td>\n");

                        string actionstring = "<a href='" + contactRoot + "Contact/ViewContact.aspx?guid=" + contact.Id + "&from=search'>View</a>\n";

                        Permissions.SiteAction action = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsContact);

                        if ((action & Permissions.SiteAction.Update) != 0)
                        {
                            actionstring += "| <a href='" + contactRoot + "Contact/EditContact.aspx?guid=" + contact.Id + "'>Edit</a>\n";
                        }

                        if ((action & Permissions.SiteAction.Delete) != 0)
                        {
                            List<KeyValuePair<string, string>> qs = CommonMethods.GetEditableQueryStringMultiple();
                            qs.Add(new KeyValuePair<string, string>("deleteContact", contact.Id.ToString()));
                            actionstring += "| <a href='" + Request.Path + "?" + CommonMethods.GetQueryStringMultiple(qs) + "' onclick='return confirm(deleteButtonText)'>Delete</a>\n";
                        }

                        sb.Append("<td>" + actionstring + "</td>\n");

                        sb.Append("</tr>\n");

                        if (!advancedSearchControlDesktop.MatchAll && advancedSearchControlDesktop.SearchCriteria.Count > 0) sb.Append("<tr class='result-bottom " + (i % 2 == 0 ? "even" : "odd") + "' ><td>&nbsp;</td><td colspan='8'>Matched: " + matchStr + "</td></tr>\n");
                    }

                    contactsTableLit.Text = sb.ToString();

                    // phone display
                    psb.Append("<div class='search-result-item " + (odd ? "odd" : "even") + "'>\n");

                    psb.Append("<div class='search-result-info'>\n");

                    string contactHref = contactRoot + "Contact/ViewContact.aspx?guid=" + contact.Id;
                    psb.Append("<div class='row'><div class='search-label'>Name:</div><div class='search-value'><a href='" + contactHref + "&from=search'>" + result.FirstName + " " + result.LastName + "</a></div></div>\n");
                    shareSb.Append("<div style='font-weight:bold;margin-top:10px;'><a href='" + externalUrl + contactHref + "'>" + result.FirstName + " " + result.LastName + "</a></div>\n");

                    if (contact.Ministry == null)
                    {
                        psb.Append("<div class='row'>\n<div class='search-label'>Company:</div>\n");
                        psb.Append("<div class='search-value'>\n");
                        shareSb.Append("<div>Company: ");

                        if (contact.ContactMediaJobTitles.Count() > 0)
                        {
                            bool frst = true;
                            foreach (ContactMediaJobTitle jobTitle in contact.ContactMediaJobTitles)
                            {
                                if (!frst)
                                {
                                    psb.Append("<br/>\n");
                                    shareSb.Append("<br/>\n");
                                }
                                frst = false;
                                string outletHref = contactRoot + "MediaOutlet/ViewOutlet.aspx?guid=" + jobTitle.CompanyId;
                                psb.Append("<a href='" + outletHref + "&from=search'>" + jobTitle.Company.CompanyName + "</a>");
                                shareSb.Append("<a href='" + externalUrl + outletHref + "'>" + jobTitle.Company.CompanyName + "</a>");

                                List<ContactMediaJobTitle> mJobTitles = contact.ContactMediaJobTitles.Where(x => x.CompanyId == jobTitle.CompanyId).ToList();
                                string mediaJobTitles = "";

                                foreach (ContactMediaJobTitle title in mJobTitles)
                                {
                                    if (!string.IsNullOrEmpty(mediaJobTitles)) mediaJobTitles += ", ";
                                    mediaJobTitles += title.MediaJobTitle.MediaJobTitleName;
                                }

                                if (!string.IsNullOrEmpty(mediaJobTitles))
                                {
                                    mediaJobTitles = " (" + mediaJobTitles + ")";
                                    psb.Append(mediaJobTitles);
                                    shareSb.Append(mediaJobTitles);
                                }
                            }
                        }
                        else
                        {
                            psb.Append("&mdash");
                        }

                        psb.Append("</div>\n</div>\n");
                        shareSb.Append("</div>\n");
                    }
                    else
                    {
                        psb.Append("<div class='row'>\n<div class='search-label'>Ministry:</div>\n");
                        shareSb.Append("<div>Ministry: ");

                        psb.Append("<div class='search-value'>" + contact.Ministry.DisplayName + "</div>\n");
                        shareSb.Append(contact.Ministry.DisplayName);

                        psb.Append("</div>\n");
                        shareSb.Append("</div>\n");
                    }

                    psb.Append("<div class='row'>\n<div class='search-label'>Primary:</div>\n");
                    shareSb.Append("<div>Primary: ");

                    //ContactPhoneNumber primary = contact.ContactPhoneNumbers.Where(p => p.PhoneType.PhoneTypeId == CommonMethods.PrimaryPhoneNumberPhoneType.PhoneTypeId).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(result.PrimaryPhone))
                    {
                        psb.Append("<div class='search-value'>" + MultiSelectorItem.GetPhoneNumberLink(result.PrimaryPhone) + (!string.IsNullOrEmpty(result.PrimaryPhoneExtension) ? " ext. " + result.PrimaryPhoneExtension : "") + "</div>\n");
                        shareSb.Append(MultiSelectorItem.GetPhoneNumberLink(result.PrimaryPhone) + (!string.IsNullOrEmpty(result.PrimaryPhoneExtension) ? " ext. " + result.PrimaryPhoneExtension : ""));
                    }
                    else
                    {
                        psb.Append("<div class='search-value'>&mdash;</div>\n");
                    }

                    psb.Append("</div>\n");
                    shareSb.Append("</div>\n");

                    psb.Append("<div class='row'>\n<div class='search-label'>Cell:</div>\n");
                    shareSb.Append("<div>Cell: ");

                    ContactPhoneNumber cell = contact.CellPhone;
                    if (cell != null)
                    {
                        psb.Append("<div class='search-value'>" + MultiSelectorItem.GetPhoneNumberLink(cell.PhoneNumber) + (!string.IsNullOrWhiteSpace(cell.PhoneNumberExtension) ? " ext. " + cell.PhoneNumberExtension : "") + "</div>\n");
                        shareSb.Append(MultiSelectorItem.GetPhoneNumberLink(cell.PhoneNumber) + (!string.IsNullOrWhiteSpace(cell.PhoneNumberExtension) ? " ext. " + cell.PhoneNumberExtension : ""));
                    }
                    else
                    {
                        psb.Append("<div class='search-value'>&mdash;</div>\n");
                    }

                    psb.Append("</div>\n");
                    shareSb.Append("</div>\n");

                    psb.Append("<div class='row'>\n<div class='search-label'>Email:</div>\n");
                    shareSb.Append("<div>Email: ");

                    ContactWebAddress email = contact.Email;
                    if (email != null)
                    {
                        psb.Append("<div class='search-value'>" + MultiSelectorItem.GetEmailLink(email.WebAddress) + "</div>\n");
                        shareSb.Append(MultiSelectorItem.GetEmailLink(email.WebAddress));
                    }
                    else
                    {
                        psb.Append("<div class='search-value'>&mdash;</div>\n");
                    }

                    psb.Append("</div>\n");
                    shareSb.Append("</div>\n");

                    psb.Append("<div class='row'>\n<div class='search-label'>Ministerial Job Title:</div>\n");
                    shareSb.Append("<div>Ministerial Job Title: ");

                    if (!string.IsNullOrWhiteSpace(result.MinisterialJobTitle))
                    {
                        psb.Append("<div class='search-value'>" + result.MinisterialJobTitle + "</div>\n");
                        shareSb.Append(result.MinisterialJobTitle);
                    }
                    else
                    {
                        psb.Append("<div class='search-value'>&mdash;</div>\n");
                    }

                    psb.Append("</div>\n");
                    shareSb.Append("</div>\n");


                    if (!advancedSearchControlDesktop.MatchAll && advancedSearchControlDesktop.SearchCriteria.Count > 0)
                    {
                        psb.Append("<div class='row'><div class='search-label'>Matched:</div><div class='search-value'>" + matchStr + "</div></div>\n");
                        shareSb.Append("<div>+Matched: " + matchStr + "</div>\n");
                    }

                    psb.Append("</div>\n");

                    psb.Append("<div class='result-arrow'><a href='" + contactRoot + "Contact/ViewContact.aspx?guid=" + contact.Id + "'>" +
                        "<img src='" + contactRoot + "images/BlueArrow@x2.png' border='0'/>" +
                        "</a></div>\n");

                    psb.Append("</div>\n");

                    if (!string.IsNullOrWhiteSpace(result.Email))
                    {
                        emailCount++;
                        emailSb.Append(result.Email + ";");
                    }
                    odd = !odd;
                }
                contactsPhoneDisplayLit.Text = psb.ToString();
            }
            else
            {
                contactsTableLit.Text = "<tr><td colspan='9'>No items to display</td></tr>\n";
                contactsPhoneDisplayLit.Text = "<div class='row no-items-found'>No items to display</div>";
            }
        }
    }

    private void SetupOutletsTab(MediaRelationsEntities ctx, IQueryable<CompanySearchResult> outlets, List<KeyValuePair<SearchLib.CriteriaType, string>> criteria, bool doDisplay, List<CompanySearchResult> companies = null)
    {
        string sort = null;
        string sortDir = null;
        sort = Request.QueryString["sort"];
        sortDir = Request.QueryString["sortDir"];
        if (sort == null) sort = "Name";
        switch (sort)
        {
            case "Name":
                sort = "CompanyName";
                if (sortDir == null) sortDir = "asc";
                if (sortDir.Equals("asc")) outlets = outlets.OrderBy(x => x.CompanyName);
                else outlets.OrderByDescending(x => x.CompanyName);
                break;
            case "Company":
                sort = "ParentCompanyName";
                if (sortDir == null) sortDir = "asc";
                if (sortDir.Equals("asc")) outlets = outlets.OrderBy(x => x.ParentCompanyName);
                else outlets = outlets.OrderByDescending(x => x.ParentCompanyName);
                break;
            case "City":
                sort = "City";
                if (sortDir == null) sortDir = "asc";
                if (sortDir.Equals("asc")) outlets = outlets.OrderBy(x => x.City);
                else outlets = outlets.OrderByDescending(x => x.City);
                break;
            case "Email":
                sort = "Email";
                if (sortDir == null) sortDir = "asc";
                if (sortDir.Equals("asc")) outlets = outlets.OrderBy(x => x.Email);
                else outlets.OrderByDescending(x => x.Email);
                break;
            case "Media Type":
                sort = "Media Type";
                if (sortDir == null) sortDir = "asc";
                if (sortDir.Equals("asc")) outlets = outlets.OrderBy(x => x.FirstMediaType);
                else outlets = outlets.OrderByDescending(x => x.FirstMediaType);
                break;
            default:
                sort = "CompanyName";
                sortDir = "asc";
                if (sortDir.Equals("asc")) outlets = outlets.OrderBy(x => x.CompanyName);
                else outlets.OrderByDescending(x => x.CompanyName);
                break;
        }
        //outlets = LinqDataMethods.OrderBy(outlets, sort, (sortDir!=null&&sortDir.Equals("desc")), false);

        outletResults = outlets.ToList();
        int outletCount = outletResults.Count;

        if (currentTab == Tabs.Outlets)
        {
            //outletDisplayPanel.CssClass = "view-panel-show";
            tabControl.SelectedTab = outletCount + tabSpacing + "Outlet" + (outletCount == 1 ? "" : "s");
        }

        Guid guid;
        if (Guid.TryParse(Request.QueryString["deleteOutlet"], out guid))
        {
            string error = CompanyAdministrationLib.DeleteCompany(guid);
            string message = error == null ? "Outlet deleted" : "Error deleting outlet: " + error;

            List<KeyValuePair<string, string>> queryString = CommonMethods.GetEditableQueryStringMultiple();
            CommonMethods.RemoveItemFromQueryString(queryString, "message");
            CommonMethods.RemoveItemFromQueryString(queryString, "deleteOutlet");
            queryString.Add(new KeyValuePair<string, string>("message", message));
            Response.Redirect(Request.Path + "?" + CommonMethods.GetQueryStringMultiple(queryString));
        }

        StringBuilder sb = new StringBuilder();
        StringBuilder psb = new StringBuilder();

        TopOutletsPaginator.Count = BottomOutletsPaginator.Count = outletResults.Count;

        if (!doDisplay) return;
        outletDisplayPanel.Visible = true;

        int start = TopOutletsPaginator.Skip;
        int end = start + TopOutletsPaginator.PerPage;

        SearchLib slib = new SearchLib();

        shareSb.Append("<h2 style='" + headingStyle + "'>" + outletResults.Count + " Outlet" + (outletResults.Count == 1 ? "" : "s") + "</h2>\n\n");

        if (outletResults.Count > 0)
        {
            bool odd = false;
            string externalUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            string contactRoot = ResolveUrl("~/Contacts/");
            for (int i = 0; i < outletResults.Count; i++)
            {
                CompanySearchResult result = outletResults[i];

                string matchStr = null;
                if (!advancedSearchControlDesktop.MatchAll) matchStr = slib.GetMatchString(result, criteria, companies);

                // common string builders for outlets
                string mediaTypeString = "";
                if (result.Company.MediaTypes.Count > 0)
                {
                    bool first = true;
                    foreach (MediaType type in result.Company.MediaTypes)
                    {
                        if (!first) mediaTypeString += "<br/>";
                        mediaTypeString += type.MediaTypeName;
                        first = false;
                    }
                }

                CompanyPhoneNumber newsDeskPhone = result.Company.NewsDeskPhone;

                // desktop display
                if (i >= start && i < end)
                {
                    sb.Append("<tr class='" + (advancedSearchControlDesktop.MatchAll || advancedSearchControlDesktop.SearchCriteria.Count == 0 ? "" : "result-top ") + (i % 2 == 0 ? "even" : "odd") + "'>\n");
                    if (canDeleteCompany) sb.Append("<td><input type=\"checkbox\" name=\"categoryAction\" value=\"" + result.Company.Id + "\"></td>\n");
                    sb.Append("<td>" + result.CompanyName + "</td>\n");

                    sb.Append("<td>" + (string.IsNullOrWhiteSpace(result.ParentCompanyName) ? "&mdash;" : result.ParentCompanyName) + "</td>\n");

                    sb.Append("<td>" + (string.IsNullOrWhiteSpace(result.City) ? "&mdash;" : result.City) + "</td>\n");


                    sb.Append("<td>");

                    string phoneString = "";

                    if (newsDeskPhone != null)
                    {
                        phoneString += MultiSelectorItem.GetPhoneNumberLink(newsDeskPhone.PhoneNumber);

                        if (!string.IsNullOrWhiteSpace(newsDeskPhone.PhoneNumberExtension))
                            phoneString += " ext. " + newsDeskPhone.PhoneNumberExtension;
                    }

                    sb.Append(string.IsNullOrWhiteSpace(phoneString) ? "&mdash;" : phoneString);

                    sb.Append("</td>\n");

                    sb.Append("<td>" + (result.Email != null ? MultiSelectorItem.GetEmailLink(result.Email) : "&mdash;") + "</td>\n");

                    // media types
                    sb.Append("<td>");

                    sb.Append(string.IsNullOrWhiteSpace(mediaTypeString) ? "&mdash;" : mediaTypeString);

                    sb.Append("</td>\n");

                    string actionstring = "<a href='" + contactRoot + (result.Company.IsOutlet ? "MediaOutlet" : "Company") + "/View" + (result.Company.IsOutlet ? "Outlet" : "Company") + ".aspx?guid=" + result.Company.Id + "&from=search'>View</a>\n";

                    Permissions.SiteAction action = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsContact);

                    if ((action & Permissions.SiteAction.Update) != 0)
                    {
                        actionstring += "| <a href='" + contactRoot + (result.Company.IsOutlet ? "MediaOutlet" : "Company") + "/Edit" + (result.Company.IsOutlet ? "Outlet" : "Company") + ".aspx?guid=" + result.Company.Id + "'>Edit</a>\n";
                    }

                    if ((action & Permissions.SiteAction.Delete) != 0)
                    {
                        List<KeyValuePair<string, string>> qs = CommonMethods.GetEditableQueryStringMultiple();
                        qs.Add(new KeyValuePair<string, string>("deleteOutlet", result.Company.Id.ToString()));
                        actionstring += "| <a href='" + Request.Path + "?" + CommonMethods.GetQueryStringMultiple(qs) + "' onclick='return confirm(deleteButtonText)'>Delete</a>\n";
                    }

                    sb.Append("<td>" + actionstring + "</td>\n");

                    sb.Append("</tr>\n");

                    if (!advancedSearchControlDesktop.MatchAll && advancedSearchControlDesktop.SearchCriteria.Count > 0) sb.Append("<tr class='result-bottom " + (i % 2 == 0 ? "even" : "odd") + "'><td>&nbsp;</td><td colspan='7'>Matched: " + matchStr + "</td></tr>\n");
                }

                outletsTableLit.Text = sb.ToString();

                // phone display
                psb.Append("<div class='search-result-item " + (odd ? "odd" : "even") + "'>\n");

                psb.Append("<div class='search-result-info'>\n");

                string outletHref = contactRoot + "MediaOutlet/ViewOutlet.aspx?guid=" + result.Company.Id;
                psb.Append("<div class='row'><div class='search-label'>Name:</div><div class='search-value'><a href='" + outletHref + "&from=search'>" + result.CompanyName + "</a></div></div>\n");
                shareSb.Append("<div style='font-weight:bold;margin-top:10px;'><a href='" + externalUrl + outletHref + "'>" + result.CompanyName + "</a></div>\n\n");

                string companyHref = contactRoot + "Company/ViewCompany.aspx?guid=" + result.Company.ParentCompanyId;
                psb.Append("<div class='row'>\n<div class='search-label'>Company:</div>\n<div class='search-value'>" +
                    (!string.IsNullOrEmpty(result.ParentCompanyName) ? "<a href='" + companyHref + "&from=search'>" : "&mdash;") +
                result.ParentCompanyName
                    + (!string.IsNullOrEmpty(result.ParentCompanyName) ? "</a>" : "")
                    + "</div>\n</div>\n");

                shareSb.Append("<div>Company: "
                    + (!string.IsNullOrEmpty(result.ParentCompanyName) ? "<a href='" + externalUrl + companyHref + "'>" : "&mdash;")
                    + result.ParentCompanyName
                    + (!string.IsNullOrEmpty(result.ParentCompanyName) ? "</a>" : "")
                    + "</div>\n");

                psb.Append("<div class='row'>\n<div class='search-label'>City:</div>\n<div class='search-value'>" + (!string.IsNullOrWhiteSpace(result.City) ? result.City : "&mdash;") + "</div>\n</div>\n");
                shareSb.Append("<div>City: " + result.City + "</div>\n");

                psb.Append("<div class='row'>\n<div class='search-label'>News Desk:</div>\n");
                shareSb.Append("<div>News Desk: ");

                if (newsDeskPhone != null)
                {
                    psb.Append("<div class='search-value'>" + MultiSelectorItem.GetPhoneNumberLink(newsDeskPhone.PhoneNumber) + (!string.IsNullOrWhiteSpace(newsDeskPhone.PhoneNumberExtension) ? " ext. " + newsDeskPhone.PhoneNumberExtension : "") + "</div>\n");
                    shareSb.Append(MultiSelectorItem.GetPhoneNumberLink(newsDeskPhone.PhoneNumber) + (!string.IsNullOrWhiteSpace(newsDeskPhone.PhoneNumberExtension) ? " ext. " + newsDeskPhone.PhoneNumberExtension : ""));
                }
                else
                {
                    psb.Append("<div class='search-value'>&mdash;</div>\n");
                }

                psb.Append("</div>\n");
                shareSb.Append("</div>\n");

                CompanyWebAddress email = result.Company.Email;
                psb.Append("<div class='row'>\n<div class='search-label'>Email:</div>\n");
                shareSb.Append("<div>Email: ");

                if (email != null)
                {
                    psb.Append("<div class='search-value'>" + MultiSelectorItem.GetEmailLink(email.WebAddress) + "</div>\n");
                    shareSb.Append(MultiSelectorItem.GetEmailLink(email.WebAddress));
                }
                else
                {
                    psb.Append("<div class='search-value'>&mdash;</div>\n");
                }

                psb.Append("</div>\n");
                shareSb.Append("</div>\n");

                psb.Append("<div class='row'>\n<div class='search-label'>Media Type:</div>\n");
                shareSb.Append("<div>Media Type: ");

                if (!string.IsNullOrWhiteSpace(mediaTypeString))
                {
                    psb.Append("<div class='search-value'>" + mediaTypeString + "</div>\n");
                    shareSb.Append(mediaTypeString.Replace("<br/>", ", "));
                }
                else
                {
                    psb.Append("<div class='search-value'>&mdash;</div>\n");
                }

                psb.Append("</div>\n");
                shareSb.Append("</div>\n");

                if (!advancedSearchControlDesktop.MatchAll && advancedSearchControlDesktop.SearchCriteria.Count > 0)
                {
                    psb.Append("<div class='row'><div class='search-label'>Matched:</div><div class='search-value'>" + matchStr + "</div></div>\n");
                    shareSb.Append("<div>Matched: " + matchStr + "</div>\n");
                }
                psb.Append("</div>\n");


                psb.Append("<div class='result-arrow'><a href='" + contactRoot + "MediaOutlet/ViewOutlet.aspx?guid=" + result.Company.Id + "'>" +
                    "<img src='" + contactRoot + "images/BlueArrow@x2.png' border='0'/>" +
                    "</a></div>\n");

                psb.Append("</div>\n");

                shareSb.Append("\n");

                if (!string.IsNullOrWhiteSpace(result.Email))
                {
                    emailCount++;
                    emailSb.Append(result.Email + ";");
                }
                odd = !odd;
            }
            outletsPhoneDisplayLit.Text = psb.ToString();
        }
        else
        {
            outletsTableLit.Text = "<tr><td colspan='9'>No items to display</td></tr>\n";
            outletsPhoneDisplayLit.Text = "<div class='row no-items-found'>No items to display</div>";
        }
    }

    private void SetupCompanyTab(MediaRelationsEntities ctx, IQueryable<CompanySearchResult> companies, List<KeyValuePair<SearchLib.CriteriaType, string>> criteria, bool doDisplay)
    {
        string sort = null;
        string sortDir = null;
        sort = Request.QueryString["sort"];
        sortDir = Request.QueryString["sortDir"];
        if (sort == null) sort = "Name";
        switch (sort)
        {
            case "Name":
                sort = "CompanyName";
                if (sortDir == null) sortDir = "asc";
                break;
            case "City":
                sort = "City";
                if (sortDir == null) sortDir = "asc";
                break;
            case "Email":
                sort = "Email";
                if (sortDir == null) sortDir = "asc";
                break;
            case "Twitter":
                sort = "Twitter";
                if (sortDir == null) sortDir = "asc";
                break;
            default:
                sort = "CompanyName";
                sortDir = "asc";
                break;
        }
        companies = LinqDataMethods.OrderBy(companies, sort, (sortDir != null && sortDir.Equals("desc")), false);

        companyResults = companies.ToList();

        int companyCount = companyResults.Count;

        if (currentTab == Tabs.Companies)
        {
            //companyDisplayPanel.CssClass = "view-panel-show";
            tabControl.SelectedTab = companyCount + tabSpacing + "Compan" + (companyCount == 1 ? "y" : "ies");
        }

        Guid guid;
        if (Guid.TryParse(Request.QueryString["deleteCompany"], out guid))
        {
            string error = CompanyAdministrationLib.DeleteCompany(guid);
            string message = error == null ? "Company deleted" : "Error deleting company: " + error;

            List<KeyValuePair<string, string>> queryString = CommonMethods.GetEditableQueryStringMultiple();
            CommonMethods.RemoveItemFromQueryString(queryString, "message");
            CommonMethods.RemoveItemFromQueryString(queryString, "deleteCompany");
            queryString.Add(new KeyValuePair<string, string>("message", message));
            Response.Redirect(Request.Path + "?" + CommonMethods.GetQueryStringMultiple(queryString));
        }

        StringBuilder sb = new StringBuilder();
        StringBuilder psb = new StringBuilder();

        topCompanyPaginator.Count = bottomCompanyPaginator.Count = companyCount;

        if (!doDisplay) return;
        companyDisplayPanel.Visible = true;

        int start = topCompanyPaginator.Skip;
        int end = start + topCompanyPaginator.PerPage;

        SearchLib slib = new SearchLib();

        shareSb.Append("<h2 style='" + headingStyle + "'>" + companyResults.Count + " Compan" + (companyResults.Count == 1 ? "y" : "ies") + "</h2>\n\n");
        bool odd = false;
        if (companyResults.Count > 0)
        {
            string externalUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            string contactRoot = ResolveUrl("~/Contacts/");
            for (int i = 0; i < companyResults.Count; i++)
            {
                CompanySearchResult result = companyResults[i];

                string matchStr = null;
                if (!advancedSearchControlDesktop.MatchAll) matchStr = slib.GetMatchString(result, criteria);

                // desktop display
                if (i >= start && i < end)
                {
                    sb.Append("<tr class='" + (advancedSearchControlDesktop.MatchAll || advancedSearchControlDesktop.SearchCriteria.Count == 0 ? "" : "result-top ") + (i % 2 == 0 ? "even" : "odd") + "'>\n");
                    if (canDeleteCompany) sb.Append("<td><input type=\"checkbox\" name=\"categoryAction\" value=\"" + result.Company.Id + "\"></td>\n");
                    sb.Append("<td>" + result.CompanyName + "</td>\n");
                    sb.Append("<td>" + (string.IsNullOrWhiteSpace(result.City) ? "&mdash;" : result.City) + "</td>\n");
                    sb.Append("<td>");

                    string phoneString = "";

                    if (result.PrimaryPhone != null)
                    {
                        phoneString += "p: " + MultiSelectorItem.GetPhoneNumberLink(result.PrimaryPhone);
                        if (!string.IsNullOrWhiteSpace(result.PrimaryPhoneExtension)) phoneString += " ext. " + result.PrimaryPhoneExtension;
                    }
                    CompanyPhoneNumber cellPhone = result.Company.CellPhone;
                    if (cellPhone != null)
                    {
                        phoneString += (!string.IsNullOrWhiteSpace(phoneString) ? "<br/>" : "") + "c: " + MultiSelectorItem.GetPhoneNumberLink(cellPhone.PhoneNumber);
                        if (!string.IsNullOrWhiteSpace(cellPhone.PhoneNumberExtension)) phoneString += " ext. " + cellPhone.PhoneNumberExtension;
                    }

                    sb.Append(string.IsNullOrWhiteSpace(phoneString) ? "&mdash;" : phoneString);

                    sb.Append("</td>\n");
                    sb.Append("<td>" + (result.Email != null ? MultiSelectorItem.GetEmailLink(result.Email) : "&mdash;") + "</td>\n");
                    sb.Append("<td>" + (result.Twitter != null ? MultiSelectorItem.GetTwitterLink(result.Twitter) : "&mdash;") + "</td>\n");

                    string actionstring = "<a href='" + contactRoot + (result.Company.IsOutlet ? "MediaOutlet" : "Company") + "/View" + (result.Company.IsOutlet ? "Outlet" : "Company") + ".aspx?guid=" + result.Company.Id + "&from=search'>View</a>\n";

                    Permissions.SiteAction action = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsContact);

                    if ((action & Permissions.SiteAction.Update) != 0)
                    {
                        actionstring += "| <a href='" + contactRoot + (result.Company.IsOutlet ? "MediaOutlet" : "Company") + "/Edit" + (result.Company.IsOutlet ? "Outlet" : "Company") + ".aspx?guid=" + result.Company.Id + "'>Edit</a>\n";
                    }

                    if ((action & Permissions.SiteAction.Delete) != 0)
                    {
                        List<KeyValuePair<string, string>> qs = CommonMethods.GetEditableQueryStringMultiple();
                        qs.Add(new KeyValuePair<string, string>("deleteCompany", result.Company.Id.ToString()));
                        actionstring += "| <a href='" + Request.Path + "?" + CommonMethods.GetQueryStringMultiple(qs) + "' onclick='return confirm(deleteButtonText)'>Delete</a>\n";
                    }

                    sb.Append("<td>" + actionstring + "</td>\n");

                    sb.Append("</tr>\n");

                    if (!advancedSearchControlDesktop.MatchAll && advancedSearchControlDesktop.SearchCriteria.Count > 0) sb.Append("<tr class='result-bottom " + (i % 2 == 0 ? "even" : "odd") + "'><td>&nbsp;</td><td colspan='7'>Matched: " + matchStr + "</td></tr>\n");
                }

                companiesTableLit.Text = sb.ToString();

                // phone display
                psb.Append("<div class='search-result-item " + (odd ? "odd" : "even") + "'>\n");

                psb.Append("<div class='search-result-info'>\n");

                string companyHref = contactRoot + "Company/ViewCompany.aspx?guid=" + result.Company.Id;
                psb.Append("<div class='row'><div class='search-label'>Name:</div><div class='search-value'><a href='" + companyHref + "&from=search'>" + result.CompanyName + "</a></div></div>\n");
                shareSb.Append("<div style='font-weight:bold;margin-top:10px;'><a href='" + externalUrl + companyHref + "'>" + result.CompanyName + "</a></div>\n\n");

                psb.Append("<div class='row'>\n<div class='search-label'>City:</div>\n<div class='search-value'>" + (!string.IsNullOrWhiteSpace(result.City) ? result.City : "&mdash;") + "</div>\n</div>\n");
                shareSb.Append("<div>City: " + result.City + "</div>\n");

                //CompanyPhoneNumber primary = result.Company.CompanyPhoneNumbers.Where(x => x.PhoneTypeId == CommonMethods.PrimaryPhoneNumberPhoneType.PhoneTypeId).FirstOrDefault();
                psb.Append("<div class='row'>\n<div class='search-label'>Primary:</div>\n");
                shareSb.Append("<div>Primary: ");

                if (!string.IsNullOrWhiteSpace(result.PrimaryPhone))
                {
                    psb.Append("<div class='search-value'>" + MultiSelectorItem.GetPhoneNumberLink(result.PrimaryPhone) + (!string.IsNullOrWhiteSpace(result.PrimaryPhoneExtension) ? " ext. " + result.PrimaryPhoneExtension : "") + "</div>\n");
                    shareSb.Append(MultiSelectorItem.GetPhoneNumberLink(result.PrimaryPhone) + (!string.IsNullOrWhiteSpace(result.PrimaryPhoneExtension) ? " ext. " + result.PrimaryPhoneExtension : ""));
                }
                else
                {
                    psb.Append("<div class='search-value'>&mdash;</div>\n");
                }

                psb.Append("</div>\n");
                shareSb.Append("</div>\n");

                CompanyPhoneNumber cell = result.Company.CellPhone;
                psb.Append("<div class='row'>\n<div class='search-label'>Cell:</div>\n");
                shareSb.Append("<div>Cell: ");

                if (cell != null)
                {
                    psb.Append("<div class='search-value'>" + MultiSelectorItem.GetPhoneNumberLink(cell.PhoneNumber) + (!string.IsNullOrWhiteSpace(cell.PhoneNumberExtension) ? " ext. " + cell.PhoneNumberExtension : "") + "</div>\n");
                    shareSb.Append(MultiSelectorItem.GetPhoneNumberLink(cell.PhoneNumber) + (!string.IsNullOrWhiteSpace(cell.PhoneNumberExtension) ? " ext. " + cell.PhoneNumberExtension : ""));
                }
                else
                {
                    psb.Append("<div class='search-value'>&mdash;</div>\n");
                }

                psb.Append("</div>\n");
                shareSb.Append("</div>\n");

                CompanyWebAddress email = result.Company.Email;
                psb.Append("<div class='row'>\n<div class='search-label'>Email:</div>\n");
                shareSb.Append("<div>Email: ");

                if (email != null)
                {
                    psb.Append("<div class='search-value'>" + MultiSelectorItem.GetEmailLink(email.WebAddress) + "</div>\n");
                    shareSb.Append(MultiSelectorItem.GetEmailLink(email.WebAddress));
                }
                else
                {
                    psb.Append("<div class='search-value'>&mdash;</div>\n");
                }

                psb.Append("</div>\n");
                shareSb.Append("</div>\n");

                CompanyWebAddress twitter = result.Company.CompanyWebAddresses.FirstOrDefault(x => x.WebAddressType.WebAddressTypeName == WebAddressType.Twitter);
                psb.Append("<div class='row'>\n<div class='search-label'>Twitter:</div>\n");
                shareSb.Append("<div>Twitter: ");

                if (twitter != null)
                {
                    psb.Append("<div class='search-value'>" + MultiSelectorItem.GetTwitterLink(twitter.WebAddress) + "</div>\n");
                    shareSb.Append(MultiSelectorItem.GetTwitterLink(twitter.WebAddress));
                }
                else
                {
                    psb.Append("<div class='search-value'>&mdash;</div>\n");
                }

                psb.Append("</div>\n");
                shareSb.Append("</div>\n");



                if (!advancedSearchControlDesktop.MatchAll && advancedSearchControlDesktop.SearchCriteria.Count > 0)
                {
                    psb.Append("<div class='row'><div class='search-label'>Matched:</div><div class='search-value'>" + matchStr + "</div></div>\n");
                    shareSb.Append("<div>Matched: " + matchStr + "</div>\n");
                }
                psb.Append("</div>\n");


                psb.Append("<div class='result-arrow'><a href='" + companyHref + "'>" +
                    "<img src='" + contactRoot + "images/BlueArrow@x2.png' border='0'/>" +
                    "</a></div>\n");

                psb.Append("</div>\n");

                shareSb.Append("\n");

                if (!string.IsNullOrWhiteSpace(result.Email))
                {
                    emailCount++;
                    emailSb.Append(result.Email + ";");
                }

                odd = !odd;
            }
            companiesPhoneDisplayLit.Text = psb.ToString();

        }
        else
        {
            companiesTableLit.Text = "<tr><td colspan='9'>No items to display</td></tr>\n";
            companiesPhoneDisplayLit.Text = "<div class='row no-items-found'>No items to display</div>";
        }
    }

    #region bulk action handlers

    private void RedirectUserAfterBulkActions(string selectedAction)
    {
        if (selectedAction != "Bulk Actions")
        {
            List<KeyValuePair<string, string>> qs = CommonMethods.GetEditableQueryStringMultiple();
            CommonMethods.RemoveItemFromQueryString(qs, "message");
            qs.Add(new KeyValuePair<string, string>("message", "Bulk actions have been performed"));
            Response.Redirect(Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryStringMultiple(qs));
        }
    }

    private void PerformCommonCompanyOutletTask(string selectedAction)
    {
        string[] values = Request.Form.GetValues("categoryAction");
        if (values != null)
        {
            foreach (string guidStr in values)
            {
                Guid guid = Guid.Parse(guidStr);
                if (selectedAction.Equals("Delete"))
                {
                    CompanyAdministrationLib.DeleteCompany(guid);
                }
            }
        }
    }

    private void PerformOutletAction(string selectedAction)
    {
        if (selectedAction.Equals("Delete")) PerformCommonCompanyOutletTask("Delete");
        RedirectUserAfterBulkActions(selectedAction);
    }

    public void TopOutletsBulkActionHandler(object sender, EventArgs e)
    {
        PerformOutletAction(TopOutletsPaginator.SelectedBulkAction);
    }

    public void BottomOutletsBulkActionHandler(object sender, EventArgs e)
    {
        PerformOutletAction(BottomOutletsPaginator.SelectedBulkAction);
    }

    private void PerformCompanyAction(string selectedAction)
    {
        if (selectedAction.Equals("Delete")) PerformCommonCompanyOutletTask("Delete");
        RedirectUserAfterBulkActions(selectedAction);
    }

    public void TopCompanyBulkActionsHandler(object sender, EventArgs e)
    {
        PerformCompanyAction(topCompanyPaginator.SelectedBulkAction);
    }

    public void BottomCompanyBulkActionsHandler(object sender, EventArgs e)
    {
        PerformCompanyAction(bottomCompanyPaginator.SelectedBulkAction);
    }

    private void PerformContactAction(string selectedAction)
    {
        string[] values = Request.Form.GetValues("categoryAction");
        if (values != null)
        {
            foreach (string guidStr in values)
            {
                if (selectedAction.Equals("Delete"))
                {
                    Guid guid = Guid.Parse(guidStr);
                    ContactAdminLib.DeleteContact(guid);
                }
            }
        }

        RedirectUserAfterBulkActions(selectedAction);
    }

    public void TopContactsBulkActionHandler(object sender, EventArgs e)
    {
        PerformContactAction(topContactsPaginator.SelectedBulkAction);
    }

    public void BottomContactsBulkActionHandler(object sender, EventArgs e)
    {
        PerformContactAction(bottomContactsPaginator.SelectedBulkAction);
    }

    #endregion
}
