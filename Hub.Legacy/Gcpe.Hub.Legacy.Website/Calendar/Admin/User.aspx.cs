extern alias legacy;
using legacy::Gcpe.Hub;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using CorporateCalendar.Data;

namespace CorporateCalendarAdmin
{
    public partial class User : System.Web.UI.Page
    {
        private enum CommContactTypeSortOrder
        {
            Is_not_a_Communication_Contact = 0,
            Comm_Director = 1,
            Comm_Manager = 2,
            Sr_PAO = 3,
            PAO = 4,
            Jr_PAOe = 5,
            Other = 6
        }

        private const string IS_VALID = "Is_Valid";
        private bool UserIsValid
        {
            get
            {
                if (ViewState[IS_VALID] != null)
                    return Convert.ToBoolean(ViewState[IS_VALID]);
                else
                    return false;
            }

            set { ViewState[IS_VALID] = value; }
        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var corporateCalendarDataContext = new CorporateCalendar.Data.CorporateCalendarDataContext();
            var ministries = corporateCalendarDataContext.Ministries.Where(m => m.IsActive).OrderBy(m => m.SortOrder).ThenBy(m => m.Abbreviation).Select(m => new { m.Abbreviation, m.Id });

            if ((ContactMinistryListBox != null) && (ContactMinistryListBox.Items.Count == 0))
            {
                //Populate ministries in drop down list box.
                foreach (var ministry in ministries)
                {
                    ContactMinistryListBox.Items.Add(new ListItem(ministry.Abbreviation, ministry.Id.ToString()));
                }
            }

            if (Request.QueryString["SystemUserId"] != null &&
                Convert.ToInt32(Request.QueryString["SystemUserId"]) > 0)
            {
                //Set contact ministry membership in dropdown
                SetMinistryContactList(corporateCalendarDataContext);
            }
        }


        /// <summary>
        /// Clear all the radio buttons.
        /// </summary>
        private void ClearRadios()
        {
            CommContactTypeSortRadioButtonList.SelectedIndex = CommContactTypeSortRadioButtonList.Items.IndexOf(CommContactTypeSortRadioButtonList.Items.FindByValue("0"));
            IsActiveRadioButtonList.SelectedIndex = IsActiveRadioButtonList.Items.IndexOf(IsActiveRadioButtonList.Items.FindByValue("false"));
            RoleRadioButtonList.SelectedIndex = RoleRadioButtonList.Items.IndexOf(RoleRadioButtonList.Items.FindByValue("1"));
        }

        /// <summary>
        /// Clear list box.
        /// </summary>
        private void ClearList()
        {
            SetList(ContactMinistryListBox, true, false);
            SetList(ContactMinistryListBox, false, false);
        }

        /// <summary>
        /// Clear textboxes.
        /// </summary>
        private void ClearEdits()
        {
            AccountNameTextBox.Text = string.Empty;
            FirstNameTextBox.Text = string.Empty;
            LastNameTextBox.Text = string.Empty;
            DisplayNameTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            JobTitleTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Clear radios, lists, textboxes before verifying user.
        /// </summary>
        private void ClearBeforeVerifyUser()
        {
            ClearRadios();
            ClearList();
            ClearEdits();
            Label2.Text = string.Empty;
        }

        /// <summary>
        /// Set the list box.
        /// </summary>
        /// <param name="lbox"></param>
        /// <param name="compareFlag"></param>
        /// <param name="setFlag"></param>
        private void SetList(ListBox lbox, bool compareFlag, bool setFlag)
        {
            if (lbox != null)
                foreach (ListItem lb in lbox.Items)
                {
                    if (lb.Selected == compareFlag)
                    {
                        lb.Selected = setFlag;
                    }
                }
        }

        /// <summary>
        /// Populate the controls when the user come into this page the first time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UserIsValid = true;

                if (Request.QueryString["SystemUserId"] != null &&
                    Convert.ToInt32(Request.QueryString["SystemUserId"]) > 0)
                {
                    int userId = int.Parse(Request.QueryString["SystemUserId"]);
                    LoadUser(userId);
                }
            }

