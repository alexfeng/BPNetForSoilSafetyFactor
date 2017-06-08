using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;

namespace WinFormForBP
{
    public partial class ComputeForm : Form
    {
        public ComputeForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!Program.isTrained)
            {
                MessageBox.Show("请先训练网络！");
                return;
            }

            if (textBox1.Text.Equals("") || textBox2.Text.Equals("") ||
                textBox3.Text.Equals("") || textBox4.Text.Equals("") ||
                textBox5.Text.Equals("") || textBox6.Text.Equals("") ||
                textBox7.Text.Equals("") || textBox8.Text.Equals("") ||
                textBox9.Text.Equals("") || textBox10.Text.Equals("") ||
                textBox11.Text.Equals(""))
            {
                MessageBox.Show("参数不能为空");
                return;
            }

            double[] result_guess = new double[1];
            double[] inp = new double[11] { double.Parse(textBox1.Text),
                double.Parse(textBox2.Text),
                double.Parse(textBox3.Text),
                double.Parse(textBox4.Text),
                double.Parse(textBox5.Text),
                double.Parse(textBox6.Text),
                double.Parse(textBox7.Text),
                double.Parse(textBox8.Text),
                double.Parse(textBox9.Text),
                double.Parse(textBox10.Text),
                double.Parse(textBox11.Text)};

            for (int j = 0; j < Program.inputParaCount; ++j)
                inp[j] = Program.premnmx(inp[j], Program.min_Data[j], Program.max_Data[j]);

            double[] t = { inp[0], inp[1], inp[2], inp[3], inp[4], inp[5], inp[6] ,inp[7],inp[8],inp[9], inp[10] };

            // 使用网络对训练样本计算输出
            double[] result = Program.network.Compute(t);
            result_guess[0] = ((result[0] + 1) * (Program.max_T - Program.min_T) / 2) + Program.min_T;
            
            //Console.WriteLine((0).ToString() + "预测值为" + result_guess[0].ToString());

            textBox12.Text = result_guess[0].ToString();
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar == 46)
            {
                if (((TextBox)sender).Text.Length <= 0)
                    e.Handled = true;
                else
                {
                    float f;
                    float oldf;
                    bool b1 = float.TryParse(((TextBox)sender).Text, out oldf);
                    bool b2 = float.TryParse(((TextBox)sender).Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }
    }
}