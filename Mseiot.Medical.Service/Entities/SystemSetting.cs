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

        public string CutshotKeyboard
        {
            get { return cutshotKeyboard; }
            set
            {
                cutshotKeyboard = value;
                RaisePropertyChanged("CutshotKeyboard");
            }
        }

        public string WakeupCallName
        {
            get { return wakeupCallName; }
            set
            {
                wakeupCallName = value;
                RaisePropertyChanged("WakeupCallName");
            }
        }

        public int MediaCount
        {
            get { return mediaCount; }
            set
            {
                mediaCount = value;
                RaisePropertyChanged("MediaCount");
            }
        }

        public int PrintImageCount
        {
            get { return printImageCount; }
            set
            {
                printImageCount = value;
                RaisePropertyChanged("PrintImageCount");
            }
        }

        public int CutshotImageCount
        {
            get { return cutshotImageCount; }
            set
            {
                cutshotImageCount = value;
                RaisePropertyChanged("CutshotImageCount");
            }
        }

        public string CutshotSound
        {
            get { return cutshotSound; }
            set
            {
                cutshotSound = value;
                RaisePropertyChanged("CutshotSound");
            }
        }

        public int OrderLength
        {
            get { return orderLength; }
            set
            {
                orderLength = value;
                RaisePropertyChanged("OrderLength");
            }
        }

        public string OrderPrefix
        {
            get { return orderPrefix; }
            set
            {
                orderPrefix = value;
                RaisePropertyChanged("OrderPrefix");
            }
        }

        public string ReportIcon
        {
            get { return reportIcon; }
            set
            {
                reportIcon = value;
                RaisePropertyChanged("ReportIcon");
            }
        }

        public string ReportName
        {
            get { return reportName; }
            set
            {
                reportName = value;
                RaisePropertyChanged("ReportName");
            }
        }

        public string ClassName
        {
            get { return className; }
            set
            {
                className = value;
                RaisePropertyChanged("ClassName");
            }
        }

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
