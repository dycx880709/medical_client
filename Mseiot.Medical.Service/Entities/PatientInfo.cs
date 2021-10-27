using Ms.Libs.Models;
using Ms.Libs.SysLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class PatientInfo : NotifyPropertyBase
    {
        private string patientCode;
        private string hospitalizationCode;
        private string className;
        private string roomCode;
        private string areaCode;
        private string doctorName;
        private int? checkTime;
        private string chargeType;
        private string checkType;
        private string outpatientCode;
        private string pathologyCode;
        private string patientHistory;
        private string patientTalk;
        private string diagnoseType;
        private int? createTime;
        private PatientStatus patientStatus;
        private string consultingRoom;
        
        /// <summary>
        /// 病患预约ID
        /// </summary>
        public int PatientInfoID { get; set; }
        /// <summary>
        /// 病患人员ID
        /// </summary>
        public int PatientID { get; set; }
        public Patient Patient { get; set; } = new Patient();
        /// <summary>
        /// 检查数据 
        /// </summary>
        public CheckInfo CheckInfo { get; set; } = new CheckInfo();
        #region 预约信息
        /// <summary>
        /// 检查诊室
        /// </summary>
        public string ConsultingRoom
        {
            get { return consultingRoom; }
            set
            {
                consultingRoom = value;
                RaisePropertyChanged("ConsultingRoom");
            }
        }
        /// <summary>
        /// 登记时间
        /// </summary>
        public int? CreateTime
        {
            get { return createTime; }
            set
            {
                createTime = value;
                RaisePropertyChanged("CreateTime");
            }
        }
        /// <summary>
        /// 诊断类型 初/复诊
        /// </summary>
        public string DiagnoseType
        {
            get { return diagnoseType; }
            set
            {
                diagnoseType = value;
                RaisePropertyChanged("DiagnoseType");
            }
        }
        /// <summary>
        /// 主诉
        /// </summary>
        public string PatientTalk
        {
            get { return patientTalk; }
            set
            {
                patientTalk = value;
                RaisePropertyChanged("PatientTalk");
            }
        }
        /// <summary>
        /// 病史
        /// </summary>
        public string PatientHistory
        {
            get { return patientHistory; }
            set
            {
                patientHistory = value;
                RaisePropertyChanged("PatientHistory");
            }
        }
        /// <summary>
        /// 病理号
        /// </summary>
        public string PathologyCode
        {
            get { return pathologyCode; }
            set
            {
                pathologyCode = value;
                RaisePropertyChanged("PathologyCode");
            }
        }
        /// <summary>
        /// 门诊号
        /// </summary>
        public string OutpatientCode
        {
            get { return outpatientCode; }
            set
            {
                outpatientCode = value;
                RaisePropertyChanged("OutpatientCode");
            }
        }
        /// <summary>
        /// 检查类型
        /// </summary>
        public string CheckType
        {
            get { return checkType; }
            set
            {
                checkType = value;
                RaisePropertyChanged("CheckType");
            }
        }
        /// <summary>
        /// 收费类型
        /// </summary>
        public string ChargeType
        {
            get { return chargeType; }
            set
            {
                chargeType = value;
                RaisePropertyChanged("ChargeType");
            }
        }
        /// <summary>
        /// 检查日期
        /// </summary>
        public int? CheckTime
        {
            get { return checkTime; }
            set
            {
                checkTime = value;
                RaisePropertyChanged("CheckTime");
            }
        }
        /// <summary>
        /// 送检医生
        /// </summary>
        public string DoctorName
        {
            get { return doctorName; }
            set
            {
                doctorName = value;
                RaisePropertyChanged("DoctorName");
            }
        }
        /// <summary>
        /// 病区号
        /// </summary>
        public string AreaCode
        {
            get { return areaCode; }
            set
            {
                areaCode = value;
                RaisePropertyChanged("AreaCode");
            }
        }
        /// <summary>
        /// 床位号
        /// </summary>
        public string RoomCode
        {
            get { return roomCode; }
            set
            {
                roomCode = value;
                RaisePropertyChanged("RoomCode");
            }
        }
        /// <summary>
        /// 送检科室
        /// </summary>
        public string ClassName
        {
            get { return className; }
            set
            {
                className = value;
                RaisePropertyChanged("ClassName");
            }
        }
        /// <summary>
        /// 住院号 
        /// </summary>
        public string HospitalizationCode
        {
            get { return hospitalizationCode; }
            set
            {
                hospitalizationCode = value;
                RaisePropertyChanged("HospitalizationCode");
            }
        }
        /// <summary>
        /// 病历编号
        /// </summary>
        public string PatientCode
        {
            get { return patientCode; }
            set
            {
                patientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }      
        /// <summary>
        /// 签到状态
        /// </summary>
        public PatientStatus PatientStatus
        {
            get { return patientStatus; }
            set
            {
                patientStatus = value;
                RaisePropertyChanged("PatientStatus");
            }
        }

        #endregion
    }


    public enum DiagnoseType
    {
        [Description("初诊")]
        PrimeraConsulta,
        [Description("复诊")]
        NuevaConsulta
    }

    public enum PatientStatus
    {
        [Description("未签到")]
        UnRegist,
        [Description("已签到")]
        Regist,
        [Description("检查中")]
        Checking,
        [Description("已检查")]
        Checked
    }
}
