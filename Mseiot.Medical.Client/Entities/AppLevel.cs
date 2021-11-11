using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Medical.Client.Entities
{
    public class AppLevel : NotifyPropertyBase
    {
        static AppLevel()
        {
            Levels = new List<AppLevel>()
            {
                new AppLevel { Name = "内镜综合管理系统", Level = "0" },
                new AppLevel { Name = "内镜预约登记系统", Level = "1" },
                new AppLevel { Name = "内镜消洗追溯系统", Level = "2" },
                new AppLevel { Name = "内镜数据分析系统", Level = "3" },
            };
        }

        public static List<AppLevel> Levels { get; }

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
