﻿using ClosedXML.Excel;
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
    public partial class Commands : MetroForm
    {
        static bool lockk = false;
        //change path
        string outputFile = "";// @"C:\amar\output.txt";
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
        public Commands()
        {
            InitializeComponent();
            btnConnect.Visible = true;
            btnDisconnect.Visible = false;
            lblApplicationVersion.Text = "Version : 1.00";
            lblCompanyName.Text = "@ Avenging Security";
            lblContactDetail.Text = "9461101915";
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
            }
            catch (Exception ex)
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
            dt.Columns.Add("LAC/TAC");
           // dt.Columns.Add("TAC");
            dt.Columns.Add("ECI");
            dt.Columns.Add("CellId");
            dt.Columns.Add("CGI");
            dt.Columns.Add("(A/E/U)RFCN");
            dt.Columns.Add("ENB");
            dt.Columns.Add("Network Type");
            dt.Columns.Add("BSIC/PSC/PCI");
            dt.Columns.Add("DBM");
            dt.Columns.Add("Net Strength");
            //for 4G

            metroGrid1.DataSource = dt;
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
        }
        #region Start/Stop

        private void btnStart_Click(object sender, EventArgs e)
        {
            Countok = 0;
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
                if (ddlmode != null && selectedNetworkcs != null && selectedNetworkcs.Name != null)
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
                            btnSave.Enabled = false;
                            btnStart.Visible = true;
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
        protected void scanAllForFast(int count = 0)
        {
            net = "ALL";
            outputFile = @"C:\amar\2goutput.txt";

            string c4 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
            string c1 = (@"AT+QNWPREFCFG=""mode_pref"",WCDMA").Replace("\r", "").Replace("\n", "");
            // string c2 = ("At+cops=?").Replace("\r", "").Replace("\n", "");
            string c3 = (@"AT+QENG=""servingcell""").Replace("\r", "").Replace("\n", "");
            if ((selectedcmbMode) == "Fast")
            {
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
                if (Countok > 8 && Countok < 11)
                {
                    serialWrite("AT+QSCAN=2,1");//);
                    Thread.Sleep(2000);
                }
                if (Countok > 10 && Countok < 13)
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
                string c11 = ("AT+QSCAN=2,1").Replace("\r", "").Replace("\n", "");
                string c2 = ("AT+QSCAN=3,1").Replace("\r", "").Replace("\n", "");

                if (Countok > 38 && Countok < 40)
                {
                    serialWrite(c33);//);
                    Thread.Sleep(2000);
                }
                if (Countok > 39 && Countok < 42)
                {
                    serialWrite(c11);
                    Thread.Sleep(2000);
                }
                if (Countok > 41 && Countok < 44)
                {
                    serialWrite(c2);
                    Thread.Sleep(2000);
                }
                int[] bandSs = { 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };
                if (!serialPort2.IsOpen)
                {
                    MessageBox.Show("Device is not connected.Scan will stop");
                    return;
                }
                if (Countok > 43 && Countok < 49)
                {
                    if (Countok > 43 && Countok < 45)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[0]); Thread.Sleep(2000);
                    }
                    if (Countok > 44 && Countok < 47)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 46 && Countok < 49)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 48 && Countok < 54)
                {
                    if (Countok > 48 && Countok < 50)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[1]); Thread.Sleep(2000);
                    }
                    if (Countok > 49 && Countok < 52)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 51 && Countok < 54)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 53 && Countok < 59)
                {
                    if (Countok > 53 && Countok < 55)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[2]); Thread.Sleep(2000);
                    }
                    if (Countok > 54 && Countok < 57)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 56 && Countok < 59)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 58 && Countok < 64)
                {
                    if (Countok > 58 && Countok < 60)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[3]); Thread.Sleep(2000);
                    }
                    if (Countok > 59 && Countok < 62)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 61 && Countok < 64)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 63 && Countok < 69)
                {
                    if (Countok > 63 && Countok < 65)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[4]); Thread.Sleep(2000);
                    }
                    if (Countok > 64 && Countok < 67)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 66 && Countok < 69)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 68 && Countok < 74)
                {
                    if (Countok > 68 && Countok < 70)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[5]); Thread.Sleep(2000);
                    }
                    if (Countok > 69 && Countok < 72)
                    {
                        serialWrite(c11); Thread.Sleep(4000);
                    }
                    if (Countok > 71 && Countok < 74)
                    {
                        serialWrite(c2); Thread.Sleep(4000);
                    }
                }
                if (Countok > 73 && Countok < 79)
                {
                    if (Countok > 73 && Countok < 75)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[6]); Thread.Sleep(2000);
                    }
                    if (Countok > 74 && Countok < 77)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 76 && Countok < 79)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 78 && Countok < 84)
                {
                    if (Countok > 78 && Countok < 80)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[7]); Thread.Sleep(2000);
                    }
                    if (Countok > 79 && Countok < 82)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 81 && Countok < 84)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 83 && Countok < 89)
                {
                    if (Countok > 83 && Countok < 85)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[8]); Thread.Sleep(2000);
                    }
                    if (Countok > 84 && Countok < 87)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 86 && Countok < 89)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 89 && Countok < 95)
                {
                    if (Countok > 89 && Countok < 91)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[9]); Thread.Sleep(2000);
                    }
                    if (Countok > 90 && Countok < 93)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 92 && Countok < 95)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }
                }
                if (Countok > 94 && Countok < 100)
                {
                    if (Countok > 94 && Countok < 96)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[10]); Thread.Sleep(2000);
                    }
                    if (Countok > 95 && Countok < 98)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 97 && Countok < 100)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 99 && Countok < 105)
                {
                    if (Countok > 98 && Countok < 100)
                    {
                        serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSs[11]); Thread.Sleep(2000);
                    }
                    if (Countok > 99 && Countok < 102)
                    {
                        serialWrite(c11); Thread.Sleep(2000);
                    }
                    if (Countok > 101 && Countok < 104)
                    {
                        serialWrite(c2); Thread.Sleep(2000);
                    }

                }
                if (Countok > 103 && Countok < 105)
                {
                    serialWrite(c33);
                    Thread.Sleep(2000);
                }


            }
        }

        //for 3G
        protected void scan3GNetwork(int count = 0)
        {
            try
            {
                net = "3G";
                string s = "\"blabla\"";
                len = 11;
                outputFile = @"C:\amar\2goutput.txt";
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
        private void scan4GNetwork(int count = 0)
        {
            try
            {
                net = "4G";
                len = 15;
                outputFile = @"C:\amar\2goutput.txt";
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
                    if (Countok >= 36 && Countok <= 37)
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
        protected void scan5GNetwork(int count = 0)
        {
            try
            {
                //string f = @"Joe said ""Hello"" to me";
                net = "5G";
                string s = "\"blabla\"";
                len = 11;
                outputFile = @"C:\amar\2goutput.txt";
                string c3 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
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
                        serialWrite("AT+QSCAN=2,1");//);
                        Thread.Sleep(10000);
                    }
                    if (Countok > 2 && Countok < 5)
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
                    if ((Countok >= 1 || Countok < 1) && Countok <= 3)
                    {
                        serialWrite(c1);
                        Thread.Sleep(4000);
                    }
                    if (Countok > 3 && Countok <= 5)
                    {
                        serialWrite(c2);
                        Thread.Sleep(4000);
                    }
                    int[] bandS = { 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };
                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if (Countok > 5 && Countok < 11)
                    {
                        if (Countok <= 6)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[0]); Thread.Sleep(2000);
                        }
                        if (Countok > 6 && Countok <= 8)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 8 && Countok < 11)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 10 && Countok < 16)
                    {
                        if (Countok <= 11)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[1]); Thread.Sleep(2000);
                        }
                        if (Countok > 11 && Countok <= 13)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 13 && Countok < 16)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 15 && Countok < 21)
                    {
                        if (Countok <= 16)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[2]); Thread.Sleep(2000);
                        }
                        if (Countok > 16 && Countok <= 18)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 18 && Countok < 21)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 20 && Countok < 26)
                    {
                        if (Countok <= 21)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[3]); Thread.Sleep(2000);
                        }
                        if (Countok > 21 && Countok <= 23)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 23 && Countok < 26)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 25 && Countok < 31)
                    {
                        if (Countok <= 26)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[4]); Thread.Sleep(2000);
                        }
                        if (Countok > 26 && Countok <= 28)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 28 && Countok < 31)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 30 && Countok < 36)
                    {
                        if (Countok <= 31)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[5]); Thread.Sleep(2000);
                        }
                        if (Countok > 31 && Countok <= 34)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 34 && Countok < 37)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 36 && Countok < 42)
                    {
                        if (Countok <= 37)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[6]); Thread.Sleep(2000);
                        }
                        if (Countok > 37 && Countok <= 39)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 39 && Countok < 41)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 40 && Countok < 46)
                    {
                        if (Countok <= 41)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[7]); Thread.Sleep(2000);
                        }
                        if (Countok > 41 && Countok <= 43)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 43 && Countok < 46)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 45 && Countok < 51)
                    {
                        if (Countok <= 46)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[8]); Thread.Sleep(2000);
                        }
                        if (Countok > 46 && Countok <= 48)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 48 && Countok < 51)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 50 && Countok < 56)
                    {
                        if (Countok <= 51)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[9]); Thread.Sleep(2000);
                        }
                        if (Countok > 51 && Countok <= 53)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 53 && Countok < 56)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 55 && Countok < 61)
                    {
                        if (Countok <= 56)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[10]); Thread.Sleep(2000);
                        }
                        if (Countok > 56 && Countok <= 58)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 58 && Countok < 61)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok > 60 && Countok < 66)
                    {
                        if (!serialPort2.IsOpen)
                        {
                            MessageBox.Show("Device is not connected.Scan will stop");
                            return;
                        }
                        if (Countok <= 61)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandS[11]); Thread.Sleep(2000);
                        }
                        if (Countok >= 61 && Countok <= 63)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok > 63 && Countok < 66)
                        {
                            serialWrite(c2); Thread.Sleep(4000);
                        }

                    }
                    if (Countok >= 65 && Countok <= 66)
                    {
                        serialWrite(c3);
                        Thread.Sleep(4000);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void scan4G5GNetwork(int count = 0)
        {
            try
            {
                net = "4G + 5G";
                len = 15;
                string c1 = ("AT+QSCAN=2,1").
                  Replace("\r", "").Replace("\n", "");
                string c3 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");

                outputFile = @"C:\amar\2goutput.txt";
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
                    if (Countok >= 2 && Countok < 5)
                    {

                        serialWrite("AT+QSCAN=1,1");//);
                        Thread.Sleep(5000);
                    }

                    if (Countok > 4 && Countok < 7)
                    {
                        serialWrite("AT+QSCAN=2,1");//);
                        Thread.Sleep(10000);
                    }

                    if (Countok > 6 && Countok < 9)
                    {
                        serialWrite("AT+QSCAN=3,1");
                        Thread.Sleep(10000);
                    }

                }
                else
                {
                    if (Countok < 1)
                    {
                        serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                        Thread.Sleep(2000);
                    }
                    string c4 = (@"AT+QNWPREFCFG = ""nr5g_band"",1:2:3:5:7:8:12:13:14:18:20:25:26:28:29:30:38:40:41:48:66:70:71:75:76:77:78:79").Replace("\r", "").Replace("\n", "");
                    if ((Countok >= 1 || Countok < 1) && Countok < 2)
                    {
                        serialWrite(c4);//);
                        Thread.Sleep(2000);
                    }
                    if (Countok >= 2 && Countok < 4)
                    {
                        serialWrite("AT+QSCAN=1,1");//);
                        Thread.Sleep(4000);
                    }
                    if (Countok >= 4 && Countok < 6)
                    {
                        serialWrite(c1);//);
                        Thread.Sleep(4000); serialWrite(c1);//);
                    }
                    if (Countok >= 6 && Countok < 8)
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
                    if (Countok >= 8 && Countok < 14)
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
                    if (Countok >= 13 && Countok <= 19)
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
                    if (Countok >= 18 && Countok <= 24)
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
                    if (Countok >= 24 && Countok <= 30)
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
                    if (Countok >= 29 && Countok <= 35)
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
                    if (Countok >= 35 && Countok <= 41)
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

                    int[] bandSS = { 3, 5, 8, 28, 40, 41, 58, 71, 77, 78, 79 };

                    if (!serialPort2.IsOpen)
                    {
                        MessageBox.Show("Device is not connected.Scan will stop");
                        return;
                    }
                    if (Countok > 41 && Countok < 47)
                    {
                        if (Countok <= 41)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[0]); Thread.Sleep(2000);
                        }
                        if (Countok > 42 && Countok < 45)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 45 && Countok < 47)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 46 && Countok < 52)
                    {
                        if (Countok <= 46)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[1]); Thread.Sleep(2000);
                        }
                        if (Countok > 47 && Countok < 50)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 50 && Countok < 52)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 51 && Countok < 57)
                    {
                        if (Countok <= 51)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[2]); Thread.Sleep(2000);
                        }
                        if (Countok > 52 && Countok < 55)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 55 && Countok < 57)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 56 && Countok < 62)
                    {
                        if (Countok <= 56)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[3]); Thread.Sleep(2000);
                        }
                        if (Countok > 57 && Countok < 60)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 60 && Countok < 62)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 61 && Countok < 67)
                    {
                        if (Countok <= 61)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[4]); Thread.Sleep(2000);
                        }
                        if (Countok > 62 && Countok < 65)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 65 && Countok < 67)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 66 && Countok < 72)
                    {
                        if (Countok <= 66)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[5]); Thread.Sleep(2000);
                        }
                        if (Countok > 67 && Countok < 70)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 70 && Countok < 72)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 71 && Countok < 77)
                    {
                        if (Countok <= 71)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[6]); Thread.Sleep(2000);
                        }
                        if (Countok > 72 && Countok < 75)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 75 && Countok < 77)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 76 && Countok < 82)
                    {
                        if (Countok <= 76)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[7]); Thread.Sleep(2000);
                        }
                        if (Countok > 77 && Countok < 80)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 80 && Countok < 82)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 81 && Countok < 87)
                    {
                        if (Countok <= 81)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[8]); Thread.Sleep(2000);
                        }
                        if (Countok > 82 && Countok < 85)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 85 && Countok < 87)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 86 && Countok < 92)
                    {
                        if (Countok <= 86)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[9]); Thread.Sleep(2000);
                        }
                        if (Countok > 87 && Countok < 90)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 90 && Countok < 92)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 91 && Countok < 97)
                    {
                        if (Countok <= 91)
                        {
                            serialWrite(@"AT+QNWPREFCFG=""nr5g_band""," + bandSS[10]); Thread.Sleep(2000);
                        }
                        if (Countok > 92 && Countok < 95)
                        {
                            serialWrite(c1); Thread.Sleep(4000);
                        }
                        if (Countok >= 95 && Countok < 97)
                        {
                            serialWrite("AT+QSCAN=3,1"); Thread.Sleep(4000);
                        }
                    }
                    if (Countok > 96 && Countok < 98)
                    {
                        serialWrite(@"AT+QNWPREFCFG = ""lte_band"",1:2:3:4:5:7:8:12:13:14:17:18:19:20:25:26:28:29:30:32:34:38:39:40:41:42:66:71");
                        Thread.Sleep(2000);
                    }
                    if (Countok > 98 && Countok < 100)
                    {
                        serialWrite(c4);//);
                        Thread.Sleep(2000);
                    }

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
                DdlMode.Enabled = false;
                metroComboBox1.Enabled = false;
            }
            else
            {
                MessageBox.Show("Command in progress");
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
                catch (Exception ex)
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
                            row["ENB"] = (map["cellid"]);
                            //string hexcell = map["cellid"].ToString().Substring(map["cellid"].Length - 2, 2);
                            row["ECI"] = (row["ENB"]).ToString().Replace("-", "");
                        }
                        //row["TAC"] = (Convert.ToInt32(map["tac"], 16));
                        row["CellId"] = (Convert.ToInt32(map["cellid"], 16));
                        row["CGI"] = map["mcc"] + "-" + map["mnc"] +  "-" + (Convert.ToInt32(map["cellid"], 16));
                        row["LAC/TAC"] = Convert.ToInt32(map["lac"], 16);
                    }
                    else
                    {
                        row["ENB"] = map["cellid"];
                        row["ECI"] = ((Convert.ToInt32(map["cellid"], 16))).ToString().Replace("-", "");
                       // row["TAC"] = map["tac"];
                        row["CellId"] = map["cellid"];
                        row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-"  + map["cellid"];
                    }
                    row["(A/E/U)RFCN"] = map["arfcn"];

                    if (map["net"] == "3G")
                    {
                        row["ECI"] = map["cellid"];
                        row["ENB"] = "NA";
                        row["CGI"] = map["mcc"] + "-" + map["mnc"] + "-" + map["lac"] + "-" + map["cellid"];
                    }
                    row["Network Type"] = map["net"];
                    row["BSIC/PSC/PCI"] = map["bsic"];
                    row["DBM"] = map["dBm"];
                    row["Net Strength"] = getNetworkStrength(map["dBm"]);
                    dt.Rows.Add(row);
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                       // metroGrid1.DataSource = null;
                        metroGrid1.DataSource = dt;
                        //metroGrid1.Update();
                        //metroGrid1.Refresh();
                        //Gridview1.Columns["Amount"].Visible = false;

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
                        scan5GNetwork(Countok);

                        if (Countok >= 66)
                        {
                            Thread.Sleep(2000);
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Enabled = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("4G") && !a.Contains("4G + 5G"))
                    {
                        scan4GNetwork(Countok);

                        if (Countok >= 37)
                        {
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            // serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Enabled = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("4G + 5G"))
                    {
                        scan4G5GNetwork(Countok);

                        if (Countok > 99)
                        {
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Enabled = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("3G"))
                    {
                        scan3GNetwork(Countok);
                        if (Countok >= 6)
                        {
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Enabled = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else
                    {
                        scanAllForFast(Countok);
                        if (Countok >= 105)
                        {
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Enabled = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }

                    }
                }

                //list.Count > 2 && 
                else if (dataRec.ToLower().Contains("ok") && selectedMode.ToLower().Contains("spot")
           && selectedcmbMode.ToLower().Contains("fast") && (dataRec.ToLower().Contains("ok") || dataRec.ToLower().Contains("error")) && selectedMode.ToLower().Contains("spot"))
                {
                    if (a.Contains("5G") && !a.Contains("4G + 5G"))
                    {
                        scan5GNetwork(Countok);

                        if (Countok >= 5)
                        {
                            Thread.Sleep(4000);
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Enabled = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });

                        }
                    }
                    else if (a.Contains("4G") && !a.Contains("4G + 5G"))
                    {
                        scan4GNetwork(Countok);

                        if (Countok >= 5)
                        {
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Enabled = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("4G + 5G"))
                    {
                        scan4G5GNetwork(Countok);

                        if (Countok >= 9)
                        {
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            // serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Enabled = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else if (a.Contains("3G"))
                    {
                        scan3GNetwork(Countok);
                        if (Countok > 3)
                        {
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            // serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Enabled = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }
                    }
                    else
                    {
                        scanAllForFast(Countok);
                        if (Countok >= 12)
                        {
                            loader.Invoke((MethodInvoker)delegate
                            {
                                loader.Visible = false;
                            });
                            MessageBox.Show("Scan Completed");
                            //serialPort2.Close();
                            btnStop.Invoke((MethodInvoker)delegate { btnStop.Visible = false; });
                            btnSave.Invoke((MethodInvoker)delegate { btnSave.Enabled = true; });
                            btnStart.Invoke((MethodInvoker)delegate { btnStart.Visible = true; });
                            DdlMode.Invoke((MethodInvoker)delegate { DdlMode.Enabled = true; });
                            cmbMode.Invoke((MethodInvoker)delegate { cmbMode.Enabled = true; });
                            metroComboBox1.Invoke((MethodInvoker)delegate { metroComboBox1.Enabled = true; });
                        }

                    }
                }
                try
                {
                    File.AppendAllText(outputFile, dataRec);
                }
                catch (Exception ex)
                {

                }
            }

            catch (Exception ex)
            {
                loader.Invoke((MethodInvoker)delegate
                {
                    loader.Visible = false;
                });
            }
        }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
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

                else if(a=="ALL")
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
                TypeList.Add(new TypeText { Name = "Select" });
                TypeList.Add(new TypeText { Name = "3G" });
                TypeList.Add(new TypeText { Name = "4G" });
                TypeList.Add(new TypeText { Name = "5G" });
                TypeList.Add(new TypeText { Name = "4G + 5G" });
                TypeList.Add(new TypeText { Name = "ALL" });
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
                TypeList.Add(new TypeText { Name = "ALL" });
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
                // TypeList.Add(new TypeText { Name = "Route" });
                DdlMode.Enabled = true;
                DdlMode.DataSource = TypeList;
                DdlMode.DisplayMember = "Name";
            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblDate.Text = System.DateTime.Now.ToString();
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
            if (lockk == false)
            {
                dt.Clear();
                scannedCellId.Clear();
                //metroGrid1.Rows.Clear();
                metroGrid1.DataSource = null;
                metroComboBox1.Enabled = false;
            }
            else
            {
                MessageBox.Show("Command is in progress, Please try again later.");

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

//public class TypeText1
//{
//    public string Name { get; set; }

//}