            //clear lower message
            Label2.Text = string.Empty;
        }


        private void LoadUser(int userId)
        {
            // this is an update
            // fetch the user
            var corporateCalendarDataContext = new CorporateCalendar.Data.CorporateCalendarDataContext();

            var systemUser = corporateCalendarDataContext.SystemUsers
               .Where(d => d.Id.Equals(userId) && d.IsActive).FirstOrDefault();


            if (systemUser != null)
            {
                AccountNameTextBox.Text = systemUser.Username;
                FirstNameTextBox.Text = systemUser.FullName.Split(' ')[0];
                LastNameTextBox.Text = systemUser.FullName.Substring(systemUser.FullName.IndexOf(' ') + 1);
                DisplayNameTextBox.Text = systemUser.DisplayName;
                JobTitleTextBox.Text = systemUser.JobTitle;
                PhoneTextBox.Text = systemUser.PhoneNumber;
                MobileTextBox.Text = systemUser.MobileNumber;
                EmailTextBox.Text = systemUser.EmailAddress;

                if (systemUser.RoleId > 0)
                {
                    RoleRadioButtonList.SelectedValue =
                        systemUser.RoleId.ToString(CultureInfo.InvariantCulture);
                }

                IsActiveRadioButtonList.SelectedValue = "true";

                // select phone, job title, min short name, sort order and isActive values from contact table based on UserID and ministry short name
                var communicationContactRecords = corporateCalendarDataContext.CommunicationContacts
                    .Where(c => c.SystemUserId.Equals(userId) && c.IsActive.Equals(true))
                    .Select(c => new { c.SortOrder, c.IsActive }); // c.PhoneNumber,  c.JobTitle, 


                var orDefault = communicationContactRecords.FirstOrDefault();

                //there is a comm contact record.
                if (orDefault != null)
                {
                    //Since we are dealing with old code, we are setting people to other
                    // who are comm contacts but aren't one of the set sort order ids in the
                    // radio button list
                    string sort = orDefault.SortOrder != null ? Convert.ToString(orDefault.SortOrder) : CommContactTypeSortOrder.Other.ToString();
                    CommContactTypeSortRadioButtonList.SelectedIndex = CommContactTypeSortRadioButtonList.Items.IndexOf(CommContactTypeSortRadioButtonList.Items.FindByValue(sort));
                    // If it's not found, default to other (till data is all consistent)
                    if (CommContactTypeSortRadioButtonList.SelectedIndex == -1)
                        CommContactTypeSortRadioButtonList.SelectedIndex = CommContactTypeSortRadioButtonList.Items.IndexOf(CommContactTypeSortRadioButtonList.Items.FindByValue(CommContactTypeSortOrder.Other.ToString()));
                }
                else
                {
                    CommContactTypeSortRadioButtonList.SelectedValue = "0";
                }
            }
        }

        /// <summary>
        /// Set the ministry memberships for the user.
        /// </summary>
        /// <param name="vCorporateCalendarDataContext"></param>
        private void SetMinistryContactList(CorporateCalendar.Data.CorporateCalendarDataContext vCorporateCalendarDataContext)
        {

            if (Request.QueryString["SystemUserId"] != null && Convert.ToInt32(Request.QueryString["SystemUserId"]) > 0 && UserIsValid)
            {
                var ministryContactRecords = vCorporateCalendarDataContext.SystemUserMinistries.Where(
                    c => c.SystemUserId.Equals(Convert.ToInt32(Request.QueryString["SystemUserId"])) &&
                    c.IsActive.Equals(true)).Select(
                        c => new { c.MinistryId });

                //unselect list box
                foreach (ListItem l in from ListItem l in ContactMinistryListBox.Items where l.Selected select l)
                {
                    l.Selected = false;
                }


                foreach (var source in ministryContactRecords.ToArray().Where(source => source.MinistryId.HasValue))
                {
                    foreach (ListItem l in from ListItem l in ContactMinistryListBox.Items where source != null && l.Value == Convert.ToString(source.MinistryId) select l)
                    {
                        l.Selected = true;
                        break;
                    }
                }

            }
        }

