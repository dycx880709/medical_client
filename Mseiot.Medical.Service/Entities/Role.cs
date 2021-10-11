using Ms.Libs.Models;
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

        public string Authority { get; set; }

        public int[] Authoritys
        {
            get { return Authority.Split(',').Select(p => Convert.ToInt32(p)).ToArray(); }
            set { this.Authority = string.Join(",", value); }
        }
    }

}
