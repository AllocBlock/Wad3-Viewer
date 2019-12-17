using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Wad3_Viewer
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        ResourceDictionary rd;

        public AboutWindow(Point pos)
        {
            InitializeComponent();
            RefreshResources();
            this.Left = pos.X - this.Width / 2;
            this.Top = pos.Y - this.Height / 2;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            Close();
        }

        private void RefreshResources()
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri(@"lang\" + (Application.Current as App).currentLang + ".xaml", UriKind.Relative)) as ResourceDictionary);
            rd = this.Resources.MergedDictionaries[0] as ResourceDictionary;
        }
    }
}
