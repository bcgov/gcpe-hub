using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.News
{
    public class HubModelException : ApplicationException
    {
        public IEnumerable<string> Errors { get; private set; }

        public HubModelException(IEnumerable<string> errors)
        {
            List<string> Errors = new List<string>();
            Errors.AddRange(errors);
            this.Errors = Errors;
        }
    }
}