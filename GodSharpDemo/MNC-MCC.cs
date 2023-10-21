using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CligenceCellIDGrabber
{
    public static class MNC_MCC
    {
        private static string _message;
        public static string Message
        {
            get
            {
                // Reads are usually simple
                return _message;
            }
            set
            {
                // You can add logic here for race conditions,
                // or other measurements
                _message = value;
            }
        }
        public static string GetMCCMNC = "404||01||Vodafone Idea||Haryana||Operational||GSM 900||"
                             + Environment.NewLine + "404||02||Airtel||Punjab||Operational||GSM 900||" + Environment.NewLine +
   "404||03||Airtel||Himachal Pradesh||Operational||GSM 900||" + Environment.NewLine +
   "404||04||Vodafone Idea||Delhi & NCR||Operational||GSM 1800||" + Environment.NewLine +
   "404||05||Vodafone Idea||Gujarat||Operational||GSM 900||Former Hutch / Fascel /Vodafone India" + Environment.NewLine +
"404||07||Vodafone Idea||Andhra Pradesh and Telangana||Operational||GSM 900||former IDEA" + Environment.NewLine +
"404||1||Vodafone Idea||Haryana||Operational||GSM 900||" + Environment.NewLine +
"404||2||Airtel||Punjab||Operational||GSM 900||" + Environment.NewLine +
"404||3||Airtel||Himachal Pradesh||Operational||GSM 900||" + Environment.NewLine +
"404||4|| Vodafone Idea||Delhi & NCR||Operational||GSM 1800||" + Environment.NewLine +
"404||5||Vodafone Idea||Gujarat||Operational||GSM 900||Former Hutch / Fascel /Vodafone India" + Environment.NewLine +
"404||7||Vodafone Idea||Andhra Pradesh and Telangana||Operational||GSM 900||former IDEA" + Environment.NewLine +
"404||10||Airtel||Delhi & NCR||Operational||GSM 900||" + Environment.NewLine +
"404||11||Vodafone Idea||Delhi & NCR||Operational||GSM 900 / GSM 1800||former Vodafone India" + Environment.NewLine +
"404||12||Vodafone Idea||Haryana||Operational||GSM 900||Former Escotel IDEA " + Environment.NewLine +
"404||13||Vodafone Idea||Andhra Pradesh and Telangana||Operational||GSM 1800||former Vodafone India " + Environment.NewLine +
"404||14||Vodafone Idea||Punjab||Operational||GSM 900 / GSM 1800||Former Spice IDEA " + Environment.NewLine +
"404||15||Vodafone Idea||Uttar Pradesh(East)||Operational||GSM 900||former Vodafone India " + Environment.NewLine +
"404||16||Airtel||North East||Operational||GSM 900||Former Hexacom " + Environment.NewLine +
"404||19|| Vodafone Idea||Kerala||Operational||GSM 900 / GSM 1800||Former Escotel IDEA " + Environment.NewLine +
"404||20||Vodafone Idea||Mumbai||Operational||GSM 900 / UMTS 2100||Former Hutchison Maxtouch / Orange / Hutch / Vodafone India" + Environment.NewLine +
"404||22||Vodafone Idea||Maharashtra & Goa||Operational||GSM 900||former IDEA " + Environment.NewLine +
"404||24||Vodafone Idea||Gujarat||Operational||GSM 900||former IDEA " + Environment.NewLine +
"404||27||Vodafone Idea||Maharashtra & Goa||Operational||GSM 900||former Vodafone India" + Environment.NewLine +
"404||30||Vodafone Idea||West Bengal||Operational||GSM 900 / GSM 1800||former Vodafone India Command / Hutch " + Environment.NewLine +
"404||31||Airtel||West Bengal||Operational||GSM 900|| " + Environment.NewLine +
"404||34||Cellone||Haryana||Operational||GSM 900 / UMTS 2100|| " + Environment.NewLine +
"404||38||Cellone||Assam||Operational||GSM 900 / UMTS 2100|| " + Environment.NewLine +
"404||40||Airtel||Tamil Nadu||Operational||Unknown|| " + Environment.NewLine +
"404||43||Vodafone Idea||Tamil Nadu||Operational||GSM 900||former Vodafone India " + Environment.NewLine +
"404||44||Vodafone Idea||Karnataka||Operational||GSM 900 / LTE 1800||Former Spice IDEA" + Environment.NewLine +
"404||45||Airtel||Karnataka||Operational||GSM / TD-LTE 2300|| " + Environment.NewLine +
"404||46||Vodafone Idea||Kerala||Operational||GSM 900||former Vodafone India" + Environment.NewLine +
"404||49||Airtel||Andhra Pradesh and Telangana||Operational||GSM 900||" + Environment.NewLine +
"404||51||Cellone||Himachal Pradesh||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||53||Cellone||Punjab||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||54||Cellone||Uttar Pradesh(West) & Uttarakhand||Operational||GSM 900 / UTMS 2100||" + Environment.NewLine +
"404||55||Cellone||Uttar Pradesh(East)||Operational||GSM 900 / UTMS 2100|| " + Environment.NewLine +
"404||56||Vodafone Idea||Uttar Pradesh(West) & Uttarakhand||Operational||GSM 900||former IDEA" + Environment.NewLine +
"404||57||Cellone||Gujarat||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||58||Cellone||Madhya Pradesh & Chhattisgarh||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||59||Cellone||Rajasthan||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||60||Vodafone Idea||Rajasthan||Operational||GSM 900||former Vodafone India" + Environment.NewLine +
"404||62||Cellone||Jammu & Kashmir||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||64||Cellone||Tamil Nadu||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||66||Cellone||Maharashtra & Goa||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||68||DOLPHIN||Delhi & NCR||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||69||DOLPHIN||Mumbai||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||70||Airtel||Rajasthan||Operational||Unknown||" + Environment.NewLine +
"404||71||Cellone||Karnataka(Bangalore)||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||72||Cellone||Kerala||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||73||Cellone||Andhra Pradesh and Telangana||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||74||Cellone||West Bengal||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||75||Cellone||Bihar||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||76||Cellone||Orissa||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||77||Cellone||North East||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||78||Vodafone Idea||Madhya Pradesh & Chhattisgarh||Operational||GSM 900 / UMTS 2100||former IDEA" + Environment.NewLine +
"404||79||Cellone||Andaman Nicobar||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||80||Cellone||Tamil Nadu||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||81||Cellone||West Bengal||Operational||GSM 900 / UMTS 2100||" + Environment.NewLine +
"404||82||Vodafone Idea||Himachal Pradesh||Operational||unknown||former IDEA[citation needed]" + Environment.NewLine +
"404||84||Vodafone Idea||Tamil Nadu||Operational||GSM 1800||former Vodafone India" + Environment.NewLine +
"404||86||Vodafone Idea||Karnataka||Operational||GSM 900 / UMTS 2100 / LTE 1800||former Vodafone India" + Environment.NewLine +
"404||87||Vodafone Idea||Rajasthan||Operational||unknown||former IDEA" + Environment.NewLine +
"404||88||Vodafone Idea||Vodafone Punjab||Operational||unknown||former Vodafone India[citation needed] " + Environment.NewLine +
"404||89||Vodafone Idea||Uttar Pradesh(East)||Operational||Unknown||former IDEA" + Environment.NewLine +
"404||90||Airtel||Maharashtra & Goa||Operational||GSM 1800||" + Environment.NewLine +
"404||92||Airtel||Mumbai||Operational||GSM 1800 / UMTS 2100||" + Environment.NewLine +
"404||93||Airtel||Madhya Pradesh & Chhattisgarh||Operational||GSM 1800||" + Environment.NewLine +
"404||94||Airtel||Tamil Nadu||Operational||Unknown||" + Environment.NewLine +
"404||95||Airtel||Kerala||Operational||GSM 1800||" + Environment.NewLine +
"404||96||Airtel||Haryana||Operational||GSM 1800||" + Environment.NewLine +
"404||97||Airtel||Uttar Pradesh(West) & Uttarakhand||Operational||Unknown||" + Environment.NewLine +
"404||98||Airtel||Gujarat||Operational||Unknown||" + Environment.NewLine +
"405||51||Airtel||West Bengal||Operational||GSM 900||" + Environment.NewLine +
"405||52||Airtel||Bihar & Jharkhand||Operational||GSM 900||" + Environment.NewLine +
"405||53||Airtel||Orissa||Operational||GSM||" + Environment.NewLine +
"405||54||Airtel||Uttar Pradesh(East)||Operational||GSM 900||" + Environment.NewLine +
"405||55||Airtel||Jammu & Kashmir||Operational||GSM 900 / UTMS 2100||" + Environment.NewLine +
"405||56||Airtel||Assam||Operational||GSM 900 / GSM 1800||" + Environment.NewLine +
"405||66||Vodafone Idea||Uttar Pradesh(West) & Uttarakhand||Operational||GSM 900 / GSM 1800||former Vodafone India" + Environment.NewLine +
"405||67||Vodafone Idea||West Bengal||Operational||Unknown||former Vodafone India" + Environment.NewLine +
"405||70||Vodafone Idea||Bihar & Jharkhand||Operational||GSM 1800||former IDEA" + Environment.NewLine +
"405||750||Vodafone Idea||Jammu & Kashmir||Operational||Unknown||former Vodafone India" + Environment.NewLine +
"405||751||Vodafone Idea||Assam||Operational||GSM 1800||former Vodafone India" + Environment.NewLine +
"405||752||Vodafone Idea||Bihar & Jharkhand||Operational||GSM 1800||former Vodafone India" + Environment.NewLine +
"405||753||Vodafone Idea||Orissa||Operational||GSM 1800||former Vodafone India" + Environment.NewLine +
"405||754||Vodafone Idea||Himachal Pradesh||Operational||GSM 1800||former Vodafone India" + Environment.NewLine +
"405||755||Vodafone Idea||North East||Operational||GSM 1800||former Vodafone India" + Environment.NewLine +
"405||756||Vodafone Idea||Madhya Pradesh & Chhattisgarh||Operational||GSM 1800||former Vodafone India" + Environment.NewLine +
"405||799||Vodafone Idea||Mumbai||Operational||GSM 900 / GSM 1800||former IDEA" + Environment.NewLine +
"405||840||Jio||West Bengal||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||845||Vodafone Idea||Assam||Operational||GSM 1800||former IDEA" + Environment.NewLine +
"405||846||Vodafone Idea||Jammu & Kashmir||Operational||GSM 1800 / UTMS 2100||former IDEA" + Environment.NewLine +
"405||847||Vodafone Idea||Karnataka||Operational||GSM 1800||former IDEA" + Environment.NewLine +
"405||848||Vodafone Idea||West Bengal||Operational||GSM 1800||former IDEA" + Environment.NewLine +
"405||849||Vodafone Idea||North East||Operational||GSM 1800||former IDEA" + Environment.NewLine +
"405||850||Vodafone Idea||Orissa||Operational||GSM 1800||former IDEA" + Environment.NewLine +
"405||851||Vodafone Idea||Punjab||Operational||GSM 1800||former IDEA" + Environment.NewLine +
"405||852||Vodafone Idea||Tamil Nadu||Operational||GSM 1800||former IDEA" + Environment.NewLine +
"405||853||Vodafone Idea||West Bengal||Operational||GSM 1800||former IDEA" + Environment.NewLine +
"405||854||Jio||Andhra Pradesh||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||855||Jio||Assam||Operational||LTE 850 / LTE 1800 / TD-LTE 2300||" + Environment.NewLine +
"405||856||Jio||Bihar||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||857||Jio||Gujarat||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||858||Jio||Haryana||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||859||Jio||Himachal Pradesh||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||860||Jio||Jammu & Kashmir||Operational||LTE 850 / LTE 1800 / TD-LTE 2300||" + Environment.NewLine +
"405||861||Jio||Karnataka||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||862||Jio||Kerala||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||863||Jio||Madhya Pradesh & Chhattisgarh||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||864||Jio||Maharashtra & Goa||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||865||Jio||North East||Operational||LTE 850 / LTE 1800 / TD-LTE 2300||" + Environment.NewLine +
"405||866||Jio||Orissa||Operational||LTE 850 / LTE 1800 / TD-LTE 2300||" + Environment.NewLine +
"405||867||Jio||Punjab||Operational||LTE 850 / LTE 1800 / TD-LTE 2300||" + Environment.NewLine +
"405||868||Jio||Rajasthan||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||869||Jio||Tamil Nadu||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||870||Jio||Uttar Pradesh(West) & Uttarakhand||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||871||Jio||Uttar Pradesh(East)||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||872||Jio||Delhi & NCR||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||873||Jio||West Bengal||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||874||Jio||Mumbai||Operational||LTE 850 / LTE 1800 / TD-LTE 2300|| " + Environment.NewLine +
"405||908||Vodafone Idea||Andhra Pradesh and Telangana||Operational||GSM 1800||former IDEA " + Environment.NewLine +
"405||909||Vodafone Idea||Delhi & NCR||Operational||GSM 1800||former IDEA " + Environment.NewLine +
"405||910||Vodafone Idea||Haryana||Operational||GSM 1800||former IDEA" + Environment.NewLine +
"405||911||Vodafone Idea||Maharashtra & Goa||Operational||GSM 1800||former IDEA"  ;
}


}
//https://cellid.lisw.in/publish/
//Cell Id Key	
//https://msg.ccas.in/api/cellId/productKey	
//key: 62220182b8deb used but deactivated
// Post
//642d07038f2b5
//642d07038fa2f
//642d07038fd71
//642d070390752
//642d070390b97