        /// <summary>
        /// Verify if the user exists in Active Directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void VerifyButton_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> user = null;
            const string message = "The username does not exist in Active Directory.";

            // Lookup user in Active Directory and populate the fields.
            try
            {
                user = CorporateCalendar.Security.DirectoryServices.GetUserInfo(AccountNameTextBox.Text);

                //take the user into update mode if user already exists in the system - Ali 2/23/12
                var corporateCalendarDataContext = new CorporateCalendarDataContext();
                if (user == null)
                {
                    ErrorLabel.Text = message;
                    ClearBeforeVerifyUser();
                    return;
                }

                var existingUser = corporateCalendarDataContext.SystemUsers.FirstOrDefault(m => m.Username == AccountNameTextBox.Text);
                if (existingUser == null)
                {
                    CommContactTypeSortRadioButtonList.SelectedValue = "0";
                    RoleRadioButtonList.SelectedIndex = 0;
                    MobileTextBox.Text = string.Empty;
                    /*Response.Redirect("User.aspx?SystemUserId=" + existingUser.Id, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;*/
                }
            }
            catch (NullReferenceException)
            {
                var log = new CorporateCalendar.Logging.Log(message, CorporateCalendar.Logging.Log.LogType.Error, true);
            }

            if (user != null)
            {
                IsActiveRadioButtonList.SelectedValue = "true";
                foreach (var properties in user)
                {
                    switch (properties.Key)
                    {
                        case "FirstName":
                            if (FirstNameTextBox != null) FirstNameTextBox.Text = (properties.Value);
                            break;
                        case "LastName":
                            if (LastNameTextBox != null) LastNameTextBox.Text = (properties.Value);
                            break;
                        case "ExchangeName":
                            if (DisplayNameTextBox != null) DisplayNameTextBox.Text = (properties.Value);
                            break;
                        case "PhoneNumber":
                            if (PhoneTextBox != null) PhoneTextBox.Text = (properties.Value);
                            break;
                        case "Email":
                            if (EmailTextBox != null) EmailTextBox.Text = (properties.Value);
                            break;
                        case "Title":
                            if (JobTitleTextBox != null) JobTitleTextBox.Text = (properties.Value);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// On save if existing user goto'update' mode or if new user 'save' mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                if (Request.QueryString["SystemUserId"] != null &&
                    Convert.ToInt32(Request.QueryString["SystemUserId"]) > 0)
                {
                    if (!IsDataValidToUpdate())
                        return;

                    Update();

                }
                else
                {
                    if (!IsDataValidToSave())
                        return;

                    Insert();
                }

            }
        }

