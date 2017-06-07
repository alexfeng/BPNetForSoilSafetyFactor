using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;

namespace WinFormForBP
{
    static class Program
    {

        public static int length_Data;
        public static int inputParaCount;
        public static int hideParaCount;
        public static int outputParaCount;
        public static double[][] initData;
        public static double[][] trainInput_Data;
        public static double[][] trainOutput_Data;
        public static double[][] Output_Data;
        public static double[] max_Data;
        public static double[] min_Data;
        public static double max_T;
        public static double min_T;
        public static ActivationNetwork network;
        public static BackPropagationLearning teacher;
        public static int trainNum_Data;

        //public static double[][] testInput_Data;
        //public static double[] testOutput_Data;

        public static string trainFilePath;

        public static bool isTrained;

        // 将num映射到[-1, 1]中
        public static double premnmx(double num, double min, double max)
        {
            if (max == min)
            {
                return -1;
            }
            if (num > max)
                num = max;
            if (num < min)
                num = min;

            return 2 * (num - min) / (max - min) - 1;
        }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //==========================================================================================
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SplashForm.ShowSplashForm();
            System.Threading.Thread.Sleep(3000);
            if(SplashForm.Instance != null)
            {
                SplashForm.Instance.BeginInvoke(new MethodInvoker(SplashForm.Instance.Dispose));
                SplashForm.Instance = null;
            }
            Application.Run(new TrainImportForm());
        }
    }
}
