using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game2048
{
    public class Theme
    {

    }

    public class ThemeForm : Form
    {
        public ThemeForm()
        {
            Text = "Game 2048";
            BackColor = Color.FromArgb(75, 88, 103);
        }
    }

    public class ThemeTile : Label
    {
        public ThemeTile()
        {
            BackColor = Color.FromArgb(51, 63, 77);
            TextAlign = ContentAlignment.MiddleCenter;
            Dock = DockStyle.Fill;
            Margin = new Padding(1);
        }

        public static readonly Dictionary<int, Color> TileColors = new Dictionary<int, Color>
                {
                    { 0,      Color.FromArgb( 51,  63,  77) },
                    { 2,      Color.FromArgb(129, 128, 215) },
                    { 4,      Color.FromArgb(162,  90, 214) },
                    { 8,      Color.FromArgb( 38, 105, 221) },
                    { 16,     Color.FromArgb( 33, 185, 213) },
                    { 32,     Color.FromArgb(  0, 202, 155) },
                    { 64,     Color.FromArgb( 67, 207,  23) },
                    { 128,    Color.FromArgb(247, 192,   1) },
                    { 256,    Color.FromArgb(245, 129,  20) },
                    { 512,    Color.FromArgb(255,  84,  61) },
                    { 1024,   Color.FromArgb(255,  20, 145) },
                    { 2048,   Color.FromArgb(190,   0, 129) },
                    { 4096,   Color.FromArgb( 20, 255, 255) },
                    { 8192,   Color.FromArgb(255, 144, 222) },
                    { 16384,  Color.FromArgb( 60,   0, 219) },
                    { 32768,  Color.FromArgb(255, 207, 171) },
                    { 65536,  Color.FromArgb(255,   0,  45) },
                    { 131072, Color.FromArgb(255, 255,   0) }
                };
    }
}
