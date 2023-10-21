using ClosedXML.Excel;
using MetroFramework.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodSharpDemo
{
    public partial class Dashboard : MetroForm
    {
        static bool lockk = false;
        //change path
        string outputFile = @"C:\cell id driver\amar\amar\GodSharpDemo\output.txt";
        string net;
        string region = "NA";
        string typ;
        string port;
        int len;
        Queue<string> queue = new Queue<string>();
        HashSet<String> scannedCellId = new HashSet<string>();
        HashSet<String> scannedMccMnc = new HashSet<string>();
        string[][] twoD = null;
        DataTable dt = new DataTable();
        //static Dictionary<String, String[]> regionCircleMap = new Dictionary<String, String[]>();
        string final = "";
        string selectedMode = "", a = "", Modetype = "";
        int round = 0;
        int delaytime = 100;
        int count2G = 0, count3G = 0, count4G = 0, count2Gchk = 0, count3Gchk = 0, count4Gchk = 0;
        string[] oneD;
        bool IsScan = true;
        DateTime start = DateTime.Now, End;
        bool MachineType;
        bool isGetoutput = false;

        public Dashboard()
        {
            InitializeComponent();
            btnConnect.Visible = true;
            btnDisconnect.Visible = false;
            btnStop.Visible = false;
            btnStart.Visible = true;
            port = srport().Trim();
            serialPort2.Close();
       
            // String mccMnc = File.ReadAllText("mcc-mnc.txt");
            string mccMnc = MNC_MCC.GetMCCMNC;
            oneD = mccMnc.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            twoD = new string[oneD.Length][];
            List<String> mccMNC = new List<string>();
            for (int i = 0; i < oneD.Length; i++)
            {
                //chages by paras
                twoD[i] = oneD[i].Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                string[] entry = new String[2];
                entry[0] = twoD[i][2].ToString().Trim();
                entry[1] = twoD[i][3].Trim();
                // System.out.println(splits[0].trim()+"-"+ splits[1].trim());
                // regionCircleMap.Add(twoD[i][0].ToString().Trim() + "-" + twoD[i][1].ToString().Trim(), entry);
            }
            dt.Columns.Add("DateTime");
            dt.Columns.Add("Circle");
            dt.Columns.Add("Operator Name");
            dt.Columns.Add("MCC");
            dt.Columns.Add("MNC");
            dt.Columns.Add("LAC");
            dt.Columns.Add("ECI");
            dt.Columns.Add("CellId");
            dt.Columns.Add("CGI");
            dt.Columns.Add("(A/E/U)RFCN");
            dt.Columns.Add("ENB");
            dt.Columns.Add("Network Type");
            dt.Columns.Add("BSIC/PSC/PCI");
            dt.Columns.Add("DBM");
            dt.Columns.Add("Net Strength");
            metroGrid1.DataSource = dt;
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
        }
        #region Start/Stop

        private void btnStart_Click(object sender, EventArgs e)
        {
            start = DateTime.Now;
            count2G = 0; count3G = 0; count4G = 0; count2Gchk = 0; count3Gchk = 0; count4Gchk = 0;
            if (DdlMode.SelectedItem != null && metroComboBox1.SelectedItem != null)
            {
                Modetype = DdlMode.SelectedItem.ToString();
                scannedCellId.Clear();
                btnStop.Visible = true;
                btnSave.Enabled = false;
                btnStart.Visible = false;
                DdlMode.Enabled = false;
                dt.Clear();
                //metroGrid1.Rows.Clear();
                metroGrid1.DataSource = null;
                metroComboBox1.Enabled = false;
                selectedMode = DdlMode.SelectedItem.ToString();
                try
                {
                    if (serialPort2.IsOpen)
                    {
                        //progressBar1.Maximum = 100;
                        //progressBar1.Step = 1;
                        //progressBar1.Value = 0;
                        //regionloader.RunWorkerAsync();
                        TypeText selectedNetwork = metroComboBox1.SelectedItem as TypeText;
                        a = selectedNetwork.Name;
                        net = selectedNetwork.Name.ToString();
                        try
                        {
                            //if (selectedNetwork.Name == "2G" || selectedNetwork.Name == "2G + 3G" || selectedNetwork.Name == "2G + 4G"
                            //   || selectedNetwork.Name == "ALL")
                            //{
                            //    Task task2a = new Task(() => scan2GNetwork());
                            //    task2a.Start();
                            //}
                            //if (selectedNetwork.Name == "3G" || selectedNetwork.Name == "2G + 3G" || selectedNetwork.Name == "ALL")
                            //{
                            //    Task task3a = new Task(() => scan3GNetwork());
                            //    task3a.Start();
                            //}
                            //if (selectedNetwork.Name == "4G" || selectedNetwork.Name == "2G + 4G" || selectedNetwork.Name == "ALL")
                            //{
                            //    Task task4a = new Task(() => scan4GNetwork());
                            //            task4a.Start();
                            //}
                            switch (selectedNetwork.Name)
                            {

                                case "2G": scan2GNetwork(); break;
                                case "3G": scan3GNetwork(); break;
                                case "4G": scan4GNetwork(); break;

                                case "2G + 3G":
                                    // await Task.WhenAll(scan2GNetwork(), scan3GNetwork());
                                    scan2GNetwork();
                                    scan3GNetwork();

                                    break;
                                // case "2G + 3G": scan2GNetwork(); scan3GNetwork(); break;
                                //  case "2G + 4G": scan2GNetwork(); scan4GNetwork(); break;
                                case "2G + 4G":
                                    // await Task.WhenAll(scan2GNetwork(),  scan4GNetwork());
                                    scan2GNetwork();
                                    scan4GNetwork();
                                   
                                    break;

                                case "ALL":
                                    //await Task.WhenAll(scan2GNetwork(), scan3GNetwork(), scan4GNetwork());
                                    //   Task.WhenAll(new[] { Task.Run(scan2GNetwork), Task.Run(scan3GNetwork), Task.Run(scan4GNetwork) });

                                    scan2GNetwork();
                                    scan3GNetwork();
                                    scan4GNetwork();
                                    break;
                                // case "ALL": scan2GNetwork(); break;
                                //scan3GNetwork(); scan4GNetwork();
                                default:
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Scan Completed");
                        }
                        //Thread.Sleep(3000);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Scan Completed");
                }
            }

            else
            {
                MessageBox.Show("Please select Type");

            }
        }
        private void scan2GNetwork()
        {
            try
            {
                outputFile = @"C:\cell id driver\amar\amar\GodSharpDemo\2goutput.txt";
                net = "2G";
                len = 10;
                //   await Task.Run(() => )
                //await Task.Run(() =>
                serialWrite("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F");//);
                waitForOutput(2);
                // await Task.Run(() =>
                serialWrite("AT+CNMP=13");//);
                waitForOutput(2);
                //await Task.Run(() => 
                serialWrite("AT+CMSSN");//);
                waitForOutput(2);
                //await Task.Run(() => 
                serialWrite("AT+CSURV");//);
                waitForOutput(2);
                //await Task.Run(() => 
                serialWrite("AT+CSURV");//);
                waitForOutput(2);

            }
            catch (Exception ex)
            {

            }

        }

        private void scan3GNetwork()
        {
            try
            {
                net = "3G";
                len = 11;
                outputFile = @"C:\cell id driver\amar\amar\GodSharpDemo\2goutput.txt";
                // await Task.Run(() => 
                serialWrite("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F");//);
                waitForOutput(2);
                //await Task.Run(() =>
                serialWrite("AT+CNMP=14");//);
                waitForOutput(2);
                //await Task.Run(() => 
                serialWrite("AT+CMSSN");//);
                waitForOutput(2);
                //await Task.Run(() =>
                serialWrite("AT+CLUARFCN");//);
                waitForOutput(2);
                //await Task.Run(() => 
                serialWrite("AT+CLUCELL");//);
                waitForOutput(3);
                //await Task.Run(() => 
                serialWrite("AT+CSNINFO?");//);
                waitForOutput(2);
                //await Task.Run(() => 
                serialWrite("AT+CSNINFO?");//);
                waitForOutput(3);
                //await Task.Run(() => 
                serialWrite("AT+CCINFO");//);
                waitForOutput(2);
                //await Task.Run(() => 
                serialWrite("AT+CCINFO");//);
                waitForOutput(2);

            }
            catch (Exception ex)
            {

            }
        }

        private void scan4GNetwork()
        {
            try
            {
                net = "4G";
                len = 15;
                outputFile = @"C:\cell id driver\amar\amar\GodSharpDemo\2goutput.txt";
                //await Task.Run(() => 
                serialWrite("AT+CNMP=38");//);
                waitForOutput(1);
                //await Task.Run(() => 
                serialWrite("AT+CMSSN");//);
                waitForOutput(1);
                string[] bandS = {"0x0000000000000001", "0x0000000000000004", "0x0000000000000010", "0x0000000000000040",
                "0x0000000000000080", "0x0000000000080000", "0x0000002000000000", "0x0000008000000000",
                "0x0000010000000000"};

                for (int i = 0; i < bandS.Length; i++)
                {
                    //File.AppendAllText(outputFile, "Initial Loop for Band : " + bandS[i] + Environment.NewLine);

                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    // await Task.Run(() =>
                    serialWrite("AT+CNBP=," + bandS[i]);//);// ;
                    waitForOutput(2);
                    //await Task.Run(() =>
                    serialWrite("AT+CSNINFO?");//);
                    waitForOutput(2);
                    //await Task.Run(() =>
                    serialWrite("AT+CMGRMI=4");//);// ;
                    waitForOutput(2);
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnDisconnect.Visible = false;
            loader.Visible = false;
            btnConnect.Visible = true;
            try
            {
                serialPort2.Close();
                dt.Clear();
                lblStatus.Text = "Status : Disconnected";
                MessageBox.Show("Connection closed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while closing connection" + ex);
                throw;
            }
            // this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            btnStop.Visible = false;
            btnSave.Enabled = true;
            btnStart.Visible = true;
            DdlMode.Enabled = true;
            metroComboBox1.Enabled = true;
        }

        #endregion

        #region Strength
        private string getNetworkStrength2G(string dbm)
        {
            int val = Int32.Parse(dbm);

            if (val == 0)
            {
                return "Poor";
            }
            else if (val >= -70)
            {
                return "Excellent";
            }
            else if (val < -70 && val >= -85)
            {
                return "Good";
            }
            else if (val < -85 && val >= -100)
            {
                return "Fair";
            }
            else if (val < -100 && val >= -110)
            {
                return "Poor";
            }
            else
            {
                return "Poor";
            }

        }

        private string getNetworkStrength4G(string dbm)
        {
            int val = Int32.Parse(dbm);

            if (val == 0)
            {
                return "Poor";
            }
            else if (val >= -80)
            {
                return "Excellent";
            }
            else if (val < -80 && val >= -90)
            {
                return "Good";
            }
            else if (val < -90 && val >= -100)
            {
                return "Fair";
            }
            else
            {
                return "Poor";
            }

        }

        #endregion
        private void getRegion()
        {
            net = "R";
            len = 10;
              outputFile = @"C:\cell id driver\amar\amar\GodSharpDemo\2goutput.txt";
            // serialWrite("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F");
            serialWrite("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F");

            serialWrite("AT+CNMP=13");

            serialWrite("AT+CMSSN");

            serialWrite("AT+CCINFO");

        }
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            btnDisconnect.Visible = false;
            loader.Visible = false;
            btnConnect.Visible = true;
            try
            {
                serialPort2.Close();
              
                dt.Clear();
                lblStatus.Text = "Status : Disconnected";
                MessageBox.Show("Connection closed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while closing connection" + ex);
                throw;
            }

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //MyMethodAsync();
            if (!serialPort2.IsOpen)
            {
                bool status = establishConnection();
                if (status)
                {
                    lblStatus.Text = "Status : Connected";
                    DdlMode.Enabled = true;
                    btnSave.Enabled = false;
                    btnDisconnect.Visible = true;
                    if (region == "NA")
                    {
                        loader.Visible = true;
                        btnConnect.Visible = false;
                        btnStart.Enabled = false;
                        metroComboBox1.Enabled = false;
                        DdlMode.Enabled = false;
                        regionloader.RunWorkerAsync();
                        //MessageBox.Show("Region Selected");                    
                    }
                }
                else
                {
                    MessageBox.Show("Machine not connected.");
                }

            }
            //if(serialPort2.IsOpen)
            //serialPort2.Write("AT+cnsvs"+Environment.NewLine);           
        }

        public bool establishConnection()
        {
            try
            {
                string[] ports = SerialPort.GetPortNames();
                //for (int h = 0; h < ports.Length; h++)
                //{
                SerialPort port = new SerialPort(ports[0]);
                // SerialPort port = new SerialPort(ports[0], 9600, Parity.Even, 8, StopBits.One);
                //SerialPort port = new SerialPort(ports[0], 115200);
                //  port.BaudRate = 115200;
                //port.DataBits = 8; port.Parity = Parity.None; port.StopBits = StopBits.One; port.Handshake = Handshake.None;
                // port.DtrEnable = true; port.NewLine = Environment.NewLine; port.ReceivedBytesThreshold = 1024; 
                try
                {
                    port.Open();
                    var mre = new AutoResetEvent(false);
                    var buffer = new StringBuilder();

                    port.DataReceived += (s, e) =>
                    {
                        buffer.Append(port.ReadExisting());
                        if (buffer.ToString().IndexOf("\r\n") >= 0)
                        {
                            Console.WriteLine("Got response: {0}", buffer);

                            mre.Set(); //allow loop to continue
                            buffer.Clear();
                        }
                    };

                }
                catch (Exception ex)
                {
                     port = new SerialPort(ports[1]);
                    port.Open();

                }
                
                //SerialPort port4 = new SerialPort(ports[4]);
                //// SerialPort port = new SerialPort(ports[0], 9600, Parity.Even, 8, StopBits.One);
                ////SerialPort port = new SerialPort(ports[0], 115200);
                //port4.BaudRate = 115200; port4.DataBits = 8; port4.Parity = Parity.None; port4.StopBits = StopBits.One; port4.Handshake = Handshake.None;
                //port4.DtrEnable = true; port4.NewLine = Environment.NewLine; port4.ReceivedBytesThreshold = 1024; port4.Open();


                // }
                //port.DataReceived += new SerialDataReceivedEventHandler(serialPort2_DataReceived);

                //port.ReadTimeout = 500;
                serialPort2.Open();

                return true;
            }
            catch (Exception e)
            {
               // SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
                return false;
                // throw e;
            }
            finally
            {
                MessageBox.Show(serialPort2.IsOpen ? "Successfully connected" : "Not connected");
            }

        }

        //void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        //{
        //    if (e.Mode != PowerModes.Resume)
        //        ports.Close();
        //}
       
        private void serialPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataRec = "";
            End = DateTime.Now;
            var result = (int)End.Subtract(start).TotalMinutes;
            //Thread.Sleep(300);
            try
            {
                // if(!string.IsNullOrEmpty( dataRec))
                dataRec = serialPort2.ReadExisting();
            }
            catch (Exception ex)
            {

            }
            if (net == "R")
            {
                string[] array1 = dataRec.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                List<Dictionary<string, string>> list1 = clean(array1);

                for (int r = 0; r < list1.Count; r++)
                {
                    if (region != "NA")
                    {
                        //MessageBox.Show("Region Selected");
                        break;
                    }
                    Dictionary<string, string> map = list1[r];
                    if (map.Count > 0 && scannedCellId.Contains(map["ID"]))
                    {
                        continue;
                    }
                    if (map.Count > 0) scannedCellId.Add(map["ID"]);
                    if (map.Count > 0)
                    {
                        for (int j = 0; j < twoD.Length; j++)
                        {
                            //if (map.Count > 0)
                            //{
                            if (twoD[j][1].Equals(map["MNC"]) && twoD[j][0].Equals(map["MCC"]))
                            {
                                region = twoD[j][3];
                                lblRegion.Invoke((MethodInvoker)delegate
                                {
                                    lblRegion.Text = "Region : " + twoD[j][3];
                                });
                                break;
                            }
                            //}
                        }
                    }
                }
                if (region == "NA")
                {
                    getRegion();
                }
                else
                {
                    loader.Invoke((MethodInvoker)delegate
                    {
                        loader.Visible = false;
                    });
                    btnStart.Invoke((MethodInvoker)delegate
                    {
                        btnStart.Enabled = true;
                    });
                    metroComboBox1.Invoke((MethodInvoker)delegate
                    {
                        //if(DdlMode.SelectedItem.ToString()=="Route")
                        metroComboBox1.Enabled = false;
                    });
                    DdlMode.Invoke((MethodInvoker)delegate
                    {
                        //if(DdlMode.SelectedItem.ToString()=="Route")
                        DdlMode.Enabled = true;
                    });
                }
            }
           //(net != "R") && result >= 5 || 

            if ((((dataRec == null) || (count2G >= 8 && net != "ALL") || (count3G >= 2) || (count4G >= 5))))
            {
                //MessageBox.Show("Scan completed");
                //btnStop.Visible = false;
                //btnSave.Enabled = true;
                //btnStart.Visible = true;
                //DdlMode.Enabled = true;
                //metroComboBox1.Enabled = false;
                //btnDisconnect.Visible = false;
                //btnConnect.Visible = true;
                try
                {
                    //this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
                    MessageBox.Show("Scan completed");
                    //btnDisconnect.Visible = false;
                    //loader.Visible = false;
                    //btnConnect.Visible = true;
                    try
                    {
                        serialPort2.Close();
                        loader.Invoke((MethodInvoker)delegate
                        {
                            loader.Visible = false;
                        });
                        btnStart.Invoke((MethodInvoker)delegate
                        {
                            btnStart.Enabled = true;
                        });


                        //lblStatus.Text = "Status : Disconnected";
                        //MessageBox.Show("Connection closed!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error while closing connection" + ex);
                        throw;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while closing connection" + ex);
                    throw;
                }

            }

            else
            {

                string[] array = dataRec.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                List<Dictionary<string, string>> list = clean(array);
              
                // if (net == "2G")
                if ((!(dataRec.Contains("CCINFO") || dataRec.Contains("ccinfo"))) && ((dataRec.Contains("arfcn:")
                        && dataRec.Contains("mcc:")) || (dataRec.Contains("ARFCN:") && dataRec.Contains("MCC:"))) 
                       && net!="R"  )
                {
                    net = "2G";
                    for (int i = 0; i < list.Count; i++)
                    {
                        Dictionary<string, string> map = list[i];
                        if (scannedCellId.Contains(map["cellId"]))
                        {
                            continue;
                        }
                        scannedCellId.Add(map["cellId"]);
                        var row = dt.NewRow();
                        for (int j = 0; j < twoD.Length; j++)
                        {
                            if (twoD[j][1].Equals(map["mnc"]) && twoD[j][0].Equals(map["mcc"]))
                            {
                                row["Circle"] = twoD[j][3];
                                row["Operator Name"] = twoD[j][2];
                                break;
                            }
                        }
                        row["DateTime"] = DateTime.Now;
                        row["MCC"] = map["mcc"];
                        row["MNC"] = map["mnc"];
                        row["LAC"] = map["lac"];
                        row["ECI"] = map["cellId"];
                        row["CellId"] = map["cellId"];
                        row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + map["lac"] + map["cellId"];
                        row["(A/E/U)RFCN"] = map["arfcn"];
                        row["ENB"] = map["dBm"];
                        row["Network Type"] = net;
                        row["BSIC/PSC/PCI"] = map["bsic"];
                        row["DBM"] = map["dBm"];
                        row["Net Strength"] = getNetworkStrength2G(map["dBm"]);
                        dt.Rows.Add(row);
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            metroGrid1.DataSource = dt;
                            count2G++;
                            metroGrid1.Update();
                            metroGrid1.Refresh();
                        // Export2Excel(dt, "2G");

                        Thread.Sleep(100);
                            try
                            {
                                string y = dataRec;
                            //if (count2G > 0 && count2Gchk > count2G && (Modetype == "Spot"))
                            //  && array[array.Length - 2].Contains("Network survey end")
                            //&& array[array.Length - 1].Contains("OK"))
                            //{
                            //if (a != "ALL")
                            //{
                            //    MessageBox.Show("Scan completed");
                            //    btnStop.Visible = false;
                            //    btnSave.Enabled = true;
                            //    btnStart.Visible = true;
                            //    DdlMode.Enabled = true;
                            //    metroComboBox1.Enabled = false;
                            //    btnDisconnect.Visible = false;
                            //    btnConnect.Visible = true;
                            //    btnDisconnect.Visible = false;
                            //    btnConnect.Visible = true;
                            //    this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
                            //    try
                            //    {
                            //        //serialPort2.Close();
                            //        lblStatus.Text = "Status : Disconnected";
                            //        MessageBox.Show("Connection closed!");
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        MessageBox.Show("Error while closing connection" + ex);
                            //        throw;
                            //    }
                            //}
                            //else
                            //{
                            //    if (a == "ALL" || a == "2G + 3G")
                            //    {

                            //        Thread.Sleep(500);
                            //        scan3GNetwork();
                            //    }
                            //    else
                            //    {
                            //        if (a == "2G + 4G")
                            //        {
                            //            scan4GNetwork();
                            //        }
                            //    }
                            //}
                            // }
                        }
                            catch (Exception ex)
                            {

                            }

                        }));
                        //chages by paras for other region
                        for (int j = 0; j < twoD.Length; j++)
                        {
                            if (twoD[j][3].Equals(row["Circle"]))// && (map["mcc"] + map["mnc"]) == (twoD[j][0] + twoD[j][1]))//&&!scannedCellId.Contains(row["CellId"]))
                            {
                                try
                                {
                                    //count2Gchk++;
                                    var index = Array.FindAll(oneD, s => s.Contains(row["Circle"].ToString()));

                                    // int[] allIndexes = twoD.FindIndexes(s => s.Contains(value)).ToArray();
                                    scannedMccMnc.Add(twoD[j][0] + twoD[j][1]);
                                    //  await Task.Run(() => 
                                    serialWrite("AT+CMSSN=" + twoD[j][0] + twoD[j][1]);//);
                                    waitForOutput(2);
                                    // serialWrite("AT+CMSSN=" + twoD[j][0] + twoD[j][1]);
                                    //  await Task.Run(() => serialWrite("AT+CMSSN=" + map["mcc"] + map["mnc"] + "\r\n"));

                                    //await Task.Run(() => 
                                    serialWrite("AT+CSURV");//);
                                    waitForOutput(2);
                                    // await Task.Run(() =>
                                    serialWrite("AT+CSURV");//) ; ;
                                    waitForOutput(2);

                                }
                                catch (Exception ex)
                                {

                                }
                            }

                        }
                        serialWrite("AT+CMSSN");
                        Thread.Sleep(100);
                    }

                }

                //if (net == "3G")
                if ( net != "R" &&(dataRec.Contains("CSNINFO") && (dataRec.Contains("SCELL")) ||
                    ((dataRec.Contains("arfcn:")
                        && dataRec.Contains("mcc:")) || (dataRec.Contains("ARFCN:") && dataRec.Contains("MCC:"))))
                    )
                {
                    net = "3G";
                    if (typ == "csn")
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            try
                            {
                                Dictionary<string, string> map = list[i];

                                String[] vals = map["3"].Split('-');
                                string cgi = vals[0] + "-" + vals[1] + "-" + map["4"] + "-" + map["5"]; // CGI = MCC + MNC + LAC + ID
                                if (scannedCellId.Contains(cgi))
                                {
                                    continue;
                                }

                                scannedCellId.Add(cgi); //cellID
                                var row = dt.NewRow();
                                for (int j = 0; j < twoD.Length; j++)
                                {
                                    if (twoD[j][1].Equals(vals[1]) && twoD[j][0].Equals(vals[0])) //mnc & mcc
                                    {
                                        row["Circle"] = twoD[j][3];
                                        row["Operator Name"] = twoD[j][2];
                                        break;
                                    }
                                }
                                row["DateTime"] = DateTime.Now;

                                row["MCC"] = vals[0];
                                row["MNC"] = vals[1];
                                row["LAC"] = map["4"];
                                row["ECI"] = map["5"];
                                row["CellId"] = map["5"];
                                row["CGI"] = cgi; //cgi
                                row["(A/E/U)RFCN"] = map["8"];
                                row["ENB"] = "NA";
                                row["Network Type"] = net;
                                row["BSIC/PSC/PCI"] = map["7"];
                                row["DBM"] = "-" + map["11"];
                                row["Net Strength"] = getNetworkStrength2G(map["13"]);

                                dt.Rows.Add(row);
                                this.Invoke(new MethodInvoker(delegate ()
                                {
                                    metroGrid1.DataSource = dt;
                                    metroGrid1.Update();
                                    metroGrid1.Refresh();
                                    count3G++;
                                    Thread.Sleep(100);

                                }));

                                for (int j = 0; j < twoD.Length; j++)
                                {
                                    if (twoD[j][3].Equals(row["Circle"]))//&&!scannedCellId.Contains(row["CellId"]))
                                    {
                                        // count3Gchk++;
                                        var index = Array.FindAll(oneD, s => s.Contains(row["Circle"].ToString()));
                                        // count3G = index.Count();
                                        scannedMccMnc.Add(twoD[j][0] + twoD[j][1]);
                                        // await Task.Run(() =>
                                        serialWrite("AT+CMSSN=" + twoD[j][0] + twoD[j][1]);//);
                                        waitForOutput(2);
                                        serialWrite("AT+CLUARFCN");
                                        waitForOutput(2);
                                        serialWrite("AT+CLUCELL");
                                        waitForOutput(2);
                                        serialWrite("AT+CSNINFO?");
                                        waitForOutput(2);
                                        serialWrite("AT+CCINFO");
                                        waitForOutput(2);
                                    }
                                }
                                serialWrite("AT+CMSSN");
                                waitForOutput(2);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        try
                        {

                            if ((Modetype == "Spot") && array.Length > 2
                                && (array[0].Contains("CSNINFO") || array[1].Contains("CSNINFO"))
                                && (array[0].Contains("SCELL") || array[1].Contains("SCELL")) && array[array.Length - 1].Contains("OK"))
                            {
                                if (a != "ALL")
                                {
                                    //MessageBox.Show("Scan completed");
                                    //btnStop.Visible = false;
                                    //btnSave.Enabled = true;
                                    //btnStart.Visible = true;
                                    //DdlMode.Enabled = true;
                                    //metroComboBox1.Enabled = false;
                                    //btnDisconnect.Visible = false;
                                    //btnConnect.Visible = true;
                                    //try
                                    //{
                                    //    lblStatus.Text = "Status : Disconnected";
                                    //    MessageBox.Show("Connection closed!");
                                    //    this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    MessageBox.Show("Error while closing connection" + ex);
                                    //    throw;
                                    //}
                                }
                                //else
                                //{
                                //    if (a == "ALL")
                                //    {
                                //        scan4GNetwork();
                                //    }
                                //}
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dictionary<string, string> map = list[i];
                            string cgi = map["MCC"] + "-" + map["MNC"] + "-" + map["LAC"] + "-" + map["ID"];
                            if (scannedCellId.Contains(cgi))
                            {
                                continue;
                            }
                            scannedCellId.Add(cgi);
                            var row = dt.NewRow();
                            for (int j = 0; j < twoD.Length; j++)
                            {
                                if (twoD[j][1].Equals(map["MNC"]) && twoD[j][0].Equals(map["MCC"]))
                                {
                                    row["Circle"] = twoD[j][3];
                                    row["Operator Name"] = twoD[j][2];
                                    break;
                                }
                            }
                            row["DateTime"] = DateTime.Now;

                            row["MCC"] = map["MCC"];
                            row["MNC"] = map["MNC"];
                            row["LAC"] = map["LAC"];
                            row["ECI"] = map["ID"];
                            row["CellId"] = map["ID"];
                            row["CGI"] = cgi;
                            row["(A/E/U)RFCN"] = map["UARFCN"];
                            row["ENB"] = "NA";
                            row["Network Type"] = net;
                            row["BSIC/PSC/PCI"] = map["PSC"];
                            row["DBM"] = map["RXLev"].Split('d')[0];
                            row["Net Strength"] = getNetworkStrength2G(map["RXLev"].Split('d')[0]);

                            dt.Rows.Add(row);
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                metroGrid1.DataSource = dt;
                                metroGrid1.Update();
                                metroGrid1.Refresh();
                                count3G++;
                                Thread.Sleep(100);

                            }));
                            for (int j = 0; j < twoD.Length; j++)
                            {
                                if (twoD[j][3].Equals(row["Circle"]))//&&!scannedCellId.Contains(row["CellId"]))
                                {
                                    try
                                    {
                                        // count3Gchk++;
                                        var index = Array.FindAll(oneD, s => s.Contains(row["Circle"].ToString()));
                                        // count3G = index.Count();

                                        scannedMccMnc.Add(twoD[j][0] + twoD[j][1]);
                                        //await Task.Run(() => 
                                        serialWrite("AT+CMSSN=" + twoD[j][0] + twoD[j][1]);//);
                                        waitForOutput(2);
                                        serialWrite("AT+CLUARFCN");
                                        waitForOutput(2);
                                        serialWrite("AT+CLUCELL");
                                        waitForOutput(2);
                                        serialWrite("AT+CSNINFO?");
                                        waitForOutput(2);
                                        serialWrite("AT+CCINFO");
                                        waitForOutput(2);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                            }
                            //await Task.Run(() => 
                            serialWrite("AT+CMSSN");//);
                            waitForOutput(2);
                        }
                        try
                        {
                            if ( net != "R" && (Modetype == "Spot") && array.Length > 2 && array[0].Contains("CSNINFO") && array[0].Contains("SCELL")
                                && array[0].Contains("SCELL") && array[array.Length - 1].Contains("OK"))

                            {
                                if (a != "ALL")
                                {
                                    //MessageBox.Show("Scan completed");
                                    //btnStop.Visible = false;
                                    //btnSave.Enabled = true;
                                    //btnStart.Visible = true;
                                    //DdlMode.Enabled = true;
                                    //metroComboBox1.Enabled = false;
                                    //btnDisconnect.Visible = false;
                                    //btnConnect.Visible = true;
                                    //try
                                    //{
                                    //    this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
                                    //    lblStatus.Text = "Status : Disconnected";
                                    //    MessageBox.Show("Connection closed!");
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    MessageBox.Show("Error while closing connection" + ex);
                                    //    throw;
                                    //}
                                }
                                //else
                                //{
                                //    if (a == "ALL")
                                //    {
                                //        scan4GNetwork();
                                //    }
                                //}
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                if ( net != "R" && (dataRec.Contains("Serving_Cell") || dataRec.Contains("CSNINFO")) 
                    )
                {
                    net = "4G";

                    if (typ == "csn")
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dictionary<string, string> map = list[i];
                            String[] vals = map["3"].Split('-');
                            string cgi = vals[0] + "-" + vals[1] + "-" + map["4"] + "-" + map["5"]; // CGI = MCC + MNC + LAC + ID
                            if (scannedCellId.Contains(cgi))
                            {
                                continue;
                            }
                            scannedCellId.Add(cgi); //cellID
                            var row = dt.NewRow();
                            for (int j = 0; j < twoD.Length; j++)
                            {
                                if (twoD[j][1].Equals(vals[1]) && twoD[j][0].Equals(vals[0])) //mnc & mcc
                                {
                                    row["Circle"] = twoD[j][3];
                                    row["Operator Name"] = twoD[j][2];
                                    break;
                                }
                            }
                            row["DateTime"] = DateTime.Now;
                            row["MCC"] = vals[0];
                            row["MNC"] = vals[1];
                            row["LAC"] = map["4"];
                            row["ECI"] = map["6"];
                            row["CellId"] = map["5"];
                            row["CGI"] = cgi; //cgi
                            row["(A/E/U)RFCN"] = map["8"];
                            row["ENB"] = "NA";
                            row["Network Type"] = net;
                            row["BSIC/PSC/PCI"] = map["11"];
                            row["DBM"] = map["12"];
                            row["Net Strength"] = getNetworkStrength4G(map["13"]);

                            dt.Rows.Add(row);
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                metroGrid1.DataSource = dt;
                                metroGrid1.Update();
                                count4G++;
                                metroGrid1.Refresh();
                                Thread.Sleep(100);

                            }));
                            for (int j = 0; j < twoD.Length; j++)
                            {
                                if (twoD[j][3].Equals(row["Circle"]))//&&!scannedCellId.Contains(row["CellId"]))
                                {
                                    //count4Gchk++;
                                    var index = Array.FindAll(oneD, s => s.Contains(row["Circle"].ToString()));
                                    //count4G = index.Count();
                                    dataRec = Environment.NewLine + "Data_Received 1 Loop for Output From : IF" + Environment.NewLine;

                                    scannedMccMnc.Add(twoD[j][0] + twoD[j][1]);
                                    serialWrite("AT+CMSSN=" + twoD[j][0] + twoD[j][1]);

                                    waitForOutput(2);
                                    string[] bandS = {"0x0000000000000001", "0x0000000000000004", "0x0000000000000010", "0x0000000000000040",
                "0x0000000000000080", "0x0000000000080000", "0x0000002000000000", "0x0000008000000000",
                "0x0000010000000000"};

                                    for (int x = 0; x < bandS.Length; x++)
                                    {
                                        if (!serialPort2.IsOpen)
                                        {
                                            MessageBox.Show("Device is not connected.Scan will stop");
                                            return;
                                        }

                                        dataRec = Environment.NewLine + "Data_Received 2 Loop for Output From : IF " + Environment.NewLine;
                                        serialWrite("AT+CNBP=," + bandS[x]);
                                        waitForOutput(2);
                                        serialWrite("AT+CSNINFO?");
                                        waitForOutput(2);
                                        serialWrite("AT+CMGRMI=4");
                                        waitForOutput(2);
                                    }

                                }
                            }
                            serialWrite("AT+CMSSN");
                            Thread.Sleep(50);
                        }

                        try
                        {
                            if (a != "ALL" && (Modetype == "Spot") && array.Length >= 2 && (array[1].Contains("Serving_Cell") || array[2].Contains("Serving_Cell"))
                            && (array[2].Contains("LTE_Intra") || array[3].Contains("LTE_Intra")) && array[array.Length - 1].Contains("OK"))
                            {
                                //MessageBox.Show("Scan completed");
                                //btnStop.Visible = false;
                                //btnSave.Enabled = true;
                                //btnStart.Visible = true;
                                //DdlMode.Enabled = true;
                                //metroComboBox1.Enabled = false;
                                //btnDisconnect.Visible = false;
                                //btnConnect.Visible = true;
                                //try
                                //{
                                //    this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);

                                //    lblStatus.Text = "Status : Disconnected";
                                //    MessageBox.Show("Connection closed!");
                                //}
                                //catch (Exception ex)
                                //{
                                //    MessageBox.Show("Error while closing connection" + ex);
                                //    throw;
                                //}
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dictionary<string, string> map = list[i];
                            string cgi = map["2"] + "-" + map["3"] + "-" + map["4"] + "-" + map["5"]; // CGI = MCC + MNC + LAC + ID
                            if (scannedCellId.Contains(cgi))
                            {
                                continue;
                            }
                            scannedCellId.Add(cgi); //cellID
                            var row = dt.NewRow();
                            for (int j = 0; j < twoD.Length; j++)
                            {
                                if (twoD[j][1].Equals(map["3"]) && twoD[j][0].Equals(map["2"])) //mnc & mcc
                                {
                                    row["Circle"] = twoD[j][3];
                                    row["Operator Name"] = twoD[j][2];
                                    break;
                                }
                            }
                            row["DateTime"] = DateTime.Now;

                            row["MCC"] = map["2"];
                            row["MNC"] = map["3"];
                            row["LAC"] = map["4"];
                            row["ECI"] = map["5"];
                            row["CellId"] = map["5"];
                            row["CGI"] = cgi; //cgi
                            row["(A/E/U)RFCN"] = map["8"];
                            row["ENB"] = "NA";
                            row["Network Type"] = net;
                            row["BSIC/PSC/PCI"] = map["7"];
                            row["DBM"] = map["11"];
                            try
                            {
                                row["Net Strength"] = getNetworkStrength4G(map["13"]);
                            }
                            catch (Exception ex)
                            {

                            }

                            dt.Rows.Add(row);
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                metroGrid1.DataSource = dt;
                                metroGrid1.Update();
                                metroGrid1.Refresh();
                                count4G++;
                                waitForOutput(2);

                            }));
                            for (int j = 0; j < twoD.Length; j++)
                            {
                                if (twoD[j][3].Equals(row["Circle"]))//&&!scannedCellId.Contains(row["CellId"]))
                                {
                                    // count4Gchk++;
                                    var index = Array.FindAll(oneD, s => s.Contains(row["Circle"].ToString()));
                                    // count4G = index.Count();
                                    dataRec = Environment.NewLine + "Data_Received 1 Loop for Output From : Else" + Environment.NewLine;


                                    scannedMccMnc.Add(twoD[j][0] + twoD[j][1]);

                                    //  await Task.Run(() => 
                                    serialWrite("AT+CMSSN=" + twoD[j][0] + twoD[j][1]);//);
                                    waitForOutput(2);
                                    //Task.Delay(delaytime).Wait();

                                    string[] bandS = {"0x0000000000000001", "0x0000000000000004", "0x0000000000000010", "0x0000000000000040",
                "0x0000000000000080", "0x0000000000080000", "0x0000002000000000", "0x0000008000000000",
                "0x0000010000000000"};

                                    for (int x = 0; x < bandS.Length; x++)
                                    {
                                        if (!serialPort2.IsOpen)
                                        {
                                            MessageBox.Show("Device is not connected.Scan will stop");
                                            return;
                                        }

                                        dataRec = Environment.NewLine + "Data_Received 2 Loop for Output From : Else" + Environment.NewLine;

                                        //  await Task.Run(() => 
                                        serialWrite("AT+CNBP=," + bandS[x]);//);
                                        waitForOutput(2);
                                        //await Task.Run(() =>
                                        serialWrite("AT+CSNINFO?");//); ;
                                        waitForOutput(2);
                                        //await Task.Run(() => 
                                        serialWrite("AT+CMGRMI=4");//);
                                        waitForOutput(2);
                                    }

                                }
                            }

                            // await Task.Run(() => 
                            serialWrite("AT+CMSSN");//);
                            Thread.Sleep(100);

                        }
                        try
                        {
                            if (a != "ALL" && (Modetype == "Spot") && array.Length >= 3 && (array[1].Contains("Serving_Cell") || array[2].Contains("Serving_Cell"))
                            && (array[2].Contains("LTE_Intra") || array[3].Contains("LTE_Intra")) && array[array.Length - 1].Contains("OK"))
                            {
                                //MessageBox.Show("Scan completed");
                                //btnStop.Visible = false;
                                //btnSave.Enabled = true;
                                //btnStart.Visible = true;
                                //DdlMode.Enabled = true;
                                //metroComboBox1.Enabled = false;
                                //btnDisconnect.Visible = false;
                                //btnConnect.Visible = true;
                                //try
                                //{
                                //    this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
                                //    lblStatus.Text = "Status : Disconnected";
                                //    MessageBox.Show("Connection closed!");
                                //}
                                //catch (Exception ex)
                                //{
                                //    MessageBox.Show("Error while closing connection" + ex);
                                //    throw;
                                //}                            //MessageBox.Show("Scan completed");
                                //btnStop.Visible = false;
                                //btnSave.Enabled = true;
                                //btnStart.Visible = true;
                                //DdlMode.Enabled = true;
                                //metroComboBox1.Enabled = false;
                                //btnDisconnect.Visible = false;
                                //btnConnect.Visible = true;
                                //try
                                //{
                                //    this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
                                //    lblStatus.Text = "Status : Disconnected";
                                //    MessageBox.Show("Connection closed!");
                                //}
                                //catch (Exception ex)
                                //{
                                //    MessageBox.Show("Error while closing connection" + ex);
                                //    throw;
                                //}
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            if (region != "NA")
            {
                try
                {
                    File.AppendAllText(outputFile, dataRec);
                }
                catch (Exception ex)
                {

                }
            }

        }
        public  void waitForOutput(int seconds)
        {
            try
            {
                int counter = 0;
                while (counter < seconds)
                {
                    if (isGetoutput)
                        break;
                    Thread.Sleep(1000);
                    counter++;
                }
                //if (!isGetoutput)(counter == seconds)
                //{
                //    MessageBox.Show("Counter has reset due to timeout");
                //}
            }
            catch (Exception ex)
            {
            }
        }
        void serialWrite(string cmd)
        {
            try
            {
                if (cmd != null)
                    serialPort2.Write(cmd + Environment.NewLine);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            if (cmd != null)
                queue.Enqueue(cmd);
            lock (this)
            {
                if (lockk == false && queue.Count > 0)
                {
                    try
                    {
                        serialPort2.Write(queue.Dequeue() + Environment.NewLine);
                        lockk = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                //try
                //{
                //    final = final + " " + queue.Dequeue().ToString();
                //    if (final.Contains("ok") || final.Contains("ok"))
                //    {
                //        MessageBox.Show("ok");

                //    }
                //}
                //catch (Exception ex)
                //{

                //}
            }

        }
        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (serialPort2.IsOpen)
                    serialPort2.Close();
            }
            catch (Exception ee)
            {
                throw;
            }
        }
        public Dictionary<String, string> dataCleaner(String data)
        {
            int xm = 0;
            string info = "false";
            typ = "csn";
            Dictionary<String, String> map = new Dictionary<string, string>();
            if (data.Contains("+CCINFO:"))
            {
                info = "true";
                typ = "ccin";
            }
            if (data.Contains("+CMGRMI:"))
            {
                typ = "cmg";
                len = 15;
            }
            String[] datas = data.Split(new char[] { ',' });
            if (datas.Length < len)
                return null;
            foreach (var val in datas)
            {
                if (string.IsNullOrEmpty(val))
                    continue;
                if (net == "R")
                {
                    String[] vals = val.Split(':');

                    if (!map.ContainsKey(vals[0]))
                    {
                        try

                        {
                            string a, b;
                            string col1, col2;

                            a = vals[0].Replace("{[", "");
                            col1 = a.Replace("]}", "");

                            b = vals[1].Replace("{[", "");
                            col2 = b.Replace("]}", "");

                            map.Add(a.Trim(), b.Trim());
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }
                if (net == "2G")
                {
                    String[] vals = val.Split(':');

                    if (!map.ContainsKey(vals[0]))
                    {
                        try
                        {
                            map.Add(vals[0].Trim(), vals[1].Trim());
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                if (net == "3G")
                {
                    try
                    {
                        if (info == "false")
                        {
                            map.Add(xm.ToString(), val.Trim());
                            xm++;
                        }
                        else
                        {
                            String[] vals = val.Split(':');

                            if (!map.ContainsKey(vals[0]))
                                map.Add(vals[0].Trim(), vals[1].Trim());
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                if (net == "4G")
                {
                    try
                    {
                        if (typ == "csn")
                        {
                            map.Add(xm.ToString(), val.Trim());
                            xm++;
                        }
                        else
                        {
                            if (datas[2] == "65535" || datas[2] == "0")
                            {
                                return null;
                            }
                            else
                            {
                                map.Add(xm.ToString(), val.Trim());
                                xm++;
                            }
                        }
                    }
                    catch (Exception ex) { }

                }

            }
            return map;
        }
        public List<Dictionary<string, string>> clean(String[] lines)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> dict;
            foreach (var line in lines)
            {
                if (line.Trim().Contains("OK"))
                {
                    lockk = false;
                    isGetoutput = true;
                    serialWrite(null);
                    break;
                }

                if (line.Contains("Network survey end") && selectedMode == "Spot")
                {
                    //count2G++;

                }

                else if (line.Contains("ERROR"))
                {
                    //MessageBox.Show("Error");
                    lockk = false;
                    serialWrite(null);
                    break;
                }
                if (line.Trim().Contains("Loop"))
                {
                    lockk = false;
                    serialWrite(null);
                    break;
                }
                dict = dataCleaner(line);
                if (dict != null)
                {
                    list.Add(dict);
                }
            }
            return list;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //if (net != "")
            //{
            //    var backgroundWorker = sender as BackgroundWorker;
            //    ////for (int j = 0; j < 100000; j++)
            //    ////{
            //    ////    double pow = Math.Pow(j, j);
            //    ////    backgroundWorker.ReportProgress((j * 100) / 100000);
            //    ////}
            //}
            lblRegion.Invoke((MethodInvoker)delegate
            {
                lblRegion.Text = "Region : Searching Region ...";
            });
            if (region == "NA")
            {
                getRegion();
            }
        }
        private void regionloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void regionloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                // MessageBox.Show("Region Selected");
                serialPort2.Close();

                serialPort2.Open();
            }
            catch (Exception ex)
            {

            }
        }
        private string srport()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startinfo = new System.Diagnostics.ProcessStartInfo();
            startinfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startinfo.FileName = "cmd.exe";
            // string newStr = "\"" + "AT Port" + "\"";
            // string s = @"/c wmic path win32_pnpentity get caption /format:table |find ""AT Port""";
            // newStr = "/c wmic path win32_pnpentity get caption /format:table |find " + newStr;
            //startinfo.Arguments = "/c wmic path win32_pnpentity get caption /format:table |find " + "''"AT Port"";
            //startinfo.Arguments = String.Concat("/c wmic path win32_pnpentity get caption /format:table |find ", "\"", "AT Port");
            //string txt = (@"""Add doublequotes""").Replace("\\", ""); 
            startinfo.Arguments = "/c wmic path win32_pnpentity get caption /format:table  <NUL";
            process.StartInfo = startinfo;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string ot = process.StandardOutput.ReadToEnd();
            // File.AppendAllText(outputFile, ot);
            string port = "";
            //changes for portfind
            MachineType = ot.Contains("Quectel USB Modem");
            string[] readtext = ot.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            int indexes = Array.FindIndex(readtext, element => element.Contains("AT Port"));
            if (indexes > 1)
            {
                string portText = readtext[indexes];

                //changes for portfind
                string subport = portText.Substring(portText.LastIndexOf('(') + 1, portText.LastIndexOf(')'));
                port = subport.Replace(")", "");

                this.serialPort2.PortName = port;
            }

            return port;
        }

        private void DdlMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<TypeText> TypeList = new List<TypeText>();
            TypeList.Clear();
            // wmic path win32_pnpentity get caption /format:table |find "AT Port"
            if ((DdlMode.SelectedItem.ToString() == "Route")) //|| (DdlMode.SelectedItem.ToString() == "Spot"))
            {
                TypeList.Add(new TypeText { Name = "Select" });
                TypeList.Add(new TypeText { Name = "2G" });
                TypeList.Add(new TypeText { Name = "3G" });
                TypeList.Add(new TypeText { Name = "4G" });
                metroComboBox1.Enabled = true;
                metroComboBox1.DataSource = TypeList;
                metroComboBox1.DisplayMember = "Name";
            }
            else if ((DdlMode.SelectedItem.ToString() == "Spot")) //|| (DdlMode.SelectedItem.ToString() == "Spot"))
            {
                TypeList.Add(new TypeText { Name = "Select" });
                TypeList.Add(new TypeText { Name = "ALL" });
                TypeList.Add(new TypeText { Name = "2G" });
                TypeList.Add(new TypeText { Name = "3G" });
                TypeList.Add(new TypeText { Name = "4G" });
                TypeList.Add(new TypeText { Name = "2G + 3G" });
                TypeList.Add(new TypeText { Name = "2G + 4G" });
                metroComboBox1.Enabled = true;
                metroComboBox1.DataSource = TypeList;
                metroComboBox1.DisplayMember = "Name";
                metroComboBox1.ValueMember = "Name";
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // End = DateTime.Now;
            //var result = (int)End.Subtract(start).TotalMinutes;
            //if(result>=5)
            //{


            //}
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    Export2Excel(dt, net);
                }
            }


        }



        public void Export2Excel(DataTable dt, string fileName)
        {
            try
            {
                DialogResult dr = sfdExcel.ShowDialog();
                // wmic path win32_pnpentity get caption /format:table |find "AT Port"
                //Exporting to Excel
                string folderPath = sfdExcel.FileName;
                //if (!Directory.Exists(folderPath))
                //{
                //    Directory.CreateDirectory(folderPath);
                //}
                using (XLWorkbook wb = new XLWorkbook())
                {
                    //for (int row = 0; row < dt.Rows.Count; row++)
                    //{
                    wb.Worksheets.Add(dt, fileName);
                    //  }

                    wb.SaveAs(folderPath);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void metroComboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            TypeText selectedPerson = metroComboBox1.SelectedItem as TypeText;
            String a = selectedPerson.Name;
        }


        private async Task MyMethodAsync()
        {
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("key", "62220182b8deb");
            try
            {
                var result = await PostHTTPRequestAsync("https://msg.ccas.in/api/cellId/productKey", postData);
                Console.WriteLine(result);
                string[] oneD = result.Split(new string[] { "/", "/" }, StringSplitOptions.RemoveEmptyEntries);
                string json_data = JsonConvert.SerializeObject(result);
                // tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonTokenResponse);
                var response = JsonConvert.DeserializeObject<ProductKeyValidation>(result);
                //  var deptList = JsonSerializer.Deserialize<IList<ProductKeyValidation>>(response);
                string h = response.error;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static readonly HttpClient client = new HttpClient();
        private async Task<string> PostHTTPRequestAsync(string url, Dictionary<string, string> data)
        {
            using (HttpContent formContent = new FormUrlEncodedContent(data))
            {
                using (HttpResponseMessage response = await client.PostAsync(url, formContent).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }
    }

}
//public class ProductKeyValidation
//{
//    public string status { get; set; }
//    public string error { get; set; }
//}

public class TypeText
{
    public string Name { get; set; }

}