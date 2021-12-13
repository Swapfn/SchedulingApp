using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.DbInitializer
{
    public interface IDbInitializer
    {
        public void initialize();
    }
}
