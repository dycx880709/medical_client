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
        private int patientID;
        private string patientNumber;
        private string patientName;
        private Sex sex;
        private AgeRange ageRange;
        private string checkBody;
        private string patientCode;
        private string hospitalizationCode;
        private string className;
        private string roomCode;
        private string areaCode;
        private string doctorName;
        private long checkDate = TimeHelper.ToUnixDate(DateTime.Now);
        private ChargeType chargeType;
        private string checkType;
        private string outpatientCode;
        private string pathologyCode;
        private string patientHistory;
        private string patientTalk;
        private DiagnoseType diagnoseType;
        private long createDate = TimeHelper.ToUnixDate(DateTime.Now);

        private string idCard;
        private string socialSecurityCode;
        private string telphoneNumber;
        private MarryType marryType;
        private string occupation;
        private int childrenCount;
        private string nation;
        private int height;
        private int weight;
        private string address;
        private string permanent;
        
        /// <summary>
        /// 户籍地址
        /// </summary>
        public string Permanent
        {
            get { return permanent; }
            set
            {
                permanent = value;
                RaisePropertyChanged("Permanent");
            }
        }

        /// <summary>
        /// 居住地址
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
        /// 体重
        /// </summary>
        public int Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                RaisePropertyChanged("Weight");
            }
        }

        /// <summary>
        /// 身高
        /// </summary>
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                RaisePropertyChanged("Height");
            }
        }

        /// <summary>
        /// 民族 
        /// </summary>
        public string Nation
        {
            get { return nation; }
            set
            {
                nation = value;
                RaisePropertyChanged("Nation");
            }
        }

        /// <summary>
        /// 子女数
        /// </summary>
        public int ChildrenCount
        {
            get { return childrenCount; }
            set
            {
                childrenCount = value;
                RaisePropertyChanged("ChildrenCount");
            }
        }

        /// <summary>
        /// 职业
        /// </summary>
        public string Occupation
        {
            get { return occupation; }
            set
            {
                occupation = value;
                RaisePropertyChanged("Occupation");
            }
        }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public MarryType MarryType
        {
            get { return marryType; }
            set
            {
                marryType = value;
                RaisePropertyChanged("MarryType");
            }
        }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string TelphoneNumber
        {
            get { return telphoneNumber; }
            set
            {
                telphoneNumber = value;
                RaisePropertyChanged("TelphoneNumber");
            }
        }

        /// <summary>
        /// 社保号码
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
        /// 身份证号
        /// </summary>
        public string IdCard
        {
            get { return idCard; }
            set
            {
                idCard = value;
                RaisePropertyChanged("IdCard");
            }
        }

        /// <summary>
        /// 登记时间
        /// </summary>
        public long CreateDate
        {
            get { return createDate; }
            set
            {
                createDate = value;
                RaisePropertyChanged("CreateDate");
            }
        }

        /// <summary>
        /// 诊断类型 初/复诊
        /// </summary>
        public DiagnoseType DiagnoseType
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
        public ChargeType ChargeType
        {
            get { return chargeType; }
            set
            {
                chargeType = value;
                RaisePropertyChanged("ChargeType");
            }
        }
        /// <summary>
        /// 检查时间
        /// </summary>
        public long CheckDate
        {
            get { return checkDate; }
            set
            {
                checkDate = value;
                RaisePropertyChanged("CheckDate");
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
        /// 检查部位
        /// </summary>
        public string CheckBody
        {
            get { return checkBody; }
            set
            {
                checkBody = value;
                RaisePropertyChanged("CheckBody");
            }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public AgeRange AgeRange
        {
            get { return ageRange; }
            set
            {
                ageRange = value;
                RaisePropertyChanged("AgeRange");
            }
        }
        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex
        {
            get { return sex; }
            set
            {
                sex = value;
                RaisePropertyChanged("Sex");
            }
        }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName
        {
            get { return patientName; }
            set
            {
                patientName = value;
                RaisePropertyChanged("PatientName");
            }
        }
        /// <summary>
        /// 预约序号
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
        /// 病患预约ID
        /// </summary>
        public int PatientID
        {
            get { return patientID; }
            set
            {
                patientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
    }

    public enum MarryType
    {
        [Description("未婚")]
        Unmarried,
        [Description("已婚")]
        Married,
        [Description("离异")]
        Divorced
    }

    public enum Sex
    { 
        [Description("男")]
        Male,
        [Description("女")]
        Female,
        [Description("未知")]
        Noknown
    }

    public enum AgeRange
    { 
        [Description("小孩")]
        Child,
        [Description("少年")]
        Juvenile,
        [Description("成年人")]
        Adult,
        [Description("中年人")]
        MiddleAger,
        [Description("老人")]
        Older
    }

    public enum DiagnoseType
    {
        [Description("初诊")]
        PrimeraConsulta,
        [Description("复诊")]
        NuevaConsulta
    }

    public enum ChargeType
    {
        [Description("自费")]
        SelfPay,
        [Description("医保")]
        Insurance
    }
}
