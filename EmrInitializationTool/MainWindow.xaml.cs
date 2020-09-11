using FirstFloor.ModernUI.Windows.Controls;
using HWKJ.Configuration;
using Librarys.CurrentInfomation;
using Librarys.FTS;
using System.Windows;
using System.Windows.Controls;

namespace EmrInitializationTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            ReadConfig();
            InitializeComponent();
            //初始化上传下载方法
            UploadDownHelper.Init(ConfigHunter.FtsHost);
            AppGlobal.CurrentPath = System.IO.Directory.GetCurrentDirectory();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<Border>(this, "Emrport");
        }

        #region 配置
        private void ReadConfig()
        {
            var url = System.Configuration.ConfigurationManager.AppSettings["configurationUrl"];
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("未加载配置文件，检查后再试");
                this.Close();
            }
            ConfigurationManager.InitializeFromRemote(url);
            /// <summary>
            /// Fts访问地址
            /// </summary>
            ConfigHunter.FtsHost = HWKJ.Configuration.ConfigurationManager.Configuration.Get<string>("/fts/webApiUrl");
            /// <summary>
            /// HIS库
            /// </summary>
            ConfigHunter.HisConnect = HWKJ.Configuration.ConfigurationManager.Configuration.RhisDbConnection();
            /// <summary>
            /// Cache库
            /// </summary>
            ConfigHunter.CacheConnect = HWKJ.Configuration.ConfigurationManager.Configuration.CacheDbConnection();
        } 
        #endregion
         
    }
     

}
