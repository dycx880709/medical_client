using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Client.Entities
{
    public class PageLevel : NotifyPropertyBase
    {
        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }
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
        public List<PageLevel> Children { get; set; }

        public static List<PageLevel> PageLevels
        {
            get 
            {
                return new List<PageLevel>()
                {
                    new PageLevel { Name = "预约登记", Level = 1 },
                    new PageLevel { Name = "检查中心", Level = 2 },
                    new PageLevel { Name = "病例中心", Level = 3 },
                    new PageLevel { Name = "清洗中心", Level = 4 },
                    new PageLevel { Name = "主任浏览", Level = 5 }
                };
            }
        }
    }
}