//deacgivated key
//642d07581fcae	

//used key
//6413f247709f2



static class ArrayExtensions
{
    public static int FindIndex<T>(this T[] array, Predicate<T> match)
    {
        return Array.FindIndex(array, match);
    }
}
//public static IEnumerable<int> FindIndexes<T>(this IEnumerable<T> items, Func<T, bool> predicate)
//{
//    int index = 0;
//    foreach (T item in items)
//    {
//        if (predicate(item))
//        {
//            yield return index;
//        }

//        index++;
//    }
//}

//AT + CCINFO

//+ CCINFO: [SCELL],ARFCN: 7,MCC: 404,MNC: 70,LAC: 660,ID: 35588,BSIC: 47,RXLev: -71dBm,C1: 30,C2: 30,TA: 0,TXPWR: 0

//+ CCINFO: [NCELL1],ARFCN: 1,MCC: 404,MNC: 70,LAC: 660,ID: 1161,BSIC: 2,RXLev: -80dBm,C1: 20,C2: 20

//+ CCINFO: [NCELL2],ARFCN: 2,MCC: 404,MNC: 70,LAC: 660,ID: 2172,BSIC: 30,RXLev: -89dBm,C1: 11,C2: 3

//+ CCINFO: [NCELL3],ARFCN: 4,MCC: 404,MNC: 70,LAC: 660,ID: 47738,BSIC: 29,RXLev: -89dBm,C1: 16,C2: 16

//+ CCINFO: [NCELL4],ARFCN: 5,MCC: 404,MNC: 70,LAC: 660,ID: 2173,BSIC: 20,RXLev: -87dBm,C1: 13,C2: 13

//OK
//public string VersionLabel
//{
//    get
//    {
//        if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
//        {
//            Version ver = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
//            return string.Format("Product Name: {4}, Version: {0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision, Assembly.GetEntryAssembly().GetName().Name);
//        }
//        else
//        {
//            var ver = Assembly.GetExecutingAssembly().GetName().Version;
//            return string.Format("Product Name: {4}, Version: {0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision, Assembly.GetEntryAssembly().GetName().Name);
//        }
//    }
//}