using MM.Medical.Client.Core;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// SetPrinterView.xaml 的交互逻辑
    /// </summary>
    public partial class SetPrinterView : UserControl
    {
        public SetPrinterView()
        {
            InitializeComponent();
            this.Loaded += SetPrinterView_Loaded;
        }

        private void SetPrinterView_Loaded(object sender, RoutedEventArgs e)
        {
            var printerList = new List<string>();
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                printerList.Add(PrinterSettings.InstalledPrinters[i]);
            lb_printer.ItemsSource = printerList;
            if (!string.IsNullOrEmpty(CacheHelper.LocalSetting.Printer))
                lb_printer.SelectedIndex = printerList.IndexOf(CacheHelper.LocalSetting.Printer);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (lb_printer.SelectedValue is string printerName)
            {
                CacheHelper.LocalSetting.Printer = printerName;
                CacheHelper.SaveLocalSetting();
                this.Close(true);
            }
        }
    }
}
