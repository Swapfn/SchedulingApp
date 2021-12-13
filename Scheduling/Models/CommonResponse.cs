using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Models
{
    public class CommonResponse<T>
    {
        // request status
        public int status { get; set; }

        // error message
        public string message { get; set; }

        //actual data
        public T dataenum { get; set; }
    }
}
