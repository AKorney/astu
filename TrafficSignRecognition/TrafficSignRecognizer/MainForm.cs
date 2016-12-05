using Emgu.CV;
using Emgu.CV.Structure;
using nn.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public MainForm()
        {
            InitializeComponent();
            //TextExtractor ext = new TextExtractor(_blues);
            //ext.PrepareData(@"D:\stud_repo\astu\train\blue", 3, new Size(40, 40));
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Task[] classifiers = new Task[2];

            using (OpenFileDialog ofd = new OpenFileDialog()
                { Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
                    })
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    Emgu.CV.Image<Rgb, byte> source = new Emgu.CV.Image<Rgb, byte>(ofd.FileName);
                    List<PredictionResult> resultRed = new List<PredictionResult>()
                        , resultBlue = new List<PredictionResult>();
                    classifiers[0] = Task.Factory.StartNew(() =>
                    {
                        resultBlue = RunBlue(ofd.FileName);
                    });
                    classifiers[1] = Task.Factory.StartNew(() =>
                    {
                        resultRed = RunRed(ofd.FileName);
                    });
                    Task.WaitAll(classifiers);

                    var redRois = from pr in resultRed select pr.ROI;
                    var bluRois = from pr in resultBlue select pr.ROI;
                    var rois = redRois.Concat(bluRois);
                    foreach (var roi in rois)
                    {
                        CvInvoke.Rectangle(source, roi, new MCvScalar(255, 255, 0));
                    }
                    pictureBox1.Image = source.Bitmap;
                      
                    //List<PredictionResult> resultRed = RunBlue(ofd.FileName);
                }
            }
        }

        private List<PredictionResult> RunBlue(string path)
        {
            List<PredictionResult> results = new List<PredictionResult>();
            Emgu.CV.Image<Rgb, byte> source = new Emgu.CV.Image<Rgb, byte>(path);
            var gray = source.Convert<Gray, byte>();
            var blueTarget = _blues.FilterColorChannel(source);
            blueTarget.Save("colored.jpg");
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
            return results;
        }
        private List<PredictionResult> RunRed(string path)
        {
            List<PredictionResult> results = new List<PredictionResult>();
            Emgu.CV.Image<Rgb, byte> source = new Emgu.CV.Image<Rgb, byte>(path);
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
            return results;
        }
    }
}
