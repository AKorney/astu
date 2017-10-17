using SignalProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Math;

namespace Lab3
{
    public partial class Form1 : Form
    {
        private const int F = 10*360;
        private const double step = 2 * PI / F;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var batt5 = CalculateBatterworthAFCDirectly(5);
            var batt8 = CalculateBatterworthAFCDirectly(8);
            var batt11 = CalculateBatterworthAFCDirectly(11);
            ChartForm battChart = new ChartForm(batt5, "w", "", "Batterworth (direct calculation)", "N=5");
            battChart.AddSeriresData(batt8, "N=8");
            battChart.AddSeriresData(batt11, "N=11");
            battChart.Show();
        }


        

        private void button2_Click(object sender, EventArgs e)
        {
            var cheb1_5 = CalculateCheb1Directly(5, 0.5);
            var cheb1_8 = CalculateCheb1Directly(8, 0.5);
            ChartForm chart = new ChartForm(cheb1_5, "w", "", "Chebyshev-1 (direct calculation)", "N=5, E=0.5");
            chart.AddSeriresData(cheb1_8, "N=8, E=0.5");
            chart.Show();
        }



        private List<Tuple<double, double>> CalculateBatterworthAFCDirectly(int n)
        {
            double batterworth(double w) => 1.0 / Sqrt(1.0 + Pow(w, 2 * n));
            
            var result = Enumerable.Range(0, F / 2).Select(
                (i) => new Tuple<double, double>(i * step, batterworth(step * i))
                ).ToList();
            return result;
        }


        private double Polynom(int n, double x)
        {
            if (n == 0) return 1;
            if (n == 1) return x;
            return 2 * x * Polynom(n - 1, x) - Polynom(n - 2, x);
        }


        private List<Tuple<double, double>> CalculateCheb1Directly(int n, double eps)
        {
            double cheb1(double x) => 1.0 / Sqrt(1.0 + Pow(eps * Polynom(n, x), 2));
            var result = Enumerable.Range(0, F / 2).Select(
                (i) => new Tuple<double, double>(i * step, cheb1(step * i))
                ).ToList();
            return result;
        }


        private List<Tuple<double, double>> CalculateCheb2Directly(int n, double eps)
        {
            double cheb2(double x) => 1.0 / Sqrt(1.0 + 1.0/Pow(eps * Polynom(n, 1.0/x), 2));
            var result = Enumerable.Range(0, F/2).Select(
                (i) => new Tuple<double, double>(i * step, cheb2(step * i))
                ).ToList();
            return result;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            var cheb2_5 = CalculateCheb2Directly(5, 0.1);
            var cheb2_8 = CalculateCheb2Directly(8, 0.1);
            ChartForm chart = new ChartForm(cheb2_5, "w", "", "Chebyshev-2 (direct calculation)", "N=5, E=0.1");
            chart.AddSeriresData(cheb2_8, "N=8, E=0.1");
            chart.Show();
        }
    }
}
