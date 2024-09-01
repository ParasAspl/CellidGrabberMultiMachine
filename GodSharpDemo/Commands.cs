using CligenceCellIDGrabber;
using ClosedXML.Excel;
using MetroFramework.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CligenceCellIDGrabber
{
    public partial class Commands : MetroForm
    {
        private CancellationTokenSource _cancellationTokenSource;

        static bool lockk = false;
        //change path
        //string outputFile = "";// @"C:\amar\output.txt";
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

        string[] oneD;

        DateTime start = DateTime.Now, End;
        bool MachineType;
        string selectedcmbMode;
        int Countok = 0;
        bool isRowClicked = false;
        System.Management.ManagementEventWatcher watcher;
        public Commands()
        {

            InitializeComponent();
            _cancellationTokenSource = new CancellationTokenSource();

            (new DropShadow()).ApplyShadows(this);
            btnConnect.Visible = true;
            btnDisconnect.Visible = false;
            lblmsg.Text = MNC_MCC.Message.Replace("Name", "").Replace(":", "");
            //lblApplicationVersion.Text = "Version : 1.00";
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                Version ver = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                lblApplicationVersion.Text = string.Format("Version: {0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision, Assembly.GetEntryAssembly().GetName().Name);
                // lblApplicationVersion.Text = string.Format("Product Name: {4}, Version: {0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision, Assembly.GetEntryAssembly().GetName().Name);
            }
            else
            {
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                lblApplicationVersion.Text = string.Format("Version: {0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision, Assembly.GetEntryAssembly().GetName().Name);
            }
            lblCompanyName.Text = "@ Avenging Security";
            lblContactDetail.Text = "Support: 8690292122";
            // metroLabel3.Text = "@ Avenging Security";
            lblDate.Text = System.DateTime.Now.ToString();
            btnStop.Visible = false;
            btnStart.Visible = true;
            cmbMode.SelectedItem = "Fast";
            cmbMode.SelectedText = "Fast";
            cmbMode.SelectedIndex = 0;
            DdlMode.Enabled = false;
            //DdlMode.SelectedItem = "Spot";
            //DdlMode.SelectedText = "Spot";
            //cmbMode.SelectedIndex = 1;
            port = srport();

            try
            {
                serialPort2.Close();
                serialPort2.Open();
            }
            catch (Exception ex)
            {

            }
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Task timerTask = RunPeriodically(Checkport, TimeSpan.FromSeconds(10), tokenSource.Token);
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
            dt.Columns.Add("LAC/TAC");
            // dt.Columns.Add("TAC");
            dt.Columns.Add("ECI");
            dt.Columns.Add("CellId");
            dt.Columns.Add("CGI");
            dt.Columns.Add("(A/E/U)RFCN");
            dt.Columns.Add("ENB");
            dt.Columns.Add("Network Type");
            dt.Columns.Add("BSIC/PSC/PCI");
            dt.Columns.Add("dBM");
            dt.Columns.Add("Net Strength");
            //for 4G

            metroGrid1.DataSource = dt;
            metroGrid1.RowHeadersVisible = true;
            metroGrid1.AllowUserToAddRows = true;
            metroGrid1.AllowUserToDeleteRows = false;
            metroGrid1.ReadOnly = true;
            metroGrid1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            metroGrid1.ColumnHeadersDefaultCellStyle.Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);
            //  metroGrid1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgv_RowPostPaint);

        }
        async Task RunPeriodically(Action action, TimeSpan interval, CancellationToken token)
        {
            while (true)
            {
                action();
                await Task.Delay(interval, token);
            }
        }
        public async void Checkport()
        {

            if (serialPort2 != null && !serialPort2.IsOpen && this.loader.Visible)
            {
                try
                {
                    await Task.Run(() => serialPort2.Close());
                    ////serialPort2.Close();
                }
                catch (Exception ex)
                { }
                MessageBox.Show("Device's Cable is not connected.");
                btnDisconnect.Visible = false;
                loader.Invoke((MethodInvoker)delegate
                {
                    loader.Visible = false;
                });

                btnConnect.Visible = true;
                cmbMode.Enabled = false;
                metroComboBox1.Enabled = false;
                DdlMode.Enabled = false;
                btnStop.Visible = false;
                btnStart.Visible = true;
                btnStart.Enabled = false;
                btnConnect.Enabled = true;
                try
                {
                    try
                    {
                        await Task.Run(() => serialPort2.Close());
                    }
                    catch (Exception ex) { }
                    // scannedCellId.Clear();
                    //  dt.Clear();
                    lblStatus.Text = "Status : Disconnected";
                    MessageBox.Show("Connection closed!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while closing connection" + ex);
                    throw;
                }

                return;
            }
            else
            {
                try
                {
                    //if (serialPort2 == null)
                    //{
                    //    port = srport();
                    //}
                    if (!serialPort2.IsOpen && this.btnStart.Visible)
                    {
                        port = await Task.Run(() => srport());
                        //Thread.Sleep(3000);
                        //bool status = establishConnection();
                        //if (status)
                        //{
                        //    if (region == "NA")
                        //    {
                        //        regionloader.RunWorkerAsync();
                        //    }
                        //}

                    }
                    else
                    {
                        btnConnect.Visible = true;
                        // MessageBox.Show("Cable is connected.");
                        btnConnect.Enabled = true;
                    }
                }
                catch (Exception ex)
                {

                }
            }

        }
        private async Task StartIoTProcessAsync(CancellationToken token)
        {
            try
            {
                // Simulated long-running IoT process
                while (!token.IsCancellationRequested)
                {
                    // Example: Replace with actual IoT communication logic
                    await Task.Delay(1000); // Simulate doing work
                    Console.WriteLine("Running IoT command...");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("IoT process was canceled.");
            }
            finally
            {
                // Perform any cleanup here
                Console.WriteLine("IoT process stopped.");
            }
        }
        private string hexToInteger(string ascii)
        {
            try
            {
                //ascii = (Convert.ToInt32(ascii, 16)).ToString();
                return ascii;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        private void Dashboard5G_Load(object sender, EventArgs e)
        {
            //string constring = @"Data Source=DESKTOP-181DVKB;Initial Catalog=A6EAFEBFD8C8DC87FBFF637565326404187726;Integrated Security=true";

            //using (SqlConnection con = new SqlConnection(constring))
            //{
            //    SqlCommand cmd = new SqlCommand("AAGetCustomersPageWise", con);
            //    cmd.Parameters.AddWithValue("@PageIndex", 1);
            //    cmd.Parameters.AddWithValue("@PageSize", 50);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    SqlDataAdapter adp = new SqlDataAdapter(cmd);
            //    DataTable ds = new DataTable();
            //    adp.Fill(ds);
            //    metroGrid1.DataSource = ds;
            //}
        }
        #region Start/Stop
        [STAThread]
        private async void btnStart_Click(object sender, EventArgs e)
        {
            Countok = 0;
            //  _cancellationTokenSource = new CancellationTokenSource(); // Create a new token source for the new operation
            // await StartIoTProcessAsync(_cancellationTokenSource.Token);
            TypeText selectedNetworks = DdlMode.SelectedItem as TypeText;
            TypeText selectedNetworkcs = metroComboBox1.SelectedItem as TypeText;
            try
            {
                if (selectedNetworkcs == null || selectedNetworks == null || selectedNetworks.Name == "Select" || selectedNetworkcs.Name == "Select")
                {
                    MessageBox.Show("Please select Network Type and Type");
                    loader.Invoke((MethodInvoker)delegate
                    {
                        loader.Visible = false;
                    });

                    return;
                }
                await Task.Run(() => serialPort2.DataReceived += serialPort2_DataReceived);
                //  serialPort2.DataReceived += serialPort2_DataReceived;
            }
            catch (Exception ex)
            {

            }
            loader.Invoke((MethodInvoker)delegate
            {
                loader.Visible = true;

            });

            //btnStart.Invoke((MethodInvoker)delegate
            //{
            //    btnStart.Enabled = true;
            //});
            //watcher.EventArrived += new System.Management.EventArrivedEventHandler(PortAddedOrRemoved);
            //watcher.Query = new System.Management.WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3");
            //watcher.Start();

            // Create a WMI query to monitor for USB device arrival and removal events
            // WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3");

            // Create a management event watcher to listen for events
            //  watcher = new ManagementEventWatcher(query);
            // ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            // watcher.EventArrived += USBDeviceChangeHandler;
            //  watcher.Start();
            // Keep the program running
            //Console.WriteLine("Listening for COM port changes. Press any key to exit...");
            //Console.ReadKey();
            // Clean up

            //System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            //var fieVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            //var version = fieVersionInfo.FileVersion;
            //if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            //{
            //    Version ver = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            //    string g= string.Format("Product Name: {4}, Version: {0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision, Assembly.GetEntryAssembly().GetName().Name);
            //}
            //else
            //{
            //    var ver = Assembly.GetExecutingAssembly().GetName().Version;
            //    string j= string.Format("Product Name: {4}, Version: {0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision, Assembly.GetEntryAssembly().GetName().Name);
            //}

            string ddlmode = selectedNetworks.Name;

            string networkType = selectedNetworkcs.Name;
            if (cmbMode.SelectedItem.ToString().ToLower() == "deep" && ddlmode.ToString().ToLower() == "route")
            {

                MessageBox.Show("Please select Deep with Spot only");
            }
            else
            {
                start = DateTime.Now;
                if (ddlmode != null && networkType != null && selectedNetworkcs.Name != null)
                {
                    Modetype = DdlMode.SelectedItem.ToString();
                    //scannedCellId.Clear();
                    btnStop.Visible = true;
                    btnSave.Visible = false;
                    btnStart.Visible = false;
                    DdlMode.Enabled = false;
                    //dt.Clear();
                    //metroGrid1.Rows.Clear();
                    //metroGrid1.DataSource = null;
                    //metroComboBox1.Enabled = false;
                    selectedMode = ddlmode;// DdlMode.SelectedItem.ToString();
                    selectedcmbMode = cmbMode.SelectedItem.ToString();
                    try
                    {
                        if (serialPort2.IsOpen)
                        {
                            loader.Visible = true;
                            //progressBar1.Maximum = 100;
                            //progressBar1.Step = 1;    
                            //progressBar1.Value = 0;
                            //regionloader.RunWorkerAsync();
                            TypeText selectedNetwork = metroComboBox1.SelectedItem as TypeText;
                            a = selectedNetwork.Name;
                            net = selectedNetwork.Name.ToString();
                            try
                            {
                                //if ((cmbMode.SelectedItem.ToString()) == "Fast")
                                //{
                                switch (selectedNetwork.Name)
                                {
                                    case "3G": scan3GNetwork(); break;
                                    case "5G": scan5GNetwork(); break;
                                    case "4G": scan4GNetwork(); break;
                                    case "4G + 5G":
                                        scan4G5GNetwork();
                                        //Task task2aa = new Task(() => scan5GNetwork());
                                        //Task task4a = new Task(() => scan4GNetwork());
                                        //task2aa.Start();
                                        //task4a.Start();
                                        //Task.WaitAll(task2aa, task4a);

                                        break;

                                    case "ALL":
                                        scanAllForFast();
                                        //await Task.WhenAll(scan2GNetwork(), scan3GNetwork(), scan4GNetwork());
                                        //   Task.WhenAll(new[] { Task.Run(scan2GNetwork), Task.Run(scan3GNetwork), Task.Run(scan4GNetwork) });
                                        //Task task1 = new Task(() => scan2GNetwork());
                                        //Task task2 = new Task(() => scan5GNetwork());
                                        //Task task3 = new Task(() => scan4GNetwork());
                                        //task1.Start();
                                        //task2.Start();
                                        //task3.Start();
                                        //Task.WaitAll(task1, task2, task3);
                                        // scan2GNetwork();
                                        // scan3GNetwork(); 
                                        //scan4GNetwork(); 
                                        break;
                                    // case "ALL": scan2GNetwork(); break;
                                    //scan3GNetwork(); scan4GNetwork();
                                    default:
                                        break;
                                }

                                // }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Scan Completed");
                            }
                            //Thread.Sleep(2000);   
                        }
                        else
                        {
                            btnStop.Visible = false;
                            btnSave.Visible = false;
                            btnStart.Visible = false;
                            MessageBox.Show("Please connect machine first.");
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
        }

        //for ALL 
        private async void scanAllForFast(int count = 0)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                await RunAsyncforAll(Countok);
            }
            catch (Exception ex)
            {

            }
        }
        protected void scanAllForFastold(int count = 0)
        {
            net = "ALL";
            //outputFile = @"C:\amar\2goutput.txt";

            string c4 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
            //add new cmd
            string c4n = (@"AT+QNWPREFCFG = ""nsa_nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");

            string c1 = (@"AT+QNWPREFCFG=""mode_pref"",WCDMA").Replace("\r", "").Replace("\n", "");
            // string c2 = ("At+cops=?").Replace("\r", "").Replace("\n", "");
            string c3 = (@"AT+QENG=""servingcell""").Replace("\r", "").Replace("\n", "");
            if ((selectedcmbMode) == "Fast")
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                if (Countok < 1)
                {
                    serialWrite(c1); Thread.Sleep(2000);
                }
                else if (Countok >= 1 && Countok < 3)
                {
                    serialWrite(c3); Thread.Sleep(3000);
                }
                if (Countok > 2 && Countok < 4)
                {
                    serialWrite(@"AT+QNWPREFCFG= ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");//);
                    Thread.Sleep(2000);
                }

                if (Countok > 3 && Countok < 6)
                {
                    serialWrite("AT+QSCAN=1,1");
                    Thread.Sleep(3000);
                }
                if (Countok > 5 && Countok < 8)
                {
                    serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(3000);
                }
                if (Countok > 7 && Countok < 9)
                {
                    serialWrite(c4);//);
                    Thread.Sleep(2000);
                }
                //Add new command
                if (Countok > 8 && Countok < 10)
                {
                    serialWrite(c4n);//);
                    Thread.Sleep(2000);
                }

                if (Countok > 9 && Countok < 12)
                {
                    serialWrite("AT+QSCAN=2,1");//);
                    Thread.Sleep(2000);
                }
                if (Countok > 11 && Countok < 14)
                {
                    serialWrite("AT+QSCAN=3,1"); Thread.Sleep(2000);
                }
            }
            else
            {
                if (Countok < 1)
                {
                    serialWrite(c1); Thread.Sleep(4000);
                }
                else if (Countok >= 1 && Countok < 3)
                {
                    serialWrite(c3); Thread.Sleep(4000);
                }
                if (Countok > 2 && Countok < 4)
                {
                    serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                    Thread.Sleep(2000);
                }
                if ((Countok >= 3) && Countok <= 5)
                {
                    serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(2000);
                }
                if (Countok > 5 && Countok < 8)
                {
                    serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(2000);
                }
                int[] bandS = { 1, 3, 5, 8, 40, 41 };
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                if ((Countok > 7 && Countok < 13))
                {

                    if (Countok > 7 && Countok < 9)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[0]); Thread.Sleep(2000);
                    }
                    if (Countok > 8 && Countok < 11)
                    {
                        serialWrite("AT+QSCAN=1,1"); Thread.Sleep(2000);
                    }
                    if (Countok > 10 && Countok < 13)
                    {
                        serialWrite("AT+QSCAN=3,1"); Thread.Sleep(2000);
                    }
                }
                if ((Countok > 12 && Countok < 18))
                {
                    if (Countok > 12 && Countok < 14)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[1]); Thread.Sleep(2000);
                    }
                    if (Countok > 13 && Countok < 16)
                    {
                        serialWrite("AT+QSCAN=1,1"); Thread.Sleep(2000);
                    }
                    if (Countok > 15 && Countok < 18)
                    {
                        serialWrite("AT+QSCAN=3,1"); Thread.Sleep(2000);
                    }
                }
                if ((Countok > 17 && Countok < 23))
                {
                    if (Countok > 17 && Countok < 19)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[2]); Thread.Sleep(2000);
                    }
                    if (Countok > 18 && Countok < 21)
                    {
                        serialWrite("AT+QSCAN=1,1"); Thread.Sleep(2000);
                    }
                    if (Countok > 20 && Countok < 23)
                    {
                        serialWrite("AT+QSCAN=3,1"); Thread.Sleep(2000);
                    }
                }
                if ((Countok > 22 && Countok < 28))
                {
                    if (Countok > 22 && Countok < 24)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[3]); Thread.Sleep(2000);
                    }
                    if (Countok > 23 && Countok < 26)
                    {
                        serialWrite("AT+QSCAN=1,1"); Thread.Sleep(2000);
                    }
                    if (Countok > 25 && Countok < 28)
                    {
                        serialWrite("AT+QSCAN=3,1"); Thread.Sleep(2000);
                    }
                }
                if ((Countok > 27 && Countok < 33))
                {
                    if (Countok > 27 && Countok < 29)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[4]); Thread.Sleep(2000);
                    }
                    if (Countok > 28 && Countok < 31)
                    {
                        serialWrite("AT+QSCAN=1,1"); Thread.Sleep(2000);
                    }
                    if (Countok > 30 && Countok < 33)
                    {
                        serialWrite("AT+QSCAN=3,1"); Thread.Sleep(2000);
                    }
                }
                if ((Countok > 32 && Countok < 38))
                {
                    if (Countok > 32 && Countok < 34)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[5]); Thread.Sleep(2000);
                    }
                    if (Countok > 33 && Countok < 36)
                    {
                        serialWrite("AT+QSCAN=1,1"); Thread.Sleep(2000);
                    }
                    if (Countok > 35 && Countok < 38)
                    {
                        serialWrite("AT+QSCAN=3,1"); Thread.Sleep(2000);
                    }
                }
                if (Countok > 37 && Countok < 39)
                {
                    serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                    Thread.Sleep(2000);
                }
                string c33 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                //add new cmd
                string c33n = (@"AT+QNWPREFCFG = ""nsa_nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");

                string c11 = ("AT+QSCAN=2,1").Replace("\r", "").Replace("\n", "");
                string c2 = ("AT+QSCAN=3,1").Replace("\r", "").Replace("\n", "");

                if (Countok > 38 && Countok < 40)
                {
                    serialWrite(c33);//);
                    Thread.Sleep(2000);
                }
                //add new command
                if (Countok > 39 && Countok < 41)
                {
                    serialWrite(c33n);
                    Thread.Sleep(2000);
                }
                if (Countok > 40 && Countok < 43)
                {
                    serialWrite(c11);
                    Thread.Sleep(2000);
                }
                if (Countok > 42 && Countok < 45)
                {
                    serialWrite(c2);
                    Thread.Sleep(2000);
                }
                int[] bandSs = { 1, 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }

                if (Countok > 44 && Countok < 50)
                {
                    if (Countok > 44 && Countok < 46)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[0]); Thread.Sleep(2000);
                    }
                    if (Countok > 45 && Countok < 48)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 47 && Countok < 50)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 49 && Countok < 55)
                {
                    if (Countok > 49 && Countok < 51)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[1]); Thread.Sleep(2000);
                    }
                    if (Countok > 50 && Countok < 53)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 52 && Countok < 55)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 54 && Countok < 60)
                {
                    if (Countok > 54 && Countok < 56)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[2]); Thread.Sleep(2000);
                    }
                    if (Countok > 55 && Countok < 58)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 57 && Countok < 60)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 59 && Countok < 65)
                {
                    if (Countok > 59 && Countok < 61)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[3]); Thread.Sleep(2000);
                    }
                    if (Countok > 60 && Countok < 63)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 62 && Countok < 65)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 64 && Countok < 70)
                {
                    if (Countok > 64 && Countok < 66)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[4]); Thread.Sleep(2000);
                    }
                    if (Countok > 65 && Countok < 68)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 67 && Countok < 70)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 69 && Countok < 75)
                {
                    if (Countok > 69 && Countok < 71)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[5]); Thread.Sleep(2000);
                    }
                    if (Countok > 70 && Countok < 73)
                    {
                        serialWrite(c11); Thread.Sleep(4000);
                    }
                    if (Countok > 72 && Countok < 75)
                    {
                        serialWrite(c2); Thread.Sleep(4000);
                    }
                }
                if (Countok > 74 && Countok < 80)
                {
                    if (Countok > 74 && Countok < 76)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[6]); Thread.Sleep(2000);
                    }
                    if (Countok > 75 && Countok < 78)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 77 && Countok < 80)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 79 && Countok < 85)
                {
                    if (Countok > 79 && Countok < 81)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[7]); Thread.Sleep(2000);
                    }
                    if (Countok > 80 && Countok < 83)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 82 && Countok < 85)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 84 && Countok < 90)
                {
                    if (Countok > 84 && Countok < 86)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[8]); Thread.Sleep(2000);
                    }
                    if (Countok > 85 && Countok < 88)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 87 && Countok < 90)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 90 && Countok < 96)
                {
                    if (Countok > 90 && Countok < 92)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[9]); Thread.Sleep(2000);
                    }
                    if (Countok > 91 && Countok < 94)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 93 && Countok < 96)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 95 && Countok < 101)
                {
                    if (Countok > 95 && Countok < 97)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[10]); Thread.Sleep(2000);
                    }
                    if (Countok > 96 && Countok < 99)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 98 && Countok < 101)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 100 && Countok < 106)
                {
                    if (Countok > 99 && Countok < 101)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[11]); Thread.Sleep(2000);
                    }
                    if (Countok > 100 && Countok < 103)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 102 && Countok < 105)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                //for new band add one by one

                if (Countok > 104 && Countok < 110)
                {
                    if (Countok > 111 && Countok < 113)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[0]); Thread.Sleep(2000);
                    }
                    if (Countok > 112 && Countok < 115)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 114 && Countok < 117)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 116 && Countok < 122)
                {
                    if (Countok > 116 && Countok < 118)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[1]); Thread.Sleep(2000);
                    }
                    if (Countok > 117 && Countok < 120)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 119 && Countok < 122)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 121 && Countok < 127)
                {
                    if (Countok > 121 && Countok < 123)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[2]); Thread.Sleep(2000);
                    }
                    if (Countok > 122 && Countok < 125)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 124 && Countok < 127)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 126 && Countok < 132)
                {
                    if (Countok > 131 && Countok < 133)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[3]); Thread.Sleep(2000);
                    }
                    if (Countok > 132 && Countok < 135)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 134 && Countok < 137)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 136 && Countok < 142)
                {
                    if (Countok > 141 && Countok < 143)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[4]); Thread.Sleep(2000);
                    }
                    if (Countok > 142 && Countok < 145)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 144 && Countok < 147)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 146 && Countok < 152)
                {
                    if (Countok > 151 && Countok < 153)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[5]); Thread.Sleep(2000);
                    }
                    if (Countok > 152 && Countok < 155)
                    {
                        serialWrite(c11); Thread.Sleep(4000);
                    }
                    if (Countok > 154 && Countok < 157)
                    {
                        serialWrite(c2); Thread.Sleep(4000);
                    }
                }
                if (Countok > 156 && Countok < 162)
                {
                    if (Countok > 161 && Countok < 163)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[6]); Thread.Sleep(2000);
                    }
                    if (Countok > 162 && Countok < 165)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 164 && Countok < 167)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 166 && Countok < 172)
                {
                    if (Countok > 171 && Countok < 173)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[7]); Thread.Sleep(2000);
                    }
                    if (Countok > 172 && Countok < 175)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 174 && Countok < 177)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 176 && Countok < 182)
                {
                    if (Countok > 176 && Countok < 178)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[8]); Thread.Sleep(2000);
                    }
                    if (Countok > 177 && Countok < 180)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 179 && Countok < 182)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 181 && Countok < 187)
                {
                    if (Countok > 181 && Countok < 183)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[9]); Thread.Sleep(2000);
                    }
                    if (Countok > 182 && Countok < 185)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 184 && Countok < 187)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 186 && Countok < 192)
                {
                    if (Countok > 186 && Countok < 188)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[10]); Thread.Sleep(2000);
                    }
                    if (Countok > 187 && Countok < 190)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 189 && Countok < 192)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 191 && Countok < 197)
                {
                    if (Countok > 191 && Countok < 193)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[11]); Thread.Sleep(2000);
                    }
                    if (Countok > 192 && Countok < 195)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 194 && Countok < 197)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                //for new band add one by one



                if (Countok > 196 && Countok < 198)
                {
                    serialWrite(c33);
                    Thread.Sleep(2000);
                }
                if (Countok > 197 && Countok < 199)
                {
                    serialWrite(c33n);
                    Thread.Sleep(2000);
                }
            }
        }

        //for 3G
        private async void scan3GNetwork(int count = 0)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                await RunAsync3g(Countok);
            }
            catch (Exception ex)
            {

            }
        }
        protected void scan3GNetworkold(int count = 0)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                net = "3G";
                string s = "\"blabla\"";
                len = 11;
                //outputFile = @"C:\amar\2goutput.txt";
                string c1 = (@"AT+QNWPREFCFG=""mode_pref"",WCDMA").Replace("\r", "").Replace("\n", "");
                // string c2 = ("At+cops=?").Replace("\r", "").Replace("\n", "");
                string c3 = (@"AT+QENG=""servingcell""").Replace("\r", "").Replace("\n", "");
                if ((selectedcmbMode) == "Fast")
                {
                    if (Countok < 1)
                    {
                        serialWrite(c1); Thread.Sleep(10000);
                    }
                    else if (Countok >= 1 && Countok <= 3)
                    {
                        serialWrite(c3); Thread.Sleep(10000);
                    }
                }
                else
                {
                    if (Countok < 1)
                    {
                        serialWrite(c1); Thread.Sleep(10000);
                    }
                    else if (Countok >= 1 && Countok < 6)
                    {
                        serialWrite(c3); Thread.Sleep(10000);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        //for 4G

        private async void scan4GNetwork(int count = 0)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                await RunAsync4g(Countok);
            }
            catch (Exception ex)
            {

            }
        }
        private void scan4GNetworkold(int count = 0)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                net = "4G";
                len = 15;
                //outputFile = @"C:\amar\2goutput.txt";
                //await Task.Run(() => 
                if ((selectedcmbMode) == "Fast")
                {

                    if (Countok < 1 && Countok < 2)
                    {
                        serialWrite(@"AT+QNWPREFCFG= ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");//);
                        Thread.Sleep(4000);
                    }
                    if (Countok >= 1 && Countok < 3)
                    {
                        serialWrite("AT+QSCAN=1,1");
                        Thread.Sleep(10000);
                    }
                    if (Countok > 2 && Countok < 5)
                    {
                        serialWrite("AT+QSCAN=3,1");
                        Thread.Sleep(10000);
                    }
                    //await Task.Run(() => 
                }
                else
                {
                    if (Countok < 1 && Countok < 2)
                    {
                        serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                        Thread.Sleep(4000);
                    }
                    if ((Countok >= 1 || Countok < 1) && Countok <= 3)
                    {
                        serialWrite("AT+QSCAN=1,1");//);
                        Thread.Sleep(4000);
                    }
                    if (Countok > 3 && Countok < 6)
                    {
                        serialWrite("AT+QSCAN=3,1");
                        Thread.Sleep(4000);
                    }
                    int[] bandS = { 1, 3, 5, 8, 40, 41 };
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if ((Countok >= 6 && Countok < 11))
                    {

                        if (Countok >= 6 && Countok < 7)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[0]); Thread.Sleep(2000);
                        }
                        if (Countok > 6 && Countok < 9)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(5000);
                        }
                        if (Countok >= 9 && Countok < 11)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if ((Countok >= 11 && Countok < 16))
                    {
                        if (Countok >= 11 && Countok < 12)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[1]); Thread.Sleep(2000);
                        }
                        if (Countok > 11 && Countok < 14)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }
                        if (Countok >= 14 && Countok < 16)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if ((Countok >= 16 && Countok < 21))
                    {
                        if (Countok >= 16 && Countok < 17)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[2]); Thread.Sleep(2000);
                        }
                        if (Countok > 16 && Countok < 19)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }
                        if (Countok >= 19 && Countok < 21)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if ((Countok >= 21 && Countok < 26))
                    {
                        if (Countok >= 21 && Countok < 22)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[3]); Thread.Sleep(2000);
                        }
                        if (Countok > 21 && Countok < 24)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }
                        if (Countok >= 24 && Countok < 26)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if ((Countok >= 26 && Countok < 31))
                    {
                        if (Countok >= 26 && Countok < 27)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[4]); Thread.Sleep(2000);
                        }
                        if (Countok > 26 && Countok < 29)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }
                        if (Countok >= 29 && Countok < 31)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if ((Countok >= 31 && Countok < 36))
                    {
                        if (Countok >= 31 && Countok < 32)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[5]); Thread.Sleep(2000);
                        }
                        if (Countok > 31 && Countok < 34)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }
                        if (Countok >= 34 && Countok < 36)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    //for (int i = 0; i < bandS.Length; i++)
                    //{



                    //if (Countok >= 18 && Countok <= 23)
                    //{
                    //    serialWrite("AT+QSCAN=1,1"); Thread.Sleep(10000);
                    //}
                    //if (Countok > 23 && Countok < 28)
                    //{
                    //    serialWrite("AT+QSCAN=3,1");
                    //    Thread.Sleep(10000);
                    //}
                    if (Countok <= 38)
                    {
                        serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        //for 5G
        private async void scan5GNetwork(int count = 0)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                await RunAsync5g(Countok);
            }
            catch (Exception ex)
            {

            }
        }
        protected void scan5GNetworkold(int count = 0)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                //string f = @"Joe said ""Hello"" to me";
                net = "5G";
                string s = "\"blabla\"";
                len = 11;
                //outputFile = @"C:\amar\2goutput.txt";
                string c3 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                string c4 = (@"AT+QNWPREFCFG = ""nsa_nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");

                string c1 = ("AT+QSCAN=2,1").
                    Replace("\r", "").Replace("\n", "");
                string c2 = ("AT+QSCAN=3,1").Replace("\r", "").Replace("\n", "");
                if ((selectedcmbMode) == "Fast")
                {
                    if (Countok < 1)
                    {
                        serialWrite(c3);//);
                        Thread.Sleep(4000);
                    }

                    if (Countok >= 1 && Countok < 3)
                    {
                        serialWrite(c4);//);
                        Thread.Sleep(4000);
                    }

                    if (Countok >= 3 && Countok < 5)
                    {
                        serialWrite("AT+QSCAN=2,1");//);
                        Thread.Sleep(10000);
                    }
                    if (Countok > 4 && Countok < 8)
                    {
                        serialWrite("AT+QSCAN=3,1"); Thread.Sleep(5000);
                    }
                }
                else
                {
                    if (Countok < 1)
                    {
                        serialWrite(c3);//);
                        Thread.Sleep(4000);
                    }
                    if (Countok >= 1 && Countok < 2)
                    {
                        serialWrite(c4);//);
                        Thread.Sleep(3000);
                    }

                    if (Countok >= 2 && Countok < 4)
                    {
                        serialWrite(c1);
                        Thread.Sleep(4000);
                    }
                    if (Countok > 3 && Countok <= 6)
                    {
                        serialWrite(c2);
                        Thread.Sleep(4000);
                    }
                    int[] bandS = { 1, 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if (Countok > 6 && Countok < 12)
                    {
                        if (Countok <= 7)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[0]); Thread.Sleep(2000);
                        }
                        if (Countok > 7 && Countok <= 9)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 9 && Countok < 12)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 11 && Countok < 17)
                    {
                        if (Countok <= 12)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[1]); Thread.Sleep(2000);
                        }
                        if (Countok > 12 && Countok <= 14)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 14 && Countok < 17)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 16 && Countok < 22)
                    {
                        if (Countok <= 17)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[2]); Thread.Sleep(2000);
                        }
                        if (Countok > 17 && Countok <= 19)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 19 && Countok < 22)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 21 && Countok < 27)
                    {
                        if (Countok <= 22)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[3]); Thread.Sleep(2000);
                        }
                        if (Countok > 22 && Countok <= 24)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 24 && Countok < 27)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 26 && Countok < 32)
                    {
                        if (Countok <= 27)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[4]); Thread.Sleep(2000);
                        }
                        if (Countok > 27 && Countok <= 29)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 29 && Countok < 32)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 31 && Countok < 37)
                    {
                        if (Countok <= 32)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[5]); Thread.Sleep(2000);
                        }
                        if (Countok > 32 && Countok <= 35)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 35 && Countok < 38)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 37 && Countok < 43)
                    {
                        if (Countok <= 38)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[6]); Thread.Sleep(2000);
                        }
                        if (Countok > 38 && Countok <= 40)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 40 && Countok < 43)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 42 && Countok < 48)
                    {
                        if (Countok <= 43)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[7]); Thread.Sleep(2000);
                        }
                        if (Countok > 43 && Countok <= 45)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 45 && Countok < 48)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 47 && Countok < 53)
                    {
                        if (Countok <= 48)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[8]); Thread.Sleep(2000);
                        }
                        if (Countok > 48 && Countok <= 50)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 50 && Countok < 53)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 52 && Countok < 58)
                    {
                        if (Countok <= 53)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[9]); Thread.Sleep(2000);
                        }
                        if (Countok > 53 && Countok <= 55)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 55 && Countok < 58)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 57 && Countok < 63)
                    {
                        if (Countok <= 58)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[10]); Thread.Sleep(2000);
                        }
                        if (Countok > 58 && Countok <= 60)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 60 && Countok < 63)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 62 && Countok < 68)
                    {


                        if (Countok <= 63)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[11]); Thread.Sleep(2000);
                        }
                        if (Countok > 63 && Countok <= 65)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 65 && Countok < 68)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }
                    }
                    //for add new command
                    if (Countok > 67 && Countok < 73)
                    {
                        if (Countok <= 67)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[0]); Thread.Sleep(2000);
                        }
                        if (Countok > 68 && Countok <= 70)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 69 && Countok < 72)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 71 && Countok < 78)
                    {
                        if (Countok <= 72)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[1]); Thread.Sleep(2000);
                        }
                        if (Countok > 72 && Countok <= 74)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 74 && Countok < 77)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 76 && Countok < 82)
                    {
                        if (Countok <= 77)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[2]); Thread.Sleep(2000);
                        }
                        if (Countok > 77 && Countok <= 79)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 79 && Countok < 82)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 81 && Countok < 87)
                    {
                        if (Countok <= 82)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[3]); Thread.Sleep(2000);
                        }
                        if (Countok > 82 && Countok <= 84)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 84 && Countok < 87)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 86 && Countok < 92)
                    {
                        if (Countok <= 87)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[4]); Thread.Sleep(2000);
                        }
                        if (Countok > 87 && Countok <= 89)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 89 && Countok < 92)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 91 && Countok < 97)
                    {
                        if (Countok <= 92)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[5]); Thread.Sleep(2000);
                        }
                        if (Countok > 92 && Countok <= 95)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 95 && Countok < 98)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 97 && Countok < 103)
                    {
                        if (Countok <= 98)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[6]); Thread.Sleep(2000);
                        }
                        if (Countok > 98 && Countok <= 100)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 100 && Countok < 103)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 102 && Countok < 108)
                    {
                        if (Countok <= 103)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[7]); Thread.Sleep(2000);
                        }
                        if (Countok > 103 && Countok <= 105)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 105 && Countok < 108)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 107 && Countok < 113)
                    {
                        if (Countok <= 108)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[8]); Thread.Sleep(2000);
                        }
                        if (Countok > 108 && Countok <= 110)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 110 && Countok < 113)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 112 && Countok < 118)
                    {
                        if (Countok <= 113)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[9]); Thread.Sleep(2000);
                        }
                        if (Countok > 113 && Countok <= 115)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 115 && Countok < 118)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 117 && Countok < 123)
                    {
                        if (Countok <= 118)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[10]); Thread.Sleep(2000);
                        }
                        if (Countok > 118 && Countok <= 120)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 120 && Countok < 123)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 122 && Countok < 128)
                    {


                        if (Countok <= 123)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[11]); Thread.Sleep(2000);
                        }
                        if (Countok > 123 && Countok <= 125)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 125 && Countok < 128)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }


                    //for add new command
                    if (Countok >= 127 && Countok < 129)
                    {
                        serialWrite(c3);
                        Thread.Sleep(1000);
                    }

                    if (Countok >= 128 && Countok < 130)
                    {
                        serialWrite(c4);
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        #region 4G5GNetwork
        private async void scan4G5GNetwork(int count = 0)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                await RunAsyncfor4G5G(Countok);
            }
            catch (Exception ex)
            {

            }
        }
        private void scan4G5GNetworkold(int count = 0)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                net = "4G + 5G";
                len = 15;
                string c1 = ("AT+QSCAN=2,1").
                  Replace("\r", "").Replace("\n", "");
                string c3 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                string c3n = (@"AT+QNWPREFCFG = ""nsa_nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");

                // outputFile = @"C:\amar\2goutput.txt";
                //await Task.Run(() => 
                if ((selectedcmbMode) == "Fast")
                {
                    //Handshake j = new Handshake();
                    if (Countok < 1)
                    {
                        serialWrite(@"AT+QNWPREFCFG= ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");//);

                        Thread.Sleep(2000);
                    }
                    if (Countok >= 1 && Countok < 2)
                    {
                        serialWrite(c3);//);
                        Thread.Sleep(2000);
                    }

                    if (Countok >= 2 && Countok <= 4)
                    {

                        serialWrite(c3n);//);
                        Thread.Sleep(2000);
                    }
                    if (Countok >= 4 && Countok < 7)
                    {

                        serialWrite("AT+QSCAN=1,1");//);
                        Thread.Sleep(5000);
                    }

                    if (Countok > 6 && Countok < 9)
                    {
                        serialWrite("AT+QSCAN=2,1");//);
                        Thread.Sleep(10000);
                    }

                    if (Countok > 8 && Countok < 11)
                    {
                        serialWrite("AT+QSCAN=3,1");
                        Thread.Sleep(10000);
                    }

                }
                else
                {
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if (Countok < 1)
                    {
                        serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                        Thread.Sleep(2000);
                    }
                    string c4 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                    string c4n = (@"AT+QNWPREFCFG =  ""nsa_nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                    if (Countok < 2)
                    {
                        serialWrite(c4);//);
                        Thread.Sleep(2000);
                    }

                    if (Countok >= 2 && Countok < 4)
                    {
                        serialWrite(c4n);//);
                        Thread.Sleep(2000);
                    }
                    if (Countok > 3 && Countok < 5)
                    {
                        serialWrite("AT+QSCAN=1,1");//);
                        Thread.Sleep(4000);
                    }
                    if (Countok >= 5 && Countok < 7)
                    {
                        serialWrite(c1);//);
                        Thread.Sleep(4000);
                    }
                    if (Countok > 6 && Countok < 8)
                    {
                        serialWrite("AT+QSCAN=3,1");
                        Thread.Sleep(4000);
                    }
                    int[] bandS = { 1, 3, 5, 8, 40, 41 };
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if (Countok > 7 && Countok < 13)
                    {
                        if (Countok <= 8)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[0]); Thread.Sleep(2000);
                        }
                        if (Countok > 8 && Countok < 11)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }
                        if (Countok > 10 && Countok < 13)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 12 && Countok < 18)
                    {
                        if (Countok <= 13)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[1]); Thread.Sleep(2000);
                        }
                        if (Countok > 13 && Countok < 16)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }
                        if (Countok > 15 && Countok < 18)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 17 && Countok < 23)
                    {
                        if (Countok <= 18)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[2]); Thread.Sleep(2000);
                        }
                        if (Countok > 18 && Countok < 21)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);

                        }
                        if (Countok > 20 && Countok < 23)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 23 && Countok < 29)
                    {
                        if (Countok <= 24)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[3]); Thread.Sleep(2000);
                        }
                        if (Countok > 24 && Countok < 27)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }
                        if (Countok > 26 && Countok < 29)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 28 && Countok < 34)
                    {
                        if (Countok <= 29)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[4]); Thread.Sleep(2000);
                        }
                        if (Countok > 29 && Countok < 33)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }
                        if (Countok > 32 && Countok < 35)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 34 && Countok < 40)
                    {
                        if (Countok <= 35)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[0]); Thread.Sleep(2000);
                        }
                        if (Countok > 35 && Countok < 38)
                        {
                            serialWrite("AT+QSCAN=1,1"); Thread.Sleep(4000);
                        }
                        if (Countok > 37 && Countok < 40)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }

                    int[] bandSS = { 1, 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };

                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if (Countok > 39 && Countok < 45)
                    {
                        if (Countok <= 40)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[0]); Thread.Sleep(2000);
                        }
                        if (Countok > 40 && Countok < 43)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 42 && Countok < 45)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 44 && Countok < 50)
                    {
                        if (Countok <= 45)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[1]); Thread.Sleep(2000);
                        }
                        if (Countok > 45 && Countok < 48)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 47 && Countok < 50)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 49 && Countok < 55)
                    {
                        if (Countok <= 50)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[2]); Thread.Sleep(2000);
                        }
                        if (Countok > 50 && Countok < 53)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 52 && Countok < 55)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 54 && Countok < 62)
                    {
                        if (Countok <= 55)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[3]); Thread.Sleep(2000);
                        }
                        if (Countok > 55 && Countok < 58)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 57 && Countok < 60)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 59 && Countok < 65)
                    {
                        if (Countok <= 60)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[4]); Thread.Sleep(2000);
                        }
                        if (Countok > 60 && Countok < 63)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 62 && Countok < 65)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 64 && Countok < 70)
                    {
                        if (Countok <= 65)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[5]); Thread.Sleep(2000);
                        }
                        if (Countok > 65 && Countok < 68)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 67 && Countok < 70)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 69 && Countok < 75)
                    {
                        if (Countok <= 70)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[6]); Thread.Sleep(2000);
                        }
                        if (Countok > 70 && Countok < 73)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 72 && Countok < 75)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 74 && Countok < 80)
                    {
                        if (Countok <= 75)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[7]); Thread.Sleep(2000);
                        }
                        if (Countok > 75 && Countok < 78)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 77 && Countok < 80)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 79 && Countok < 85)
                    {
                        if (Countok <= 80)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[8]); Thread.Sleep(2000);
                        }
                        if (Countok > 80 && Countok < 83)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 82 && Countok < 85)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 84 && Countok < 90)
                    {
                        if (Countok <= 85)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[9]); Thread.Sleep(2000);
                        }
                        if (Countok > 85 && Countok < 88)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 87 && Countok < 90)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 89 && Countok < 95)
                    {
                        if (Countok <= 90)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[10]); Thread.Sleep(2000);
                        }
                        if (Countok > 90 && Countok < 93)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 92 && Countok < 95)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 94 && Countok < 100)
                    {
                        if (Countok <= 95)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[11]); Thread.Sleep(2000);
                        }
                        if (Countok > 95 && Countok < 98)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 97 && Countok < 100)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    //add new band for 5g 
                    if (Countok > 99 && Countok < 105)
                    {
                        if (Countok <= 100)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[0]); Thread.Sleep(2000);
                        }
                        if (Countok > 100 && Countok < 103)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 102 && Countok < 105)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 104 && Countok < 110)
                    {
                        if (Countok <= 105)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[1]); Thread.Sleep(2000);
                        }
                        if (Countok > 105 && Countok < 108)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 107 && Countok < 110)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 109 && Countok < 115)
                    {
                        if (Countok <= 110)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[2]); Thread.Sleep(2000);
                        }
                        if (Countok > 110 && Countok < 113)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 112 && Countok < 115)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 114 && Countok < 112)
                    {
                        if (Countok <= 115)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[3]); Thread.Sleep(2000);
                        }
                        if (Countok > 115 && Countok < 118)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 117 && Countok < 120)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 119 && Countok < 125)
                    {
                        if (Countok <= 120)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[4]); Thread.Sleep(2000);
                        }
                        if (Countok > 120 && Countok < 123)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 122 && Countok < 125)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 124 && Countok < 130)
                    {
                        if (Countok <= 125)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[5]); Thread.Sleep(2000);
                        }
                        if (Countok > 125 && Countok < 128)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 127 && Countok < 130)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 129 && Countok < 135)
                    {
                        if (Countok <= 130)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[6]); Thread.Sleep(2000);
                        }
                        if (Countok > 130 && Countok < 133)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 132 && Countok < 135)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 134 && Countok < 140)
                    {
                        if (Countok <= 135)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[7]); Thread.Sleep(2000);
                        }
                        if (Countok > 135 && Countok < 138)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 137 && Countok < 140)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 139 && Countok < 145)
                    {
                        if (Countok <= 140)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[8]); Thread.Sleep(2000);
                        }
                        if (Countok > 140 && Countok < 143)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 142 && Countok < 145)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 144 && Countok < 150)
                    {
                        if (Countok <= 145)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[9]); Thread.Sleep(2000);
                        }
                        if (Countok > 145 && Countok < 148)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 147 && Countok < 150)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 149 && Countok < 155)
                    {
                        if (Countok <= 150)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[10]); Thread.Sleep(2000);
                        }
                        if (Countok > 150 && Countok < 153)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 152 && Countok < 155)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 154 && Countok < 160)
                    {
                        if (Countok <= 155)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[11]); Thread.Sleep(2000);
                        }
                        if (Countok > 155 && Countok < 158)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 157 && Countok < 160)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }

                    //add new band for 5g
                    if (Countok > 159 && Countok < 161)
                    {
                        serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                        Thread.Sleep(2000);
                    }
                    if (Countok > 160 && Countok < 162)
                    {
                        serialWrite(c4);//);
                        Thread.Sleep(2000);
                    }
                    if (Countok > 161 && Countok < 163)
                    {
                        serialWrite(c4n);//);
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        #endregion
        private async void btnStop_Click(object sender, EventArgs e)
        {
            //progressbar(0);
            // watcher.Stop();
            //_cancellationTokenSource.Cancel();
            if (selectedMode.ToString().ToLower() == "route")
            {
                try
                {
                    lockk = false;
                    await Task.Run(() => serialPort2.Close());// Thread.Sleep(3000);
                    Thread.Sleep(1000);
                    // serialPort2.Close();
                    //serialPort2.Open();
                    //change 310324
                    // Thread.Sleep(3000);
                    Thread.Sleep(1000);
                    loader.Visible = false;
                    btnStop.Visible = false;
                    btnSave.Visible = true;
                    btnStart.Visible = true;
                    cmbMode.Enabled = true;
                    metroComboBox1.Enabled = true;
                    DdlMode.Enabled = true;

                    MessageBox.Show("Stopped");
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                lockk = false;
                if (lockk == false)
                {
                    btnDisconnect.Visible = false;
                    loader.Visible = false;
                    btnConnect.Visible = true;
                    try
                    {
                        await Task.Run(() => serialPort2.Close());

                        // scannedCellId.Clear();
                        // dt.Clear();

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
                    btnSave.Visible = true;
                    btnStart.Visible = true;
                    DdlMode.Enabled = false;
                    metroComboBox1.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Command in progress");
                }
            }
        }

        #endregion

        #region Strength
        private string getNetworkStrength(string dbm)
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
            else if (val < -100 && val >= -110)
            {
                return "Poor";
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
            //  outputFile = @"C:\cell id driver\amar\amar\GodSharpDemo\2goutput.txt";
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
            cmbMode.Enabled = false;
            metroComboBox1.Enabled = false;
            DdlMode.Enabled = false;
            btnStop.Visible = false;
            btnStart.Visible = true;
            btnStart.Enabled = false;
            try
            {
                serialPort2.Close();
                // scannedCellId.Clear();
                //  dt.Clear();
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
            try
            {
                //MyMethodAsync();
                if (!serialPort2.IsOpen)
                {
                    bool status = establishConnection();
                    if (status)
                    {
                        lblStatus.Text = "Status : Connected";
                        DdlMode.Enabled = true;
                        btnSave.Visible = false;
                        btnDisconnect.Visible = true;
                        btnStart.Enabled = true;
                        if (region == "NA")
                        {
                            //loader.Visible = true;
                            btnConnect.Visible = false;
                            btnStart.Enabled = true;
                            cmbMode.Enabled = true;
                            metroComboBox1.Enabled = true;
                            DdlMode.Enabled = true;
                            regionloader.RunWorkerAsync();
                            //MessageBox.Show("Region Selected");                    
                        }
                    }
                }
                else if (serialPort2.IsOpen)
                {
                    bool status = establishConnection();
                    lblStatus.Text = "Status : Connected";
                    DdlMode.Enabled = true;
                    btnSave.Visible = false;
                    btnDisconnect.Visible = true;
                    btnStart.Enabled = true;
                    if (region == "NA")
                    {
                        //loader.Visible = true;
                        btnConnect.Visible = false;
                        btnStart.Enabled = true;
                        cmbMode.Enabled = true;
                        metroComboBox1.Enabled = true;
                        DdlMode.Enabled = true;
                        regionloader.RunWorkerAsync();
                        //MessageBox.Show("Region Selected");                    
                    }
                }

                //else
                //{
                //    MessageBox.Show("In Progress.");
                //}


            }
            catch (Exception ex)
            {

            }
            //if(serialPort2.IsOpen)
            //serialPort2.Write("AT+cnsvs"+Environment.NewLine);           
        }
        private static bool IsPortHiddenOrUnused(string portName)
        {
            try
            {
                using (var port = new SerialPort(portName))
                {
                    // Try to open the port
                    port.Open();

                    // If successfully opened, it's not hidden or unused
                    port.Close();
                    return false;
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Port is in use
                return true;
            }
            catch (IOException)
            {
                // Port is hidden or doesn't exist
                return true;
            }
        }

        private static bool IsATPort(string portName)
        {
            // You can adjust this condition based on the naming convention
            // or other characteristics of AT Ports on your system
            return portName.ToLower().Contains("at");
        }
        public bool establishConnection()
        {
            try
            {
                serialPort2.DtrEnable = true;
                serialPort2.RtsEnable = true;
                string[] ports = SerialPort.GetPortNames();
                // ports = ports.Where(port => IsATPort(port)).ToArray();

                // Filter out hidden or unused ports
                //ports = ports.Where(port => !IsPortHiddenOrUnused(port)).ToArray();
                //for (int h = 0; h < ports.Length; h++)
                //{
                try
                {

                    // SerialPort port = new SerialPort(ports[0].Trim(), 115200, Parity.None, 8, StopBits.One);
                    SerialPort port = new SerialPort(serialPort2.PortName.Trim(), 115200);
                    //SerialPort port = new SerialPort(ports[0]);
                    //if (serialPort2.PortName.Trim() == port.PortName.Trim())
                    //{
                    //     port = new SerialPort(ports[1]);
                    //}
                    //port.DtrEnable = true;
                    //// port.RtsEnable = true;
                    Thread.Sleep(2000);
                    port.Open();
                }
                catch (Exception ex)
                {
                    //port = new SerialPort(ports[1]);
                    //port.Open();

                }
                //SerialPort port4 = new SerialPort(ports[4]);
                //// SerialPort port = new SerialPort(ports[0], 9600, Parity.Even, 8, StopBits.One);
                ////SerialPort port = new SerialPort(ports[0], 115200);
                //port4.BaudRate = 115200; port4.DataBits = 8; port4.Parity = Parity.None; port4.StopBits = StopBits.One; port4.Handshake = Handshake.None;
                //port4.DtrEnable = true; port4.NewLine = Environment.NewLine; port4.ReceivedBytesThreshold = 1024; port4.Open();


                // }
                //port.DataReceived += new SerialDataReceivedEventHandler(serialPort2_DataReceived);

                //port.ReadTimeout = 500;
                try
                {
                    serialPort2.Close();
                    // serialPort2.Dispose();
                    Thread.Sleep(2000);
                    serialPort2.Open();
                    Thread.Sleep(2000);
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Handle unauthorized access exception
                    Console.WriteLine("Unauthorized access: " + ex.Message);
                }
                catch (IOException ex)
                {
                    // Handle other I/O exceptions
                    Console.WriteLine("I/O error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    //if (serialPort2 != null)
                    //{
                    //  serialPort2.Dispose();
                    //    serialPort2 = null;
                    //}
                }
                //if (!serialPort2.IsOpen)
                //{
                //    return false;
                //}
                //else
                //{
                return true;
                ///}
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
        [STAThread]
        private void serialPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataRec = "";
            End = DateTime.Now;
            var result = (int)End.Subtract(start).TotalMinutes;
            //Thread.Sleep(300);
            try
            {
                // if(!string.IsNullOrEmpty( dataRec)) AT+QNWPREFCFG="lte_band",3
                dataRec = serialPort2.ReadExisting();
            }
            catch (Exception ex)
            {

            }
            //string[] array1 = dataRec.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            //List<Dictionary<string, string>> list1 = clean(array1);
            //loader.Invoke((MethodInvoker)delegate
            //{
            //    loader.Visible = true;
            //});

            //btnStart.Invoke((MethodInvoker)delegate
            //{
            //    btnStart.Enabled = true;
            //});
            metroComboBox1.Invoke((MethodInvoker)delegate
            {
                //if(DdlMode.SelectedItem.ToString()=="Route")freceiv
                metroComboBox1.Enabled = false;
            });
            DdlMode.Invoke((MethodInvoker)delegate
            {
                //if(DdlMode.SelectedItem.ToString()=="Route")
                DdlMode.Enabled = true;
            });
            cmbMode.Invoke((MethodInvoker)delegate
            {
                //if(DdlMode.SelectedItem.ToString()=="Route")
                cmbMode.Enabled = true;
            });//
               //}
               //(net != "R") && result >= 5 || 

            string[] array = dataRec.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<Dictionary<string, string>> list = clean(array);
            string c3 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
            string c4 = (@"AT+QNWPREFCFG= ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");//);

            //if (Countok == 1 && dataRec.ToString().Contains(c3) && a.Contains("5G") )
            //{
            //    serialWrite("AT+QSCAN=2,1");
            //}
            //else if (Countok == 1 && dataRec.ToString().Contains(c4) && a.Contains("4G"))
            //{
            //    serialWrite("AT+QSCAN=1,1"); ;
            //}

            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, string> map = list[i];
                try
                {
                    if (map.Count > 0 && scannedCellId.Contains(map["cellid"]))
                    {
                        continue;
                    }
                    scannedCellId.Add(map["cellid"]);
                    var row = dt.NewRow();
                    for (int j = 0; j < twoD.Length; j++)
                    {
                        if (twoD[j][1].Trim().Equals(map["mnc"].Trim()) && twoD[j][0].Trim().Equals(map["mcc"].Trim()))
                        {
                            row["Circle"] = twoD[j][3];
                            row["Operator Name"] = twoD[j][2];
                            break;
                        }
                    }
                    row["DateTime"] = DateTime.Now;
                    row["MCC"] = map["mcc"];
                    row["MNC"] = map["mnc"];
                    row["LAC/TAC"] = map["lac"];
                    // row["ECI"] = map["cellId"];enb= same (cell id) of Jio																									
                    //eci = cell id(convert hexa decimal to integer) for Jio
                    //enb = cellid / 256(means divided by 256) for Airtel & Vodafone
                    //  eci = enb + (hex cell id-- > last 2 digits-- > reverse them) for Airtel & Vodafone

                    if (row["Operator Name"].ToString().ToLower() != "jio")
                    {
                        try
                        {
                            row["ENB"] = Convert.ToInt32(map["cellid"], 16) / 256;
                            string hexcell = map["cellid"].ToString().Substring(map["cellid"].Length - 2, 2);
                            row["ECI"] = (row["ENB"] + "" + Reverse(hexcell)).Replace("-", "");
                        }
                        catch (Exception ex)
                        {
                            row["ENB"] = (((Convert.ToInt32(map["cellid"], 16))).ToString().Replace("-", ""));
                            //string hexcell = map["cellid"].ToString().Substring(map["cellid"].Length - 2, 2);
                            row["ECI"] = (row["ENB"]).ToString().Replace("-", "");
                        }
                        //row["TAC"] = (Convert.ToInt32(map["tac"], 16));
                        row["CellId"] = (Convert.ToInt32(map["cellid"], 16));
                        row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + (Convert.ToInt32(map["cellid"], 16));
                        row["LAC/TAC"] = Convert.ToInt32(map["lac"], 16);
                        if (map["net"] == "3G")
                        {
                            row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + row["LAC/TAC"] + "-" + (Convert.ToInt32(map["cellid"], 16));
                        }
                        try
                        {
                            if (map["net"] == "4G" && row["Operator Name"].ToString().ToLower() == "airtel")
                            {
                                //row["ENB"] = Convert.ToInt32(map["cellid"], 16) / 256;
                                //string hexcell = map["cellid"].ToString().Substring(map["cellid"].Length - 2, 2);
                                //row["ECI"] = (row["ENB"] + "" + Reverse(hexcell)).Replace("-", "");

                                //new formula
                                string id = map["cellid"].ToString().Substring(map["cellid"].Length - 2, 2);
                                row["ECI"] = (Convert.ToInt32(map["cellid"], 16) / 256).ToString() + "" + Convert.ToInt32(id, 16).ToString();
                                row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + row["LAC/TAC"] + "-" + (Convert.ToInt32(map["cellid"], 16));
                            }
                            if (map["net"] == "4G" && row["Operator Name"].ToString().ToLower().Contains("vodafone"))
                            {
                                // eci = enb + (hex cell id --> last 2 digits-- > convert to int) for Vodafone
                                row["ENB"] = Convert.ToInt32(map["cellid"], 16) / 256;
                                string id = map["cellid"].ToString().Substring(map["cellid"].Length - 2, 2);
                                row["ECI"] = (Convert.ToInt32(map["cellid"], 16) / 256).ToString() + "" + Convert.ToInt32(id, 16).ToString();
                                row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + row["ECI"];
                            }
                        }
                        catch (Exception ex) { }
                    }
                    else
                    {
                        row["ENB"] = map["cellid"];
                        row["ECI"] = (map["cellid"]).ToString().Replace("-", "");
                        // row["TAC"] = map["tac"];
                        row["CellId"] = map["cellid"];

                        row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + map["cellid"];
                        if (map["net"] == "3G")
                        {
                            row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + row["LAC/TAC"] + "-" + map["cellid"];
                        }
                        if (map["net"] == "4G")
                        {
                            if (row["CGI"].ToString().Replace("-", "").Length == 12)
                                row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + "0" + map["cellid"];

                            if (row["CGI"].ToString().Replace("-", "").Length == 11)
                                row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + "00" + map["cellid"];
                        }
                    }
                    row["(A/E/U)RFCN"] = map["arfcn"];

                    if (map["net"] == "3G")
                    {
                        row["ECI"] = (Convert.ToInt32(map["cellid"], 16));// map["cellid"];
                        row["ENB"] = "NA";
                        //row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + map["lac"] + "-" + map["cellid"];
                    }
                    row["Network Type"] = map["net"];
                    row["BSIC/PSC/PCI"] = map["bsic"];
                    row["dBM"] = map["dBm"];
                    row["Net Strength"] = getNetworkStrength(map["dBm"]);
                    dt.Rows.Add(row);
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        //metroGrid1.RowHeadersVisible = true;
                        //metroGrid1.AllowUserToAddRows = true;
                        //metroGrid1.AllowUserToDeleteRows = false;
                        //metroGrid1.ReadOnly = true;
                        //metroGrid1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                        //metroGrid1.ColumnHeadersDefaultCellStyle.Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);
                        //  metroGrid1.CellMouseClick += new DataGridViewCellMouseEventHandler(this.dgv_CellMouseClick);

                        metroGrid1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgv_RowPostPaint);
                        metroGrid1.DataSource = dt;

                    }));
                }
                catch (Exception ex)
                {

                }
            }
            try
            {
                if (selectedcmbMode.ToLower().Contains("deep") && (dataRec.ToLower().Contains("ok") || dataRec.ToLower().Contains("error")) && selectedMode.ToLower().Contains("spot"))
                {
                    if (a.Contains("5G") && !a.Contains("4G + 5G"))
                    {
                        progressbar(1);
                        scan5GNetwork(Countok);

                        if (Countok > 130)
                        {
                            //if (this.loader.Visible)
                            //{
                            //    MessageBox.Show("Scan Completed");
                            //}
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            Progrsbr.Invoke((MethodInvoker)delegate
                            {
                                // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                                Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                                Progrsbr.Value = Progrsbr.Minimum;
                                Progrsbr.Value = 0;

                            });
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Visible = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("4G") && !a.Contains("4G + 5G"))
                    {
                        scan4GNetwork(Countok);
                        if (Countok <= 34)
                        {
                            progressbar(3);
                        }
                        if (Countok >= 38 && Countok < 39)
                        {
                            //if (this.loader.Visible)
                            //{
                            //    MessageBox.Show("Scan Completed");
                            //}
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            Progrsbr.Invoke((MethodInvoker)delegate
                            {
                                // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                                Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                                Progrsbr.Value = Progrsbr.Minimum;
                                Progrsbr.Value = 0;

                            });
                            // serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Visible = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("4G + 5G"))
                    {
                        scan4G5GNetwork(Countok);
                        progressbar(1);
                        if (Countok >= 163)
                        {
                            //if (this.loader.Visible)
                            //{
                            //    MessageBox.Show("Scan Completed");
                            //}
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            Progrsbr.Invoke((MethodInvoker)delegate
                            {
                                // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                                Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                                Progrsbr.Value = Progrsbr.Minimum;
                                Progrsbr.Value = 0;

                            });
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Visible = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("3G"))
                    {
                        progressbar(17);
                        scan3GNetwork(Countok);
                        if (Countok >= 6)
                        {
                            //if (this.loader.Visible)
                            //{
                            //    MessageBox.Show("Scan Completed");
                            //}
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            Progrsbr.Invoke((MethodInvoker)delegate
                            {
                                // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                                Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                                Progrsbr.Value = Progrsbr.Minimum;
                                Progrsbr.Value = 0;

                            });
                            MessageBox.Show("Scan Completed");
                            //MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            Progrsbr.Invoke((MethodInvoker)delegate
                            {
                                // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                                Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                                Progrsbr.Value = Progrsbr.Minimum;
                                Progrsbr.Value = 0;

                            });
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Visible = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else
                    {
                       
                        progressbar(1);
                        scanAllForFast(Countok);
                        if (Countok >= 168)
                        {
                            //if (this.loader.Visible)
                            //{
                            //    MessageBox.Show("Scan Completed");
                            //}
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            Progrsbr.Invoke((MethodInvoker)delegate
                            {
                                // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                                Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                                Progrsbr.Value = Progrsbr.Minimum;
                                Progrsbr.Value = 0;

                            });
                            //MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Visible = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                }

                //list.Count > 2 && 
                else if (dataRec.ToLower().Contains("ok") && (selectedMode.ToLower().Contains("spot") || selectedMode.ToLower().Contains("route"))
           && selectedcmbMode.ToLower().Contains("fast") && (dataRec.ToLower().Contains("ok") || dataRec.ToLower().Contains("error")) && (selectedMode.ToLower().Contains("spot")) || selectedMode.ToLower().Contains("route"))
                {
                    if (a.Contains("5G") && !a.Contains("4G + 5G"))
                    {
                        progressbar(25);
                        scan5GNetwork(Countok);

                        if (Countok >= 5 && !selectedMode.ToLower().Contains("route"))
                        {
                            //if (this.loader.Visible)
                            //{
                            //    MessageBox.Show("Scan Completed");
                            //}
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            Progrsbr.Invoke((MethodInvoker)delegate
                            {
                                // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                                Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                                Progrsbr.Value = Progrsbr.Minimum;
                                Progrsbr.Value = 0;

                            });
                            //MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Visible = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("4G") && !a.Contains("4G + 5G"))
                    {
                        progressbar(19);
                        scan4GNetwork(Countok);

                        if (Countok >= 5 && !selectedMode.ToLower().Contains("route"))
                        {
                            //if (this.loader.Visible)
                            //{
                            //    MessageBox.Show("Scan Completed");
                            //}
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            // MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            Progrsbr.Invoke((MethodInvoker)delegate
                            {
                                // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                                Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                                Progrsbr.Value = Progrsbr.Minimum;
                                Progrsbr.Value = 0;

                            });
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Visible = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("4G + 5G"))
                    {
                        progressbar(10);
                        scan4G5GNetwork(Countok);

                        if (Countok >= 9 && !selectedMode.ToLower().Contains("route"))
                        {
                            //if (this.loader.Visible)
                            //{
                            //    MessageBox.Show("Scan Completed");
                            //}
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            Progrsbr.Invoke((MethodInvoker)delegate
                            {
                                // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                                Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                                Progrsbr.Value = Progrsbr.Minimum;
                                Progrsbr.Value = 0;

                            });
                            // serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Visible = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("3G"))
                    {
                        progressbar(25);
                        scan3GNetwork(Countok);
                        if (Countok > 3 && !selectedMode.ToLower().Contains("route"))
                        {
                            //if (this.loader.Visible)
                            //{
                            //    MessageBox.Show("Scan Completed");
                            //}
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            // MessageBox.Show("Scan Completed");
                            // serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Visible = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else
                    {
                        progressbar(8);
                        scanAllForFast(Countok);
                        if (Countok > 12 && !selectedMode.ToLower().Contains("route"))
                        {
                            //if (this.loader.Visible)
                            //{
                            //    MessageBox.Show("Scan Completed");
                            //}
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            Progrsbr.Invoke((MethodInvoker)delegate
                            {
                                // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                                Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                                Progrsbr.Value = Progrsbr.Minimum;
                                Progrsbr.Value = 0;

                            });
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Visible = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }

                    }
                }
                //try
                //{
                //   // File.AppendAllText(outputFile, dataRec);
                //}
                //catch (Exception ex)
                //{

                //}

            }

            catch (Exception ex)
            {
                loader.Invoke((MethodInvoker)delegate
                {
                    loader.Visible = false;
                });
            }
        }
        public void progressbarwrong(decimal increment)
        {
            if (this.Progrsbr.Visible)
            {
                Progrsbr.Invoke((MethodInvoker)delegate
                {
                    // Calculate the integer increment based on the decimal value
                    int intIncrement = (int)Math.Round(increment);

                    // Safely adjust the progress bar value
                    int newValue = Progrsbr.Value + intIncrement;
                    if (newValue >= Progrsbr.Maximum)
                    {
                        Progrsbr.Value = Progrsbr.Maximum;
                    }
                    else if (newValue <= Progrsbr.Minimum)
                    {
                        Progrsbr.Value = Progrsbr.Minimum;
                    }
                    else
                    {
                        Progrsbr.Value = newValue;
                    }

                    // Calculate the percentage
                    int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) /
                        (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);

                    // Draw the percentage string
                    using (Graphics graphics = Progrsbr.CreateGraphics())
                    {
                        string percentageText = per.ToString() + "%";
                        SizeF textSize = graphics.MeasureString(percentageText, SystemFonts.DefaultFont);
                        PointF location = new PointF(Progrsbr.Width / 2 - textSize.Width / 2.0F,
                                                     Progrsbr.Height / 2 - textSize.Height / 2.0F);

                        graphics.DrawString(percentageText, SystemFonts.DefaultFont, Brushes.Black, location);
                    }
                });
            }
        }


        public void progressbar(int increment)
        {
            if (this.Progrsbr.Visible)
            {
                Progrsbr.Invoke((MethodInvoker)delegate
                {
                    this.Progrsbr.Increment(increment);
                    int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) /
                        (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                    //if (selectedcmbMode.ToLower().Contains("deep") || selectedMode.ToLower().Contains("spot"))
                    //{
                    //    if (a == "ALL")
                    //    {
                    //        var pers = per / 2;
                    //        using (Graphics graphics = Progrsbr.CreateGraphics())
                    //        {
                    //            graphics.DrawString(pers.ToString() + "%", SystemFonts.DefaultFont, Brushes.Black,
                    //                new PointF(Progrsbr.Width / 2 - (graphics.MeasureString(pers.ToString() + "%",
                    //                SystemFonts.DefaultFont).Width / 2.0F),
                    //                Progrsbr.Height / 2 - (graphics.MeasureString(pers.ToString() + "%",
                    //                SystemFonts.DefaultFont).Height / 2.0F)));
                    //        }
                    //    }
                    //}
                    //else
                    //{
                        using (Graphics graphics = Progrsbr.CreateGraphics())
                        {
                            graphics.DrawString(per.ToString() + "%", SystemFonts.DefaultFont, Brushes.Black,
                                new PointF(Progrsbr.Width / 2 - (graphics.MeasureString(per.ToString() + "%",
                                SystemFonts.DefaultFont).Width / 2.0F),
                                Progrsbr.Height / 2 - (graphics.MeasureString(per.ToString() + "%",
                                SystemFonts.DefaultFont).Height / 2.0F)));
                        }
                    //}
                });
            }
        }
        //private void dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        //{
        //    if (metroGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && !string.IsNullOrWhiteSpace(metroGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
        //    {
        //        metroGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = new DataGridViewCellStyle { ForeColor = Color.White, BackColor = Color.Blue };
        //    }
        //    else
        //    {
        //        metroGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = metroGrid1.DefaultCellStyle;
        //    }
        //}
        private void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                isRowClicked = true;  // Set the flag when a row is clicked
            }
        }

        private void dgv_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //if(loader.Visible)
            //{
            //    isRowClicked = false;
            //    return;
            //}
            try
            {
                var grid = sender as DataGridView;
                var rowIdx = (e.RowIndex + 1).ToString();

                var centerFormat = new StringFormat()
                {
                    // right alignment might actually make more sense for numbers
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString(rowIdx, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold), Brushes.White, headerBounds, centerFormat);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        #region Changes for deadlock
        private async Task SendSerialCommandAsync(string command, int delayMilliseconds, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                serialWrite(command);

                // Simulate a delay (replaces Thread.Sleep)
                Task.Delay(delayMilliseconds, cancellationToken).Wait();
               
            }, cancellationToken);
        }
        #region 4G5GNetwork
        public async Task RunAsyncfor4G5G(int Countok)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                // Set a global timeout for the entire operation
                cts.CancelAfter(TimeSpan.FromSeconds(30)); // e.g., 30 seconds

                await ExecuteIoTCommandAsyncfor4G5G(cts.Token, Countok);
            }
        }
        public async Task ExecuteIoTCommandAsyncfor4G5G(CancellationToken cancellationToken, int Countok)
        {
            try
            {
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                net = "4G + 5G";
                len = 15;
                string c1 = ("AT+QSCAN=2,1").
                  Replace("\r", "").Replace("\n", "");
                string c3 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                string c3n = (@"AT+QNWPREFCFG = ""nsa_nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");

                // outputFile = @"C:\amar\2goutput.txt";
                //await Task.Run(() => 
                if ((selectedcmbMode) == "Fast")
                {
                    //Handshake j = new Handshake();
                    if (Countok < 1)
                    {
                        await SendSerialCommandAsync(@"AT+QNWPREFCFG= ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71", 2000, cancellationToken);
                    }
                    if (Countok >= 1 && Countok < 2)
                    {
                        await SendSerialCommandAsync(c3,2000,cancellationToken);
                    }
                    if (Countok >= 2 && Countok <= 4)
                    {

                        await SendSerialCommandAsync(c3n, 2000, cancellationToken);
                    }
                    if (Countok >= 4 && Countok < 7)
                    {
                        await SendSerialCommandAsync("AT+QSCAN=1,1", 5000, cancellationToken);
                    }

                    if (Countok > 6 && Countok < 9)
                    {
                        await SendSerialCommandAsync("AT+QSCAN=2,1",10000,cancellationToken);
                    }

                    if (Countok > 8 && Countok < 11)
                    {
                        await SendSerialCommandAsync("AT+QSCAN=3,1",10000,cancellationToken);
                    }

                }
                else
                {
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if (Countok < 1)
                    {
                        await SendSerialCommandAsync(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71",2000,cancellationToken);
                    }
                    string c4 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                    string c4n = (@"AT+QNWPREFCFG =  ""nsa_nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                    if (Countok < 2)
                    {
                        await SendSerialCommandAsync(c4, 2000, cancellationToken);
                    }
                    if (Countok >= 2 && Countok < 4)
                    {
                        await SendSerialCommandAsync(c4n, 2000, cancellationToken);
                    }
                    if (Countok > 3 && Countok < 5)
                    {

                        await SendSerialCommandAsync("AT+QSCAN=1,1", 4000, cancellationToken);
                    }
                    if (Countok >= 5 && Countok < 7)
                    {
                        await SendSerialCommandAsync(c1, 4000, cancellationToken);
                    }
                    if (Countok > 6 && Countok < 8)
                    {
                        await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                    }
                    int[] bandS = { 1, 3, 5, 8, 40, 41 };
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if (Countok > 7 && Countok < 13)
                    {
                        if (Countok <= 8)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[0],2000,cancellationToken);
                        }
                        if (Countok > 8 && Countok < 11)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1",4000,cancellationToken);
                        }
                        if (Countok > 10 && Countok < 13)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 12 && Countok < 18)
                    {
                        if (Countok <= 13)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[1],2000,cancellationToken);
                        }
                        if (Countok > 13 && Countok < 16)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1",4000,cancellationToken);
                        }
                        if (Countok > 15 && Countok < 18)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 17 && Countok < 23)
                    {
                        if (Countok <= 18)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[2],2000,cancellationToken);
                        }
                        if (Countok > 18 && Countok < 21)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000, cancellationToken);

                        }
                        if (Countok > 20 && Countok < 23)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1",4000,cancellationToken);
                        }

                    }
                    if (Countok > 23 && Countok < 29)
                    {
                        if (Countok <= 24)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[3],2000,cancellationToken);
                        }
                        if (Countok > 24 && Countok < 27)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1",4000,cancellationToken);
                        }
                        if (Countok > 26 && Countok < 29)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 28 && Countok < 34)
                    {
                        if (Countok <= 29)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[4],2000,cancellationToken);
                        }
                        if (Countok > 29 && Countok < 33)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1",2000,cancellationToken);
                        }
                        if (Countok > 32 && Countok < 34)
                        {
                            //Countok++;
                            await SendSerialCommandAsync("AT+QSCAN=3,1",2000,cancellationToken);
                        }
                    }
                    if (Countok > 33 && Countok < 40)
                    {
                        if (Countok <= 35)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[5],2000,cancellationToken);
                        }
                        if (Countok > 35 && Countok < 38)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1",4000,cancellationToken);
                        }
                        if (Countok > 37 && Countok < 40)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }

                    int[] bandSS = { 1, 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };

                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if (Countok > 39 && Countok < 45)
                    {
                        if (Countok <= 40)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[0],2000,cancellationToken);
                        }
                        if (Countok > 40 && Countok < 43)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 42 && Countok < 45)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 44 && Countok < 50)
                    {
                        if (Countok <= 45)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[1],2000,cancellationToken);
                        }
                        if (Countok > 45 && Countok < 48)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 47 && Countok < 50)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 49 && Countok < 55)
                    {
                        if (Countok <= 50)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[2],2000,cancellationToken);
                        }
                        if (Countok > 50 && Countok < 53)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 52 && Countok < 55)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 54 && Countok < 62)
                    {
                        if (Countok <= 55)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[3],2000,cancellationToken);
                        }
                        if (Countok > 55 && Countok < 58)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 57 && Countok < 60)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 59 && Countok < 65)
                    {
                        if (Countok <= 60)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[4],2000,cancellationToken);
                        }
                        if (Countok > 60 && Countok < 63)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 62 && Countok < 65)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 64 && Countok < 70)
                    {
                        if (Countok <= 65)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[5],2000,cancellationToken);
                        }
                        if (Countok > 65 && Countok < 68)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 67 && Countok < 70)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 69 && Countok < 75)
                    {
                        if (Countok <= 70)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[6],2000,cancellationToken);
                        }
                        if (Countok > 70 && Countok < 73)
                        {
                          await  SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 72 && Countok < 75)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 74 && Countok < 80)
                    {
                        if (Countok <= 75)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[7],2000,cancellationToken);
                        }
                        if (Countok > 75 && Countok < 78)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 77 && Countok < 80)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 79 && Countok < 85)
                    {
                        if (Countok <= 80)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[8],2000,cancellationToken);
                        }
                        if (Countok > 80 && Countok < 83)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 82 && Countok < 85)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 84 && Countok < 90)
                    {
                        if (Countok <= 85)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[9],2000,cancellationToken);
                        }
                        if (Countok > 85 && Countok < 88)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 87 && Countok < 90)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 89 && Countok < 95)
                    {
                        if (Countok <= 90)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[10],2000,cancellationToken);
                        }
                        if (Countok > 90 && Countok < 93)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok >= 92 && Countok < 95)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 94 && Countok < 100)
                    {
                        if (Countok <= 95)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[11],2000,cancellationToken);
                        }
                        if (Countok > 95 && Countok < 98)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 97 && Countok < 100)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    //add new band for 5g 
                    if (Countok > 99 && Countok < 105)
                    {
                        if (Countok <= 100)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[0],2000,cancellationToken);
                        }
                        if (Countok > 100 && Countok < 103)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 102 && Countok < 105)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 104 && Countok < 110)
                    {
                        if (Countok <= 105)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[1],2000,cancellationToken);
                        }
                        if (Countok > 105 && Countok < 108)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 107 && Countok < 110)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 109 && Countok < 115)
                    {
                        if (Countok <= 110)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[2],2000,cancellationToken);
                        }
                        if (Countok > 110 && Countok < 113)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 112 && Countok < 115)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 114 && Countok < 112)
                    {
                        if (Countok <= 115)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[3],2000,cancellationToken);
                        }
                        if (Countok > 115 && Countok < 118)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 117 && Countok < 120)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 119 && Countok < 125)
                    {
                        if (Countok <= 120)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[4],2000,cancellationToken);
                        }
                        if (Countok > 120 && Countok < 123)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 122 && Countok < 125)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 124 && Countok < 130)
                    {
                        if (Countok <= 125)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[5],2000,cancellationToken);
                        }
                        if (Countok > 125 && Countok < 128)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 127 && Countok < 130)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 129 && Countok < 135)
                    {
                        if (Countok <= 130)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[6],2000,cancellationToken);
                        }
                        if (Countok > 130 && Countok < 133)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 132 && Countok < 135)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 134 && Countok < 140)
                    {
                        if (Countok <= 135)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[7],2000,cancellationToken);
                        }
                        if (Countok > 135 && Countok < 138)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 137 && Countok < 140)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 139 && Countok < 145)
                    {
                        if (Countok <= 140)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[8],2000,cancellationToken);
                        }
                        if (Countok > 140 && Countok < 143)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 142 && Countok < 145)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 144 && Countok < 150)
                    {
                        if (Countok <= 145)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[9],2000,cancellationToken);
                        }
                        if (Countok > 145 && Countok < 148)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 147 && Countok < 150)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 149 && Countok < 155)
                    {
                        if (Countok <= 150)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[10],2000,cancellationToken);
                        }
                        if (Countok > 150 && Countok < 153)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok >= 152 && Countok < 155)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }
                    if (Countok > 154 && Countok < 160)
                    {
                        if (Countok <= 155)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSS[11],2000,cancellationToken);
                        }
                        if (Countok > 155 && Countok < 158)
                        {
                            await SendSerialCommandAsync(c1,4000,cancellationToken);
                        }
                        if (Countok > 157 && Countok < 160)
                        {
                            await  SendSerialCommandAsync("AT+QSCAN=3,1",4000,cancellationToken);
                        }
                    }

                    //add new band for 5g
                    if (Countok > 159 && Countok < 161)
                    {
                        await SendSerialCommandAsync(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71",2000,cancellationToken);
                       
                    }
                    if (Countok > 160 && Countok < 162)
                    {
                        await SendSerialCommandAsync(c4,2000,cancellationToken);//);
                       
                    }
                    if (Countok > 161 && Countok < 163)
                    {
                        await SendSerialCommandAsync(c4n,2000,cancellationToken);//);
                        Thread.Sleep(2000);
                    }
                }
            }
            catch(Exception ex)
            {

            }
            }
            #endregion

            #region RunAsyncforAll
            public async Task RunAsyncforAll(int Countok)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                // Set a global timeout for the entire operation
                cts.CancelAfter(TimeSpan.FromSeconds(30)); // e.g., 30 seconds

                await ExecuteIoTCommandAsyncforAll(cts.Token, Countok);
            }
        }
        public async Task ExecuteIoTCommandAsyncforAll(CancellationToken cancellationToken, int Countok)
        {
            try
            {
                net = "ALL";
                //outputFile = @"C:\amar\2goutput.txt";

                string c4 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                //add new cmd
                string c4n = (@"AT+QNWPREFCFG = ""nsa_nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");

                string c1 = (@"AT+QNWPREFCFG=""mode_pref"",WCDMA").Replace("\r", "").Replace("\n", "");
                // string c2 = ("At+cops=?").Replace("\r", "").Replace("\n", "");
                string c3 = (@"AT+QENG=""servingcell""").Replace("\r", "").Replace("\n", "");
                if ((selectedcmbMode) == "Fast")
                {
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if (Countok < 1)
                    {
                        await SendSerialCommandAsync(c1, 2000, cancellationToken);
                    }
                    else if (Countok >= 1 && Countok < 3)
                    {
                        await SendSerialCommandAsync(c3, 3000, cancellationToken);
                    }
                    if (Countok > 2 && Countok < 4)
                    {
                        await SendSerialCommandAsync(@"AT+QNWPREFCFG= ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71", 2000, cancellationToken);
                    }

                    if (Countok > 3 && Countok < 6)
                    {
                        await SendSerialCommandAsync("AT+QSCAN=1,1", 3000, cancellationToken);
                    }
                    if (Countok > 5 && Countok < 8)
                    {
                        await SendSerialCommandAsync("AT+QSCAN=3,1", 3000, cancellationToken);
                    }
                    if (Countok > 7 && Countok < 9)
                    {
                        await SendSerialCommandAsync(c4, 2000, cancellationToken);
                    }
                    //Add new command
                    if (Countok > 8 && Countok < 10)
                    {
                        await SendSerialCommandAsync(c4n, 2000, cancellationToken);
                    }

                    if (Countok > 9 && Countok < 12)
                    {
                        await SendSerialCommandAsync("AT+QSCAN=2,1", 2000, cancellationToken);
                    }
                    if (Countok > 11 && Countok < 14)
                    {
                        await SendSerialCommandAsync("AT+QSCAN=3,1", 2000, cancellationToken);
                    }
                }
                else
                {
                    if (Countok < 1)
                    {
                        await SendSerialCommandAsync(c1, 4000, cancellationToken);
                    }
                    else if (Countok >= 1 && Countok < 3)
                    {
                        await SendSerialCommandAsync(c3, 4000, cancellationToken);
                    }
                    if (Countok > 2 && Countok < 4)
                    {
                        await SendSerialCommandAsync(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71", 2000, cancellationToken);
                    }
                    if ((Countok >= 3) && Countok <= 5)
                    {
                        await SendSerialCommandAsync("AT+QSCAN=1,1", 2000, cancellationToken);
                    }
                    if (Countok > 5 && Countok < 8)
                    {
                        await SendSerialCommandAsync("AT+QSCAN=3,1", 2000, cancellationToken);
                    }
                    int[] bandS = { 1, 3, 5, 8, 40, 41 };
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if ((Countok > 7 && Countok < 13))
                    {

                        if (Countok > 7 && Countok < 9)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[0], 2000, cancellationToken);
                        }
                        if (Countok > 8 && Countok < 11)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1", 2000, cancellationToken);
                        }
                        if (Countok > 10 && Countok < 13)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1", 2000, cancellationToken);
                        }
                    }
                    if ((Countok > 12 && Countok < 18))
                    {
                        if (Countok > 12 && Countok < 14)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[1], 2000, cancellationToken);
                        }
                        if (Countok > 13 && Countok < 16)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1", 2000, cancellationToken);
                        }
                        if (Countok > 15 && Countok < 18)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1", 2000, cancellationToken);
                        }
                    }
                    if ((Countok > 17 && Countok < 23))
                    {
                        if (Countok > 17 && Countok < 19)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[2], 2000, cancellationToken);
                        }
                        if (Countok > 18 && Countok < 21)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1", 2000, cancellationToken);
                        }
                        if (Countok > 20 && Countok < 23)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1", 2000, cancellationToken);
                        }
                    }
                    if ((Countok > 22 && Countok < 28))
                    {
                        if (Countok > 22 && Countok < 24)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[3], 2000, cancellationToken);
                        }
                        if (Countok > 23 && Countok < 26)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1", 2000, cancellationToken);
                        }
                        if (Countok > 25 && Countok < 28)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1", 2000, cancellationToken);
                        }
                    }
                    if ((Countok > 27 && Countok < 33))
                    {
                        if (Countok > 27 && Countok < 29)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[4], 2000, cancellationToken);
                        }
                        if (Countok > 28 && Countok < 31)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1", 2000, cancellationToken);
                        }
                        if (Countok > 30 && Countok < 33)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1", 2000, cancellationToken);
                        }
                    }
                    if ((Countok > 32 && Countok < 38))
                    {
                        if (Countok > 32 && Countok < 34)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[5], 2000, cancellationToken);
                        }
                        if (Countok > 33 && Countok < 36)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1", 2000, cancellationToken);
                        }
                        if (Countok > 35 && Countok < 38)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1", 2000, cancellationToken);
                        }
                    }
                    if (Countok > 37 && Countok < 39)
                    {
                        await SendSerialCommandAsync(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71", 2000, cancellationToken);
                    }
                    string c33 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                    //add new cmd
                    string c33n = (@"AT+QNWPREFCFG = ""nsa_nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");

                    string c11 = ("AT+QSCAN=2,1").Replace("\r", "").Replace("\n", "");
                    string c2 = ("AT+QSCAN=3,1").Replace("\r", "").Replace("\n", "");

                    if (Countok > 38 && Countok < 40)
                    {
                        await SendSerialCommandAsync(c33, 2000, cancellationToken);
                    }
                    //add new command
                    if (Countok > 39 && Countok < 41)
                    {
                        await SendSerialCommandAsync(c33n, 2000, cancellationToken);
                    }
                    if (Countok > 40 && Countok < 43)
                    {
                        await SendSerialCommandAsync(c11, 2000, cancellationToken);
                    }
                    if (Countok > 42 && Countok < 45)
                    {
                        await SendSerialCommandAsync(c2, 2000, cancellationToken);
                    }
                    int[] bandSs = { 1, 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }

                    if (Countok > 44 && Countok < 50)
                    {
                        if (Countok > 44 && Countok < 46)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[0], 2000, cancellationToken);
                        }
                        if (Countok > 45 && Countok < 48)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 47 && Countok < 50)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    if (Countok > 49 && Countok < 55)
                    {
                        if (Countok > 49 && Countok < 51)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[1], 2000, cancellationToken);
                        }
                        if (Countok > 50 && Countok < 53)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 52 && Countok < 55)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    if (Countok > 54 && Countok < 60)
                    {
                        if (Countok > 54 && Countok < 56)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[2], 2000, cancellationToken);
                        }
                        if (Countok > 55 && Countok < 58)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 57 && Countok < 60)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    if (Countok > 59 && Countok < 65)
                    {
                        if (Countok > 59 && Countok < 61)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[3], 2000, cancellationToken);
                        }
                        if (Countok > 60 && Countok < 63)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 62 && Countok < 65)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    if (Countok > 64 && Countok < 70)
                    {
                        if (Countok > 64 && Countok < 66)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[4], 2000, cancellationToken);
                        }
                        if (Countok > 65 && Countok < 68)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 67 && Countok < 70)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    if (Countok > 69 && Countok < 75)
                    {
                        if (Countok > 69 && Countok < 71)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[5], 2000, cancellationToken);
                        }
                        if (Countok > 70 && Countok < 73)
                        {
                            await SendSerialCommandAsync(c11, 4000, cancellationToken);
                        }
                        if (Countok > 72 && Countok < 75)
                        {
                            await SendSerialCommandAsync(c2, 4000, cancellationToken);
                        }
                    }
                    if (Countok > 74 && Countok < 80)
                    {
                        if (Countok > 74 && Countok < 76)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[6], 2000, cancellationToken);
                        }
                        if (Countok > 75 && Countok < 78)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 77 && Countok < 80)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    if (Countok > 79 && Countok < 85)
                    {
                        if (Countok > 79 && Countok < 81)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[7], 2000, cancellationToken);
                        }
                        if (Countok > 80 && Countok < 83)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 82 && Countok < 85)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    if (Countok > 84 && Countok < 90)
                    {
                        if (Countok > 84 && Countok < 86)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[8], 2000, cancellationToken);
                        }
                        if (Countok > 85 && Countok < 88)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 87 && Countok < 90)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    if (Countok > 90 && Countok < 96)
                    {
                        if (Countok > 90 && Countok < 92)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[9], 2000, cancellationToken);
                        }
                        if (Countok > 91 && Countok < 94)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 93 && Countok < 96)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    if (Countok > 95 && Countok < 101)
                    {
                        if (Countok > 95 && Countok < 97)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[10], 2000, cancellationToken);
                        }
                        if (Countok > 96 && Countok < 99)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 98 && Countok < 101)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    if (Countok > 100 && Countok < 106)
                    {
                        if (Countok > 100 && Countok < 102)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[11], 2000, cancellationToken);
                        }
                        if (Countok > 101 && Countok < 104)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 103 && Countok < 106)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    //for new band add one by one

                    if (Countok > 105 && Countok < 111)
                    {
                        if (Countok > 105 && Countok < 107)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[0], 2000, cancellationToken);
                        }
                        if (Countok > 106 && Countok < 109)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 108 && Countok < 111)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    if (Countok > 110 && Countok < 116)
                    {
                        if (Countok > 110 && Countok < 112)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[1], 2000, cancellationToken);
                        }
                        if (Countok > 111 && Countok < 114)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 113 && Countok < 116)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    if (Countok > 115 && Countok < 121)
                    {
                        if (Countok > 115 && Countok < 117)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[2], 2000, cancellationToken);
                        }
                        if (Countok > 116 && Countok < 119)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 118 && Countok < 121)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    if (Countok > 120 && Countok < 126)
                    {
                        if (Countok > 120 && Countok < 122)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[3], 2000, cancellationToken);
                        }
                        if (Countok > 121 && Countok < 124)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 123 && Countok < 126)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    if (Countok > 125 && Countok < 131)
                    {
                        if (Countok > 125 && Countok < 127)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[4], 2000, cancellationToken);
                        }
                        if (Countok > 126 && Countok < 129)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 128 && Countok < 131)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    if (Countok > 130 && Countok < 136)
                    {
                        if (Countok > 130 && Countok < 132)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[5], 2000, cancellationToken);
                        }
                        if (Countok > 131 && Countok < 134)
                        {
                            await SendSerialCommandAsync(c11, 4000, cancellationToken);
                        }
                        if (Countok > 133 && Countok < 136)
                        {
                            await SendSerialCommandAsync(c2, 4000, cancellationToken);
                        }
                    }
                    if (Countok > 135 && Countok < 141)
                    {
                        if (Countok > 135 && Countok < 137)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[6], 2000, cancellationToken);
                        }
                        if (Countok > 136 && Countok < 139)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 138 && Countok < 141)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    if (Countok > 140 && Countok < 146)
                    {
                        if (Countok > 140 && Countok < 142)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[7], 2000, cancellationToken);
                        }
                        if (Countok > 141 && Countok < 144)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 143 && Countok < 146)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    if (Countok > 145 && Countok < 151)
                    {
                        if (Countok > 145 && Countok < 147)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[8], 2000, cancellationToken);
                        }
                        if (Countok > 146 && Countok < 149)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 148 && Countok < 151)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    if (Countok > 150 && Countok < 156)
                    {
                        if (Countok > 150 && Countok < 152)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[9], 2000, cancellationToken);
                        }
                        if (Countok > 151 && Countok < 154)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 153 && Countok < 156)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    if (Countok > 155 && Countok < 161)
                    {
                        if (Countok > 155 && Countok < 157)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[10], 2000, cancellationToken);
                        }
                        if (Countok > 156 && Countok < 159)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 158 && Countok < 161)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }

                    }
                    if (Countok > 160 && Countok < 166)
                    {
                        if (Countok > 160 && Countok < 162)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandSs[11], 2000, cancellationToken);
                        }
                        if (Countok > 161 && Countok < 164)
                        {
                            await SendSerialCommandAsync(c11, 2000, cancellationToken);
                        }
                        if (Countok > 163 && Countok < 166)
                        {
                            await SendSerialCommandAsync(c2, 2000, cancellationToken);
                        }
                    }
                    //for new band add one by one

                    if (Countok > 165 && Countok < 167)
                    {
                        await SendSerialCommandAsync(c33, 2000, cancellationToken);
                    }
                    if (Countok > 166 && Countok < 168)
                    {
                        await SendSerialCommandAsync(c33n, 3000, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
           
        }
        #endregion
        #region 3GNetwork 
        public async Task RunAsync3g(int Countok)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                // Set a global timeout for the entire operation
                cts.CancelAfter(TimeSpan.FromSeconds(30)); // e.g., 30 seconds

                await ExecuteIoTCommandAsync3G(cts.Token, Countok);
            }
        }
        public async Task ExecuteIoTCommandAsync3G(CancellationToken cancellationToken, int Countok)
        {
                try
                {
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    net = "3G";
                    string s = "\"blabla\"";
                    len = 11;
                    //outputFile = @"C:\amar\2goutput.txt";
                    string c1 = (@"AT+QNWPREFCFG=""mode_pref"",WCDMA").Replace("\r", "").Replace("\n", "");
                    // string c2 = ("At+cops=?").Replace("\r", "").Replace("\n", "");
                    string c3 = (@"AT+QENG=""servingcell""").Replace("\r", "").Replace("\n", "");
                    if ((selectedcmbMode) == "Fast")
                    {
                        if (Countok < 1)
                        {
                            await SendSerialCommandAsync(c1, 10000, cancellationToken);
                        }
                        else if (Countok >= 1 && Countok <= 3)
                        {
                            await SendSerialCommandAsync(c3, 10000, cancellationToken);
                        }
                    }
                    else
                    {
                        if (Countok < 1)
                        {
                            await SendSerialCommandAsync(c1,10000,cancellationToken);
                        }
                        else if (Countok >= 1 && Countok < 6)
                        {
                            await SendSerialCommandAsync(c3,10000,cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
                
            }
            #endregion

            #region 4GNetwork
            public async Task RunAsync4g(int Countok)
            {
                using (CancellationTokenSource cts = new CancellationTokenSource())
                {
                    // Set a global timeout for the entire operation
                    cts.CancelAfter(TimeSpan.FromSeconds(30)); // e.g., 30 seconds

                    await ExecuteIoTCommandAsync4G(cts.Token, Countok);
                }
            }

            public async Task ExecuteIoTCommandAsync4G(CancellationToken cancellationToken, int Countok)
            {
                try
                {
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected. Scan will stop.");
                        return;
                    }

                    if ((selectedcmbMode) == "Fast")
                    {
                        if (Countok < 1 && Countok < 2)
                        {
                            await SendSerialCommandAsync(
                          @"AT+QNWPREFCFG= ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71", 5000, cancellationToken);

                        }
                        if (Countok >= 1 && Countok < 3)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1", 10000, cancellationToken);

                        }
                        if (Countok > 2 && Countok < 5)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1", 10000, cancellationToken);

                        }
                        //await Task.Run(() => 
                    }
                    else
                    {
                        if (Countok < 1 && Countok < 2)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71", 4000, cancellationToken);
                        }
                        if ((Countok >= 1 || Countok < 1) && Countok <= 3)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=1,1", 4000, cancellationToken);
                        }
                        if (Countok > 3 && Countok < 6)
                        {
                            await SendSerialCommandAsync("AT+QSCAN=3,1", 4000, cancellationToken);
                        }
                        int[] bandS = { 1, 3, 5, 8, 40, 41 };
                        if (!serialPort2.IsOpen)
                        {
                            MessageBox.Show("Device is not connected.Scan will stop");
                            return;
                        }
                        if ((Countok >= 6 && Countok < 11))
                        {

                            if (Countok >= 6 && Countok < 7)
                            {
                                await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[0], 3000, cancellationToken);
                            }
                            if (Countok > 6 && Countok < 9)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=1,1", 5000, cancellationToken);
                            }
                            if (Countok >= 9 && Countok < 11)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=3,1", 4000, cancellationToken);
                            }
                        }
                        if ((Countok >= 11 && Countok < 16))
                        {
                            if (Countok >= 11 && Countok < 12)
                            {
                                await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[1], 3000, cancellationToken);
                            }
                            if (Countok > 11 && Countok < 14)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=1,1", 4000, cancellationToken);
                            }
                            if (Countok >= 14 && Countok < 16)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=3,1", 4000, cancellationToken);
                            }
                        }
                        if ((Countok >= 16 && Countok < 21))
                        {
                            if (Countok >= 16 && Countok < 17)
                            {
                                await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[2], 3000, cancellationToken);
                            }
                            if (Countok > 16 && Countok < 19)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=1,1", 4000, cancellationToken);
                            }
                            if (Countok >= 19 && Countok < 21)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=3,1", 4000, cancellationToken);
                            }
                        }
                        if ((Countok >= 21 && Countok < 26))
                        {
                            if (Countok >= 21 && Countok < 22)
                            {
                                await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[3], 3000, cancellationToken);
                            }
                            if (Countok > 21 && Countok < 24)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=1,1", 4000, cancellationToken);
                            }
                            if (Countok >= 24 && Countok < 26)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=3,1", 4000, cancellationToken);
                            }
                        }
                        if ((Countok >= 26 && Countok < 31))
                        {
                            if (Countok >= 26 && Countok < 27)
                            {
                                await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[4], 2000, cancellationToken);
                            }
                            if (Countok > 26 && Countok < 29)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=1,1", 4000, cancellationToken);
                            }
                            if (Countok >= 29 && Countok < 31)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=3,1", 4000, cancellationToken);
                            }
                        }
                        if ((Countok >= 31 && Countok < 36))
                        {
                            if (Countok >= 31 && Countok < 32)
                            {
                                await SendSerialCommandAsync(@"AT+QNWPREFCFG=""lte_band""," + bandS[5], 3000, cancellationToken);
                            }
                            if (Countok > 31 && Countok < 34)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=1,1", 4000, cancellationToken);
                            }
                            if (Countok >= 34 && Countok < 36)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=3,1", 4000, cancellationToken);
                            }
                        }

                        if (Countok <= 38)
                        {
                            await SendSerialCommandAsync(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71", 3000, cancellationToken);

                        }
                    }


                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Operation was canceled due to a timeout or user request.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }

            #endregion

            #region 5GNetwork
            public async Task RunAsync5g(int Countok)
            {
                using (CancellationTokenSource cts = new CancellationTokenSource())
                {
                    // Set a global timeout for the entire operation
                    cts.CancelAfter(TimeSpan.FromSeconds(30)); // e.g., 30 seconds
                    await ExecuteIoTCommandAsync5G(cts.Token, Countok);
                }
            }

            public async Task ExecuteIoTCommandAsync5G(CancellationToken cancellationToken, int Countok)
            {
                try
                {
                    try
                    {
                        if (!serialPort2.IsOpen)
                        {
                            MessageBox.Show("Device is not connected.Scan will stop");
                            return;
                        }
                        //string f = @"Joe said ""Hello"" to me";
                        net = "5G";
                        len = 11;
                        //outputFile = @"C:\amar\2goutput.txt";
                        string c3 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                        string c4 = (@"AT+QNWPREFCFG = ""nsa_nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");

                        string c1 = ("AT+QSCAN=2,1").
                            Replace("\r", "").Replace("\n", "");
                        string c2 = ("AT+QSCAN=3,1").Replace("\r", "").Replace("\n", "");
                        if ((selectedcmbMode) == "Fast")
                        {
                            if (Countok < 1)
                            {
                                await SendSerialCommandAsync(c3, 4000, cancellationToken);//);
                            }

                            if (Countok >= 1 && Countok < 3)
                            {
                                await SendSerialCommandAsync(c4, 4000, cancellationToken);
                            }

                            if (Countok >= 3 && Countok < 5)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=2,1", 10000, cancellationToken);
                            }
                            if (Countok > 4 && Countok < 8)
                            {
                                await SendSerialCommandAsync("AT+QSCAN=3,1", 5000, cancellationToken);
                            }
                        }
                        else
                        {
                            if (Countok < 1)
                            {
                                await SendSerialCommandAsync(c3, 4000, cancellationToken);//);

                            }
                            if (Countok >= 1 && Countok < 2)
                            {
                                await SendSerialCommandAsync(c4, 3000, cancellationToken);
                            }

                            if (Countok >= 2 && Countok < 4)
                            {
                                await SendSerialCommandAsync(c1, 4000, cancellationToken);
                            }
                            if (Countok > 3 && Countok <= 6)
                            {
                                await SendSerialCommandAsync(c2, 4000, cancellationToken);
                            }
                            int[] bandS = { 1, 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };
                            if (!serialPort2.IsOpen)
                            {
                                MessageBox.Show("Device is not connected.Scan will stop");
                                return;
                            }
                            if (Countok > 6 && Countok < 12)
                            {
                                if (Countok <= 7)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[0], 3000, cancellationToken);
                                }
                                if (Countok > 7 && Countok <= 9)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 9 && Countok < 12)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }
                            }
                            if (Countok > 11 && Countok < 17)
                            {
                                if (Countok <= 12)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[1], 2000, cancellationToken);
                                }
                                if (Countok > 12 && Countok <= 14)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 14 && Countok < 17)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 16 && Countok < 22)
                            {
                                if (Countok <= 17)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[2], 2000, cancellationToken);
                                }
                                if (Countok > 17 && Countok <= 19)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 19 && Countok < 22)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 21 && Countok < 27)
                            {
                                if (Countok <= 22)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[3], 2000, cancellationToken);
                                }
                                if (Countok > 22 && Countok <= 24)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 24 && Countok < 27)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 26 && Countok < 32)
                            {
                                if (Countok <= 27)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[4], 2000, cancellationToken);
                                }
                                if (Countok > 27 && Countok <= 29)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 29 && Countok < 32)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }
                            }
                            if (Countok > 31 && Countok < 37)
                            {
                                if (Countok <= 32)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[5], 2000, cancellationToken);
                                }
                                if (Countok > 32 && Countok <= 35)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 35 && Countok < 38)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 37 && Countok < 43)
                            {
                                if (Countok <= 38)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[6], 2000, cancellationToken);
                                }
                                if (Countok > 38 && Countok <= 40)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 40 && Countok < 43)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }
                            }
                            if (Countok > 42 && Countok < 48)
                            {
                                if (Countok <= 43)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[7], 2000, cancellationToken);
                                }
                                if (Countok > 43 && Countok <= 45)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 45 && Countok < 48)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 47 && Countok < 53)
                            {
                                if (Countok <= 48)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[8], 2000, cancellationToken);
                                }
                                if (Countok > 48 && Countok <= 50)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 50 && Countok < 53)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }
                            }
                            if (Countok > 52 && Countok < 58)
                            {
                                if (Countok <= 53)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[9], 2000, cancellationToken);
                                }
                                if (Countok > 53 && Countok <= 55)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 55 && Countok < 58)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 57 && Countok < 63)
                            {
                                if (Countok <= 58)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[10], 2000, cancellationToken);
                                }
                                if (Countok > 58 && Countok <= 60)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 60 && Countok < 63)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 62 && Countok < 68)
                            {
                                if (Countok <= 63)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[11], 2000, cancellationToken);
                                }
                                if (Countok > 63 && Countok <= 65)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 65 && Countok < 68)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }
                            }
                            //for add new command
                            if (Countok > 67 && Countok < 73)
                            {
                                if (Countok <= 67)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[0], 2000, cancellationToken);
                                }
                                if (Countok > 68 && Countok <= 70)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 69 && Countok < 72)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 71 && Countok < 78)
                            {
                                if (Countok <= 72)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[1], 2000, cancellationToken);
                                }
                                if (Countok > 72 && Countok <= 74)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 74 && Countok < 77)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 76 && Countok < 82)
                            {
                                if (Countok <= 77)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[2], 2000, cancellationToken);
                                }
                                if (Countok > 77 && Countok <= 79)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 79 && Countok < 82)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 81 && Countok < 87)
                            {
                                if (Countok <= 82)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[3], 2000, cancellationToken);
                                }
                                if (Countok > 82 && Countok <= 84)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 84 && Countok < 87)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 86 && Countok < 92)
                            {
                                if (Countok <= 87)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[4], 2000, cancellationToken);
                                }
                                if (Countok > 87 && Countok <= 89)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 89 && Countok < 92)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 91 && Countok < 97)
                            {
                                if (Countok <= 92)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[5], 2000, cancellationToken);
                                }
                                if (Countok > 92 && Countok <= 95)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 95 && Countok < 98)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 97 && Countok < 103)
                            {
                                if (Countok <= 98)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[6], 2000, cancellationToken);
                                }
                                if (Countok > 98 && Countok <= 100)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 100 && Countok < 103)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }
                            }
                            if (Countok > 102 && Countok < 108)
                            {
                                if (Countok <= 103)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[7], 2000, cancellationToken);
                                }
                                if (Countok > 103 && Countok <= 105)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 105 && Countok < 108)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }
                            }
                            if (Countok > 107 && Countok < 113)
                            {
                                if (Countok <= 108)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[8], 2000, cancellationToken);
                                }
                                if (Countok > 108 && Countok <= 110)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 110 && Countok < 113)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }
                            }
                            if (Countok > 112 && Countok < 118)
                            {
                                if (Countok <= 113)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[9], 2000, cancellationToken);
                                }
                                if (Countok > 113 && Countok <= 115)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 115 && Countok < 118)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }
                            }
                            if (Countok > 117 && Countok < 123)
                            {
                                if (Countok <= 118)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[10], 2000, cancellationToken);
                                }
                                if (Countok > 118 && Countok <= 120)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 120 && Countok < 123)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            if (Countok > 122 && Countok < 128)
                            {


                                if (Countok <= 123)
                                {
                                    await SendSerialCommandAsync(@"AT+QNWPREFCFG=""nsa_nr5g_band""," + bandS[11], 2000, cancellationToken);
                                }
                                if (Countok > 123 && Countok <= 125)
                                {
                                    await SendSerialCommandAsync(c1, 4000, cancellationToken);
                                }
                                if (Countok > 125 && Countok < 128)
                                {
                                    await SendSerialCommandAsync(c2, 4000, cancellationToken);
                                }

                            }
                            //for add new command
                            if (Countok >= 127 && Countok < 129)
                            {
                                await SendSerialCommandAsync(c3, 4000, cancellationToken);
                            }

                            if (Countok >= 128 && Countok < 130)
                            {
                                await SendSerialCommandAsync(c4, 3000, cancellationToken);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Operation was canceled due to a timeout or user request.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            #endregion

            void serialWrite(string cmd)
            {
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
            #endregion
            private void Dashboard5G_FormClosing(object sender, FormClosingEventArgs e)
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
                if (data.Contains("QSCAN:") || data.Contains("LTE") || data.Contains("NR5G") || data.Contains("QENG"))
                {
                    info = "true";
                    typ = "ccin";
                }
                //if (data.Contains("+CMGRMI:"))
                //{
                //    typ = "cmg";
                //    len = 15;
                //}
                String[] datas = data.Split(new char[] { ',' });
                //if (datas.Length < len)
                //    return null;
                foreach (var val in datas)
                {
                    if (string.IsNullOrEmpty(val) || val.Contains("QSCAN") || val.Contains("LTE") || val.Contains("WCDMA") || val.Contains(@"QNWPREFCFG=""mode_pref""")
                        || val.Contains(@"AT+QENG=""servingcell""") || val.Contains(@"+QENG: ""servingcell"""))
                        continue;

                    if (a == "4G" && net == "4G" && data.Contains("LTE"))
                    {
                        String[] vals = val.Split(':');

                        if (!map.ContainsKey(vals[0]))
                        {
                            try
                            {
                                if (!map.ContainsKey("mcc"))
                                {
                                    if (datas[1] == vals[0])
                                        map.Add("mcc", vals[0].Trim());
                                }
                                if (!map.ContainsKey("mnc"))
                                {
                                    if (datas[2] == vals[0])
                                        map.Add("mnc", vals[0].Trim());
                                }
                                if (!map.ContainsKey("lac"))
                                {
                                    if (datas[10] == vals[0])
                                        map.Add("lac", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("arfcn"))
                                {
                                    if (datas[3] == vals[0])
                                        map.Add("arfcn", vals[0].Trim());
                                }
                                if (!map.ContainsKey("bsic"))
                                {
                                    if (datas[4] == vals[0])
                                        map.Add("bsic", vals[0].Trim());
                                }

                                if (!map.ContainsKey("dBm"))
                                {
                                    if (datas[5] == vals[0])
                                        map.Add("dBm", vals[0].Trim());
                                }

                                if (!map.ContainsKey("cellid"))
                                {
                                    if (datas[9] == vals[0])
                                        map.Add("cellid", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("tac"))
                                {
                                    if (datas[10] == vals[0])
                                        map.Add("tac", vals[0].Trim());
                                }
                                if (!map.ContainsKey("net"))
                                {
                                    if (datas[0].Contains("NR5G"))
                                        map.Add("net", "5G");
                                    if (datas[0].Contains("LTE"))
                                        map.Add("net", "4G");
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    else if (a == "5G" && net == "5G" && data.Contains("NR5G"))
                    {
                        String[] vals = val.Split(':');

                        if (!map.ContainsKey(vals[0]))
                        {
                            try
                            {
                                if (!map.ContainsKey("mcc"))
                                {
                                    if (datas[1] == vals[0])
                                        map.Add("mcc", vals[0].Trim());
                                }
                                if (!map.ContainsKey("mnc"))
                                {
                                    if (datas[2] == vals[0])
                                        map.Add("mnc", vals[0].Trim());
                                }
                                if (!map.ContainsKey("lac"))
                                {
                                    if (datas[10] == vals[0])
                                        map.Add("lac", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("cellid"))
                                {
                                    if (datas[9] == vals[0])
                                        map.Add("cellid", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("arfcn"))
                                {
                                    if (datas[3] == vals[0])
                                        map.Add("arfcn", vals[0].Trim());
                                }
                                if (!map.ContainsKey("bsic"))
                                {
                                    if (datas[4] == vals[0])
                                        map.Add("bsic", vals[0].Trim());
                                }
                                if (!map.ContainsKey("dBm"))
                                {
                                    if (datas[5] == vals[0])
                                        map.Add("dBm", vals[0].Trim());
                                }
                                if (!map.ContainsKey("tac"))
                                {
                                    if (datas[10] == vals[0])
                                        map.Add("tac", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("net"))
                                {
                                    if (datas[0].Contains("NR5G"))
                                        map.Add("net", "5G");
                                    if (datas[0].Contains("LTE"))
                                        map.Add("net", "4G");
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    else if (a == "4G + 5G")
                    {
                        String[] vals = val.Split(':');

                        if (!map.ContainsKey(vals[0]))
                        {
                            try
                            {
                                if (!map.ContainsKey("mcc"))
                                {
                                    if (datas[1] == vals[0])
                                        map.Add("mcc", vals[0].Trim());
                                }
                                if (!map.ContainsKey("mnc"))
                                {
                                    if (datas[2] == vals[0])
                                        map.Add("mnc", vals[0].Trim());
                                }
                                if (!map.ContainsKey("lac"))
                                {
                                    if (datas[10] == vals[0])
                                        map.Add("lac", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("cellid"))
                                {
                                    if (datas[9] == vals[0])
                                        map.Add("cellid", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("arfcn"))
                                {
                                    if (datas[3] == vals[0])
                                        map.Add("arfcn", vals[0].Trim());
                                }
                                if (!map.ContainsKey("bsic"))
                                {
                                    if (datas[4] == vals[0])
                                        map.Add("bsic", vals[0].Trim());
                                }
                                if (!map.ContainsKey("dBm"))
                                {
                                    if (datas[5] == vals[0])
                                        map.Add("dBm", vals[0].Trim());
                                }
                                if (!map.ContainsKey("tac"))
                                {
                                    if (datas[10] == vals[0])
                                        map.Add("tac", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("net"))
                                {
                                    if (datas[0].Contains("NR5G"))
                                        map.Add("net", "5G");
                                    if (datas[0].Contains("LTE"))
                                        map.Add("net", "4G");
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    else if (a == "3G")
                    {
                        // +QENG: "servingcell","LIMSRV","WCDMA",404,59,1C21,13DA00C,10732,114,42,-67,-8,-,-,-,-,- AT+QENG=\"servingcell\
                        //In WCDMA mode:
                        //+QENG:"servingcell",<state>,"WCDMA",<MCC>,<MNC>,<LAC>,<cellID>,<uarfcn>,<PSC>,<RAC>,<RSCP>,<ecio>,<phych>,<SF>,<slot>,<speech_code>,<comMod>
                        //                        0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17
                        String[] vals = val.Split(':');

                        if (!map.ContainsKey(vals[0]))
                        {
                            try
                            {
                                if (!map.ContainsKey("mcc"))
                                {
                                    if (datas[3] == vals[0])
                                        map.Add("mcc", vals[0].Trim());
                                }
                                if (!map.ContainsKey("mnc"))
                                {
                                    if (datas[4] == vals[0])
                                        map.Add("mnc", vals[0].Trim());
                                }
                                if (!map.ContainsKey("lac"))
                                {
                                    if (datas[5] == vals[0])
                                        map.Add("lac", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("cellid"))
                                {
                                    if (datas[6] == vals[0])
                                        map.Add("cellid", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("arfcn"))
                                {
                                    if (datas[7] == vals[0])
                                        map.Add("arfcn", vals[0].Trim());
                                }
                                if (!map.ContainsKey("bsic"))
                                {
                                    if (datas[8] == vals[0])
                                        map.Add("bsic", vals[0].Trim());
                                }
                                if (!map.ContainsKey("tac"))
                                {
                                    if (datas[5] == vals[0])
                                        map.Add("tac", vals[0].Trim());
                                }
                                if (!map.ContainsKey("dBm"))
                                {
                                    if (datas[10] == vals[0])
                                        map.Add("dBm", hexToInteger(vals[0].Trim()));
                                }
                                if (!map.ContainsKey("net"))
                                {
                                    map.Add("net", "3G");
                                }
                                // row["(A/E/U)RFCN"] = map["arfcn"];
                                // row["ENB"] = map["dBm"];btn
                                // row["ECI"] = map["cellid"];
                                // row["Network Type"] = map["net"];
                                // row["BSIC/PSC/PCI"] = map["bsic"];
                                //row["DBM"] = map["dBm"];
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    else if (a == "ALL")
                    {
                        if (data.Contains("LTE"))
                        {
                            String[] vals = val.Split(':');

                            if (!map.ContainsKey(vals[0]))
                            {
                                try
                                {
                                    if (!map.ContainsKey("mcc"))
                                    {
                                        if (datas[1] == vals[0])
                                            map.Add("mcc", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("mnc"))
                                    {
                                        if (datas[2] == vals[0])
                                            map.Add("mnc", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("lac"))
                                    {
                                        if (datas[10] == vals[0])
                                            map.Add("lac", hexToInteger(vals[0].Trim()));
                                    }
                                    if (!map.ContainsKey("arfcn"))
                                    {
                                        if (datas[3] == vals[0])
                                            map.Add("arfcn", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("bsic"))
                                    {
                                        if (datas[4] == vals[0])
                                            map.Add("bsic", vals[0].Trim());
                                    }

                                    if (!map.ContainsKey("dBm"))
                                    {
                                        if (datas[5] == vals[0])
                                            map.Add("dBm", vals[0].Trim());
                                    }

                                    if (!map.ContainsKey("cellid"))
                                    {
                                        if (datas[9] == vals[0])
                                            map.Add("cellid", hexToInteger(vals[0].Trim()));
                                    }
                                    if (!map.ContainsKey("tac"))
                                    {
                                        if (datas[10] == vals[0])
                                            map.Add("tac", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("net"))
                                    {
                                        if (datas[0].Contains("NR5G"))
                                            map.Add("net", "5G");
                                        if (datas[0].Contains("LTE"))
                                            map.Add("net", "4G");
                                    }

                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }

                        else if (data.Contains("NR5G"))
                        {
                            String[] vals = val.Split(':');

                            if (!map.ContainsKey(vals[0]))
                            {
                                try
                                {
                                    if (!map.ContainsKey("mcc"))
                                    {
                                        if (datas[1] == vals[0])
                                            map.Add("mcc", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("mnc"))
                                    {
                                        if (datas[2] == vals[0])
                                            map.Add("mnc", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("lac"))
                                    {
                                        if (datas[10] == vals[0])
                                            map.Add("lac", hexToInteger(vals[0].Trim()));
                                    }
                                    if (!map.ContainsKey("cellid"))
                                    {
                                        if (datas[9] == vals[0])
                                            map.Add("cellid", hexToInteger(vals[0].Trim()));
                                    }
                                    if (!map.ContainsKey("arfcn"))
                                    {
                                        if (datas[3] == vals[0])
                                            map.Add("arfcn", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("bsic"))
                                    {
                                        if (datas[4] == vals[0])
                                            map.Add("bsic", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("dBm"))
                                    {
                                        if (datas[5] == vals[0])
                                            map.Add("dBm", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("tac"))
                                    {
                                        if (datas[10] == vals[0])
                                            map.Add("tac", hexToInteger(vals[0].Trim()));
                                    }
                                    if (!map.ContainsKey("net"))
                                    {
                                        if (datas[0].Contains("NR5G"))
                                            map.Add("net", "5G");
                                        if (datas[0].Contains("LTE"))
                                            map.Add("net", "4G");
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        else
                        {
                            // +QENG: "servingcell","LIMSRV","WCDMA",404,59,1C21,13DA00C,10732,114,42,-67,-8,-,-,-,-,- AT+QENG=\"servingcell\
                            //In WCDMA mode:
                            //+QENG:"servingcell",<state>,"WCDMA",<MCC>,<MNC>,<LAC>,<cellID>,<uarfcn>,<PSC>,<RAC>,<RSCP>,<ecio>,<phych>,<SF>,<slot>,<speech_code>,<comMod>
                            //                        0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17
                            String[] vals = val.Split(':');

                            if (!map.ContainsKey(vals[0]))
                            {
                                try
                                {
                                    if (!map.ContainsKey("mcc"))
                                    {
                                        if (datas[3] == vals[0])
                                            map.Add("mcc", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("mnc"))
                                    {
                                        if (datas[4] == vals[0])
                                            map.Add("mnc", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("lac"))
                                    {
                                        if (datas[5] == vals[0])
                                            map.Add("lac", hexToInteger(vals[0].Trim()));
                                    }
                                    if (!map.ContainsKey("cellid"))
                                    {
                                        if (datas[6] == vals[0])
                                            map.Add("cellid", hexToInteger(vals[0].Trim()));
                                    }
                                    if (!map.ContainsKey("arfcn"))
                                    {
                                        if (datas[7] == vals[0])
                                            map.Add("arfcn", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("bsic"))
                                    {
                                        if (datas[8] == vals[0])
                                            map.Add("bsic", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("tac"))
                                    {
                                        if (datas[5] == vals[0])
                                            map.Add("tac", vals[0].Trim());
                                    }
                                    if (!map.ContainsKey("dBm"))
                                    {
                                        if (datas[10] == vals[0])
                                            map.Add("dBm", hexToInteger(vals[0].Trim()));
                                    }
                                    if (!map.ContainsKey("net"))
                                    {
                                        map.Add("net", "3G");
                                    }
                                    // row["(A/E/U)RFCN"] = map["arfcn"];
                                    // row["ENB"] = map["dBm"];
                                    // row["ECI"] = map["cellid"];
                                    // row["Network Type"] = map["net"];
                                    // row["BSIC/PSC/PCI"] = map["bsic"];
                                    //row["DBM"] = map["dBm"];
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
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
                    if (line.Trim().ToUpper().Contains("OK"))
                    {
                        Countok++;
                        // count
                        lockk = false;
                        serialWrite(null);
                        break;
                    }

                    if (line.Contains("Network survey end") && selectedMode == "Spot")
                    {
                        //count2G++;
                    }

                    else if (line.Contains("ERROR"))
                    {
                        Countok++;
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
                //lblRegion.Invoke((MethodInvoker)delegate
                //{
                //    lblRegion.Text = "Region : Searching Region ...";
                //});
                //if (region == "NA")
                //{
                //    getRegion();
                //}
            }
            private void regionloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
            {
                try
                {
                    this.Progrsbr.Value = e.ProgressPercentage;
                }
                catch (Exception ex)
                { }
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
                try
                {
                    if (serialPort2 != null)
                    {
                        if (serialPort2.IsOpen)
                        {
                            serialPort2.BaseStream.Dispose();
                        }
                        serialPort2.Dispose();
                        //serialPort2 = null;
                    }
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
                        serialPort2.PortName = port.Trim();
                        serialPort2.BaudRate = 115200;
                        Thread.Sleep(1000);
                        //serialPort2.Parity = Parity.None;
                        //serialPort2.DataBits = 8;
                        //serialPort2.StopBits = StopBits.One;
                        serialPort2.Open();
                    }
                }
                catch (Exception ex)
                {
                }
                return port;
            }

            private void DdlMode_SelectedIndexChanged(object sender, EventArgs e)
            {
                List<TypeText> TypeList = new List<TypeText>();
                TypeList.Clear();

                TypeText selectedNetwork = DdlMode.SelectedItem as TypeText;
                a = selectedNetwork.Name;
                // wmic path win32_pnpentity get caption /format:table |find "AT Port"
                if ((selectedNetwork.Name.ToString() == "Route")) //|| (DdlMode.SelectedItem.ToString() == "Spot"))
                {
                    Progrsbr.Invoke((MethodInvoker)delegate
                    {
                        Progrsbr.Visible = false;
                    });
                    TypeList.Add(new TypeText { Name = "Select" });
                    TypeList.Add(new TypeText { Name = "3G" });
                    TypeList.Add(new TypeText { Name = "4G" });
                    TypeList.Add(new TypeText { Name = "5G" });
                    TypeList.Add(new TypeText { Name = "4G + 5G" });
                    TypeList.Add(new TypeText { Name = "ALL" });
                    metroComboBox1.Enabled = true;
                    metroComboBox1.DataSource = TypeList;
                    metroComboBox1.DisplayMember = "Name";
                    cmbMode.Enabled = false;

                }
                else if ((selectedNetwork.Name.ToString() == "Spot")) //|| (DdlMode.SelectedItem.ToString() == "Spot"))
                {
                    Progrsbr.Invoke((MethodInvoker)delegate
                    {
                        Progrsbr.Visible = true;
                    });
                    TypeList.Add(new TypeText { Name = "Select" });
                    TypeList.Add(new TypeText { Name = "3G" });
                    TypeList.Add(new TypeText { Name = "4G" });
                    TypeList.Add(new TypeText { Name = "5G" });
                    TypeList.Add(new TypeText { Name = "4G + 5G" });
                    TypeList.Add(new TypeText { Name = "ALL" });
                    metroComboBox1.Enabled = true;
                    metroComboBox1.DataSource = TypeList;
                    metroComboBox1.DisplayMember = "Name";
                    metroComboBox1.ValueMember = "Name";
                    cmbMode.Enabled = true;
                }
            }
            private void cmbMode_SelectedIndexChanged(object sender, EventArgs e)
            {
                List<TypeText> TypeList = new List<TypeText>();
                TypeList.Clear();
                if (cmbMode.SelectedItem.ToString() == "Deep")
                {
                    TypeList.Add(new TypeText { Name = "Select" });
                    TypeList.Add(new TypeText { Name = "Spot" });

                    DdlMode.Enabled = true;
                    DdlMode.DataSource = TypeList;
                    DdlMode.DisplayMember = "Name";
                }
                else if (cmbMode.SelectedItem.ToString() == "Fast")
                {
                    TypeList.Add(new TypeText { Name = "Select" });
                    TypeList.Add(new TypeText { Name = "Spot" });
                    TypeList.Add(new TypeText { Name = "Route" });
                    DdlMode.Enabled = true;
                    DdlMode.DataSource = TypeList;
                    DdlMode.DisplayMember = "Name";
                }
            }

            private void timer_Tick(object sender, EventArgs e)
            {
                lblDate.Text = System.DateTime.Now.ToString();
            }
            //  [STAThread]
            private void btnSave_Click(object sender, EventArgs e)
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {
                            string folderPath = "";
                            //Thread t = new Thread((ThreadStart)(() =>
                            //{

                            // wmic path win32_pnpentity get caption /format:table |find "AT Port"
                            //Exporting to Excel
                            //try
                            //{
                            //  var dr = sfdExcel.ShowDialog();
                            //sfdExcel.ShowDialog();
                            // folderPath = sfdExcel.FileName;
                            //}
                            //catch (Exception ex)
                            //{
                            //    MessageBox.Show(ex.Message.ToString());
                            //}
                            DataTable tblFilteredJio = new DataTable();
                            DataTable tblFilteredAirtel = new DataTable();
                            DataTable tblFilteredVodafoneIdea = new DataTable();
                            DataTable tblFilteredCellone = new DataTable();
                            DataTable tblother = new DataTable();
                            DataTable tblAll = new DataTable();
                            DataSet ds = new DataSet();
                            tblAll = dt.AsEnumerable().CopyToDataTable();
                            ds.Tables.Add(tblAll);
                            string[] tabName = { "All", "Jio", "Airtel", "Vodafone Idea", "Cellone", "Other" };
                            try
                            {
                                tblFilteredJio = dt.AsEnumerable().Where(r => r.Field<string>("Operator Name") == "Jio").CopyToDataTable();
                                ds.Tables.Add(tblFilteredJio);
                            }
                            catch (Exception ex)
                            {
                                ds.Tables.Add(tblFilteredJio);
                            }
                            try
                            {
                                tblFilteredAirtel = dt.AsEnumerable().Where(r => r.Field<string>("Operator Name") == "Airtel").CopyToDataTable();
                                ds.Tables.Add(tblFilteredAirtel);
                            }
                            catch (Exception ex)
                            {
                                ds.Tables.Add(tblFilteredAirtel);
                            }
                            try
                            {
                                tblFilteredVodafoneIdea = dt.AsEnumerable().Where(r => r.Field<string>("Operator Name") == "Vodafone Idea").CopyToDataTable();
                                ds.Tables.Add(tblFilteredVodafoneIdea);
                            }
                            catch (Exception ex)
                            {
                                ds.Tables.Add(tblFilteredVodafoneIdea);
                            }
                            try
                            {
                                tblFilteredCellone = dt.AsEnumerable().Where(r => r.Field<string>("Operator Name") == "Cellone").CopyToDataTable();
                                ds.Tables.Add(tblFilteredCellone);
                            }
                            catch (Exception ex)
                            {
                                ds.Tables.Add(tblFilteredCellone);
                            }
                            try
                            {
                                tblother = dt.AsEnumerable()
                                       .Where(r => r.Field<string>("Operator Name") != "Airtel")
                                       .Where(r => r.Field<string>("Operator Name") != "Jio")
                                       .Where(x => x.Field<string>("Operator Name") != "Vodafone Idea")
                                       .Where(x => x.Field<string>("Operator Name") != "Cellone")
                                       .CopyToDataTable();
                                ds.Tables.Add(tblother);
                            }
                            catch (Exception ex)
                            {
                                ds.Tables.Add(tblother);
                            }
                            try
                            {
                                folderPath = GetfileData();
                                using (XLWorkbook wb = new XLWorkbook())
                                {
                                    for (int i = 0; i < ds.Tables.Count; i++)
                                    {
                                        wb.Worksheets.Add(ds.Tables[i], tabName[i].ToString());
                                        wb.SaveAs(folderPath);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString());
                            }
                            MessageBox.Show("File successfully saved on below Path" + Environment.NewLine + folderPath);
                            //}));
                            System.Diagnostics.Process.Start(folderPath);
                            //// Run your code from a thread that joins the STA Thread
                            //t.SetApartmentState(ApartmentState.STA);
                            //t.Start();
                            //t.Join();
                            //Export2Excel(dt, net);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }

                    }
                }
                else
                {
                    MessageBox.Show("No data found");
                }
            }
            #region MyRegion
            public string GetfileData()
            {
                string filepath = "";

                var dir = @"C:\CligenceExcel";  // folder location
                filepath = "C:\\CligenceExcel\\" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "-CligenceExcelReport" + ".xlsx";

                // folder location
                //  var directoryInfo = new DirectoryInfo("C:\\Sys\\");

                if (!Directory.Exists(dir))
                {
                    // if it doesn't exist, create
                    try
                    {
                        Directory.CreateDirectory(dir);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                return filepath;



            }
            #endregion
            public bool FileExists(string fileName)
            {
                var dir = @"C:\CligenceExcel";
                var workingDirectory = dir;
                var file = $"{workingDirectory}\\{fileName}";
                return File.Exists(file);
            }

            [STAThread]
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

            private void btnClear_Click(object sender, EventArgs e)
            {
                if (lockk == false)
                {
                    dt.Clear();
                    scannedCellId.Clear();
                    //metroGrid1.Rows.Clear();
                    metroGrid1.DataSource = null;
                    Progrsbr.Invoke((MethodInvoker)delegate
                    {
                    // int per = (int)(((double)(Progrsbr.Value - Progrsbr.Minimum) / (double)(Progrsbr.Maximum - Progrsbr.Minimum)) * 100);
                    Progrsbr.Value = Progrsbr.Minimum + 1; // Temporarily set it to just above the minimum
                    Progrsbr.Value = Progrsbr.Minimum;
                        Progrsbr.Value = 0;

                    });
                    //  metroComboBox1.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Command is in progress, Please try again later.");

                }
            }


        private static readonly HttpClient client = new HttpClient();

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private void serialPort2_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialPinChange.CDChanged:
                    MessageBox.Show("Carrier Detect (CD) pin changed.");
                    break;
                case SerialPinChange.CtsChanged:
                    MessageBox.Show("Clear-to-Send (CTS) pin changed.");
                    break;
                case SerialPinChange.DsrChanged:
                    MessageBox.Show("Data Set Ready (DSR) pin changed.");
                    break;
                case SerialPinChange.Ring:
                    MessageBox.Show("Ring indicator (RI) pin changed.");
                    break;
                default:
                    MessageBox.Show("Unknown pin change event.");
                    break;
            }
            //  MessageBox.Show("Pin changed: " + e.EventType);
        }

        private void serialPort2_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            MessageBox.Show("Error received: " + e.EventType);


        }

        private void metroGrid1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                try
                {
                    string data = GetSelectedCellsData(metroGrid1);
                    Clipboard.SetText(data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error copying data: " + ex.Message);
                }
            }
        }
        private string GetSelectedCellsData(MetroFramework.Controls.MetroGrid grid)
        {
            // Customize this to suit your needs
            var selectedCells = grid.SelectedCells;
            string clipboardData = "";

            foreach (DataGridViewCell cell in selectedCells)
            {
                clipboardData += cell.Value?.ToString() + "\t"; // Tab-separated values
            }

            return clipboardData;
        }
        #region SerialportTask
        static void USBDeviceChangeHandler(object sender, EventArrivedEventArgs e)
        {
            int eventType = int.Parse(e.NewEvent.GetPropertyValue("EventType").ToString());
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.GetPropertyValue("TargetInstance");

            if (eventType == 2) // Device removed
            {
                string deviceName = targetInstance["Name"].ToString();
                MessageBox.Show($"Device {deviceName} removed.");
                // Add your logic here for handling device removal
            }
            else if (eventType == 3) // Device inserted
            {
                string deviceName = targetInstance["Name"].ToString();
                MessageBox.Show($"Device {deviceName} inserted.");
                // Add your logic here for handling device insertion
            }
        }
        static void PortAddedOrRemovedc(object sender, System.Management.EventArrivedEventArgs e)
        {
            // Check if a COM port was added or removed
            if ((uint)e.NewEvent.GetPropertyValue("EventType") == 2) // COM port removed
            {
                string portName = e.NewEvent.GetPropertyValue("InstanceName").ToString();
                MessageBox.Show($"COM port {portName} removed.");
                // Console.WriteLine($"COM port {portName} removed.");
                // Here you can add your logic to handle the removal of the GSM chip
            }
            else if ((uint)e.NewEvent.GetPropertyValue("EventType") == 3) // COM port added
            {
                string portName = e.NewEvent.GetPropertyValue("InstanceName").ToString();
                MessageBox.Show($"COM port {portName}added.");
                //  Console.WriteLine($"COM port {portName} added.");
                // Here you can add your logic to handle the addition of the GSM chip
            }
        }

        public void PortAddedOrRemoved(object sender, System.Management.EventArrivedEventArgs e)
        {
            // Check if a COM port was added or removed
            if ((uint)e.NewEvent.GetPropertyValue("EventType") == 2) // COM port removed
            {
                string portName = e.NewEvent.GetPropertyValue("InstanceName").ToString();
                // Console.WriteLine($"COM port {portName} removed.");
                MessageBox.Show($"COM port {portName} removed.");
                if (serialPort2 != null && serialPort2.PortName == portName)
                {
                    serialPort2.Close();
                    serialPort2 = null;
                    // Console.WriteLine("Disconnected from previous port.");
                    MessageBox.Show("Disconnected from previous port.");
                }
            }
            else if ((uint)e.NewEvent.GetPropertyValue("EventType") == 3) // COM port added
            {
                string portName = e.NewEvent.GetPropertyValue("InstanceName").ToString();
                // Console.WriteLine($"COM port {portName} added.");
                MessageBox.Show($"COM port {portName}added.");
                if (serialPort2 == null)
                {
                    serialPort2 = new SerialPort(portName);
                    serialPort2.Open();
                    //Console.WriteLine("Connected to new port.");
                    MessageBox.Show("Connected to new port.");
                    // Here you can resume normal operation or perform any initialization
                }
            }
        }

        #endregion
    }

}
