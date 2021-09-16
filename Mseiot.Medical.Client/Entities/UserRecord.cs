using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Client.Entities
{
    public class UserRecord
    {
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
        public long LoginTime { get; set; }
    }
}
