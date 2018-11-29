using System;

namespace Gcpe.Hub.Data.Entity
{
    [Flags]
    public enum PublishOptions : int
    {
        PublishNewsArchives = 1,
        PublishNewsOnDemand = 2,
        PublishMediaContacts = 4
    }
}
