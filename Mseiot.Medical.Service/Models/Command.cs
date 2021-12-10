using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Models
{
    public partial class Command : Ms.Libs.TcpLib.CommandBase
    {
        public const byte ConsultingRoom_Login = 4;
        public const byte System_ForceTaskOffline = 5;

        public const byte Module_Appointment = 1;
        public const byte Allot_Appointment = 0;
        public const byte Change_Appointment = 1;

        public const byte Module_Version = 100;
        public const byte New_Version = 0;
	}
}
