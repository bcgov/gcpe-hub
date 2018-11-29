using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaRelationsDatabase;

namespace MediaRelationsLibrary
{
    public class SearchResult
    {
        public Guid ItemId;
        public SearchLib.EntityType ItemType;
        public string ItemName;
        public DateTime CreationDate;
        public DateTime ModifiedDate;
        public Contact OriginalContact;
        public Company OriginalCompany;
    }
}
