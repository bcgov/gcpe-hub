using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gcpe.Hub.Data.Entity
{
    partial class HubDbContext
    {
        //This constructor is required for .NET Core dependency injection
        public HubDbContext(DbContextOptions<HubDbContext> options) : base(options)
        {
        }
    }
}