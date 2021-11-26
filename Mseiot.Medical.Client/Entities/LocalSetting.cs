using Mseiot.Medical.Service.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Medical.Client.Entities
{
    public class LocalSetting
    {
        public List<UserRecord> UserRecords { get; set; }
        public UserRecord UserRecord { get; set; }
        public ServerSetting ServerSetting { get; set; }
        public List<ServerSetting> ServerSettingRecords { get; set; }
        public string Printer { get; set; }
        public string ConsultingRoomName { get; set; }
        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool IsRemember { get; set; }
        /// <summary>
        /// 自动登录
        /// </summary>
        public bool AutoLogin { get; set; }
        public string LoginName { get { return UserRecord.LoginName; } }
        public string LoginPwd { get { return UserRecord.LoginPwd; } }
        public string RFIDCom { get; set; }
        public LocalSetting()
        {
            this.UserRecords = new List<UserRecord>();
            this.UserRecord = new UserRecord();
            this.ServerSetting = new ServerSetting();
            this.ServerSettingRecords = new List<ServerSetting>();
        }
    }
}
