using System.Collections.Generic;
using System;

namespace Gcpe.Hub.Website.Models
{
    public class FilterDto
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public bool isChecked { get; set; }
    }
    public class FacetDto
    {
        public string Name { get; set; }
        public IEnumerable<FilterDto> Filters { get; set; }
    }
    public class SearchResultsDto
    {
        public IEnumerable<MediaRequestDto> MediaRequests { get; set; }
        public IEnumerable<FacetDto> Facets { get; set; }
    }
    
    public class SearchQueryDto
    {
        public static DateTime MinDate = DateTime.Parse("2011/03/12");
        public SearchQueryDto(string text, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Text = text;
            FromDate = fromDate ?? MinDate;
            ToDate = toDate ?? DateTime.Today;
        }

        public string Text { get; }
        public DateTime ToDate { get; }
        public DateTime FromDate { get; }
        public bool UseCustomRange()
        {
            return FromDate != MinDate || ToDate != DateTime.Today;
        }

    }
}