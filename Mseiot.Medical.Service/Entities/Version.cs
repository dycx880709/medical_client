using Ms.Libs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class Version : NotifyPropertyBase
    {
        private string code;
        private string content;
        private string localPath;

        public int VersionID { get; set; }

        public long Time { get; set; }

        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                RaisePropertyChanged("Content");
            }
        }
        public string UserID { get; set; }
        public string UserName { get; set; }

        public long Size { get; set; }

        public string Code
        {
            get { return code; }
            set
            {
                code = value;
                RaisePropertyChanged("Code");
            }
        }

        public string Path { get; set; }

        [JsonIgnore]
        public string LocalPath
        {
            get { return localPath; }
            set
            {
                localPath = value;
                RaisePropertyChanged("LocalPath");
            }
        }

    }
}
