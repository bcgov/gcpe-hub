using System;
using System.Collections.Generic;
using MediaRelationsDatabase;

namespace MediaRelationsLibrary
{
    public class EmailAddressInstance
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
    public class WebAddressDisplay
    {
        public Guid Id { get; set; }
        public bool IsNew { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsModified { get; set; }
        public Guid WebAddressTypeId { get; set; }
        public string WebAddressTypeName { get; set; }
        public string WebAddress { get; private set; }
        public string NewWebAddress { get; private set; }
        public string[] MediaDistributionLists { get; set; }
        public bool CanBeDeleted(bool isAdmin) { return isAdmin || MediaDistributionLists == null; }
        private int _omsCount = 0;
        public int OriginalMediaSubscriptionCount
        {
            get
            {
                return this._omsCount;
            }
            set
            {
                this._omsCount = value;
            }
        }

        public string MediaDistributions
        {
            get
            {
                string ret = "";
                if (this.MediaDistributionLists != null)
                {
                    ret = string.Join(", <br>", this.MediaDistributionLists);
                }
                return ret;

            }
        }

        public IList<EmailAddressInstance> EmailAddressInfo { get; set; }

        public WebAddressDisplay(Guid id, string webAddress = null)
        {
            this.Id = id;
            if (webAddress == null)
            {
                IsNew = true;
            }
            else
            {
                WebAddress = webAddress;
                NewWebAddress = webAddress;
            }
            this.IsDeleted = false;
            this.IsModified = false;
        }

        public void Update(string newWebAddress)
        {
            if (NewWebAddress == newWebAddress) return;
            NewWebAddress = newWebAddress;
            IsModified = true;
        }

        public void Update(List<string> newMediaDistributionLists)
        {
            MediaDistributionLists = newMediaDistributionLists.Count != 0 ? newMediaDistributionLists.ToArray() : null;
            IsModified = true;
        }
    }
}
