using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Client.Entities
{
    public class ServerSetting : NotifyPropertyBase
    {
        private string address;
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

        private int httpPort;
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

        private int tcpPort;
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
