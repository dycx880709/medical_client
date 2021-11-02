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
        public static LocalSetting LocalSetting { get; private set; }
        public static User CurrentUser { get; set; } = new User();
        public static string ConsultingRoomName { get; set; }

        public static string GetConfig(string name)
        {
            if (ConfigurationManager.AppSettings.HasKeys())
            {
                foreach (var key in ConfigurationManager.AppSettings.AllKeys)
                {
                    if (key.ToUpper().Equals(name.ToUpper()))
                        return ConfigurationManager.AppSettings[key];
                }
            }
            return "";
        }

        public static void LoadLocalSetting()
        {
            CacheHelper.ConsultingRoomName = CacheHelper.GetConfig("ConsultingRoom");
            if (File.Exists(SettingPath))
            {
                var json = File.ReadAllText(SettingPath, Encoding.Unicode);
                CacheHelper.LocalSetting = JsonConvert.DeserializeObject<LocalSetting>(json);
            }
            else CacheHelper.LocalSetting = new LocalSetting();
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
