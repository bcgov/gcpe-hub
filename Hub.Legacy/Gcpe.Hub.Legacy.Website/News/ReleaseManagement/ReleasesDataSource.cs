extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.News.ReleaseManagement
{
    using legacy::Gcpe.Hub.Data.Entity;

    public class ReleasesDataSource
    {
        public ReleasesModel Model { get; set; }

        public IEnumerable<ReleasesModel.Result> GetNewsReleases(int maximumRows, int startRowIndex)
        {
            if (Model == null || Model.ResultSet == null)
                return (new List<ReleasesModel.Result>()).ToArray();
            return Model.GetResults(maximumRows, startRowIndex);
        }

        public int GetNewsReleasesCount()
        {
            if (Model == null || Model.ResultSet == null)
                return 0;
                return Model.CountResults();
        }

        public IEnumerable<ReleasesModel.Result> GetActiveActivities(int maximumRows, int startRowIndex)
        {
            return Model.GetActiveActivities(maximumRows, startRowIndex);
        }

        public int GetActivityCount()
        {
            return Model.GetActivityCount();
        }
    }
}