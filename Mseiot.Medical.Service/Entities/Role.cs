using Ms.Libs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class Role : NotifyPropertyBase
    {
        private string name;

        public int RoleID { get; set; }

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
        /// 是否可用编辑
        /// </summary>
        public bool Editable { get; set; } = true;
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateTime { get; set; }
        public string Authority { get; set; }
    }
}
