using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace AutoHeatShutdown
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int packageTemp = 0;
        int pcShutdownTemp = 100;

        Computer myComputer;
        private void Form1_Load(object sender, EventArgs e)
        {

            myComputer = new Computer() { CPUEnabled = true };
            myComputer.Open();


        }
        List<int> packageTempAvarage = new List<int>();
        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var hardwareItem in myComputer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.CPU)
                {
                    hardwareItem.Update();

                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name == "CPU Package")
                        {
                            packageTemp = (int)sensor.Value.Value;

                        }
                    }
                }
            }

            packageTempAvarage.Add(packageTemp);

            if(packageTempAvarage.Count > 10)
                packageTempAvarage.RemoveAt(0);

            label3.Text = ((int)packageTempAvarage.Average()).ToString() + "℃";

            if ((int)packageTempAvarage.Average() > pcShutdownTemp && packageTempAvarage.Count >= 9)
            {
                var psi = new ProcessStartInfo("shutdown", "/s /t 0");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process.Start(psi);
                Console.WriteLine("Auto shutdown commencing..");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pcShutdownTemp = (int)numericUpDown1.Value;
        }
    }
}
