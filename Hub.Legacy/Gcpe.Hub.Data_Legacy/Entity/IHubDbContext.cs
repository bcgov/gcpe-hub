using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.Data.Entity
{
    public interface IHubDbContext : IHubEntities
    {
        int SaveChanges();
        void Dispose();
    }
}
