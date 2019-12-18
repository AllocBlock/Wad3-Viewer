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

namespace Wad3_Viewer
{
    /// <summary>
    /// ColorBlock.xaml 的交互逻辑
    /// </summary>
    public partial class ColorBlock : UserControl
    {
        private Color color;

        public ColorBlock()
        {
            InitializeComponent();
            updateColor(Colors.Black);
        }
        public ColorBlock(Color color)
        {
            InitializeComponent();
            updateColor(color);
        }

        public void updateColor(Color color)
        {
            this.color = color;
            main.Background = new SolidColorBrush(color);
            main.ToolTip = ColorToString(color);
        }

        private string ColorToString(Color color)
        {
            string argb = color.ToString();
            string rgb = "#" + argb.Substring(3, 6);
            return rgb;
        }
    }
}
