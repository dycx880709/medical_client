using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace MM.Medical.Client.Core
{
    public class HotKeyManager
    {
        private Dictionary<int, Action> actions;
        private HotKeyManager() 
        {
            this.actions = new Dictionary<int, Action>();
        }

        public static HotKeyManager Instance { get; set; } = new HotKeyManager();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int WM_HOTKEY = 0x0312;
        public const uint MOD_NONE = 0x0000;
        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;

        public void RegisterHotKey(Window widow, int id, Key key, uint modifiers, Action callback)
        {
            var helper = new WindowInteropHelper(widow);
            var windowHandle = helper.Handle;
            HwndSource source = HwndSource.FromHwnd(windowHandle);
            source.AddHook(HwndHook);
            if (HotKeyManager.RegisterHotKey(windowHandle, id, modifiers, (uint)KeyInterop.VirtualKeyFromKey(key)))
                actions.Add(id, callback);
        }

        public void UnregisterHotKey(Window widow, int id)
        {
            var helper = new WindowInteropHelper(widow);
            HotKeyManager.UnregisterHotKey(helper.Handle, id);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == HotKeyManager.WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (actions.ContainsKey(id))
                {
                    actions[id].Invoke();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }
    }
}
