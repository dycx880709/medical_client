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
        private string errorMsg;
        private string videoPath;

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

        [JsonIgnore]
        public string ErrorMsg
        {
            get { return errorMsg; }
            set
            {
                errorMsg = value;
                RaisePropertyChanged("ErrorMsg");
            }
        }
        public string ExaminationPart
        {
            get { return examinationPart; }
            set
            {
                examinationPart = value;
                RaisePropertyChanged("ExaminationPart");
            }
        }
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                RaisePropertyChanged("Path");
            }
        }
        public string VideoPath
        {
            get { return videoPath; }
            set
            {
                videoPath = value;
                RaisePropertyChanged("VideoPath");
            }
        }
        [JsonIgnore]
        public string LocalVideoPath { get; set; }
        [JsonIgnore]
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
