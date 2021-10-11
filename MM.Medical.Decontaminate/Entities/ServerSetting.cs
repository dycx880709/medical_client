using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Medical.Decontaminate.Entities
{
    public class ServerSetting : NotifyPropertyBase
    {
        private int httpPort = 19000;
        private string address = "10.211.55.2";
        private int tcpPort = 19001;

        /// <summary>
        /// 服务地址
        /// </summary>
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                RaisePropertyChanged("Address");
            }
        }

        /// <summary>
        /// 服务Http端口
        /// </summary>
        public int HttpPort
        {
            get { return httpPort; }
            set
            {
                httpPort = value;
                RaisePropertyChanged("HttpPort");
            }
        }

        /// <summary>
        /// 服务Tcp端口
        /// </summary>
        public int TcpPort
        {
            get { return tcpPort; }
            set
            {
                tcpPort = value;
                RaisePropertyChanged("TcpPort");
            }
        }
    }
}
