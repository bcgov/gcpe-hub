extern alias legacy;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web.UI.WebControls;
using CorporateCalendar;
using CorporateCalendar.Data;
using CorporateCalendar.Security;
using legacy::Gcpe.Hub.Data.Entity;
using Ministry = CorporateCalendar.Data.Ministry;
using ci = System.Globalization.CultureInfo;
using Gcpe.Hub.News.ReleaseManagement;
using Gcpe.Hub.Properties;
using System.Web.UI;

namespace Gcpe.Hub.Calendar
{
    public partial class Activity : System.Web.UI.Page
    {
        public ActivityModel Model { get; } = new ActivityModel();
        private CorporateCalendarDataContext calendarDataContext = new CorporateCalendarDataContext();

        public override void Dispose()
        {
            calendarDataContext.Dispose();
            base.Dispose();
        }

        #region Properties

        private bool IsUserSessionExpired
        {
            get { return (Session.SessionID == null); }
        }

        public bool IsUserReadOnly
        {
            get { return Master.CustomPrincipal.IsInRole(SecurityRole.ReadOnly); }
        }

        public bool IsAdvancedUser
        {
            get { return Master.CustomPrincipal.IsInRoleOrGreater(SecurityRole.Administrator); }
        }

        bool? isHq = null;
        private bool IsHq
        {
            get
            {
                if (!isHq.HasValue)
                    isHq = Master.CustomPrincipal.IsGCPEHQ && Master.CustomPrincipal.IsInRoleOrGreater(SecurityRole.Editor);
                return isHq.Value;
            }
        }

        public int? ActivityId
        {
            get
            {
                int activityId;
                var id = Request.QueryString["ActivityId"];
                if (!string.IsNullOrEmpty(id) && int.TryParse(id, out activityId))
                    return activityId;
                else
                    return null;
            }
        }

        private bool IsNewActivity
        {
            get { return (ActivityId == null || ActivityId < 1); }
        }

        private ActiveActivity _currentActiveActivity = null;
        public ActiveActivity CurrentActiveActivity
        {
            get
            {
                if (null != _currentActiveActivity)
                {
                    return _currentActiveActivity;
                }
                else
                {
                    if (IsNewActivity)
                    {
                        _currentActiveActivity = new ActiveActivity();
                        return _currentActiveActivity;
                    }
                    else
                    {
                        var activity = ActivityWebService.GetActiveActivityById((int)ActivityId);
                        if (activity != null)
                        {
                            _currentActiveActivity = activity; //.First();
                            return _currentActiveActivity;
                        }
                        else
                        {
                            _currentActiveActivity = new ActiveActivity();
                            return _currentActiveActivity;
                        }
                    }
                }

            }
            set { _currentActiveActivity = value; }
        }

        private bool? IsActiveActivity
        {
            get
            {
                if (CurrentActiveActivity.Id > 0)
                    return CurrentActiveActivity.IsActive;
                else
                    return false;
            }
        }

        private Guid[] SharedWithMinistryIds
        {
            get
            {
                IQueryable<Guid> activitiesSharedWith = calendarDataContext.ActivitySharedWiths
                                    .Where(p => p.ActivityId == ActivityId)
                                    .Select(p => p.MinistryId);

                return activitiesSharedWith.ToArray();
            }
        }

        #endregion

        private void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NRTime.Items.Add("");
                for (DateTime time = DateTime.MinValue; time < DateTime.MinValue.AddDays(1d); time = time.AddMinutes(5))
                {
                    StartTime.Items.Add(time.ToString("h:mm tt", ci.InvariantCulture));
                    NRTime.Items.Add(time.ToString("h:mm tt", ci.InvariantCulture));
                    EndTime.Items.Add(time.ToString("h:mm tt", ci.InvariantCulture));
                }

                StartTime.SelectedValue = "8:00 AM";
                NRTime.SelectedValue = "";
                EndTime.SelectedValue = "6:00 PM";

                LACommentsTextBox.Text = "**";
            }

            ErrorNotice.Style.Clear();
            ErrorNotice.Style.Add("display", "none");

            StrategyIsRequiredFieldValidator.Visible = Settings.Default.StrategyIsRequired;

            SignificanceRequiredFieldValidator.Visible = Settings.Default.SignificanceIsRequired;

            SchedulingFieldValidator.Visible = Settings.Default.SchedulingIsRequired;

            // Why is this here?
            CategoriesDropDownListRequiredFieldValidator.Attributes.Add("style", "display:none;color:red");
            StartTimeValidator.Attributes.Add("style", "display:none;color:red");
            EndTimeRequiredValidator.Attributes.Add("style", "display:none;color:red");

            // Empty the IsPostBackHiddenField
            IsPostBackHiddenField.Text = string.Empty;

