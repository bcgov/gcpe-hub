using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.News
{
    public class ListItem<T>
    {
        public bool Selected { get; set; }

        public string Text { get; set; }

        public T Value { get; set; }
    }
}