using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gcpe.Hub.Data.Entity
{
    public interface IHubCategory
    {
        string DisplayName { get; set; }
        System.DateTime Timestamp { get; set; }
        string MiscHtml { get; set; }
        string MiscRightHtml { get; set; }
        string TwitterUsername { get; set; }
        string FlickrUrl { get; set; }
        string YoutubeUrl { get; set; }
        string AudioUrl { get; set; }
        string FacebookEmbedHtml { get; set; }
        string YoutubeEmbedHtml { get; set; }
        string AudioEmbedHtml { get; set; }
        NewsRelease TopRelease { get; set; }
        NewsRelease FeatureRelease { get; set; }
        bool IsActive { get; set; }
    }

    public partial class Ministry : IHubCategory
    {
    }

    public partial class Sector : IHubCategory
    {
    }
}
