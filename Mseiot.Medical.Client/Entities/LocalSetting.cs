using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Client.Entities
{
    public class LocalSetting
    {
        public List<UserRecord> UserRecords { get; set; }
        public UserRecord UserRecord { get; set; }
        public ServerSetting ServerSetting { get; set; }
        public List<ServerSetting> ServerSettingRecords { get; set; }
        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool IsRemember { get; set; }
        public string LoginName { get { return UserRecord.LoginName; } }
        public string LoginPwd { get { return UserRecord.LoginPwd; } }

        public LocalSetting()
        {
            this.UserRecords = new List<UserRecord>();
            this.UserRecord = new UserRecord();
            this.ServerSetting = new ServerSetting();
            this.ServerSettingRecords = new List<ServerSetting>();
        }
    }
}
