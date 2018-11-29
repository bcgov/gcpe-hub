using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.News
{
    public class CategoryItem<T> : ListItem<T>
    {
        public Guid? TopReleaseId { get; set; }

        public Guid? FeatureReleaseId { get; set; }
    }
}