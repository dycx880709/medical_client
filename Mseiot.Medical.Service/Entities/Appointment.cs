﻿using Ms.Libs.Models;
using Ms.Libs.SysLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private AppointmentStatus appointmentStatus;
        private Examination examination;
        private int endoscopeID;
        private string[] appointmentType;
        private string consultingRoomName;
        private string telephone;
        private string anesthesia;
        private string patientSource;
        private string hospitalID;
        private string roomID;
        private string department;
        private bool isPrior;
        private string watchInfo;
        private int backingTime;
        private int born = (int)TimeHelper.ToUnixTime(new DateTime(2000, 1, 1));
        private string backReason;
        private string patientNumber;
        private string clinicCode;
        private string applyDoctor;
        private string applyDepartment;
        private string address;
        public int PatientID { get; set; }
        /// <summary>
        /// 家庭地址
        /// </summary>
        public string Address
        {
            get { return address; }
            set
            { 
                address = value;
                RaisePropertyChanged("Address");
            }
        }

        /// <summary>
        /// 申请科室
        /// </summary>
        public string ApplyDepartment
        {
            get { return applyDepartment; }
            set 
            { 
                applyDepartment = value;
                RaisePropertyChanged("ApplyDepartment");
            }
        }
        /// <summary>
        /// 申请医生
        /// </summary>
        public string ApplyDoctor
        {
            get { return applyDoctor; }
            set
            { 
                applyDoctor = value;
                RaisePropertyChanged("ApplyDoctor");
            }
        }
        /// <summary>
        /// 门诊号
        /// </summary>
        public string ClinicCode
        {
            get { return clinicCode; }
            set
            { 
                clinicCode = value;
                RaisePropertyChanged("ClinicCode");
            }
        }
        /// <summary>
        /// 病人ID
        /// </summary>
        public string PatientNumber
        {
            get { return patientNumber; }
            set 
            { 
                patientNumber = value; 
                RaisePropertyChanged("PatientNumber"); 
            }
        }
        /// <summary>
        /// 回访原因
        /// </summary>
        public string BackReason
        {
            get { return backReason; }
            set 
            { 
                backReason = value; 
                RaisePropertyChanged("BackReason");
            }
        }
        /// <summary>
        /// 签到时间
        /// </summary>
        public int PunchinTime { get; set; }
        public int Born
        {
            get { return born; }
            set
            { 
                born = value;
                RaisePropertyChanged("Born");
            }
        }
        /// <summary>
        /// 回访时期
        /// </summary>
        public int BackingTime
        {
            get { return backingTime; }
            set 
            { 
                backingTime = value;
                RaisePropertyChanged("BackingTime");
            }
        }
        /// <summary>
        /// 关注信息
        /// </summary>
        public string WatchInfo
        {
            get { return watchInfo; }
            set 
            {
                watchInfo = value;
                RaisePropertyChanged("WatchInfo");
            }
        }
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

        public string AppointmentTypeStr
        {
            get { return AppointmentType != null ? string.Join(",", AppointmentType) : string.Empty; }
            set { AppointmentType = value.Split(',');  }
        }
        /// <summary>
        /// 检查类型
        /// </summary>
        public string[] AppointmentType
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
        public ObservableCollection<Examination> Examinations { get; set; }
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
