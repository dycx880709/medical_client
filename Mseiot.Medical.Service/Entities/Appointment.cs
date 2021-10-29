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
        public string IDCard { get; set; }
        /// <summary>
        /// 社保账号
        /// </summary>
        public string SocialSecurityCode { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public long Birthday { get; set; }
        /// <summary>
        /// 预约状态
        /// </summary>
        public AppointmentStatus AppointmentStatus { get; set; }
        /// <summary>
        /// 检查信息
        /// </summary>
        public Examination Examination { get; set; }
    }

    public enum AppointmentStatus
    {
        [Description("已预约")]
        Reserved,
        [Description("已签到")]
        PunchIn,
        [Description("候诊中")]
        Waiting,
        [Description("检查中")]
        Checking,
        [Description("已检查")]
        Checked,
        [Description("已报告")]
        Reported,
        [Description("已取消")]
        Cancel,
        [Description("已过期")]
        Exprire
    }
}
