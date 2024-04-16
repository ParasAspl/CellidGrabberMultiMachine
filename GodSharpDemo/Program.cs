using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CligenceCellIDGrabber
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static bool MachineType = false;
        static string MachineName = "";

        [STAThread]
        static async Task Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //if (EncryptionHelper.CheckForInternetConnection())
            //{
            var dir1 = @"C:\Sys\log";  // folder location
            var dir = @"C:\Sys\log";  // folder location
            string Machine = Srport();
            if (Directory.Exists(dir) && FileExists("Sys.dat"))
            {
                string readText = File.ReadAllText(Path.Combine(dir, "Sys.dat"));
                string decryptt = EncryptionHelper.Decrypt(readText);
                string[] key = decryptt.Split(';');
                try
                {
                    MNC_MCC.Message = "This software is licensed to " + key[3] + " " + key[2] + " Product key : " + key[0];
                }
                catch(Exception ex)
                {
                    MNC_MCC.Message = "This software is licensed to Product key : " + key[0];
                }
                Dictionary<string, string> postData = new Dictionary<string, string>();
                postData.Add("key", key[0]);//"62220182b8deb" 64e72a5c4a8fa
                if (EncryptionHelper.CheckForInternetConnection())
                {
                    var result = await PostHTTPRequestAsync("https://msg.ccas.in/api/cellId/productKey", postData);
                    if (result.Contains("deactivated"))
                    {
                        //MessageBox.Show(result.ToString());
                        Application.Run(new ActivationForm());
                        //return;
                    }
                }

                if (key.Length > 0 && GetID() == key[1])
                {
                    if (!Machine.Contains("Quectel") && MachineType)
                        Application.Run(new Dashboard());
                    else if ((MachineType) && Machine.Contains("Quectel"))
                    {
                        Application.Run(new Commands());
                        //Dashboard dsb = new Dashboard();
                        //dsb.Show();
                        //Application.Run(new Dashboard5G());
                    }
                    else
                    {
                      //  Application.Run(new Commands());
                        //Application.Run(new Commands());
                       MessageBox.Show("Please connect Machine");
                        //  else
                        //Dashboard dsb = new Dashboard();
                        //dsb.Show();
                    }
                }
                else
                {
                    Application.Run(new ActivationForm());

                }

            }
            else
            {
                Application.Run(new ActivationForm());
            }
            //}
            //else 
            //{
            //    MessageBox.Show("Please check Internet Connection");
            //}

        }


        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> PostHTTPRequestAsync(string url, Dictionary<string, string> data)
        {
            try
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
            catch (Exception ex)
            {
                return "Try again later.";
            }
        }
        public static bool FileExists(string fileName)
        {
            var dir = @"C:\Sys\log";
            var workingDirectory = dir;
            var file = $"{workingDirectory}\\{fileName}";
            return File.Exists(file);
        }

        public static string Srport()
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startinfo = new System.Diagnostics.ProcessStartInfo();
                startinfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                try
                {
                    startinfo.FileName = "cmd.exe";
                }
                catch (Exception ex)
                {}
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
                    MachineType = true;
                    MachineName = "Quectel USB Modem";
                }
                else
                {
                    MachineType = false;
                    MachineName = "";
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            return MachineName + MachineType;
        }
        public static string GetID()
        {
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
            mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {
                id = mo["ProcessorID"].ToString();
            }

            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mos.Get();
            string motherBoard = "";
            foreach (ManagementObject mo in moc)
            {
                //  {\\DESKTOP - 181DVKB\root\cimv2: Win32_BaseBoard.Tag = "Base Board"}
                motherBoard = motherBoard + (string)mo["SerialNumber"];
            }

            string uniqueSystemId = id + motherBoard;
            return uniqueSystemId;
        }
    }
}

//stop show, machine not reconnect ,