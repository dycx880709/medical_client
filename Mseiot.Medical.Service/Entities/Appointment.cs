using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class Appointment
    {
        /// <summary>
        /// 预约ID
        /// </summary>
        public int AppointmentID { get; set; }

        /// <summary>
        /// 预约编号
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 病人姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 预约时间
        /// </summary>
        public long AppointmentTime { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public long Birthday { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }
    }
}