        /// <summary>
        /// Save process.
        /// </summary>
        private void Insert()
        {
            string isActive = IsActiveRadioButtonList.SelectedValue;
            string role = RoleRadioButtonList.SelectedValue;
            //Default role to 1 if not set
            if (string.IsNullOrEmpty(role)) role = "1";

            using (var corporateCalendarDataContext = new CorporateCalendar.Data.CorporateCalendarDataContext())
            {
                try
                {
                    corporateCalendarDataContext.Connection.Open();
                    corporateCalendarDataContext.Transaction =
                        corporateCalendarDataContext.Connection.BeginTransaction();

                    bool isActiveBool = (!String.IsNullOrEmpty(isActive) && isActive == "true") || (String.IsNullOrEmpty(isActive)) ? true : false;

                    string fullNameString = string.Format("{0} {1}", FirstNameTextBox.Text, LastNameTextBox.Text);
                    int roleInt = Convert.ToInt32(role);
                    var customPrincipal =
                        new CorporateCalendar.Security.CustomPrincipal(HttpContext.Current.User.Identity);


                    /************************** SystemUser record - create a new record ************************/
                    // System User
                    var systemUser = new SystemUser
                    {
                        IsActive = isActiveBool,
                        FullName = fullNameString,
                        Username = AccountNameTextBox.Text,
                        DisplayName = DisplayNameTextBox.Text,
                        RoleId = roleInt,
                        JobTitle = JobTitleTextBox.Text,
                        PhoneNumber = PhoneTextBox.Text,
                        MobileNumber = Utility.FormatPhoneNumber(MobileTextBox.Text),
                        EmailAddress = EmailTextBox.Text,
                        RowGuid = Guid.NewGuid()
                    };

                    corporateCalendarDataContext.SystemUsers.InsertOnSubmit(systemUser);
                    corporateCalendarDataContext.SubmitChanges();
                    var systemUserId = systemUser.Id;
                    /*******************************************End***************************************************/


                    // Ministries - add to SystemUserMinistries and CommunicationContacts if required
                    foreach (ListItem listItem in ContactMinistryListBox.Items)
                    {
                        var ministryId = Guid.Parse(listItem.Value);

                        if (listItem.Selected)
                        {
                            /***************** SystemUserMinistry record - create a new record ************************/
                            var systemUserMinistry = new SystemUserMinistry()
                            {
                                SystemUserId = systemUserId,
                                MinistryId = ministryId,
                                IsActive = true,
                                CreatedBy = customPrincipal.Id,
                                LastUpdatedBy = customPrincipal.Id,
                                CreatedDateTime = DateTime.Now,
                                LastUpdatedDateTime = DateTime.Now,
                            };

                            corporateCalendarDataContext.SystemUserMinistries.InsertOnSubmit(systemUserMinistry);
                            corporateCalendarDataContext.SubmitChanges();
                            /*****************************************End*****************************************************/


                            /***************** CommunicationContact record - create a new record ****************************/
                            if (CommContactTypeSortRadioButtonList.SelectedValue != "0")
                            {
                                var communicationContact = new CommunicationContact()
                                {
                                    Name = fullNameString,
                                    MinistryShortName = listItem.Text,
                                    MinistryId = ministryId,
                                    IsActive = true,
                                    SystemUserId = systemUserId,
                                    SortOrder = !String.IsNullOrEmpty(CommContactTypeSortRadioButtonList.SelectedValue) ? Convert.ToInt32(CommContactTypeSortRadioButtonList.SelectedValue) : (int?)null
                                };

                                corporateCalendarDataContext.CommunicationContacts.InsertOnSubmit(communicationContact);
                                corporateCalendarDataContext.SubmitChanges();

                            }
                            /**************************************End********************************************************/
                        }
                    }


                    corporateCalendarDataContext.Transaction.Commit();
                    SetLabelColorText(Label2, "Saved successfully.", "green");
                    UserIsValid = true;

                    if (Request.QueryString["user"] != null && Request.QueryString["user"] == "new")
                        SaveButton.Enabled = false;
                    else
                    {
                        SaveButton.Enabled = true;
                    }
                }
                catch
                {
                    //TODO: Remove this for Debugging
                    throw;

                    corporateCalendarDataContext.Transaction.Rollback();
                    SetLabelColorText(Label2, "Save failed.", "red");
                    UserIsValid = false;
                }
            }
        }

