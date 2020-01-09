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
using System.Windows.Shapes;
using Wad3Parser;
using Wad3Convertor;
using System.IO;

namespace Wad3_Viewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        const double scaleMax = 8.0, scaleMin = 0.1;
        ResourceDictionary rd;
        bool isMouseLeftDown;
        Point lastMousePos, lastImagePos;

        public MainWindow()
        {
            InitializeComponent();
            RefreshResources();
            PreviewClear();
            PalatteDefault();
        }
        private void PreviewReset()
        {
            focusImageScale.ScaleX = focusImageScale.ScaleY = 1.0;
            Canvas.SetLeft(focusImage, preview.ActualWidth / 2 - focusImage.ActualWidth / 2);
            Canvas.SetTop(focusImage, preview.ActualHeight / 2 - focusImage.ActualHeight / 2);
        }
        private void PreviewClear()
        {
            PreviewReset();
            focusImage.Source = null;
            focusImageName.Text = rd["DefaultTips"] as string;
        }
        private void RefreshResources()
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri(@"lang\" + (Application.Current as App).currentLang + ".xaml", UriKind.Relative)) as ResourceDictionary);
            rd = this.Resources.MergedDictionaries[0] as ResourceDictionary;
        }


        private void PalatteDefault()
        {
            palatte.Children.Clear();
            palatteTip.Visibility = Visibility.Visible;
        }

        private void PalatteUpdate(List<Color> newPalatte)
        {
            palatte.Children.Clear();

            for (int i = 0; i < 256; i++)
            {
                this.palatte.Children.Add(new ColorBlock(newPalatte[i]));
            }
            palatteTip.Visibility = Visibility.Hidden;
        }

        public void SetFocusPic(Texture texture)
        {
            focusImage.Source = texture.fullImage;
            Canvas.SetLeft(focusImage, preview.ActualWidth / 2 - texture.width / 2);
            Canvas.SetTop(focusImage, preview.ActualHeight / 2 - texture.height / 2);
            focusImageScale.CenterX = texture.width / 2;
            focusImageScale.CenterY = texture.height / 2;

            focusImageName.Text = String.Format("{0} ({1}x{2}, 0x{3})", texture.textureName, texture.width, texture.height, Convert.ToString(texture.textureType, 16));

            if (texture.hasPalatte) PalatteUpdate(texture.palatte);
            else PalatteDefault();
        }

        void Open_MenuClick(object sender, RoutedEventArgs e)
        {
            string fileName;
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.DefaultExt = ".wad";
            dlg.Filter = "WAD3 (*.wad)|*.wad";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                fileName = dlg.FileName;
            else
                return;
            
            OpenFile(fileName);
        }

        private void OpenFile(string fileName)
        {
            table.Children.Clear();
            Parser wad3Parser = new Parser();
            List<WadLump> lumps = wad3Parser.Parse(fileName);
            if (lumps == null)
            {
                
                MessageBox.Show(rd["FileFormatError"] as string, rd["Error"] as string, 0);
                return;
            }
            foreach (WadLump lump in lumps)
            {
                table.Children.Add(new TextureFrame(lump, new TextureFrame.SetFocusImage(this.SetFocusPic)));
            }

            PreviewClear();
        }

        private void Preview_MouseWheel (object sender, MouseWheelEventArgs e){
            double zoom = e.Delta > 0 ? .2 : -.2;
            
            focusImageScale.ScaleX = Math.Max(Math.Min(focusImageScale.ScaleX + zoom, scaleMax), scaleMin);
            focusImageScale.ScaleY = Math.Max(Math.Min(focusImageScale.ScaleY + zoom, scaleMax), scaleMin);
        }

        private void Preview_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseLeftDown)
            {
                Vector delta = lastMousePos - e.GetPosition(preview);
                Canvas.SetLeft(focusImage, lastImagePos.X - delta.X);
                Canvas.SetTop(focusImage, lastImagePos.Y - delta.Y);
            }
        }

        private void Preview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isMouseLeftDown = true;
                lastMousePos = e.GetPosition(preview);
                lastImagePos = new Point(Canvas.GetLeft(focusImage), Canvas.GetTop(focusImage));
                Mouse.Capture(preview);
            }
        }
        private void Preview_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isMouseLeftDown = false;
                Mouse.Capture(null);
            }
            
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            PreviewReset();
        }

        private void ExportAll_MenuClick(object sender, RoutedEventArgs e)
        {
            ExportWindow ew = new ExportWindow(new Point(this.Left + this.ActualWidth / 2, this.Top + this.ActualHeight / 2 ), table.Children);
            ew.ShowDialog();
        }

        private void Exit_MenuClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_MenuClick(object sender, RoutedEventArgs e)
        {
            AboutWindow aw = new AboutWindow(new Point(this.Left + this.ActualWidth / 2, this.Top + this.ActualHeight / 2));
            aw.ShowDialog();
        }

        private void Program_Drop(object sender, DragEventArgs e)
        {
            string fileName = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            OpenFile(fileName);
        }

        private void LangCn_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).currentLang = "zh-CN";
            RefreshResources();
        }

        private void LangEn_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).currentLang = "en";
            RefreshResources();
        }

      
    }
}
