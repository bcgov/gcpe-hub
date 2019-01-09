using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaRelationsLibrary;
using MediaRelationsDatabase;
using System.Text;
using Gcpe.Hub.Properties;

public partial class UserControls_ViewContact : System.Web.UI.UserControl
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

    public string Mode = "View";

    protected Guid guid = Guid.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        Guid.TryParse(Request.QueryString["guid"], out guid);

        if (!Enum.TryParse(Request.QueryString["tab"], out currentTab))
            currentTab = Tabs.ContactInfo;

        Dictionary<string, string> tabs = new Dictionary<string, string>();
        tabs.Add(Tabs.ContactInfo.ToString(), Request.Path + "?tab=" + Tabs.ContactInfo + (guid == Guid.Empty ? "" : "&guid=" + guid));
        tabs.Add(Tabs.WebAddress.ToString(), Request.Path + "?tab=" + Tabs.WebAddress + (guid == Guid.Empty ? "" : "&guid=" + guid));
        tabs.Add(Tabs.Location.ToString(), Request.Path + "?tab=" + Tabs.Location + (guid == Guid.Empty ? "" : "&guid=" + guid));
        tabs.Add(Tabs.Media.ToString(), Request.Path + "?tab=" + Tabs.Media + (guid == Guid.Empty ? "" : "&guid=" + guid));
        tabs.Add(Tabs.Ministry.ToString(), Request.Path + "?tab=" + Tabs.Ministry + (guid == Guid.Empty ? "" : "&guid=" + guid));
        TabControl.Tabs = tabs;

        TabControl.SelectedTab = currentTab.ToString();

        if (Mode == null || Mode.Equals("View")) TabControl.OnTabClickEvent = "ChangeCurrentTab";

        if (currentTab == Tabs.ContactInfo) ContactPanel.CssClass = "contact-tab view-panel-show";
        if (currentTab == Tabs.WebAddress) WebAddressPanel.CssClass = "contact-tab view-panel-show";
        if (currentTab == Tabs.Location) LocationPanel.CssClass = "contact-tab view-panel-show";
        if (currentTab == Tabs.Media) MediaPanel.CssClass = "contact-tab view-panel-show";
        if (currentTab == Tabs.Ministry) MinistryPanel.CssClass = "contact-tab view-panel-show";

        bool isAdmin = Permissions.IsAdmin();
        bool isContributor = Permissions.IsContributor();

        TopButtonPanel.Visible = true;
        if (isAdmin || isContributor)
        {
            BottomButtonPanel.Visible = true;
        }
        else
        {

            BottomButtonPanel.Visible = false;
        }

        ContactAdminLib lib = new ContactAdminLib();

        using (var ctx = new MediaRelationsEntities())
        {
            Contact con = null;
            try
            {
                con = (from c in ctx.Contacts where c.Id == guid where c.IsActive select c).FirstOrDefault();
            }
            catch { }


            if (guid == Guid.Empty)
            {
                ErrorLit.Text = "Please specify a contact to view";
                ViewContactControl.Visible = false;
            }
            else if (con == null)
            {
                ErrorLit.Text = "The specified contact does not exist";
                ViewContactControl.Visible = false;
            }
            else
            {
                Control control = this.Parent;
                Literal contactNameDisplayLit = (Literal)control.FindControl("contactNameDisplayLabel");
                contactNameDisplayLit.Text = "(" + con.FirstName + " " + con.LastName + ")";

                /** back button setup **/
                if (Request.QueryString["from"] != null)
                {
                    if (Request.QueryString["from"].Trim().ToLower().Equals("search"))
                    {
                        backButtonHref.HRef = "javascript:history.back();";
                        backButtonHref.InnerHtml = "BACK TO SEARCH";

                        mobileBackBtn.Visible = true;
                    }
                }

                ContactWebAddress email = con.Email;

                if (email == null)
                {
                    emailMobileHref.NavigateUrl = emailHref.NavigateUrl = "javascript:alert(noEmailFoundText.replace('###type###', 'contact'));";
                    emailHref.Attributes.Remove("onclick");
                    emailMobileHref.Attributes.Remove("onclick");
                }
                else
                {
                    bool disableEmail = Settings.Default.DisableEmail;

                    if (disableEmail)
                    {
                        emailMobileHref.NavigateUrl = emailHref.NavigateUrl = "javascript:alert('this function has been disabled')";
                    }
                    else
                    {
                        emailMobileHref.NavigateUrl = emailHref.NavigateUrl = "mailto:" + email.WebAddress;
                    }
                }

                LastUpdatedDate.Text = con.ModifiedDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR);

                BottomButtonPanel.Visible = false;

                shareLink.Visible = true;
                shareSeparator.Visible = true;
                emailLink.Visible = true;
                emailSeparator.Visible = true;
                printLink.Visible = true;

                if (isAdmin || isContributor)
                {
                    if (isAdmin)
                    {
                        DeleteButton.Visible = true;
                        printSeparator.Visible = true;
                        if (con.MediaRequestContacts.Count == 0)
                        {
                            DeleteButton.OnClientClick = "return confirm(deleteButtonText)";
                        }
                        else
                        {
                            DeleteButton.OnClientClick = "var contact = prompt('This contact is associated with media requests.\\n\\n1. To keep historical requests click OK.\\nOR\\n2. To assign these to the correct reporter, enter first and last name:', '" + con.FirstName + " " + con.LastName + "'); if (contact == null)return false; $('#hidAltContact').val(contact)";
                        }
                    }
                    else
                    {
                        DeleteButton.Visible = false;
                        printSeparator.Visible = false;
                    }
                    if (isAdmin || isContributor)
                    {
                        historyHref.Visible = true;
                        historySeparator.Visible = true;
                        historyFrame.Src = ResolveUrl("~/Contacts/") + "RecordHistory.aspx?type=contact&guid=" + guid;

                        EditButton.Visible = true;
                        editSeparator.Visible = true;

                        CancelButton.Visible = false;
                        cancelSeparator.Visible = false;
                    }
                }
                else
                {
                    EditButton.Visible = false;
                    editSeparator.Visible = false;
                    CancelButton.Visible = false;
                    cancelSeparator.Visible = false;
                }


                bool fst = false;

                StringBuilder sb = new StringBuilder();

                //universal
                ContactName.Text = con.FirstName + " " + con.LastName;
                contactNameLitMobile.Text = con.FirstName + " " + con.LastName;


                //contact tab

                //media job titles
                if (con.ContactMediaJobTitles.Count > 0)
                {
                    fst = false;
                    sb.Append("<div>");
                    sb.Append("<span class='view-label" + "'>Media Job Title: </span>");
                    foreach (ContactMediaJobTitle mjt in con.ContactMediaJobTitles.OrderBy(t => t.MediaJobTitle.MediaJobTitleName))
                    {
                        if (fst) sb.Append(", ");
                        else fst = true;
                        string editClass = null;

                        sb.Append("<span class='" + (editClass != null && editClass.Equals("view-deleted") ? " " + editClass : "") + "'>" + mjt.MediaJobTitle.MediaJobTitleName.Trim() + ": " + mjt.Company.CompanyName.Trim() + "</span>");
                    }
                    sb.Append("</div>");
                }

                //phone numbers
                if (con.ContactPhoneNumbers.Count > 0)
                {
                    Dictionary<PhoneType, bool> phoneChanges = new Dictionary<PhoneType, bool>();
                    Dictionary<PhoneType, List<string>> phoneChangesHtml = new Dictionary<PhoneType, List<string>>();
                    foreach (ContactPhoneNumber cpn in con.ContactPhoneNumbers.OrderBy(t => t.PhoneType.SortOrder))
                    {
                        PhoneNumberInfo info = new PhoneNumberInfo(cpn.PhoneNumber, cpn.PhoneNumberExtension);

                        string editClass = null;

                        if (!string.IsNullOrWhiteSpace(editClass))
                        {
                            if (phoneChanges.ContainsKey(cpn.PhoneType)) phoneChanges[cpn.PhoneType] = true;
                            else phoneChanges.Add(cpn.PhoneType, true);
                        }
                        else
                        {
                            if (!phoneChanges.ContainsKey(cpn.PhoneType)) phoneChanges.Add(cpn.PhoneType, false);
                        }

                        if (!phoneChangesHtml.ContainsKey(cpn.PhoneType)) phoneChangesHtml.Add(cpn.PhoneType, new List<string>());
                        phoneChangesHtml[cpn.PhoneType].Add("<span class='" + (editClass != null && editClass.Equals("view-deleted") ? " " + editClass : "") + "'>" + MultiSelectorItem.GetPhoneNumberLink(info.PhoneNumber) + (!string.IsNullOrWhiteSpace(info.PhoneNumberExtension) ? " ext. " + info.PhoneNumberExtension : "") + "</span>");


                    }

                    List<KeyValuePair<PhoneType, List<string>>> sortedHtml = new List<KeyValuePair<PhoneType, List<string>>>(phoneChangesHtml);
                    sortedHtml.Sort(delegate (KeyValuePair<PhoneType, List<string>> a, KeyValuePair<PhoneType, List<string>> b)
                    {
                        return a.Key.SortOrder.CompareTo(b.Key.SortOrder);
                    });

                    foreach (KeyValuePair<PhoneType, bool> pair in phoneChanges)
                    {
                        sb.Append("<div>\n");
                        sb.Append("<span class='view-label" + (pair.Value ? " view-changed" : "") + "'>" + pair.Key.PhoneTypeName + ": </span>");

                        bool first = true;
                        foreach (string html in phoneChangesHtml[pair.Key])
                        {
                            if (!first) sb.Append("<br/>\n");
                            sb.Append(html);
                            first = false;
                        }

                        sb.Append("</div>\n");
                    }

                }

                ContactLiteral.Text = sb.ToString();

                //WebAddress Tab

                sb.Remove(0, sb.Length);

                RenderWebAddressViewHtml(ctx, con, sb);

                WebAddressLiteral.Text = sb.ToString();

                //location tab
                sb.Remove(0, sb.Length);

                ContactAddress contactAddress = (from s in con.ContactAddresses select s).FirstOrDefault();

                if ((contactAddress != null && !string.IsNullOrWhiteSpace(contactAddress.StreetAddress)))
                {
                    sb.Append("<div>");
                    sb.Append("<span class='view-label" + "'>Street Address: </span>");
                    sb.Append("<span class='view-value'>" + (contactAddress == null ? "&mdash;" : contactAddress.StreetAddress.Replace("\n", "<br/>")) + "</span>");
                    sb.Append("</div>");
                }

                string cityNameStr = (contactAddress == null ? null : contactAddress.CityName);
                sb.Append("<div>");
                sb.Append("<span class='view-label" + "'>City: </span>");
                sb.Append("<span class='view-value'>" + (string.IsNullOrWhiteSpace(cityNameStr) ? "&mdash;" : cityNameStr) + "</span>");
                sb.Append("</div>");

                string provNameStr = (contactAddress == null ? null : contactAddress.ProvStateName);
                sb.Append("<div>");
                sb.Append("<span class='view-label" + "'>Province: </span>");
                sb.Append("<span class='view-value'>" + (string.IsNullOrWhiteSpace(provNameStr) ? "&mdash;" : provNameStr) + "</span>");
                sb.Append("</div>");

                string countryNameStr = (contactAddress == null ? null : contactAddress.Country.CountryName);
                sb.Append("<div>");
                sb.Append("<span class='view-label" + "'>Country: </span>");
                sb.Append("<span class='view-value'>" + (string.IsNullOrWhiteSpace(countryNameStr) ? "&mdash;" : countryNameStr) + "</span>");
                sb.Append("</div>");

                if ((contactAddress != null && !string.IsNullOrWhiteSpace(contactAddress.PostalZipCode)))
                {
                    sb.Append("<div>");
                    sb.Append("<span class='view-label" + "'>Postal Code: </span>");
                    sb.Append("<span class='view-value'>" + contactAddress.PostalZipCode + "</span>");
                    sb.Append("</div>");
                }

                //regions
                bool wereRegions = false;
                fst = false;
                sb.Append("<div>");
                sb.Append("<span class='view-label" + "'>Region: </span>");
                foreach (Region reg in con.Regions.OrderBy(t => t.RegionName))
                {
                    if (fst) sb.Append(", ");
                    else fst = true;
                    string editClass = null;

                    sb.Append("<span class='" + (editClass != null && editClass.Equals("view-deleted") ? " " + editClass : "") + "'>" + reg.RegionName.Trim() + "</span>");
                    wereRegions = true;
                }

                if (!wereRegions) sb.Append("<span>&mdash;</span>");

                sb.Append("</div>");

                //electoral districts
                bool wereDistricts = false;
                fst = false;
                sb.Append("<div>");
                sb.Append("<span class='view-label" + "'>Electoral District: </span>");
                foreach (ElectoralDistrict dst in con.ElectoralDistricts.OrderBy(t => t.DistrictName))
                {
                    if (fst) sb.Append(", ");
                    else fst = true;
                    string editClass = null;

                    sb.Append("<span class='" + (editClass != null && editClass.Equals("view-deleted") ? " " + editClass : "") + "'>" + dst.DistrictName.Trim() + "</span>");
                    wereDistricts = true;
                }

                if (!wereDistricts) sb.Append("<span>&mdash;</span>");

                sb.Append("</div>");

                //sectors
                bool wereSectors = false;

                if (con.Sectors.Count > 0)
                {
                    fst = false;
                    sb.Append("<div>");
                    sb.Append("<span class='view-label" + "'>Sector: </span>");
                    foreach (Sector sector in con.Sectors.OrderBy(sec => sec.DisplayName))
                    {
                        if (fst) sb.Append(", ");
                        else fst = true;
                        string editClass = null;

                        sb.Append("<span class='" + (editClass != null && editClass.Equals("view-deleted") ? " " + editClass : "") + "'>" + sector.DisplayName.Trim() + "</span>");
                        wereSectors = true;
                    }

                    if (!wereSectors) sb.Append("<span>&mdash;</span>");
                    sb.Append("</div>");
                }

                LocationLiteral.Text = sb.ToString();

                //media tab
                sb.Remove(0, sb.Length);
                //beats
                fst = false;

                if (con.ContactBeats.Count > 0)
                {
                    sb.Append("<div>");
                    sb.Append("<span class='view-label" + "'>Beat: </span>");
                    foreach (ContactBeat bet in con.ContactBeats.OrderBy(t => t.Beat.BeatName))
                    {
                        if (fst) sb.Append(", ");
                        else fst = true;
                        string editClass = null;

                        sb.Append("<span class='" + (editClass != null && editClass.Equals("view-deleted") ? " " + editClass : "") + "'>" + bet.Beat.BeatName.Trim() + ": " + bet.Company.CompanyName.Trim() + "</span>");
                    }
                    sb.Append("</div>");
                }

                bool ipg = con.IsPressGallery;

                string pressGalleryStr = (con.IsPressGallery ? "Yes" : "No");

                sb.Append("<div>");
                sb.Append("<span class='view-label" + "'>Press Gallery: </span>");
                sb.Append("<span class='view-value'>" + pressGalleryStr + "</span>");
                sb.Append("</div>");

                MediaLiteral.Text = sb.ToString();

                //ministry tab
                sb.Remove(0, sb.Length);

                if (con.MinistryId != null)
                {
                    string ministryNameStr = (con.Ministry == null ? null : con.Ministry.DisplayName);
                    sb.Append("<div>");
                    sb.Append("<span class='view-label" + "'>Ministry Name: </span>");
                    sb.Append("<span class='view-value'>" + ministryNameStr + "</span>");
                    sb.Append("</div>");
                }

                if (con.MinisterialJobTitleId != null)
                {
                    string ministerialJobTitleStr = (con.MinisterialJobTitle == null ? null : con.MinisterialJobTitle.MinisterialJobTitleName);
                    sb.Append("<div>");
                    sb.Append("<span class='view-label" + "'>Ministerial Job Title: </span>");
                    sb.Append("<span class='view-value'>" + ministerialJobTitleStr + "</span>");
                    sb.Append("</div>");
                }

                if (con.MLAAssignmentId != null)
                {
                    string mlaAssignmentStr = null;
                    var mlaAssign = (from dist in ctx.ElectoralDistricts where dist.Id == con.MLAAssignmentId select dist).FirstOrDefault();
                    if (mlaAssign != null) mlaAssignmentStr = mlaAssign.DistrictName;
                    sb.Append("<div>");
                    sb.Append("<span class='view-label" + "'>MLA Assignment: </span>");
                    sb.Append("<span class='view-value'>" + mlaAssignmentStr + "</span>");
                    sb.Append("</div>");
                }


                string ministerAssignmentStr = (con.HasMinisterAssignment ? "Yes" : "No");
                sb.Append("<div>");
                sb.Append("<span class='view-label" + "'>Minister Assignment: </span>");
                sb.Append("<span class='view-value'>" + ministerAssignmentStr + "</span>");
                sb.Append("</div>");

                string primaryMediaStr = (con.IsPrimaryMediaContact ? "Yes" : "No");
                sb.Append("<div>");
                sb.Append("<span class='view-label" + "'>Primary Media Contact: </span>");
                sb.Append("<span class='view-value'>" + primaryMediaStr + "</span>");
                sb.Append("</div>");

                string secondaryMediaStr = (con.IsSecondaryMediaContact ? "Yes" : "No");
                sb.Append("<div>");
                sb.Append("<span class='view-label" + "'>Secondary Media Contact: </span>");
                sb.Append("<span class='view-value'>" + secondaryMediaStr + "</span>");
                sb.Append("</div>");

                MinistryLiteral.Text = sb.ToString();

            }
        }
    }

    private static void RenderWebAddressViewHtml(MediaRelationsEntities ctx, Contact con, StringBuilder sb)
    {
        //web addresses
        if (con.ContactWebAddresses.Count > 0)
        {
            Dictionary<WebAddressType, bool> webAddrChanges = new Dictionary<WebAddressType, bool>();
            Dictionary<WebAddressType, List<string>> webAddrHtml = new Dictionary<WebAddressType, List<string>>();

            foreach (ContactWebAddress cwa in con.ContactWebAddresses.OrderBy(t => t.WebAddressType.SortOrder))
            {
                string thisHtml = "";

                string editClass = null;


                if (string.IsNullOrWhiteSpace(editClass))
                {
                    if (!webAddrChanges.ContainsKey(cwa.WebAddressType))
                    {
                        webAddrChanges.Add(cwa.WebAddressType, false);
                    }
                }

                string link = MultiSelectorItem.GetLink(cwa.WebAddressType, cwa.WebAddress.Trim());

                thisHtml = "<span class='" + (editClass != null && editClass.Equals("view-deleted") ? " " + editClass : "") + "'>" + link;
                if (cwa.WebAddressType.WebAddressTypeName == WebAddressType.Email)
                {
                    //let's ask for the current subscriptions

                    Gcpe.Hub.Services.Legacy.Models.SubscriberInfo subscriberInfo = NodSubscriptions.GetSubscriberInfo(cwa.WebAddress.Trim());
                    IList<string> subscriberMdl;
                    if (subscriberInfo?.SubscribedCategories != null
                        && subscriberInfo.SubscribedCategories.TryGetValue("media-distribution-lists", out subscriberMdl))
                    {
                        thisHtml += "<br/>Media Distribution Lists <ul>";
                        foreach (var listKey in subscriberMdl)
                        {
                            thisHtml += "<li>" + listKey + "</li>";
                        }
                        thisHtml += "</ul>";
                    }

                }
                thisHtml += "</span>";
                if (!webAddrHtml.ContainsKey(cwa.WebAddressType)) webAddrHtml.Add(cwa.WebAddressType, new List<string>());
                webAddrHtml[cwa.WebAddressType].Add(thisHtml);
            }

            List<KeyValuePair<WebAddressType, bool>> sortedWebAddrChanges = new List<KeyValuePair<WebAddressType, bool>>(webAddrChanges);
            sortedWebAddrChanges.Sort(delegate (KeyValuePair<WebAddressType, bool> a, KeyValuePair<WebAddressType, bool> b)
            {
                return a.Key.SortOrder.CompareTo(b.Key.SortOrder);
            });

            // display
            foreach (KeyValuePair<WebAddressType, bool> pair in sortedWebAddrChanges)
            {
                sb.Append("<div>");
                sb.Append("<span class='view-label" + (pair.Value ? " view-changed" : "") + "'>" + pair.Key.WebAddressTypeName + ": </span>");

                bool first = true;
                foreach (string html in webAddrHtml[pair.Key])
                {
                    if (!first) sb.Append("<br/>\n");
                    sb.Append(html);
                    first = false;
                }
                sb.Append("</div>");
            }
        }
    }

    public void ShareButton_Click(object sender, EventArgs e)
    {
        string labelStyle = "font-weight:bold;margin-top:10px;";
        string headingStyle = "font-weight:bold;font-size:14px;";
        Guid guid;
        Guid.TryParse(Request.QueryString["guid"], out guid);
        using (var ctx = new MediaRelationsEntities())
        {
            Contact con = (from c in ctx.Contacts where c.Id == guid where c.IsActive select c).FirstOrDefault();

            string email = Permissions.GetEmailForUser(CommonMethods.GetLoggedInUser());
            if (con != null && email != null)
            {
                StringBuilder sb = new StringBuilder();
                bool fst = false;

                //contact tab
                sb.Append("<div style='font-family:Verdana;font-size:14px'>\n");
                string contactRoot = ResolveUrl("~/Contacts/");
                string externalUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                sb.Append("<h2 style='" + headingStyle + "'><a href='" + externalUrl + contactRoot + "Contact/ViewContact.aspx?guid=" + con.Id + "'>" + con.FirstName + " " + con.LastName + "</a></h2>");

                sb.Append("<h3 style='" + headingStyle + "'>Contact Info</h3>\n");

                /*if (con.Companies.Count > 0)
                {
                    fst = false;
                    sb.Append("<div style='" + labelStyle + "'>Outlets</div><div> ");
                    foreach (Company cmp in con.Companies)
                    {
                        if (fst) sb.Append(", ");
                        else fst = true;
                        sb.Append("<a href='" + externalUrl + contactRoot + (cmp.IsOutlet ? "MediaOutlet/ViewOutlet.aspx" : "Company/ViewCompany.aspx") + "?guid=" + cmp.Id + "'>" + cmp.CompanyName + "</a>");
                    }
                    sb.Append("</div>\n");
                }*/
                if (con.ContactBeats.Count > 0)
                {
                    fst = false;
                    sb.Append("<div style='" + labelStyle + "'>Beats</div><div> ");
                    foreach (ContactBeat bet in con.ContactBeats)
                    {
                        if (fst) sb.Append(", ");
                        else fst = true;
                        sb.Append(bet.Beat.BeatName);
                    }
                    sb.Append("</div>\n");
                }
                if (con.ContactWebAddresses.Count > 0)
                {
                    foreach (ContactWebAddress cwa in con.ContactWebAddresses)
                    {
                        sb.Append("<div style='" + labelStyle + "'>" + cwa.WebAddressType.WebAddressTypeName + "</div><div>" + MultiSelectorItem.GetLink(cwa.WebAddressType, cwa.WebAddress) + "</div>");
                    }
                }
                if (con.ContactPhoneNumbers.Count > 0)
                {
                    foreach (ContactPhoneNumber cpn in con.ContactPhoneNumbers)
                    {
                        sb.Append("<div style='" + labelStyle + "'>" + cpn.PhoneType.PhoneTypeName + "</div><div> " + MultiSelectorItem.GetPhoneNumberLink(cpn.PhoneNumber) + "</div>");
                    }
                }

                sb.Append("<div style='" + labelStyle + "'>Show Notes</div><div> ");
                sb.Append(con.ShowNotes);
                sb.Append("</div>");

                //location tab
                sb.Append("<h3 style='" + headingStyle + "'>Location</h3>\n");

                var contactAddress = con.ContactAddresses.FirstOrDefault();
                if (contactAddress != null)
                {
                    if (!String.IsNullOrEmpty(contactAddress.StreetAddress)) sb.Append("<div style='" + labelStyle + "'>Street Address</div><div>" + (contactAddress.StreetAddress == null ? "" : contactAddress.StreetAddress.Replace("\n", "<br/>")) + "</div>\n");

                    if (contactAddress.City != null) sb.Append("<div style='" + labelStyle + "'>City</div><div>" + contactAddress.City.CityName + "</div>\n");
                    if (contactAddress.ProvState != null) sb.Append("<div style='" + labelStyle + "'>Province/State</div><div>" + contactAddress.ProvState.ProvStateName + "</div>\n");
                    if (contactAddress.PostalZipCode != null) sb.Append("<div style='" + labelStyle + "'>Postal/Zip Code</div><div>" + contactAddress.PostalZipCode + "</div>\n");

                    if (contactAddress.Country != null) sb.Append("<div style='" + labelStyle + "'>Country</div><div>" + contactAddress.Country.CountryName + "</div>\n");
                }
                else
                {
                    sb.Append("<div style='" + labelStyle + "'>City</div><div>&mdash;</div>\n");
                    sb.Append("<div style='" + labelStyle + "'>Province/State</div><div>&mdash;</div>\n");
                    sb.Append("<div style='" + labelStyle + "'>Country</div><div>&mdash;</div>\n");
                }
                sb.Append("<div style='" + labelStyle + "'>Region</div><div> ");
                if (con.Regions.Count > 0)
                {

                    fst = false;
                    foreach (Region reg in con.Regions)
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
                if (con.ElectoralDistricts.Count > 0)
                {

                    fst = false;
                    foreach (ElectoralDistrict dst in con.ElectoralDistricts)
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

                if (con.Sectors.Count > 0)
                {
                    sb.Append("<div style='" + labelStyle + "'>Sector</div><div> ");
                    fst = false;
                    foreach (Sector sec in con.Sectors)
                    {
                        if (fst) sb.Append(", ");
                        else fst = true;
                        sb.Append(sec.DisplayName);
                    }
                    sb.Append("</div>\n");
                }

                //media tab
                sb.Append("<h3 style='" + headingStyle + "'>Media</h3>\n");
                if (con.ContactMediaJobTitles.Count > 0)
                {
                    sb.Append("<div style='" + labelStyle + "'>Media Job Title</div><div> ");
                    fst = false;
                    foreach (ContactMediaJobTitle mjt in con.ContactMediaJobTitles)
                    {
                        if (fst) sb.Append(", ");
                        else fst = true;
                        sb.Append(mjt.MediaJobTitle.MediaJobTitleName + ": " + mjt.Company.CompanyName);
                    }
                    sb.Append("</div>\n");
                }
                sb.Append("<div style='" + labelStyle + "'>Press Gallery</div><div> " + (con.IsPressGallery ? "Yes" : "No") + "</div>\n");

                //Ministry Tab
                sb.Append("<h3 style='" + headingStyle + "'>Ministry</h3>\n");

                sb.Append("<div style='" + labelStyle + "'>Has Minister Assignment</div><div> " + (con.HasMinisterAssignment ? "Yes: " + (con.Ministry == null ? "" : con.Ministry.MinisterName) : "No") + "</div>\n");
                sb.Append("<div style='" + labelStyle + "'>Primary Media Contact</div><div> " + (con.IsPrimaryMediaContact ? "Yes" : "No") + "</div>\n");
                sb.Append("<div style='" + labelStyle + "'>Secondary Media Contact</div><div> " + (con.IsSecondaryMediaContact ? "Yes" : "No") + "</div>\n");
                if (con.Ministry != null) sb.Append("<div style='" + labelStyle + "'>Ministry</div><div> " + con.Ministry.DisplayName + "</div>\n");
                if (con.MinisterialJobTitle != null) sb.Append("<div style='" + labelStyle + "'>Ministerial Job Title</div><div> " + con.MinisterialJobTitle.MinisterialJobTitleName + "</div>\n");
                if (con.MLAAssignmentId != null && con.MLAAssignmentId != Guid.Empty) sb.Append("<div style='" + labelStyle + "'>MLA Assignment</div><div> " + con.ElectoralDistrict.DistrictName + "</div>\n");

                sb.Append("</div>\n");
                CommonMethods.SendEmail(Settings.Default.FromEmailAddress, email, "Media Relations Contact Share", sb.ToString(), true);
                CommonEventLogging.WriteActivityLogEntry(con, "", CommonEventLogging.ActivityType.Share);

                bottomScriptArea.Text = "alert(sentShareEmailText.replace(\"###email###\", \"" + email.Replace("\"", "\\\"") + "\"));\n";
            }
        }
    }

    public void EditButton_Click(Object sender, EventArgs e)
    {
        var hidTab = Request.Form["hidTab"];
        if (string.IsNullOrWhiteSpace(hidTab)) hidTab = Request.QueryString["tab"];
        Response.Redirect("~/Contacts/Contact/EditContact.aspx?tab=" + hidTab + "&guid=" + Request.QueryString["guid"]);
    }

    public void CancelButton_Click(object sender, EventArgs e)
    {
        //todo - add logic here
        Response.Redirect("/Contacts/Contact/ViewContact.aspx?tab=" + currentTab + "guid=" + Request.QueryString["guid"]);
    }

    public void DeleteButton_Click(object sender, EventArgs e)
    {
        string error = ContactAdminLib.DeleteContact(guid, Request.Form["hidAltContact"]);

        if (error == null) Response.Redirect("~/Contacts/Default.aspx?message=" + Server.UrlEncode("Contact successfully deleted"));
        else ErrorLit.Text = error;
    }
}