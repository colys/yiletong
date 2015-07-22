﻿using Sashulin;
using Sashulin.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Common;

namespace WinForm
{

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class frmMain : Form
    {
        string userName="福州易乐通";
        string password = "Ylt123456";
        Queue<string> terminalQueue = new Queue<string>();
        string evalActionUrl;
        string controllerUrl;
        EncryptionUtility encryption;
		
		System.Threading.Thread waitQueryThread;
		bool inQuery = false,inMonitor=false;
        bool systemExit = false;
        public frmMain()
        {
            InitializeComponent();

            CSharpBrowserSettings settings = new CSharpBrowserSettings();
//			Uri url = new Uri ("file:///" + Application.StartupPath.Replace ('\\', '/') + "/main.html");
			Uri url = new Uri (Application.StartupPath + "\\main.html");
            settings.DefaultUrl = "http://teaerp.sinaapp.com/";
            //settings.UserAgent = "Mozilla/5.0 (Linux; Android 4.2.1; en-us; Nexus 4 Build/JOP40D) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Mobile Safari/535.19";
            settings.CachePath = @"C:\temp\caches";
            chromeWebBrowser1.Initialize(settings);
			webBrowser1.Url = url;
            webBrowser1.ObjectForScripting = this;
            myBrowser.ObjectForScripting = this;
            myPage.Hide();
			timer1.Interval = Convert.ToInt32 (getSetting ("listenInterval"));
            string host = getSetting("webHost");
            if (host[host.Length - 1] != '/') host += "/";
            controllerUrl = host + "Home";
            evalActionUrl = controllerUrl + "/Eval";
            encryption = new EncryptionUtility(Application.StartupPath + "\\yiletong.key");
			
        }

        private string getNextTerminal()
        {
            if (terminalQueue.Count == 0) { FillQueue(); }
			if (terminalQueue.Count == 0)
				return null;
            return terminalQueue.Dequeue();
        }
        System.Threading.Timer timer;
        public void InitOK()
        {
            chromeWebBrowser1.LoadHtml("正在载入，请稍后......");
            timer = new System.Threading.Timer(new System.Threading.TimerCallback(BeginLoadWeb), null, 0, 500);
            string timeStr;
            if (timer_pay.Interval > 59000)
            {
                double d = (timer_pay.Interval / 60000);
                if (d == 1) timeStr = "分钟";
                else timeStr = d + "分钟";
            }
            else
            {
                timeStr = (timer_pay.Interval / 1000) + "秒钟";
            }
            webBrowser1.Document.InvokeScript("setSumStatus", new string[] { DateTime.Now.ToString("HH:mm:ss") + "监控开始，每" + timeStr + "一次" });
        }

        private void BeginLoadWeb(object o)
        {
            timer.Change(-1, 0);
            this.BeginInvoke(new QueryFinish(LoadRemoteWeb));            
        }
        private void LoadRemoteWeb()
        {
            openLogin();
            tabControl1.SelectedIndex = 0;
        }
		public void onError(string msg){
            if (MessageBox.Show(msg, "发生异常，是否要退出？", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                Application.Exit();
            }
		}

        private void FillQueue()
        {
            string str= execQuery("customers", "terminal", "status = 1", null);
            JsonMessage<Customer[]> jm = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<Customer[]>>(str);
            if (jm.Message!=null) { 
                onError(jm.Message);
            }
            foreach (Customer ter in jm.Result)
            {
				terminalQueue.Enqueue (ter.terminal);
			}
        }

        private void 载入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadRemoteWeb();
        }

       

		private void openLogin(){
			chromeWebBrowser1.OpenUrl("https://119.4.99.217:7300/mcrm/login.jsp");
		}

        private void 触发登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CwbElement buttons = chromeWebBrowser1.Document.GetElementById("submitButton");
            buttons.Click();
        }

