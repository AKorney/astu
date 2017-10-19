using FourierTransform.Fourier;
using FourierTransform.Lab2;
using FourierTransform.SignalRepresentation;
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
    public partial class FiltersForm : Form
    {
        const int L = 1 << 20;
        const int H = 44;
        const int fileSize = 2 * L + H;
        Complex[] sourceSignal;
        private Complex[] fft;
        byte[] head = new byte[H];
        SignalProfile wav = new SignalProfile(0, 1, 44100, "c", "");
        private List<SignalPoint> signal;
        public FiltersForm()
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

                    for (int i = 0; i < length / 2; i++)
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
        private void SaveWav(string path, int[] data)
        {
            using (FileStream wavStream = new FileStream(path,
                    FileMode.OpenOrCreate, FileAccess.Write))
            {
                wavStream.Write(head, 0, H);
                for (int i = 0; i < data.Length; i++)
                {
                    short sh = (short)data[i];
                    wavStream.Write(BitConverter.GetBytes(sh), 0, 2);
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
                    /*
                    SaveWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\test.wav", testData);

                    chart1.DataSource = signal;
                    chart1.Series[0].XValueMember = "X";
                    chart1.Series[0].YValueMembers = "Y";
                    chart1.DataBind();
                    //*/
                }
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            double rawFc = Convert.ToDouble(textBox1.Text);
            double normFC = rawFc / (wav.Frequency / 2);
            int N = Convert.ToInt32(textBox2.Text);

            WindowType window = WindowType.Rectangular;
            if (hamm.Checked) window = WindowType.Hamming;
            if (hann.Checked) window = WindowType.Hanning;
            if (bart.Checked) window = WindowType.Bartlet;
            if (black.Checked) window = WindowType.Blackman;

            var result = Filter.ApplyLowFilter(signal.Select(p => p.Y).ToArray(), normFC, N, window);

            SaveWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\" 
                + string.Format("f-{0}-w-{1}-n-{2}.wav", rawFc, window.ToString(), N),
                result.Select(d=>(int)d).ToArray());

            var newSignal = result.Select((d, i) => new SignalPoint { X = i / wav.Frequency, Y = d });
            chart2.DataSource = newSignal;
            chart2.Series[0].XValueMember = "X";
            chart2.Series[0].YValueMembers = "Y";
            chart2.DataBind();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double rawFc = Convert.ToDouble(textBox1.Text);
            double normFC = rawFc / (wav.Frequency / 2);
            int N = Convert.ToInt32(textBox2.Text);

            WindowType window = WindowType.Rectangular;
            if (hamm.Checked) window = WindowType.Hamming;
            if (hann.Checked) window = WindowType.Hanning;
            if (bart.Checked) window = WindowType.Bartlet;
            if (black.Checked) window = WindowType.Blackman;
            var AFCData = Filter.CalculateAFC(normFC, N, window);

            var sigPoints = AFCData.result.Take(N).Select((d, i) => new SignalPoint { X = i, Y = d });
            chart3.DataSource = sigPoints;
            chart3.Series[0].XValueMember = "X";
            chart3.Series[0].YValueMembers = "Y";
            chart3.DataBind();


            var afcPoints = AFCData.simpleAFC.Select((d, i) => new SignalPoint { X = i, Y = d });

            chart4.DataSource = afcPoints;
            chart4.Series[0].XValueMember = "X";
            chart4.Series[0].YValueMembers = "Y";
            chart4.DataBind();

            var dbPoints = AFCData.dbAFC.Select((d, i) => new SignalPoint { X = i, Y = d });
            chart5.DataSource = dbPoints;
            chart5.Series[0].XValueMember = "X";
            chart5.Series[0].YValueMembers = "Y";
            chart5.DataBind();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double rawFc = Convert.ToDouble(textBox1.Text);
            double normFC = rawFc / (wav.Frequency / 2);
            int N = Convert.ToInt32(textBox2.Text);
            var result = Filter.ApplyCheb(signal.Select(p => p.Y).ToArray(), normFC, 4);

            SaveWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\"
                + string.Format("cheb-{0}.wav", rawFc),
                result.Select(d => (int)d).ToArray());
        }
    }
}
