extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaRelationsDatabase;
using MediaRelationsLibrary;

public partial class UserControls_ViewCompany : System.Web.UI.UserControl
{
    private const string CHANGED_CLASS = "view-label view-changed";
    private const string DELETED_CLASS = "view-label view-deleted";
    private const string CHANGED_CLASS_VALUE = "";
    private const string DELETED_CLASS_VALUE = "view-value view-deleted";
    private const string CHANGED_NAME_CLASS = "label labelbold view-changed";

    private enum Tabs
    {
        ContactInfo,
        WebAddress,
        Location,
        Outlets,
        Media
    }


    private Tabs currentTab = Tabs.ContactInfo;
    public string Mode { get; set; }

    protected Guid guid = Guid.Empty;
    private Company company = null;

    private bool IsAdmin
    {
        get { return Permissions.IsAdmin(); }
    }

    private bool IsContributor
    {
        get { return Permissions.IsContributor(); }
    }

    public bool IsMediaOutlet
    {
        get;
        set;
    }

    Permissions.SiteAction permissions = Permissions.SiteAction.None;

    protected void Page_Init(object sender, EventArgs e)
    {
        permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsCompany);

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            Guid.TryParse(Request.QueryString["guid"], out guid);
            company = (from c in ctx.Companies where c.Id == guid where c.IsActive select c).FirstOrDefault();

            if (company == null)
            {
                errorPanel.Visible = true;
                return;
            }

            if (company.IsOutlet != IsMediaOutlet)
            {
                errorPanel.Visible = true;
                return;
            }

            companyDisplayPanel.Visible = true;

            if (company.IsOutlet)
            {
                mediaTabDisplay.Visible = true;
                mediaTabBar.Visible = true;
                mediaTab1.Visible = true;
                mediaTab2.Visible = true;
                mediaTab3.Visible = true;
                mediaTab4.Visible = true;

                outletsTabBar.Visible = false;
                outletsTab1.Visible = false;
                outletsTab2.Visible = false;
                outletsTab3.Visible = false;
                outletsTab4.Visible = false;
            }
            else
            {
                mediaTabBar.Visible = false;
                mediaTab1.Visible = false;
                mediaTab2.Visible = false;
                mediaTab3.Visible = false;
                mediaTab4.Visible = false;

                outletTabDisplay.Visible = true;
                outletsTabBar.Visible = true;
                outletsTab1.Visible = true;
                outletsTab2.Visible = true;
                outletsTab3.Visible = true;
                outletsTab4.Visible = true;
            }

            if (!Enum.TryParse(Request.QueryString["tab"], out currentTab))
                currentTab = Tabs.ContactInfo;

            // check to see if the user did a function on the outlets page to see
            // if the page should be changed to the outlets page instead of remain
            // where it is - due to javascript changes of tabs
            if (Request.QueryString["sort"] != null ||
                Request.QueryString["page"] != null ||
                Request.QueryString["perpage"] != null)
            {
                currentTab = Tabs.Outlets;
            }

            if (Mode.Equals("View"))
            {
                Page_Init_View();
            }

            // setup tabs
            Dictionary<string, string> tabList = new Dictionary<string, string>();

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
            if (company.IsOutlet)
            {
                AddTabToList(tabList, Tabs.Media, addedUrl);
            }
            else
            {
                AddTabToList(tabList, Tabs.Outlets, addedUrl);
            }
            tabControl.Tabs = tabList;
            tabControl.SelectedTab = currentTab.ToString();

            if (Mode.Equals("View")) tabControl.OnTabClickEvent = "ChangeCurrentTab";

            DisplayContactTabInformation(ctx);
            DisplayWebAddressTabInformation(ctx);
            DisplayLocationTabInformation(ctx);

