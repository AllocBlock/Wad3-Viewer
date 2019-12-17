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
using System.Windows.Navigation;
using Wad3Parser;
using Wad3Convertor;
using System.IO;

namespace Wad3_Viewer
{
    /// <summary>
    /// TextureFrame.xaml 的交互逻辑
    /// </summary>
    
    
    public partial class TextureFrame : UserControl
    {
        //定义一个委托
        public delegate void SetFocusImage(Texture texture);
        //定义一个事件
        public event SetFocusImage eventSetFocusPic;


        public double textBlockHeight = 15.0;
        public WadLump lump;

        ResourceDictionary rd;

        public TextureFrame(WadLump lump, SetFocusImage setDelegate)
        {
            InitializeComponent();
            RefreshResources();
            this.lump = lump;

            eventSetFocusPic += setDelegate;

            pic.Source = new Texture(lump).image;
            main.Width = lump.width;
            main.Height = lump.height + textBlockHeight;
            text.Height = textBlockHeight;
            textureNameText.Header = text.Text = new string(lump.lumpInfo.name);
            row2.Height = new GridLength(textBlockHeight, GridUnitType.Pixel);
        }

        public void TextureFrame_MouseLeftUp(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left)
            {
                eventSetFocusPic(new Texture(lump));
            }
        }

        private void SaveImage(BitmapSource bs, string format, string filePath)
        {
            BitmapEncoder encoder = null;
            if (format == "bmp") encoder = new JpegBitmapEncoder();
            else if (format == "jpg" || format == "jpeg") encoder = new BmpBitmapEncoder();
            else if (format == "png") encoder = new PngBitmapEncoder();
            else MessageBox.Show(rd["FormatNotSupported"] as string, rd["Error"] as string, 0);

            encoder.Frames.Add(BitmapFrame.Create(bs));
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                encoder.Save(stream);
        }

        private void Export_ContextClick(object sender, RoutedEventArgs e)
        {
            string filePath;
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.FileName = new string(lump.lumpInfo.name).Replace("\0", "");
            dlg.DefaultExt = ".bmp";
            dlg.Filter = "BMP (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                filePath = dlg.FileName;
            else
                return;

            SaveImage((BitmapSource)pic.Source, Path.GetExtension(filePath).Replace("." ,""), filePath);
        }

        private void RefreshResources()
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri(@"lang\" + (Application.Current as App).currentLang + ".xaml", UriKind.Relative)) as ResourceDictionary);
            rd = this.Resources.MergedDictionaries[0] as ResourceDictionary;
        }
    }
}
