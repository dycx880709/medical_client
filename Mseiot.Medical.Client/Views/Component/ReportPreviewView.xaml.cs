using MM.Medical.Client.Core;
using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;
using Size = System.Windows.Size;

namespace MM.Medical.Client.Views
{
    /// <summary>
    /// ReportPreviewView.xaml 的交互逻辑
    /// </summary>
    public partial class ReportPreviewView : UserControl, INotifyPropertyChanged
    {
        
        private Examination examination;
        private int appointmentID;

        public event PropertyChangedEventHandler PropertyChanged;
        public string Printer
        {
            get { return CacheHelper.LocalSetting.Printer; }
            set 
            {
                CacheHelper.LocalSetting.Printer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Printer"));
            }
        }


        public ReportPreviewView(int appointmentID)
        {
            InitializeComponent();
            this.appointmentID = appointmentID;
            this.Loaded += ReportPreviewView_Loaded;
        }

        private void ReportPreviewView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ReportPreviewView_Loaded;
            gb_content.Width = gb_content.ActualHeight / 1123 * 794;
            LoadExaminationInfo();
            LoadSystemSetting();
        }

        private void LoadExaminationInfo()
        {
            if (this.appointmentID != 0)
            {
                var result = loading.AsyncWait("获取检查信息中,请稍后", SocketProxy.Instance.GetExaminationsByAppointmentID(this.appointmentID));
                if (result.IsSuccess)
                {
                    this.examination = result.Content;
                    var CollectionView = CollectionViewSource.GetDefaultView(examination.Images ?? new ObservableCollection<ExaminationMedia>());
                    CollectionView.Filter = t => t is ExaminationMedia media && media.IsSelected;
                    gd_content.DataContext = result.Content;
                }
                else
                {
                    Alert.ShowMessage(true, AlertType.Error, $"获取检查信息失败,{ result.Error }");
                    this.Close();
                }
            }
        }

        private void LoadSystemSetting()
        {
            var result = loading.AsyncWait("获取模板配置中,请稍后", SocketProxy.Instance.GetSystemSetting());
            if (result.IsSuccess) sp_title.DataContext = result.Content;
            else
            {
                Alert.ShowMessage(true, AlertType.Error, $"获取模板配置失败,{ result.Error }");
                this.Close();
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(this.Printer))
            //{
            //    var view = new SetPrinterView();
            //    if (!cw.ShowDialog("打印机列表", view)) 
            //        return;
            //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Printer"));
            //}
            //var filePath = SaveJpeg(CacheHelper.TempPath);
            //var pd = new PrintDocument();
            //pd.PrinterSettings.PrinterName = CacheHelper.LocalSetting.Printer;
            //var i = Image.FromFile(filePath);
            //pd.DefaultPageSettings.PaperSize = new PaperSize("检 查报告单", i.Width, i.Height);
            //pd.DefaultPageSettings.Landscape = false;
            //pd.PrintPage += (_, ev) =>
            //{
            //    var m = ev.PageBounds;
            //    ev.Graphics.DrawImage(i, m);
            //    i.Dispose();
            //    examination.Appointment.AppointmentStatus = AppointmentStatus.Reported;
            //    var result = loading.AsyncWait("保存打印中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(examination.Appointment));
            //    if (result.IsSuccess)
            //        Alert.ShowMessage(true, AlertType.Success, "打印完成");
            //    File.Delete(filePath);
            //};
            //pd.Print();

            //var printDialog = new PrintDialog();
            //printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            //printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;
            //printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
            //printDialog.PrintTicket.PageBorderless = PageBorderless.None;
            //printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOB4);
            //if (printDialog.ShowDialog().Value)
            //{
            //    printDialog.PrintVisual(this.gb_print, "检查单报告");
            //    examination.Appointment.AppointmentStatus = AppointmentStatus.Reported;
            //    var result = loading.AsyncWait("保存打印中,请稍后", SocketProxy.Instance.ModifyAppointmentStatus(examination.Appointment));
            //    if (result.IsSuccess)
            //        Alert.ShowMessage(true, AlertType.Success, "打印完成");
            //}

            var fe = this.gb_print;
            var feSize = new Size(fe.Width, fe.Height); //for resize back to original size
            try
            {
                var dialog = new PrintDialog();
                dialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
                dialog.PrintTicket = dialog.PrintQueue.DefaultPrintTicket;
                dialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
                dialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA3);
                if (dialog.ShowDialog().Value)
                {
                    //Get the capabilities of the selected printer.
                    var capabilities = dialog.PrintQueue.GetPrintCapabilities(dialog.PrintTicket);
                    //Here my view is resized  
                    fe.Height = capabilities.PageImageableArea.ExtentHeight;
                    fe.Width = capabilities.PageImageableArea.ExtentWidth;
                    fe.UpdateLayout();
                    fe.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size size = new Size(fe.DesiredSize.Width, fe.DesiredSize.Height);
                    fe.Measure(size);
                    size = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
                    fe.Measure(size);
                    fe.Arrange(new Rect(size));
                    dialog.PrintVisual(fe, $"{ examination.Appointment.Name }检查报告单");
                }
            }
            finally
            {
                //Here I resize my view to the original size
                fe.Height = feSize.Height;
                fe.Width = feSize.Width;
                fe.UpdateLayout();
                fe.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size size = new Size(fe.ActualWidth, fe.ActualHeight);
                fe.Measure(size);
                fe.Arrange(new Rect(size));
            }

        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.DesktopDirectory;
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SaveJpeg(dialog.SelectedPath);
                Alert.ShowMessage(true, AlertType.Success, "检查单另存为成功");
            }
        }

        private string SaveJpeg(string folderPath)
        {
            var filePath = Path.Combine(folderPath, appointmentID.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg");
            var brush = new VisualBrush(this.gb_print);
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                context.DrawRectangle(brush, null, new Rect(0, 0, gb_print.ActualWidth, gb_print.ActualHeight));
                context.Close();
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)gb_print.ActualWidth, (int)gb_print.ActualHeight, 96, 96, PixelFormats.Default);
            bitmap.Render(visual);
            JpegBitmapEncoder encode = new JpegBitmapEncoder();
            encode.Frames.Add(BitmapFrame.Create(bitmap));
            using (FileStream file = new FileStream(filePath, FileMode.Create))
                encode.Save(file);
            return filePath;
        }

        private void PrinterSetting_Click(object sender, RoutedEventArgs e)
        {
            var view = new SetPrinterView();
            if (cw.ShowDialog("打印机设置", view))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Printer"));
        }
    }
}
