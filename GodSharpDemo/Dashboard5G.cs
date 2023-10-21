using CligenceCellIDGrabber;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodSharpDemo
{
    public partial class Dashboard5G : MetroForm
    {
        static bool lockk = false;
        //change path
       // string outputFile = @"C:\amar\output.txt";
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
        bool IslteBand = false, IsNr5gBand = false;
        int Countok = 0;
        public Dashboard5G()
        {
            InitializeComponent();
            btnConnect.Visible = true;
            btnDisconnect.Visible = false;
            btnStop.Visible = false;
            btnStart.Visible = true;
            cmbMode.SelectedItem = "Fast";
            cmbMode.SelectedText = "Fast";
            cmbMode.SelectedIndex = 0;

            //DdlMode.SelectedItem = "Spot";
            //DdlMode.SelectedText = "Spot";
            //cmbMode.SelectedIndex = 1;
            port = srport();
            try
            {
                serialPort2.Close();
            }
            catch(Exception ex)
            {

            }

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
            dt.Columns.Add("TAC");
            dt.Columns.Add("CellId");
            dt.Columns.Add("CGI");
            //dt.Columns.Add("(A/E/U)RFCN");
            //dt.Columns.Add("ENB");
            dt.Columns.Add("Network Type");
            //dt.Columns.Add("BSIC/PSC/PCI");
            //dt.Columns.Add("DBM");
            //dt.Columns.Add("Net Strength");
            metroGrid1.DataSource = dt;
        }

        private void Dashboard5G_Load(object sender, EventArgs e)
        {
        }
        #region Start/Stop

        private void btnStart_Click(object sender, EventArgs e)
        {
            TypeText selectedNetworks = DdlMode.SelectedItem as TypeText;
            string ddlmode = selectedNetworks.Name;
            TypeText selectedNetworkcs = metroComboBox1.SelectedItem as TypeText;
            if (cmbMode.SelectedItem.ToString().ToLower() == "deep" && ddlmode.ToString().ToLower() == "route")
            {

                MessageBox.Show("Please select Deep with Spot only");
            }
            else
            {
                start = DateTime.Now;
                if (ddlmode != null && selectedNetworkcs.Name != null)
                {
                    Modetype = DdlMode.SelectedItem.ToString();
                    //scannedCellId.Clear();
                    btnStop.Visible = true;
                    btnSave.Enabled = false;
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

                                    case "2G": scan2GNetwork(); break;
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
        }
        private void scan2GNetwork()
        {
            try
            {
                //outputFile = @"C:\amar\2goutput.txt";
                net = "2G";
                len = 10;
                //   await Task.Run(() => )
                //await Task.Run(() =>
                serialWrite("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F");//);
                Thread.Sleep(100);
                // await Task.Run(() =>
                serialWrite("AT+CNMP=13");//);
                Thread.Sleep(1000);
                //await Task.Run(() => 
                serialWrite("AT+CMSSN");//);
                Thread.Sleep(100);
                //await Task.Run(() => 
                serialWrite("AT+CSURV");//);
                Thread.Sleep(100);
                //await Task.Run(() => 
                serialWrite("AT+CSURV");//);
                Thread.Sleep(100);

            }
            catch (Exception ex)
            {

            }

        }
        //for 5G
        private void scan5GNetwork()
        {
            try
            {
                //string f = @"Joe said ""Hello"" to me";
                net = "5G";
                string s = "\"blabla\"";
                len = 11;
                //outputFile = @"C:\amar\2goutput.txt";
                string c3 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                string c1 = ("AT+QSCAN=2,1").
                    Replace("\r", "").Replace("\n", "");
                string c2 = ("AT+QSCAN=3,1").Replace("\r", "").Replace("\n", "");
                if ((selectedcmbMode) == "Fast")
                {
                    // await Task.Run(() => 
                    //await Task.Run(() =>
                   // serialWrite(@"AT + QNWPREFCFG = ""mode_pref"",NR5G");
                    serialWrite(c3);//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=2,1");//);
                    Thread.Sleep(200); serialWrite(c1);//);
                    Thread.Sleep(200); serialWrite(c1);//);
                    Thread.Sleep(200); serialWrite(c1);//);
                    Thread.Sleep(200); serialWrite(c1);//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=3,1");//);
                    Thread.Sleep(200); serialWrite(c2);//);
                    Thread.Sleep(200); serialWrite(c2);//);
                    Thread.Sleep(200); serialWrite(c2);//);
                    Thread.Sleep(200); serialWrite(c2);//);
                    Thread.Sleep(200); serialWrite(c2);//);
                    Thread.Sleep(200);

                }
                else
                {
                    serialWrite(c3);//);
                    Thread.Sleep(200);

                    serialWrite(c1);
                    Thread.Sleep(200); serialWrite(c1);
                    Thread.Sleep(200); serialWrite(c2);
                    Thread.Sleep(200); serialWrite(c2);

                    Thread.Sleep(200);
                    int[] bandS = { 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };

                    for (int i = 0; i < bandS.Length; i++)
                    {
                        if (!serialPort2.IsOpen)
                        {
                            MessageBox.Show("Device is not connected.Scan will stop");
                            return;
                        }
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[i]);
                        Thread.Sleep(200);
                    }

                    //serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",1");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",3");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",5");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",8");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"", 28");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",40");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",41");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",58");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",71");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",77");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",78");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""nr5g_band"",79");

                    Thread.Sleep(200); serialWrite(c1);
                    Thread.Sleep(200); serialWrite(c1);
                    Thread.Sleep(200); serialWrite(c2);
                    Thread.Sleep(200); serialWrite(c2);
                    Thread.Sleep(200); serialWrite(c3);
                    Thread.Sleep(200);
                }
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
                //outputFile = @"C:\amar\2goutput.txt";
                //await Task.Run(() => 
                if ((selectedcmbMode) == "Fast")
                {
                    //: AT + QNWPREFCFG = "mode_pref",WCDMA
//AT + QNWPREFCFG = "mode_pref",LTE
//5g: AT + QNWPREFCFG = "mode_pref",NR5G


                   // serialWrite(@"AT + QNWPREFCFG =""mode_pref"",LTE"); Thread.Sleep(200);
                    serialWrite(@"AT+QNWPREFCFG= ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200);
                    //await Task.Run(() => 
                }
                else
                {
                    serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200);

                    int[] bandS = { 1, 3, 5, 8, 40, 41 };

                    for (int i = 0; i < bandS.Length; i++)
                    {
                        if (!serialPort2.IsOpen)
                        {
                            MessageBox.Show("Device is not connected.Scan will stop");
                            return;
                        }
                        serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[i]);
                        Thread.Sleep(200);
                    }
                    //serialWrite(@"AT+QNWPREFCFG =""lte_band"" ,1");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""lte_band"",3");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""lte_band"",5");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""lte_band"",8");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""lte_band"",40");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""lte_band"",41");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200);
                    serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void scan4G5GNetwork()
        {   try
            {
                net = "4G + 5G";
                len = 15;
                string c1 = ("AT+QSCAN=2,1").
                  Replace("\r", "").Replace("\n", "");
                string c3 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");

                //outputFile = @"C:\amar\2goutput.txt";
                //await Task.Run(() => 
                if ((selectedcmbMode) == "Fast")
                {
                    //Handshake j = new Handshake();
                   
                    serialWrite(@"AT+QNWPREFCFG= ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");//);
                    
                    Thread.Sleep(200);
                    serialWrite(c3);//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=2,1");//);
                    Thread.Sleep(200); serialWrite(c1);//);
                    Thread.Sleep(200); serialWrite(c1);//);
                    Thread.Sleep(200); serialWrite(c1);//);
                    Thread.Sleep(200); serialWrite(c1);//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200);
                    //await Task.Run(() => 
                }
                else
                {
                    serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                    Thread.Sleep(200);
                    string c4 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                    serialWrite(c4);//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200);
                    serialWrite(c1);//);
                    Thread.Sleep(200); serialWrite(c1);//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200);

                    int[] bandS = { 1, 3, 5, 8, 40, 41 };

                    for (int i = 0; i < bandS.Length; i++)
                    {
                        if (!serialPort2.IsOpen)
                        {
                            MessageBox.Show("Device is not connected.Scan will stop");
                            return;
                        }
                        serialWrite(@"AT+QNWPREFCFG=""lte_band""," + bandS[i]);
                        Thread.Sleep(200);
                    }
                    //serialWrite(@"AT+QNWPREFCFG =""lte_band"" ,1");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""lte_band"",3");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""lte_band"",5");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""lte_band"",8");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""lte_band"",40");
                    //Thread.Sleep(2000); serialWrite(@"AT+QNWPREFCFG =""lte_band"",41");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200); serialWrite("AT+QSCAN=1,1");//);
                    Thread.Sleep(200);
                    serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200);
                    int[] bandSS = { 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };

                    for (int i = 0; i < bandSS.Length; i++)
                    {
                        if (!serialPort2.IsOpen)
                        {
                            MessageBox.Show("Device is not connected.Scan will stop");
                            return;
                        }
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[i]);
                        Thread.Sleep(200);
                    }

                    serialWrite(c1);//);
                    Thread.Sleep(200); serialWrite(c1);//);
                    Thread.Sleep(200); 
                    serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200); serialWrite("AT+QSCAN=3,1");
                    Thread.Sleep(200);
                    serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                    Thread.Sleep(200);
                    serialWrite(c4);//);
                    Thread.Sleep(200);

                }
            }
            catch (Exception ex)
            {

            }

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            lockk = false;
            if (lockk == false)
            {
                btnDisconnect.Visible = false;
                loader.Visible = false;
                btnConnect.Visible = true;
                try
                {
                    serialPort2.Close();
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
                btnSave.Enabled = true;
                btnStart.Visible = true;
                DdlMode.Enabled = true;
                metroComboBox1.Enabled = true;
            }
            else
            {
                MessageBox.Show("Command in progress");

            }
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
            try
            {
                serialPort2.Close();
                scannedCellId.Clear();
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
                        btnSave.Enabled = false;
                        btnDisconnect.Visible = true;
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
            }
            catch (Exception ex)
            {

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

                try
                {
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
                    serialPort2.Open();
                }
                catch(Exception ex)
                {

                }

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
                // if(!string.IsNullOrEmpty( dataRec)) AT+QNWPREFCFG="lte_band",3
                dataRec = serialPort2.ReadExisting();
            }
            catch (Exception ex)
            {

            }
            //string[] array1 = dataRec.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            //List<Dictionary<string, string>> list1 = clean(array1);
            loader.Invoke((MethodInvoker)delegate
            {
                loader.Visible = true;
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
            cmbMode.Invoke((MethodInvoker)delegate
            {
                //if(DdlMode.SelectedItem.ToString()=="Route")
                cmbMode.Enabled = true;
            });
            //}

            //(net != "R") && result >= 5 || 

            string[] array = dataRec.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<Dictionary<string, string>> list = clean(array);
            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, string> map = list[i];
                try
                {
                    if (map.Count>0 && scannedCellId.Contains(map["cellid"]))
                    {
                        continue;
                    }
                    scannedCellId.Add(map["cellid"]);
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
                    // row["ECI"] = map["cellId"];
                    if (row["Operator Name"].ToString().ToLower() != "jio")
                    {
                        row["TAC"] = (Convert.ToInt32(map["tac"], 16));
                        row["CellId"] = (Convert.ToInt32(map["cellid"], 16));
                        row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + (map["lac"]) + (Convert.ToInt32(map["cellid"], 16));
                    }
                    else
                    {
                        row["TAC"] = map["tac"];
                        row["CellId"] = map["cellid"];
                        row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + map["lac"] + map["cellid"];
                    }
                    //row["(A/E/U)RFCN"] = map["arfcn"];
                    //row["ENB"] = map["dBm"];
                    row["Network Type"] = map["net"];
                    //row["BSIC/PSC/PCI"] = map["bsic"];
                    //row["DBM"] = map["dBm"];
                    //row["Net Strength"] = getNetworkStrength2G(map["dBm"]);
                    dt.Rows.Add(row);
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        metroGrid1.DataSource = dt;
                        metroGrid1.Update();
                        metroGrid1.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }

            if (a == "4G + 5G" && selectedcmbMode.ToLower().Contains("deep") && dataRec.ToLower().Contains("ok") && selectedMode.ToLower().Contains("spot")
              && (dataRec.Contains(@"AT+QNWPREFCFG=""lte_band"",41") || dataRec.Contains(@"AT+QNWPREFCFG=""nr5g_band"",79")))
            {
                if (dataRec.Contains(@"AT+QNWPREFCFG=""lte_band"",41")) IslteBand = true;
                if (dataRec.Contains(@"AT+QNWPREFCFG=""nr5g_band"",79")) IsNr5gBand = true;
                if (IsNr5gBand && IslteBand)
                {
                    // loader.Visible = false;
                    MessageBox.Show("Scan Completed");
                    serialPort2.Close();
                    IsNr5gBand = false;
                    IslteBand = false;
                    loader.Invoke((MethodInvoker)delegate
                    {
                        loader.Visible = false;
                    });
                }
            }

            else if (selectedcmbMode.ToLower().Contains("deep") && dataRec.ToLower().Contains("ok") && selectedMode.ToLower().Contains("spot")
                 && (dataRec.Contains(@"AT+QNWPREFCFG=""lte_band"",41") || dataRec.Contains(@"AT+QNWPREFCFG=""nr5g_band"",79")))
            {
                loader.Visible = false;
                MessageBox.Show("Scan Completed");
                serialPort2.Close();

            }
            else if (list.Count > 2 && dataRec.ToLower().Contains("ok") && selectedMode.ToLower().Contains("spot")
               && selectedcmbMode.ToLower().Contains("fast") && dataRec.ToLower().Contains("ok") && selectedMode.ToLower().Contains("spot"))
            {
                //Countok++;
                if (Countok >= 4)
                {
                    loader.Invoke((MethodInvoker)delegate
                    {
                        loader.Visible = false;
                    });
                    MessageBox.Show("Scan Completed");
                    serialPort2.Close();
                }
            }

            //try
            //{
            //    File.AppendAllText(outputFile, dataRec);
            //}
            //catch (Exception ex)
            //{

            //}
        }

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
                        //lockk = true;
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
            if (data.Contains("QSCAN:") || data.Contains("LTE") || data.Contains("NR5G"))
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
                if (string.IsNullOrEmpty(val) || val.Contains("QSCAN") || val.Contains("LTE"))
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
                                if (datas[3] == vals[0])
                                    map.Add("lac", vals[0].Trim());
                            }
                            if (!map.ContainsKey("cellid"))
                            {
                                if (datas[9] == vals[0])
                                    map.Add("cellid", vals[0].Trim());
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

                if (a == "5G" && net == "5G" && data.Contains("NR5G"))
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
                                if (datas[3] == vals[0])
                                    map.Add("lac", vals[0].Trim());
                            }
                            if (!map.ContainsKey("cellid"))
                            {
                                if (datas[9] == vals[0])
                                    map.Add("cellid", vals[0].Trim());
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

                if (a == "4G + 5G")
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
                                if (datas[3] == vals[0])
                                    map.Add("lac", vals[0].Trim());
                            }
                            if (!map.ContainsKey("cellid"))
                            {
                                if (datas[9] == vals[0])
                                    map.Add("cellid", vals[0].Trim());
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
                progressBar1.Value = e.ProgressPercentage;
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
            }
            catch(Exception ex)
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
                TypeList.Add(new TypeText { Name = "Select" });
                TypeList.Add(new TypeText { Name = "3G" });
                TypeList.Add(new TypeText { Name = "4G" });
                TypeList.Add(new TypeText { Name = "5G" });
                TypeList.Add(new TypeText { Name = "4G + 5G" });
                metroComboBox1.Enabled = true;
                metroComboBox1.DataSource = TypeList;
                metroComboBox1.DisplayMember = "Name";
            }
            else if ((selectedNetwork.Name.ToString() == "Spot")) //|| (DdlMode.SelectedItem.ToString() == "Spot"))
            {
                TypeList.Add(new TypeText { Name = "Select" });
                TypeList.Add(new TypeText { Name = "3G" });
                TypeList.Add(new TypeText { Name = "4G" });
                TypeList.Add(new TypeText { Name = "5G" });
                TypeList.Add(new TypeText { Name = "4G + 5G" });
                metroComboBox1.Enabled = true;
                metroComboBox1.DataSource = TypeList;
                metroComboBox1.DisplayMember = "Name";
                metroComboBox1.ValueMember = "Name";
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (Countok >= 3 && lockk == false)
            {
                dt.Clear();
                scannedCellId.Clear();
                //metroGrid1.Rows.Clear();
                metroGrid1.DataSource = null;
                metroComboBox1.Enabled = false;
            }
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

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

}
//public class ProductKeyValidation
//{
//    public string status { get; set; }
//    public string error { get; set; }
//}

public class TypeText1
{
    public string Name { get; set; }

}