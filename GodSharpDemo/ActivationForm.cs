using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CligenceCellIDGrabber;
using MetroFramework.Forms;
using Newtonsoft.Json;

namespace CligenceCellIDGrabber
{
    public partial class ActivationForm : MetroForm
    {
        public ActivationForm()
        {
            InitializeComponent();
        }

        private void ActivationForm_Load(object sender, EventArgs e)
        {
            //string FILE_NAME = "Sys.dat";
            //string rt = "";
            //if (File.Exists(FILE_NAME))
            //{
            //    using (FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read))
            //    {
            //        using (BinaryReader r = new BinaryReader(fs))
            //        {
            //            rt = rt + (r.ReadString());
            //        }
            //    }
            //    string decrypt = EncryptionHelper.Decrypt(rt);
            //    string[] key = decrypt.Split(';');
            //    if (key[0].Length > 0)
            //    {
            //        this.Hide();
            //        Dashboard dsb = new Dashboard();
            //        dsb.Show();
            //    }
            //}
            //string sURL = "https://msg.ccas.in/api/user/login";
            //WebRequest wrGETURL;
            //wrGETURL = WebRequest.Create(sURL);

            //wrGETURL.Method = "POST";
            //wrGETURL.ContentType = @"application/json; charset=utf-8";
            //using (var stream = new StreamWriter(wrGETURL.GetRequestStream()))
            //{
            //    var bodyContent = new
            //    {
            //        userAgent = "",
            //        deviceId = "",
            //        mobile = "8888888889",
            //        password = "manoj@1234"
            //    }; // This will need to be changed to an actual class after finding what the specification sheet requires.

            //    var json = JsonConvert.SerializeObject(bodyContent);

            //    stream.Write(json);
            //}
            //HttpWebResponse webresponse = wrGETURL.GetResponse() as HttpWebResponse;

            //Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            //// read response stream from response object
            //StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            //// read string from stream data
            //string strResult = loResponseStream.ReadToEnd();
            //// close the stream object
            //loResponseStream.Close();
            //string json_data = JsonConvert.SerializeObject(strResult);
            //// tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonTokenResponse);
            //var response = JsonConvert.DeserializeObject<ProductKeyValidation>(strResult);
            //// close the response object
            //webresponse.Close();

        }
      
        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtActivationKey.Text))
            {
                MessageBox.Show("Please enter valid key.");
                return;
            }
            else
            {
                if (EncryptionHelper.CheckForInternetConnection())
                {
                    var status = MyMethodAsync(txtActivationKey.Text);
                }
                else if(!EncryptionHelper.CheckForInternetConnection())
                {
                    MessageBox.Show("Please check Internet Connection");
                }
                else
                {
                    GetfileData(SystemId(), txtActivationKey.Text);
                }
            }
            //64f07a7d4e3be  64f07a7d4e127
            //}

        }

        #region CallAPI/Encrypt
        public  async  Task  MyMethodAsync(string Activationkey,string exist="")
        {
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("key", Activationkey);//"62220182b8deb");
            try
            {
                var result = await PostHTTPRequestAsync("https://msg.ccas.in/api/cellId/productKey", postData);
                Console.WriteLine(result);
                string[] oneD = result.Split(new string[] { "/", "/" }, StringSplitOptions.RemoveEmptyEntries);
                string json_data = JsonConvert.SerializeObject(result);
                // tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonTokenResponse);
                var response = JsonConvert.DeserializeObject<ProductKeyValidation>(result);
                //  var deptList = JsonSerializer.Deserialize<IList<ProductKeyValidation>>(response);
                string status = response.error;
               // { "status":true,"error":"Licence key has been verified.","mobile":"9090909091","product_key":"662d15a67eed7","is_used":"0","firstName":"Test","lastName":"","displayName":"Test ","urlName":"test-"}
                if (status != null && status.ToString().Contains("Licence key has been verified."))
                {
                    GetfileData(SystemId(), txtActivationKey.Text, response.mobile,response.firstName,response.displayName,response.lastName);
                }
                else if (status != null && status.ToString().Contains("Licence key has been deactivated, please contact to admin."))
                { 
                     //GetfileData(SystemId(), txtActivationKey.Text);
                    MessageBox.Show("Licence key has been deactivated, please contact to admin.");
                    return;
                }
                else if (status != null && status.ToString().Contains("Licence key is already in use."))
                {
                    MessageBox.Show("Licence key is already in use.");
                    return;
                }
                else
                {
                    //GetfileData(SystemId(), txtActivationKey.Text);
                    MessageBox.Show(status.ToString());
                  
                }
                //return message;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // return ex.Message;
            }
        }
     
        
        private static readonly HttpClient client = new HttpClient();
        private async Task<string> PostHTTPRequestAsync(string url, Dictionary<string, string> data)
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
        Func<string> SystemId = () =>
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
        };
        #endregion

        #region MyRegion
        public void GetfileData(string encryptkey, string activationkey, string mobile = "", string firstName = "", string displayName = "", string lastName = "")
        {
            String passphraseEncrypt = EncryptionHelper.Encrypt(activationkey + ";" + encryptkey 
                + ";"+"Mobile No: " + mobile+ ";"+ " "+ displayName);
            var dir = @"C:\Sys\log";  // folder location
            var dir1 = @"C:\Sys";  // folder location
            var directoryInfo = new DirectoryInfo("C:\\Sys\\");
            
            if (!Directory.Exists(dir1))
            {
                // if it doesn't exist, create
                try
                {
                    Directory.CreateDirectory(dir1);
                    if (directoryInfo.Exists)
                    {
                        directoryInfo.CreateSubdirectory("log");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
            if (!FileExists("Sys.dat"))
            {
                // use Path.Combine to combine 2 strings to a path
                File.WriteAllText(Path.Combine(dir, "Sys.dat"), passphraseEncrypt);
            }
            string readText = File.ReadAllText(Path.Combine(dir, "Sys.dat"));
            string decryptt = EncryptionHelper.Decrypt(readText);
            string[] key = decryptt.Split(';');

            if (key.Length > 0)
            {
                if (key[0].ToString() == txtActivationKey.Text && key[1].ToString() == encryptkey)
                {
                    this.Hide();
                    MNC_MCC.Message = "This software is licensed to " + key[3] + " " + key[3] + " Product key is " + key[0];
                    string Machine = Program.Srport();
                    if (!Machine.Contains("Quectel") && !Machine.ToLower().Contains("false") )
                        Application.Run(new Dashboard());
                    else if ( Machine.Contains("Quectel"))
                    {
                        Application.Run(new Commands());
                        //Application.Run(new Dashboard5G());
                    }
                    else
                        MessageBox.Show("Please connect Machine");
                   
                }
                else
                {
                    MessageBox.Show("Please try with valid key");
                }
            }
          
        }
        #endregion
        public bool FileExists(string fileName)
        {
            var dir = @"C:\Sys\log";
            var workingDirectory = dir;
            var file = $"{workingDirectory}\\{fileName}";
            return File.Exists(file);
        }


        public void ConsumeTruecallerAPI(string userAuthenticationURI,string Activationkey)
        {
           
            //string sURL = userAuthenticationURI;
            //WebRequest wrGETURL;
            //wrGETURL = WebRequest.Create(sURL);

            //wrGETURL.Method = "POST";
            //wrGETURL.ContentType = @"application/json; charset=utf-8";
            //using (var stream = new StreamWriter(wrGETURL.GetRequestStream()))
            //{
            //    var bodyContent = new
            //    {
            //        key = Activationkey,
                    
            //    }; // This will need to be changed to an actual class after finding what the specification sheet requires.

            //    var json = JsonConvert.SerializeObject(bodyContent);

            //    stream.Write(json);
            //}
            //HttpWebResponse webresponse = wrGETURL.GetResponse() as HttpWebResponse;

            //Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            //// read response stream from response object
            //StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            //// read string from stream data
            //string strResult = loResponseStream.ReadToEnd();
            //// close the stream object
            //loResponseStream.Close();
            //// close the response object
            //webresponse.Close();





            //try
            //{
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(userAuthenticationURI);
                req.Method = "GET";
                req.ContentType = @"application/json; charset=utf-8";
                //req.Headers.Add("Authorization", "Bearer a1i0V--gQMYaa-rktbe-Ohwp7NVtk6QO7EojFVRXvmM1CwHLDYvUxB9GK6rr-f6_");
                // req.Headers.Add("Authorization", "Bearer " + token);
                req.Headers.Add("key", Activationkey);

                // Skip validation of SSL/TLS certificate
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                WebResponse respon = req.GetResponse();
                Stream res = respon.GetResponseStream();
                Encoding enc1 = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader loResponseStream1 = new StreamReader(respon.GetResponseStream(), enc1);
                // read string from stream data
                string strResult1 = loResponseStream1.ReadToEnd();

                //    dynamic resultObject = JsonConvert.DeserializeObject(strResult);
                //    //List<Address> addresses = new List<Address>();
                //    //Address address = new Address();
                //    ////internetAddresses address =new internetAddresses();  
                //    //// List<Phone> phone = new List<Phone>();
                //    ////List<Root> roots = new List<Root>();
                //    //Root roots = new Root();
                //    try
                //    {
                //        foreach (var project in resultObject.data)
                //        {

                //            // roots.id = project.value.id;
                //            // roots.name = project.value.name;
                //            // roots.altName = project.value.altName;
                //            // roots.imId = project.value.imId;
                //            // roots.about = project.value.about;
                //            // roots.gender = project.value.gender;
                //            // roots.about = project.value.about;
                //            // roots.jobTitle = project.value.jobTitle;
                //            // roots.score = Convert.ToDouble( project.value.score);
                //            // roots.access = project.value.access;
                //            // roots.enhanced =Convert.ToBoolean(project.value.enhanced);
                //            // roots.companyName = project.value.companyName;

                //            //foreach(var val in project.value.addresses)
                //            // {
                //            //     address.zipCode = "";
                //            //     address.city = "";
                //            //     address.countryCode = "";
                //            //     address.address = "";
                //            //     address.street = "";
                //            //     address.timeZone = "";
                //            //     address.type = "";
                //            //   roots.addresses.Add(address);
                //            // }



                //            //List name,altname,image,emailid,addresslist
                //            //roots.phones = project.phones;
                //            //roots.addresses = project.addresses;
                //            //  public List<object> internetAddresses { get; set; }
                //            //public List<string> badges { get; set; }
                //            //public List<object> tags { get; set; }
                //            //public int cacheTtl { get; set; }
                //            //public List<object> sources { get; set; }
                //            //public List<object> searchWarnings { get; set; }
                //            //public List<Survey> surveys { get; set; }
                //            //roots.commentsStats.showComments =Convert.ToBoolean( project.value.commentsStats);
                //            //roots.manualCallerIdPrompt = project.value.manualCallerIdPrompt;
                //            //roots.ns = project.value.ns;
                //            if (project.value == null)
                //                return project.ToString();
                //            else
                //                return project.value.ToString();
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        return ex.Message.ToString() + "500error";
                //    }
                //    //dynamic a = resultObject.data;
                //    //string id = a.
                //    // var a = json.data;
                //    ////dynamic json1 = JsonConvert.DeserializeObject(json);
                //    //string a =json.data;
                //    //var serializer = new JavaScriptSerializer();
                //    //var reds = (IDictionary<string, object>)serializer.DeserializeObject(strResult);
                //    ////MyMessageBox.Show((reds["data"]["addresses"]).ToString(),MyMessageBox.MyType.Info);
                //    //dynamic output = JsonConvert.DeserializeObject(((object[])reds["data"])[0].ToString());


                //    loResponseStream.Close();
                //    // close the response object
                //    respon.Close();
                //    return "";


                //}
                //catch (Exception ex)
                //{
                //    return ex.Message.ToString() + "500error";
                //}
            }


    }
}
