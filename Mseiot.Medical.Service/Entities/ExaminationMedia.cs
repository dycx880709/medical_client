using Ms.Libs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class ExaminationMedia : NotifyPropertyBase
    {
        private string path;
        private bool isSelected;
        private int examinationMediaID;
        private string examinationPart;
        
        public string ExaminationPart
        {
            get { return examinationPart; }
            set
            {
                examinationPart = value;
                RaisePropertyChanged("ExaminationPart");
            }
        }
        public int ExaminationMediaID
        {
            get { return examinationMediaID; }
            set
            {
                examinationMediaID = value;
                RaisePropertyChanged("ExaminationMediaID");
            }
        }
        public int ExaminationID { get; set; }
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                RaisePropertyChanged("Path");
            }
        }
        public byte[] Buffer { get; set; }
        public MediaType MediaType { get; set; }
        [JsonIgnore]
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
    }

    public enum MediaType
    { 
        Image,
        Video
    }
}
