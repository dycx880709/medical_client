using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MM.Libs.RFID
{
    public class EPCInfo
    {
        public int DeviceID { get; set; }

        public int EPC { get; set; }
    }
 

    /// <summary>
    /// RFID封装类
    /// 采用小端序解析
    /// </summary>
    public class RFIDProxy
    {
        public event EventHandler<EPCInfo> NotifyEPCReceived;
        public event EventHandler<bool> NotifyDeviceStatusChanged;

        #region 串口操作

        SerialPort serialPort = null;
        bool isStart = true;

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="com"></param>
        public void Open(string com)
        {
            Close();
            if (string.IsNullOrWhiteSpace(com))
            {
                return;
            }
            serialPort = new SerialPort
            {
                BaudRate = 57600,
                PortName = com
            };
            serialPort.DataReceived += SerialPort_DataReceived;
            this.isStart = true;
            Task.Run(() =>
            {
                while (this.isStart)
                {
                    if (!serialPort.IsOpen)
                    {
                        try
                        {
                            serialPort.Open();

                            NotifyDeviceStatusChanged?.Invoke(this, true);
                        }
                        catch (Exception)
                        {
                            NotifyDeviceStatusChanged?.Invoke(this, false);

                        }
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public void OpenWait(string com)
        {
            if (string.IsNullOrWhiteSpace(com))
            {
                return;
            }
            serialPort = new SerialPort
            {
                BaudRate = 57600,
                PortName = com
            };
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (serialPort != null)
            {
                serialPort.DataReceived -= SerialPort_DataReceived;
                serialPort.Close();
                serialPort.Dispose();
            }
            this.isStart = false;
        }

        #endregion

        #region 串口交互

        byte[] readDatas = null;
        byte waitReply = 0x00;

        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] datas = new byte[serialPort.BytesToRead];
            serialPort.Read(datas, 0, datas.Length);
            Console.WriteLine("收到数据", datas);
            if (datas.Length >= 5)
            {
                if (datas[0] == 0x10 && datas[1] == 0x20)
                {
                    int deviceID = datas[2];

                    List<byte> buffer = new List<byte>();
                    string ascii = Encoding.ASCII.GetString(datas.Skip(3).Take(8).ToArray()).ToUpper();
                    for (int i = 0; i < ascii.Length/2; i++)
                    {
                        buffer.Add(Convert.ToByte(ascii.Substring(i * 2, 2), 16));
                    }
                    int epc = BitConverter.ToInt32(buffer.ToArray(), 0);
                    NotifyEPCReceived?.Invoke(this, new EPCInfo { DeviceID = deviceID, EPC = epc });
                }
                else
                {
                    byte cmd = datas[2];
                    if (cmd == waitReply)
                    {
                        readDatas = datas;
                    }
                    else if (cmd == 0xEE)
                    {
                        int epc = BitConverter.ToInt32(datas, 4);
                        NotifyEPCReceived?.Invoke(this, new EPCInfo { EPC = epc });
                    }
                }
            }
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private byte[] WriteMessage(List<byte> datas)
        {
            waitReply = datas[1];
            datas.Insert(0, (byte)(datas.Count + 2));
            byte[] crc = CRC16(datas.ToArray());
            datas.Add(crc[1]);
            datas.Add(crc[0]);
            serialPort.Write(datas.ToArray(), 0, datas.Count);
            readDatas = null;
            int index = 0;
            while (true)
            {
                if (readDatas != null)
                {
                    return readDatas;
                }
                if (index == 1000 / 5)
                {
                    throw new Exception("与设备通信超时");
                }
                index++;
                Thread.Sleep(5);
            }
        }

        #endregion

        #region 设备信息

        /// <summary>
        /// 设置设备地址
        /// </summary>
        public void SetDeviceAddress()
        {
            List<byte> datas = new List<byte> { 0xFF, 0x24, 0x23 };
            WriteMessage(datas);
        }

        /// <summary>
        /// 获取设备地址
        /// </summary>
        public void GetDeviceAddress()
        {
            List<byte> datas = new List<byte> { 0xFF, 0x21 };
            WriteMessage(datas);
        }

        #endregion

        #region EPC 操作

        /// <summary>
        /// 读EPC数据
        /// </summary>
        /// <returns></returns>
        public int GetEPC()
        {
            int epc;
            byte cmd = 0x0F;
            List<byte> datas = new List<byte> { 0xFF, cmd };
            byte[] result = WriteMessage(datas);
            switch (result[3])
            {
                case 0x01:
                    epc = BitConverter.ToInt32(result, 6);
                    break;
                default:
                    throw new Exception(GetError(result[3]));
            }
            return epc;
        }

        /// <summary>
        /// 写EPC数据
        /// </summary>
        /// <param name="epc"></param>
        public Task WriteEPC(int epc)
        {
            return Task.Run((() =>
             {
                     byte cmd = 0x04;
                     byte[] epcBuffer = BitConverter.GetBytes((Int32)epc);
                     List<byte> datas = new List<byte> { 0xFF, cmd, 0x02, 0x00, 0x00, 0x00, 0x00 };
                     datas.AddRange(epcBuffer);
                     byte[] result = WriteMessage(datas);
                     switch (result[3])
                     {
                         case 0x00:
                             break;
                         default:
                             throw new Exception(GetError(result[3]));

                     }
             }));
        }

        #endregion

        #region 通用方法

        /// <summary>
        /// 获取错误
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string GetError(byte code)
        {
            switch (code)
            {
                case 0x04:
                    return "读写器存储空间已满";
                case 0x05:
                    return "访问密码错误";
                case 0xFA:
                    return "有电子标签，但通信不畅，操作失败";
                case 0xFB:
                    return "无电子标签可操作";
                default:
                    return "未知错误";
            }
        }

        //计算CRC16 MSB-LSB
        private byte[] CRC16(byte[] data)
        {
            byte CRC16Lo;
            byte CRC16Hi;           //CRC寄存器 
            byte CL; byte CH;       //多项式码 ccitt的多项式是x16+x12+x5+1,多项式码是0x1021,但由于ccitt是默认先传LSB而不是MSB，故这里应该将多项式码按bit反转得到0x8408
            byte SaveHi; byte SaveLo;
            byte[] tmpData;
            int Flag;
            CRC16Lo = 0xFF;
            CRC16Hi = 0xFF;
            CL = 0x08;
            CH = 0x84;
            tmpData = data;
            for (int i = 0; i < tmpData.Length; i++)
            {
                CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器进行异或 
                for (Flag = 0; Flag <= 7; Flag++)
                {
                    SaveHi = CRC16Hi;
                    SaveLo = CRC16Lo;
                    CRC16Hi = (byte)(CRC16Hi >> 1);      //高位右移一位 
                    CRC16Lo = (byte)(CRC16Lo >> 1);      //低位右移一位 
                    if ((SaveHi & 0x01) == 0x01) //如果高位字节最后一位为1 
                    {
                        CRC16Lo = (byte)(CRC16Lo | 0x80);   //则低位字节右移后前面补1 
                    }             //否则自动补0 
                    if ((SaveLo & 0x01) == 0x01) //如果LSB为1，则与多项式码进行异或 
                    {
                        CRC16Hi = (byte)(CRC16Hi ^ CH);
                        CRC16Lo = (byte)(CRC16Lo ^ CL);
                    }
                }
            }
            byte[] ReturnData = new byte[2];
            ReturnData[0] = CRC16Hi;       //CRC高位 
            ReturnData[1] = CRC16Lo;       //CRC低位 
            return ReturnData;
        }

        #endregion
    }
}
