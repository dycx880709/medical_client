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
        private bool sex;
        private string idCard;
        private string patientNumber;
        private string socialSecurityCode;
        private string telphone;
        private string marryType;
        private string occupation;
        private int childrenCount;
        private string nation;
        private int height;
        private int weight;
        private string address;
        private int born;


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
        public string Telephone
        {
            get { return telphone; }
            set
            {
                telphone = value;
                RaisePropertyChanged("Telephone");
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
        public string IDCard
        {
            get { return idCard; }
            set
            {
                idCard = value;
                RaisePropertyChanged("IDCard");
            }
        }
        #endregion
    }
}
