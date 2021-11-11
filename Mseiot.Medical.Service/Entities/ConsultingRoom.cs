using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class ConsultingRoom : NotifyPropertyBase
    {
        private string name;
        private bool isUsed;
        private string userName;
        private int consultingRoomID;
        private bool isSelected;
        private string checkType;

        public string CheckType
        {
            get { return checkType; }
            set
            {
                checkType = value;
                RaisePropertyChanged("CheckType");
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public int ConsultingRoomID
        {
            get { return consultingRoomID; }
            set
            {
                consultingRoomID = value;
                RaisePropertyChanged("ConsultingRoomID");
            }
        }

        public string UserID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUsed
        {
            get { return isUsed; }
            set
            {
                isUsed = value;
                RaisePropertyChanged("IsUsed");
            }
        }
        /// <summary>
        /// 诊室名称
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
        /// 使用人
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                RaisePropertyChanged("UserName");
            }
        }
    }
}
