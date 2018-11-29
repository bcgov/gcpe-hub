using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class ActivityFilter :  System.Web.Services.WebService
{
    /// <summary>
    /// user is trying to save a "my query"
    /// </summary>
    /// <param name="name"></param>
    /// <param name="queryString"></param>
    /// <returns></returns>
    [WebMethod]
    public int SaveNewQuery(string name, string queryString)
    {
        var dc = new CorporateCalendar.Data.CorporateCalendarDataContext();
        using (dc)
        {
            var activityFilter = new CorporateCalendar.Data.ActivityFilter();
            var customPrincipal = new CorporateCalendar.Security.CustomPrincipal(HttpContext.Current.User.Identity);

            int? newSortVal = (from p in dc.ActivityFilters
                                where (p.CreatedBy == customPrincipal.Id && p.IsActive == true)
                                select p).Select(p => p.SortOrder).Max();
            if (newSortVal == null)
                newSortVal = 1;
            else
                newSortVal = newSortVal + 1; //get next value;

            activityFilter.Name = name;
            activityFilter.QueryString = queryString;
            activityFilter.CreatedBy = customPrincipal.Id;
            activityFilter.CreatedDateTime = DateTime.Now;
            activityFilter.LastUpdatedDateTime = DateTime.Now;
            activityFilter.LastUpdatedBy = customPrincipal.Id;
            activityFilter.IsActive = true;
            activityFilter.SortOrder = newSortVal;
            dc.ActivityFilters.InsertOnSubmit(activityFilter);

            dc.SubmitChanges();
            return activityFilter.Id;
        }
    }

    /// <summary>
    /// From Default.aspx saves the name, order and any deletes
    /// </summary>
    /// <param name="names"></param>
    /// <param name="todelete"></param>
    /// <returns></returns>
    [WebMethod]
    public List<CorporateCalendar.Data.ActivityFilter> SaveMyQueries(string names, string todelete)
    {

        var serializer = new JavaScriptSerializer();

        // Index is the order, then Id|Name
        object[] idOrderName = serializer.Deserialize<dynamic>(names);
        string deleteIds = serializer.Deserialize<string>(todelete);

        var customPrincipalId = new CorporateCalendar.Security.CustomPrincipal(HttpContext.Current.User.Identity).Id;


        var dc = new CorporateCalendar.Data.CorporateCalendarDataContext();
        dc.Refresh(RefreshMode.OverwriteCurrentValues, dc.ActivityFilters);

            var activityFilter = new CorporateCalendar.Data.ActivityFilter();

            // Delete if required
            if (deleteIds != string.Empty)
            {
                int[] idsToDelete = deleteIds.Split('|').Select(n => Convert.ToInt32(n)).ToArray();

                var allToDelete =
                    from x in dc.ActivityFilters
                    where idsToDelete.Contains(x.Id)
                    select x;

                //Set IsActive = false instead of delete
                //dc.ActivityFilters.DeleteAllOnSubmit(allToDelete);
                foreach (CorporateCalendar.Data.ActivityFilter af in allToDelete)
                {
                    af.IsActive = false;
                }
            }
            dc.SubmitChanges();

            

            // Now update the order and name for each 
            for (int i = 0; i < idOrderName.Length; i++)
            {
                //i = index
                //item[0] = id
                //item[1] = name
                if (idOrderName[i].ToString() != "0") // When empty list, or all deleted this is what is returned.
                {
                    string[] item = idOrderName[i].ToString().Split('|');
                    int id = int.Parse(item[0]);
                    string name = item[1];

                    var dbItem = dc.ActivityFilters.Single(p => p.Id == id);
                    dbItem.Name = name;
                    dbItem.SortOrder = i;
                    //dbItem.LastUpdatedBy = customPrincipalId;
                }
            }
            dc.SubmitChanges();

            //Regenerate the list to avoid JavaScriptSerializer's Serialization exception
            IOrderedQueryable<CorporateCalendar.Data.ActivityFilter> filters = dc.ActivityFilters.Where(p => (p.CreatedBy == customPrincipalId && p.IsActive == true)).Select(p => p).OrderBy(p => p.SortOrder);
            List<CorporateCalendar.Data.ActivityFilter> filtersList = new List<CorporateCalendar.Data.ActivityFilter>();
            foreach(CorporateCalendar.Data.ActivityFilter filter in filters)
            {
                CorporateCalendar.Data.ActivityFilter newFilter = new CorporateCalendar.Data.ActivityFilter();
                newFilter.Id = filter.Id;
                newFilter.Name = filter.Name;
                newFilter.QueryString = filter.QueryString;
                newFilter.IsActive = filter.IsActive;
                newFilter.SortOrder = filter.SortOrder;
                filtersList.Add(newFilter);
            }
            if (filtersList.Count == 0) return null;
            return filtersList;

    }
}