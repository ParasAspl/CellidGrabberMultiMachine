using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodSharpDemo
{
    public class DataCleaner
    {
        /*String ARFCN;
        String BSIC;
        String DBM;
        String MCC;
        String MNC;
        String LAC;
        String CELLID;
        String CELLSTATUS;
        String CELLSUITABLE;
        String NUMARFCN;
        String ARFCN2;
        String NUMCHANNELS;
        String ARRAY;*/
        
        public   Dictionary<String,string> dataCleaner(String data)
        {
            Dictionary<String, String> map = new Dictionary<string, string>();
        String[] datas=data.Split(new char[] {','});
            if (datas.Length < 10)
                return null;
            foreach(var val in datas)
            {
                if (string.IsNullOrEmpty(val))
                    continue;
                String[] vals=val.Split(':');
                if(!map.ContainsKey(vals[0]))
                    map.Add(vals[0].Trim(),vals[1].Trim());

            }
            return map;
        }
        public  List<Dictionary<string, string>> clean(String[] lines)
        {
            List<Dictionary<string,string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> dict;
            foreach (var line in lines)
            {
                if(line.Contains("Network survey end"))
                {
                    MessageBox.Show("Scan completed");
                    break;
                }
                else if(line.Contains("ERROR"))
                {
                    MessageBox.Show("Error");
                    break;
                }
                dict=dataCleaner(line);
                if(dict!=null)
                {
                    list.Add(dict);
                }
            }
            return list;
        }
       
    }
}
