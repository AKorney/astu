using FourierTransform.Fourier;
using FourierTransform.SignalRepresentation;
using SignalProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FourierTransform.Lab2UI
{
    public partial class Lab2Main : Form
    {
        const int L = 1 << 20;
        const int H = 44;
        const int fileSize = 2 * L + H;
        Complex[] sourceSignal;
        private Complex[] fft;
        byte[] head = new byte[H];
        SignalProfile wav = new SignalProfile(0, 1, 44100, "c", "");
        private List<SignalPoint> signal;

        public Lab2Main()
        {
            InitializeComponent();
         


        }


        public List<SignalPoint> ReadWav(string path)
        {
            try
            {
                using (FileStream wavStream = new FileStream(path,
                    FileMode.Open, FileAccess.Read))
                {
                    int readHeader = wavStream.Read(head, 0, H);
                    int length =
                            (((int)head[43]) << 24) +
                            (((int)head[42]) << 16) +
                            (((int)head[41]) << 8) +
                            (int)head[40];
                   
                    var content = new byte[length];
                    wavStream.Read(content, 0, length);
                    List<SignalRepresentation.SignalPoint> rawwav = new List<SignalRepresentation.SignalPoint>();

                    for (int i = 0; i < length/2; i++)
                    {
                        byte byteOne = content[i * 2];
                        byte byteTwo = content[i * 2 + 1];
                        rawwav.Add(new SignalPoint { X = i, Y = (int)(short)(byteOne | byteTwo << 8) });
                    }
                    return SignalConverter.ConvertWithProfile(rawwav, wav); 
                }
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }           
        }

        public void CutWav(string sourcePath, string targetPath, int offset = 0)
        {
            var content = ReadWavFragment(sourcePath, offset);
            File.WriteAllBytes(targetPath, content);
        }
        public byte[] ReadWavFragment(string path, int offset = 0)
        {
            
            byte[] content = new byte[fileSize];
            try
            {
                
                using (FileStream wavStream = new FileStream(path,
                    FileMode.Open, FileAccess.Read))
                {
                    
                    int readHeader = wavStream.Read(head, 0, H);
                    int k;
                    k = L * 2 + H - 8;
                    head[4] = Convert.ToByte(k % 256);
                    k >>= 8;
                    head[5] = Convert.ToByte(k % 256);
                    k >>= 8;
                    head[6] = Convert.ToByte(k % 256);
                    head[7] = 0;

                    head[36] = Convert.ToByte('d');
                    head[37] = Convert.ToByte('a');
                    head[38] = Convert.ToByte('t');
                    head[39] = Convert.ToByte('a');

                    k = L * 2;
                    head[40] = Convert.ToByte(k % 256);
                    k >>= 8;
                    head[41] = Convert.ToByte(k % 256);
                    k >>= 8;
                    head[42] = Convert.ToByte(k % 256);
                    head[43] = 0;
                    for (int i = 0; i < offset; i++)
                    {
                        wavStream.ReadByte();
                    }
                    head.CopyTo(content, 0);
                    wavStream.Read(content, H, 2*L);
                }
            }
            catch (FileNotFoundException ioEx)
            {
                
            }
            return content;
        }

        private void SaveWav(string path, int[] data)
        {
            using (FileStream wavStream = new FileStream(path,
                    FileMode.OpenOrCreate, FileAccess.Write))
            {
                wavStream.Write(head, 0, H);
                for (int i = 0; i < data.Length; i++)
                {
                    short sh = (short)data[i];
                    wavStream.Write(BitConverter.GetBytes(sh),0,2);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "wav files (*.wav)|*.wav";
                ofd.InitialDirectory = @"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav";
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    signal = ReadWav(ofd.FileName);

                    var testData = signal.Select(p => (int)p.Y).ToArray();
                    SaveWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\test.wav", testData);

                    chart1.DataSource = signal;
                    chart1.Series[0].XValueMember = "X";
                    chart1.Series[0].YValueMembers = "Y";
                    chart1.DataBind();

                    
                    /*
                    int pow = (int)Math.Floor(Math.Log(signal.Count, 2));
                    sourceSignal = signal.Select(s => new Complex(s.Y, 0)).ToList().GetRange(0,(int) Math.Pow(2,pow)).ToArray();
                    fft = FourierTransformer.FFT(sourceSignal, false);
                    var specs = FourierTransformer.CalculateSpecs(fft);

                    chart2.DataSource = specs.amplitudeSpec;
                    chart2.Series[0].XValueMember = "X";
                    chart2.Series[0].YValueMembers = "Y";
                    chart2.DataBind();

                    chart3.DataSource = specs.phaseSpec;
                    chart3.Series[0].XValueMember = "X";
                    chart3.Series[0].YValueMembers = "Y";
                    chart3.DataBind();
                    //*/
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var hff = FourierTransformer.HFF(fft, wav.Frequency, Convert.ToDouble(textBox1.Text));
            var data = hff.Select((c, i) => new SignalPoint { X = i, Y = c.Real });
            SaveWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\hff_test.wav"
                   , data.Select(p => (int)p.Y).ToArray());
            chart4.DataSource = data; 
            chart4.Series[0].XValueMember = "X";
            chart4.Series[0].YValueMembers = "Y";
            chart4.DataBind();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var lff = FourierTransformer.LFF(fft, wav.Frequency, Convert.ToDouble(textBox2.Text));
            var data = lff.Select((c, i) => new SignalPoint { X = i, Y = c.Real });
            SaveWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\lff_test.wav"
                    , data.Select(p => (int)p.Y).ToArray());
            chart5.DataSource = data;
            chart5.Series[0].XValueMember = "X";
            chart5.Series[0].YValueMembers = "Y";
            chart5.DataBind();
        }
  
        private Wavelets.WaveletTransformer _waveletWorker = new Wavelets.WaveletTransformer();
        private void button9_Click(object sender, EventArgs e)
        {
            int order = (int)numericUpDown3.Value;
            if (radioButton1.Checked)
            {
                var invert = _waveletWorker.LowPassFilterHaar(signal                    
                    .Select(p => p.Y)
                    .ToArray(), order);
                var invertData = invert.Select((d, i) => new Tuple<double, double>(signal[i].X, d));
                ChartForm invertChart = new ChartForm(invertData, "", "", "Haar's transform (invert)", "order: " + order.ToString());
                invertChart.Show();
                SaveWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\"
                    +"haar_lp_order_"+order.ToString()+".wav"
                   , invert.Select(d=>(int)d).ToArray());
                return;
            }
            if (radioButton2.Checked)
            {
                var invert = _waveletWorker.HighPassFilterHaar(signal                    
                    .Select(p => p.Y)
                    .ToArray(), order);
                var invertData = invert.Select((d, i) => new Tuple<double, double>(signal[i].X, d));
                ChartForm invertChart = new ChartForm(invertData, "", "", "Haar's transform (invert)", "order: " + order.ToString());
                invertChart.Show();
                SaveWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\"
                   + "haar_hp_order_" + order.ToString() + ".wav"
                  , invert.Select(d => (int)d).ToArray());
                return;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int order = (int)numericUpDown3.Value;
            if (radioButton1.Checked)
            {
                var invert = _waveletWorker.LowPassFilterDaubechies(signal
                    .Select(p => p.Y)
                    .ToArray(), order);
                var invertData = invert.Select((d, i) => new Tuple<double, double>(signal[i].X, d));
                ChartForm invertChart = new ChartForm(invertData, "", "", "Daubechies's transform (invert)", "order: " + order.ToString());
                invertChart.Show();
                SaveWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\"
                   + "daub_lp_order_" + order.ToString() + ".wav"
                  , invert.Select(d => (int)d).ToArray());
                return;
            }
            if (radioButton2.Checked)
            {
                var invert = _waveletWorker.HighPassFilterDaubechies(signal
                    .Select(p => p.Y)
                    .ToArray(), order);
                var invertData = invert.Select((d, i) => new Tuple<double, double>(signal[i].X, d));
                ChartForm invertChart = new ChartForm(invertData, "", "", "Daubechies's transform (invert)", "order: " + order.ToString());
                invertChart.Show();
                SaveWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\"
                   + "daub_hp_order_" + order.ToString() + ".wav"
                  , invert.Select(d => (int)d).ToArray());
                return;
            }
        }
    }
}
