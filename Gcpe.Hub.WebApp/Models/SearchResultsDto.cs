using System.Collections.Generic;

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
}