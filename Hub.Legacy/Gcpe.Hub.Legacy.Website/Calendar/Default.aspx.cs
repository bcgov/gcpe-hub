using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CorporateCalendar.Security;
using CorporateCalendar.Data;

namespace Gcpe.Hub.Calendar
{
    public partial class Default : System.Web.UI.Page
    {
        protected string activeAccordionSection = "3";
        protected string userName = string.Empty;

        protected string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var customPrincipal = Master.CustomPrincipal;
            if (!IsPostBack)
            {
                if (!customPrincipal.IsInApplicationOwnerOrganizations
                    || !(customPrincipal.IsInRoleOrGreater(SecurityRole.Advanced)))
                {
                    accordionadmin.Style.Add("display", "none");
                }
            }

            // Check first to see if the CustomPrincipal is null. Session may have timed out.
            if (customPrincipal == null)
            {
                Response.Redirect("~/Calendar/CustomErrorPages/SessionTimeout.aspx");
                return;
            }

            UserName = customPrincipal.FirstName + " " + customPrincipal.LastName;

            bool isHq = customPrincipal.IsGCPEHQ && customPrincipal.IsInRoleOrGreater(SecurityRole.Editor);
            if (isHq)
            {
                chkBxStatus.Items.Add("LA New");
                chkBxStatus.Items.Add("LA Changed");
            }
            else
            {
                ClearLAStatus.Visible = false;
            }

            bool canSeeAllMinistries = customPrincipal.IsInApplicationOwnerOrganizations;
            using (var dc = new CorporateCalendarDataContext())
            {
                LoadMyQueries(dc, customPrincipal.Id);

                var ddm = new DropDownListManager(dc);
                // Government Representatives are loaded into the dropdown list.
                BindSelectFilter(frepresentative, "Select Representative", ddm.GetActiveGovernmentRepresentativeOptions());

                object dataSource;
                // Contact Ministries are loaded into the dropdown list.
                if (fministry != null)
                {
                    if (canSeeAllMinistries)
                    {
                        dataSource = ddm.GetAllActiveMinistryOptions();
                    }
                    else
                    {
                        dataSource = ddm.GetActiveMinistryOptions(customPrincipal);
                    }
                    BindSelectFilter(fministry, "Select Ministry", dataSource, "Abbreviation", "Id");
                }

                // TODO: it is unclear what the following choices are and how they are arrived at. add comments to distinguish differences below -DW
                string ministry;
                if (fministry != null && (fministry.Items.Count > 1 && fministry.Items[0].Value == "*"))
                {
                    ministry = fministry.Items[fministry.SelectedIndex].Value;  // changed from index 1 to "fministry.SelectedIndex" - DW Nov 5 2012
                }
                else if (fministry != null && (fministry.Items.Count == 1 || (fministry.Items.Count > 1 && fministry.Items[0].Value != "*")))
                {
                    ministry = fministry.Items[0].Value;
                }
                else
                {
                    ministry = "*";
                }

                // Communication Contacts are loaded into the dropdown list.
                if (fcontact != null)
                {
                    // Boolean check for "*" in ministry
                    Guid minID;
                    bool hasMinistry = Guid.TryParse(ministry, out minID);

                    if (!customPrincipal.IsInApplicationOwnerOrganizations && hasMinistry) // get comm. contact for specific ministry -DW
                    {
                        dataSource = ddm.GetCommunicationContactByMinistryId(minID, DropDownListManager.CommunicationSortOrder.FirstName);
                    }
                    else
                    {
                        dataSource = ddm.GetCommunicationContactsByCurrentUser(DropDownListManager.CommunicationSortOrder.FirstName, customPrincipal);
                    }
                    BindSelectFilter(fcontact, "Select Comm. Contact", dataSource, "Name", "SystemUserId");
                }


                // Statuses are loaded into the dropdown list.
                BindSelectFilter(fstatus, "Select Status", ddm.GetStatusOptions());

                // Premier Requested are loaded into the dropdown list.
                BindSelectFilter(fpremier, "Select Premier Requested", ddm.GetActivePremierRequestedOptions());

                // Distributions are loaded into the dropdown list.
                BindSelectFilter(fdistribution, "Select Distribution", ddm.GetActiveNrDistributionOptions());

                // Categories are loaded into the dropdown list.
                BindSelectFilter(fcategory, "Select Category", ddm.GetActiveCategoryOptions(isHq));

                dataSource = ddm.GetActiveKeywordOptionsSecurely(customPrincipal.Id, customPrincipal.IsInApplicationOwnerOrganizations);
                Activity.BindHtmlSelect(KeywordList, dataSource, "Name", "Id");

                // Initiatives are loaded into the dropdown list.
                BindSelectFilter(finitiative, "Select Initiative", ddm.GetActiveInitiativeOptions());
            }

            // show all for "Application Owners" exclude shared for regular users
            if (!Page.IsPostBack)
            {
                if (customPrincipal.SystemUserMinistryIds.Count == 1 && !canSeeAllMinistries)
                    DisplayRadioButtonList.Items[1].Text = customPrincipal.SingleMinistryName + " Activities Only";

                int? display = customPrincipal.FilterDisplayValue;
                DisplayRadioButtonList.SelectedValue = display.HasValue ? display.ToString() : "3";
            }

            ActivityGrid.HiddenColumns = customPrincipal.HiddenColumns?.Split(',') ?? ColumnModel.HiddenByDefault;
        }

        void BindSelectFilter(System.Web.UI.HtmlControls.HtmlSelect fSelect, string title, object dataSource)
        {
            BindSelectFilter(fSelect, title, dataSource, "Name", "Id");
        }

        void BindSelectFilter(System.Web.UI.HtmlControls.HtmlSelect fSelect, string title, object dataSource, string dataTextField, string dataValueField)
        {
            Activity.BindHtmlSelect(fSelect, dataSource, dataTextField, dataValueField);
            fSelect.Items.Insert(0, new ListItem(title, "*", true));
        }

        void LoadMyQueries(CorporateCalendarDataContext dc, int customPrincipalId)
        {
            dc.Refresh(RefreshMode.OverwriteCurrentValues, dc.ActivityFilters);
            var myQueryItems = dc.ActivityFilters.Where(p => (p.CreatedBy == customPrincipalId && p.IsActive == true)).Select(p => p).OrderBy(p => p.SortOrder);

            MyQueriesRepeater.DataSource = myQueryItems;
            MyQueriesRepeater.DataBind();
        }
    }
}