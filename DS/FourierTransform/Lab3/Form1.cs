using SignalProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Math;

namespace Lab3
{
    public partial class Form1 : Form
    {
        private const int F = 5*360;
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

        private void button3_Click(object sender, EventArgs e)
        {
            var cheb2_5 = CalculateCheb2Directly(5, 0.1);
            var cheb2_8 = CalculateCheb2Directly(8, 0.1);
            ChartForm chart = new ChartForm(cheb2_5, "w", "", "Chebyshev-2 (direct calculation)", "N=5, E=0.1");
            chart.AddSeriresData(cheb2_8, "N=8, E=0.1");
            chart.Show();
        }
        #region DIRECT CALCUALTION

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
        #endregion
        #region COMPLEX AFC|FFC CALCULATIONS
        

        private Complex BatterwothH(Complex s, int n)
        {
            int coeff = n % 2;
            Complex mult = 1+s*coeff;
                        
            for (int k= 1; k <=(n-coeff)/2; k++)
            {
                double theta = (2 * k - 1) * PI / (2 * n);
                mult *= (s * s + 2 * Sin(theta) * s + 1);
            }
            return 1 / mult;
        }

        private (List<Tuple<double, double>> afc, List<Tuple<double, double>> ffc) CalculateBatterworth(int n)
        {
            
            var result = Enumerable.Range(0, F/2).Select(
                (i) => new Tuple<double, Complex>(i * step, BatterwothH(new Complex(0, i*step),n))
                ).ToList();

            var afcval = result.Select(p => new Tuple<double, double>(p.Item1, p.Item2.Magnitude));
            var ffcval = result.Select(p => new Tuple<double, double>(p.Item1, p.Item2.Phase + (p.Item2.Phase >0? -PI:0)));

            return (afcval.ToList(), ffcval.ToList());
        }


        private (List<Tuple<double, double>> afc, List<Tuple<double, double>> ffc) CalculateCheb1(int n, double e)
        {

            var result = Enumerable.Range(0, F / 2).Select(
                (i) => new Tuple<double, Complex>(i * step, Cheb1H(new Complex(0, i * step), n, e))
                ).ToList();

            var afcval = result.Select(p => new Tuple<double, double>(p.Item1, p.Item2.Magnitude));
            var ffcval = result.Select(p => new Tuple<double, double>(p.Item1, p.Item2.Phase + (p.Item2.Phase > 0 ? -PI : 0)));

            return (afcval.ToList(), ffcval.ToList());
        }
            
        private const double epsilon = 1e-1;
       
        private Complex Cheb1H(Complex s, int n, double e)
        {
            double arsh(double x) => Log(x + Sqrt(x * x + 1));
            double betta = arsh(1.0 / e) / n;
            double sig0 = -Sinh(betta);

            double Gp = n % 2 == 1 ? 1.0 : 1.0 / Sqrt(1.0 + e * e);
            Complex C = n % 2 == 1 ? -sig0 / (s - sig0) : 1.0;

            Complex mul = 1;
            for (int k = 1; k <= (n-n%2)/2; k++)
            {
                double thetta = (2 * k - 1) * PI / (2 * n);
                double sig = -Sin(thetta) * Sinh(betta);
                double w = Cos(thetta) * Cosh(betta);
                mul *= (sig * sig + w * w) / (s * s - 2 * sig * s + sig * sig + w * w);
            }
            return mul * Gp * C;
        }


        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            var batt_ch5 = CalculateBatterworth(5);
            var batt_ch8 = CalculateBatterworth(8);
            var batt_ch11 = CalculateBatterworth(11);
            ChartForm afcf = new ChartForm(batt_ch5.afc, "w", "", "batterwort h(s) based afc", "N=5");
            afcf.AddSeriresData(batt_ch8.afc, "N=8");
            afcf.AddSeriresData(batt_ch11.afc, "N=11");
            afcf.Show();


            ChartForm ffcf = new ChartForm(batt_ch5.ffc, "w", "", "batterwort h(s) based ffc", "N=5");
            ffcf.AddSeriresData(batt_ch8.ffc, "N=8");
            ffcf.AddSeriresData(batt_ch11.ffc, "N=11");
            ffcf.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var cheb_ch5 = CalculateCheb1(5, 0.5);
            var cheb_ch8 = CalculateCheb1(8, 0.5);
            
            ChartForm afcf = new ChartForm(cheb_ch5.afc, "w", "", "chebyshev h(s) based afc", "N=5 E=0.5");
            afcf.AddSeriresData(cheb_ch8.afc, "N=8 E=0.5");
            afcf.Show();


            ChartForm ffcf = new ChartForm(cheb_ch5.ffc, "w", "", "chebyshev h(s) based ffc", "N=5 E=0.5");
            ffcf.AddSeriresData(cheb_ch8.ffc, "N=8 E=0.5");
            ffcf.Show();
        }
    }
}