        /// <summary>
        /// Update or Add a record to the System User Ministry, Communication Contact, System User tables
        /// Commit and rollback accordingly in case of error.
        /// Added by Ali - February 22, 2012
        /// </summary>
        private void Update()
        {
            // get basic info from admin page
            string isActive = IsActiveRadioButtonList.SelectedValue;
            string role = RoleRadioButtonList.SelectedValue;

            //Default role to 1 if not set
            if (string.IsNullOrEmpty(role)) role = "1";

            using (var corporateCalendarDataContext = new CorporateCalendarDataContext())
            {
                corporateCalendarDataContext.Connection.Open();
                corporateCalendarDataContext.Transaction =
                    corporateCalendarDataContext.Connection.BeginTransaction();

                //default unselected IsActive to 1
                bool isActiveBool = (!String.IsNullOrEmpty(isActive) && isActive == "true") || (String.IsNullOrEmpty(isActive)) ? true : false;
                string fullNameString = string.Format("{0} {1}", FirstNameTextBox.Text, LastNameTextBox.Text);
                int roleInt = Convert.ToInt32(role);

                var customPrincipal =
                    new CorporateCalendar.Security.CustomPrincipal(HttpContext.Current.User.Identity);

                var systemUserId = Convert.ToInt32(Request.QueryString["SystemUserId"]);
                foreach (Ministry ministry in corporateCalendarDataContext.Ministries.Where(s => s.ContactUserId == systemUserId || s.SecondContactUserId == systemUserId))
                {
                    ministry.Timestamp = DateTime.Now;
                }

                SystemUser systemUserUpdt = corporateCalendarDataContext.SystemUsers.FirstOrDefault(s => s.Id == systemUserId);

                /************************** SystemUser record - update new record ************************/
                //Set the SystemUser
                if (systemUserUpdt != null)
                {
                    systemUserUpdt.IsActive = isActiveBool;
                    systemUserUpdt.FullName = fullNameString;
                    systemUserUpdt.Username = AccountNameTextBox.Text;
                    systemUserUpdt.DisplayName = DisplayNameTextBox.Text;
                    systemUserUpdt.RoleId = roleInt;
                    systemUserUpdt.JobTitle = JobTitleTextBox.Text;
                    systemUserUpdt.PhoneNumber = PhoneTextBox.Text;
                    systemUserUpdt.MobileNumber = Utility.FormatPhoneNumber(MobileTextBox.Text);
                    systemUserUpdt.EmailAddress = EmailTextBox.Text;
                    systemUserUpdt.LastUpdatedBy = customPrincipal.Id;
                    systemUserUpdt.LastUpdatedDateTime = DateTime.Now;
                }
                corporateCalendarDataContext.SubmitChanges();

                /*******************************************End***************************************************/

                bool isComActive = CommContactTypeSortRadioButtonList.SelectedValue != "0";

                // System User Ministries
                foreach (ListItem listItem in ContactMinistryListBox.Items)
                {
                    var ministryId = Guid.Parse(listItem.Value);

                    SystemUserMinistry systemUserMinistry = corporateCalendarDataContext.SystemUserMinistries
                            .FirstOrDefault(s => s.MinistryId == ministryId && s.SystemUserId == systemUserId);

                    CommunicationContact communicationContact =
                     corporateCalendarDataContext.CommunicationContacts.FirstOrDefault(s => s.MinistryId == ministryId && s.SystemUserId == systemUserId);


                    if (listItem.Selected) // Add or Update
                    {
                        // SystemUserMinistry record 
                        if (systemUserMinistry != null)
                        {
                            // Just updating to active is not already, and updating last updated dates.
                            systemUserMinistry.SystemUserId = systemUserId;
                            systemUserMinistry.MinistryId = ministryId;
                            systemUserMinistry.IsActive = true;
                            systemUserMinistry.LastUpdatedBy = customPrincipal.Id;
                            systemUserMinistry.LastUpdatedDateTime = DateTime.Now;
                        }
                        else
                        {
                            // Add new record for the user/ministry association
                            var systemUserMinistryNew = new SystemUserMinistry()
                            {
                                SystemUserId = systemUserId,
                                MinistryId = ministryId,
                                IsActive = true,
                                CreatedBy = customPrincipal.Id,
                                LastUpdatedBy = customPrincipal.Id,
                                CreatedDateTime = DateTime.Now,
                                LastUpdatedDateTime = DateTime.Now,
                            };

                            corporateCalendarDataContext.SystemUserMinistries.InsertOnSubmit(systemUserMinistryNew);
                        }
                        corporateCalendarDataContext.SubmitChanges();


                        // Communication Contact
                        if (isComActive)
                        {
                            if (communicationContact != null)
                            {
                                //Update
                                communicationContact.Name = fullNameString;
                                communicationContact.MinistryShortName = listItem.Text;
                                communicationContact.MinistryId = ministryId;
                                communicationContact.IsActive = true;
                                communicationContact.SystemUserId = systemUserId;
                                communicationContact.SortOrder = !string.IsNullOrEmpty(CommContactTypeSortRadioButtonList.SelectedValue) ? Convert.ToInt32(CommContactTypeSortRadioButtonList.SelectedValue) : (int?)null; ;
                            }
                            else
                            {
                                //Insert
                                var communicationContactNew = new CommunicationContact()
                                {
                                    Name = fullNameString,
                                    MinistryShortName = listItem.Text,
                                    MinistryId = ministryId,
                                    IsActive = isComActive,
                                    SystemUserId = systemUserId,
                                    SortOrder = !string.IsNullOrEmpty(CommContactTypeSortRadioButtonList.SelectedValue) ? Convert.ToInt32(CommContactTypeSortRadioButtonList.SelectedValue) : (int?)null
                                };

                                corporateCalendarDataContext.CommunicationContacts.InsertOnSubmit(
                                    communicationContactNew);
                            }
                            corporateCalendarDataContext.SubmitChanges();
                        }
                        else
                        {
                            // User is may have been communication contact, set to false
                            if (communicationContact != null && communicationContact.MinistryShortName.Equals(listItem.Text))
                            {
                                communicationContact.IsActive = false;
                                corporateCalendarDataContext.SubmitChanges();
                            }
                        }
                    } // Not Selected, check for remove
                    else
                    {
                        if (communicationContact != null && communicationContact.MinistryShortName.Equals(listItem.Text))
                        {
                            communicationContact.IsActive = false;
                        }
                        if (systemUserMinistry != null)
                        {
                            systemUserMinistry.IsActive = false;
                            systemUserMinistry.LastUpdatedBy = customPrincipal.Id;
                            systemUserMinistry.LastUpdatedDateTime = DateTime.Now;
                        }
                        corporateCalendarDataContext.SubmitChanges();
                    }
                }

                corporateCalendarDataContext.Transaction.Commit();
                SetLabelColorText(Label2, "Saved successfully.", "green");
                UserIsValid = true;
            }
        }

