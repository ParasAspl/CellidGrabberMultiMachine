using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Windows.Controls;
using System.Windows.Forms;
using ClosedXML.Excel;
using MetroFramework.Forms;

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


        private async void btnStart_ClickAsync(object sender, EventArgs e)
        {
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
                if (serialPort2.IsOpen)
                {
                    //progressBar1.Maximum = 100;
                    //progressBar1.Step = 1;
                    //progressBar1.Value = 0;
                    //regionloader.RunWorkerAsync();
                    TypeText selectedNetwork = metroComboBox1.SelectedItem as TypeText;
                    a = selectedNetwork.Name;
                    net = selectedNetwork.Name.ToString();
                    switch (selectedNetwork.Name)
                    {
                        case "2G": scan2GNetwork(); break;
                        case "3G": scan3GNetwork(); break;
                        case "4G": scan4GNetwork(); break;

                        case "2G + 3G":
                            // await Task.WhenAll(scan2GNetwork(), scan3GNetwork());
                            scan2GNetwork();
                           scan3GNetwork();
                            //await scan3GNetwork();
                            //Task task2a = new Task(() => scan2GNetwork());
                            //Task task3a = new Task(() => scan3GNetwork());
                            //task2a.Start();
                            //task3a.Start();
                            //Task.WaitAll(task2a, task3a);
                            break;
                        // case "2G + 3G": scan2GNetwork(); scan3GNetwork(); break;
                        //  case "2G + 4G": scan2GNetwork(); scan4GNetwork(); break;
                        case "2G + 4G":
                            // await Task.WhenAll(scan2GNetwork(),  scan4GNetwork());
                             scan2GNetwork();
                             scan4GNetwork();
                            //Task task2aa = new Task(() => scan2GNetwork());
                            //Task task4a = new Task(() => scan4GNetwork());
                            //task2aa.Start();
                            //task4a.Start();
                            //Task.WaitAll(task2aa, task4a);

                            break;

                        case "ALL":
                            //await Task.WhenAll(scan2GNetwork(), scan3GNetwork(), scan4GNetwork());
                            //   Task.WhenAll(new[] { Task.Run(scan2GNetwork), Task.Run(scan3GNetwork), Task.Run(scan4GNetwork) });
                            Task task1 = new Task(() => scan2GNetwork());
                            Task task2 = new Task(() => scan3GNetwork());
                            Task task3 = new Task(() => scan4GNetwork());
                            //https://stackoverflow.com/questions/14630770/sequential-processing-of-asynchronous-tasks
                            task1.Start();
                            task2.Start();
                            task3.Start();
                            //Task.WaitAll(task1, task2, task3);
                            //scan2GNetwork();
                            //scan3GNetwork();
                            //scan4GNetwork();

                            try
                            {
                                // ScanAll();
                                //await scan2GNetwork();
                                //await scan3GNetwork();
                                //await scan4GNetwork();
                                Console.WriteLine("All done");
                            }
                            catch (Exception ex) // For illustration purposes only. Catch specific exceptions!
                            {
                                Console.WriteLine(ex);
                            }
                            //var task = scan2GNetwork().Then(() => scan3GNetwork()).Then(() => scan4GNetwork());
                            //task.ContinueWith(t =>
                            //{
                            //    if (t.IsFaulted || t.IsCanceled)
                            //    {
                            //        var e = t.Exception.InnerException;
                            //        // exception handling
                            //    }
                            //    else
                            //    {
                            //        Console.WriteLine("All done");
                            //    }
                            //}, TaskContinuationOptions.ExecuteSynchronously);

    //                        scan2GNetwork().ToObservable()
    //.SelectMany(_ => scan3GNetwork().ToObservable())
    //.SelectMany(_ => scan4GNetwork().ToObservable())
    //.Subscribe(_ => { Console.WriteLine("All done"); },
        //e => { Console.WriteLine(e); });
                            // scan2GNetwork();
                            // scan3GNetwork(); 
                            //scan4GNetwork(); 
                            break;
                        // case "ALL": scan2GNetwork(); break;
                        //scan3GNetwork(); scan4GNetwork();
                        default:
                            break;
                    }
                    //Thread.Sleep(3000);
                }
            }

            else
            {
                MessageBox.Show("Please select Type");

            }
        }
        //#region forsyncTask
        //public  Task Then(this Task first, Func<Task> next)
        //{
        //    var tcs = new TaskCompletionSource<object>();
        //    first.ContinueWith(_ =>
        //    {
        //        if (first.IsFaulted) tcs.TrySetException(first.Exception.InnerExceptions);
        //        else if (first.IsCanceled) tcs.TrySetCanceled();
        //        else
        //        {
        //            try
        //            {
        //                next().ContinueWith(t =>
        //                {
        //                    if (t.IsFaulted) tcs.TrySetException(t.Exception.InnerExceptions);
        //                    else if (t.IsCanceled) tcs.TrySetCanceled();
        //                    else tcs.TrySetResult(null);
        //                }, TaskContinuationOptions.ExecuteSynchronously);
        //            }
        //            catch (Exception exc) { tcs.TrySetException(exc); }
        //        }
        //    }, TaskContinuationOptions.ExecuteSynchronously);
        //    return tcs.Task;
        //}


        //#endregion
        private void ScanNework(string sNet, string sLen)
        {
            try
            {
                if (sNet == "ALL")
                {
                }
                else if (sNet == "ALL")
                { }

            }
            catch (Exception ex)
            {

            }

        }
        private  void scan2GNetwork()
        {
            try
            {
                outputFile = @"C:\cell id driver\amar\amar\GodSharpDemo\2goutput.txt";
                net = "2G";
                len = 10;
                   
                serialWrite("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F");

                 serialWrite("AT+CNMP=13"); Thread.Sleep(500);
                serialWrite("AT+CMSSN"); Thread.Sleep(2000);
                serialWrite("AT+CSURV"); Thread.Sleep(2000);
                serialWrite("AT+CSURV"); Thread.Sleep(2000);

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
                serialWrite("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F"); Thread.Sleep(500);
                serialWrite("AT+CNMP=14"); Thread.Sleep(500);
                serialWrite("AT+CMSSN"); Thread.Sleep(500);
                serialWrite("AT+CLUARFCN"); Thread.Sleep(500);
                serialWrite("AT+CLUCELL"); Thread.Sleep(500);
                serialWrite("AT+CSNINFO?"); Thread.Sleep(500);
                serialWrite("AT+CSNINFO?"); Thread.Sleep(500);
                serialWrite("AT+CCINFO"); Thread.Sleep(500);
                serialWrite("AT+CCINFO"); Thread.Sleep(500);
            }
            catch (Exception ex)
            {

            }
        }

        private void  scan4GNetwork()
        {
            try
            {
                net = "4G";
                len = 15;
                outputFile = @"C:\cell id driver\amar\amar\GodSharpDemo\2goutput.txt";
                 serialWrite("AT+CNMP=38");
                Thread.Sleep(500);
                 serialWrite("AT+CMSSN"); Thread.Sleep(500);
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
                    serialWrite("AT+CNBP=," + bandS[i]); Thread.Sleep(500);

                    serialWrite("AT+CSNINFO?"); Thread.Sleep(2000);

                    serialWrite("AT+CMGRMI=4"); Thread.Sleep(500);

                }
            }
            catch (Exception ex)
            {

            }

        }

        private void ScanAll()
        {
            outputFile = @"C:\cell id driver\amar\amar\GodSharpDemo\2goutput.txt";
           //2G
serialWrite("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F"); Thread.Sleep(2000);
            serialWrite("AT+CNMP=13"); Thread.Sleep(2000);
            serialWrite("AT+CMSSN"); Thread.Sleep(2000);
            serialWrite("AT+CSURV"); Thread.Sleep(2000);
            serialWrite("AT+CSURV"); Thread.Sleep(2000);
            //3G
           serialWrite("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F"); Thread.Sleep(2000);
            serialWrite("AT+CNMP=14"); Thread.Sleep(2000);
            serialWrite("AT+CMSSN"); Thread.Sleep(2000);
            serialWrite("AT+CLUARFCN"); Thread.Sleep(2000);
            serialWrite("AT+CLUCELL"); Thread.Sleep(2000);
            serialWrite("AT+CSNINFO?"); Thread.Sleep(2000);
            serialWrite("AT+CSNINFO?"); Thread.Sleep(2000);
            serialWrite("AT+CCINFO"); Thread.Sleep(2000);
            serialWrite("AT+CCINFO"); Thread.Sleep(2000);

            //4G
            serialWrite("AT+CNMP=38"); Thread.Sleep(2000);
            serialWrite("AT+CMSSN"); Thread.Sleep(2000);
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
                serialWrite("AT+CNBP=," + bandS[i]); Thread.Sleep(2000);
                serialWrite("AT+CSNINFO?"); Thread.Sleep(2000);
                serialWrite("AT+CMGRMI=4"); Thread.Sleep(2000);
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
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

            btnDisconnect.Visible = true;

            if (!serialPort2.IsOpen)
            {
                establishConnection();
                lblStatus.Text = "Status : Connected";
                DdlMode.Enabled = true;
                btnSave.Enabled = false;
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
            //if(serialPort2.IsOpen)
            //serialPort2.Write("AT+cnsvs"+Environment.NewLine);           
        }

        void establishConnection()
        {
            try
            {
                serialPort2.Open();
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                MessageBox.Show(serialPort2.IsOpen ? "Successfully connected" : "Not connected");
            }

        }
        public static void waitForOutput(int seconds)
        {
            try
            {
                int counter = 0;
                while (counter < seconds)
                {

                    Thread.Sleep(1000);
                    counter++;
                }
                if (counter == seconds)
                {
                    MessageBox.Show("Counter has reset due to timeout");
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void serialPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataRec = "";
            //End = DateTime.Now;
            //var result = (int)End.Subtract(start).TotalMinutes;
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

            if ((((dataRec == null) || (count2G >= 5 && net != "ALL") || (count3G >= 2) || (count4G >= 5))))
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
                    //MessageBox.Show("Scan completed");
                    //btnDisconnect.Visible = false;
                    //loader.Visible = false;
                    //btnConnect.Visible = true;
                    //try
                    //{
                    //    serialPort2.Close();

                    //    lblStatus.Text = "Status : Disconnected";
                    //    MessageBox.Show("Connection closed!");
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("Error while closing connection" + ex);
                    //    throw;
                    //}

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while closing connection" + ex);
                    throw;
                }

            }

            else
            {

                if (region != "NA" && region != "R" && Modetype != "")
                {
                    //File.AppendAllText(outputFile,dataRec); (!(dataRec.Contains("CCINFO") || dataRec.Contains("ccinfo"))) &&
                    if (((dataRec.Contains("arfcn:") && dataRec.Contains("mcc:")) || (dataRec.Contains("ARFCN:")
                        && dataRec.Contains("MCC:")) || (net == "2G")))
                    {
                        net = "2G";
                    }
                    //else if ((dataRec.Contains("CSNINFO") && (dataRec.Contains("SCELL"))) && net == "3G")
                    //{
                    //    net = "3G";
                    //}
                    //else if ((dataRec.Contains("Serving_Cell") || dataRec.Contains("CSNINFO")) && net == "4G")
                    //{
                    //    net = "4G";
                    //}
                }
                string[] array = dataRec.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                List<Dictionary<string, string>> list = clean(array);

                // if (net == "2G")
                if ((!(dataRec.Contains("CCINFO") || dataRec.Contains("ccinfo"))) && ((dataRec.Contains("arfcn:")
                        && dataRec.Contains("mcc:")) || (dataRec.Contains("ARFCN:") && dataRec.Contains("MCC:"))) && ((net == "2G") || a == "ALL"))
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

                            Thread.Sleep(1000);
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
                                    Thread.Sleep(1000);
                                    // serialWrite("AT+CMSSN=" + twoD[j][0] + twoD[j][1]);
                                    //  await Task.Run(() => serialWrite("AT+CMSSN=" + map["mcc"] + map["mnc"] + "\r\n"));

                                    //await Task.Run(() => 
                                    serialWrite("AT+CSURV");//);
                                    Thread.Sleep(1000);
                                    // await Task.Run(() =>
                                    serialWrite("AT+CSURV");//) ; ;
                                    Thread.Sleep(100);
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
                if ((dataRec.Contains("CSNINFO") && (dataRec.Contains("SCELL")) ||
                    ((dataRec.Contains("arfcn:")
                        && dataRec.Contains("mcc:")) || (dataRec.Contains("ARFCN:") && dataRec.Contains("MCC:"))))
                    && ((net == "3G") || a == "ALL" || a.Contains("2G + 3G")))
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
                                        Thread.Sleep(200);
                                        serialWrite("AT+CLUARFCN");
                                        Thread.Sleep(200);
                                        serialWrite("AT+CLUCELL");
                                        Thread.Sleep(2000);
                                        serialWrite("AT+CSNINFO?");
                                        Thread.Sleep(200);
                                        serialWrite("AT+CCINFO");
                                        Thread.Sleep(200);
                                    }
                                }
                                serialWrite("AT+CMSSN");
                                Thread.Sleep(200);
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
                            try
                            {
                                row["(A/E/U)RFCN"] = map["UARFCN"];
                            }
                            catch (Exception ex)
                            {

                            }
                            row["ENB"] = "NA";
                            row["Network Type"] = net;
                            try
                            {
                                row["BSIC/PSC/PCI"] = map["PSC"];
                            }
                            catch (Exception ex)
                            {

                            }
                            try { 
                            row["Net Strength"] = getNetworkStrength2G(map["RXLev"].Split('d')[0]);
                            }
                            catch (Exception ex)
                            {

                            }
                            try
                            {
                                row["DBM"] = map["RXLev"].Split('d')[0];
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
                                        Thread.Sleep(100);
                                        serialWrite("AT+CLUARFCN");
                                        Thread.Sleep(100);
                                        serialWrite("AT+CLUCELL");
                                        Thread.Sleep(100);
                                        serialWrite("AT+CSNINFO?");
                                        Thread.Sleep(100);
                                        serialWrite("AT+CCINFO");
                                        Thread.Sleep(100);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                            }
                            //await Task.Run(() => 
                            serialWrite("AT+CMSSN");//);
                            Thread.Sleep(100);
                        }
                        try
                        {
                            if ((Modetype == "Spot") && array.Length > 2 && array[0].Contains("CSNINFO") && array[0].Contains("SCELL")
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

                if ((dataRec.Contains("Serving_Cell") || dataRec.Contains("CSNINFO")) && ((net == "4G") || a == "ALL" || a.Contains("2G + 4G")))
                {
                    net = "4G";

                    if (typ == "csn")
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dictionary<string, string> map = list[i];
                            try {   
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

                                    Thread.Sleep(50);
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
                                        Thread.Sleep(50);
                                        serialWrite("AT+CSNINFO?");
                                        Thread.Sleep(50);
                                        serialWrite("AT+CMGRMI=4");
                                        Thread.Sleep(50);
                                    }

                                }

                            }
                            serialWrite("AT+CMSSN");
                            Thread.Sleep(50);
                            }
                            catch (Exception ex)
                            {

                            }
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
                                Thread.Sleep(100);

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
                                    Thread.Sleep(100);
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
                                        Thread.Sleep(100);
                                        //await Task.Run(() =>
                                        serialWrite("AT+CSNINFO?");//); ;
                                        Thread.Sleep(100);
                                        //await Task.Run(() => 
                                        serialWrite("AT+CMGRMI=4");//);
                                        Thread.Sleep(100);
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
        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (serialPort2.IsOpen)
                    serialPort2.Close();
            }
            catch (Exception ee)
            {
               // throw;
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
                        map.Add(vals[0].Trim(), vals[1].Trim());
                }
                if (net == "3G")
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
                if (net == "4G")
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
                    serialWrite(null);
                    break;
                }

                if (line.Contains("Network survey end") && selectedMode == "Spot")
                {
                    count2G++;

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

        private void timer_Tick(object sender, EventArgs e)
        {

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

            getRegion();
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

        private void regionloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // MessageBox.Show("Region Selected");
            serialPort2.Close();

            serialPort2.Open();
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


        private void btnSave_Click(object sender, EventArgs e)
        {
            Export2Excel(dt, net);
        }

        private void regionloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Export2Excel(DataTable dt, string fileName)
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
    }
}


public class TypeText
{
    public string Name { get; set; }

}