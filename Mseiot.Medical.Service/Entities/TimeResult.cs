using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class TimeResult
    {
        public int TimeStamp { get; set; }
        public int Count { get; set; }
    }

    public class TimeResultCollection : List<TimeResult>
    { 
    
    }
}
