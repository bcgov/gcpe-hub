using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.News
{
    public class NavigationItem
    {
        public string Text { get; set; }
        public string NavigateUrl { get; set; }
        public bool IsActive { get; set; }
    }
}