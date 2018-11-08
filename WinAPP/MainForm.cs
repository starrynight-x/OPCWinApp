using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using OPCAutomation;
using CefSharp.WinForms;
using CefSharp;
using Hylasoft.Opc.Da;

namespace WinAPP

{
    public partial class MainForm : Form
    {
        public static ChromiumWebBrowser WebBrowser = null;

        public static DaClient _client;
        public static String localIP = GetClientLocalIPv4Address(); // 本机IP地址
        public static string ClientUrl; // 客户端访问的服务地址

        public static JObject EQInfo; // 现场空调系统的变量名所对应的编码配置
        public static JObject sendEQInfo; // 传递给主站的空调系统数据

        public MainForm()
        {
            InitializeComponent();
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ReadJson();

            InitDA(); // 初始化OPC连接

            InitWebBrower();
        }

        public void InitWebBrower()
        {
            WebBrowser = new ChromiumWebBrowser(Application.StartupPath + @"\Index.html");
            this.Controls.Add(WebBrowser);

            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            WebBrowser.RegisterJsObject("JsObj", new WebSocketHandle(), new CefSharp.BindingOptions { CamelCaseJavascriptNames = false });
            //WebBrowser.RegisterJsObject("logInfo", LogHelp.logInfo, new CefSharp.BindingOptions { CamelCaseJavascriptNames = false });
            //WebBrowser.RegisterJsObject("logErr", LogHelp.logErr, new CefSharp.BindingOptions { CamelCaseJavascriptNames = false });
            //WebBrowser.RegisterJsObject("JsObj", new WebSocketHandle(), false);
        }

        public void InitDA()
        {
            String serverName = EQInfo["SERVER_NAME"].ToString();
            ClientUrl = "opcda://" + localIP + "/" + serverName;
            _client = new DaClient(new Uri(ClientUrl));
            _client.Connect();
        }

        public static void ReadJson()
        {
            string jsonfile = Application.StartupPath + @"\EQInfo.json";
            StreamReader file = null;
            try
            {
                file = File.OpenText(jsonfile);
            }
            catch
            {
                MessageBox.Show("未找到相应的配置文件！");
            }
            JsonTextReader reader = new JsonTextReader(file);
            EQInfo = (JObject)JToken.ReadFrom(reader);
        }

        /// <summary>  
        /// 获取客户端内网IPv4地址  
        /// </summary>  
        /// <returns>客户端内网IPv4地址</returns>  
        public static string GetClientLocalIPv4Address()
        {
            string strLocalIP = string.Empty;
            try
            {
                IPHostEntry ipHost = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHost.AddressList[0];
                strLocalIP = ipAddress.ToString();
                return strLocalIP;
            }
            catch
            {
                return "unknown";
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cef.Shutdown();
        }

    }
}
