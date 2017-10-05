using FourierTransform.SignalRepresentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
        SignalRepresentation.SignalProfile wav = new SignalRepresentation.SignalProfile(0, 1, 44100, "c", "");
        public Lab2Main()
        {
            InitializeComponent();
            CutWav(@"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\song_full.wav",
                @"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav\song_cut_test.wav");


        }


        public List<SignalPoint> ReadWav(string path)
        {
            try
            {
                using (FileStream wavStream = new FileStream(path,
                    FileMode.Open, FileAccess.Read))
                {
                    byte[] head = new byte[H];
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
                    byte[] head = new byte[H];
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

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "wav files (*.wav)|*.wav";
                ofd.InitialDirectory = @"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\wav";
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var signal = ReadWav(ofd.FileName);
                    chart1.DataSource = signal;
                    chart1.Series[0].XValueMember = "X";
                    chart1.Series[0].YValueMembers = "Y";
                    chart1.DataBind();
                }
            }
        }
    }
}
