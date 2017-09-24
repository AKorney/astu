﻿namespace FourierTransform
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.sourceChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button1 = new System.Windows.Forms.Button();
            this.ampSpec = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.phaseSpec = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sourceChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ampSpec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.phaseSpec)).BeginInit();
            this.SuspendLayout();
            // 
            // sourceChart
            // 
            chartArea4.Name = "ChartArea1";
            this.sourceChart.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.sourceChart.Legends.Add(legend4);
            this.sourceChart.Location = new System.Drawing.Point(12, 12);
            this.sourceChart.Name = "sourceChart";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "SourceSignal";
            this.sourceChart.Series.Add(series4);
            this.sourceChart.Size = new System.Drawing.Size(1053, 300);
            this.sourceChart.TabIndex = 0;
            this.sourceChart.Text = "chart1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1071, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(161, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Select File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ampSpec
            // 
            chartArea5.Name = "ChartArea1";
            this.ampSpec.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            this.ampSpec.Legends.Add(legend5);
            this.ampSpec.Location = new System.Drawing.Point(12, 318);
            this.ampSpec.Name = "ampSpec";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Legend = "Legend1";
            series5.Name = "Amp";
            this.ampSpec.Series.Add(series5);
            this.ampSpec.Size = new System.Drawing.Size(623, 300);
            this.ampSpec.TabIndex = 2;
            this.ampSpec.Text = "chart1";
            // 
            // phaseSpec
            // 
            chartArea6.Name = "ChartArea1";
            this.phaseSpec.ChartAreas.Add(chartArea6);
            legend6.Name = "Legend1";
            this.phaseSpec.Legends.Add(legend6);
            this.phaseSpec.Location = new System.Drawing.Point(641, 318);
            this.phaseSpec.Name = "phaseSpec";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Legend = "Legend1";
            series6.Name = "Phase";
            this.phaseSpec.Series.Add(series6);
            this.phaseSpec.Size = new System.Drawing.Size(591, 300);
            this.phaseSpec.TabIndex = 2;
            this.phaseSpec.Text = "chart1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1071, 41);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(161, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "DFT (classic)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1071, 71);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(161, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "DFT (improved)";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1071, 101);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(161, 23);
            this.button4.TabIndex = 5;
            this.button4.Text = "FFT";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1244, 624);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.phaseSpec);
            this.Controls.Add(this.ampSpec);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.sourceChart);
            this.Name = "MainForm";
            this.Text = "FourierTransform";
            ((System.ComponentModel.ISupportInitialize)(this.sourceChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ampSpec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.phaseSpec)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart sourceChart;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataVisualization.Charting.Chart ampSpec;
        private System.Windows.Forms.DataVisualization.Charting.Chart phaseSpec;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}