            if (company.IsOutlet) DisplayMediaTabInformation(ctx);
            if (!company.IsOutlet) DisplayOutletTabInformation(ctx);
        }
    }


    private void Page_Init_View()
    {
        SetupPageButtonsView();
    }

    private void SetupPageButtonsCommon()
    {
        if (IsAdmin || IsContributor)
        {
            historyHref.Visible = true;
            historySeparator.Visible = true;

            historyFrame.Src = ResolveUrl("~/Contacts/") + "RecordHistory.aspx?type=company&guid=" + company.Id;
        }

        if (company.IsActive)
        {
            if ((permissions & Permissions.SiteAction.Update) != 0)
            {
                editButton.Visible = true;
                editSeparator.Visible = true;
            }
            if (currentTab == Tabs.Outlets && !IsAdmin && !IsContributor) editButton.Visible = false;
        }
    }

    private void SetupPageButtonsView()
    {
        SetupPageButtonsCommon();

        if ((permissions & Permissions.SiteAction.Delete) != 0)
        {
            deleteButton.Visible = true;
            printSeparator.Visible = true;
        }

        if (IsAdmin && !company.IsOutlet)
        {
            convertToOutletBtn.Visible = true;
            convertToOutletSeparator.Visible = true;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (company == null) return;

        Control parent = this.Parent;
        Literal companyNameDisplayLit = (Literal)parent.FindControl("companyNameDisplayLabel");
        companyNameDisplayLit.Text = "(" + company.CompanyName + ")";

        if (Request.QueryString["from"] != null)
        {
            if (Request.QueryString["from"].ToLower().Equals("search"))
            {
                backButton.Text = "BACK TO SEARCH";
                backButton.NavigateUrl = "javascript:history.back();";

                mobileBackBtn.Visible = true;
            }
        }
    }

    private void AddTabToList(Dictionary<string, string> tabList, Tabs tab, string addedUrl)
    {
        string tabName = tab.ToString();
        string url = Request.Url.AbsolutePath + "?tab=" + tabName + (!string.IsNullOrWhiteSpace(addedUrl) ? "&" + addedUrl : "");
        string contributorTabName = tabName;
        if (tab == Tabs.ContactInfo) contributorTabName = "Contact";
        tabList.Add(tabName, url);
    }

    private void DisplayContactTabInformation(MediaRelationsEntities ctx)
    {
        if (currentTab == Tabs.ContactInfo) contactTabDisplay.CssClass = "company-tab view-panel-show";

        companyNameLit.Text = company.CompanyName;
        companyNameLitMobile.Text = company.CompanyName;

        descriptionValue.InnerHtml = (company.CompanyDescription != null ? company.CompanyDescription.Replace("\n", "<br/>") : "");

        lastUpdatedLit.Text = company.ModifiedDate.ToString(CommonMethods.FULL_DATE_STR);

        if (string.IsNullOrWhiteSpace(company.CompanyDescription))
        {
            descriptionLabel.Style.Add("display", "none");
            descriptionValue.Style.Add("display", "none");
        }

        parentCompanyLabel.Style.Add("display", "none");
        parentCompanyValue.Style.Add("display", "none");

        if (company.IsOutlet)
        {
            if (company.ParentCompanyId != null)
            {
                Company parentCompany = (from c in ctx.Companies where c.Id == company.ParentCompanyId && c.IsActive select c).First();
                parentCompanyLabel.Style.Add("display", "");
                parentCompanyValue.Style.Add("display", "");

                parentCompanyValue.InnerHtml = "<a href='" + ResolveUrl("~/Contacts/") + "Company/ViewCompany.aspx?guid=" + parentCompany.Id + "'>" + parentCompany.CompanyName + "</a>";
            }

            Guid parentCompanyId = Guid.Empty;
            if (company.ParentCompanyId != null) parentCompanyId = (Guid)company.ParentCompanyId;
        }

        StringBuilder sb = new StringBuilder();

        Dictionary<PhoneType, bool> phoneChanges = new Dictionary<PhoneType, bool>();
        Dictionary<PhoneType, List<string>> phoneHtml = new Dictionary<PhoneType, List<string>>();

        List<CompanyPhoneNumber> companyPhoneNumbers = company.CompanyPhoneNumbers.OrderBy(x => x.PhoneType.SortOrder).ToList();

        foreach (CompanyPhoneNumber number in companyPhoneNumbers)
        {
            PhoneNumberInfo info = new PhoneNumberInfo(number.PhoneNumber, number.PhoneNumberExtension);

            string className = "";
            string itemValue = "";
            string phoneExtension = null;

            itemValue = number.PhoneNumber;
            phoneExtension = number.PhoneNumberExtension;
            string originalItemValue = itemValue;
            string origiginalExtension = phoneExtension;

            //bool isChange = false;

            if (!phoneChanges.ContainsKey(number.PhoneType)) phoneChanges.Add(number.PhoneType, false);
            if (!string.IsNullOrWhiteSpace(className))
            {
                phoneChanges[number.PhoneType] = true;
            }

            if (!phoneHtml.ContainsKey(number.PhoneType)) phoneHtml.Add(number.PhoneType, new List<string>());
            /*phoneHtml[number.PhoneType].Add("<span class='view-value "+(className == "view-deleted" ? className : "")+"'>"+
                "<a href='tel:"+itemValue+"'>"+CommonMethods.GetFormattedPhoneNumber(itemValue)+"</a></span>\n");*/
            phoneHtml[number.PhoneType].Add("<span class='view-value " + (className == "view-deleted" ? className : "") + "'>" +
                MultiSelectorItem.GetPhoneNumberLink(itemValue) + (!string.IsNullOrWhiteSpace(phoneExtension) ? " ext. " + phoneExtension : "") + "</span>\n");

            /*sb.Append("<div class=\"info-row\"><span class='view-label "+className+"'>"+number.PhoneType.PhoneTypeName+
                ": </span> <span class='view-value "+(className == "view-deleted" ? className : "")+"'>"+
                "<a href='tel:"+itemValue+"'>"+CommonMethods.GetFormattedPhoneNumber(itemValue)+"</a></span></div>\n");*/
        }

        List<KeyValuePair<PhoneType, bool>> sortedList = new List<KeyValuePair<PhoneType, bool>>(phoneChanges);
        sortedList.Sort(delegate (KeyValuePair<PhoneType, bool> a, KeyValuePair<PhoneType, bool> b)
        {
            return a.Key.SortOrder.CompareTo(b.Key.SortOrder);
        });

        foreach (KeyValuePair<PhoneType, bool> pair in sortedList)
        {
            sb.Append("<div class=\"info-row\">\n");

            sb.Append("<span class=\"view-label" + (pair.Value ? " view-changed" : "") + "\">" + pair.Key.PhoneTypeName + "</span>\n");

            //bool first = true;
            foreach (string html in phoneHtml[pair.Key])
            {
                //if (!first) sb.Append("<br/>\n");
                sb.Append(html);
                //first = false;
            }

            sb.Append("</div>\n");
        }

        phoneNumberDisplayLit.Text = sb.ToString();


        sb.Remove(0, sb.Length);
        IEnumerable<ContactMediaJobTitle> companyJobs = ctx.ContactMediaJobTitles.Where(j => j.CompanyId == company.Id && j.Contact.IsActive).ToList();
        foreach (ContactMediaJobTitle job in companyJobs)
        {
            string city = null;
            Contact contact = job.Contact;
            ContactAddress addr = contact.ContactAddresses.FirstOrDefault();
            if (addr != null) city = addr.CityName;

            /*sb.Append("<div><a href='" + ResolveUrl("~/Contacts/") + "Contact/ViewContact.aspx?guid="+contact.Id+"'>"+
                contact.FirstName + " " + contact.LastName + (string.IsNullOrWhiteSpace(city) ? "" : ", " + city) +
                "</a></div>\n");*/

            sb.Append("<div>\n");

            sb.Append("<a href='" + ResolveUrl("~/Contacts/") + "Contact/ViewContact.aspx?guid=" + contact.Id + "'>" + contact.FirstName + " " + contact.LastName + "</a>");

            sb.Append(", " + job.MediaJobTitle.MediaJobTitleName);

            if (contact.ContactBeats.Where(x => x.CompanyId == company.Id).Count() > 0)
            {
                foreach (ContactBeat beat in contact.ContactBeats.Where(x => x.CompanyId == company.Id).OrderBy(x => x.Beat.BeatName).ToList())
                {
                    sb.Append(", " + beat.Beat.BeatName);
                }
            }

            if (!string.IsNullOrEmpty(contact.ShowNotes)) sb.Append(", " + contact.ShowNotes);

            sb.Append("</div>\n");
        }

        if (companyJobs.Count() == 0) contactsLabel.Style.Add("display", "none");

        contactsDisplayLit.Text = sb.ToString();

    }

    private string RenderWebAddressDisplay(MediaRelationsEntities ctx)
    {
        bool didContainEmail = false;

        StringBuilder sb = new StringBuilder();

        List<CompanyWebAddress> companyWebAddresses = company.CompanyWebAddresses.OrderBy(w => w.WebAddressType.SortOrder).ToList();

        Dictionary<WebAddressType, bool> webAddrChange = new Dictionary<WebAddressType, bool>();
        Dictionary<WebAddressType, List<string>> webHtml = new Dictionary<WebAddressType, List<string>>();

        foreach (CompanyWebAddress webAddress in companyWebAddresses)
        {
            if (webAddress.WebAddressType.WebAddressTypeName == WebAddressType.Email)
            {
                didContainEmail = true;
                bool disableEmail = Gcpe.Hub.Configuration.App.Settings.DisableEmail;

                if (disableEmail) emailHref.HRef = emailMobileButton.NavigateUrl = "javascript:alert('this function has been disabled')";
                else emailHref.HRef = emailMobileButton.NavigateUrl = "mailto:" + webAddress.WebAddress;
            }

            string className = "";
            string itemValue = "";

            itemValue = webAddress.WebAddress;
            string webAddressExtraInfo = String.Empty;

            if (webAddress.WebAddressType.WebAddressTypeName == WebAddressType.Email)
            {
                Gcpe.Hub.Services.Legacy.Models.SubscriberInfo subscriberInfo = NodSubscriptions.GetSubscriberInfo(webAddress.WebAddress);
                IList<string> subscriberMdl;
                if (subscriberInfo?.SubscribedCategories != null
                    && subscriberInfo.SubscribedCategories.TryGetValue("media-distribution-lists", out subscriberMdl))
                {
                    foreach (var listKey in subscriberMdl)
                    {
                        webAddressExtraInfo += "<li>" + listKey + "</li>\n";
                    }
                    if (!String.IsNullOrEmpty(webAddressExtraInfo))
                    {
                        webAddressExtraInfo = "<br/>Media Distribution Lists\n <ul>" + webAddressExtraInfo + "</ul>";
                    }
                }
            }


            string originalItemValue = itemValue;

            //bool isChange = false;

            if (!webAddrChange.ContainsKey(webAddress.WebAddressType)) webAddrChange.Add(webAddress.WebAddressType, false);
            if (!string.IsNullOrWhiteSpace(className))
            {
                webAddrChange[webAddress.WebAddressType] = true;
            }

            if (!webHtml.ContainsKey(webAddress.WebAddressType)) webHtml.Add(webAddress.WebAddressType, new List<string>());
            webHtml[webAddress.WebAddressType].Add("<span class='view-value " + (className == "view-deleted" ? "view-deleted" : "") + "'>" +
                MultiSelectorItem.GetLink(webAddress.WebAddressType, itemValue) + webAddressExtraInfo + "</span>\n");


            /*sb.Append("<div  class=\"info-row\"><span class='view-label "+className+"'>"+webAddress.WebAddressType.WebAddressTypeName+
                ":</span> <span class='view-value "+(className == "view-deleted" ? "view-deleted" : "")+"'>"+
                MultiSelectorItem.GetLink(webAddress.WebAddressType, itemValue) + "</span></div>\n");*/
        }

        List<KeyValuePair<WebAddressType, bool>> sortedWA = new List<KeyValuePair<WebAddressType, bool>>(webAddrChange);
        sortedWA.Sort(delegate (KeyValuePair<WebAddressType, bool> a, KeyValuePair<WebAddressType, bool> b)
        {
            return a.Key.SortOrder.CompareTo(b.Key.SortOrder);
        });

        foreach (KeyValuePair<WebAddressType, bool> pair in sortedWA)
        {
            sb.Append("<div class=\"info-row\">\n");

            sb.Append("<span class=\"view-label" + (pair.Value ? " view-changed" : "") + "\">" + pair.Key.WebAddressTypeName + "</span>\n");

            //bool first = true;
            foreach (string html in webHtml[pair.Key])
            {
                //if (!first) sb.Append("<br/>\n");
                sb.Append(html);
                //first = false;
            }

            sb.Append("</div>\n");
        }

        if (!didContainEmail)
        {
            emailHref.HRef = emailMobileButton.NavigateUrl = "javascript:alert(noEmailFoundText.replace('###type###', '" + (company.IsOutlet ? "outlet" : "company") + "'))";
            emailHref.Attributes.Remove("onclick");
            emailMobileButton.Attributes.Remove("onclick");
        }
        return sb.ToString();
    }

    #region location tab display

    private void DisplayLocationTabInformation(MediaRelationsEntities ctx)
    {
        if (currentTab == Tabs.Location) locationTabDisplay.CssClass = "company-tab view-panel-show";

        DisplayPhysicalAddress(ctx);
        DisplayMailingAddress(ctx);

        if (physicalAddressPanel.Visible == false && mailingAddressPanel.Visible == false)
        {
            physicalAddressPanel.Visible = true;
            cityValue.InnerHtml = "&mdash;";
            provinceValue.InnerHtml = "&mdash;";
            countryValue.InnerHtml = "&mdash;";
        }

        DisplayRegions(ctx);
        DisplayElectoralDistricts(ctx);
        DisplaySectors(ctx);
    }

    private void DisplayWebAddressTabInformation(MediaRelationsEntities ctx)
    {
        if (currentTab == Tabs.WebAddress) webAddressTabDisplay.CssClass = "web-address-tab view-panel-show";

        webAddressDisplayTabLit.Text = RenderWebAddressDisplay(ctx);

        // TODO: What black magic do we need to put here for saving things?
    }

    private void DisplayPhysicalAddress(MediaRelationsEntities ctx)
    {
        CompanyAddress physicalAddress = company.CompanyAddresses.Where(x => x.AddressType == (int)CommonMethods.AddressType.Physical).FirstOrDefault();

        // todo dispaly the mailing address info
        bool showPhysicalAddress = true;

        string addressStr = "";
        string cityStr = "";
        string provinceStr = "";
        string countryStr = "";
        string postalCodeStr = "";

        string physicalAddressCityName = "";
        string physicalAddressProvinceName = "";

        if (physicalAddress != null)
        {
            addressStr = physicalAddress.StreetAddress;
            cityStr = physicalAddress.CityName;
            provinceStr = physicalAddress.ProvStateName;
            countryStr = physicalAddress.Country.CountryName;
            postalCodeStr = physicalAddress.PostalZipCode;

            if (physicalAddress.CityName != null) physicalAddressCityName = physicalAddress.CityName;
            if (physicalAddress.ProvStateName != null) physicalAddressProvinceName = physicalAddress.ProvStateName;
        }
        else
        {
            showPhysicalAddress = false;
        }

        if (string.IsNullOrWhiteSpace(addressStr))
        {
            addressLabel.Style.Add("display", "none");
            addressValue.Style.Add("display", "none");
        }

        if (string.IsNullOrWhiteSpace(postalCodeStr))
        {
            postalCodeLabel.Style.Add("display", "none");
            postalCodeValue.Style.Add("display", "none");
        }

        if (showPhysicalAddress)
        {
            addressValue.InnerHtml = (addressStr != null ? addressStr.Replace("\n", "<br/>") : "");
            cityValue.InnerHtml = cityStr;
            provinceValue.InnerHtml = provinceStr;
            countryValue.InnerHtml = countryStr;
            postalCodeValue.InnerHtml = postalCodeStr;
        }
        else
        {
            physicalAddressPanel.Visible = false;
        }
    }

    private void DisplayMailingAddress(MediaRelationsEntities ctx)
    {
        CompanyAddress mailingAddress = company.CompanyAddresses.Where(x => x.AddressType == (int)CommonMethods.AddressType.Mailing).FirstOrDefault();

        // todo display the mailing address info
        bool showMailingAddress = true;
        bool mailingAddressChanges = false;

        string addressStr = "";
        string cityStr = "";
        string provinceStr = "";
        string countryStr = "";
        string postalCodeStr = "";

        mailingAddressLabel.Style.Add("display", "none");
        mailingAddressValue.Style.Add("display", "none");
        mailingCityLabel.Style.Add("display", "none");
        mailingCityValue.Style.Add("display", "none");
        mailingProvinceLabel.Style.Add("display", "none");
        mailingProvinceValue.Style.Add("display", "none");
        mailingCountryLabel.Style.Add("display", "none");
        mailingCountryValue.Style.Add("display", "none");
        mailingPostalCodeLabel.Style.Add("display", "none");
        mailingPostalCodeValue.Style.Add("display", "none");

        string mailingAddressCityName = "";
        string mailingAddressProvinceName = "";

        if (mailingAddress != null)
        {
            addressStr = mailingAddress.StreetAddress;
            cityStr = mailingAddress.CityName;
            provinceStr = mailingAddress.ProvStateName;
            countryStr = mailingAddress.Country.CountryName;
            postalCodeStr = mailingAddress.PostalZipCode;

            if (mailingAddress.CityName != null) mailingAddressCityName = mailingAddress.CityName;
            if (mailingAddress.ProvStateName != null) mailingAddressProvinceName = mailingAddress.ProvStateName;
        }
        else
        {
            showMailingAddress = false;
        }

        if (string.IsNullOrWhiteSpace(postalCodeStr))
        {
            postalCodeLabel.Style.Add("display", "none");
            postalCodeValue.Style.Add("display", "none");
        }

        if (showMailingAddress)
        {
            bool doShowMailingAddress = true;

            if (!mailingAddressChanges)
            {
                doShowMailingAddress = false;

                if (!string.IsNullOrWhiteSpace(addressStr)) doShowMailingAddress = true;
                if (!string.IsNullOrWhiteSpace(cityStr)) doShowMailingAddress = true;
                if (!string.IsNullOrWhiteSpace(provinceStr)) doShowMailingAddress = true;
                if (!string.IsNullOrWhiteSpace(postalCodeStr)) doShowMailingAddress = true;
            }

            if (doShowMailingAddress)
            {
                mailingAddressPanel.Visible = true;

                if (physicalAddressPanel.Visible == true)
                {
                    mailingAddressPanel.Attributes.Add("style", "margin-top: 20px;");
                }

                mailingAddressValue.InnerHtml = (addressStr != null ? addressStr.Replace("\n", "<br/>") : "");
                mailingCityValue.InnerHtml = cityStr;
                mailingProvinceValue.InnerHtml = provinceStr;
                mailingCountryValue.InnerHtml = countryStr;
                mailingPostalCodeValue.InnerHtml = postalCodeStr;

                if (!string.IsNullOrWhiteSpace(addressStr))
                {
                    mailingAddressLabel.Style.Add("display", "");
                    mailingAddressValue.Style.Add("display", "");
                }

                if (!string.IsNullOrWhiteSpace(cityStr))
                {
                    mailingCityLabel.Style.Add("display", "");
                    mailingCityValue.Style.Add("display", "");
                }

                if (!string.IsNullOrWhiteSpace(provinceStr))
                {
                    mailingProvinceLabel.Style.Add("display", "");
                    mailingProvinceValue.Style.Add("display", "");
                }

                if (!string.IsNullOrWhiteSpace(countryStr))
                {
                    mailingCountryLabel.Style.Add("display", "");
                    mailingCountryValue.Style.Add("display", "");
                }

                if (!string.IsNullOrWhiteSpace(postalCodeStr))
                {
                    mailingPostalCodeLabel.Style.Add("display", "");
                    mailingPostalCodeValue.Style.Add("display", "");
                }

            }
        }
        else
        {
            mailingAddressPanel.Visible = false;
        }
    }

    private void DisplayRegions(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        foreach (Region region in company.Regions)
        {
            if (count > 0) sb.Append(", ");
            sb.Append(region.RegionName);
            count++;
        }

        if (company.Regions.Count == 0)
        {
            sb.Append("&mdash;");
        }

        regionValue.InnerHtml = sb.ToString();
    }

    private void DisplayElectoralDistricts(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        foreach (ElectoralDistrict district in company.ElectoralDistricts)
        {
            if (count > 0) sb.Append(", ");
            sb.Append(district.DistrictName);
            count++;
        }

        if (company.ElectoralDistricts.Count == 0)
        {
            sb.Append("&mdash;");
        }

        electoralDistrictValue.InnerHtml = sb.ToString();
    }

    private void DisplaySectors(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        if (company.Sectors.Count == 0)
        {
            sectorsLabel.Style.Add("display", "none");
            sectorsValue.Style.Add("display", "none");
        }

        foreach (Sector sector in company.Sectors)
        {
            if (count > 0) sb.Append(", ");
            sb.Append(sector.DisplayName);
            count++;
        }

        sectorsValue.InnerHtml = sb.ToString();
    }

    #endregion

    #region media tab display

    private void DisplayMediaTabInformation(MediaRelationsEntities ctx)
    {
        if (currentTab == Tabs.Media) mediaTabDisplay.CssClass = "company-tab view-panel-show";

        DisplayMediaDesks(ctx);
        DisplayMediaPartners(ctx);
        DisplayDistribution(ctx);
        DisplayLanguage(ctx);
        DisplayPublicationDays(ctx);
        DisplaySpecialtyPublications(ctx);
        DisplayMediaType(ctx);
        DisplayEthnicities(ctx);

        bool isEthnicMedia = company.IsEthnicMedia == true;
        bool isMajorMedia = company.IsMajorMedia == true;
        bool isLiveMedia = company.IsLiveMedia == true;

        ethnicMediaValue.InnerHtml = (isEthnicMedia ? "Yes" : "No");
        majorMediaValue.InnerHtml = (isMajorMedia ? "Yes" : "No");
        liveMediaOpportunityValue.InnerHtml = (isLiveMedia ? "Yes" : "No");

        if (company.PublicationFrequency == null)
        {
            publicationFrequencyLabel.Style.Add("display", "none");
            publicationFrequencyValue.Style.Add("display", "none");
        }
        else
        {
            publicationFrequencyValue.InnerHtml = company.PublicationFrequency.PublicationFrequencyName;
        }

        keyProgramsValue.InnerHtml = (company.KeyPrograms != null ? company.KeyPrograms.Replace("\n", "<br/>") : "");
        circulationValue.InnerHtml = (company.CirculationDescription != null ? company.CirculationDescription.Replace("\n", "<br/>") : "");
        deadlinesValue.InnerHtml = (company.Deadlines != null ? company.Deadlines.Replace("\n", "<br/>") : "");

        if (string.IsNullOrWhiteSpace(keyProgramsValue.InnerHtml))
        {
            keyProgramsLabel.Style.Add("display", "none");
            keyProgramsValue.Style.Add("display", "none");
        }

        if (string.IsNullOrWhiteSpace(circulationValue.InnerHtml))
        {
            circulationLabel.Style.Add("display", "none");
            circulationValue.Style.Add("display", "none");
        }

        if (string.IsNullOrWhiteSpace(deadlinesValue.InnerHtml))
        {
            deadlinesLabel.Style.Add("display", "none");
            deadlinesValue.Style.Add("display", "none");
        }

    }

    private void DisplayMediaDesks(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        if (company.MediaDesks.Count == 0)
        {
            mediaDeskLabel.Style.Add("display", "none");
            mediaDeskValue.Style.Add("display", "none");
        }

        foreach (var mediaDesk in company.MediaDesks)
        {
            if (count > 0) sb.Append(", ");
            sb.Append(mediaDesk.MediaDeskName);
            count++;
        }

        mediaDeskValue.InnerHtml = sb.ToString();
    }

    private void DisplayMediaPartners(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        if (company.PartnerCompanies.Count == 0)
        {
            mediaPartnersLabel.Style.Add("display", "none");
            mediaPartnersValue.Style.Add("display", "none");
        }

        foreach (Company partner in company.PartnerCompanies)
        {
            if (count > 0) sb.Append(", ");
            sb.Append(partner.CompanyName);
            count++;
        }

        mediaPartnersValue.InnerHtml = sb.ToString();
    }

    private void DisplayDistribution(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        foreach (Distribution dist in company.Distributions)
        {
                if (count > 0) sb.Append(", ");
                sb.Append(dist.DistributionName);
                count++;
        }

        if (company.Distributions.Count == 0)
        {
            sb.Append("&mdash;");
        }

        distributionValue.InnerHtml = sb.ToString();
    }

    private void DisplayLanguage(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        foreach (Language lang in company.Languages)
        {
                if (count > 0) sb.Append(", ");
                sb.Append(lang.LanguageName);
                count++;
        }

        if (company.Languages.Count == 0)
        {
            sb.Append("&mdash;");
        }

        languageValue.InnerHtml = sb.ToString();
    }

    private void DisplayPublicationDays(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        if (company.PublicationDays.Count == 0)
        {
            publicationsDayLabel.Style.Add("display", "none");
            publicationsDayValue.Style.Add("display", "none");
        }

        Dictionary<PublicationDay, string> pubDaysHtml = new Dictionary<PublicationDay, string>();

        foreach (var pubDay in company.PublicationDays)
        {
                //if (count > 0) sb.Append(", ");
                //sb.Append(pubDay.PublicationDaysName);
                pubDaysHtml.Add(pubDay, pubDay.PublicationDaysName);
                //count++;
        }

        List<KeyValuePair<PublicationDay, string>> htmlList = new List<KeyValuePair<PublicationDay, string>>(pubDaysHtml);
        htmlList.Sort(delegate (KeyValuePair<PublicationDay, string> a, KeyValuePair<PublicationDay, string> b)
        {
            return a.Key.SortOrder.CompareTo(b.Key.SortOrder);
        });

        count = 0;
        foreach (KeyValuePair<PublicationDay, string> item in htmlList)
        {
            if (count != 0) sb.Append(", ");
            sb.Append(item.Value);
            count++;
        }

        publicationsDayValue.InnerHtml = sb.ToString();
    }

    private void DisplaySpecialtyPublications(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        if (company.PublicationDays.Count == 0)
        {
            specialtyPublicationsLabel.Style.Add("display", "none");
            specialtyPublicationsValue.Style.Add("display", "none");
        }

        foreach (SpecialtyPublication spub in company.SpecialtyPublications)
        {
                if (count > 0) sb.Append(", ");
                sb.Append(spub.SpecialtyPublicationName);
                count++;
        }

        specialtyPublicationsValue.InnerHtml = sb.ToString();
    }

    private void DisplayMediaType(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        foreach (var mediaType in company.MediaTypes)
        {
            string printCategoryType = "";

            if (mediaType.MediaTypeName == "Print")
            {
                if (company.PrintCategory != null) printCategoryType = company.PrintCategory.PrintCategoryName;
            }

                string updatedPrintCategoryType = printCategoryType;

                if (count > 0) sb.Append(", ");

                if (mediaType.MediaTypeName != "Print")
                {
                    sb.Append(mediaType.MediaTypeName);
                }
                else
                {
                    if (updatedPrintCategoryType != printCategoryType)
                    {
                        //sb.Append("<span class='" + DELETED_CLASS_VALUE + "'>Print (" + printCategoryType + ")</span> " +
                        //"<span class='" + CHANGED_CLASS + "'>Print (" + updatedPrintCategoryType + ")</span>");
                        mediaTypeLabel.Attributes.Add("class", CHANGED_CLASS);
                        sb.Append("Print (" + updatedPrintCategoryType + ")");
                    }
                    else
                    {
                        sb.Append("Print (" + printCategoryType + ")");
                    }

                }

                count++;
        }

        if (company.MediaTypes.Count == 0)
        {
            sb.Append("&mdash;");
        }

        mediaTypeValue.InnerHtml = sb.ToString();
    }

    private void DisplayEthnicities(MediaRelationsEntities ctx)
    {
        StringBuilder sb = new StringBuilder();
        int count = 0;

        if (company.Ethnicities.Count == 0)
        {
            ethnicitiesLabel.Style.Add("display", "none");
            ethnicitiesValue.Style.Add("display", "none");
        }

        foreach (var ethnicity in company.Ethnicities)
        {
                if (count > 0) sb.Append(", ");
                sb.Append(ethnicity.EthnicityName);
                count++;
        }

        ethnicitiesValue.InnerHtml = sb.ToString();
    }

    #endregion

    private void DisplayOutletTabInformation(MediaRelationsEntities ctx)
    {
        if (currentTab == Tabs.Outlets) outletTabDisplay.CssClass = "company-tab view-panel-show";

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
                   where comp.ParentCompanyId == company.Id && comp.IsActive
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

        var objects = LinqDataMethods.OrderBy(obj, sortColumn, isDescending, false);
        PaginatorTop.Count = PaginatorBottom.Count = objects.Count();

        var listOfObjects = objects.Skip(PaginatorTop.Skip).Take(PaginatorTop.PerPage).ToList();

        StringBuilder psb = new StringBuilder();

        int count = 0;
        foreach (var item in listOfObjects)
        {
            string outletHref = "href='" + ResolveUrl("~/Contacts/") + "MediaOutlet/ViewOutlet.aspx?guid=" + item.CompanyId + "'";
            psb.Append("<p><a " + outletHref + ">" + item.CompanyName + "</a></p>\n");

            sb.Append("<tr class='" + (count % 2 == 0 ? "even" : "odd") + "'>\n");
            sb.Append("<td>" + item.CompanyName + "</td>\n");
            sb.Append("<td>" + (string.IsNullOrWhiteSpace(item.CityName) ? "&mdash;" : item.CityName) + "</td>\n");

            string mediaTypeString = "";
            int innerCount = 0;
            foreach (MediaType type in item.MediaTypes)
            {
                if (innerCount != 0) mediaTypeString += "<br/>";
                mediaTypeString += type.MediaTypeName;
                innerCount++;
            }

            sb.Append("<td>" + (string.IsNullOrWhiteSpace(mediaTypeString) ? "&mdash;" : mediaTypeString) + "</td>\n");
            sb.Append("<td>" + (string.IsNullOrWhiteSpace(item.PhoneNumber) ? "&mdash;" : legacy::Gcpe.Hub.Utility.FormatPhoneNumber(item.PhoneNumber)) + "</td>\n");
            sb.Append("<td>" + (string.IsNullOrWhiteSpace(item.Email) ? "&mdash;" : item.Email) + "</td>\n");
            sb.Append("<td>" + (string.IsNullOrWhiteSpace(item.Twitter) ? "&mdash;" : item.Twitter) + "</td>\n");
            sb.Append("<td><a " + outletHref + ">View</a></td>\n");
            sb.Append("</tr>\n");
            count++;
        }

        if (count == 0) sb.Append("<tr><td colspan='9'>No items to display</td></tr>\n");

        outletTableLit.Text = sb.ToString();
        outletsPhoneDisplay.Text = psb.ToString();
    }

    public void ShareButton_Click(object sender, EventArgs e)
    {
        string labelStyle = "font-weight:bold;margin-top:10px;";
        string headingStyle = "font-weight:bold;font-size:14px;";

        var email = Permissions.GetEmailForUser(CommonMethods.GetLoggedInUser());
        Guid guid;
        Guid.TryParse(Request.QueryString["guid"], out guid);
        using (var ctx = new MediaRelationsEntities())
        {
            var cmp = (from c in ctx.Companies where c.Id == guid select c).FirstOrDefault();
            if (cmp != null && email != null)
            {
                bool fst = false;

                StringBuilder sb = new StringBuilder();
                sb.Append("<div style='font-family:Verdana;font-size:14px'>\n");
                //contact tab
                string externalUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                sb.Append("<h2 style='" + headingStyle + "'><a href='" + externalUrl + ResolveUrl("~/Contacts/") + (cmp.IsOutlet ? "MediaOutlet/ViewOutlet.aspx" : "Company/ViewCompany.aspx") + "?guid=" + cmp.Id + "'>" + cmp.CompanyName + "</a></h2>");

                sb.Append("<h3 style='" + headingStyle + "'>Contact Info</h3>\n");

                if (!String.IsNullOrEmpty(cmp.CompanyDescription)) sb.Append("<div style='" + labelStyle + "'>Description</div><div>" + cmp.CompanyDescription + "</div>\n");

                if (cmp.ParentCompanyId != null && cmp.ParentCompanyId != Guid.Empty) sb.Append("<div style='" + labelStyle + "'>Parent Company</div><div>" + (from c in ctx.Companies where c.Id == c.ParentCompanyId select c.CompanyName).FirstOrDefault());

                if (cmp.CompanyWebAddresses.Count > 0)
                {
                    foreach (CompanyWebAddress cwa in cmp.CompanyWebAddresses)
                    {
                        sb.Append("<div style='" + labelStyle + "'>" + cwa.WebAddressType.WebAddressTypeName + "</div><div> " + MultiSelectorItem.GetLink(cwa.WebAddressType, cwa.WebAddress) + "</div>");
                    }
                }
                if (cmp.CompanyPhoneNumbers.Count > 0)
                {
                    foreach (CompanyPhoneNumber cpn in cmp.CompanyPhoneNumbers)
                    {
                        sb.Append("<div style='" + labelStyle + "'>" + cpn.PhoneType.PhoneTypeName + "</div><div> " + MultiSelectorItem.GetPhoneNumberLink(cpn.PhoneNumber) + "</div>");
                    }
                }

                //location tab
                sb.Append("<h3 style='" + headingStyle + "'>Location</h3>\n");
                if (cmp.CompanyAddresses == null || cmp.CompanyAddresses.Count == 0)
                {
                    sb.Append("<div style='" + labelStyle + "'>City</div><div>&mdash;</div>\n");
                    sb.Append("<div style='" + labelStyle + "'>Province/State</div><div>&mdash;</div>\n");
                    sb.Append("<div style='" + labelStyle + "'>Country</div><div>&mdash;</div>\n");
                }
                else
                {
                    foreach (CompanyAddress addr in cmp.CompanyAddresses)
                    {
                        string atyp = ((CommonMethods.AddressType)Enum.ToObject(typeof(CommonMethods.AddressType), addr.AddressType)).ToString();
                        sb.Append("<div style='" + labelStyle + "'>Address " + (atyp == null ? "" : "(" + atyp + ")") + "</div>");
                        if (!String.IsNullOrEmpty(addr.StreetAddress)) sb.Append("<div style='" + labelStyle + "'>Street Address</div><div>" + addr.StreetAddress + "</div>\n");
                        if (addr.City != null) sb.Append("<div style='" + labelStyle + "'>City</div><div>" + addr.City.CityName + "</div> ");
                        else sb.Append("<div style='" + labelStyle + "'>City</div><div>&mdash;</div>\n");

                        if (addr.ProvState != null) sb.Append("<div style='" + labelStyle + "'>Province/State</div><div>" + addr.ProvState.ProvStateName + "</div> ");
                        else sb.Append("<div style='" + labelStyle + "'>Province/State</div><div>&mdash;</div>\n");

                        if (addr.PostalZipCode != null) sb.Append("<div style='" + labelStyle + "'>Postal/Zip Code</div><div>" + addr.PostalZipCode + "</div>");
                        if (addr.Country != null) sb.Append("<div style='" + labelStyle + "'>Country</div><div>" + addr.Country.CountryName + "</div>\n");
                        else sb.Append("<div style='" + labelStyle + "'>Country</div><div>&mdash;</div>\n");
                    }
                }

                sb.Append("<div style='" + labelStyle + "'>Region</div><div> ");
                if (cmp.Regions.Count > 0)
                {
                    fst = false;
                    foreach (Region reg in cmp.Regions)
                    {
                        if (fst) sb.Append(", ");
                        else fst = true;
                        sb.Append(reg.RegionName);
                    }
                }
                else
                {
                    sb.Append("&mdash;");
                }
                sb.Append("</div>\n");

                sb.Append("<div style='" + labelStyle + "'>Electoral District</div><div> ");
                if (cmp.ElectoralDistricts.Count > 0)
                {
                    fst = false;
                    foreach (ElectoralDistrict dst in cmp.ElectoralDistricts)
                    {
                        if (fst) sb.Append(", ");
                        else fst = true;
                        sb.Append(dst.DistrictName);
                    }
                }
                else
                {
                    sb.Append("&mdash;");
                }
                sb.Append("</div>\n");
                if (cmp.Sectors.Count > 0)
                {
                    sb.Append("<div style='" + labelStyle + "'>Sector</div><div> ");
                    fst = false;
                    foreach (Sector sec in cmp.Sectors)
                    {
                        if (fst) sb.Append(", ");
                        else fst = true;
                        sb.Append(sec.DisplayName);
                    }
                    sb.Append("</div>\n");
                }


                if (cmp.IsOutlet)
                {
                    //media tab (outlets only)
                    sb.Append("<h3 style='" + headingStyle + "'>Media</h3>\n");
                    if (cmp.MediaDesks.Count > 0)
                    {
                        sb.Append("<div style='" + labelStyle + "'>Media Desk</div><div> ");
                        fst = false;
                        foreach (MediaDesk dsk in cmp.MediaDesks)
                        {
                            if (fst) sb.Append(", ");
                            else fst = true;
                            sb.Append(dsk.MediaDeskName);
                        }
                        sb.Append("</div>\n");
                    }
                    if (cmp.PartnerCompanies.Count > 0)
                    {
                        sb.Append("<div style='" + labelStyle + "'>Media Partner</div><div> ");
                        fst = false;
                        foreach (Company prt in cmp.PartnerCompanies)
                        {
                            if (fst) sb.Append(", ");
                            else fst = true;
                            sb.Append(cmp.CompanyName);
                        }
                        sb.Append("</div>\n");
                    }
                    sb.Append("<div style='" + labelStyle + "'>Distribution</div><div> ");
                    if (cmp.Distributions.Count > 0)
                    {
                        fst = false;
                        foreach (Distribution dtr in cmp.Distributions)
                        {
                            if (fst) sb.Append(", ");
                            else fst = true;
                            sb.Append(dtr.DistributionName);
                        }

                    }
                    else
                    {
                        sb.Append("&mdash;");
                    }
                    sb.Append("</div>\n");

                    sb.Append("<div style='" + labelStyle + "'>Language</div><div> ");
                    if (cmp.Languages.Count > 0)
                    {
                        fst = false;
                        foreach (Language lng in cmp.Languages)
                        {
                            if (fst) sb.Append(", ");
                            else fst = true;
                            sb.Append(lng.LanguageName);
                        }
                    }
                    else
                    {
                        sb.Append("&mdash;");
                    }
                    sb.Append("</div>\n");

                    if (cmp.PublicationDays.Count > 0)
                    {
                        sb.Append("<div style='" + labelStyle + "'>Publication Days</div><div> ");
                        fst = false;
                        foreach (PublicationDay pdy in cmp.PublicationDays)
                        {
                            if (fst) sb.Append(", ");
                            else fst = true;
                            sb.Append(pdy.PublicationDaysName);
                        }
                        sb.Append("</div>\n");
                    }
                    if (cmp.SpecialtyPublications.Count > 0)
                    {
                        sb.Append("<div style='" + labelStyle + "'>Specialty Publication</div><div> ");
                        fst = false;
                        foreach (SpecialtyPublication spb in cmp.SpecialtyPublications)
                        {
                            if (fst) sb.Append(", ");
                            else fst = true;
                            sb.Append(spb.SpecialtyPublicationName);
                        }
                        sb.Append("</div>\n");
                    }

                    sb.Append("<div style='" + labelStyle + "'>Media Type</div><div> ");
                    if (cmp.MediaTypes.Count > 0)
                    {

                        fst = false;
                        foreach (MediaType mtp in cmp.MediaTypes)
                        {
                            if (fst) sb.Append(", ");
                            else fst = true;
                            sb.Append(mtp.MediaTypeName);
                        }

                    }
                    else
                    {
                        sb.Append("&mdash;");
                    }
                    sb.Append("</div>\n");

                    if (cmp.PrintCategory != null) sb.Append("<div style='" + labelStyle + "'>Print Category</div><div>" + cmp.PrintCategory.PrintCategoryName + "</div>\n");

                    sb.Append("<div style='" + labelStyle + "'>Ethnic Media</div><div> " + (cmp.IsEthnicMedia == true ? "Yes" : "No") + "</div>\n");
                    if (cmp.Ethnicities.Count > 0)
                    {
                        sb.Append("<div style='" + labelStyle + "'>Ethnicities:</div><div> ");
                        fst = false;
                        foreach (Ethnicity eth in cmp.Ethnicities)
                        {
                            if (fst) sb.Append(", ");
                            else fst = true;
                            sb.Append(eth.EthnicityName);
                        }
                        sb.Append("</div>\n");
                    }
                    if (cmp.PublicationFrequency != null && cmp.PublicationFrequencyId != Guid.Empty) sb.Append("<div style='" + labelStyle + "'>Publication Frequency</div><div> " + cmp.PublicationFrequency.PublicationFrequencyName + "</div>\n");

                    sb.Append("<div style='" + labelStyle + "'>Major Media</div><div> " + (cmp.IsMajorMedia == true ? "Yes" : "No") + "</div>\n");
                    sb.Append("<div style='" + labelStyle + "'>Live Media</div><div> " + (cmp.IsLiveMedia == true ? "Yes" : "No") + "</div>\n");
                    if (!String.IsNullOrEmpty(cmp.KeyPrograms)) sb.Append("<div style='" + labelStyle + "'>Key Programs</div><div> "
                        + (cmp.KeyPrograms != null ? cmp.KeyPrograms.Replace("\n", "<br/>") : "") + "</div>\n");
                    if (!String.IsNullOrEmpty(cmp.CirculationDescription)) sb.Append("<div style='" + labelStyle + "'>Key Programs</div><div> "
                        + (cmp.CirculationDescription != null ? cmp.CirculationDescription.Replace("\n", "<br/>") : "") + "</div>\n");
                    if (!String.IsNullOrEmpty(cmp.Deadlines)) sb.Append("<div style='" + labelStyle + "'>Deadlines</div><div> "
                        + (cmp.Deadlines != null ? cmp.Deadlines.Replace("\n", "<br/>") : "") + "</div>\n");
                }
                else
                {
                    //outlets tab (companies only)
                    sb.Append("<h3 style='" + headingStyle + "'>Outlets</h3>\n");
                    if (cmp.PartnerCompanies.Count > 0)
                    {
                        sb.Append("<div><span style='font-weight:bold'>Companies:</span> ");
                        fst = false;
                        foreach (Company otl in cmp.PartnerCompanies.Where(c => c.IsActive))
                        {
                            if (fst) sb.Append(", ");
                            else fst = true;
                            sb.Append(otl.CompanyName);
                        }
                        sb.Append("</div>\n");
                    }
                }
                sb.Append("</div>\n");

                CommonMethods.SendEmail(Gcpe.Hub.Configuration.App.Settings.FromEmailAddress, email, "Media Relations " + (cmp.IsOutlet ? "Outlet" : "Company") + " share", sb.ToString(), true);

                bottomScriptArea.Text = "<script type='text/javascript'>alert(sentShareEmailText.replace(\"###email###\", \"" + email.Replace("\"", "\\\"") + "\"));</script>\n";
            }
        }
    }

    protected void DeleteButtonClick(object sender, EventArgs e)
    {
        string error = CompanyAdministrationLib.DeleteCompany(company.Id);

        if (error == null)
        {
            Response.Redirect("~/Contacts/Default.aspx?message=" + Server.UrlEncode((company.IsOutlet ? "Outlet" : "Company") + " successfully deleted"));
        }
        else
        {
            Response.Redirect(Request.Url.AbsolutePath + "?guid=" + company.Id + "&tab=" + currentTab + "&message=" + Server.UrlEncode("Could not delete this " + (company.IsOutlet ? "outlet: " : "parent company: ") + error));
        }
    }

    protected void EditButtonClick(object sender, EventArgs e)
    {
        string tabString = currentTab.ToString();

        if (!string.IsNullOrWhiteSpace(changedTabHiddenField.Value)) tabString = changedTabHiddenField.Value;

        Response.Redirect((company.IsOutlet ? "~/Contacts/MediaOutlet/EditOutlet.aspx" : "~/Contacts/Company/EditCompany.aspx") + "?guid=" + company.Id + "&tab=" + tabString);
    }

    protected void ConvertToOutletClick(object sender, EventArgs e)
    {
        CompanyAdministrationLib lib = new CompanyAdministrationLib();

        int errors = lib.ConvertCompanyToOutlet(guid);

        if (errors != 0)
        {
            if ((errors & 1) != 0) Response.Redirect(Request.Url.PathAndQuery + "&message=" + Server.UrlEncode("Company not found"));
            if ((errors & 2) != 0) Response.Redirect(Request.Url.PathAndQuery + "&message=" + Server.UrlEncode("This is already a media outlet"));
            if ((errors & 4) != 0) Response.Redirect(Request.Url.PathAndQuery + "&message=" + Server.UrlEncode("An outlet with the company name already exists"));
        }
        else
        {
            Response.Redirect("~/Contacts/MediaOutlet/EditOutlet.aspx?guid=" + guid + "&tab=Media&message=" + Server.UrlEncode("Company has been converted to an outlet. Please fill out this page"));
        }
    }
}

