using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Medical.Management.Entities
{
    public class AppLevel : NotifyPropertyBase
    {
        /// <summary>
        /// 层级
        /// </summary>
        public string Level { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }

        private bool isSelected;
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        /// <summary>
        /// 子集
        /// </summary>
        public List<AppLevel> Children { get; set; }
    }
}
