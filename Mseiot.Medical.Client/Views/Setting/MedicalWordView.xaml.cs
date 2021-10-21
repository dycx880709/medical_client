using Ms.Controls;
using Mseiot.Medical.Service.Entities;
using Mseiot.Medical.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// MedicalWordView.xaml 的交互逻辑
    /// </summary>
    public partial class MedicalWordView : UserControl
    {
        public MedicalWordView()
        {
            InitializeComponent();
            this.Loaded += MedicalWordView_Loaded;
        }

        private void MedicalWordView_Loaded(object sender, RoutedEventArgs e)
        {
            var result = loading.AsyncWait("获取医学词库中,请稍后", SocketProxy.Instance.GetMedicalWords());
            if (result.IsSuccess) lt_diagnosis.ItemsSource = SortMedicalWords(result.Content, 0);
            else MsWindow.ShowDialog($"获取医学词库失败,{ result.Error }", "软件提示");
        }

        private ObservableCollection<MedicalWord> SortMedicalWords(IEnumerable<MedicalWord> medicalWords, int parentId)
        {
            var results = new ObservableCollection<MedicalWord>();
            foreach (var medicalWord in medicalWords)
            {
                if (medicalWord.ParentID == parentId)
                {
                    medicalWord.MedicalWords = SortMedicalWords(medicalWords, medicalWord.MedicalWordID);
                    results.Add(medicalWord);
                }
            }
            return results;
        }
    }
}