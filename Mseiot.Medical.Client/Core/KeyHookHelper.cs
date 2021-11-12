using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MM.Medical.Client.Core
{
    public static class KeyHookHelper
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event")]

        public static extern void keybd_event(
        byte bVk, //虚拟键值
        byte bScan,// 一般为0
        int dwFlags, //这里是整数类型 0 为按下，2为释放
        int dwExtraInfo //这里是整数类型 一般情况下设成为0
        );

        #region 模拟按键
        public static void ResetSystem()
        {
            keybd_event(121, 0, 0, 0);
            keybd_event(121, 0, 2, 0);
        }
        #endregion
    }
}
