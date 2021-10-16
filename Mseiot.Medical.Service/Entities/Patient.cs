using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class Patient : NotifyPropertyBase
    {
        private string name;
        private string idCard;
        private string socialSecurityCode;
        private string telphoneNumber;
        private string marryType;
        private string occupation;
        private int childrenCount;
        private string nation;
        private int height;
        private int weight;
        private string address;
        private string permanent;
        private string patientNumber;
        private string sex;
        private int age;

        #region 病人信息
        public int PatientID { get; set; }
        /// <summary>
        /// 姓名
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
        /// 年龄
        /// </summary>
        public int Age
        {
            get { return age; }
            set
            {
                age = value;
                RaisePropertyChanged("Age");
            }
        }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex
        {
            get { return sex; }
            set
            {
                sex = value;
                RaisePropertyChanged("Sex");
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// 病人编号
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
        public string MarryType
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
        #endregion
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

    public enum MarryType
    {
        [Description("未婚")]
        Unmarried,
        [Description("已婚")]
        Married,
        [Description("离异")]
        Divorced
    }
}
