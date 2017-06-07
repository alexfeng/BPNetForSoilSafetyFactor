using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormForBP
{
    public partial class SplashForm : Form
    {
        static SplashForm instance;

        public static SplashForm Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public SplashForm()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Bitmap bitmap = new Bitmap(Properties.Resources.边坡3);
            ClientSize = bitmap.Size;

            const string showInfo = "土质边坡安全系数快速预测系统v1.0";
            using (Font font = new Font("宋体", 16, FontStyle.Bold))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawString(showInfo, font, Brushes.Black, 130, 100);
                }
            }
            BackgroundImage = bitmap;
        }

        public static void ShowSplashForm()
        {
            instance = new SplashForm();
            instance.Show();
        }
    }
}
