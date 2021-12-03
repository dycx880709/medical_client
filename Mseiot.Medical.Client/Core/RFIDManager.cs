using MM.Libs.RFID;
using Ms.Libs.Models;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Medical.Client.Core
{
    public class RFIDManager
    {
        public static RFIDManager Instance { get; set; } = new RFIDManager();

        private List<RFIDProxyExtend> proxys;
        public List<RFIDDevice> Devices { get; set; }

        private RFIDManager() 
        {
            this.proxys = new List<RFIDProxyExtend>();
            this.Devices = new List<RFIDDevice>();
        }

        public async Task<(bool, string)> Load()
        {
            var result = await SocketProxy.Instance.GetRFIDDevices();
            if (result.IsSuccess)
                AddCom(result.Content.ToArray());
            return (result.IsSuccess, result.Error);
        }

        private void AddCom(params RFIDDevice[] devcices)
        {
            foreach (var devcice in devcices)
            {
                if (!proxys.Any(t => t.Equals(devcice.Com)))
                {
                    var proxy = new RFIDProxyExtend { Com = devcice.Com };
                    proxy.NotifyEPCReceived += (s, e) =>
                    {
                        if (s is RFIDProxyExtend rfid && rfid.RFIDNotify != null)
                            rfid.RFIDNotify(new EPCInfoArgs(e) { Com = rfid.Com });
                    };
                    proxy.NotifyDeviceStatusChanged += (sender, status) =>
                    {
                        if (sender is RFIDProxyExtend rfid)
                        {
                            if (status)
                                LogHelper.Instance.Error($"串口{ rfid.Com }连接失败");
                            else
                                LogHelper.Instance.Info($"串口{ rfid.Com }连接成功");
                        }
                    };
                    proxy.Open(devcice.Com);
                    proxys.Add(proxy);
                }
                Devices.Add(devcice);
            }
        }

        public void SubscribePort(string com, RFIDNotifyDelegate rFIDNotifyDelegate)
        {
            var proxy = proxys.FirstOrDefault(t => t.Com.Equals(com));
            if (proxy != null) 
                proxy.RFIDNotify += rFIDNotifyDelegate;
        }

        public void UnSubcribePort(string com, RFIDNotifyDelegate rFIDNotifyDelegate)
        {
            var proxy = proxys.FirstOrDefault(t => t.Com.Equals(com));
            if (proxy != null)
                proxy.RFIDNotify -= rFIDNotifyDelegate;
        }

        private class RFIDProxyExtend : RFIDProxy
        {
            public RFIDNotifyDelegate RFIDNotify { get; set; }
            public string Com { get; set; }
        }

        public class EPCInfoArgs : EPCInfo
        {
            public string Com { get; set; }
            public EPCInfoArgs(EPCInfo ePCInfo)
            {
                ePCInfo.CopyTo(this);
            }
        }

        public delegate string RFIDNotifyDelegate(EPCInfoArgs ePCInfo);
    }
}
