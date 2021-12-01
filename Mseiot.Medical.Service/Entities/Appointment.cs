using Ms.Libs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class Appointment : NotifyPropertyBase
    {
        private string name;
        private long appointmentTime;
        private string idCard;
        private string socialSecurityCode;
        private bool sex;
        private int birthday;
        private AppointmentStatus appointmentStatus;
        private Examination examination;
        private int endoscopeID;
        private string appointmentType;
        private string consultingRoomName;
        private string telephone;
        private string anesthesia;
        private string patientSource;
        private string hospitalID;
        private string roomID;
        private string department;
        private bool isPrior;

        /// <summary>
        /// ID
        /// </summary>
        public int AppointmentID { get; set; }
        /// <summary>
        /// 预约编号
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex
        {
            get { return sex; }
            set
            {
                sex = value;
                RaisePropertyChanged("Sex");
            }
        }

        /// <summary>
        /// 社保账号
        /// </summary>
        public string SocialSecurityCode
        {
            get { return socialSecurityCode; }
            set
            {
                socialSecurityCode = value;
                RaisePropertyChanged("SocialSecurityCode");
            }
        }
        /// <summary>
        /// 预约时间
        /// </summary>
        public long AppointmentTime
        {
            get { return appointmentTime; }
            set
            {
                appointmentTime = value;
                RaisePropertyChanged("AppointmentTime");
            }
        }
        /// <summary>
        /// 病人姓名
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard
        {
            get { return idCard; }
            set
            {
                idCard = value;
                RaisePropertyChanged("IDCard");
            }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Birthday
        {
            get { return birthday; }
            set
            {
                birthday = value;
                RaisePropertyChanged("Birthday");
            }
        }
        /// <summary>
        /// 内镜编号
        /// </summary>
        public int EndoscopeID
        {
            get { return endoscopeID; }
            set
            {
                endoscopeID = value;
                RaisePropertyChanged("EndoscopeID");
            }
        }
        /// <summary>
        /// 预约状态
        /// </summary>
        public AppointmentStatus AppointmentStatus
        {
            get { return appointmentStatus; }
            set
            {
                appointmentStatus = value;
                RaisePropertyChanged("AppointmentStatus");
            }
        }
        /// <summary>
        /// 检查类型
        /// </summary>
        public string AppointmentType
        {
            get { return appointmentType; }
            set
            {
                appointmentType = value;
                RaisePropertyChanged("AppointmentType");
            }
        }
        /// <summary>
        /// 检查信息
        /// </summary>
        [JsonIgnore]
        public Examination Examination
        {
            get { return examination; }
            set
            {
                examination = value;
                RaisePropertyChanged("Examination");
            }
        }
        /// <summary>
        /// 预约诊室
        /// </summary>
        public string ConsultingRoomName
        {
            get { return consultingRoomName; }
            set
            { 
                consultingRoomName = value;
                RaisePropertyChanged("ConsultingRoomName");
            }
        }
        /// <summary>
        /// 科室
        /// </summary>
        public string Department
        {
            get { return department; }
            set
            {
                department = value;
                RaisePropertyChanged("Department");
            }
        }
        /// <summary>
        /// 床号
        /// </summary>
        public string RoomID
        {
            get { return roomID; }
            set
            {
                roomID = value;
                RaisePropertyChanged("RoomID");
            }
        }
        /// <summary>
        /// 住院号
        /// </summary>
        public string HospitalID
        {
            get { return hospitalID; }
            set
            {
                hospitalID = value;
                RaisePropertyChanged("HospitalID");
            }
        }
        /// <summary>
        /// 患者来源
        /// </summary>
        public string PatientSource
        {
            get { return patientSource; }
            set
            {
                patientSource = value;
                RaisePropertyChanged("PatientSource");
            }
        }
        /// <summary>
        /// 麻醉方法
        /// </summary>
        public string Anesthesia
        {
            get { return anesthesia; }
            set
            {
                anesthesia = value;
                RaisePropertyChanged("Anesthesia");
            }
        }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Telephone
        {
            get { return telephone; }
            set
            {
                telephone = value;
                RaisePropertyChanged("Telephone");
            }
        }
        /// <summary>
        /// 是否优先
        /// </summary>
        public bool IsPrior
        {
            get { return isPrior; }
            set
            {
                isPrior = value;
                RaisePropertyChanged("IsPrior");
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateTime { get; set; }
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
        Exprire,
        [Description("已过号")]
        Cross
    }
}
