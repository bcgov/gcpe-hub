extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.News
{
    using legacy::Gcpe.Hub.Data.Entity;

    public class SearchDataSource
    {
        public SearchModel Model { get; set; }

        public IEnumerable<SearchModel.SearchResult> GetResults(int maximumRows, int startRowIndex)
        {
            return Model.GetResults(maximumRows, startRowIndex);
        }

        public int GetResultsCount()
        {
            return Model.CountResults();
        }
    }
}