using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRelationsDatabase.Configuration
{
    public interface IConnectionConfiguration
    {
        String DbServer { get; }
        String DbName { get; }

    }
}