        private void 跳转到ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadFrame();
            
        }

        private void loadFrame()
		{			
			if (waitQueryThread != null && waitQueryThread.IsAlive)
				return;
			object length = chromeWebBrowser1.EvaluateScript ("$('#resTree .tree-node[node-id=10051]').length");
			if (length.Equals( 0)) {
				MessageBox.Show ("请先载入");
				return;
			}				
			string js= "$('#resTree .tree-node[node-id=10051]').trigger('click')')";
			chromeWebBrowser1.ExecuteScript (js);
			waitQueryThread = new System.Threading.Thread (new System.Threading.ParameterizedThreadStart (watinFrameLoad));
			waitQueryThread.Start ();

		}

        

        private void 触发查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object frameUrl = chromeWebBrowser1.EvaluateScript("window.frames['收单日志']==null");
            if (frameUrl == null || frameUrl.Equals(true))
            {
                MessageBox.Show("请先跳转到收单日志");
                return;
            }
			if (!GoQuery ())
				return;
            timer1.Start();            
            tabControl1.SelectedIndex = 1;
            触发查询ToolStripMenuItem.Enabled = false;
        }
        string termID,beginDate,endDate;
		private bool GoQuery()
        {
			if (waitQueryThread != null && waitQueryThread.IsAlive) return false;
			if (inQuery) return false;
            inQuery = true;
            //CwbElement tabFrame = chromeWebBrowser1.Document.GetElementById("tab1");
            //if(tabFrame==null) return;
            object notFunction = chromeWebBrowser1.EvaluateScript("typeof( window.frames['收单日志'].queryByCondition) == 'undefined'");
            if (notFunction != null && notFunction.Equals(true))
            {
                onError("完蛋了，查询方法被修改了！");
				return false;
            }
            else
            {

                termID = getNextTerminal();
				if (termID == null) {
					MessageBox.Show ("没有终端，请先添加客户");
					return false;
				}
                object date1 = chromeWebBrowser1.EvaluateScript(" $(window.frames['收单日志'].document).find('#beginstdate').next().find('.combo-value').val()");
                object date2 = chromeWebBrowser1.EvaluateScript(" $(window.frames['收单日志'].document).find('#endstdate').next().find('.combo-value').val()");
                if (date1!=null) beginDate = date1.ToString();
                else date1 = "找不到";
                if (date2 != null) endDate = date2.ToString();
                else endDate = "找不到";
               string js=@"
$(window.frames['收单日志'].document).find('#tid').val('" + termID + @"');
window.frames['收单日志'].queryByCondition();";
                chromeWebBrowser1.ExecuteScript(js);
                //chromeWebBrowser1.ExecuteScript("");
                setStatus(DateTime.Now.ToString("HH:mm:ss")+ "：正在查询" + termID + "的刷卡情况");
                waitQueryThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(watinQuery));
                waitQueryThread.Start();
            }
			return true;
        }
        public void OnQueryFinish()
        {
            setStatus(DateTime.Now.ToString("HH:mm:ss") + "：" + termID + "终端" + beginDate + "到" + endDate + "的刷卡情况:");
            object table = chromeWebBrowser1.EvaluateScript("$(window.frames['收单日志'].document).find('.datagrid-btable:eq(1)')[0].outerHTML");
            HtmlElement hidden_div = webBrowser1.Document.GetElementById("hidden_div");
            hidden_div.InnerHtml = table.ToString();
            object[] objects = new object[1];
            object jsonStr = webBrowser1.Document.InvokeScript("parseTableJson", objects);
            //            if(jsonStr!=null) MessageBox.Show(jsonStr.ToString());
            inQuery = false;
            while (waitQueryThread != null && waitQueryThread.IsAlive)
            {
                System.Threading.Thread.Sleep(100);
            }
            if (terminalQueue.Count > 0 && !systemExit) GoQuery();
        }
        private void setStatus(string str)
        {
            object[] objects = new object[1];
            objects[0] =str;
            webBrowser1.Document.InvokeScript("setStatus", objects);
        }

		public void OnFrameLoadFinish(){
			chromeWebBrowser1.ExecuteScript ("$(window.frames['收单日志'].document).find('.pagination-page-list:eq(0)').find('option:last').attr('selected',true);");
		}

     

        public delegate void QueryFinish();


		private void watinFrameLoad(object o)
		{
			while (true)
			{
				object isFinish= chromeWebBrowser1.EvaluateScript("typeof( window.frames['收单日志'].queryByCondition) != 'undefined'");
				if (isFinish != null && isFinish.Equals(true))
				{
					this.BeginInvoke(new QueryFinish(OnFrameLoadFinish));
					return;
				}
				else System.Threading.Thread.Sleep(300);
			}
		}


        private void watinQuery(object o)
        {
            while (true)
            {
               object isViable= chromeWebBrowser1.EvaluateScript("$(window.frames['收单日志'].document).find('.datagrid-mask-msg').is(':visible')");
				if (isViable != null && isViable.Equals(false))
               {
                   this.BeginInvoke(new QueryFinish(OnQueryFinish));
					while (inQuery) {
						System.Threading.Thread.Sleep(100);
					}
                   return;
               }
               else System.Threading.Thread.Sleep(300);
            }
        }

        private void chromeWebBrowser1_PageLoadFinishEventhandler(object sender, EventArgs e)
        { 
        }

        private void goLogin()
        {        
            chromeWebBrowser1.ExecuteScript("$('#username').val('"+userName+"')");
            chromeWebBrowser1.ExecuteScript("$('#password').val('" + password + "')");
            chromeWebBrowser1.ExecuteScript("$('input[name=codeVal]').focus()");
             
        }

        private void chromeWebBrowser1_BrowserNavigated(object sender, EventArgs e)
        {
            
        }

        private void chromeWebBrowser1_BrowserDocumentCompleted(object sender, EventArgs e)
		{
			if (chromeWebBrowser1.Document == null)
				return;
			int pos = chromeWebBrowser1.Url.LastIndexOf ("/");
			string pageName = chromeWebBrowser1.Url.Substring (pos + 1).ToLower ();
			switch (pageName) {
			case "default.html":
				openLogin ();
				break;
			case "login.jsp":
			case "j_spring_security_check":
                    //还要有登录失败的判断
				goLogin ();
				break;
			case "index":                    
				loadFrame();
				break;

			} 
		}

        private void chromeWebBrowser1_BrowserFrameLoadEnd(object sender, EventArgs e)
        {
            
        }

        private void chromeWebBrowser1_BrowserCreated(object sender, EventArgs e)
        {
			chromeWebBrowser1.LoadHtml("点击菜单 文件->载入");
			string url = "file:///" + Application.StartupPath.Replace ('\\', '/') + "/default.html";
			chromeWebBrowser1.OpenUrl (url);
           
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string cerFile = getSetting("publicCer");
                if (cerFile.IndexOf(":") != 1) { 
                    //相对路径
                    if (cerFile[0] != '\\' && cerFile[0] != '/') { cerFile = "\\" + cerFile.Replace('/', '\\'); }
                    cerFile = Application.StartupPath + cerFile;
                }
			
            }
            catch (Exception ex)
            {
                onError(ex.Message);
            }
        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (!inQuery) GoQuery();
        }


        #region mysql
        public string GetTableName(string table)
        {
            return RunHttp("GetTableName", table);           
        }
        //string table_Suffix="";
        
        //MySqlConnection conn;
        //MySqlCommand cmd;
        

        //private void OpenMysql(){           
        //    string conStr = getSetting ("connstr");
        //    if (conn == null) conn = new MySqlConnection(conStr);
        //    if(conn.State== ConnectionState.Closed) conn.Open();
        //}

        private string getSetting(string name)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[name] == null)
            {
                MessageBox.Show("配置文件不正确！");
                Application.Exit();
            }
            return System.Configuration.ConfigurationManager.AppSettings[name].ToString();
        }

   

        public string GetNextVal(string table)
        {

            return RunHttp("GetNextVal", table);
          
        }

        private string RunHttp(string action,params string[] paraStrArr)
        {
            MyHttpUtility http = new MyHttpUtility();
            string content = "action=" + action + "&dataArrStr=";
           
            if(paraStrArr!=null){
                foreach (string str in paraStrArr)
                {
                    content += System.Web.HttpUtility.UrlEncode(str) + ",";
                }
                content = content.Substring(0,content.Length - 1);
            }
			try{
            return http.DoPost(evalActionUrl, encryption.EncryptData(content));
			}catch(Exception ex){
				onError (ex.Message);
				return null;
			}
        }
       
        public string execQuery(string table,string fields,string where,string order){
            return RunHttp("ExecQuery", table, fields, where, order);
        
        }

        public string execDb(string jsonArrStr){
            return RunHttp("ExecDb", jsonArrStr);        
		}
	

        #endregion

        private void 客户列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoToMyUrl("customers.html", 客户列表ToolStripMenuItem.Text);
        }

        private void GoToMyUrl(string pageName,string title)
        {
            myPage.Text = title;
            myPage.Show();
            tabControl1.SelectedTab = myPage;
            myBrowser.Url = new Uri(Application.StartupPath + "\\"+ pageName);            
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            systemExit = true;
			if (inQuery || inMonitor)
            {                
                MessageBox.Show("正在查询，请稍候。。。");                
                e.Cancel = true;
                return;
            }
            timer_pay.Enabled = false;
            timer1.Enabled = false;
        }

        private void 结算记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoToMyUrl("transactionsum.html", 结算记录ToolStripMenuItem.Text);
        }

        private void 添加客户ToolStripMenuItem_Click(object sender, EventArgs e)
        {
			GoToMyUrl("transactionlog.html", transactionlogToolStripMenuItem.Text);
        }

        private void sqlToolStripMenuItem_Click(object sender, EventArgs e)
        {
			string json = execDb ("[{ table:\"transactionLogs\" ,action: 1,fields:[\"status\"],values:[1],where:\"1=1\"}]");
			MessageBox.Show (json);           
        }

        private void timer_pay_Tick(object sender, EventArgs e)
        {
            if (systemExit) { timer_pay.Enabled = false; return; }
            if (inMonitor) return;
            inMonitor = true;
            AppendSumLog("发起结算");
            var json = RunHttp("ToRongBao");
            JsonMessage<string> resultJson= Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<string>>(json);
            if (!string.IsNullOrEmpty(resultJson.Message))
            {
                AppendSumLog("结算失败：" + resultJson.Message);
            }
            else
            {
               
                if (!string.IsNullOrEmpty(resultJson.Result))
                {
                    string[] terminal = resultJson.Result.Split(',');
                    AppendSumLog("上传成功,终端号有：" + resultJson.Result);
                    AppendSumLog("监控银行处理情况");
                    System.Threading.Thread monitorThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(MonitorBankResult));
                    monitorThread.Start(resultJson.Result);
                }
                else
                {
                    AppendSumLog("没有要结算的交易！");
                }
               
            }
            inMonitor = false;
        }

        private void AppendSumLog(string str)
        {
            webBrowser1.Document.InvokeScript("appendSumLog", new string[] { DateTime.Now.ToString("HH:mm:ss") +":"+ str });
        }


        public void MonitorBankResult(object obj)
        {
            string[] terminals = obj.ToString().Split();
            MyHttpUtility http = new MyHttpUtility();
            string result = http.DoGet("");
        }
	}


    public class Customer{
        public string terminal;
    }

}
