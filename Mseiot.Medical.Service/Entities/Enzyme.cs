using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    /// <summary>
    /// 清洗酶
    /// </summary>
    public class Enzyme:BaseModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int EnzymeID { get; set; }

        private bool enabled;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled 
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    NotifyPropertyChanged("Enabled");
                }
            }
        }

        public int Timeout { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
}
