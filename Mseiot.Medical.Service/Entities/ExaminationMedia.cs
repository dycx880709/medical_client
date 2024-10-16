﻿using Ms.Libs.Models;
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
        private string bodyPart;
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
        public string BodyPart
        {
            get { return bodyPart; }
            set
            {
                bodyPart = value;
                RaisePropertyChanged("BodyPart");
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
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        private string roi;
        public string ROI
        {
            get { return roi; }
            set
            { 
                roi = value;
                RaisePropertyChanged("ROI");
            }
        }

    }

    public enum MediaType
    { 
        Image,
        Video
    }
}
