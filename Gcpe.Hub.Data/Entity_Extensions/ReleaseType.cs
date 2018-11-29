using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gcpe.Hub.Data.Entity
{   
    public enum ReleaseType : int
    {
        Release = 1,
        Story = 2,
        Factsheet = 3,
        Update = 4, 
        Advisory = 5
    }
}
