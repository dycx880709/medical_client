using Ms.Libs.HttpLib;
using Ms.Libs.Models;
using Ms.Libs.TcpLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Services
{
    public partial class SocketProxy
    {
        public static SocketProxy Instance { get; } = new SocketProxy();
        public HttpProxy HttpProxy { get; private set; }
        public TcpProxy TcpProxy { get; private set; }

        public void Load(string address, int httpPort, int tcpPort)
        {
            var url = $"http://{ address }:{ httpPort }";
            this.HttpProxy = new HttpProxy(url);
            this.TcpProxy = new TcpProxy(address, tcpPort);
        }

        public async Task Start()
        {
            if (TcpProxy != null)
            {
                await TcpProxy.Start();
            }
        }

        public async Task Close()
        {
            if (TcpProxy != null)
            {
                await TcpProxy.Stop();
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public async Task<MsResult<string>> UploadFile(string localName)
        {
            var remoteName = Path.GetFileName(localName);
            return await HttpProxy.UploadFile<string>(localName, remoteName);
        }
    }
}
