using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;


namespace UART_FORM_NETlib
{
    public partial class Form1 : Form
    {
        private Thread cpuThread;
        public Form1()
        {
            InitializeComponent();
        }
        string dataOUT;
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cbCOMPORT.Items.AddRange(ports);

            // Set title.
            chart1.Titles.Add("Line Chart");
            chart1.ChartAreas[0].AxisX.Minimum = 0;
  

        }

        private void getData()
        {
            while(true)
            {
                //X
                Data.CirArrayX[Data.CirArrayX.Length - 1] = Math.Round(Convert.ToDouble(Data.data[0]), 0);
                Array.Copy(Data.CirArrayX, 1, Data.CirArrayX, 0, Data.CirArrayX.Length - 1);
                //Y
                Data.CirArrayY[Data.CirArrayY.Length - 1] = Math.Round(Convert.ToDouble(Data.data[1]), 0);
                Array.Copy(Data.CirArrayY, 1, Data.CirArrayY, 0, Data.CirArrayY.Length - 1);
                //Z
                Data.CirArrayZ[Data.CirArrayZ.Length - 1] = Math.Round(Convert.ToDouble(Data.data[2]), 0);
                Array.Copy(Data.CirArrayZ, 1, Data.CirArrayZ, 0, Data.CirArrayZ.Length - 1);

                //update
                if (chart1.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateChart(); });
                }

                Thread.Sleep(100);
            }

        }
        private void UpdateChart()
        {
            chart1.Series["X"].Points.Clear();
            chart1.Series["Y"].Points.Clear();
            chart1.Series["Z"].Points.Clear();
            for (int i=0 ; i < Data.CirArrayX.Length - 1; ++i)
            {
                chart1.Series["X"].Points.AddY(Data.CirArrayX[i]);
                chart1.Series["Y"].Points.AddY(Data.CirArrayY[i]);
                chart1.Series["Z"].Points.AddY(Data.CirArrayZ[i]);
            }


        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cbCOMPORT.Text;
                serialPort1.BaudRate = Convert.ToInt32(cbBAUDRATE.Text);
                serialPort1.DataBits = Convert.ToInt32(8);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One");
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), "None");

                serialPort1.Open();
                progressBar1.Value = 100;
                timer1.Start();


            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen)
            {
                serialPort1.Close();
                timer1.Stop();
;
                progressBar1.Value = 0;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen)
            {
                dataOUT = txtData.Text;
                serialPort1.WriteLine(dataOUT);
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Data.serialRAW[Data.RingCounter] = serialPort1.ReadExisting();
            //Data.RingCounter++;
            //if (Data.RingCounter == 100)
            //{
            //    Data.RingCounter = 0;
            //}

            if(serialPort1.BytesToRead >= 7)
            {
                int bytes = serialPort1.BytesToRead;
                byte[] buffer = new byte[bytes];
                serialPort1.Read(buffer, 0, bytes);
                if (buffer[6] == 171)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        byte temp = buffer[i * 2 + 1];
                        buffer[i * 2 + 1] = buffer[i * 2];
                        buffer[i * 2] = temp;
                    }

                    for (int i = 0; i < Data.data.Length; i++)
                        Data.data[i] = BitConverter.ToInt16(buffer, i * 2);
                }
                else
                {
                   for (int i = 0; i < Data.data.Length; i++)
                    Data.data[i] = 0;
                }



            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Data.serialRAW++;
            if (Data.serialRAW>1000)
            {
                Data.serialRAW = 500;
            }
        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            cpuThread = new Thread(new ThreadStart(getData));
            cpuThread.IsBackground = true;
            cpuThread.Start();

            //var sevenItems = new byte[] { 0x13, 0x45, 0x73, 0x32, 0x42, 0x34, 0xab };

            //if (sevenItems[6] == 171)
            //{
            //    for (int i = 0; i < 3; i++)
            //    {
            //        byte temp = sevenItems[i * 2 + 1];                
            //        sevenItems[i * 2 + 1] = sevenItems[i * 2]; 
            //        sevenItems[i * 2] = temp;
            //    }


            //    for (int i = 0; i < Data.datatest.Length; i++)
            //        Data.datatest[i] = BitConverter.ToInt16(sevenItems, i * 2);
            //}
            //txtData.Text = Convert.ToString(Data.datatest[0]);

        }
    }
}
