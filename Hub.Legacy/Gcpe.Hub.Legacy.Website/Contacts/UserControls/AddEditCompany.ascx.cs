extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;

public partial class UserControls_AddEditCompany : System.Web.UI.UserControl
{
    private enum Tabs
    {
        ContactInfo,
        WebAddress,
        Location,
        Outlets,
        Media
    }

    private Guid guid = Guid.Empty;

    public string Mode { get; set; }
    public bool IsMediaOutlet { get; set; }

    bool canCreate = false;
    bool canUpdate = false;
    CommonEventLogging log = null;
    bool isContributor = false;
    bool isAdmin = false;

    private Tabs currentTab = Tabs.ContactInfo;

    private Company GetCompany(MediaRelationsEntities ctx)
    {
        if (Guid.TryParse(Request.QueryString["guid"], out guid))
        {
            if (Mode.Equals("Edit"))
            {
                return ctx.Companies.Find(guid);
            }
            else
            {
                return (Company)Session["CreateCompany_" + guid];
            }
        }
        return null;
    }
    protected void Page_Init(object sender, EventArgs e)
    {
        log = new CommonEventLogging();
        isContributor = Permissions.IsContributor();
        isAdmin = Permissions.IsAdmin();

        canCreate = ((Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsCompany, log) & Permissions.SiteAction.Create) != 0);
        canUpdate = ((Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsCompany, log) & Permissions.SiteAction.Update) != 0);

        addNewOutletHiddenField.Value = Request.Form.Get(addNewOutletHiddenField.UniqueID);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            Company company = GetCompany(ctx);
            if (Mode.Equals("Edit"))
            {
                bool valid = (company != null && IsMediaOutlet == company.IsOutlet && company.IsActive);// prevent outlets from being edited in company screen and vise versa
                if (valid && !isAdmin)
                {
                    if (!isContributor)
                    {
                        ErrorLit.Text = "You do not have permission to edit";
                        valid = false;
                    }
                }

                if (!valid)
                {
                    guid = Guid.Empty;

                    tabControl.Visible = false;
                    buttonContainerPanel.Visible = false;
                    return;
                }
                Control control = this.Parent;
                Literal companyNameDisplayLit = (Literal)control.FindControl("companyNameDisplayLabel");
                companyNameDisplayLit.Text = "(" + company.CompanyName + ")";

                timestamp.Value = company.ModifiedDate.ToOADate().ToString();
            }

            if (Mode.Equals("Add"))
            {
                tabControl.IsAddScreen = true;
                if (Request.QueryString["guid"] != null)
                {
                    if (company == null) Response.Redirect(Request.Path + "?tab=" + Tabs.ContactInfo + "&message=" + Server.UrlEncode("Your session information has been lost"));
                }
            }

            if (!Enum.TryParse(Request.QueryString["tab"], out currentTab))
                currentTab = Tabs.ContactInfo;
            else
            {
                if (currentTab == Tabs.Media && !IsMediaOutlet) currentTab = Tabs.ContactInfo;
                if (currentTab == Tabs.Outlets && IsMediaOutlet) currentTab = Tabs.ContactInfo;

                if (currentTab == Tabs.Outlets && isContributor) currentTab = Tabs.ContactInfo;
            }

            // setup which buttons to display
            SetupPageButtons();

            if (currentTab == Tabs.ContactInfo)
            {
                if (IsMediaOutlet) parentCompanyPanel.Visible = true;

                if (!IsPostBack && IsMediaOutlet)
                {
                    List<Company> companies = (from c in ctx.Companies where !c.IsOutlet where c.IsActive select c).ToList();
                    parentCompanyDD.Items.Add(new ListItem("", Guid.Empty.ToString()));
                    foreach (Company comp in companies)
                    {
                        parentCompanyDD.Items.Add(new ListItem(comp.CompanyName, comp.Id.ToString()));
                    }

                    if (company != null && company.ParentCompanyId != null)
                    {
                        parentCompanyDD.SelectedIndex = parentCompanyDD.Items.IndexOf(
                            parentCompanyDD.Items.FindByValue(company.ParentCompanyId.ToString()));
                    }
                }


                // setup the fields that have been entered
                if (!IsPostBack && company != null)
                {
                    mediaNameTb.Text = company.CompanyName;
                    descriptionTb.Text = company.CompanyDescription;
                }

                // phone number
                List<PhoneType> phoneTypes = (from p in ctx.PhoneTypes orderby p.SortOrder ascending select p).ToList();
                Dictionary<string, MultiSelectorItem> items2 = new Dictionary<string, MultiSelectorItem>();
                foreach (PhoneType phoneType in phoneTypes)
                {
                    bool selected = false;
                    string value = "";

                    items2.Add(phoneType.Id.ToString(),
                        new MultiSelectorItem(phoneType.PhoneTypeName, selected, MultiSelectorItem.PHONE_REGEX, value, true));
                }

                phoneNumberSelector.Items = items2;

                if (company != null)
                {
                    foreach (CompanyPhoneNumber cpn in company.CompanyPhoneNumbers)
                    {
                        PhoneNumberInfo pinfo = new PhoneNumberInfo(cpn.PhoneNumber, cpn.PhoneNumberExtension);

                        phoneNumberSelector.SelectedItems.Add(new KeyValuePair<string, string>(cpn.PhoneTypeId.ToString(), pinfo.PhoneNumberString));
                    }
                }

                phoneNumberSelector.Refresh();
            }

            if (currentTab == Tabs.WebAddress)
            {
                if (IsPostBack)
                {
                    if (Session["WebAddressDisplay"] == null)
                    {
                        RedirectToView("&message=Your session has expired and information entered has been lost.");
                        return;
                    }
                }
                else // Showing the new WebAddress tab
                {
                    List<WebAddressType> webAddressTypes = (from w in ctx.WebAddressTypes orderby w.SortOrder ascending select w).ToList();

                    cboAddressTypes.DataSource = webAddressTypes;
                    cboAddressTypes.DataTextField = "WebAddressTypeName";
                    cboAddressTypes.DataValueField = "Id";
                    cboAddressTypes.DataBind();

                    var mediaDistributionLists = NodSubscriptions.GetMediaDistributionLists();

                    mediaDistributionListBox.DataSource = mediaDistributionLists;
                    mediaDistributionListBox.DataTextField = "Value";
                    mediaDistributionListBox.DataValueField = "Key";
                    mediaDistributionListBox.DataBind();


                    var CWebAddr = new List<WebAddressDisplay>();
                    foreach (CompanyWebAddress cwa in company.CompanyWebAddresses)
                    {
                        WebAddressDisplay cd = new WebAddressDisplay(cwa.Id, cwa.WebAddress);
                        cd.WebAddressTypeId = cwa.WebAddressTypeId;
                        cd.WebAddressTypeName = cwa.WebAddressType.WebAddressTypeName;

                        // ********************
                        // web addresses
                        // ********************

                        // ********************
                        if (cd.WebAddressTypeName == WebAddressType.Email)
                        {
                            //fetch the media distribution subscriptions from Nods
                            Gcpe.Hub.Services.Legacy.Models.SubscriberInfo subscriberInfo = NodSubscriptions.GetSubscriberInfo(cwa.WebAddress);

                            IList<string> subscriberMdl;
                            if (subscriberInfo?.SubscribedCategories != null
                                && subscriberInfo.SubscribedCategories.TryGetValue("media-distribution-lists", out subscriberMdl))
                            {
                                cd.MediaDistributionLists = subscriberMdl.ToArray();
                            }

                            //fetch other contacts that have uses email address
                            IList<Contact> otherContacts = (
                                from ca in ctx.ContactWebAddresses
                                join co in ctx.Contacts on ca.ContactId equals co.Id
                                where ca.Id != cwa.Id && co.IsActive
                                where ca.WebAddress == cwa.WebAddress
                                select co).ToList();

                            //fetch other companies that have uses email address
                            IList<Company> otherCompanies = (
                                from ca in ctx.CompanyWebAddresses
                                join co in ctx.Companies on ca.CompanyId equals co.Id
                                where ca.Id != cwa.Id && co.IsActive
                                where ca.WebAddress == cwa.WebAddress
                                select co).ToList();

                            //Now Compile the list
                            List<EmailAddressInstance> otherInfo = new List<EmailAddressInstance>();
                            foreach (Contact c in otherContacts)
                            {
                                EmailAddressInstance emi = new EmailAddressInstance();
                                emi.Type = "Contact";
                                emi.Name = c.FirstName + ' ' + c.LastName;
                                emi.Id = c.Id;
                                otherInfo.Add(emi);
                            }
                            foreach (Company c in otherCompanies)
                            {
                                EmailAddressInstance emi = new EmailAddressInstance();
                                emi.Type = "Outlet";
                                emi.Name = c.CompanyName;
                                emi.Id = c.Id;
                                otherInfo.Add(emi);
                            }
                            //does this email have Media Distribution Subscriptions?
                            EmailAddressInstance emi1 = new EmailAddressInstance();
                            emi1.Type = "Media Distribution Lists";
                            if (subscriberInfo != null && (bool)subscriberInfo.ExpiredLinkOrUnverifiedEmail)
                            {
                                emi1.Name = "Unverified Email Address";
                            }
                            else
                            {
                                emi1.Name = cd.MediaDistributionLists != null ? "Has subscriptions" : "No subscriptions";
                            }


                            otherInfo.Add(emi1);

                            EmailAddressInstance emi2 = new EmailAddressInstance();
                            emi2.Type = "News On Demand";
                            emi2.Name = "No subscriptions";

                            if (cd.MediaDistributionLists != null && subscriberInfo != null && subscriberInfo.SubscribedCategories.Count > 1)
                            {
                                emi2.Name = "Has subscriptions";
                            }
                            if (cd.MediaDistributionLists == null && subscriberInfo != null && subscriberInfo.SubscribedCategories.Count > 0)
                            {
                                emi2.Name = "Has subscriptions";
                                cd.OriginalMediaSubscriptionCount = subscriberInfo.SubscribedCategories.Count;
                            }
                            otherInfo.Add(emi2);
                            cd.EmailAddressInfo = otherInfo;
                        }

                        CWebAddr.Add(cd);
                    }
                    Session["WebAddressDisplay"] = CWebAddr;
                }
            }

            if (currentTab == Tabs.Location)
            {
                if (!IsPostBack)
                {
                    CompanyAddress physicalMailingAddress = null;
                    CompanyAddress mailingAddress = null;

                    int physicalType = (int)CommonMethods.AddressType.Physical;
                    int mailingType = (int)CommonMethods.AddressType.Mailing;

                    if (company != null && company.CompanyAddresses.Count > 0) physicalMailingAddress = company.CompanyAddresses.Where(x => x.AddressType == physicalType).FirstOrDefault();
                    if (company != null && company.CompanyAddresses.Count > 0) mailingAddress = company.CompanyAddresses.Where(x => x.AddressType == mailingType).FirstOrDefault();

                    // cities
                    List<City> cities = (from c in ctx.Cities orderby c.CityName ascending select c).ToList();

                    cityDD.Items.Add(new ListItem("Select City", Guid.Empty.ToString()));
                    mailingCityDD.Items.Add(new ListItem("Select city", Guid.Empty.ToString()));
                    foreach (City city in cities)
                    {
                        cityDD.Items.Add(new ListItem(city.CityName, city.Id.ToString()));
                        mailingCityDD.Items.Add(new ListItem(city.CityName, city.Id.ToString()));
                    }

                    // physical
                    if (physicalMailingAddress != null && physicalMailingAddress.CityId != null)
                    {
                        cityDD.SelectedIndex = cityDD.Items.IndexOf(cityDD.Items.FindByValue(physicalMailingAddress.CityId.ToString()));
                    }
                    // mailing
                    if (mailingAddress != null && mailingAddress.CityId != null)
                    {
                        mailingCityDD.SelectedIndex = cityDD.Items.IndexOf(mailingCityDD.Items.FindByValue(mailingAddress.CityId.ToString()));
                    }

                    bool wereCityChanges = false;
                    bool wereMailingCityChanges = false;

                    if (wereCityChanges) cityChanged.Visible = true;
                    if (wereMailingCityChanges) mailingCityChanged.Visible = true;

                    // province / state
                    List<ProvState> provStates = (from provmember in ctx.ProvStates orderby provmember.ProvStateName ascending select provmember).ToList();

                    provinceDD.Items.Add(new ListItem("Select Province/State", Guid.Empty.ToString()));
                    mailingProvinceDD.Items.Add(new ListItem("Select Province/State", Guid.Empty.ToString()));
                    foreach (ProvState provState in provStates)
                    {
                        provinceDD.Items.Add(new ListItem(provState.ProvStateName, provState.Id.ToString()));
                        mailingProvinceDD.Items.Add(new ListItem(provState.ProvStateName, provState.Id.ToString()));
                    }

                    // physical
                    if (physicalMailingAddress != null && physicalMailingAddress.ProvStateId != null)
                    {
                        provinceDD.SelectedIndex = provinceDD.Items.IndexOf(provinceDD.Items.FindByValue(physicalMailingAddress.ProvStateId.ToString()));
                    }

                    // mailing
                    if (mailingAddress != null && mailingAddress.ProvStateId != null)
                    {
                        mailingProvinceDD.SelectedIndex = mailingProvinceDD.Items.IndexOf(mailingProvinceDD.Items.FindByValue(mailingAddress.ProvStateId.ToString()));
                    }

                    bool wereProvinceChanges = false;
                    bool wereMailingProvinceChanges = false;

                    if (wereProvinceChanges) provinceChanged.Visible = true;
                    if (wereMailingProvinceChanges) mailingProvinceChanged.Visible = true;

                    // country
                    List<Country> countries = (from c in ctx.Countries orderby c.CountryName ascending select c).ToList();

                    countryDD.Items.Add(new ListItem("Select Country", Guid.Empty.ToString()));
                    mailingCountryDD.Items.Add(new ListItem("Select Country", Guid.Empty.ToString()));
                    foreach (Country country in countries)
                    {
                        countryDD.Items.Add(new ListItem(country.CountryName, country.Id.ToString()));
                        mailingCountryDD.Items.Add(new ListItem(country.CountryName, country.Id.ToString()));
                    }

                    // physical address
                    if (physicalMailingAddress != null)
                    {
                        countryDD.SelectedIndex = countryDD.Items.IndexOf(countryDD.Items.FindByValue(physicalMailingAddress.CountryId.ToString()));
                    }

                    // mailing address
                    if (mailingAddress != null)
                    {
                        mailingCountryDD.SelectedIndex = mailingCountryDD.Items.IndexOf(mailingCountryDD.Items.FindByValue(mailingAddress.CountryId.ToString()));
                    }

                    // postal code and street address

                    // physical address
                    if (physicalMailingAddress != null)
                    {
                        postalCodeTb.Text = physicalMailingAddress.PostalZipCode;
                        streetAddressTb.Text = physicalMailingAddress.StreetAddress;
                    }


                    // mailing address
                    if (mailingAddress != null)
                    {
                        mailingPostalCodeTb.Text = mailingAddress.PostalZipCode;
                        mailingStreetAddressTb.Text = mailingAddress.StreetAddress;
                    }
                }

                // regions
                Dictionary<string, MultiSelectorItem> items3 = new Dictionary<string, MultiSelectorItem>();
                List<Region> regions = (from r in ctx.Regions orderby r.RegionName ascending select r).ToList();
                foreach (Region region in regions)
                {
                    bool isSelected = false;

                    if (company != null)
                    {
                        foreach (Region r in company.Regions)
                        {
                            if (r.Id == region.Id) isSelected = true;
                        }
                    }

                    items3.Add(region.Id.ToString(), new MultiSelectorItem(region.RegionName, isSelected, "", "", false));
                }
                regionSelector.Items = items3;

                // electoral districts
                Dictionary<string, MultiSelectorItem> items4 = new Dictionary<string, MultiSelectorItem>();
                List<ElectoralDistrict> districts = (from ed in ctx.ElectoralDistricts orderby ed.DistrictName ascending select ed).ToList();
                foreach (ElectoralDistrict district in districts)
                {
                    bool isSelected = false;

                    if (company != null)
                    {
                        foreach (ElectoralDistrict dist in company.ElectoralDistricts)
                        {
                            if (dist.Id == district.Id) isSelected = true;
                        }
                    }

                    items4.Add(district.Id.ToString(), new MultiSelectorItem(district.DistrictName, isSelected, "", "", false));
                }

                electoralDistrictSelector.Items = items4;

                // sectors
                Dictionary<string, MultiSelectorItem> items5 = new Dictionary<string, MultiSelectorItem>();
                List<Sector> sectors = (from s in ctx.Sectors where s.IsActive orderby s.DisplayName ascending select s).ToList();
                foreach (Sector sector in sectors)
                {
                    bool isSelected = false;

                    if (company != null)
                    {
                        foreach (Sector sec in company.Sectors)
                        {
                            if (sec.Id == sector.Id) isSelected = true;
                        }
                    }

                    items5.Add(sector.Id.ToString(), new MultiSelectorItem(sector.DisplayName, isSelected, "", "", false));
                }
                sectorSelector.Items = items5;
            }

            if (!IsMediaOutlet && currentTab == Tabs.Outlets)
            {
                outletPopup.SaveButtonId = outletsButton.ClientID;
                outletPopup.HiddenFieldFlagId = addNewOutletHiddenField.ClientID;

                SetupOutletsPage(ctx, company, true);
            }

            if (IsMediaOutlet && currentTab == Tabs.Media)
            {
                SetupMediaOutletTabSelectors(ctx, company);

                if (!IsPostBack)
                {
                    List<PublicationFrequency> frequencies = (from f in ctx.PublicationFrequencies orderby f.PublicationFrequencyName ascending select f).ToList();
                    publicationFrequencyDD.Items.Add(new ListItem("Select Frequency"));
                    foreach (PublicationFrequency freq in frequencies)
                    {
                        publicationFrequencyDD.Items.Add(new ListItem(freq.PublicationFrequencyName, freq.Id.ToString()));
                    }

                    if (company != null && company.PublicationFrequencyId != null)
                    {
                        publicationFrequencyDD.SelectedIndex =
                            publicationFrequencyDD.Items.IndexOf(publicationFrequencyDD.Items.FindByValue(company.PublicationFrequencyId.ToString()));
                    }


                    if (company != null)
                    {
                        ethnicMediaRb.Checked = company.IsEthnicMedia == true;
                        notEthnicMediaRb.Checked = !ethnicMediaRb.Checked;

                        majorMediaRb.Checked = company.IsMajorMedia == true;
                        notMjaorMediaRb.Checked = !majorMediaRb.Checked;

                        liveMediaYesRb.Checked = company.IsLiveMedia == true;
                        liveMediaNoRb.Checked = !liveMediaYesRb.Checked;

                        if (company.PublicationFrequencyId != null)
                        {
                            publicationFrequencyDD.SelectedIndex = publicationFrequencyDD.Items.IndexOf(
                                publicationFrequencyDD.Items.FindByValue(company.PublicationFrequencyId.ToString()));
                        }

                        keyProgramsTb.Text = company.KeyPrograms;
                        circulationTb.Text = company.CirculationDescription;
                        deadlinesTb.Text = company.Deadlines;
                    }
                }
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Mode.Equals("Edit") && guid == Guid.Empty) return; // invalid page request

        if ((Mode == null || Mode.Equals("Add")) && Session["CreateCompany_" + guid.ToString()] != null)
        {
            AlertLit.Text = "doUnloadCheck=true;\n";
        }

        if (currentTab == Tabs.ContactInfo) contactPanel.Visible = true;
        if (currentTab == Tabs.WebAddress) webAddressPanel.Visible = true;
        if (currentTab == Tabs.Location) locationPanel.Visible = true;
        if (currentTab == Tabs.Outlets) outletsPanel.Visible = true;
        if (currentTab == Tabs.Media) mediaPanel.Visible = true;

        var tabList = new Dictionary<string, string>();

        Dictionary<string, string> parameters = CommonMethods.GetEditableQueryString();
        parameters.Remove("tab");
        parameters.Remove("message");
        parameters.Remove("perpage");
        parameters.Remove("page");
        parameters.Remove("sort");
        parameters.Remove("sortDir");

        string addedUrl = CommonMethods.GetQueryString(parameters);

        AddTabToList(tabList, Tabs.ContactInfo, addedUrl);
        AddTabToList(tabList, Tabs.WebAddress, addedUrl);
        AddTabToList(tabList, Tabs.Location, addedUrl);

        if (!IsMediaOutlet)
        {
            if (!isContributor)
            {
                AddTabToList(tabList, Tabs.Outlets, addedUrl);
            }
        }
        else
        {
            AddTabToList(tabList, Tabs.Media, addedUrl);
        }

        tabControl.Tabs = tabList;
        tabControl.SelectedTab = currentTab.ToString();
        tabControl.OnTabClickEvent = "DoTabChange";
    }

    private void AddTabToList(Dictionary<string, string> tabList, Tabs tab, string addedUrl)
    {
        string tabName = tab.ToString();
        string url = Request.Url.AbsolutePath + "?tab=" + tabName + (!string.IsNullOrWhiteSpace(addedUrl) ? "&" + addedUrl : "");
        string contributorTabName = tabName;
        if (tab == Tabs.ContactInfo) contributorTabName = "Contact";
        tabList.Add(tabName, url);
    }

    #region page setup methods

    private void SetupPageButtons()
    {
        cancelButton.Visible = true;

        if (isContributor)
        {
            cancelButton.OnClientClick = "return confirm(cancelAllChangesButtonText);";
        }
        else
        {
            cancelButton.OnClientClick = "return confirm(cancelButtonText);";
        }

        if (currentTab == Tabs.WebAddress)
        {
            previousButton.Visible = true;
        }

        if (currentTab == Tabs.Location)
        {
            nextButton.Visible = true;
            previousButton.Visible = true;

            if (isContributor && (Mode.Equals("Add") && !IsMediaOutlet))
            {
                nextButton.Visible = false;
                saveButton.Visible = true;
            }
        }

        if (currentTab == Tabs.Outlets || currentTab == Tabs.Media)
        {
            nextButton.Visible = false;
            previousButton.Visible = true;
            if (Mode.Equals("Add"))
            {
                saveButton.Visible = true;
            }
        }

        if (Mode.Equals("Edit"))
        {
            saveButton.Visible = true;
        }

    }

    private void SetupMediaOutletTabSelectors(MediaRelationsEntities ctx, Company company)
    {
        // media desk
        Dictionary<string, MultiSelectorItem> items = new Dictionary<string, MultiSelectorItem>();
        List<MediaDesk> mediaDesks = (from md in ctx.MediaDesks orderby md.MediaDeskName ascending select md).ToList();
        foreach (MediaDesk mediaDesk in mediaDesks)
        {
            bool isSelected = false;

            if (company.MediaDesks.Count > 0)
            {
                if (company.MediaDesks.Contains(mediaDesk)) isSelected = true;
            }

            items.Add(mediaDesk.Id.ToString(), new MultiSelectorItem(mediaDesk.MediaDeskName, isSelected, "", "", false));
        }
        mediaDeskSelector.Items = items;

        // media partners
        Dictionary<string, MultiSelectorItem> items2 = new Dictionary<string, MultiSelectorItem>();
        List<Company> mediaPartners = (from c in ctx.Companies
                                       where c.Id != company.Id
                                       where c.IsActive
                                       orderby c.CompanyName ascending
                                       select c).ToList();
        foreach (Company partnerCompany in mediaPartners)
        {
            bool isSelected = false;

            if (company.PartnerCompanies.Count > 0)
            {
                if (company.PartnerCompanies.Contains(partnerCompany)) isSelected = true;
            }

            items2.Add(partnerCompany.Id.ToString(), new MultiSelectorItem(partnerCompany.CompanyName, isSelected, "", "", false));
        }
        mediaPartnersSelector.Items = items2;

        // distribution
        Dictionary<string, MultiSelectorItem> items3 = new Dictionary<string, MultiSelectorItem>();
        List<Distribution> dists = (from d in ctx.Distributions orderby d.DistributionName ascending select d).ToList();
        foreach (Distribution distribution in dists)
        {
            bool isSelected = false;

            if (company.Distributions.Count > 0)
            {
                if (company.Distributions.Contains(distribution)) isSelected = true;
            }

            items3.Add(distribution.Id.ToString(), new MultiSelectorItem(distribution.DistributionName, isSelected, "", "", false));
        }
        distributionSelector.Items = items3;

        // language
        Dictionary<string, MultiSelectorItem> items4 = new Dictionary<string, MultiSelectorItem>();
        List<Language> languages = (from d in ctx.Languages orderby d.LanguageName ascending select d).ToList();
        foreach (Language lang in languages)
        {
            bool isSelected = false;

            if (company.Languages.Count > 0)
            {
                if (company.Languages.Contains(lang)) isSelected = true;
            }

            items4.Add(lang.Id.ToString(), new MultiSelectorItem(lang.LanguageName, isSelected, "", "", false));
        }
        languageSelector.Items = items4;

        // media type
        Dictionary<string, MultiSelectorItem> items5 = new Dictionary<string, MultiSelectorItem>();
        List<MediaType> types = (from d in ctx.MediaTypes orderby d.MediaTypeName ascending select d).ToList();
        //Response.Write(company.MediaTypes.Count);
        foreach (MediaType mediaType in types)
        {

            bool showSubItem = false;
            string subItemValue = "";
            if (mediaType.MediaTypeName.Equals("Print")) showSubItem = true;

            bool isSelected = false;

            if (company.MediaTypes.Count > 0)
            {
                if (company.MediaTypes.Contains(mediaType))
                {
                    isSelected = true;
                    if (mediaType.MediaTypeName.Equals("Print")) subItemValue = company.PrintCategoryId.ToString();
                }
            }

            items5.Add(mediaType.Id.ToString(), new MultiSelectorItem(mediaType.MediaTypeName, isSelected, "", subItemValue, showSubItem));
        }


        // add print categories to media type selector
        List<PrintCategory> printCategories = (from c in ctx.PrintCategories orderby c.PrintCategoryName ascending select c).ToList();
        Dictionary<string, string> cats = new Dictionary<string, string>();
        foreach (PrintCategory printCat in printCategories)
        {
            cats.Add(printCat.Id.ToString(), printCat.PrintCategoryName);
        }
        mediaTypeSelector.Items2 = cats;
        mediaTypeSelector.Items = items5;

        // ethnicity
        Dictionary<string, MultiSelectorItem> items6 = new Dictionary<string, MultiSelectorItem>();
        List<Ethnicity> ethnicities = (from d in ctx.Ethnicities orderby d.EthnicityName ascending select d).ToList();
        foreach (Ethnicity ethnicity in ethnicities)
        {

            bool isSelected = false;

            if (company.Ethnicities.Count > 0)
            {
                if (company.Ethnicities.Contains(ethnicity)) isSelected = true;
            }

            items6.Add(ethnicity.Id.ToString(), new MultiSelectorItem(ethnicity.EthnicityName, isSelected, "", "", false));
        }
        ethnicitySelector.Items = items6;

        // publication days
        Dictionary<string, MultiSelectorItem> items7 = new Dictionary<string, MultiSelectorItem>();
        List<PublicationDay> days = (from d in ctx.PublicationDays orderby d.SortOrder ascending select d).ToList();
        foreach (PublicationDay publicationDay in days)
        {
            bool isSelected = false;

            if (company.PublicationDays.Count > 0)
            {
                if (company.PublicationDays.Contains(publicationDay)) isSelected = true;
            }

            items7.Add(publicationDay.Id.ToString(), new MultiSelectorItem(publicationDay.PublicationDaysName, isSelected, "", "", false));
        }
        publicationDaysSelector.Items = items7;

        // specialty publications
        Dictionary<string, MultiSelectorItem> items8 = new Dictionary<string, MultiSelectorItem>();
        List<SpecialtyPublication> publications = (from d in ctx.SpecialtyPublications orderby d.SpecialtyPublicationName ascending select d).ToList();
        foreach (SpecialtyPublication specialtyPub in publications)
        {

            bool isSelected = false;
            if (company.SpecialtyPublications.Count > 0)
            {
                if (company.SpecialtyPublications.Contains(specialtyPub)) isSelected = true;
            }

            items8.Add(specialtyPub.Id.ToString(), new MultiSelectorItem(specialtyPub.SpecialtyPublicationName, isSelected, "", "", false));
        }
        specialtyPublicationsSelector.Items = items8;
    }

    private void SetupOutletsPage(MediaRelationsEntities ctx, Company company, bool doOutletPopupItemGeneration)
    {
        List<Guid> mediaOutlets = new List<Guid>();
        List<Company> currentMediaOutlets = new List<Company>();
        currentMediaOutlets = (from c in ctx.Companies where c.ParentCompanyId == company.Id && c.IsActive select c).ToList();

        if (string.IsNullOrWhiteSpace(addNewOutletHiddenField.Value))
        {
            if (Mode.Equals("Add"))
            {
                if (Session["Company_MediaOutlets_" + company.Id] != null)
                {
                    mediaOutlets = (List<Guid>)Session["Company_MediaOutlets_" + company.Id];
                }
            }
            else if (Mode.Equals("Edit"))
            {
                foreach (var outlet in currentMediaOutlets)
                {
                    mediaOutlets.Add(outlet.Id);
                }
            }
            Session.Remove("CompanyOutlets_" + company.Id);
        }
        else
        {
            addNewOutletHiddenField.Value = "true";
            List<string> guidStrList = new List<string>();

            if (Session["CompanyOutlets_" + company.Id] != null)
            {
                guidStrList = (List<string>)Session["CompanyOutlets_" + company.Id];
            }

            foreach (string guidStr in guidStrList)
            {
                mediaOutlets.Add(Guid.Parse(guidStr));
            }
        }

        // setup the items for the selected lists
        List<Company> allAvailableMediaOutlets = (from c in ctx.Companies
                                                  where c.IsOutlet && c.IsActive
                                                  where c.ParentCompanyId == null
                                                  orderby c.CompanyName ascending
                                                  select c).ToList();
        allAvailableMediaOutlets.AddRange(currentMediaOutlets); // add the list of current media outlets to all list so that these can be included in all lists if required
        allAvailableMediaOutlets.Sort(delegate (Company a, Company b) { return a.CompanyName.CompareTo(b.CompanyName); });

        if (doOutletPopupItemGeneration)
        {
            Dictionary<string, KeyValuePair<string, bool>> items = new Dictionary<string, KeyValuePair<string, bool>>();
            foreach (Company outlet in allAvailableMediaOutlets)
            {
                bool isSelected = mediaOutlets.Contains(outlet.Id);
                items.Add(outlet.Id.ToString(), new KeyValuePair<string, bool>(outlet.CompanyName, isSelected));
            }
            outletPopup.Items = items;
        }

        string sort = "company";
        if (!string.IsNullOrWhiteSpace(Request.QueryString["sort"]))
        {
            sort = Request.QueryString["sort"].Trim().ToLower();
        }
        string sortColumn = "CompanyName";
        switch (sort)
        {
            case "company":
                sortColumn = "CompanyName";
                break;
            case "city":
                sortColumn = "CityName";
                break;
            case "media type":
                sortColumn = "MediaTypeName";
                break;
            case "email":
                sortColumn = "Email";
                break;
            case "twitter":
                sortColumn = "Twitter";
                break;
            default:
                sortColumn = "CompanyName";
                break;
        }

        bool isDescending = false;
        if (!string.IsNullOrWhiteSpace(Request.QueryString["sortDir"]))
        {
            if (Request.QueryString["sortDir"].Trim().ToLower().Equals("desc")) isDescending = true;
        }

        StringBuilder sb = new StringBuilder();

        string primaryPhone = PhoneType.Primary;
        string emailAddress = WebAddressType.Email;
        string twitterHandle = WebAddressType.Twitter;

        var obj = (from comp in ctx.Companies
                   join phoneNumber in
                       (from s in ctx.CompanyPhoneNumbers where s.PhoneType.PhoneTypeName == primaryPhone select s) on comp equals phoneNumber.Company into companyPhoneNumbers
                   from companyPhone in companyPhoneNumbers.DefaultIfEmpty()
                   join email in
                       (from s in ctx.CompanyWebAddresses where s.WebAddressType.WebAddressTypeName == emailAddress select s) on comp equals email.Company into companyEmailAddresses
                   from companyEmail in companyEmailAddresses.DefaultIfEmpty()
                   join twitter in
                       (from s in ctx.CompanyWebAddresses where s.WebAddressType.WebAddressTypeName == twitterHandle select s) on comp equals twitter.Company into companyTwitters
                   from companyTwitter in companyTwitters.DefaultIfEmpty()
                   where comp.IsActive
                   select new
                   {
                       CompanyId = comp.Id,
                       CompanyName = comp.CompanyName,
                       CityName = comp.CompanyAddresses.Count > 0 ? comp.CompanyAddresses.FirstOrDefault().City.CityName : null,
                       PhoneNumber = companyPhone.PhoneNumber,
                       Email = companyEmail.WebAddress,
                       Twitter = companyTwitter.WebAddress,
                       MediaTypes = comp.MediaTypes
                   });
        obj = obj.Where(c => mediaOutlets.Contains(c.CompanyId));

        var objects = LinqDataMethods.OrderBy(obj, sortColumn, isDescending, false);
        PaginatorTop.Count = PaginatorBottom.Count = objects.Count();

        var listOfObjects = objects.Skip(PaginatorTop.Skip).Take(PaginatorTop.PerPage).ToList();


        int count = 0;
        foreach (var item in listOfObjects)
        {
            sb.Append("<tr class='" + (count % 2 == 0 ? "even" : "odd") + "'>\n");
            sb.Append("<td>" + item.CompanyName + "</td>\n");
            sb.Append("<td>" + (!string.IsNullOrWhiteSpace(item.CityName) ? item.CityName : "&mdash;") + "</td>\n");

            string mediaTypeString = "";
            int innerCount = 0;
            foreach (MediaType type in item.MediaTypes)
            {
                if (innerCount != 0) mediaTypeString += "<br/>";
                mediaTypeString += type.MediaTypeName;
                innerCount++;
            }

            sb.Append("<td>" + (!string.IsNullOrWhiteSpace(mediaTypeString) ? mediaTypeString : "&mdash;") + "</td>\n");
            sb.Append("<td>" + (!string.IsNullOrWhiteSpace(item.PhoneNumber) ? legacy::Gcpe.Hub.Utility.FormatPhoneNumber(item.PhoneNumber) : "&mdash;") + "</td>\n");
            sb.Append("<td>" + (!string.IsNullOrWhiteSpace(item.Email) ? item.Email : "&mdash;") + "</td>\n");
            sb.Append("<td>" + (!string.IsNullOrWhiteSpace(item.Twitter) ? item.Twitter : "&mdash;") + "</td>\n");
            sb.Append("</tr>\n");
            count++;
        }

        if (count == 0) sb.Append("<tr><td colspan='9'>No items to display</td></tr>\n");





        outletTableLit.Text = sb.ToString();

    }

    #endregion

    private void DisplaySaveMessage()
    {

        StringBuilder sb = new StringBuilder();
        sb.Append("<script type='text/javascript'>\n");
        sb.Append("$(document).ready(function() {\n");
        sb.Append("     alert(\"Changes have been saved\");\n");
        sb.Append("});\n");
        sb.Append("</script>\n");

        jsLit.Text = sb.ToString();
    }

    private void GoToTab(Tabs clickedTab)
    {
        Response.Redirect(Request.Url.AbsolutePath + "?tab=" + clickedTab + "&guid=" + guid);
    }
    private bool SaveChanges(Company company, MediaRelationsEntities ctx)
    {
        if (ctx.ChangeTracker.HasChanges())
        {
            bool modifiedByAnotherUser = (timestamp.Value != company.ModifiedDate.ToOADate().ToString());
            if (modifiedByAnotherUser)
            {
                ErrorLit.Text = "The company was updated by another user and your changes cannot be saved.<br/> Please open the company again in order to see the most recent changes";
                return false;
            }
            else
            {
                company.RecordEditedBy = CommonMethods.GetLoggedInUser();
                company.ModifiedDate = DateTime.Now;
                ctx.SaveChanges();
            }
        }
        return true;
    }


    #region button handlers

    protected void SaveButtonClick(object sender, EventArgs e)
    {
        jsLit.Text = "";

        CompanyAdministrationLib lib = new CompanyAdministrationLib();

        using (var ctx = new MediaRelationsEntities())
        {
            Company company = GetCompany(ctx);
            if (Mode.Equals("Add") && canCreate)
            {
                bool doRedirect = true;

                Guid newGuid = Guid.Empty;

                if (IsMediaOutlet)
                {
                    string returnVal = PerformMediaTabSave(company, ctx, true);
                    doRedirect = false;
                    if (returnVal != null)
                    {
                        newGuid = lib.CreateCompanyFinalize(company, isContributor);
                        doRedirect = true;
                    }
                }
                else
                {
                    string returnVal = null;

                    if (isContributor) returnVal = PerformLocationTabSave(company, ctx, true);
                    else returnVal = PerformOutletTabSave(company, ctx, true);

                    doRedirect = false;
                    if (returnVal != null)
                    {
                        newGuid = lib.CreateCompanyFinalize(company, isContributor);
                        doRedirect = true;
                    }
                }

                string message = "Success! " + (IsMediaOutlet ? "Media Outlet" : "Company") + " has been added.";

                if (doRedirect)
                {
                    Response.Redirect("~/Contacts/Default.aspx?message=" + Server.UrlEncode(message));
                }

            }
            else if (Mode.Equals("Edit") && canUpdate)
            {
                Tabs clickedTab = Tabs.Media;
                bool didClickOnTab = Enum.TryParse(tabHiddenField.Value, out clickedTab);

                string val = null;

                if (currentTab == Tabs.ContactInfo)
                {
                    val = PerformContactTabSave(company, ctx, true);
                }
                else if (currentTab == Tabs.WebAddress)
                {
                    val = PerformWebAddressTabSave(company, ctx, true);
                }
                else if (currentTab == Tabs.Location)
                {
                    val = PerformLocationTabSave(company, ctx, true);
                }
                else if (currentTab == Tabs.Outlets)
                {
                    val = PerformOutletTabSave(company, ctx, true);
                }
                else if (currentTab == Tabs.Media)
                {
                    val = PerformMediaTabSave(company, ctx, true);
                }

                if (val != null)
                {
                    if (!didClickOnTab)
                    {
                        RedirectToView("&message=Changes have been saved");
                    }
                    else
                    {
                        GoToTab(clickedTab);
                    }

                }
            }
        }
    }

    protected void SaveOutletsClick(object sender, EventArgs e)
    {
        if (currentTab == Tabs.Outlets)
        {
            if (addNewOutletHiddenField.Value != "")
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    Company company = GetCompany(ctx);
                    List<KeyValuePair<string, string>> selectedItems = outletPopup.SelectedValues;

                    List<string> outletGuids = new List<string>();
                    foreach (KeyValuePair<string, string> item in selectedItems)
                    {
                        outletGuids.Add(item.Value);
                    }

                    Session.Add("CompanyOutlets_" + company.Id, outletGuids);
                    SetupOutletsPage(ctx, company, true);
                }
            }
        }
    }

    private string PerformMediaTabSave(Company company, MediaRelationsEntities ctx, bool requiredCheck)
    {
        distributionSelector.ErrorMessage = "";
        languageSelector.ErrorMessage = "";
        mediaTypeSelector.ErrorMessage = "";
        mediaTypeSelector.ErrorMessage = "";
        distributionSelector.ErrorMessage = "";

        keyProgramsError.InnerHtml = "";
        circulationError.InnerHtml = "";
        deadlinesError.InnerHtml = "";

        CompanyAdministrationLib lib = new CompanyAdministrationLib();

        Guid? publicationFrequencyId = publicationFrequencyDD.SelectedIndex <= 0 ? (Guid?)null : Guid.Parse(publicationFrequencyDD.SelectedValue);

        int errors = 0;
        string guidReturnStr = null;
        if (Mode.Equals("Add") && canCreate)
        {
            KeyValuePair<int, string> returnVal = lib.CreateMediaOutletThirdTab(company, mediaDeskSelector.SelectedItems, mediaPartnersSelector.SelectedItems,
                distributionSelector.SelectedItems, languageSelector.SelectedItems, publicationDaysSelector.SelectedItems,
                specialtyPublicationsSelector.SelectedItems, mediaTypeSelector.SelectedItems, ethnicitySelector.SelectedItems, ethnicMediaRb.Checked,
                publicationFrequencyId, majorMediaRb.Checked, liveMediaYesRb.Checked, keyProgramsTb.Text.Trim(), circulationTb.Text.Trim(),
                deadlinesTb.Text.Trim(), requiredCheck);

            errors = returnVal.Key;
            guidReturnStr = returnVal.Value;
        }
        else if (Mode.Equals("Edit") && canUpdate)
        {
            errors = lib.EditMediaOutletThirdTab(company, ctx, mediaDeskSelector.SelectedItems, mediaPartnersSelector.SelectedItems,
                distributionSelector.SelectedItems, languageSelector.SelectedItems, publicationDaysSelector.SelectedItems,
                specialtyPublicationsSelector.SelectedItems, mediaTypeSelector.SelectedItems, ethnicitySelector.SelectedItems, ethnicMediaRb.Checked,
                publicationFrequencyId, majorMediaRb.Checked, liveMediaYesRb.Checked, keyProgramsTb.Text.Trim(), circulationTb.Text.Trim(),
                deadlinesTb.Text.Trim(), requiredCheck);
            guidReturnStr = errors.ToString();
        }

        if (errors != 0 || !SaveChanges(company, ctx))
        {
            if ((errors & 1) != 0) distributionSelector.ErrorMessage = "At least one distribution must be selected";
            if ((errors & 2) != 0) languageSelector.ErrorMessage = "At least one language must be selected";
            if ((errors & 4) != 0) mediaTypeSelector.ErrorMessage = "At least one media type must be selected";
            if ((errors & 8) != 0) mediaTypeSelector.ErrorMessage = "Print category must be selected";
            if ((errors & 16) != 0) distributionSelector.ErrorMessage = "At least one ethnicity must be selected (ethnic media selected)";
            if ((errors & 32) != 0) keyProgramsError.InnerHtml = "Must be less than 500 characters";
            if ((errors & 64) != 0) circulationError.InnerHtml = "Must be less than 500 characters";
            if ((errors & 128) != 0) deadlinesError.InnerHtml = "Must be less than 500 characters";

            return null;
        }

        return guidReturnStr;
    }

    private string PerformContactTabSave(Company company, MediaRelationsEntities ctx, bool requiredCheck)
    {
        CompanyAdministrationLib lib = new CompanyAdministrationLib();

        mediaNameError.InnerHtml = "";
        phoneNumberSelector.ErrorMessage = "";
        descriptionError.InnerHtml = "";


        string phoneNumberError = "";
        //string webAddressError = "";

        int errors = 0;
        string guidReturn = null;

        Guid? parentCompanyId = null;
        if (IsMediaOutlet) parentCompanyId = Guid.Parse(parentCompanyDD.SelectedValue);

        List<KeyValuePair<string, string>> numbers = phoneNumberSelector.SelectedItems;
        List<KeyValuePair<string, PhoneNumberInfo>> numbersInfo = new List<KeyValuePair<string, PhoneNumberInfo>>();

        foreach (KeyValuePair<string, string> pair in numbers)
        {
            PhoneNumberInfo phoneInfo = PhoneNumberInfo.GetPhoneNumberInfo(pair.Value);
            phoneInfo.PhoneNumber = legacy::Gcpe.Hub.Utility.FormatPhoneNumber(phoneInfo.PhoneNumber);
            numbersInfo.Add(new KeyValuePair<string, PhoneNumberInfo>(pair.Key, phoneInfo));
        }

        if (Mode.Equals("Add") && canCreate)
        {
            KeyValuePair<int, string> returnVal = lib.CreateCompanyFirstTab(mediaNameTb.Text.Trim(), descriptionTb.Text.Trim(),
             numbersInfo, IsMediaOutlet, parentCompanyId, company, requiredCheck);

            errors = returnVal.Key;
            guidReturn = returnVal.Value;
        }
        else if (Mode.Equals("Edit") && canUpdate)
        {
            errors = lib.EditCompanyFirstTab(company, ctx, mediaNameTb.Text.Trim(), descriptionTb.Text.Trim(),
                numbersInfo, IsMediaOutlet, parentCompanyId, requiredCheck);
            guidReturn = errors.ToString();
        }

        if (errors != 0 || !SaveChanges(company, ctx))
        {
            if ((errors & 1) != 0) mediaNameError.InnerHtml = "Media name cannot be empty";
            if ((errors & 2) != 0) phoneNumberError = "At least one phone number must be entered";
            //if ((errors & 4) != 0) phoneNumberError = "Invalid primary phone number"; -- validation in dll doesnt generate this
            //if ((errors & 8) != 0) webAddressError = "There is invalid data provided";
            if ((errors & 16) != 0) phoneNumberError = "There is invalid data provided";
            if ((errors & 32) != 0) descriptionError.InnerHtml = "Description can only be 500 characters long";
            if ((errors & 64) != 0) mediaNameError.InnerHtml = "A" + (IsMediaOutlet ? "n Outlet" : " Company") + " with that name already exists. All " + (IsMediaOutlet ? "Outlet" : "Company") + " must have a unique name. If you wish to add this  please change the name to be unique.";

            phoneNumberSelector.ErrorMessage = phoneNumberError;

            return null;
        }
        return guidReturn;
    }

    private string PerformLocationTabSave(Company company, MediaRelationsEntities ctx, bool requiredCheck)
    {
        CompanyAdministrationLib lib = new CompanyAdministrationLib();

        addressesError.InnerHtml = "";
        addressError.InnerHtml = "";
        cityError.InnerHtml = "";
        provinceError.InnerHtml = "";
        countryError.InnerHtml = "";
        mailingAddressError.InnerHtml = "";
        mailingCityError.InnerHtml = "";
        mailingProvinceError.InnerHtml = "";
        mailingCountryError.InnerHtml = "";
        regionSelector.ErrorMessage = "";
        electoralDistrictSelector.ErrorMessage = "";

        int errors = 0;
        string guidStrReturn = null;

        if (Mode.Equals("Add") && canCreate)
        {
            KeyValuePair<int, string> returnVal = lib.CreateCompanySecondTab(company, streetAddressTb.Text.Trim(),
                Guid.Parse(cityDD.SelectedValue), Guid.Parse(provinceDD.SelectedValue), Guid.Parse(countryDD.SelectedValue), postalCodeTb.Text.Trim(),
                mailingStreetAddressTb.Text.Trim(), Guid.Parse(mailingCityDD.SelectedValue), Guid.Parse(mailingProvinceDD.SelectedValue),
                Guid.Parse(mailingCountryDD.SelectedValue), mailingPostalCodeTb.Text.Trim(),
                regionSelector.SelectedItems, electoralDistrictSelector.SelectedItems, sectorSelector.SelectedItems, requiredCheck);
            errors = returnVal.Key;
            guidStrReturn = returnVal.Value;
        }
        else if (Mode.Equals("Edit") && canUpdate)
        {
            errors = lib.EditCompanySecondTab(company, ctx, streetAddressTb.Text.Trim(),
                Guid.Parse(cityDD.SelectedValue), Guid.Parse(provinceDD.SelectedValue), Guid.Parse(countryDD.SelectedValue), postalCodeTb.Text.Trim(),
                mailingStreetAddressTb.Text.Trim(), Guid.Parse(mailingCityDD.SelectedValue), Guid.Parse(mailingProvinceDD.SelectedValue),
                Guid.Parse(mailingCountryDD.SelectedValue), mailingPostalCodeTb.Text.Trim(),
                regionSelector.SelectedItems, electoralDistrictSelector.SelectedItems, sectorSelector.SelectedItems, requiredCheck);
            guidStrReturn = errors.ToString();
        }

        if (errors != 0 || !SaveChanges(company, ctx))
        {
            if ((errors & 2) != 0) cityError.InnerHtml = "City has to be selected ";
            if ((errors & 4) != 0) provinceError.InnerHtml = "Province/State has to be selected";
            if ((errors & 8) != 0) countryError.InnerHtml = "Country has to be selected";
            if ((errors & 16) != 0) regionSelector.ErrorMessage = "At least one region must be selected";
            if ((errors & 32) != 0) electoralDistrictSelector.ErrorMessage = "At least one electoral district has to be selected";
            if ((errors & 64) != 0) addressesError.InnerHtml = "At least one address must be filled out";
            if ((errors & 128) != 0) mailingCityError.InnerHtml = "City has to be selected";
            if ((errors & 256) != 0) mailingProvinceError.InnerHtml = "Province/State has to be selected";
            if ((errors & 512) != 0) mailingCountryError.InnerHtml = "Country has to be selected";
            if ((errors & 1024) != 0) addressError.InnerHtml = "Address cannot be longer than 250 characters long";
            if ((errors & 2048) != 0) mailingAddressError.InnerHtml = "Address cannot be longer than 250 characters long";

            return null;
        }
        return guidStrReturn;
    }

    private string PerformWebAddressTabSave(Company company, MediaRelationsEntities ctx, bool requiredCheck)
    {
        CompanyAdministrationLib lib = new CompanyAdministrationLib();
        if (btnApply.Visible)
        {
            btnApply_Click(null, null);
        }
        var CWebAddr = (List<WebAddressDisplay>)Session["WebAddressDisplay"];
        //bool dbSave = true;
        //lib.UpdateCompanyTabWebAddress(company, CWebAddr, dbSave); //TODO handle the xlm bit still.
        //return "foo";
        int errors = 0;
        string guidStrReturn = null;


        if (Mode.Equals("Add") && canCreate)
        {
            KeyValuePair<int, string> returnVal = lib.CreateCompanyWebAddressTab(company, CWebAddr, requiredCheck);
            errors = returnVal.Key;
            guidStrReturn = returnVal.Value;
        }
        else if (Mode.Equals("Edit") && canUpdate)
        {
            errors = lib.EditCompanyTabWebAddress(company, ctx, CWebAddr, true);
            guidStrReturn = errors.ToString();
        }

        if (errors != 0 || !SaveChanges(company, ctx))
        {
            //if ((errors & 2) != 0) cityError.InnerHtml = "City has to be selected ";
            //if ((errors & 4) != 0) provinceError.InnerHtml = "Province/State has to be selected";
            //if ((errors & 8) != 0) countryError.InnerHtml = "Country has to be selected";
            //if ((errors & 16) != 0) regionSelector.ErrorMessage = "At least one region must be selected";
            //if ((errors & 32) != 0) electoralDistrictSelector.ErrorMessage = "At least one electoral district has to be selected";
            //if ((errors & 64) != 0) addressesError.InnerHtml = "At least one address must be filled out";
            //if ((errors & 128) != 0) mailingCityError.InnerHtml = "City has to be selected";
            //if ((errors & 256) != 0) mailingProvinceError.InnerHtml = "Province/State has to be selected";
            //if ((errors & 512) != 0) mailingCountryError.InnerHtml = "Country has to be selected";
            //if ((errors & 1024) != 0) addressError.InnerHtml = "Address cannot be longer than 250 characters long";
            //if ((errors & 2048) != 0) mailingAddressError.InnerHtml = "Address cannot be longer than 250 characters long";

            return null;
        }
        return guidStrReturn;
    }

    private string PerformOutletTabSave(Company company, MediaRelationsEntities ctx, bool requiredCheck)
    {
        if (string.IsNullOrWhiteSpace(addNewOutletHiddenField.Value)) return company.Id.ToString(); // no changes to outlets where made

        List<string> listOfGuids = new List<string>();
        if (Session["CompanyOutlets_" + company.Id] != null)
        {
            listOfGuids = (List<string>)Session["CompanyOutlets_" + company.Id];

            int errors = 0;
            string returnStr = null;

            CompanyAdministrationLib lib = new CompanyAdministrationLib();
            if (Mode.Equals("Add") && canCreate)
            {
                KeyValuePair<int, string> ret = lib.CreateCompanyThirdTab(company, listOfGuids, requiredCheck);
                errors = ret.Key;
                returnStr = ret.Value;
            }
            else if (Mode.Equals("Edit") && canUpdate)
            {
                errors = lib.EditCompanyThirdTab(company, ctx, listOfGuids, requiredCheck);
                returnStr = company.Id.ToString();
            }

            if (errors == 0 && SaveChanges(company, ctx))
            {
                return returnStr;
            }
        }

        return null;
    }

    protected void PreviousButtonClick(object sender, EventArgs e)
    {
        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            Company company = GetCompany(ctx);
            if (currentTab == Tabs.Location)
            {
                string returnVal = PerformLocationTabSave(company, ctx, false);
                if (returnVal != null) Response.Redirect(Request.Path + "?tab=" + Tabs.WebAddress + "&guid=" + guid);
            }
            if (currentTab == Tabs.WebAddress)
            {
                string returnVal = PerformWebAddressTabSave(company, ctx, false);
                if (returnVal != null) Response.Redirect(Request.Path + "?tab=" + Tabs.ContactInfo + "&guid=" + guid);
            }

            if (currentTab == Tabs.Outlets || currentTab == Tabs.Media)
            {
                bool doRedirect = true;

                bool requireChanges = true;
                if (Mode.Equals("Add")) requireChanges = false;

                if (IsMediaOutlet)
                {
                    string val = PerformMediaTabSave(company, ctx, requireChanges);
                    if (val == null) doRedirect = false;
                }
                else
                {
                    string val = PerformOutletTabSave(company, ctx, requireChanges);
                    if (val == null) doRedirect = false;
                }

                if (doRedirect) Response.Redirect(Request.Path + "?tab=" + Tabs.Location.ToString() + "&guid=" + guid.ToString());
            }
        }
    }

    protected void NextButtonClick(object sender, EventArgs e)
    {
        CompanyAdministrationLib lib = new CompanyAdministrationLib();

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            Company company = GetCompany(ctx);
            if (currentTab == Tabs.ContactInfo)
            {
                string guid = PerformContactTabSave(company, ctx, true);
                if (guid != null)
                {
                    if (Mode.Equals("Edit"))
                    {
                        guid = company.Id.ToString();
                    }
                    Response.Redirect(Request.Path + "?tab=" + Tabs.WebAddress + "&guid=" + guid);
                }
            }

            if (currentTab == Tabs.WebAddress)
            {
                string guid = PerformWebAddressTabSave(company, ctx, true);
                if (guid != null)
                {
                    if (Mode.Equals("Edit"))
                    {
                        guid = company.Id.ToString();
                    }
                    Response.Redirect(Request.Path + "?tab=" + Tabs.Location + "&guid=" + guid);
                }
            }

            if (currentTab == Tabs.Location)
            {
                string guid = PerformLocationTabSave(company, ctx, true);

                Tabs nextTab = Tabs.Outlets;
                if (IsMediaOutlet) nextTab = Tabs.Media;

                if (guid != null) Response.Redirect(Request.Path + "?tab=" + nextTab + "&guid=" + company.Id);
            }
        }
    }

    protected void RedirectToView(string message)
    {
        Response.Redirect((IsMediaOutlet ? "~/Contacts/MediaOutlet/ViewOutlet.aspx?guid=" : "~/Contacts/Company/ViewCompany.aspx?guid=") +
            Request.QueryString["guid"] + "&tab=" + currentTab + message);
    }

    protected void CancelButtonClick(object sender, EventArgs e)
    {
        if (Mode.Equals("Edit"))
        {
            RedirectToView("");
        }
        else if (Mode.Equals("Add"))
        {
            Response.Redirect("~/Contacts/");
        }
    }

    #endregion

    protected void btnAddWebAddress_Click(object sender, EventArgs e)
    {
        var CWebAddr = (List<WebAddressDisplay>)Session["WebAddressDisplay"];
        CWebAddr.Add(new WebAddressDisplay(Guid.NewGuid()));
        GridViewWebAddress.DataBind();
        GridViewSelectEventArgs e1 = new GridViewSelectEventArgs(CWebAddr.Count - 1);
        GridViewWebAddress.SelectedIndex = CWebAddr.Count - 1;
        GridViewWebAddress_SelectWebAddress(null, e1);
    }

    protected void GridViewWebAddress_SelectWebAddress(object sender, GridViewSelectEventArgs e)
    {
        WebAddressEditPanel.Visible = true;
        btnAddWebAddress.Visible = !WebAddressEditPanel.Visible;
        var CWebAddr = (List<WebAddressDisplay>)Session["WebAddressDisplay"];
        WebAddressDisplay rowItem = CWebAddr.ElementAt(e.NewSelectedIndex);
        txtWebAddress.Text = rowItem.NewWebAddress;
        GridViewOtherInfo.DataSource = rowItem.EmailAddressInfo;
        GridViewOtherInfo.DataBind();
        cboAddressTypes.Visible = rowItem.IsNew;
        lblWebAddress.Visible = !cboAddressTypes.Visible;
        if (lblWebAddress.Visible)
        {
            lblWebAddress.Text = rowItem.WebAddressTypeName;
        }
        mediaDistributionListBox.Visible = isAdmin && (rowItem.WebAddressTypeName == null || rowItem.WebAddressTypeName == WebAddressType.Email);
        if (mediaDistributionListBox.Visible)
        {
            foreach (ListItem item in mediaDistributionListBox.Items)
            {
                item.Selected = false;
                if (rowItem.MediaDistributionLists != null && rowItem.MediaDistributionLists.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
        }
        mediaDistributionListBoxLabel.Visible = mediaDistributionListBox.Visible;
    }

    protected void GridViewWebAddress_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        var cWebAddr = (List<WebAddressDisplay>)Session["WebAddressDisplay"];
        if (cWebAddr != null)
        {
            var id = GridViewWebAddress.Rows[e.RowIndex].Cells[1].Text;
            var rowItem = cWebAddr.FirstOrDefault(r => r.Id.ToString() == id);
            if (rowItem != null)
            {
                rowItem.IsDeleted = !rowItem.IsDeleted;
            }
            GridViewWebAddress.DataBind();
        }
    }

    protected void UpdatePanelWebAddressLoad(object sender, EventArgs e)
    {
        if (currentTab == Tabs.WebAddress)
        {
            GridViewWebAddress.DataSource = Session["WebAddressDisplay"];
            GridViewWebAddress.DataBind();
        }
    }

    protected void GridViewWebAddress_DataBound(object sender, EventArgs e)
    {
        var cWebAddr = (List<WebAddressDisplay>)Session["WebAddressDisplay"];
        foreach (GridViewRow row in GridViewWebAddress.Rows)
        {
            if (!cWebAddr[row.RowIndex].CanBeDeleted(isAdmin))
            {
                foreach (DataControlFieldCell field in row.Controls)
                {
                    ButtonField buttonField = field.ContainingField as ButtonField;
                    if (buttonField != null && buttonField.CommandName == "Delete")
                    {
                        var deleteBtn = (WebControl)field.Controls[0];
                        deleteBtn.Enabled = false;
                        deleteBtn.CssClass = "disabled";
                    }
                }
            }
        }
    }
    protected void cboAddressTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (isAdmin)
        {
            mediaDistributionListBox.Visible = (cboAddressTypes.SelectedItem.Text == WebAddressType.Email);
            mediaDistributionListBoxLabel.Visible = mediaDistributionListBox.Visible;
        }
    }

    protected void txtWebAddress_TextChanged(object sender, EventArgs e)
    {
        Gcpe.Hub.Services.Legacy.Models.SubscriberInfo subscriberInfo = NodSubscriptions.GetSubscriberInfo(txtWebAddress.Text);
        IList<string> subscriberMdl;
        if (subscriberInfo?.SubscribedCategories != null
            && subscriberInfo.SubscribedCategories.TryGetValue("media-distribution-lists", out subscriberMdl))
        {
            foreach (ListItem listItem in mediaDistributionListBox.Items)
            {
                listItem.Selected = subscriberMdl.Contains(listItem.Value);
            }
        }
    }

    protected void btnApply_Click(object sender, EventArgs e)
    {
        //update the data in the session, ready to be saved or cancelled
        var cWebAddr = (List<WebAddressDisplay>)Session["WebAddressDisplay"];
        WebAddressDisplay rowItem = cWebAddr.ElementAt(GridViewWebAddress.SelectedIndex);
        if (rowItem.IsNew)
        {
            rowItem.WebAddressTypeId = new Guid(cboAddressTypes.SelectedValue);
            rowItem.WebAddressTypeName = cboAddressTypes.SelectedItem.Text;
        }
        rowItem.Update(txtWebAddress.Text);
        if (isAdmin)
        {
            List<string> selectedItems = new List<string>();
            if (rowItem.WebAddressTypeName == WebAddressType.Email)
            {
                foreach (ListItem item in mediaDistributionListBox.Items)
                {
                    if (item.Selected)
                    {
                        selectedItems.Add(item.Value);
                    }
                }
            }
            rowItem.Update(selectedItems);
        }
        HideWebAddressEditPanel();
    }
    protected void HideWebAddressEditPanel()
    {
        WebAddressEditPanel.Visible = false;
        btnAddWebAddress.Visible = !WebAddressEditPanel.Visible;

        GridViewWebAddress.DataBind();
        GridViewOtherInfo.DataSource = null;
        GridViewOtherInfo.DataBind();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        var cWebAddr = (List<WebAddressDisplay>)Session["WebAddressDisplay"];
        WebAddressDisplay rowItem = cWebAddr.ElementAt(GridViewWebAddress.SelectedIndex);
        if (rowItem.IsNew)
        {
            cWebAddr.Remove(rowItem);
        }
        HideWebAddressEditPanel();
    }
}
