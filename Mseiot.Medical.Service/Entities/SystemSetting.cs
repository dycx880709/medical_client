using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class SystemSetting : NotifyPropertyBase
    {
        private string hospitalName;
        private string className;
        private string reportName;
        private string reportIcon;
        private string orderPrefix;
        private int orderLength;
        private string cutshotSound;
        private int cutshotImageCount;
        private int printImageCount;
        private int mediaCount;
        private string wakeupCallName;
        private string cutshotKeyboard;
        private string recordKeyboard;

        /// <summary>
        /// 采图热键
        /// </summary>
        public string CutshotKeyboard
        {
            get { return cutshotKeyboard; }
            set
            {
                cutshotKeyboard = value;
                RaisePropertyChanged("CutshotKeyboard");
            }
        }

        public string RecordKeyboard
        {
            get { return recordKeyboard; }
            set
            { 
                recordKeyboard = value;
                RaisePropertyChanged("RecordKeyboard");
            }
        }

        /// <summary>
        /// 语音唤醒称呼
        /// </summary>
        public string WakeupCallName
        {
            get { return wakeupCallName; }
            set
            {
                wakeupCallName = value;
                RaisePropertyChanged("WakeupCallName");
            }
        }

        /// <summary>
        /// 录像数量上限
        /// </summary>
        public int MediaCount
        {
            get { return mediaCount; }
            set
            {
                mediaCount = value;
                RaisePropertyChanged("MediaCount");
            }
        }
        
        /// <summary>
        /// 报告打印图片上限
        /// </summary>
        public int PrintImageCount
        {
            get { return printImageCount; }
            set
            {
                printImageCount = value;
                RaisePropertyChanged("PrintImageCount");
            }
        }

        /// <summary>
        /// 采图数量上限
        /// </summary>
        public int CutshotImageCount
        {
            get { return cutshotImageCount; }
            set
            {
                cutshotImageCount = value;
                RaisePropertyChanged("CutshotImageCount");
            }
        }

        /// <summary>
        /// 采图生效
        /// </summary>
        public string CutshotSound
        {
            get { return cutshotSound; }
            set
            {
                cutshotSound = value;
                RaisePropertyChanged("CutshotSound");
            }
        }

        /// <summary>
        /// 检查序号长度
        /// </summary>
        public int OrderLength
        {
            get { return orderLength; }
            set
            {
                orderLength = value;
                RaisePropertyChanged("OrderLength");
            }
        }

        /// <summary>
        /// 检查序号前缀
        /// </summary>
        public string OrderPrefix
        {
            get { return orderPrefix; }
            set
            {
                orderPrefix = value;
                RaisePropertyChanged("OrderPrefix");
            }
        }

        /// <summary>
        /// 报告单图标
        /// </summary>
        public string ReportIcon
        {
            get { return reportIcon; }
            set
            {
                reportIcon = value;
                RaisePropertyChanged("ReportIcon");
            }
        }

        /// <summary>
        /// 报告单名称
        /// </summary>
        public string ReportName
        {
            get { return reportName; }
            set
            {
                reportName = value;
                RaisePropertyChanged("ReportName");
            }
        }

        /// <summary>
        /// 科室名称
        /// </summary>
        public string ClassName
        {
            get { return className; }
            set
            {
                className = value;
                RaisePropertyChanged("ClassName");
            }
        }

        /// <summary>
        /// 医院名称
        /// </summary>
        public string HospitalName
        {
            get { return hospitalName; }
            set
            {
                hospitalName = value;
                RaisePropertyChanged("HospitalName");
            }
        }
    }
}
