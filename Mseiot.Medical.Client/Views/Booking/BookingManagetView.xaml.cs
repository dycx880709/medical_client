using System;
using System.Collections.Generic;
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

namespace Mseiot.Medical.Client.Views
{
    /// <summary>
    /// BookingManagetView.xaml 的交互逻辑
    /// </summary>
    public partial class BookingManagetView : UserControl
    {
        public BookingManagetView()
        {
            InitializeComponent();
        }

        private void AddBooking_Click(object sender, RoutedEventArgs e)
        {
            var view = new AddBookingView();
            child.ShowDialog("预约登记", view);
        }
    }
}
