using MM.Libs.RFID;
using Mseiot.Medical.Service.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Medical.Client.Entities
{
    public class RFIDExProxy : RFIDProxy
    {
        public string Com { get; set; }
    }
}
