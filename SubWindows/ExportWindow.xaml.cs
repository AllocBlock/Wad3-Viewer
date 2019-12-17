using System;
using System.Collections.Generic;
using System.IO;
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
    /// ExportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExportWindow : Window
    {
        private UIElementCollection tfs;
        ResourceDictionary rd;
        public ExportWindow(Point pos, UIElementCollection tfs)
        {
            InitializeComponent();
            RefreshResources();
            this.Left = pos.X - this.Width/2;
            this.Top = pos.Y - this.Height/2;
            this.tfs = tfs;
        }


        private void SaveImage(BitmapSource bs, string format, string filePath)
        {
            BitmapEncoder encoder = null;
            if (format == "bmp") encoder = new JpegBitmapEncoder();
            else if (format == "jpg" || format == "jpeg") encoder = new BmpBitmapEncoder();
            else if (format == "png") encoder = new PngBitmapEncoder();
            else  MessageBox.Show(rd["FormatNotSupported"] as string, rd["Error"] as string, 0);

            encoder.Frames.Add(BitmapFrame.Create(bs));
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                encoder.Save(stream);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var selectFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (selectFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                filePathText.Text = selectFolderDialog.SelectedPath;
            else
                return;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            Close();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            // 检查路径有效性
            try
            {
                if (! new DirectoryInfo(filePathText.Text).Exists)
                {
                    MessageBox.Show(rd["PathNotExists"] as string, rd["Error"] as string, 0);
                    return;
                }

            }
            catch {
                MessageBox.Show(rd["PathNotCorrect"] as string, rd["Error"] as string, 0);
                return;
            }

            string extend = "";

            // 生成指定类型编码器
            if (radioBmp.IsChecked == true) extend = "bmp";
            else if (radioJpg.IsChecked == true) extend = "jpg";
            else if (radioPng.IsChecked == true)  extend = "png";
            else MessageBox.Show(rd["FormatNotSupported"] as string, rd["Error"] as string, 0);

            // 导出
            foreach (TextureFrame tf in tfs)
            {
                string filePath = filePathText.Text + "\\" + new string(tf.lump.lumpInfo.name).Replace("\0", "") + "." + extend;
                SaveImage((BitmapSource)tf.pic.Source, extend, filePath);
            }

            System.Diagnostics.Process.Start("explorer.exe", filePathText.Text);
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
