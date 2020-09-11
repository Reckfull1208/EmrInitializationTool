using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmrInitializationTool.Views.EmrViews
{
    /// <summary>
    /// EmrExport.xaml 的交互逻辑
    /// </summary>
    public partial class EmrExport : UserControl
    {
        public EmrExport()
        {
            InitializeComponent();
          //  Messenger.Default.Send(ring, "Emrport");
        }

        /// <summary>
        /// 此方式用来解决Treeview SelectedItem属性为只读的情况
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewSelectedHelper.Content = e.NewValue;
        }
    }
}
