using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace CorporateCalendarAdmin
{
    public partial class Transfer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var corporateCalendarDataContext = new CorporateCalendar.Data.CorporateCalendarDataContext();

            var communicationContacts = corporateCalendarDataContext.ActiveCommunicationContacts
                .Select(c => new { c.Id, c.Name, c.MinistryId, c.MinistryShortName }).OrderBy(c => c.Name);

            foreach (var communicationContact in communicationContacts)
            {
                CommunicationContactFromDropDownList.Items.Add(
                    new ListItem(string.Format("{0} ({1})", communicationContact.Name, communicationContact.MinistryShortName),
                        communicationContact.Id.ToString(CultureInfo.InvariantCulture)));
                CommunicationContactToDropDownList.Items.Add(
                    new ListItem(string.Format("{0} ({1})", communicationContact.Name, communicationContact.MinistryShortName),
                        communicationContact.Id.ToString(CultureInfo.InvariantCulture)));
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // Transfer the activities from one communication contact to another
            using (var corporateCalendarDataContext = new CorporateCalendar.Data.CorporateCalendarDataContext()) { 

                // The "to" communication contact's ministryId must be obtained
                // Since system users can be associated with multiple ministries, we must parse out
                // the ministry abbreviation from the Selected Text and do a look up.

                #region Regular Expression explanation

                // (?<=\().*(?=\))
                // 
                // Options: case insensitive; ^ and $ match at line breaks
                // 
                // Assert that the regex below can be matched, with the match ending at this position (positive lookbehind) «(?<=\()»
                //    Match the character “(” literally «\(»
                // Match any single character that is not a line break character «.*»
                //    Between zero and unlimited times, as many times as possible, giving back as needed (greedy) «*»
                // Assert that the regex below can be matched, starting at this position (positive lookahead) «(?=\))»
                //    Match the character “)” literally «\)»

                #endregion

                // Parse out the ministry abbreviation
                string ministryAbbreviation = Regex.Match(
                    CommunicationContactToDropDownList.SelectedItem.Text, @"(?<=\().*(?=\))",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline).Value;

                // Get the ministry id
                var ministryId =
                    corporateCalendarDataContext.Ministries.Where(s => s.Abbreviation == ministryAbbreviation).Select(m => m.Id).SingleOrDefault();


                var activities =
                    corporateCalendarDataContext.Activities.Where(
                        a => a.CommunicationContactId == Convert.ToInt32(CommunicationContactFromDropDownList.SelectedValue)
                             && a.IsActive).ToList();


                int activityCount = activities.Count;

                foreach (var activity in activities.ToList())
                {
                    activity.CommunicationContactId = Convert.ToInt32(CommunicationContactToDropDownList.SelectedValue);
                    activity.ContactMinistryId = ministryId;
                }
                // Submit the changes to the database.
                try
                {
                    corporateCalendarDataContext.SubmitChanges();
                    Label3.Text = string.Format("{0} activities transferred successfully from {1} to {2}.", activityCount, CommunicationContactFromDropDownList.SelectedItem.Text, CommunicationContactToDropDownList.SelectedItem.Text);
                    Label3.Style.Add("color", "red");
                    Label3.Style.Add("weight", "bold");
                }
                catch (Exception ex)
                {
                    Response.Write(ex.ToString());
                    // log the problem
                }
            }
        }
    }
}