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
        public int ConsultingRoomID { get; set; }
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
