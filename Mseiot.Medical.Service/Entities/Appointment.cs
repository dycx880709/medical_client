using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// 身份证
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }

        /// <summary>
        /// 预约状态
        /// </summary>
        public AppointmentStatus AppointmentStatus { get; set; }
    }

    public enum AppointmentStatus
    {
        [Description("已预约")]
        Reserved,
        [Description("已报到")]
        Report,
        [Description("已检查")]
        Check
    }
}
