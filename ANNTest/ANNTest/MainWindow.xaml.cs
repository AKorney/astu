﻿using System;
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

namespace ANNTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //TrainHog.PrepareNegative(75, 40, 40, new string[] { @"D:\tmp\8755478.jpg", @"D:\учеба\train\54961551.jpg" });
            //var vec = TrainHog.GetVector(new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(@"D:\учеба\train\red circle\1\16.png"));
            //List<TrainSample> all = new List<TrainSample>();
            //for (int i = 0; i < 5; i++)
            //{
            //    var sm0 = TrainHog.ComputeForClass(string.Format("{0}{1}", @"D:\stud_repo\astu\train\red circle\",i), i);
            //    all.AddRange(sm0);
            //}
            //var neg = TrainHog.ComputeForClass( @"D:\stud_repo\astu\train\neg\", 5);
            //all.AddRange(neg);
            //(new ANNHelper()).Train(all, new Emgu.CV.Matrix<int>(new int [] { 324, 600, 6}));
            var test = new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(@"D:\stud_repo\train\99898.png");
            //var inp = TrainHog.GetVector(test);
            //var response = (new ANNHelper()).Predict(inp);
        }
    }
}
