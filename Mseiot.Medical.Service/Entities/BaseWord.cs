using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class BaseWord : NotifyPropertyBase
    {
        private string title;
        private string content;
        private bool editable = true;

        /// <summary>
        /// 基础词汇ID
        /// </summary>
        public int BaseWordID { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                RaisePropertyChanged("Content");
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool Editable
        {
            get { return editable; }
            set
            {
                editable = value;
                RaisePropertyChanged("Editable");
            }
        }

        public int CreateTime { get; set; }
    }
}
