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
                        new AppLevel { Name = "预约登记", Level="11", Identify="MM.Medical.Client.Views.AppointmentManage", Reusability=true }
                    }
                },
                new AppLevel
                {
                    Name = "内镜消洗追溯系统", Level = "2", Describe = "清洗中心模块",
                    Children = new List<AppLevel>
                    {
                        new AppLevel { Name="消洗中心", Level = "21", Identify="MM.Medical.Client.Module.Decontaminate.DecontaminateTaskView" },
                        new AppLevel { Name="内镜管理", Level = "22", Identify="MM.Medical.Client.Module.Decontaminate.EndoscopeManage" },
                        new AppLevel { Name="采集设备", Level = "23", Identify="MM.Medical.Client.Module.Decontaminate.RFIDDeviceManage" },
                        new AppLevel { Name="流程管理", Level = "24", Identify="MM.Medical.Client.Module.Decontaminate.DecontaminateFlowView" },
                        new AppLevel { Name="清洗记录", Level = "25", Identify="MM.Medical.Client.Module.Decontaminate.DecontaminateTaskManage", Reusability=true },
                        new AppLevel { Name="清洗酶", Level = "26", Identify="MM.Medical.Client.Module.Decontaminate.EnzymeManage" }
                    }
                },
                new AppLevel 
                { 
                    Name = "医学影像图文系统", Level = "3",
                    Children = new List<AppLevel>()
                    {
                        new AppLevel { Name="检查中心", Level = "31", Identify="MM.Medical.Client.Views.ExaminationManageView", Reusability=true },
                        new AppLevel { Name="病历中心", Level = "32", Identify="MM.Medical.Client.Views.MedicalManageView" },
                        new AppLevel { Name="我的工作台", Level="33", Identify="MM.Medical.Client.Views.SelfWorkStatisticsView" },
                        new AppLevel { Name="主任管理", Level="34", Identify="MM.Medical.Client.Views.SpecialStatisticsView" },
                        new AppLevel
                        {
                            Name = "系统管理", Level = "35",
                            Children= new List<AppLevel>
                            { 
                                new AppLevel { Name="用户管理", Level="351", Identify="MM.Medical.Client.Views.UserManageView" },
                                new AppLevel { Name="角色管理", Level="352", Identify="MM.Medical.Client.Views.RoleManageView" },
                                new AppLevel { Name="系统设置", Level="353", Identify="MM.Medical.Client.Views.SystemSettingView" },
                                new AppLevel { Name="基础词库", Level="354", Identify="MM.Medical.Client.Views.BaseWordView" },
                                new AppLevel { Name="诊断模板", Level="355", Identify="MM.Medical.Client.Views.DiagnosticTemplateView" },
                                new AppLevel { Name="医学词库", Level="356", Identify="MM.Medical.Client.Views.MedicalWordView" },
                                new AppLevel { Name="诊室管理", Level="357", Identify="MM.Medical.Client.Views.ConsultingManageView" },
                                new AppLevel { Name="数据备份", Level="358", Identify="MM.Medical.Client.Views.BackupDBManage" },
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
        /// <summary>
        /// 是否复用
        /// </summary>
        public bool Reusability { get; set; }
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
