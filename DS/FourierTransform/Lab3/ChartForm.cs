using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SignalProcessing
{
    public partial class ChartForm : Form
    {
        public ChartForm(IEnumerable<Tuple<double,double>> values,string xTitle,string yTitle,string title, string legendText)
        {
            InitializeComponent();
            this.Text = title;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.RoundAxisValues();
            chart1.ChartAreas[0].AxisX.Title = xTitle;
            chart1.ChartAreas[0].AxisY.Title = yTitle;
            values.ForEach((point) => chart1.Series[0].Points.Add(new DataPoint(point.Item1, point.Item2)));
            chart1.Series[0].LegendText = legendText;
           
        }

        public void AddSeriresData(IEnumerable<Tuple<double, double>> values, string legendText)
        {
            chart1.Series.Add("Series" + (chart1.Series.Count+1).ToString());
            chart1.Series[chart1.Series.Count - 1].ChartType = SeriesChartType.Line;
            chart1.Series[chart1.Series.Count - 1].LegendText = legendText;
            values.ForEach((point) => chart1.Series[chart1.Series.Count-1].Points.Add(new DataPoint(point.Item1, point.Item2)));
        }
    }
}
