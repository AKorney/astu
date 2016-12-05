using Emgu.CV;
using Emgu.CV.Structure;
using nn.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrafficSign.Detection.Blue;
using TrafficSign.Detection.Red;
using TrafficSign.Recognition.Convertion;
using TrafficSign.Recognition.TrainingSetExtraction;
using TrafficSignRecognizer.Recognition.Neural;

namespace TrafficSignRecognizer
{
    public partial class MainForm : Form
    {
        RedCirclesDetector _reds = new RedCirclesDetector();
        BlueCirclesDetection _blues = new BlueCirclesDetection();
        NeuralNetwork _redNet = new NeuralNetwork("ad.xml");
        NeuralNetwork _blueNet = new NeuralNetwork("normal.xml");
        Dictionary<int, Bitmap> _redClasses = new Dictionary<int, Bitmap>();
        Dictionary<int, Bitmap> _blueClasses = new Dictionary<int, Bitmap>();
        public MainForm()
        {
            InitializeComponent();
            _redClasses.Add(1, new Bitmap(@"D:\stud_repo\astu\TrafficSignRecognition\TrafficSignRecognizer\res\r0.jpg"));
            _redClasses.Add(2, new Bitmap(@"D:\stud_repo\astu\TrafficSignRecognition\TrafficSignRecognizer\res\r1.jpg"));
            _redClasses.Add(3, new Bitmap(@"D:\stud_repo\astu\TrafficSignRecognition\TrafficSignRecognizer\res\r2.png"));

            _blueClasses.Add(1, new Bitmap(@"D:\stud_repo\astu\TrafficSignRecognition\TrafficSignRecognizer\res\b0.jpg"));
            _blueClasses.Add(2, new Bitmap(@"D:\stud_repo\astu\TrafficSignRecognition\TrafficSignRecognizer\res\b1.jpg"));
            _blueClasses.Add(3, new Bitmap(@"D:\stud_repo\astu\TrafficSignRecognition\TrafficSignRecognizer\res\b2.jpg"));

            //TextExtractor ext = new TextExtractor(_blues);
            //ext.PrepareData(@"D:\stud_repo\astu\train\blue", 3, new Size(40, 40));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task[] classifiers = new Task[2];
            flowLayoutPanel1.Controls.Clear();
            using (OpenFileDialog ofd = new OpenFileDialog()
                { Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
                    })
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    Emgu.CV.Image<Rgb, byte> source = new Emgu.CV.Image<Rgb, byte>(ofd.FileName);
                    List<PredictionResult> resultRed = new List<PredictionResult>()
                        , resultBlue = new List<PredictionResult>();
                    Stopwatch watch = Stopwatch.StartNew();
                    classifiers[0] = Task.Factory.StartNew(() =>
                    {   
                        resultBlue = RunBlue(source);

                   });
                    classifiers[1] = Task.Factory.StartNew(() =>
                    {                        
                        resultRed = RunRed(source);
                        
                    });
                    Task.WaitAll(classifiers);
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine("parallel: " + watch.ElapsedMilliseconds);
                    var redRois = from pr in resultRed select pr.ROI;
                    var bluRois = from pr in resultBlue select pr.ROI;
                    var rois = redRois.Concat(bluRois);
                    foreach (var roi in rois)
                    {
                        CvInvoke.Rectangle(source, roi, new MCvScalar(255, 255, 0));
                    }
                    pictureBox1.Image = source.Bitmap;

                    foreach (var res in resultBlue)
                    {
                        PictureBox pb = new PictureBox() { Size = new Size(60, 60), SizeMode = PictureBoxSizeMode.Zoom };
                        pb.Image = _blueClasses[res.ClassId];
                        flowLayoutPanel1.Controls.Add(pb);
                    }

                    foreach (var res in resultRed)
                    {
                        PictureBox pb = new PictureBox() { Size = new Size(60, 60), SizeMode = PictureBoxSizeMode.Zoom };
                        pb.Image = _redClasses[res.ClassId];
                        flowLayoutPanel1.Controls.Add(pb);
                    }
                    //List<PredictionResult> resultRed = RunBlue(ofd.FileName);
                }
            }
        }

        private List<PredictionResult> RunBlue(Emgu.CV.Image<Rgb, byte> source)
        {
            List<PredictionResult> results = new List<PredictionResult>();
           // = new Emgu.CV.Image<Rgb, byte>(path);
            Stopwatch watch = Stopwatch.StartNew();
            var gray = source.Convert<Gray, byte>();
            var blueTarget = _blues.FilterColorChannel(source);
            //blueTarget.Save("colored.jpg");
            Emgu.CV.Image<Gray, Byte> accumulator = new Emgu.CV.Image<Gray, byte>(source.Width, source.Height, new Gray(0));
            var ROIs = _blues.FindROIs(blueTarget, accumulator);
            //_blues.Highlight(ROIs, source);
            //pictureBox1.Image = source.Bitmap;
            foreach (var roi in ROIs)
            {
                gray.ROI = roi;
                accumulator.ROI = roi;
                var img = _blues.FindObjectOfInterest(gray, blueTarget, new Size(40, 40));
                double[] answer;
                _blueNet.NetworkOut(ImageToSample.ConvertImageToSample(img), out answer);
                results.Add(new PredictionResult(answer, roi));
                //MessageBox.Show(string.Format("ans: {0}\n{1}\n{2}", answer[0], answer[1], answer[2]));
            }
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("blue " + watch.ElapsedMilliseconds);
            return results;
        }
        private List<PredictionResult> RunRed(Emgu.CV.Image<Rgb, byte> source)
        {
            List<PredictionResult> results = new List<PredictionResult>();
            //Emgu.CV.Image<Rgb, byte> source = new Emgu.CV.Image<Rgb, byte>(path);
            Stopwatch watch = Stopwatch.StartNew();
            var gray = source.Convert<Gray, byte>();
            var redTarget = _reds.FilterColorChannel(source);
            Emgu.CV.Image<Gray, Byte> accumulator = new Emgu.CV.Image<Gray, byte>(source.Width, source.Height, new Gray(0));
            var ROIs = _reds.FindROIs(redTarget, accumulator);
            //_reds.Highlight(ROIs, source);
            //pictureBox1.Image = source.Bitmap;
            foreach (var roi in ROIs)
            {
                gray.ROI = roi;
                redTarget.ROI = roi;
                var img = _reds.FindObjectOfInterest(gray, redTarget, new Size(40, 40));
                double[] answer;
                
                _redNet.NetworkOut(ImageToSample.ConvertImageToSample(img), out answer);
                
                results.Add(new PredictionResult(answer, roi));
                //MessageBox.Show(string.Format("ans: {0}\n{1}\n{2}", answer[0], answer[1], answer[2]));
            }
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("red " + watch.ElapsedMilliseconds);
            return results;
        }
    }
}
