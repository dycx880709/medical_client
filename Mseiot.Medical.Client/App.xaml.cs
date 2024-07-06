using LiveCharts;
using LiveCharts.Configurations;
using MM.Medical.Client.Core;
using MM.Medical.Client.Views;
using Ms.Libs.SysLib;
using Mseiot.Medical.Service.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MM.Medical.Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Test();
            ThreadPool.SetMaxThreads(100, 100);
            CacheHelper.LoadSetting();
            if (e.Args.Length > 0)
            {
                if (CacheHelper.LocalSetting.AutoLogin)
                {
                    CacheHelper.LocalSetting.AutoLogin = false;
                    CacheHelper.SaveLocalSetting();
                }
            }
            CacheHelper.InitialLogSetting();

            #region LiveChart
            Charting.For<TimeResult>(Mappers.Xy<TimeResult>().X(model => model.TimeStamp).Y(model => model.Count));
            #endregion

            #region 异常处理注册
            //UI线程未捕获异常处理事件
            this.DispatcherUnhandledException += (o,ex) => LogHelper.Instance.Error("捕获主线程未处理异常:", ex.Exception);
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += (o,ex) => LogHelper.Instance.Error(ex.Exception);
            //非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += (o,ex)=> LogHelper.Instance.Error($"程序发生错误：\n{(Exception)ex.ExceptionObject}");
            #endregion

            var login = new LoginView();
            login.ShowDialog();
        }
    }
}
