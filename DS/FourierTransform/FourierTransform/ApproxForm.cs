﻿using FourierTransform.Fourier;
using FourierTransform.SignalRepresentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FourierTransform
{
    public partial class ApproxForm : Form
    {
        private Dictionary<string, SignalProfile> _profiles = new Dictionary<string, SignalProfile>();
        private List<SignalPoint> _transformedSource;
        private List<SignalPoint> _rawSource;
        private const int High = 30;
        private const int Low = 5;
        public ApproxForm()
        {
            InitializeComponent();
            _profiles.Add("cardio", new SignalProfile(127, 60, 360, "сек", "мВ"));
            _profiles.Add("reo", new SignalProfile(0, 50, 360, "сек", "мОм"));
            _profiles.Add("velo", new SignalProfile(512, 120, 360, "сек", "мВ"));
            _profiles.Add("spiro", new SignalProfile(512, 100, 360, "сек", "Л"));
            _profiles.Add("default", new SignalProfile(0, 256, 360, "сек", ""));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "txt files (*.txt)|*.txt";
                ofd.InitialDirectory = @"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\test";
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _rawSource = new List<SignalPoint>();

                    var profileKey = Regex.Replace(Path.GetFileNameWithoutExtension(ofd.FileName), "[0-9]", "");
                    if (!_profiles.ContainsKey(profileKey))
                        profileKey = "default";

                    var rawContent = File.ReadAllText(ofd.FileName);
                    var splittedText = rawContent.Split(new Char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < splittedText.Length; i++)
                    {
                        _rawSource.Add(new SignalPoint { X = i, Y = Convert.ToDouble(splittedText[i].Replace(".", ",")) });
                    }
                    _transformedSource = SignalConverter.ConvertWithProfile(_rawSource, _profiles[profileKey]);

                    chart1.DataSource = _transformedSource;
                    chart1.Series[0].XValueMember = "X";
                    chart1.Series[0].YValueMembers = "Y";

                    chart1.ChartAreas[0].AxisX.Title = _profiles[profileKey].XLabel;
                    chart1.ChartAreas[0].AxisY.Title = _profiles[profileKey].YLabel;
                    chart1.DataBind();
                    chart2.ChartAreas[0].AxisX.Title = _profiles[profileKey].XLabel;
                    chart2.ChartAreas[0].AxisY.Title = _profiles[profileKey].YLabel;
                    chart3.ChartAreas[0].AxisX.Title = _profiles[profileKey].XLabel;
                    chart3.ChartAreas[0].AxisY.Title = _profiles[profileKey].YLabel;
                    chart4.ChartAreas[0].AxisX.Title = _profiles[profileKey].XLabel;
                    chart4.ChartAreas[0].AxisY.Title = _profiles[profileKey].YLabel;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var transform = FourierTransformer.CalculateAB(_transformedSource);
            var allHarms = FourierTransformer.InverseTransform(transform);

            chart2.DataSource = allHarms;
            chart2.Series[0].XValueMember = "X";
            chart2.Series[0].YValueMembers = "Y";
            chart2.DataBind();

            for (int i = High; i < transform.a.Length-High; i++)
            {
                transform.a[i] = transform.b[i] = 0;
            }
            var resultHigh = FourierTransformer.InverseTransform(transform);
            chart3.DataSource = resultHigh;
            chart3.Series[0].XValueMember = "X";
            chart3.Series[0].YValueMembers = "Y";
            chart3.DataBind();


            for (int i = Low; i < transform.a.Length - Low; i++)
            {
                transform.a[i] = transform.b[i] = 0;
            }
            var resultLow = FourierTransformer.InverseTransform(transform);
            chart4.DataSource = resultLow;
            chart4.Series[0].XValueMember = "X";
            chart4.Series[0].YValueMembers = "Y";
            chart4.DataBind();
        }
    }
}
