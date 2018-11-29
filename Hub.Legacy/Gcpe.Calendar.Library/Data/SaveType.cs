using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorporateCalendar.Data
{
    public class SaveType : Enumeration
    {
        public static readonly SaveType Insert = new SaveType(0, "Insert");
        public static readonly SaveType Update = new SaveType(1, "Update");

        private SaveType() { }
        private SaveType(int value, string displayName) : base(value, displayName) { }
    }
}