            // Check Session status
            if (IsUserSessionExpired)
            {
                Response.Redirect("~/Calendar/CustomErrorPages/SessionTimeout.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            // Handle Security
            if (!IsPostBack)
            {
                CheckSecurity();

                // Verify and Display Activity Status
                if (IsNewActivity)
                {
                    SetNewActivityDefaults();

                    LoadMinistryScript();
                    FavoriteButton.Visible = false;
                    /*ReviewButton.Visible = false;
                    DeleteButton.Visible = false;
                    CloneButton.Visible = false;
                    StatusInfoLabel.Text = "New";*/
                    LAStatusRadioButtonList.SelectedValue = "";
                }

                // Show or hide the action buttons depending on various conditions
                ShowHideButtons();

                // Load Current Activity
                if (!IsNewActivity && CurrentActiveActivity != null)
                {
                    LoadExistingActivity();
                    if (Request.QueryString["new"] != null)
                    {
                        SavedSuccessfullyNotice.Style.Clear();
                        InactivityNotice.Style.Add("display", "none");
                    }
                }
                else
                {
                    documentsRepeater.DataSource = new ActivityFile[0];
                    documentsRepeater.DataBind();
                }
            }
            else
            {
                //Reload Ministry Script
                List<Guid> list = (from d in ContactMinistryDropDownList.Items.Cast<ListItem>() select d.Value)
                    .Where(d => d != null && d != "").Select(d => Guid.Parse(d)).ToList();
                LoadMinistryScript(list);
            }

            if (IsHq)
            {
                //HQ users see all fields
                DetailsRequiredFieldValidator.Visible = false;
                SignificanceRequiredFieldValidator.Visible = false;
                SchedulingFieldValidator.Visible = false;
            }
            else
            {
                LACommentsCustomValidator.Visible = false;
                if (!Settings.Default.ShowHqCommentsField)
                {
                    LACommentsRow.Visible = false;
                    LAStatusRow.Visible = false;
                    laFieldset.Style.Add("display", "none");
                }

                if (string.IsNullOrEmpty(CurrentActiveActivity.Significance) && !Settings.Default.ShowSignificanceField)
                {
                    SignificanceRow.Style.Add("display", "none");
                    // if we are not showing this field, do not validate - cannot enter data, cannot pass validation.
                    SignificanceRequiredFieldValidator.Visible = false;
                    SignificanceRequiredFieldValidator.Enabled = false;
                }

                if (string.IsNullOrEmpty(CurrentActiveActivity.Schedule) &&
                    !Settings.Default.ShowScheduleField)
                {
                    ScheduleRow.Style.Add("display", "none");
                    // if we are not showing this field, do not validate - cannot enter data, cannot pass validation.
                    SchedulingFieldValidator.Visible = false;
                    SchedulingFieldValidator.Enabled = false;
                }


            }

            if (!Settings.Default.ShowRecordsSection)
                RecordsSection.Style.Add("display", "none");
        }

        private void LoadMinistryScript(List<Guid> selectedMinistryIds = null)
        {
            var ddm = new DropDownListManager(calendarDataContext);
            IQueryable<Ministry> ministries = (IQueryable<Ministry>)ddm.GetAllActiveMinistryOptions(selectedMinistryIds);
            System.Text.StringBuilder stbScript = new System.Text.StringBuilder();
            stbScript.Append("var ministries = {");
            foreach (Ministry ministry in ministries)
            {
                stbScript.Append(String.Format("\"{0}\":\"{1}\",", ministry.Id, ministry.DisplayName));
            }

            if (stbScript.Length > 1)
            {
                //to remove final comma
                stbScript.Length--;
            }
            stbScript.Append("};");
            ClientScript.RegisterStartupScript(this.GetType(), "Ministries", stbScript.ToString(), true);
        }

        private void CheckSecurity()
        {
            if (IsNewActivity)
            {
                // This is a read-only user trying to go to the Activity.aspx page without a query string. Redirect them to the not authorized page
                // User is not Authorized to view this activity
                if (IsUserReadOnly) //
                {
                    Response.Redirect("~/CustomErrorPages/NotAuthorized.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }
            else if (IsActiveActivity.HasValue && IsActiveActivity.Value == false)
            {
                Response.Redirect("~/Calendar/CustomErrorPages/ActivityDoesNotExist.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            else
            {

                if (!Master.CustomPrincipal.IsInApplicationOwnerOrganizations)
                {
                    if (!IsCurrentUserInActivityMinistryOrSharedWith())
                    {
                        // this activity is not one that has a ministry id in the user's list of ministries
                        // User is not Authorized to view this activity
                        Response.Redirect("~/Calendar/CustomErrorPages/NotAuthorized.aspx", false);
                        //throw new CorporateCalendar.Exception.NotAuthorizedException(
                        //    "You are not authorized to view this ministry's activities.");
                        return;
                    }
                }
            }
        }

        private void SetNewActivityDefaults()
        {
            PopulateDropDownLists(null, null, null, null, null, null, null, null, null, null);

            // Set the default dates
            //EndDate.Value = StartDate.Value = DateTime.Now.ToString("MM/dd/yyyy", ci.InvariantCulture);

            FavoriteButton.Visible = false;
            ReviewButton.Visible = false;

            var userMinistryIds = Master.CustomPrincipal.SystemUserMinistryIds;
            if (userMinistryIds.Count() == 1)
            {
                ContactMinistryDropDownList.Value = userMinistryIds.FirstOrDefault().ToString();
            }

            PopulateCommContactDropDownList(userMinistryIds.FirstOrDefault());

            string defaultContact = "";
            int index = -1;

            /*IQueryable<Ministry> ministry =
                calendarDataContext.Ministries.Where(m => m.Id == userMinistryIds.FirstOrDefault());

             Does this ministry have a default contact?
            if (ministry.Any() && (ministry.First().ContactUserId != null))
            {
                defaultContact = ministry.First().ContactUserId.ToString();
                index = CommContactDropDownList.Items.IndexOf(CommContactDropDownList.Items.FindByValue(defaultContact));
            }*/

            // No ministry default contact, so use logged in user id
            if (index < 0 || defaultContact == "")
            {
                defaultContact = Master.CustomPrincipal.Id.ToString();
                index = CommContactDropDownList.Items.IndexOf(CommContactDropDownList.Items.FindByValue(defaultContact));
            }

            if (index >= 0)
            {
                CommContactDropDownList.SelectedIndex = index;
                CommContactDropDownList.Value = defaultContact;
                ComContactSelectedValue.Value = defaultContact;

                if (CommContactDropDownList.SelectedIndex > 0)
                    CommunicationContactRequiredFieldValidator.IsValid = true;
            }
        }

        // We might want to put these in the Language table if the need for a "Add Hindi" button (for example) arises in NRMS ... and news
        static string[] defaultTranslations = { "Arabic", "Chinese (Simplified)", "Chinese (Traditional)", "Dutch", "Farsi", "Finnish", "French", "Gujarati", "Hebrew", "Hindi", "Indonesian", "Japanese", "Korean", "Portuguese", "Punjabi", "Russian", "Somali", "Spanish", "Swahili", "Tagalog", "Ukrainian", "Urdu", "Vietnamese" };
        private void PopulateDropDownLists(ActiveActivity activity, List<int> categories, List<Guid> sectors, List<Guid> themes, List<int> keywords, List<string> translations, List<int> initiatives, List<int> commMaterials, List<int> nrOrigins, List<Guid> tags)
        {
            var ddm = new DropDownListManager(calendarDataContext);
            object dataSource;
            // Government Representatives
            if (RepresentativeDropDownList != null)
            {
                dataSource = ddm.GetActiveGovernmentRepresentativeOptions(activity?.GovernmentRepresentativeId);
                BindDropDownList(RepresentativeDropDownList, "Name", dataSource);
                RepresentativeDropDownList.Items.Insert(0, new ListItem(" ", " ", true));
            }

            // This is a reusable item to specify List items in the multiselects
            // I'm using them to remove items with certain text strings
            ListItem itemToRemove = ContactMinistryDropDownList.Items.FindByText("GCPEHQ");

            // Contact Ministries
            if (ContactMinistryDropDownList != null)
            {
                var selectedMinistryIds = new List<Guid>();

                if (activity != null && activity.ContactMinistryId.HasValue)
                    selectedMinistryIds.Add(activity.ContactMinistryId.Value);

                if (!Master.CustomPrincipal.IsInApplicationOwnerOrganizations)
                {
                    dataSource = ddm.GetActiveMinistryOptions(Master.CustomPrincipal, selectedMinistryIds).ToArray();
                }
                else
                {
                    dataSource = ddm.GetAllActiveMinistryOptions(selectedMinistryIds);
                }
                BindDropDownList(ContactMinistryDropDownList, "Abbreviation", dataSource);

                itemToRemove = ContactMinistryDropDownList.Items.FindByText("UNK");
                if (itemToRemove != null) { ContactMinistryDropDownList.Items.Remove(itemToRemove); }
            }

            PopulateCommContactDropDownList(CurrentActiveActivity.ContactMinistryId);

            // Cities
            if (CityDropDownList != null)
            {
                dataSource = ddm.GetActiveCityOptions(activity?.CityId);

                BindDropDownList(CityDropDownList, "Name", dataSource);
                CityDropDownList.Items.Insert(0, new ListItem(" ", " ", true));
            }

            // Shares (other ministries)
            if (SharedWithDropDownList != null)
            {
                bool hasSharedMinistries = activity != null && !string.IsNullOrEmpty(activity.SharedWithMinistryIds);
                List<Guid> sharedWithIds = hasSharedMinistries ? activity.SharedWithMinistryIds.Split(',').Select(e => Guid.Parse(e)).ToList() : null;
                dataSource = ddm.GetAllActiveMinistryOptions(sharedWithIds);
                BindDropDownList(SharedWithDropDownList, "Abbreviation", dataSource);
            }

            string[] sharedWithMinistryExcludes = Settings.Default.SharedWithExcludes.Split(',');
            // Remove application owners from the list
            foreach (string sharedWithMinistryExclude in sharedWithMinistryExcludes)
            {
                ListItem item = SharedWithDropDownList.Items.FindByText(sharedWithMinistryExclude);
                if (item != null)
                    SharedWithDropDownList.Items.Remove(item);
            }

            // Categories
            if (CategoriesDropDownList != null)
            {
                dataSource = ddm.GetActiveCategoryOptions(IsHq, categories);
                BindDropDownList(CategoriesDropDownList, "Name", dataSource);
            }

            // Communication Materials
            if (CommMaterialsDropDownList != null)
            {
                dataSource = ddm.GetActiveCommunicationMaterialsOptions(commMaterials);
                BindDropDownList(CommMaterialsDropDownList, "Name", dataSource);
            }

            // NR Origins
            if (NROriginDropDownList != null)
            {
                dataSource = ddm.GetActiveNrOriginOptions(nrOrigins);
                BindDropDownList(NROriginDropDownList, "Name", dataSource);
                NROriginDropDownList.Items.Insert(0, new ListItem(" ", " ", true));
            }

            // NR Distributions
            if (NRDistributionDropDownList != null)
            {
                if (activity != null)
                    dataSource = ddm.GetActiveNrDistributionOptions(activity.NRDistributionId);
                else
                    dataSource = ddm.GetActiveNrDistributionOptions();
                BindDropDownList(NRDistributionDropDownList, "Name", dataSource);
                NRDistributionDropDownList.Items.Insert(0, new ListItem(" ", " ", true));
            }

            // Sectors
            if (SectorDropDownList != null)
            {
                dataSource = ddm.GetActiveSectorOptions(sectors);
                BindDropDownList(SectorDropDownList, "DisplayName", dataSource);
            }
            // Remove not Defined
            itemToRemove = SectorDropDownList.Items.FindByText("Not Defined");
            if (itemToRemove != null) { SectorDropDownList.Items.Remove(itemToRemove); }

            // Themes
            if (ThemeDropDownList != null)
            {
                dataSource = ddm.GetActiveThemeOptions(themes);
                BindDropDownList(ThemeDropDownList, "DisplayName", dataSource);
            }

            // Tags
            if (TagsDropDownList != null)
            {
                dataSource = ddm.GetActiveTagOptions(tags);
                BindDropDownList(TagsDropDownList, "DisplayName", dataSource);
            }

            // Keywords
            if (KeywordList != null)
            {
                dataSource = ddm.GetActiveKeywordOptionsSecurely(Master.CustomPrincipal.Id, Master.CustomPrincipal.IsInApplicationOwnerOrganizations, keywords);
                BindDropDownList(KeywordList, "Name", dataSource);
            }

            // Translations Required
            if (TranslationsRequired != null)
            {
                TranslationsRequired.Items.Clear();
                foreach (string t in defaultTranslations)
                {
                    TranslationsRequired.Items.Add(t);
                }
                for (int i = 0; i < (translations != null ? translations.Count : 0); i++)
                {
                    string translation = translations[i].TrimStart();
                    if (defaultTranslations.Contains(translation)) continue;
                    TranslationsRequired.Items.Add(translation);
                }
            }

            if (LAStatusRadioButtonList != null)
            {
                LAStatusRadioButtonList.DataSource = ddm.GetHQStatusOptions();
                LAStatusRadioButtonList.DataTextField = "Name";
                LAStatusRadioButtonList.DataValueField = "Id";
                LAStatusRadioButtonList.DataBind();

                LAStatusRadioButtonList.Items.Insert(0, new ListItem("None", ""));
            }

            if (LASectionCheckBoxList.Items.Count == 0)
            {
                var selectedLASection = (LookAheadSection)Math.Abs(CurrentActiveActivity.HqSection);
                var lookAheadSections = Enum.GetValues(typeof(LookAheadSection));
                foreach (LookAheadSection section in lookAheadSections)
                {
                    var li = new ListItem { Text = "", Value = ((int)section).ToString(), Selected = (section == selectedLASection) };
                    if (section != LookAheadSection.Not_On_LA)
                    {
                        li.Text = section.ToString().Replace("_and_", " & ").Replace(" Reports", "&nbsp;Reports").Replace('_', ' ');
                    }
                    LASectionCheckBoxList.Items.Add(li);
                }
                LASectionCheckBoxList.Items.Add(new ListItem { Text = "Long Term Outlook", Selected = CurrentActiveActivity.IsForLongTermOutlook });
            }

            // Initiatives
            if (InitiativeDropDownList != null)
            {
                dataSource = ddm.GetActiveInitiativeOptions(initiatives);
                BindDropDownList(InitiativeDropDownList, "Name", dataSource);
            }

            // Remove not Defined
            itemToRemove = InitiativeDropDownList.Items.FindByText("Not Defined");
            if (itemToRemove != null) { InitiativeDropDownList.Items.Remove(itemToRemove); }

            // Premier Requested
            if (PremierRequestedDropDownList != null)
            {
                dataSource = ddm.GetActivePremierRequestedOptions(activity?.PremierRequestedId);

                BindDropDownList(PremierRequestedDropDownList, "Name", dataSource);
                PremierRequestedDropDownList.Items.Insert(0, new ListItem(" ", " ", true));
            }

            // Event Planner
            if (EventPlannerDropDownList != null)
            {
                dataSource = ddm.GetActiveEventPlannerOptions(activity?.EventPlannerId);
                BindDropDownList(EventPlannerDropDownList, "Name", dataSource);
                EventPlannerDropDownList.Items.Insert(0, new ListItem(" ", " ", true));
            }

            // Videographer
            if (VideographerDropDownList != null)
            {
                dataSource = ddm.GetActiveVideographerOptions(activity?.VideographerId);
                BindDropDownList(VideographerDropDownList, "Name", dataSource);
                VideographerDropDownList.Items.Insert(0, new ListItem(" ", " ", true));
            }
        }

        void BindDropDownList(System.Web.UI.HtmlControls.HtmlSelect fSelect, string dataTextField, object dataSource)
        {
            BindHtmlSelect(fSelect, dataSource, dataTextField, "Id");
        }

        static public void BindHtmlSelect(System.Web.UI.HtmlControls.HtmlSelect fSelect, object dataSource, string dataTextField, string dataValueField)
        {
            if (fSelect != null)
            {
                fSelect.DataSource = dataSource;
                fSelect.DataTextField = dataTextField;
                fSelect.DataValueField = dataValueField;
                fSelect.DataBind();
            }
        }

        private void PopulateCommContactDropDownList(Guid? ministryId)
        {
            // Communication Contacts
            // NOTE: The DataTextField is the Comm. Contact's name concatenated with their phone number.
            // This information is pulled from a stored procedure form the database view.
            if (CommContactDropDownList != null)
            {
                var ddm = new DropDownListManager(calendarDataContext);
                object dataSource;
                // if the activity is assigned to a ministry, make sure that the selected contact is also part of that ministry and populate the
                // com contact drop down with
                if (CurrentActiveActivity != null && ministryId.HasValue)
                {
                    dataSource = ddm.GetCommunicationContactByMinistryIdIncludingId(ministryId.Value, CurrentActiveActivity.CommunicationContactId, DropDownListManager.CommunicationSortOrder.RoleThenName);
                }
                else
                {
                    dataSource = ddm.GetCommunicationContactsByCurrentUser(DropDownListManager.CommunicationSortOrder.RoleThenName, Master.CustomPrincipal);
                }

                BindHtmlSelect(CommContactDropDownList, dataSource, "NameAndNumber", "SystemUserId");
            }
        }

        private void LoadExistingActivity()
        {
            //Update the activity with new values from db.
            //especially important after save of existing activity when it reloads values
            this.CurrentActiveActivity = ActivityWebService.GetActiveActivityById((int)ActivityId);
            string categoriesSelectedValues = calendarDataContext.GetActivityCategories(ActivityId).SingleOrDefault()?.categories;
            string commMaterialsSelectedValues = calendarDataContext.GetActivityCommunicationMaterials(ActivityId).SingleOrDefault()?.activityCommunicationMaterials;
            string nrOriginsSelectedValues = calendarDataContext.GetActivityNewsReleaseOrigins(ActivityId).SingleOrDefault()?.activityNewsReleaseOrigins;
            string sectorsSelectedValues = calendarDataContext.GetActivitySectors(ActivityId).SingleOrDefault()?.activitySectors;
            string themesSelectedValues = calendarDataContext.GetActivityThemes(ActivityId).SingleOrDefault()?.activityThemes;
            string tagsSelectedValues = calendarDataContext.GetActivityTags(ActivityId).SingleOrDefault()?.activityTags;
            string initiativeSelectedValues = calendarDataContext.sGetActivityInitiatives(ActivityId);
            var keywordsSelectedValues = calendarDataContext.GetActivityKeywords(ActivityId);


            List<Guid> selectedSectorIds = null;
            if (!string.IsNullOrEmpty(sectorsSelectedValues))
                selectedSectorIds = sectorsSelectedValues.Split(',').Select(e => Guid.Parse(e)).ToList();

            List<Guid> selectedThemeIds = null;
            if (!string.IsNullOrEmpty(themesSelectedValues))
                selectedThemeIds = themesSelectedValues.Split(',').Select(e => Guid.Parse(e)).ToList();

            List<Guid> selectedTagIds = null;
            if (!string.IsNullOrEmpty(tagsSelectedValues))
                selectedTagIds = tagsSelectedValues.Split(',').Select(e => Guid.Parse(e)).ToList();

            List<int> selectedKeywords = keywordsSelectedValues.Select(k => k.keywordId.Value).ToList();

            List<int> selectedInitiatives = null;
            if (!string.IsNullOrEmpty(initiativeSelectedValues))
                selectedInitiatives = initiativeSelectedValues.Split(',').Select(e => int.Parse(e)).ToList();

            List<string> selectedTranslations = null;
            if (!string.IsNullOrEmpty(CurrentActiveActivity.Translations))
                selectedTranslations = CurrentActiveActivity.Translations.Split(',').ToList();

            List<int> selectedCategoryIds = null;
            if (!string.IsNullOrEmpty(categoriesSelectedValues))
                selectedCategoryIds = categoriesSelectedValues.Split(',').Select(e => int.Parse(e)).ToList();

            List<int> selectedCommMaterialIds = null;
            if (commMaterialsSelectedValues != null && !string.IsNullOrEmpty(commMaterialsSelectedValues))
                selectedCommMaterialIds = commMaterialsSelectedValues.Split(',').Select(e => int.Parse(e)).ToList();

            List<int> selectedNROriginIds = null;
            if (!string.IsNullOrWhiteSpace(nrOriginsSelectedValues))
                selectedNROriginIds = nrOriginsSelectedValues.Split(',').Select(e => int.Parse(e)).ToList();

            PopulateDropDownLists(CurrentActiveActivity, selectedCategoryIds, selectedSectorIds, selectedThemeIds, selectedKeywords, selectedTranslations, selectedInitiatives, selectedCommMaterialIds, selectedNROriginIds, selectedTagIds);
            timestamp.Value = (CurrentActiveActivity.LastUpdatedDateTime ?? CurrentActiveActivity.CreatedDateTime ?? DateTime.MinValue).ToOADate().ToString();

            var selectedMinistryIds = new List<Guid>();
            if (CurrentActiveActivity.ContactMinistryId.HasValue)
                selectedMinistryIds.Add(CurrentActiveActivity.ContactMinistryId.Value);
            LoadMinistryScript(selectedMinistryIds);

            ApplicationOwnerOrganizations.Text = Settings.Default.ApplicationOwnerOrganizations;

            //Add "Activity ID" label (made of ministry short name and ActivityID)
            ActiveID.Text = string.Format("{0}-{1}", CurrentActiveActivity.Ministry, ActivityId);
            ActiveID.Visible = true;
            ActiveIDLabel.Visible = true;

            //LastUpdated.Text = string.Format("{0:MMM d, yyyy}&nbsp;(<a target='_blank' href='History.aspx?ActivityID={1}'>View Changes</a>)", CurrentActiveActivity.LastUpdatedDateTime, ActivityId.ToString());
            //LastUpdated.Visible = true;
            //LastUpdatedLabel.Visible = true;

            #region Populate TextBoxes

            var activityIdTextBox = (System.Web.UI.HtmlControls.HtmlInputText)Master.FindControl("ActivityIdTextBox");
            if (activityIdTextBox != null)
                activityIdTextBox.Value = ActivityId.ToString();

            TitleTextBox.Text = CurrentActiveActivity.Title;

            //Details(Summary), LAComments, Internal Notes(Comments)
            DetailsTextBox.Text = CurrentActiveActivity.Details;
            if (LACommentsRow.Visible)
            {
                LACommentsTextBox.Text = CurrentActiveActivity.HqComments;
                LAStatusRadioButtonList.SelectedValue = CurrentActiveActivity.HqStatusId?.ToString() ?? "";
            }
            CommentsTextBox.Text = CurrentActiveActivity.Comments?.Trim();

            SignificanceTextBox.Text = CurrentActiveActivity.Significance;
            SchedulingTextBox.Text = CurrentActiveActivity.Schedule;
            TranslationsTextbox.Text = CurrentActiveActivity.Translations?.Replace(", ", "~");
            KeywordsTextBox.Text = string.Join("~", keywordsSelectedValues.Select(k => k.keywordName));

            StrategyTextBox.Text = CurrentActiveActivity.Strategy;

            LeadOrganizationTextBox.Text = CurrentActiveActivity.LeadOrganization;
            VenueTextBox.Text = CurrentActiveActivity.Venue;

            // Start date and time
            StartDate.Value = CurrentActiveActivity.StartDateTime?.ToString("MM/dd/yyyy", ci.InvariantCulture) ?? "";
            StartTime.SelectedValue = CurrentActiveActivity.StartDateTime == null ? "" : CurrentActiveActivity.StartDateTime.Value.ToString("h:mm tt", ci.InvariantCulture);

            // End date and time
            EndDate.Value = CurrentActiveActivity.EndDateTime?.ToString("MM/dd/yyyy", ci.InvariantCulture) ?? "";
            EndTime.SelectedValue = CurrentActiveActivity.EndDateTime?.ToString("h:mm tt", ci.InvariantCulture) ?? "";

            // Release (NR) date and time
            NRDate.Value = CurrentActiveActivity.NRDateTime?.ToString("MM/dd/yyyy", ci.InvariantCulture) ?? "";
            NRTime.SelectedValue = CurrentActiveActivity.NRDateTime?.ToString("h:mm tt", ci.InvariantCulture) ?? "";

            #endregion

            #region Check CheckBoxes

            IsAllDayCheckBox.Checked = CurrentActiveActivity.IsAllDay;

            IsConfidentialCheckBox.Checked = CurrentActiveActivity.IsConfidential;

            // Categories used to be a multi select field, and Issue was a category
            var categories = (CurrentActiveActivity.Categories ?? "").Split(',').ToList();
            int posIssue = categories.IndexOf(" Issue");
            if (posIssue != -1) categories.RemoveAt(posIssue);
            IsIssueCheckBox.Checked = CurrentActiveActivity.IsIssue || posIssue != -1;
            IsCrossGovernmentCheckBox.Checked = CurrentActiveActivity.IsCrossGovernment;
            IsAtLegislatureCheckBox.Checked = CurrentActiveActivity.IsAtLegislature;

            IsMilestoneCheckBox.Checked = CurrentActiveActivity.IsMilestone;

            #endregion

            #region Set DropDown / radiobutton value(s)

            if (CurrentActiveActivity.IsConfirmed)
                IsDateConfirmed.Checked = true;

            PotentialDatesTextBox.Text = CurrentActiveActivity.PotentialDates;

            // Single-select DropDown
            if (CurrentActiveActivity.GovernmentRepresentativeId.HasValue)
                RepresentativeDropDownList.Value = CurrentActiveActivity.GovernmentRepresentativeId.ToString();

            if (CurrentActiveActivity.ContactMinistryId.HasValue)
                ContactMinistryDropDownList.Value = CurrentActiveActivity.ContactMinistryId.Value.ToString();


            if (CurrentActiveActivity.CommunicationContactId.HasValue &&
                CurrentActiveActivity.CommunicationContactId.Value > 0)
            {
                // The Communication Contact drop down value is the SystemUserId, so need to find that for the
                // communication contact.
                IQueryable<CommunicationContact> comContact =
                    calendarDataContext.CommunicationContacts.Where(
                                 r => r.Id == CurrentActiveActivity.CommunicationContactId.Value);

                if (comContact.Any())
                {
                    int comContactSystemUserId = comContact.First().SystemUserId;
                    CommContactDropDownList.SelectedIndex = CommContactDropDownList.Items.IndexOf(CommContactDropDownList.Items.FindByValue(comContactSystemUserId.ToString()));
                    ComContactSelectedValue.Value = comContactSystemUserId.ToString();
                }

                if (CommContactDropDownList.SelectedIndex > 0)
                    CommunicationContactRequiredFieldValidator.IsValid = true;
            }

            if (CurrentActiveActivity.CityId.HasValue && CurrentActiveActivity.CityId.Value > 0)
            {
                CityDropDownList.SelectedIndex = CityDropDownList.Items.IndexOf(CityDropDownList.Items.FindByValue(CurrentActiveActivity.CityId.Value.ToString()));
            }

            if (CityDropDownList.Value == "311")
            {
                OtherCityTextBox.Text = CurrentActiveActivity.OtherCity;
            }


            if (categories.Count != 0)
                CategoriesDropDownList.Value = CategoriesDropDownList.Items.FindByText(categories.First().TrimStart())?.Value;

            if (CurrentActiveActivity.NROrigins != null)
                NROriginDropDownList.Value = NROriginDropDownList.Items.FindByText(CurrentActiveActivity.NROrigins.TrimStart())?.Value;

            if (CurrentActiveActivity.PremierRequestedId.HasValue)
                PremierRequestedDropDownList.Value = CurrentActiveActivity.PremierRequestedId.ToString();

            if (CurrentActiveActivity.EventPlannerId.HasValue)
                EventPlannerDropDownList.Value = CurrentActiveActivity.EventPlannerId.ToString();

            if (CurrentActiveActivity.VideographerId.HasValue)
                VideographerDropDownList.Value = CurrentActiveActivity.VideographerId.ToString();

            if (CurrentActiveActivity.NRDistributionId.HasValue)
                NRDistributionDropDownList.Value = CurrentActiveActivity.NRDistributionId.ToString();

            // Multi-select DropDownList are populated on the client-side and use the comma-delimited strings stored
            // in the hidden fields placed on the page for the server-side values. See the

            #endregion

            #region Populate HiddenFields

            var sharedWithSelectedValues = calendarDataContext.GetActivitySharedWithMinistries(ActivityId).SingleOrDefault();
            if (sharedWithSelectedValues != null)
                SharedWithSelectedValuesServerSide.Text =
                    SharedWithSelectedValues.Text = (sharedWithSelectedValues.sharedWithMinistries ?? "").ToLower();

            // TO DO: Rename to SharedWithMinistries (upper start)


            CommMaterialsSelectedValuesServerSide.Text = CommMaterialsSelectedValues.Text = commMaterialsSelectedValues ?? "";

            SectorsSelectedValuesServerSide.Text = SectorsSelectedValues.Text = (sectorsSelectedValues ?? "").ToLower();

            ThemesSelectedValuesServerSide.Text = ThemesSelectedValues.Text = (themesSelectedValues ?? "").ToLower();

            TagsSelectedValuesServerSide.Text = TagsSelectedValues.Text = (tagsSelectedValues ?? "").ToLower();

            InitiativesSelectedValuesServerSide.Text = InitiativesSelectedValues.Text = initiativeSelectedValues ?? "";


            #endregion

            #region show news release from Hub

            using (var hub = new legacy::Gcpe.Hub.Data.Entity.HubEntities())
            {
                var releases = hub.NewsReleases.Where(r => r.ActivityId == ActivityId && r.IsActive).ToList();

                foreach (var Item in releases)
                {
                    var releaseType = Enum.GetName(typeof(ReleaseType), Item.ReleaseType);
                    var pageTitle = Item.Documents.First().English().PageTitle;

                    string documentType = ReleaseModel.ReleaseDocumentType(releaseType, pageTitle);
                    documentType += releaseType == "Story" && pageTitle == "Release" ? "" : pageTitle;

                    string publishStatus = "Draft";
                    if (Item.PublishDateTime.HasValue)
                        publishStatus = ReleaseModel.FormatPublishStatusDate(Item.PublishDateTime.Value, Item.IsPublished, Item.IsCommitted);

                    Model.Releases.Add(new ActivityModel.Release()
                    {
                        Id = Item.Id,
                        Color = ReleaseModel.ReleaseColor(releaseType),
                        DocumentType = documentType,
                        PublishStatus = publishStatus
                    });
                }

                ActivityNewsList.DataBind();
            }
            #endregion

            if (calendarDataContext.FavoriteActivities.Any(f => f.ActivityId == ActivityId && f.SystemUserId == Master.CustomPrincipal.Id))
                FavoriteButton.Text = "Remove from Watchlist";

            UpdateFavoriteIcon(Master.CustomPrincipal.Id);

            var files = calendarDataContext.ActivityFiles.Where(f => f.ActivityId == ActivityId);

            if (!files.Any() && !Settings.Default.ShowRecordsSection)
                RecordsSection.Style.Add("display", "none");

            documentsRepeater.DataSource = files.ToList();
            documentsRepeater.DataBind();
        }

        public void ReleaseLinksItems_Click(object sender, RepeaterCommandEventArgs e)
        {
            var customPrincipal = Master.CustomPrincipal;

            var releaseId = Guid.Parse((string)e.CommandArgument);

            using (var hub = new legacy::Gcpe.Hub.Data.Entity.HubEntities())
            {
                var release = hub.NewsReleases.SingleOrDefault(r => r.Id == releaseId);

                //Redirect the user to the Hub if they have access, otherwise return the PDF document.

                if (hub.Users.Any(u => u.EmailAddress == customPrincipal.Email))
                {
                    Response.Redirect(ReleaseModel.ReleaseHubUrl(release), false);
                }
                else
                {
                    var releaseTemplate = Gcpe.News.ReleaseManagement.Templates.Release.FromEntity(release);
                    byte[] pdfData = releaseTemplate.ToPortableDocument();

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename=New-" + DateTime.UtcNow.Ticks + ".pdf");
                    Response.BinaryWrite(pdfData);
                }
            }

            Context.ApplicationInstance.CompleteRequest();
        }

        #region Actions

        private DateTime GetDateTime(string dateInput, string timeInput, bool isEnd)
        {
            DateTime dateTime;
            if (!DateTime.TryParse(dateInput, out dateTime))
                dateTime = DateTime.MinValue;

            if (timeInput != null)
            {
                DateTime time = DateTime.ParseExact(timeInput, "h:mm tt", ci.InvariantCulture);
                // Assign the formatted values
                dateTime = dateTime.Date.AddHours(time.Hour).AddMinutes(time.Minute);
            }
            else if (isEnd)
            {
                dateTime = dateTime.Date.AddHours(23).AddMinutes(45);
            }
            return dateTime;
        }

        /// <summary>
        /// Insert a new activity
        /// </summary>
        private void Insert()
        {
            var newActivity = new CorporateCalendar.Data.Activity();
            var customPrincipal = Master.CustomPrincipal;

            var db = calendarDataContext;
            try
            {
                db.Connection.Open();
                db.Transaction =
                    db.Connection.BeginTransaction();

                // "Is" Booleans (top of the page)
                newActivity.IsConfirmed = IsDateConfirmed.Checked;
                newActivity.IsConfidential = IsConfidentialCheckBox.Checked;
                newActivity.PotentialDates = PotentialDatesTextBox.Text.Trim();

                newActivity.IsIssue = IsIssueCheckBox.Checked;
                newActivity.IsCrossGovernment = IsCrossGovernmentCheckBox.Checked;
                newActivity.IsAtLegislature = IsAtLegislatureCheckBox.Checked;

                newActivity.IsMilestone = IsMilestoneCheckBox.Checked;

                // General Fieldset
                string title = TitleTextBox.Text.Trim().Replace("\r", " ").Replace("\n", " ");
                newActivity.Title = title;

                //Details(Summary), Internal Notes(Comments)
                newActivity.Details = DetailsTextBox.Text.Trim();
                newActivity.Comments = CommentsTextBox.Text.Trim();
                SetLookAheadParams(newActivity, customPrincipal);

                newActivity.Significance = SignificanceTextBox.Text.Trim();
                newActivity.Schedule = SchedulingTextBox.Text.Trim();
                newActivity.LeadOrganization = LeadOrganizationTextBox.Text;

                newActivity.Strategy = StrategyTextBox.Text;

                if (!string.IsNullOrEmpty(ContactMinistryDropDownList.Value))
                    newActivity.ContactMinistryId = Guid.Parse(ContactMinistryDropDownList.Value);

                // The drop down has the SystemUserID, but we want to store the
                // comm contact id
                if (!string.IsNullOrEmpty(ComContactSelectedValue.Value))
                {
                    int comContactSystemUserId = int.Parse(ComContactSelectedValue.Value);
                    var comContactId = calendarDataContext.ActiveCommunicationContacts.Where(
                                    r =>
                                    r.SystemUserId == comContactSystemUserId &&
                                    r.MinistryId == Guid.Parse(ContactMinistryDropDownList.Value)).First().Id;

                    newActivity.CommunicationContactId = (int?)comContactId;
                }

                if (string.IsNullOrWhiteSpace(RepresentativeDropDownList.Value))
                    newActivity.GovernmentRepresentativeId = null;
                else
                    newActivity.GovernmentRepresentativeId = Convert.ToInt32(RepresentativeDropDownList.Value);

                // Location Fieldset
                if (!string.IsNullOrWhiteSpace(CityDropDownList.Value))
                    newActivity.CityId = Convert.ToInt32(CityDropDownList.Value);

                newActivity.OtherCity = OtherCityTextBox.Text;


                newActivity.Venue = VenueTextBox.Text;

                //+ Date & Time Fieldset

                newActivity.StartDateTime = GetDateTime(StartDate.Value, IsAllDayCheckBox.Checked ? null : StartTime.SelectedValue, false);
                newActivity.EndDateTime = GetDateTime(EndDate.Value, IsAllDayCheckBox.Checked ? null : EndTime.SelectedValue, true);
                newActivity.IsAllDay = IsAllDayCheckBox.Checked;

                // Activity Details
                if (!string.IsNullOrWhiteSpace(NRDistributionDropDownList.Value))
                    newActivity.NRDistributionId =
                        Convert.ToInt32(NRDistributionDropDownList.Value);

                if (!string.IsNullOrWhiteSpace(PremierRequestedDropDownList.Value))
                    newActivity.PremierRequestedId = Convert.ToInt32(PremierRequestedDropDownList.Value);

                if (!string.IsNullOrWhiteSpace(EventPlannerDropDownList.Value))
                    newActivity.EventPlannerId = Convert.ToInt32(EventPlannerDropDownList.Value);

                if (!string.IsNullOrWhiteSpace(VideographerDropDownList.Value))
                    newActivity.VideographerId = Convert.ToInt32(VideographerDropDownList.Value);

                //GetAndSetPriorityValue(newActivity);
                //newActivity.IsReviewed = false;
                newActivity.StatusId = 7; // New

                // Meta Data (The rest of the fields are handled by the database using simple default values)
                newActivity.LastUpdatedBy = customPrincipal.Id;
                newActivity.LastUpdatedDateTime = DateTime.Now;
                newActivity.CreatedDateTime = DateTime.Now;

                newActivity.CreatedBy = customPrincipal.Id;
                newActivity.IsActive = true;

                db.Activities.InsertOnSubmit(newActivity);
                db.SubmitChanges();
                int newActivityId = newActivity.Id;

                ActivityWebService.UpdateLinkingTables(newActivityId, GetDropDownListValues(), customPrincipal, db);

                db.SubmitChanges();

                InsertNewsFeed("add", "added activity", newActivity.Title, newActivity.Id);

                SaveDocuments(db, newActivity);

                //transactionScope.Complete();
                db.Transaction.Commit();
            }
            catch (Exception e)
            {
                db.Transaction.Rollback();
                throw e;
            }

            #endregion

            Response.Redirect("~/Calendar/Activity.aspx?ActivityId=" + newActivity.Id + "&new=1", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        protected string Markup(bool needsReview)
        {
            return IsHq && needsReview ? "reviewed" : "";
        }

        string ReplaceSpecialCharacters(string text)
        {
            // Replace: smart single quotes and apostrophe, smart double quotes, ellipsis, dashes, circumflex, open and closed angle brackets, and spaces
            text = System.Text.RegularExpressions.Regex.Replace(text, "[\u2018\u2019\u201A]", "'");
            text = System.Text.RegularExpressions.Regex.Replace(text, "[\u201C\u201D\u201E]", "\"");
            text = System.Text.RegularExpressions.Regex.Replace(text, "\u2026", "...");
            text = System.Text.RegularExpressions.Regex.Replace(text, "[\u2013\u2014]", "-");
            text = System.Text.RegularExpressions.Regex.Replace(text, "\u02C6", "^");
            text = System.Text.RegularExpressions.Regex.Replace(text, "\u2039", "<");
            text = System.Text.RegularExpressions.Regex.Replace(text, "\u203A", ">");
            return System.Text.RegularExpressions.Regex.Replace(text, "[\u02DC\u00A0]", " ");
        }

        private void SetLookAheadParams(CorporateCalendar.Data.Activity activity, CustomPrincipal customPrincipal)
        {
            if (LACommentsRow.Visible)
            {
                activity.HqComments = LACommentsTextBox.Text.Trim();
                bool isNone = string.IsNullOrEmpty(LAStatusRadioButtonList.SelectedValue);
                activity.HqStatusId = isNone ? null : (int?)Convert.ToInt32(LAStatusRadioButtonList.SelectedValue);
            }

            if (IsNewActivity && !customPrincipal.IsInRole(SecurityRole.Administrator) && !LACommentsRow.Visible)
            {
                // draw attention to the look ahead section so that admins have to go back in and edit it
                activity.HqComments = "**";
            }

            activity.HqSection = string.IsNullOrEmpty(LASectionCheckBoxList.SelectedValue) ? (int)LookAheadSection.Not_On_LA
                                                                                           : Convert.ToInt32(LASectionCheckBoxList.SelectedValue);
            if (LASectionCheckBoxList.Items[LASectionCheckBoxList.Items.Count - 1].Selected) // Long Term Outlook
                activity.HqSection = -activity.HqSection;
        }

        /// <summary>
        /// Update an existing activity
        /// Update the local CurrentActivity property
        /// </summary>
        private bool Update()
        {
            var activity = calendarDataContext.Activities.Single(p => p.Id == ActivityId);

            if (timestamp.Value != (activity.LastUpdatedDateTime ?? activity.CreatedDateTime ?? DateTime.MinValue).ToOADate().ToString())
            {
                ErrorNotice.Style.Clear();
                return false;
            }
            SavedSuccessfullyNotice.Style.Clear();
            InactivityNotice.Style.Add("display", "none");


            var customPrincipal = Master.CustomPrincipal;

            bool hasRepresentative = !string.IsNullOrWhiteSpace(RepresentativeDropDownList.Value);
            int? newRepresentativeId = hasRepresentative ? (int?)Convert.ToInt32(RepresentativeDropDownList.Value) : null;

            int? newCityId = !string.IsNullOrWhiteSpace(CityDropDownList.Value) ? (int?)Convert.ToInt32(CityDropDownList.Value) : null;

            SignificanceTextBox.Text = SignificanceTextBox.Text.Trim();
            SchedulingTextBox.Text = SchedulingTextBox.Text.Trim();
            StrategyTextBox.Text = StrategyTextBox.Text.Trim();

            bool premierRequestedHasValue = !string.IsNullOrWhiteSpace(PremierRequestedDropDownList.Value);
            int? premierRequestedId = premierRequestedHasValue ? (int?)Convert.ToInt32(PremierRequestedDropDownList.Value) : null;

            bool NRDistributionHasValue = !string.IsNullOrWhiteSpace(NRDistributionDropDownList.Value);
            int? NRDistributionId = NRDistributionHasValue ? (int?)Convert.ToInt32(NRDistributionDropDownList.Value) : null;

            string newNROrigin = NROriginDropDownList.Items.FindByText(CurrentActiveActivity.NROrigins?.TrimStart())?.Value;

            bool significanceHasChanged = activity.Significance != SignificanceTextBox.Text;
            bool internalNotesHasChanged = activity.Comments != CommentsTextBox.Text;
            bool scheduleHasChanged = activity.Schedule != SchedulingTextBox.Text;
            bool strategyHasChanged = activity.Strategy != StrategyTextBox.Text;

            bool premierRequestedHasChanged = false;
            if (premierRequestedHasValue)
                premierRequestedHasChanged = (int?)Convert.ToInt32(PremierRequestedDropDownList.Value) != activity.PremierRequestedId;

            bool digitalHasValue = !string.IsNullOrWhiteSpace(VideographerDropDownList.Value);
            bool digitalHasChanged = false;
            if (digitalHasValue)
                digitalHasChanged = (int?)Convert.ToInt32(VideographerDropDownList.Value) != activity.VideographerId;

            bool eventPlannerHasValue = !string.IsNullOrWhiteSpace(EventPlannerDropDownList.Value);
            bool eventPlannerHasChanged = false;
            if (eventPlannerHasValue)
                eventPlannerHasChanged = (int?)Convert.ToInt32(EventPlannerDropDownList.Value) != activity.EventPlannerId;

            bool initiativesHaveChanged = InitiativesSelectedValuesServerSide.Text != InitiativesSelectedValues.Text;
            var selectedKeywords = KeywordsTextBox.Text.Split('~');
            var selectedKeywordIds = calendarDataContext
                .Keywords
                .Where(k => selectedKeywords.Contains(k.Name))
                .Distinct()
                .Select(k => k.Id)
                .ToArray();
            var keywordIdsSelectedServerSide = calendarDataContext
                .ActivityKeywords
                .Where(ak => ak.ActivityId == activity.Id)
                .Select(ak => ak.KeywordId)
                .ToArray();
            bool keywordsHaveChanged = keywordIdsSelectedServerSide.Length != selectedKeywordIds.Length;

            bool NRDistributionHasChanged = false;
            if (NRDistributionHasValue)
                NRDistributionHasChanged = (int?)Convert.ToInt32(NRDistributionDropDownList.Value) != activity.NRDistributionId;

            bool NROriginHasChanged = NROriginDropDownList.Value != (newNROrigin ?? "");
            bool leadOrganizationHasChanged = activity.LeadOrganization != LeadOrganizationTextBox.Text;
            var translationsServerSide = activity.Translations ?? "";
            bool translationsRequiredHasChanged = translationsServerSide != TranslationsTextbox.Text.Replace("~", ", ");
            bool venueHasChanged = activity.Venue != VenueTextBox.Text;

            if (activity.IsConfirmed != IsDateConfirmed.Checked || activity.Significance != SignificanceTextBox.Text
                || activity.Schedule != SchedulingTextBox.Text || activity.LeadOrganization != LeadOrganizationTextBox.Text
                || activity.Strategy != StrategyTextBox.Text || activity.IsAllDay != IsAllDayCheckBox.Checked
                || activity.IsCrossGovernment != IsCrossGovernmentCheckBox.Checked || activity.PremierRequestedId != premierRequestedId
                || activity.NRDistributionId != NRDistributionId || NROriginDropDownList.Value != (newNROrigin ?? ""))
            {
                activity.IsConfirmed = IsDateConfirmed.Checked;
                activity.IsAllDay = IsAllDayCheckBox.Checked;
                activity.Significance = SignificanceTextBox.Text;
                activity.Schedule = SchedulingTextBox.Text;
                activity.OtherCity = OtherCityTextBox.Text;
                activity.IsCrossGovernment = IsCrossGovernmentCheckBox.Checked;
                activity.LeadOrganization = LeadOrganizationTextBox.Text;
                activity.Strategy = StrategyTextBox.Text.Trim();
                activity.PremierRequestedId = premierRequestedId;
                activity.NRDistributionId = NRDistributionId;
                activity.StatusId = 1; // Changed
            }

            activity.IsAtLegislature = IsAtLegislatureCheckBox.Checked;

            activity.IsMilestone = IsMilestoneCheckBox.Checked;

            if (!string.IsNullOrEmpty(NRDate.Value) && NRDate.Value != "MM/DD/YYYY")
                activity.NRDateTime = GetDateTime(NRDate.Value, NRTime.SelectedValue, false);
            else
                activity.NRDateTime = null;

            // General Fieldset
            string title = TitleTextBox.Text.Trim().Replace("\r", " ").Replace("\n", " ");

            SetLookAheadParams(activity, customPrincipal);

            activity.Comments = CommentsTextBox.Text.Trim();

            activity.ContactMinistryId = Guid.Parse(ContactMinistryDropDownList.Value);

            if (!string.IsNullOrEmpty(ComContactSelectedValue.Value))
                activity.CommunicationContactId = calendarDataContext.CommunicationContacts.Where(
                   t =>
                    t.SystemUserId == Convert.ToInt32(ComContactSelectedValue.Value) &&
                    t.MinistryId == activity.ContactMinistryId).First().Id;

            if (CityDropDownList.SelectedIndex == -1 || CityDropDownList.Items[CityDropDownList.SelectedIndex].Text != "Other...")
                OtherCityTextBox.Text = string.Empty;

            activity.Venue = VenueTextBox.Text;

            activity.EventPlannerId = eventPlannerHasValue ? (int?)Convert.ToInt32(EventPlannerDropDownList.Value) : null;

            bool videographerHasValue = !string.IsNullOrWhiteSpace(VideographerDropDownList.Value);
            activity.VideographerId = videographerHasValue ? (int?)Convert.ToInt32(VideographerDropDownList.Value) : null;

            //GetAndSetPriorityValue(activity);

            // Meta Data (The rest of the fields are handled by the database using simple default values)
            // Don't update this info if current user is GCPEHQ Admin and the activity is not a GCPEHQ activity
            if (!customPrincipal.IsGCPEHQ || !customPrincipal.IsInRoleOrGreater(SecurityRole.Administrator) ||
                activity.Ministry.Abbreviation == customPrincipal.GCPEHQ_Ministry || activity.LastUpdatedDateTime == null)
            {
                activity.LastUpdatedBy = customPrincipal.Id;
                activity.LastUpdatedDateTime = DateTime.Now;
            }

            bool categoryHasChanged = CategoriesDropDownList.Value != CategoriesDropDownList.Items.FindByText(CurrentActiveActivity.Categories.TrimStart())?.Value;
            bool commMaterialsHaveChanged = CommMaterialsSelectedValuesServerSide.Text != CommMaterialsSelectedValues.Text;
            string translations = TranslationsTextbox.Text.Replace("~", ", ");

            ActivityManager.UpdateActivity(calendarDataContext, customPrincipal, activity,
                ReplaceSpecialCharacters(title), ReplaceSpecialCharacters(DetailsTextBox.Text.Trim()),
                GetDateTime(StartDate.Value, IsAllDayCheckBox.Checked ? null : StartTime.SelectedValue, false),
                GetDateTime(EndDate.Value, IsAllDayCheckBox.Checked ? null : EndTime.SelectedValue, true), PotentialDatesTextBox.Text.Trim(),
                newRepresentativeId, newCityId, OtherCityTextBox.Text, categoryHasChanged, commMaterialsHaveChanged,
                IsIssueCheckBox.Checked, IsConfidentialCheckBox.Checked, translations, GetDropDownListValues(), significanceHasChanged,
                internalNotesHasChanged, scheduleHasChanged, strategyHasChanged, premierRequestedHasChanged, venueHasChanged, digitalHasChanged, eventPlannerHasChanged, initiativesHaveChanged,
                keywordsHaveChanged, leadOrganizationHasChanged, NRDistributionHasChanged, NROriginHasChanged, translationsRequiredHasChanged);

            SaveDocuments(calendarDataContext, activity);

            return true;
        }

        private void SaveDocuments(CorporateCalendarDataContext db, CorporateCalendar.Data.Activity activity)
        {
            string[] prohibitedExtensions = new string[] {
                #region ...
                //https://support.office.com/en-us/article/Types-of-files-that-cannot-be-added-to-a-list-or-library-30be234d-e551-4c2a-8de8-f8546ffbf5b3

                ".ashx",        /* ASP.NET Web handler file. Web handlers are software modules that handle raw HTTP requests received by ASP.NET. */
                ".asmx",        /* ASP.NET Web Services source file */
                ".json",        /* JavaScript Object Notation file */
                ".soap",        /* Simple Object Access Protocol file */
                ".svc",         /* Windows Communication Foundation (WCF) service file */
                ".xamlx",       /* Visual Studio Workflow service file */

                                ".ade",         /* Microsoft Access project extension */
                ".adp",         /* Microsoft Access project */
                ".asa",         /* ASP declarations file */
                ".ashx",        /* ASP.NET Web handler file. Web handlers are software modules that handle raw HTTP requests received by ASP.NET. */
                ".asmx",        /* ASP.NET Web Services source file */
                ".asp",         /* Active Server Pages */
                ".bas",         /* Microsoft Visual Basic class module */
                ".bat",         /* Batch file */
                ".cdx",         /* Compound index */
                ".cer",         /* Certificate file */
                ".chm",         /* Compiled HTML Help file */
                ".class",       /* Java class file */
                ".cmd",         /* Microsoft Windows NT command script */
                ".com",         /* Microsoft MS-DOS program */
                ".config",      /* Configuration file */
                ".cnt",         /* Help Contents file */
                ".cpl",         /* Control Panel extension */
                ".crt",         /* Security certificate */
                ".csh",         /* Script file */
                ".der",         /* DER Certificate file */
                ".dll",         /* Windows dynamic-link library */
                ".exe",         /* Executable file */
                ".fxp",         /* Microsoft Visual FoxPro compiled program */
                ".gadget",      /* Windows Gadget */
                ".grp",         /* SmarterMail group file */
                ".hlp",         /* Help file */
                ".hpj",         /* Hemera Photo Objects Image File */
                ".hta",         /* HTML program */
                ".htr",         /* Script file */
                ".htw",         /* HTML document */
                ".ida",         /* Internet Information Services file */
                ".idc",         /* Internet database connector file */
                ".idq",         /* Internet data query file */
                ".ins",         /* Internet Naming Service */
                ".isp",         /* Internet Communication settings */
                ".its",         /* Internet Document Set file */
                ".jse",         /* JScript Encoded script file */
                ".json",        /* JavaScript Object Notation file */
                ".ksh",         /* Korn Shell script file */
                ".lnk",         /* Shortcut */
                ".mad",         /* Shortcut */
                ".maf",         /* Shortcut */
                ".mag",         /* Shortcut */
                ".mam",         /* Shortcut */
                ".maq",         /* Shortcut */
                ".mar",         /* Shortcut */
                ".mas",         /* Microsoft Access stored procedure */
                ".mat",         /* Shortcut */
                ".mau",         /* Shortcut */
                ".mav",         /* Shortcut */
                ".maw",         /* Shortcut */
                ".mcf",         /* Multimedia Container Format */
                ".mda",         /* Microsoft Access add-in program */
                ".mdb",         /* Microsoft Access program */
                ".mde",         /* Microsoft Access MDE database */
                ".mdt",         /* Microsoft Access data file */
                ".mdw",         /* Microsoft Access workgroup */
                ".mdz",         /* Microsoft Access wizard program */
                ".ms-one-stub", /* Microsoft OneNote stub */
                ".msc",         /* Microsoft Common Console document */
                ".msh",         /* Microsoft Agent script helper */
                ".msh1",        /* Microsoft Agent script helper */
                ".msh1xml",     /* Microsoft Agent script helper */
                ".msh2",        /* Microsoft Agent script helper */
                ".msh2xml",     /* Microsoft Agent script helper */
                ".mshxml",      /* Microsoft Agent script helper */
                ".msi",         /* Microsoft Windows Installer package */
                ".msp",         /* Windows Installer update package file */
                ".mst",         /* Visual Test source files */
                ".ops",         /* Microsoft Office profile settings file */
                ".pcd",         /* Photo CD image or Microsoft Visual Test compiled script */
                ".pif",         /* Shortcut to MS-DOS program */
                ".pl",          /* Perl script */
                ".prf",         /* System file */
                ".prg",         /* Program source file */
                ".printer",     /* Printer file */
                ".ps1",         /* Windows PowerShell Cmdlet file */
                ".ps1xml",      /* Windows PowerShell Display configuration file */
                ".ps2",         /* Windows PowerShell Cmdlet file */
                ".ps2xml",      /* Windows PowerShell Display configuration file */
                ".psc1",        /* Windows PowerShell Console file */
                ".psc2",        /* Windows PowerShell Console file */
                ".pst",         /* Microsoft Outlook personal folder file */
                ".reg",         /* Registration entries */
                ".rem",         /* ACT! database maintenance file */
                ".scf",         /* Windows Explorer command file */
                ".scr",         /* Screen saver */
                ".sct",         /* Script file */
                ".shb",         /* Windows shortcut */
                ".shs",         /* Shell Scrap object */
                ".shtm",        /* HTML file that contains server-side directives */
                ".shtml",       /* HTML file that contains server-side directives */
                ".soap",        /* Simple Object Access Protocol file */
                ".stm",         /* HTML file that contains server-side directives */
                ".svc",         /* Windows Communication Foundation (WCF) service file */
                ".url",         /* Uniform Resource Locator (Internet shortcut) */
                ".vb",          /* Microsoft Visual Basic Scripting Edition (VBScript) file */
                ".vbe",         /* VBScript Encoded Script file */
                ".vbs",         /* VBScript file */
                ".vsix",        /* Visual Studio Extension */
                ".ws",          /* Windows Script file */
                ".wsc",         /* Windows Script Component */
                ".wsf",         /* Windows Script file */
                ".wsh",         /* Windows Script Host settings file */
                ".xamlx",       /* Visual Studio Workflow service file */
                #endregion
            };

            var deletedDocumentIds = deletedDocuments.Value.Split(',').Where(e => e != string.Empty).Select(int.Parse).ToList();

            var filesToDelete = db.ActivityFiles.Where(a => deletedDocumentIds.Contains(a.Id)).ToList();
            db.ActivityFiles.DeleteAllOnSubmit(filesToDelete);

            var activityFiles = db.ActivityFiles.Where(a => a.ActivityId == activity.Id).ToList();

            //Cannot use a foreach look as Request.Files.Keys is not unique
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                if (file.FileName == string.Empty)
                    continue;

                if (file.ContentLength == 0)
                    throw new ApplicationException();

                if (prohibitedExtensions.Any(e => file.FileName.ToLower().EndsWith(e)))
                    throw new ApplicationException();

                byte[] fileData = null;
                string fileMd5;
                int length = file.ContentLength;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileData = binaryReader.ReadBytes(file.ContentLength);
                    using (var md5 = MD5.Create())
                        fileMd5 = BitConverter.ToString(md5.ComputeHash(fileData)).Replace("-", "");
                }

                var activityFile = new CorporateCalendar.Data.ActivityFile()
                {
                    ActivityId = activity.Id,
                    FileLength = length,
                    FileName = Path.GetFileName(file.FileName),
                    FileType = file.ContentType,
                    Data = new System.Data.Linq.Binary(fileData),
                    Md5 = fileMd5,
                    LastUpdatedBy = activity.LastUpdatedBy,
                    LastUpdatedDateTime = DateTime.Now
                };

                var oldFiles = activityFiles.Where(f => f.FileName.ToLower() == activityFile.FileName.ToLower()).ToList();
                db.ActivityFiles.DeleteAllOnSubmit(oldFiles);

                db.ActivityFiles.InsertOnSubmit(activityFile);
            }
            db.SubmitChanges();
        }

        protected void SaveButtonClick(object sender, EventArgs e)
        {
            if (!IsNewActivity)
            {
                if (Update())
                {
                    InsertNewsFeed("change", "changed activity", CurrentActiveActivity.Title, this.ActivityId);
                    LoadExistingActivity(); //Reload the activity to show updated values.
                }
            }
            else
            {
                Insert();
            }
            ForcePageToClose();
        }

        protected void ReviewButtonClick(object sender, EventArgs e)
        {
            if (!IsNewActivity)
            {
                bool success = Update();
                if (success)
                {
                    if (ActivityManager.ReviewActivity(ActivityId.Value, timestamp.Value))
                        InsertNewsFeed("review", "reviewed activity", CurrentActiveActivity.Title, this.ActivityId);
                    LoadExistingActivity(); //Reload the activity to show updated values.
                }
                else
                {
                    ErrorNotice.Style.Clear();
                }
            }
            ForcePageToClose();
        }

        private void ForcePageToClose()
        {
            // force the tab/window to close so ministry users are forced to re-open the activity and get any subsequent changes made by HQ
            // previously, ministry users would leave the tab open, HQ would make changes and ministry users would indavertently
            // overwrite those changes made by HQ with later edits to the open activity, causing data to be incorrect
            var changesPendingMsg = "<i class=\"fa fa-exclamation-triangle fa-fw\" aria-hidden=\"true\"></i> Close this tab & re-open the activity to make changes.";
            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "window.close(); if(!window.closed) { alert('Please close this tab to avoid editing conflicts. To continue editing, close this tab & open activity again.'); $('#ActionsFieldset').hide(); $('#ChangesPendingAlert').html('" + changesPendingMsg + "'); $('#ChangesPending').removeClass('ChangesNotice-Modifier').addClass('ChangesNotice-Modifier-1').find('table:eq(0)').remove(); $('#SecondaryAlert').css({'background-color': '#F9F1C6', 'color': '#6C4A00'})}", true);
        }

        protected void FavoriteButtonClick(object sender, EventArgs e)
        {
            if (!IsNewActivity)
            {
                var dc = calendarDataContext;
                var customPrincipalId = Master.CustomPrincipal.Id;
                var favorite = dc.FavoriteActivities.SingleOrDefault(f => f.ActivityId == ActivityId && f.SystemUserId == customPrincipalId);

                if (favorite == null)
                {
                    dc.FavoriteActivities.InsertOnSubmit(new FavoriteActivity() { ActivityId = ActivityId.Value, SystemUserId = customPrincipalId });

                    FavoriteButton.Text = "Remove from Watchlist";
                }
                else
                {
                    dc.FavoriteActivities.DeleteOnSubmit(favorite);

                    FavoriteButton.Text = "Add to Watchlist";
                }

                dc.SubmitChanges();

                UpdateFavoriteIcon(customPrincipalId);

                var changesPendingMsg = "<i class=\"fa fa-exclamation-triangle fa-fw\" aria-hidden=\"true\"></i> Close this tab & re-open the activity to make changes.";
                ClientScript.RegisterStartupScript(typeof(Page), "closePage", "$('#ActionsFieldset').hide(); $('#ChangesPendingAlert').html('" + changesPendingMsg + "'); $('#ChangesPending').removeClass('ChangesNotice-Modifier').addClass('ChangesNotice-Modifier-1').find('table:eq(0)').remove(); $('#SecondaryAlert').css({'background-color': '#F9F1C6', 'color': '#6C4A00'});", true);
            }
        }

        private void UpdateFavoriteIcon(int customPrincipalId)
        {
            var favoriteUsers = calendarDataContext.FavoriteActivities.Where(f => f.ActivityId == ActivityId).Select(f => f.SystemUser);

            if (favoriteUsers.Any())
            {
                string userText = string.Join(", ", favoriteUsers.OrderBy(u => u.FullName).Select(u => u.FullName).OrderBy(n => n));

                string icon = favoriteUsers.Any(u => u.Id == customPrincipalId) ? "../images/icon-star.png" : "../images/icon-star-grey.png";

                FavoriteIcon.InnerHtml = "<img title=\"" + userText.Replace("'", "''") + "\" style=\"margin-top: 2px; margin-right: 4px;\" src=\"" + icon + "\" />";
            }
            else
            {
                FavoriteIcon.InnerHtml = "";
            }
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!IsNewActivity)
                ActivityManager.DeleteActivity(ActivityId.Value);

            /* Remove_RecordLocks
            if (!IsNewActivity) ReleaseCurrentPageRecordLock();
            */

            InsertNewsFeed("delete", "deleted activity", CurrentActiveActivity.Title, this.ActivityId);

            Response.Redirect(@"~/Calendar/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        protected void CloneButtonClick(object sender, EventArgs e)
        {
            if (!IsNewActivity)
            {
                int? clonedActivityId = ActivityWebService.CloneActiveActivity(CurrentActiveActivity, GetDropDownListValues(), Master.CustomPrincipal);
                if (clonedActivityId > 0)
                {
                    InsertNewsFeed("clone", "added new clone", CurrentActiveActivity.Title, clonedActivityId);

                    /* Remove_RecordLocks
                    if (!IsNewActivity)
                        ReleaseCurrentPageRecordLock();
                    */

                    Response.Redirect(@"~/Calendar/Activity.aspx?ActivityID=" + clonedActivityId + "&IsCloned=true", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            /* Remove_RecordLocks
            if (!IsNewActivity)
                ReleaseCurrentPageRecordLock();
            */

            Response.Redirect("~/Calendar/CancelChanges.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }


        private void InsertNewsFeed(string desc, string textDesc, string title, int? idToLink)
        {
            var dc = calendarDataContext;
            var customPrincipal = Master.CustomPrincipal;

            string dateTimeIconHtml = string.Format("<img src=\"images/calendar-edit-icon.png\" title=\"{0}\" align=\"absmiddle\" />&nbsp;", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());

            // Update News Feed table
            var newsFeed = new CorporateCalendar.Data.NewsFeed
            {
                ActivityId = idToLink,
                MinistryId = Guid.Parse(ContactMinistryDropDownList.Value),
                Text =
                    string.Format(
                        "{7}<a href=\"mailto:{2}\" style=\"color: Blue\">{0}</a> {8} <a href=\"Activity.aspx?ActivityId={1}\" style=\"color: Blue\" title=\"{6}\">{3}-{1}</a> at {4} on {5}.",
                        string.Format("{0} {1}",
                                      customPrincipal.FirstName,
                                      customPrincipal.LastName),
                        idToLink, customPrincipal.Email,
                        ContactMinistryDropDownList.Items[ContactMinistryDropDownList.SelectedIndex].Text,
                        DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortDateString(), title, dateTimeIconHtml,
                        textDesc),
                LastUpdatedBy = customPrincipal.Id,
                CreatedBy = customPrincipal.Id,
                Description = desc,
                CreatedDateTime = DateTime.Now,
                LastUpdatedDateTime = DateTime.Now,
                IsActive = true
            };

            dc.NewsFeeds.InsertOnSubmit(newsFeed);
            dc.SubmitChanges();
        }

        #region Methods

        private bool IsCurrentUserInActivityMinistryOrSharedWith()
        {
            foreach (Guid minId in Master.CustomPrincipal.SystemUserMinistryIds)
            {
                if (minId == CurrentActiveActivity.ContactMinistryId) return true;
                else
                {
                    foreach (Guid sharedWithid in SharedWithMinistryIds)
                    {
                        if (minId == sharedWithid) return true;
                    }
                }
            }
            return false;
        }

        private void DisableFormInput()
        {
            ActionsFieldset.Visible = false;
        }

        private void ShowHideButtons()
        {
            // Hide the entire actionsfieldset if the activity is
            // user is read-only

            if (IsUserReadOnly)
            {
                DisableFormInput();
            }
            else
            {
                ActionsFieldset.Visible = true;
            }

            /* CloneButton.Visible =
            ReviewButton.Visible =
            DeleteButton.Visible = (Master.CustomPrincipal.IsInApplicationOwnerOrganizations
            || ((CurrentUserMinistryIds.Contains(CurrentActiveActivity.MinistryId) && Master.CustomPrincipal.RoleId > 1)));
            */

            CloneButton.Visible =
                DeleteButton.Visible = (ActivityId != null) && (ActivityId != -1);

            // If the contact ministry is not in the current user's ministry membership group,
            // then that user cannot edit the record
            if (!IsNewActivity && !Master.CustomPrincipal.IsInApplicationOwnerOrganizations)
            {
                if (!CurrentActiveActivity.ContactMinistryId.HasValue || !Master.CustomPrincipal.SystemUserMinistryIds.Contains(CurrentActiveActivity.ContactMinistryId.Value))
                {
                    ActionsFieldset.Visible = false;
                }
            }
            SaveButton.Visible = ActionsFieldset.Visible;

            ReviewButton.Visible =
                !IsNewActivity && Master.CustomPrincipal.RoleId > 2 && Master.CustomPrincipal.IsInApplicationOwnerOrganizations;
        }

        private Dictionary<string, string> GetDropDownListValues()
        {
            var dropDownListValues = new Dictionary<string, string>
                                        {
                                            {"ActivitySharedWithIds", SharedWithSelectedValues.Text},
                                            {"ActivityCategoryIds", CategoriesDropDownList.Value},
                                            {"ActivityCommunicationMaterialIds", CommMaterialsSelectedValues.Text},
                                            {"ActivityNewsReleaseOriginIds", NROriginDropDownList.Value},
                                            {"ActivityNewsReleaseDistributionIds", NRDistributionDropDownList.Value}, // SPECIAL CASE
                                            {"ActivitySectorIds", SectorsSelectedValues.Text},
                                            {"ActivityThemeIds", ThemesSelectedValues.Text},
                                            {"ActivityTagIds", TagsSelectedValues.Text},
                                            {"ActivityKeywords", KeywordsTextBox.Text},
                                            {"ActivityInitiativeIds", InitiativesSelectedValues.Text}
                                        };

            return dropDownListValues;
        }

        public int? CloneActivity()
        {
            int? clonedActivityId = null;

            /* Remove_RecordLocks
            if (!IsNewActivity)
            {
                clonedActivityId = ActivityWebService.CloneActiveActivity(CurrentActiveActivity, GetDropDownListValues(), Master.CustomPrincipal);
                if (clonedActivityId > 0)
                {
                    if (!IsNewActivity) ReleaseCurrentPageRecordLock();
                }
            }
            */

            return clonedActivityId;
        }
        #endregion
    }
}