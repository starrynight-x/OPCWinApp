using CefSharp;
using CefSharp.Internals;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace WinAPP
{
    class WebSocketHandle
    {
        //实例化Timer类  
        System.Timers.Timer aTimer = new System.Timers.Timer();

        string websocket_url = GetAppConfig("websocket_url");

        public void DoSomething()
        {

        }

        public void SetTimerParam()
        {
            websocket_url += MainForm.EQInfo["ORG_NO"];
            MainForm.WebBrowser.ExecuteScriptAsync("initWebsocket", websocket_url);

            aTimer.Start();
            //bool startFlag = false;
            //while (!startFlag)
            //{
            //    var currentTime = DateTime.Now;
            //    if (currentTime.Second % 5 == 0)
            //    {
            //        aTimer.Start();
            //        startFlag = true;
            //    }
            //}

            //到时间的时候执行事件  
            aTimer.Elapsed += new ElapsedEventHandler(getData);
            aTimer.Interval = 1000;
            aTimer.AutoReset = true;//执行一次 false，一直执行true  
            //是否执行System.Timers.Timer.Elapsed事件  
            aTimer.Enabled = true;
        }
        //JArray jArray = null;
        private void getData(object source, System.Timers.ElapsedEventArgs e)
        {
            var currentTime = DateTime.Now;
            if (currentTime.Second % 5 != 0)
            {
                return;
            }

            MainForm.sendEQInfo = new JObject();
            JArray sendEQArr = new JArray();
            MainForm.sendEQInfo.Add("DATA_TIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            MainForm.sendEQInfo.Add("ORG_NO", MainForm.EQInfo["ORG_NO"]);
            MainForm.sendEQInfo.Add("DATA_ITEMS", sendEQArr);

            JArray jArray = JArray.Parse(MainForm.EQInfo["DATA_ITEMS"].ToString());
            for (int i = 0, j = 0; i < jArray.Count; i++)
            {
                JObject eqObject = JObject.Parse(jArray[i].ToString());
                String eq_name = eqObject["EQ_NAME"].ToString();
                var data_val = new Object();
                try
                {
                    String data_type = eqObject["DATA_TYPE"].ToString();
                    switch (data_type)
                    {
                        case "01":
                        case "05":
                            data_val = MainForm._client.Read<Boolean>(eq_name + ".Value").Value;
                            break;
                        case "02":
                        case "06":
                            data_val = MainForm._client.Read<int>(eq_name + ".Value").Value;
                            break;
                        case "03":
                        case "07":
                            data_val = MainForm._client.Read<Single>(eq_name + ".Value").Value;
                            break;
                        case "04":
                        case "08":
                            data_val = MainForm._client.Read<String>(eq_name + ".Value").Value;
                            break;
                    }
                    //Type data_type = MainForm._client.GetDataType(eq_name + ".Value").Name;
                    //switch (data_type)
                    //{
                    //    case "Int16":
                    //    case "Int32":
                    //    case "float":
                    //    case "double":
                    //        data_val = MainForm._client.Read<Single>(eq_name + ".Value").Value;
                    //        break;
                    //    case "boolean":
                    //    case "Boolean":
                    //        data_val = MainForm._client.Read<Boolean>(eq_name + ".Value").Value;
                    //        break;
                    //    case "string":
                    //    case "String":
                    //        data_val = MainForm._client.Read<String>(eq_name + ".Value").Value;
                    //        break;
                    //}
                }
                catch (NullReferenceException ex)
                {
                    LogHelp.Error(ex.Message);
                    data_val = null;
                }

                if (data_val != null)
                {
                    Console.WriteLine(eq_name + "：{0}", data_val);
                    eqObject.Add("DATA_VAL", data_val == null ? "" : data_val.ToString());
                    sendEQArr.Insert(j, eqObject);
                    j++;
                }

            }

            //MainForm.WebBrowser.ExecuteScriptAsync("_msg=" + JObject.Parse(MainForm.sendEQInfo.ToString()));
            //MainForm.WebBrowser.ExecuteScriptAsync("initWebsocket", websocket_url);
            try
            {
                MainForm.WebBrowser.ExecuteScriptAsync("send", JObject.Parse(MainForm.sendEQInfo.ToString()));
                LogHelp.Info(MainForm.sendEQInfo.ToString());
            } catch(Exception ex)
            {
                LogHelp.Error(ex.Message);
            }
        }

        ///<summary> 
        ///返回*.exe.config文件中appSettings配置节的value项  
        ///</summary> 
        ///<param name="strKey"></param> 
        ///<returns></returns> 
        public static string GetAppConfig(string strKey)
        {
            string file = Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == strKey)
                {
                    return config.AppSettings.Settings[strKey].Value.ToString();
                }
            }
            return null;
        }
    }
}
