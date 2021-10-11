using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Libs.RFID.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            RFIDProxy rfidProxy = new RFIDProxy();
            rfidProxy.NotifyEPCReceived += RfidProxy_NotifyEPCReceived;
            rfidProxy.NotifyDeviceStatusChanged += RfidProxy_NotifyDeviceStatusChanged;
            rfidProxy.Open("COM3");

                RFIDProxy rfidProxy1 = new RFIDProxy();
            rfidProxy1.NotifyEPCReceived += RfidProxy_NotifyEPCReceived;
            rfidProxy1.NotifyDeviceStatusChanged += RfidProxy_NotifyDeviceStatusChanged;
            rfidProxy1.Open("COM4");

            Console.ReadKey();
        }

        private static void RfidProxy_NotifyEPCReceived(object sender, int e)
        {
            Console.WriteLine("读到数据:{0}", e);
        }

        private static void RfidProxy_NotifyDeviceStatusChanged(object sender, bool e)
        {
            if (e)
            {
                Console.WriteLine("已经连接设备");
            }
            else
            {
                Console.WriteLine("设备连接中断");
            }
        }
    }
}
