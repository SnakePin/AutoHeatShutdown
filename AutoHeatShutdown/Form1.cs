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

        int pcShutdownTemp = 100;
        Computer thisComputer;
        List<int> packageTempAvarage = new List<int>();
        private void Form1_Load(object sender, EventArgs e)
        {

            thisComputer = new Computer() { CPUEnabled = true };
            thisComputer.Open();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IHardware hardwareItem = thisComputer.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.CPU);
            hardwareItem.Update();

            int packageTemp = (int)hardwareItem.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Temperature && x.Name == "CPU Package").Value.Value;

            packageTempAvarage.Add(packageTemp);

            if (packageTempAvarage.Count > 10)
            {
                while (packageTempAvarage.Count != 10)
                {
                    packageTempAvarage.RemoveAt(0);
                }
            }

            temperatureLabel.Text = ((int)packageTempAvarage.Average()).ToString() + "℃";

            if ((int)packageTempAvarage.Average() >= pcShutdownTemp && packageTempAvarage.Count == 10)
            {
                Console.WriteLine("Auto shutdown commencing..");
                ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/s /t 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi);
            }
        }

        private void setButton_Click(object sender, EventArgs e)
        {
            pcShutdownTemp = (int)pcShutdownTempNumeric.Value;
        }
    }
}
