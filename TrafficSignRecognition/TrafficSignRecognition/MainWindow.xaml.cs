﻿using System;
using System.Collections.Generic;
using System.Drawing;
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

using TrafficSignRecognition.CV;

using Emgu.CV;
using Emgu.CV.Structure;

namespace TrafficSignRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Image<Rgb, Byte> img = new Image<Rgb, byte>(new Bitmap(@"D:\stud_repo\samples\yandex\crop\7.png"));

            Image<Gray, Byte> result = SignDetectionHelper.GrayScaleByChannel(img, 0);
            result.Bitmap.Save("fltr.jpg");
            SignDetectionHelper.DetectEdges(result, img);
            img.Bitmap.Save("test.jpg");


        }
    }
}
