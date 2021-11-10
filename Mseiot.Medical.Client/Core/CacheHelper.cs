using MM.Medical.Client.Entities;
using Mseiot.Medical.Service.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MM.Medical.Client.Core
{
    public class CacheHelper : Share.Core.CacheHelper
    {
        public static string UserName { get { return CurrentUser.Name; } }
        public static string RoleName { get { return CurrentUser.RoleName; } }
        public static LocalSetting LocalSetting { get; private set; }
        public static User CurrentUser { get; set; } = new User();
        public static string ConsultingRoomName { get; set; }
        public static string RFIDCom { get; set; }
        public static int EndoscopeDeviceID { get; set; }
        public static string VideoPath { get; set; }
        public static bool IsDebug { get; set; }

        public static T GetConfig<T>(string name)
        {
            if (ConfigurationManager.AppSettings.HasKeys())
            {
                foreach (var key in ConfigurationManager.AppSettings.AllKeys)
                {
                    if (key.ToUpper().Equals(name.ToUpper()))
                        return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
                }
            }
            return default;
        }

        public static void LoadSetting()
        {
            CacheHelper.RFIDCom = CacheHelper.GetConfig<string>("RFIDCom");
            CacheHelper.ConsultingRoomName = CacheHelper.GetConfig<string>("ConsultingRoom");
            CacheHelper.EndoscopeDeviceID = CacheHelper.GetConfig<int>("EndoscopeDeviceID");
            CacheHelper.IsDebug = CacheHelper.GetConfig<bool>("IsDebug");

            if (File.Exists(SettingPath))
            {
                var json = File.ReadAllText(SettingPath, Encoding.Unicode);
                CacheHelper.LocalSetting = JsonConvert.DeserializeObject<LocalSetting>(json);
            }
            else CacheHelper.LocalSetting = new LocalSetting();
            CacheHelper.VideoPath = Path.Combine(Directory.GetCurrentDirectory(), "videos");
            if (Directory.Exists(CacheHelper.VideoPath))
                Directory.Delete(CacheHelper.VideoPath, true);
            Directory.CreateDirectory(CacheHelper.VideoPath);

            CacheHelper.LocalSetting.ServerSetting.Address= CacheHelper.GetConfig<string>("Address");
            CacheHelper.LocalSetting.ServerSetting.HttpPort = CacheHelper.GetConfig<int>("HttpPort");
            CacheHelper.LocalSetting.ServerSetting.TcpPort = CacheHelper.GetConfig<int>("TcpPort");
        }

        public static void SaveLocalSetting()
        {
            var json = JsonConvert.SerializeObject(CacheHelper.LocalSetting);
            File.WriteAllText(SettingPath, json, Encoding.Unicode);
        }

        public static ObservableCollection<MedicalTemplate> SortMedicalTemplates(IEnumerable<MedicalTemplate> templates, int parentId)
        {
            var results = new ObservableCollection<MedicalTemplate>();
            foreach (var template in templates)
            {
                if (template.ParentID == parentId)
                {
                    template.MedicalTemplates = CacheHelper.SortMedicalTemplates(templates, template.MedicalTemplateID);
                    results.Add(template);
                }
            }
            return results;
        }

        public static ObservableCollection<MedicalWord> SortMedicalWords(IEnumerable<MedicalWord> words, int parentId)
        {
            var results = new ObservableCollection<MedicalWord>();
            foreach (var word in words)
            {
                if (word.ParentID == parentId)
                {
                    word.MedicalWords = CacheHelper.SortMedicalWords(words, word.MedicalWordID);
                    results.Add(word);
                }
            }
            return results;
        }

        public static MedicalWord GetMedicalWordParent(IEnumerable<MedicalWord> words, int findId, int parentId = 0)
        {
            foreach (var word in words)
            {
                if (word.MedicalWordID == findId)
                {
                    if (word.ParentID != parentId)
                        return CacheHelper.GetMedicalWordParent(words, word.ParentID, parentId);
                    else return word;
                }
            }
            return null;
        }

        public static MedicalTemplate GetMedicalTemplateParent(IEnumerable<MedicalTemplate> templates, int parentId, int findId)
        {
            foreach (var template in templates)
            {
                if (template.MedicalTemplateID == parentId)
                {
                    if (template.MedicalTemplateID != findId) 
                        CacheHelper.GetMedicalTemplateParent(templates, template.ParentID, findId);
                    else return template;
                }
            }
            return null;
        }
    }
}
