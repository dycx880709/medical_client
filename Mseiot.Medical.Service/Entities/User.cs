using Ms.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mseiot.Medical.Service.Entities
{
    public class User : NotifyPropertyBase
    {
        private string name;
        private string loginName;
        private string loginPwd;
        private bool isOnline;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                RaisePropertyChanged("Name");
            }
        }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleID { get; set; }
        /// <summary>
        /// 登录名称
        /// </summary>
        public string LoginName
        {
            get { return this.loginName; }
            set
            {
                this.loginName = value;
                RaisePropertyChanged("LoginName");
            }
        }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPwd
        {
            get { return this.loginPwd; }
            set
            {
                this.loginPwd = value;
                RaisePropertyChanged("LoginPwd");
            }
        }
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline
        {
            get { return this.isOnline; }
            set
            {
                this.isOnline = value;
                RaisePropertyChanged("IsOnline");
            }
        }
        public int CreateTime { get; set; }
        public string Token { get; set; }
    }
}
