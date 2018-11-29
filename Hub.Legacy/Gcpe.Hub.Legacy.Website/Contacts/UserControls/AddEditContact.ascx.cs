extern alias legacy;

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;

public partial class UserControls_AddEditContact : System.Web.UI.UserControl
{
    private enum Tabs
    {
        ContactInfo,
        WebAddress,
        Location,
        Media,
        Ministry
    }

    Tabs currentTab = Tabs.ContactInfo;

    public string Mode = "Add";
    Guid guid = Guid.Empty;
    //Permissions.SiteAction userPermissions = 0;
    CommonEventLogging log = null;
    bool isContributor = false;
    bool isAdmin = false;

    bool canCreate = false;
    bool canUpdate = false;
    bool canDelete = false;

    protected void Page_Init(object sender, EventArgs e)
    {
        log = new CommonEventLogging();
        isContributor = Permissions.IsContributor();
        isAdmin = Permissions.IsAdmin();

        canCreate = ((Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsContact, log) & Permissions.SiteAction.Create) != 0);
        canUpdate = ((Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsContact, log) & Permissions.SiteAction.Update) != 0);
        canDelete = ((Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsContact, log) & Permissions.SiteAction.Delete) != 0);

        //set cancel button action
        if (Mode == null || Mode.Equals("Add")) CancelButton.OnClientClick = "return confirm(cancelButtonText);";
        else CancelButton.OnClientClick = "return confirm(cancelAllChangesButtonText);";

        //count of companies to be added to javascript (for validation on tab 3)
        //int companyCount = 0;

        //get guid
        Guid.TryParse(Request.QueryString["guid"], out guid);

        //get current tab
        if (!string.IsNullOrWhiteSpace(Request.QueryString["tab"]))
        {
            if (!Enum.TryParse(Request.QueryString["tab"], out currentTab))
                currentTab = Tabs.ContactInfo;
        }
        if (currentTab == Tabs.ContactInfo) ContactPanel.Visible = true;
        if (currentTab == Tabs.WebAddress) WebAddressPanel.Visible = true;
        if (currentTab == Tabs.Location) LocationPanel.Visible = true;
        if (currentTab == Tabs.Media) MediaPanel.Visible = true;
        if (currentTab == Tabs.Ministry) MinistryPanel.Visible = true;

        //show/hide buttons
        if (Mode == null || Mode.Equals("Add"))
        {
            if (currentTab == Tabs.ContactInfo)
            {
                BackButton.Visible = false;
            }

            if (currentTab == Tabs.Ministry)
            {
                NextButton.Visible = false;
                SaveButton.Visible = true;
            }
            else
            {
                SaveButton.Visible = false;
            }
            SaveButton.Text = "Save";

            TabControl.IsAddScreen = true;
        }
        else
        {
            TabControl.IsAddScreen = false;

            if (currentTab == Tabs.ContactInfo) BackButton.Visible = false;
            else BackButton.Visible = true;

            if (currentTab == Tabs.Ministry) NextButton.Visible = false;
            else NextButton.Visible = true;

            SaveButton.Visible = true;
        }

        //for displaying count of changes on tabs
        Dictionary<string, int> tabChangeCount = new Dictionary<string, int>();

        using (var ctx = new MediaRelationsEntities())
        {
            //if in add mode but contact Guid exists in database, redirect (this is used to redirect after final save in add mode)
            if ((Mode == null || Mode.Equals("Add") && canCreate) && guid != Guid.Empty)
            {
                var existingCon = (from c in ctx.Contacts where c.Id == guid where c.IsActive select c).FirstOrDefault();
                if (existingCon != null)
                {
                    Response.Redirect("~/Contacts/default.aspx?message=Success! Contact has been added.");
                }
            }

            //get contact object
            Contact con = null;
            if (guid != Guid.Empty)
            {
                con = (from c in ctx.Contacts where c.Id == guid where c.IsActive select c).FirstOrDefault();
                if (con == null)
                {
                    if (Session["mru_edit_contact_" + guid] != null)
                    {
                        con = (Contact)Session["mru_edit_contact_" + guid];
                    }
                }
            }

            //if add mode and session contact object cannot be found
            if (con == null)
            {
                if (Mode == null || Mode == "Add")
                {
                    if (currentTab != Tabs.ContactInfo) RedirectToView("&message=Your session has expired and information entered has been lost.");
                }
            }
            else
            {
                BeatSelector.Visible = con.ContactMediaJobTitles.Count > 0;

                if (Mode != null && Mode.Equals("Edit"))
                {
                    Control control = this.Parent;
                    Literal contactNameDisplayLit = (Literal)control.FindControl("contactNameDisplayLabel");
                    contactNameDisplayLit.Text = "(" + con.FirstName + " " + con.LastName + ")";

                    timestamp.Value = con.ModifiedDate.ToOADate().ToString();
                    if (Permissions.IsAdmin() || Permissions.IsContributor())
                    {
                        //ok to edit
                        ErrorLit.Text = "";
                        FormPanel.Visible = true;
                    }
                    else
                    {
                        //no edit
                        ErrorLit.Text = "Only administrators and contributors may edit records";
                        FormPanel.Visible = false;
                        buttonContainerPanel.Visible = false;
                    }
                }
            }

            var outlets = (from c in ctx.Companies where c.IsActive && c.IsOutlet orderby c.CompanyName select c).ToList(); // //outlets only
            Dictionary<string, string> mediaJobTitleItems2 = new Dictionary<string, string>();
            foreach (var o in outlets)
            {
                mediaJobTitleItems2.Add(o.Id.ToString(), o.CompanyName);
            }
            var webAddressTypes = (from s in ctx.WebAddressTypes orderby s.SortOrder select s).ToList();

            cboAddressTypes.DataSource = webAddressTypes;
            cboAddressTypes.DataTextField = "WebAddressTypeName";
            cboAddressTypes.DataValueField = "Id";
            cboAddressTypes.DataBind();


            var phoneTypes = (from s in ctx.PhoneTypes orderby s.SortOrder select s).ToList();
            Dictionary<string, MultiSelectorItem> phoneNumberItems = new Dictionary<string, MultiSelectorItem>();
            foreach (var phoneType in phoneTypes)
            {
                phoneNumberItems.Add(phoneType.Id.ToString(), new MultiSelectorItem(phoneType.PhoneTypeName, false, MultiSelectorItem.PHONE_REGEX, "", true));
            }

            var regions = (from s in ctx.Regions orderby s.RegionName select s).ToList();
            Dictionary<string, MultiSelectorItem> regionItems = new Dictionary<string, MultiSelectorItem>();
            foreach (var rg in regions)
            {
                regionItems.Add(rg.Id.ToString(), new MultiSelectorItem(rg.RegionName, false, "", "", true));
            }

            var electoralDistricts = (from s in ctx.ElectoralDistricts orderby s.DistrictName select s).ToList();
            Dictionary<string, MultiSelectorItem> electoralDistrictItems = new Dictionary<string, MultiSelectorItem>();
            foreach (var ed in electoralDistricts)
            {
                electoralDistrictItems.Add(ed.Id.ToString(), new MultiSelectorItem(ed.DistrictName, false, "", "", true));
            }

            var sectors = (from s in ctx.Sectors where s.IsActive orderby s.DisplayName select s).ToList();
            Dictionary<string, MultiSelectorItem> sectorItems = new Dictionary<string, MultiSelectorItem>();
            foreach (var sec in sectors)
            {
                sectorItems.Add(sec.Id.ToString(), new MultiSelectorItem(sec.DisplayName, false, "", "", true));
            }

            var mediaJobTitles = (from s in ctx.MediaJobTitles orderby s.MediaJobTitleName select s).ToList();
            Dictionary<string, MultiSelectorItem> mediaJobTitleItems = new Dictionary<string, MultiSelectorItem>();
            foreach (var jt in mediaJobTitles)
            {
                mediaJobTitleItems.Add(jt.Id.ToString(), new MultiSelectorItem(jt.MediaJobTitleName, false, "", "", true));
            }

            var beats = (from s in ctx.Beats orderby s.BeatName select s).ToList();
            Dictionary<string, MultiSelectorItem> beatItems = new Dictionary<string, MultiSelectorItem>();
            foreach (var bt in beats)
            {
                beatItems.Add(bt.Id.ToString(), new MultiSelectorItem(bt.BeatName, false, "", "", true));
            }

            var mediaDistributionLists = NodSubscriptions.GetMediaDistributionLists();

            mediaDistributionListBox.DataSource = mediaDistributionLists;
            mediaDistributionListBox.DataTextField = "Value";
            mediaDistributionListBox.DataValueField = "Key";
            mediaDistributionListBox.DataBind();


            var cities = (from s in ctx.Cities orderby s.CityName select s).ToList();
            City.Items.Add(new ListItem("Please Select", ""));
            foreach (var c in cities)
            {
                City.Items.Add(new ListItem(c.CityName, c.Id.ToString()));
            }

            var provinces = (from s in ctx.ProvStates orderby s.ProvStateName select s).ToList();
            Province.Items.Add(new ListItem("Please Select", ""));
            foreach (var prov in provinces)
            {
                Province.Items.Add(new ListItem(prov.ProvStateName, prov.Id.ToString()));
            }

            var countries = (from s in ctx.Countries orderby s.CountryName select s).ToList();
            Country.Items.Add(new ListItem("Please Select", ""));
            foreach (var c in countries)
            {
                Country.Items.Add(new ListItem(c.CountryName, c.Id.ToString()));
            }

            var ministries = (from m in ctx.Ministries where m.IsActive && m.MinisterName != "" orderby m.SortOrder select m).ToList();
            Ministry.Items.Add(new ListItem("Please Select", ""));
            foreach (var ministry in ministries)
            {
                Ministry.Items.Add(new ListItem(ministry.DisplayName, ministry.Id.ToString()));
            }

            //var ministers = (from s in ctx.Ministers where s.IsActive && s.IsActive orderby s.MinisterName select s).ToList();
            //li = new ListItem("Please Select", "");
            //Minister.Items.Add(li);
            //foreach (var x in ministers)
            //{
            //    Minister.Items.Add(new ListItem(x.MinisterName, x.MinisterGuid.ToString()));
            //}

            var ministerialJobTitles = (from s in ctx.MinisterialJobTitles orderby s.MinisterialJobTitleName select s).ToList();
            MinisterialJobTitle.Items.Add(new ListItem("Please Select", ""));
            foreach (var jobTitle in ministerialJobTitles)
            {
                MinisterialJobTitle.Items.Add(new ListItem(jobTitle.MinisterialJobTitleName, jobTitle.Id.ToString()));
            }

            MLAAssignment.Items.Add(new ListItem("Please Select", ""));
            foreach (var edistrict in electoralDistricts)
            {
                MLAAssignment.Items.Add(new ListItem(edistrict.DistrictName, edistrict.Id.ToString()));
            }

            if (con != null)
            {
                Dictionary<string, string> beatItems2 = new Dictionary<string, string>();
                foreach (var jobTitle in con.ContactMediaJobTitles)
                {
                    beatItems2.Add(jobTitle.CompanyId.ToString(), jobTitle.Company.CompanyName);
                }
                BeatSelector.Items2 = beatItems2;

                if (IsPostBack)
                {
                    if (currentTab == Tabs.WebAddress && Session["WebAddressDisplay"] == null)
                    {
                        RedirectToView("&message=Your session has expired and information entered has been lost.");
                        return;
                    }
                }
                else
                {
                    switch (currentTab)
                    {
                        case Tabs.ContactInfo:
                            FirstName.Text = con.FirstName;

                            LastName.Text = con.LastName;

                            ShowNotes.Text = con.ShowNotes;

                            StringBuilder companyErrorSb = new StringBuilder();
                            foreach (ContactMediaJobTitle mjt in con.ContactMediaJobTitles)
                            {
                                MediaJobTitleSelector.SelectedItems.Add(new KeyValuePair<string, string>(mjt.MediaJobTitleId.ToString(), mjt.CompanyId.ToString()));
                                if (!outlets.Any(o => o.Id == mjt.CompanyId))
                                {
                                    companyErrorSb.Append("<a href='" + ResolveUrl("~/Contacts/") + "MediaOutlet/EditOutlet.aspx?guid=" + mjt.CompanyId + "'>" + mjt.Company.CompanyName + "</a><br/>\n");
                                }
                            }
                            if (canUpdate && companyErrorSb.Length > 0)
                            {
                                ErrorLit.Text = ("<div><h2>WARNING <img src='" + ResolveUrl("~/Contacts/") + "images/alerticon.png'/></h2>This contact is attached to the following unapproved outlet:<br/>\n" + companyErrorSb.ToString()) + "<br/>If you continue editing this contact you will lose this outlet attachment. If you wish for the contact to remain associated with this outlet, please approve the outlet first.</div>\n";
                            }

                            MediaJobTitleSelector.Refresh();

                            foreach (ContactPhoneNumber cpn in con.ContactPhoneNumbers)
                            {
                                PhoneNumberInfo pinfo = new PhoneNumberInfo(cpn.PhoneNumber, cpn.PhoneNumberExtension);

                                PhoneNumberSelector.SelectedItems.Add(new KeyValuePair<string, string>(cpn.PhoneTypeId.ToString(), pinfo.PhoneNumberString));
                            }
                            PhoneNumberSelector.Refresh();

                            break;
                        case Tabs.WebAddress:
                            IList<WebAddressDisplay> CWebAddr;

                            CWebAddr = new List<WebAddressDisplay>();
                            foreach (ContactWebAddress cwa in con.ContactWebAddresses)
                            {
                                WebAddressDisplay cd = new WebAddressDisplay(cwa.Id, cwa.WebAddress);
                                cd.WebAddressTypeId = cwa.WebAddressTypeId;
                                cd.WebAddressTypeName = cwa.WebAddressType.WebAddressTypeName;
                                if (cd.WebAddressTypeName == WebAddressType.Email)
                                {
                                    //fetch the media distribution subscriptions from Nods
                                    Gcpe.Hub.Services.Legacy.Models.SubscriberInfo subscriberInfo = NodSubscriptions.GetSubscriberInfo(cwa.WebAddress);

                                    IList<string> subscriberMdl;
                                    if (subscriberInfo?.SubscribedCategories != null
                                        && subscriberInfo.SubscribedCategories.TryGetValue("media-distribution-lists", out subscriberMdl))
                                    {
                                        cd.MediaDistributionLists = subscriberMdl.ToArray();
                                        cd.OriginalMediaSubscriptionCount = cd.MediaDistributionLists.Count();
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

                                    if (subscriberInfo != null && subscriberInfo.ExpiredLinkOrUnverifiedEmail == true)
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
                                    }
                                    otherInfo.Add(emi2);
                                    cd.EmailAddressInfo = otherInfo;
                                }

                                CWebAddr.Add(cd);
                            }

                            Session["WebAddressDisplay"] = CWebAddr;

                            break;
                        case Tabs.Location:
                            ContactAddress contactAddress = con.ContactAddresses.FirstOrDefault();

                            Address.Text = (contactAddress == null ? null : contactAddress.StreetAddress);

                            City.SelectedIndex = City.Items.IndexOf(City.Items.FindByValue((contactAddress == null ? "" : contactAddress.CityId.ToString())));

                            CustomCity.Text = (contactAddress == null ? null : contactAddress.CityName);

                            Province.SelectedIndex = Province.Items.IndexOf(Province.Items.FindByValue((contactAddress == null ? "" : contactAddress.ProvStateId.ToString())));

                            CustomProvince.Text = (contactAddress == null ? null : contactAddress.ProvStateName);

                            Country.SelectedIndex = Country.Items.IndexOf(Country.Items.FindByValue((contactAddress == null ? "" : contactAddress.CountryId.ToString())));

                            PostalCode.Text = (contactAddress == null ? null : contactAddress.PostalZipCode);

                            foreach (Region reg in con.Regions)
                            {
                                regionItems[reg.Id.ToString()].Selected = true;
                            }
                            foreach (ElectoralDistrict dst in con.ElectoralDistricts)
                            {
                                electoralDistrictItems[dst.Id.ToString()].Selected = true;
                            }

                            break;
                        case Tabs.Media:

                            foreach (ContactBeat bet in con.ContactBeats)
                            {
                                //if (mediaJobTitleChanges == null || !mediaJobTitleChanges.ContainsKey(mjt.MediaJobTitleId))
                                BeatSelector.SelectedItems.Add(new KeyValuePair<string, string>(bet.BeatId.ToString(), bet.CompanyId.ToString()));
                            }
                            BeatSelector.Refresh();

                            try
                            {
                                PressGallery.SelectedIndex = PressGallery.Items.IndexOf(PressGallery.Items.FindByValue(con.IsPressGallery ? "true" : "false"));
                            }
                            catch { }

                            break;
                        case Tabs.Ministry:
                            Ministry.SelectedIndex = Ministry.Items.IndexOf(Ministry.Items.FindByValue(con.MinistryId.ToString()));

                            MinisterialJobTitle.SelectedIndex = MinisterialJobTitle.Items.IndexOf(MinisterialJobTitle.Items.FindByValue(con.MinisterialJobTitleId.ToString()));
                            MLAAssignment.SelectedIndex = MLAAssignment.Items.IndexOf(MLAAssignment.Items.FindByValue(con.MLAAssignmentId.ToString()));
                            break;
                    }
                }
            }

            BeatSelector.Items = beatItems;
            PhoneNumberSelector.Items = phoneNumberItems;
            MediaJobTitleSelector.Items = mediaJobTitleItems;
            MediaJobTitleSelector.Items2 = mediaJobTitleItems2;

            RegionSelector.Items = regionItems;
            ElectoralDistrictSelector.Items = electoralDistrictItems;
            SectorSelector.Items = sectorItems;
        }

        var tabs = new Dictionary<string, string>();
        tabs.Add(Tabs.ContactInfo.ToString(), Request.Path + "?tab=" + Tabs.ContactInfo + (guid == Guid.Empty ? "" : "&guid=" + guid));
        tabs.Add(Tabs.WebAddress.ToString(), Request.Path + "?tab=" + Tabs.WebAddress + (guid == Guid.Empty ? "" : "&guid=" + guid));
        tabs.Add(Tabs.Location.ToString(), Request.Path + "?tab=" + Tabs.Location + (guid == Guid.Empty ? "" : "&guid=" + guid));
        tabs.Add(Tabs.Media.ToString(), Request.Path + "?tab=" + Tabs.Media + (guid == Guid.Empty ? "" : "&guid=" + guid));
        tabs.Add(Tabs.Ministry.ToString(), Request.Path + "?tab=" + Tabs.Ministry + (guid == Guid.Empty ? "" : "&guid=" + guid));
        TabControl.Tabs = tabs;

        TabControl.SelectedTab = currentTab.ToString();
        TabControl.OnTabClickEvent = "DoSave";

        //StringBuilder sb = new StringBuilder();
        //sb.Append("var primaryPhoneType = '" + CommonMethods.PrimaryPhoneNumberPhoneType.Id + "';\n");
        //sb.Append("var phoneRegex = /" + MultiSelectorItem.PHONE_REGEX + "/;\n");
        //sb.Append("var companyCount = " + companyCount + ";\n");
        //sb.Append("var ministers=new Array();\n");
        //using (var ctx = new MediaRelationsEntities())
        //{
        //    var ministries = (from s in ctx.Ministries select s).ToList();
        //    foreach (var ministry in ministries)
        //    {
        //        Minister minister = ministry.Ministers.FirstOrDefault();
        //        if (minister != null)
        //        {
        //            sb.Append("ministers['" + ministry.Id + "'] = '" + minister.MinisterGuid + "';\n");
        //        }
        //    }
        //}
        //ScriptLiteral.Text = sb.ToString();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if ((Mode == null || Mode.Equals("Add")) && Session["mru_edit_contact_" + guid] != null)
        {
            AlertLiteral.Text = "doUnloadCheck=true;\n";
        }
    }

    public void ShowErrorMessages(int errors)
    {
        switch (currentTab)
        {
            case Tabs.ContactInfo:
                FirstNameError.InnerText = "";
                LastNameError.InnerText = "";
                PhoneNumberSelector.ErrorMessage = "";
                if (errors != 0)
                {
                    string pnError = "";
                    if ((errors & 1) != 0) FirstNameError.InnerText = "First Name must not be empty";
                    if ((errors & 2) != 0) LastNameError.InnerText = "Last Name must not be empty";
                    if ((errors & 4) != 0) pnError += "Please add at least one phone number (primary is required)<br/>\n";
                    if ((errors & 8) != 0) pnError += "Phone number format is invalid<br/>\n";
                    if ((errors & 16) != 0) pnError += "Please add a primary phone number<br/>\n";

                    if ((errors & 32) != 0) FirstNameError.InnerHtml += "A Contact with that name already exists. All Contacts must have a unique name. If you wish to add this Contact please change the name to be unique.\n";

                    if ((errors & (4 + 8 + 16)) != 0) PhoneNumberSelector.ErrorMessage = pnError;
                }
                break;
            case Tabs.Location:

                AddressError.InnerText = "";
                CountryError.InnerText = "";
                RegionSelector.ErrorMessage = "";
                ElectoralDistrictSelector.ErrorMessage = "";

                if (errors != 0)
                {
                    /*if ((errors & 1) != 0) CityError.InnerText = "Please select a city";
                    if ((errors & 2) != 0) ProvinceError.InnerText = "Please select a province";
                    if ((errors & 8) != 0) RegionSelector.ErrorMessage = "Please add at least one region";
                    if ((errors & 16) != 0) ElectoralDistrictSelector.ErrorMessage = "Please add at least one electoral district";*/
                    if ((errors & 4) != 0) CountryError.InnerText = "Please select a country";
                    if ((errors & 32) != 0) AddressError.InnerText = "Street Address must be 250 characters or less";
                }
                break;
            case Tabs.Media:
                bool pressGallery;
                pressGallery = bool.TryParse(PressGallery.SelectedValue, out pressGallery);

                MediaJobTitleSelector.ErrorMessage = "";

                if (errors != 0)
                {
                    if ((errors & 1) != 0) MediaJobTitleSelector.ErrorMessage = "Please add at least one media job title";
                }
                break;
            case Tabs.Ministry:
                MinistryError.InnerText = "";
                MinisterialJobTitleError.InnerText = "";
                MLAAssignmentError.InnerText = "";

                if (errors != 0)
                {
                    if ((errors & 1) != 0) MinisterialJobTitleError.InnerText = "Please select a Ministerial Job Title";
                    if ((errors & 2) != 0) MLAAssignmentError.InnerText = "Please select an MLA Assignment";
                }
                break;
        }
    }

    private KeyValuePair<Guid, int> UpdateContact()
    {
        ContactAdminLib lib = new ContactAdminLib();
        Contact contact = null;
        int errors = 0;
        using (var ctx = new MediaRelationsEntities())
        {
            if (Mode == null || Mode.Equals("Add"))
            {
                contact = (Contact)Session["mru_edit_contact_" + guid];
                if (contact == null)
                {
                    contact = new Contact();
                    contact.Id = Guid.NewGuid();
                }
            }
            else
            {
                contact = (from con in ctx.Contacts where con.Id == guid where con.IsActive select con).FirstOrDefault();
            }

            switch (currentTab)
            {
                case Tabs.ContactInfo:
                    if (contact != null)
                    {
                        List<KeyValuePair<string, string>> numbers = PhoneNumberSelector.SelectedItems;
                        List<KeyValuePair<string, PhoneNumberInfo>> numbersInfo = new List<KeyValuePair<string, PhoneNumberInfo>>();
                        foreach (KeyValuePair<string, string> pair in numbers)
                        {
                            PhoneNumberInfo phoneInfo = PhoneNumberInfo.GetPhoneNumberInfo(pair.Value);
                            phoneInfo.PhoneNumber = legacy::Gcpe.Hub.Utility.FormatPhoneNumber(phoneInfo.PhoneNumber);
                            numbersInfo.Add(new KeyValuePair<string, PhoneNumberInfo>(pair.Key, phoneInfo));
                        }

                        if (canUpdate || canCreate)
                        {
                            errors = lib.UpdateContactTab1(contact, ctx, FirstName.Text, LastName.Text, MediaJobTitleSelector.SelectedItems, numbersInfo, ShowNotes.Text, (Mode != null && !Mode.Equals("Add")));
                        }

                        if (errors == 0)
                        {
                            // the changes were saved, now save the media advisory subscriptions, if they've changed.

                        }
                        ShowErrorMessages(errors);
                    }
                    break;
                case Tabs.WebAddress:
                    if (contact != null)
                    {
                        if (btnApply.Visible)
                        {
                            btnApply_Click(null, null);
                        }
                        var CWebAddr = (IList<WebAddressDisplay>)Session["WebAddressDisplay"];

                        if (canCreate || canUpdate)
                        {
                            errors = lib.UpdateContactTabWebAddress(contact, ctx, CWebAddr, (Mode != null && !Mode.Equals("Add")));
                        }

                        if (errors == 0)
                        {
                            // the changes were saved, now save the media advisory subscriptions, if they've changed.

                        }
                        ShowErrorMessages(errors);
                    }
                    break;
                case Tabs.Location:
                    if (contact != null)
                    {
                        if (canCreate || canUpdate) errors = lib.UpdateContactTab2(
                             contact,
                             ctx,
                             Address.Text,
                             City.SelectedValue,
                             CustomCity.Text,
                             Province.SelectedValue,
                             CustomProvince.Text,
                             Country.SelectedValue,
                             PostalCode.Text,
                             RegionSelector.SelectedItems,
                             ElectoralDistrictSelector.SelectedItems,
                             SectorSelector.SelectedItems,
                             (Mode != null && !Mode.Equals("Add"))
                             );
                        ShowErrorMessages(errors);
                    }
                    break;
                case Tabs.Media:
                    bool pressGallery;
                    bool.TryParse(PressGallery.SelectedValue, out pressGallery);
                    if (contact != null)
                    {
                        errors = lib.UpdateContactTab3(contact, ctx, pressGallery, BeatSelector.SelectedItems, (Mode != null && !Mode.Equals("Add")));
                        ShowErrorMessages(errors);
                    }
                    break;
                case Tabs.Ministry:
                    if (contact != null)
                    {
                        lib.UpdateContactTab4(contact, ctx, Ministry.SelectedValue, MinisterialJobTitle.SelectedValue, MLAAssignment.SelectedValue, (Mode != null && !Mode.Equals("Add")));

                        ShowErrorMessages(errors);
                    }
                    break;
            }
            if (contact != null)
            {
                if (Mode != null && Mode.Equals("Edit"))
                {
                    if (ctx.ChangeTracker.HasChanges())
                    {
                        bool modifiedByAnotherUser = (timestamp.Value != contact.ModifiedDate.ToOADate().ToString());
                        if (modifiedByAnotherUser)
                        {
                            ErrorLit.Text = "The contact was updated by another user and your changes have not been saved.<br/> Please review your changes below and re-apply them.";
                            errors = -1;
                        }
                        else
                        {
                            contact.RecordEditedBy = CommonMethods.GetLoggedInUser();
                            contact.ModifiedDate = DateTime.Now;
                            ctx.SaveChanges();
                        }
                    }

                    var tabs = new Dictionary<string, string>();
                    tabs.Add(Tabs.ContactInfo.ToString(), Request.Path + "?tab=" + Tabs.ContactInfo + (guid == Guid.Empty ? "" : "&guid=" + guid));
                    tabs.Add(Tabs.Location.ToString(), Request.Path + "?tab=" + Tabs.Location + (guid == Guid.Empty ? "" : "&guid=" + guid));
                    tabs.Add(Tabs.Media.ToString(), Request.Path + "?tab=" + Tabs.Media + (guid == Guid.Empty ? "" : "&guid=" + guid));
                    tabs.Add(Tabs.Ministry.ToString(), Request.Path + "?tab=" + Tabs.Ministry + (guid == Guid.Empty ? "" : "&guid=" + guid));
                    TabControl.Tabs = tabs;

                    TabControl.Refresh();
                }
                return new KeyValuePair<Guid, int>(contact.Id, errors);
            }
        }
        return new KeyValuePair<Guid, int>(Guid.Empty, errors);
    }

    protected void RedirectToView(string message)
    {
        Response.Redirect("~/Contacts/Contact/ViewContact.aspx?guid=" + Request.QueryString["guid"] + "&tab=" + currentTab + message);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Guid.TryParse(Request.QueryString["guid"], out guid);

        if (Mode == null || Mode.Equals("Add"))
        {
            Session["mru_edit_contact_" + guid] = null;
            Response.Redirect("~/Contacts/");
        }
        else
        {
            using (var ctx = new MediaRelationsEntities())
            {
                var contact = (from con in ctx.Contacts where con.Id == guid where con.IsActive select con).FirstOrDefault();
                if (contact != null)
                {
                    RedirectToView("");
                }
                else
                {
                    Response.Redirect("~/Contacts/");
                }
            }
        }
    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
        if ((Mode == null || Mode.Equals("Add")) && canCreate || Mode.Equals("Edit") && canUpdate)
        {
            KeyValuePair<Guid, int> validation = UpdateContact();
            Guid contactGuid = validation.Key;
            //save and continue to next tab
            Tabs nextTab = Tabs.ContactInfo;
            switch (currentTab)
            {
                case Tabs.ContactInfo:
                    nextTab = Tabs.ContactInfo;
                    break;
                case Tabs.WebAddress:
                    nextTab = Tabs.ContactInfo;
                    break;
                case Tabs.Location:
                    nextTab = Tabs.WebAddress;
                    break;
                case Tabs.Media:
                    nextTab = Tabs.Location;
                    break;
                case Tabs.Ministry:
                    nextTab = Tabs.Media;
                    break;
            }
            if (validation.Value == 0) Response.Redirect(Request.Path + "?tab=" + nextTab + (contactGuid == Guid.Empty ? "" : "&guid=" + contactGuid));
        }
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        if ((Mode == null || Mode.Equals("Add")) && canCreate || Mode.Equals("Edit") && canUpdate)
        {
            KeyValuePair<Guid, int> validation = UpdateContact();
            Guid contactGuid = validation.Key;
            //save and continue to next tab
            Tabs nextTab = Tabs.ContactInfo;
            switch (currentTab)
            {
                case Tabs.ContactInfo:
                    nextTab = Tabs.WebAddress;
                    break;
                case Tabs.WebAddress:
                    nextTab = Tabs.Location;
                    break;
                case Tabs.Location:
                    nextTab = Tabs.Media;
                    break;
                case Tabs.Media:
                    nextTab = Tabs.Ministry;
                    break;
                case Tabs.Ministry:
                    nextTab = Tabs.Ministry;
                    break;
            }
            if (validation.Value == 0) Response.Redirect(Request.Path + "?tab=" + nextTab + (contactGuid == Guid.Empty ? "" : "&guid=" + contactGuid));
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        string hid = Request.Form["tabHiddenField"];
        if (Mode == null || Mode.Equals("Add") && canCreate)
        {
            KeyValuePair<Guid, int> updateResult = UpdateContact();
            Guid contactGuid = updateResult.Key;
            int errors = updateResult.Value;

            if (errors == 0)
            {
                //save and continue to next tab
                Tabs nextTab = currentTab;

                switch (currentTab)
                {
                    case Tabs.ContactInfo:
                        nextTab = Tabs.Location;
                        break;
                    case Tabs.Location:
                        nextTab = Tabs.Media;
                        break;
                    case Tabs.Media:
                        nextTab = Tabs.Ministry;
                        break;
                    case Tabs.Ministry:
                        ContactAdminLib lib = new ContactAdminLib();
                        Contact con = (Contact)Session["mru_edit_contact_" + guid];
                        if (con == null)
                        {
                            con = new Contact();
                            con.Id = Guid.NewGuid();
                        }

                        contactGuid = lib.FinalizeContact(con);
                        Session["mru_edit_contact_" + guid] = null;
                        nextTab = Tabs.ContactInfo;
                        break;
                }
                Response.Redirect(Request.Path + "?tab=" + nextTab + (contactGuid == Guid.Empty ? "" : "&guid=" + contactGuid));
            }
        }
        else if (canUpdate)
        {
            KeyValuePair<Guid, int> updateResult = UpdateContact();
            Guid contactGuid = updateResult.Key;
            int errors = updateResult.Value;

            if (errors == 0)
            {
                if (string.IsNullOrEmpty(hid))
                {
                    RedirectToView("&message=Changes have been saved");
                }
                else
                {
                    Response.Redirect(Request.Path + "?guid=" + contactGuid + "&tab=" + hid);
                }
            }
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

    protected void GridViewWebAddress_SelectWebAddress(object sender, GridViewSelectEventArgs e)
    {
        WebAddressEditPanel.Visible = true;
        btnAddWebAddress.Visible = !WebAddressEditPanel.Visible;

        var CWebAddr = (List<WebAddressDisplay>)Session["WebAddressDisplay"];
        WebAddressDisplay rowItem = CWebAddr.ElementAt(e.NewSelectedIndex);
        txtWebAddress.Text = rowItem.NewWebAddress;
        GridViewOtherInfo.DataSource = rowItem.EmailAddressInfo;
        GridViewOtherInfo.DataBind();
        if (string.IsNullOrEmpty(rowItem.WebAddress))
        {
            GridViewOtherInfoLabel.Text = "";
        }
        else
        {
            GridViewOtherInfoLabel.Text = "Details for: " + rowItem.WebAddress;
        }

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

        GridViewOtherInfoLabel.Text = "";
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

    protected void btnAddWebAddress_Click(object sender, EventArgs e)
    {
        var CWebAddr = (List<WebAddressDisplay>)Session["WebAddressDisplay"];
        CWebAddr.Add(new WebAddressDisplay(Guid.NewGuid()));
        GridViewWebAddress.DataBind();
        GridViewSelectEventArgs e1 = new GridViewSelectEventArgs(CWebAddr.Count - 1);
        GridViewWebAddress.SelectedIndex = CWebAddr.Count - 1;
        GridViewWebAddress_SelectWebAddress(null, e1);
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
}
