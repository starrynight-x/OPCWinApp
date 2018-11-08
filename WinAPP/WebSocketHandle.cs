using CefSharp;
using CefSharp.Internals;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WinAPP
{
    class WebSocketHandle
    {
        //实例化Timer类  
        System.Timers.Timer aTimer = new System.Timers.Timer();

        public void DoSomething()
        {

        }

        public void SetTimerParam()
        {
            //到时间的时候执行事件  
            aTimer.Elapsed += new ElapsedEventHandler(getData);
            aTimer.Interval = 1000;
            aTimer.AutoReset = true;//执行一次 false，一直执行true  
            //是否执行System.Timers.Timer.Elapsed事件  
            aTimer.Enabled = true;
        }

        private void getData(object source, System.Timers.ElapsedEventArgs e)
        {
            MainForm.WebBrowser.ExecuteScriptAsync("initWebsocket();");
            MainForm.sendEQInfo = new JObject();
            JArray sendEQArr = new JArray();
            MainForm.sendEQInfo.Add("ORG_NO", MainForm.EQInfo["ORG_NO"]);
            MainForm.sendEQInfo.Add("DATA_ITEMS", sendEQArr);

            JArray jArray = JArray.Parse(MainForm.EQInfo["DATA_ITEMS"].ToString());
            for (int i = 0; i < jArray.Count; i++)
            {
                JObject eqObject = JObject.Parse(jArray[i].ToString());
                String eq_name = eqObject["EQ_NAME"].ToString();
                //var data_val = _client.Read<String>(eq_name);
                //Console.WriteLine(eq_name + "：", data_val.Value);
                var data_val = MainForm._client.Read<String>("$Time.Value");
                eqObject.Add("DATA_VAL", data_val.Value);
                sendEQArr.Insert(i, eqObject);
            }

            MainForm.WebBrowser.ExecuteScriptAsync("_msg=" + JObject.Parse(MainForm.sendEQInfo.ToString()));
        }
    }
}
