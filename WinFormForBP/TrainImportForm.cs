using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using System.IO;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;

namespace WinFormForBP
{
    public partial class TrainImportForm : Form
    {
        public TrainImportForm()
        {
            InitializeComponent();
            Program.trainFilePath = @"初始数据.txt";
            loadTrainData(Program.trainFilePath);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(((int)e.KeyChar < 48 || (int)e.KeyChar > 57)&& (int)e.KeyChar !=8 && (int)e.KeyChar != 46)
                e.Handled = true;

            if ((int)e.KeyChar == 46)
            {
                if (textBox1.Text.Length <= 0)
                    e.Handled = true;
                else
                {
                    float f;
                    float oldf;
                    bool b1 = float.TryParse(textBox1.Text, out oldf);
                    bool b2 = float.TryParse(textBox1.Text + e.KeyChar.ToString(), out f);
                    if(b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(*.txt)|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;
                loadTrainData(path);
            }
        }

        private delegate void delegateCall();//这里定义一个委托
        private void train()
        {
            double errorOut;
            bool b = double.TryParse(textBox1.Text, out errorOut);
            if (b)
            {
                double error = 10;
                int iteration = 1;
                while (error > errorOut)
                {
                    error = Program.teacher.RunEpoch(Program.trainInput_Data, Program.trainOutput_Data);
                    this.listBox1.Invoke(new delegateCall(delegate
                    {
                        this.listBox1.Items.Add("第" + iteration.ToString() + "轮误差为" + error.ToString());
                        this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;
                    }));
                    //this.listBox1.Items.Add("第" + iteration.ToString() + "轮误差为" + error.ToString());
                    Console.WriteLine("第" + iteration.ToString() + "轮误差为" + error.ToString());
                    iteration++;
                }
                Program.isTrained = true;
                this.Invoke(new delegateCall(delegate {
                    MessageBox.Show("训练完成！训练误差是" + error.ToString());
                    ComputeForm compute = new ComputeForm();
                    compute.Show();
                }));
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            IterationRemoveItem(listBox1);
            Thread thread = new Thread(new ThreadStart(train));//定义线程
            thread.Start();//启动线程

            /*
            double errorOut;
            bool b = double.TryParse(textBox1.Text,out errorOut);
            if (b)
            {
                double error = 10;
                int iteration = 1;
                while (error > errorOut)
                {
                    error = Program.teacher.RunEpoch(Program.trainInput_Data, Program.trainOutput_Data);
                    this.listBox1.Items.Add("第" + iteration.ToString() + "轮误差为" + error.ToString());
                    Console.WriteLine("第" + iteration.ToString() + "轮误差为" + error.ToString());
                    iteration++;
                }
                Program.isTrained = true;
                MessageBox.Show("训练完成！训练误差是"+error.ToString());
                ComputeForm compute = new ComputeForm();
                compute.Show();
            }
            */
        }

        private void IterationRemoveItem(ListBox listbox)
        {
            for (int i = 0; i < listbox.Items.Count; i++)
            {
                listbox.Items.RemoveAt(i);
            }

            for (int j = 0; j < listbox.Items.Count; j++)
            {
                IterationRemoveItem(listbox);
            }
        }

        public void loadTrainData(string fileName)
        {
            Program.inputParaCount = 11;
            Program.outputParaCount = 1;
            Program.hideParaCount = (int)Math.Floor(Math.Sqrt(Program.inputParaCount + Program.outputParaCount)) + 10;
            Program.max_Data = new double[Program.inputParaCount];
            Program.min_Data = new double[Program.inputParaCount];

            Program.max_T = double.MinValue;
            Program.min_T = double.MaxValue;

            for (int i = 0; i < Program.inputParaCount; i++)
            {
                Program.max_Data[i] = double.MinValue;
                Program.min_Data[i] = double.MaxValue;
            }

            // 获取初始数据
            Program.length_Data = File.ReadAllLines(fileName).Length; ; // 初始数据

            Program.initData = new double[Program.length_Data][];

            StreamReader init_Data = new StreamReader(fileName);
            for (int i = 0; i < Program.length_Data; i++)
            {
                string value = init_Data.ReadLine();
                string[] temp = value.Split('\t');
                Program.initData[i] = new double[Program.inputParaCount + Program.outputParaCount];
                for (int j = 0; j < Program.inputParaCount + Program.outputParaCount; j++)
                    Program.initData[i][j] = double.Parse(temp[j]);
            }
            init_Data.Close();

            Program.trainNum_Data = Program.length_Data; // 训练数据
            Program.trainInput_Data = new double[Program.trainNum_Data][];
            Program.trainOutput_Data = new double[Program.trainNum_Data][];
            Program.Output_Data = new double[Program.trainNum_Data][];

            // 生成训练数据
            StreamWriter train_Data = new StreamWriter("训练数据.txt");
            for (int i = 0; i < Program.trainNum_Data; i++)
            {
                string temp = null;
                for (int j = 0; j < Program.inputParaCount + Program.outputParaCount; j++)
                    temp += Program.initData[i][j].ToString() + '\t';
                train_Data.WriteLine(temp);
            }
            train_Data.Close();

            // 读取训练数据
            StreamReader reader_trainData = new StreamReader("训练数据.txt");
            for (int i = 0; i < Program.trainNum_Data; i++)
            {
                string value = reader_trainData.ReadLine();
                string[] temp = value.Split('\t');

                Program.trainInput_Data[i] = new double[Program.inputParaCount];
                Program.trainOutput_Data[i] = new double[1];
                Program.Output_Data[i] = new double[1];

                for (int j = 0; j < Program.inputParaCount; j++)
                {
                    Program.trainInput_Data[i][j] = double.Parse(temp[j]);
                    if (Program.trainInput_Data[i][j] > Program.max_Data[j])
                        Program.max_Data[j] = Program.trainInput_Data[i][j];

                    if (Program.trainInput_Data[i][j] < Program.min_Data[j])
                        Program.min_Data[j] = Program.trainInput_Data[i][j];
                }

                Program.trainOutput_Data[i][0] = double.Parse(temp[Program.inputParaCount]);
                Program.Output_Data[i][0] = double.Parse(temp[Program.inputParaCount]);
                if (Program.trainOutput_Data[i][0] > Program.max_T)
                    Program.max_T = Program.trainOutput_Data[i][0];
                if (Program.trainOutput_Data[i][0] < Program.min_T)
                    Program.min_T = Program.trainOutput_Data[i][0];
            }
            reader_trainData.Close();
            // 归一化训练数据
            for (int i = 0; i < Program.trainNum_Data; i++)
            {
                for (int j = 0; j < Program.inputParaCount; j++)
                    Program.trainInput_Data[i][j] = Program.premnmx(Program.trainInput_Data[i][j], Program.min_Data[j], Program.max_Data[j]);

                Program.trainOutput_Data[i][0] = Program.premnmx(Program.trainOutput_Data[i][0], Program.min_T, Program.max_T);
            }

            // 构建训练网络
            // 创建一个多层神经网络，采用S形激活函数，各层分别7，20，1个神经元
            // (其中7是输入个数，1是输出个数，20是隐层节点个数)
            Program.network = new ActivationNetwork(new BipolarSigmoidFunction(3), Program.inputParaCount, Program.hideParaCount, Program.outputParaCount);
            //new SigmoidFunction()
            Program.teacher = new BackPropagationLearning(Program.network);
            Program.teacher.LearningRate = 0.01;//学习率
            Program.teacher.Momentum = 0;
        }
    }
}
