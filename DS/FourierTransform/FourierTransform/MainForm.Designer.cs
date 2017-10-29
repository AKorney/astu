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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea10 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend10 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea11 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend11 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea12 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend12 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.sourceChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button1 = new System.Windows.Forms.Button();
            this.ampSpec = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.phaseSpec = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button6 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sourceChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ampSpec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.phaseSpec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // sourceChart
            // 
            chartArea10.Name = "ChartArea1";
            this.sourceChart.ChartAreas.Add(chartArea10);
            legend10.Name = "Legend1";
            this.sourceChart.Legends.Add(legend10);
            this.sourceChart.Location = new System.Drawing.Point(12, 12);
            this.sourceChart.Name = "sourceChart";
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series10.Legend = "Legend1";
            series10.Name = "SourceSignal";
            this.sourceChart.Series.Add(series10);
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
            chartArea11.Name = "ChartArea1";
            this.ampSpec.ChartAreas.Add(chartArea11);
            legend11.Name = "Legend1";
            this.ampSpec.Legends.Add(legend11);
            this.ampSpec.Location = new System.Drawing.Point(12, 318);
            this.ampSpec.Name = "ampSpec";
            series11.ChartArea = "ChartArea1";
            series11.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series11.Legend = "Legend1";
            series11.Name = "Amp";
            this.ampSpec.Series.Add(series11);
            this.ampSpec.Size = new System.Drawing.Size(623, 300);
            this.ampSpec.TabIndex = 2;
            this.ampSpec.Text = "chart1";
            // 
            // phaseSpec
            // 
            chartArea12.Name = "ChartArea1";
            this.phaseSpec.ChartAreas.Add(chartArea12);
            legend12.Name = "Legend1";
            this.phaseSpec.Legends.Add(legend12);
            this.phaseSpec.Location = new System.Drawing.Point(641, 318);
            this.phaseSpec.Name = "phaseSpec";
            series12.ChartArea = "ChartArea1";
            series12.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series12.Legend = "Legend1";
            series12.Name = "Phase";
            this.phaseSpec.Series.Add(series12);
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
            this.button3.Text = "FFTn";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1071, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Time:";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1071, 237);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(161, 23);
            this.button5.TabIndex = 7;
            this.button5.Text = "Walsh";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(1072, 211);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(160, 20);
            this.numericUpDown1.TabIndex = 8;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(1071, 267);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(161, 23);
            this.button6.TabIndex = 9;
            this.button6.Text = "Hadamard";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1244, 624);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label1);
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
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart sourceChart;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataVisualization.Charting.Chart ampSpec;
        private System.Windows.Forms.DataVisualization.Charting.Chart phaseSpec;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button6;
    }
}

