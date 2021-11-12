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
                new AppLevel
                {
                    Name = "内镜预约登记系统", Level = "1",
                    Children = new List<AppLevel>
                    {
                        new AppLevel { Name = "预约登记", Level="11", Identify="MM.Medical.Client.Views.AppointmentManage"}
                    }
                },
                new AppLevel
                {
                    Name = "内镜消洗追溯系统", Level = "2", Describe = "清洗中心模块",
                    Children = new List<AppLevel>
                    {
                        new AppLevel { Name="消洗中心", Level = "21" },
                        new AppLevel { Name="内镜管理", Level = "22" },
                        new AppLevel { Name="读卡器", Level = "23" },
                        new AppLevel { Name="流程管理", Level = "24" },
                        new AppLevel { Name="清洗记录", Level = "25" },
                        new AppLevel { Name="清洗酶", Level = "26" }
                    }
                },
                new AppLevel 
                { 
                    Name = "医学影像图文系统", Level = "3",
                    Children = new List<AppLevel>()
                    {
                        new AppLevel { Name="检查中心", Level = "31", Identify="MM.Medical.Client.Views.ExaminationManageView" },
                        new AppLevel { Name="病历中心", Level = "32", Identify="MM.Medical.Client.Views.MedicalManageView" },
                        new AppLevel
                        {
                            Name="数据管理", Level = "33",
                            Children = new List<AppLevel>()
                            {
                                new AppLevel { Name="我的工作台", Level="331", Identify="MM.Medical.Client.Views.SelfWorkStatisticsView" },
                                new AppLevel { Name="专项统计", Level="332", Identify="MM.Medical.Client.Views.SpecialStatisticsView" }
                            }
                        },
                        new AppLevel 
                        {
                            Name = "系统管理", Level = "34",
                            Children= new List<AppLevel>
                            { 
                                new AppLevel { Name="用户管理", Level="341", Identify="MM.Medical.Client.Views.UserManageView" },
                                new AppLevel { Name="角色管理", Level="342", Identify="MM.Medical.Client.Views.RoleManageView" },
                                new AppLevel { Name="系统设置", Level="343", Identify="MM.Medical.Client.Views.SystemSettingView" },
                                new AppLevel { Name="基础词库", Level="344", Identify="MM.Medical.Client.Views.BaseWordView" },
                                new AppLevel { Name="诊断模板", Level="345", Identify="MM.Medical.Client.Views.DiagnosticTemplateView" },
                                new AppLevel { Name="医学词库", Level="346", Identify="MM.Medical.Client.Views.MedicalWordView" },
                                new AppLevel { Name="诊室管理", Level="347", Identify="MM.Medical.Client.Views.ConsultingManageView" },
                                new AppLevel { Name="数据备份", Level="348", Identify="MM.Medical.Client.Views.DataBackingView" },
                            }
                        }
                    }
                }
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
        public string Identify { get; set; }
        /// <summary>
        /// 子集
        /// </summary>
        public List<AppLevel> Children { get; set; }
    }
}
