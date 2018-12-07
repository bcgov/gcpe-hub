using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.Hub.Data.Configuration
{

    public static class App
    {
        readonly static IConnectionConfiguration instance = new ConnectionConfiguration(new RES.Configuration.Configuration());
        public static IConnectionConfiguration Settings => instance;
    }

    public class ConnectionConfiguration: IConnectionConfiguration
    {
        readonly RES.Configuration.Configuration configuration;

        public ConnectionConfiguration(RES.Configuration.Configuration configuration)
        {
            Contract.Requires(configuration != null);
            this.configuration = configuration;

            Validate();
        }

        void Validate()
        {
            using (var validator = configuration.CreateValidator)
            {
                validator.Check(() => DbServer);
                validator.Check(() => DbName);
            }
        }

        public string DbServer => configuration.GetString(MethodBase.GetCurrentMethod());
        public string DbName => configuration.GetString(MethodBase.GetCurrentMethod());
    }
}