        /// <summary>
        /// Perform validation before update.
        /// </summary>
        /// <returns></returns>
        private bool IsDataValidToUpdate()
        {
            if (string.IsNullOrEmpty(FirstNameTextBox.Text) || string.IsNullOrEmpty(LastNameTextBox.Text) ||
                string.IsNullOrEmpty(AccountNameTextBox.Text)) 
            {
                UserIsValid = false;
                SetLabelColorText(Label2, "Mandatory field is missing.", "red");
                return false;
            }
            if (ContactMinistryListBox.SelectedItem == null)
            {
                UserIsValid = false;
                SetLabelColorText(Label2, "Please select at least 1 Ministry.", "red");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Perform Validation on the data.
        /// </summary>
        /// <returns></returns>
        private bool IsDataValidToSave()
        {
            if (!IsDataValidToUpdate())
                return false;

            if (!IsUserValid())
            {
                UserIsValid = false;
                SetLabelColorText(Label2, string.Format("User {0} already exists. Please enter a new user.", AccountNameTextBox.Text), "red");
                return false;
            }

            if (!IsContactMinistrySelected())
            {
                UserIsValid = false;
                SetLabelColorText(Label2, "Please select at least one ministry to associate with the user.", "red");
                return false;
            }
            return true;
        }

        private bool IsContactMinistrySelected()
        {
            bool isSelected = false;

            // loop through all listItems to see if at least one is selected
            foreach (ListItem listItem in ContactMinistryListBox.Items)
            {
                if (listItem.Selected)
                {
                    isSelected = true;
                }
            }
            return isSelected;
        }


        /// <summary>
        /// Does the user exist
        /// </summary>
        /// <returns></returns>
        private bool IsUserValid()
        {
            var corporateCalendarDataContext = new CorporateCalendarDataContext();
            if (corporateCalendarDataContext.SystemUsers.Any(a => a.Username.ToUpper().Equals(AccountNameTextBox.Text.ToUpper())))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Set message color.
        /// </summary>
        /// <param name="lbl"></param>
        /// <param name="errMsg"></param>
        /// <param name="color"></param>
        private void SetLabelColorText(Label lbl, String errMsg, String color)
        {
            lbl.Text = errMsg;
            lbl.Style.Add("color", color);
        }
    }
}
