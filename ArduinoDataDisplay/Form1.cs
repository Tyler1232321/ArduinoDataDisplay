using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Management;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO.Ports;
using System.IO;
using System.Runtime.InteropServices;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        #region Initialize Variables
        String Input; // Gets input from the arduino that is used to determine which tab the next input is designed for
        int data; // Gets the data value from the arduino
        int counter = 0; // Unused counter
        String[] ports; // List of the ports detected
        SerialPort port;
        bool connected = false; // Boolean that tracks whether tab1 is currently running
        Stopwatch sw; // A stopwatch instance for the first tab
        int seriesNo = 0; // Tracks the numbers of series plotted
        String selectedPort; // Stores name of the selected port
        String fileName; // Stores the filename

        private List<DataPoint> dataArray = new List<DataPoint>(); // List of data points to be used for plotting
        private Series curr_series; // The series we will be plotting

        List<String> lines; // List of strings holding the data recieved from the arduino for saving to a file

        #region Tab2 Initialize Variables

        String[] ports2;
        SerialPort port2;
        bool connected2 = false;
        Stopwatch sw2;
        int seriesNo2 = 0;
        String selectedPort2;
        String fileName2;

        private List<DataPoint> dataArray2 = new List<DataPoint>();
        private Series curr_series2;

        List<String> lines2;

        #endregion
        #endregion

        public Form1()
        {
            InitializeComponent();
            getAvailableComPorts(); // Get the list of Com Ports and display them in the comboBox

            comboBox1.Items.Clear();
            for (int i = 1; i < 50; i++)
            {
                comboBox2.Items.Add(i); // Add 50 seconds worth of time options to the time comboBox
            }
            comboBox2.Items.Add("Forever"); // Add forever
            string[] portNames = SerialPort.GetPortNames(); // Get Port names
            foreach (string name in portNames) // Add the port names to the port selection comboBox
            {
                comboBox1.Items.Add(name); 
            }

            if (portNames.Length == 0) // Send no serial port available if no port detected
            {
                MessageBox.Show("No Serial Port Avaliable");
            }
            // Setup the plotting charts
            MainChart.ChartAreas[0].AxisX.Title = "Time";
            MainChart.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
            MainChart.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
            MainChart.ChartAreas[0].AxisY.Title = "Value Read";
            MainChart.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;
            MainChart.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Rotated270;

            //selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);

            #region Tab2 Setup the Form

            comboBox3.Items.Clear();
            for (int i = 1; i < 50; i++)
            {
                comboBox4.Items.Add(i);
            }
            comboBox4.Items.Add("Forever");
            string[] portNames2 = SerialPort.GetPortNames();
            foreach (string name in portNames2)
            {
                comboBox3.Items.Add(name);
            }

            if (portNames2.Length == 0)
            {
                MessageBox.Show("No Serial Port Avaliable");
            }

            chart1.ChartAreas[0].AxisX.Title = "Time";
            chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
            chart1.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
            chart1.ChartAreas[0].AxisY.Title = "Value Read";
            chart1.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;
            chart1.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Rotated270;

            #endregion
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            sw = new Stopwatch();
            sw.Start();
            seriesNo++;
            button3.Enabled = true;
            lines = new List<String>();
            connected = true;

            if (seriesNo == 1)
            {
                curr_series = MainChart.Series["Series" + seriesNo.ToString()];
                curr_series.ChartType = SeriesChartType.Spline;
            }
            else
            {
                // create new array
                curr_series = new Series("Series" + seriesNo.ToString(), 15);
                curr_series.ChartType = SeriesChartType.Spline;
                curr_series.MarkerColor = Color.Red;

                MainChart.Series.Add(curr_series);
            }

            if (serialPort1.IsOpen)
            {
                serialPort1.Write("S");
            }
            else
            {
                serialPort1.Open();
                serialPort1.Write("S");
            }
        }

        private void T2StartButton_Click(object sender, EventArgs e)
        {
            sw2 = new Stopwatch();
            sw2.Start();
            seriesNo2++;
            T2SaveDataButton.Enabled = true;
            lines2 = new List<String>();
            connected2 = true;

            if (seriesNo2 == 1)
            {
                curr_series2 = chart1.Series["Series" + seriesNo2.ToString()];
                curr_series2.ChartType = SeriesChartType.Spline;
            }
            else
            {
                // create new array
                curr_series2 = new Series("Series" + seriesNo2.ToString(), 15);
                curr_series2.ChartType = SeriesChartType.Spline;
                curr_series2.MarkerColor = Color.Red;

                chart1.Series.Add(curr_series2);
            }

            if (serialPort1.IsOpen)
            {
                serialPort1.Write("T");
            }
            else
            {
                serialPort1.Open();
                serialPort1.Write("T");
            }
        }

        void getAvailableComPorts()
        {
            ports = SerialPort.GetPortNames();
            ports2 = ports;
        }

        /*
        private void disconnectFromArduino()
        {
            port.Write("X");
            port.Close();
            textBox2.Text = "NewText";
            connected = false;
        }
        */

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPort2 = comboBox3.GetItemText(comboBox3.SelectedItem);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (StreamWriter writetext = new StreamWriter(fileName + "\\" + FileName.Text))
            {
                foreach (String Line in lines)
                {
                    writetext.WriteLine(Line);
                }
                
            }
        }

        private void T2SaveDataButton_Click(object sender, EventArgs e)
        {
            using (StreamWriter writetext = new StreamWriter(fileName2 + "\\" + FileName3.Text))
            {
                foreach (String Line in lines2)
                {
                    writetext.WriteLine(Line);
                }

            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            
            int choice = serialPort1.ReadByte();
            
            //if (counter%2 == 0 && connected)
            if (choice == 83 && connected)
            {
                Input = serialPort1.ReadLine();
                data = Convert.ToInt32(Input);
                var new_point = new DataPoint(sw.ElapsedMilliseconds, data);
                dataArray.Add(new_point);
                curr_series.Points.Add(new_point);
                textBox4.Text = "Time: " + Convert.ToString(sw.ElapsedMilliseconds) + ". Value = " + Convert.ToString(data);
                lines.Add("Time: " + Convert.ToString(sw.ElapsedMilliseconds) + ". Value = " + Convert.ToString(data));
                if (comboBox2.GetItemText(comboBox2.SelectedItem) != "Forever")
                {
                    if (sw.ElapsedMilliseconds / 1000 == Convert.ToInt32(comboBox2.GetItemText(comboBox2.SelectedItem)))
                    {
                        connected = false;
                        serialPort1.Write("U");
                    }
                }
            }
            //if (counter%2 == 1 && connected2)
            if (choice == 84 && connected2)
            {
                Input = serialPort1.ReadLine();
                data = Convert.ToInt32(Input);
                var new_point = new DataPoint(sw2.ElapsedMilliseconds, data);
                dataArray2.Add(new_point);
                curr_series2.Points.Add(new_point);
                textBox8.Text = "Time: " + Convert.ToString(sw2.ElapsedMilliseconds) + ". Value = " + Convert.ToString(data);
                lines2.Add("Time: " + Convert.ToString(sw2.ElapsedMilliseconds) + ". Value = " + Convert.ToString(data));
                if (comboBox4.GetItemText(comboBox4.SelectedItem) != "Forever")
                {
                    if (sw2.ElapsedMilliseconds / 1000 == Convert.ToInt32(comboBox4.GetItemText(comboBox4.SelectedItem)))
                    {
                        connected2 = false;
                        serialPort1.Write("V");
                    }
                }
            }
                counter++;
              
            if (!connected && !connected2)
            {
                serialPort1.Close();
            }
        }

        private void LocationButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if ((fbd.ShowDialog()) == System.Windows.Forms.DialogResult.OK)
            {
                fileName = fbd.SelectedPath;
            }
        }

        private void T2LocationButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if ((fbd.ShowDialog()) == System.Windows.Forms.DialogResult.OK)
            {
                fileName2 = fbd.SelectedPath;
            }
        }
    }
}
