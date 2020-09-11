using HWKJ.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Librarys.CurrentInfomation
{
    public class ConfigHunter
    {
        /// <summary>
        /// 内部服务URL
        /// </summary>
        public static string ServiceUrl = HWKJ.Configuration.ConfigurationManager.Configuration.Get<string>("/rhis/subsys.service/url") + "/";
         
        /// <summary>
        /// Fts访问地址
        /// </summary>
        public static string FtsHost { get; set; }
        /// <summary>
        /// HIS库
        /// </summary>
        public static string HisConnect { get; set; }
        /// <summary>
        /// Cache库
        /// </summary>
        public static string CacheConnect { get; set; }

        /// <summary>
        /// 测试用
        /// </summary>
        public static string testSqlCon = "Data Source=192.168.1.195;Initial Catalog=RHIS;User ID=sa;password=123;Integrated Security=False";
    }
}
