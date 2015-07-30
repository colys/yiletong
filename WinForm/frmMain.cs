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
		Queue<Customer> terminalQueue = new Queue<Customer>();
        string evalActionUrl;
        string controllerUrl;
        EncryptionUtility encryption;
		
		System.Threading.Thread waitQueryThread;
		bool inQuery = false,inMonitor=false;
        bool systemExit = false;
		int listenInterval;
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
			timer1.Interval = listenInterval=Convert.ToInt32 (getSetting ("listenInterval"));
            string host = getSetting("webHost");
            if (host[host.Length - 1] != '/') host += "/";
            controllerUrl = host + "Home";
            evalActionUrl = controllerUrl + "/Eval";
			encryption = new EncryptionUtility("TfoPqado2GvjxvC1GsmY6Q==");
			
        }

		private Customer getNextTerminal()
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
            timer = new System.Threading.Timer(new System.Threading.TimerCallback(BeginLoadWeb), null, 100, -1);
            string timeStr;
			timer_pay.Interval = Convert.ToInt32 (getSetting ("jieSuangInterval"));
			timer_pay.Enabled = false;
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
			string qjson= execQuery ("transactionsum", "distinct batchCurrnum,uploadDate", "status in (1,2) ", null);
			JsonMessage<Dictionary<string,string>[]> jm = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<Dictionary<string,string>[]>>(qjson);
			if (jm.Message != null) {
				onError ("query transactionsum un recive data exception");
			} else {
				Dictionary<string,string>[] data = jm.Result;
				foreach (Dictionary<string,string> dic in data) {
					startModnitorank (new string[]{ dic ["batchCurrnum"], Convert.ToDateTime( dic ["uploadDate"]).ToString("yyyyMMdd"), "" });
				}
			}	
			timer_pay.Start ();

        }



        private void BeginLoadWeb(object o)
        {            
            this.BeginInvoke(new QueryFinish(LoadRemoteWeb));            
        }
        private void LoadRemoteWeb()
        {
            openLogin();
            tabControl1.SelectedIndex = 0;
        }

		public void onError(string msg,Exception ex = null,bool inThead=false){
			log4net.ILog log = log4net.LogManager.GetLogger(this.GetType());
			if (ex != null) {
				string output = DateTime.Now.ToString () + " exceptiont:" + ex.Message;
				if (ex.StackTrace != null)
					output += " :" + ex.StackTrace;				
				log.Error (msg, ex);
			} else {				
				log.Error (msg);
			}
			if (msg == null)
				msg = ex.Message;
			else if (ex != null) {
				msg += ex.Message;
			}
			if (inThead)
				this.BeginInvoke (new delegateOnParam (setStatus),msg);
			else	setStatus (msg);

		}

        private void FillQueue()
        {
			string str= execQuery("customers", "terminal,lastQuery", "status = 1", null);
            JsonMessage<Customer[]> jm = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<Customer[]>>(str);
			if (jm.Message != null) { 
				throw new Exception (jm.Message);
			} else {
				if (jm.Result == null)
					throw new Exception ("查询客户时返回空结果，可能是网络问题");
				foreach (Customer ter in jm.Result) {
					terminalQueue.Enqueue (ter);
				}
			}
        }

        private void 载入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadRemoteWeb();
        }

       

		private void openLogin(){			
			chromeWebBrowser1.OpenUrl("https://119.4.99.217:7300/mcrm/login.jsp");
			loginOutime = false;
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
			if (inQuery)
				return;
			if (cookieStr == null) cookieStr = chromeWebBrowser1.Document.Cookie;
			GoQuery (null);
            timer1.Start();            
            触发查询ToolStripMenuItem.Enabled = false;
			this.tabControl1.SelectedIndex = 1;
        }
        string termID,beginDate,endDate;
		private void GoQuery(object o)
        {
			if (inQuery)	return;
			if (waitQueryThread != null && waitQueryThread.IsAlive) return;
			timer1.Enabled = false;
			try
			{
				
				inQuery = true;
				//string cookie = chromeWebBrowser1.Document.Cookie;
				//MessageBox.Show (cookie);
				Customer cus = getNextTerminal();
				termID = cus.terminal;
				DateTime dtLastQuery;
				if(string.IsNullOrEmpty(cus.lastQuery)){
					if(DateTime.Now.Hour< 10){ dtLastQuery=DateTime.Today.AddDays(-1);}
					else dtLastQuery=DateTime.Today;
				}
				else dtLastQuery = Convert.ToDateTime(cus.lastQuery);
				beginDate = dtLastQuery.ToString("yyyyMMdd");
				if(DateTime.Now.Hour>22)
					endDate=DateTime.Today.AddDays(1).ToString("yyyyMMdd");
				else 
					endDate=DateTime.Today.ToString("yyyyMMdd");
				setStatus("正在查询" + termID + "的刷卡情况");
				string js = "$('#hidden_json').val(''); $.ajax({url:'https://119.4.99.217:7300/mcrm/bca/txnlog_findBy',type:'POST',data:'beginstdate="+beginDate+"&endstdate="+endDate+"&branchId=&refno=&mid=&tid="+termID+"&midName=&transid=&rspcode=&mgrid=&rsp=&lpName=&rows=100&page=1',error:function(str,e){window.CallCSharpMethod('queryJsReturn',e);},success:function(d){ var str= JSON.stringify(d);$('#hidden_json').val(str);window.CallCSharpMethod('queryJsReturn','ok'); }})";
				chromeWebBrowser1.ExecuteScript(js);
				waitQueryThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(watinQuery));
				waitQueryThread.Start();
			}
			catch (Exception ex)
			{
				onError(null,ex);
			}
        }
		 
		private bool loginOutime = false;

		public void OnQueryFinish()
        {			
			setStatus(termID + "终端" + beginDate + "到" + endDate + "的刷卡情况:");
			object jsonObj= chromeWebBrowser1.EvaluateScript ("$('#hidden_json').val()");
			if (jsonObj == null) onError ("OnQueryFinish get json error", null);
			string json = jsonObj.ToString ();
            //object table = chromeWebBrowser1.EvaluateScript("$(window.frames['收单日志'].document).find('.datagrid-btable:eq(1)')[0].outerHTML");
            //HtmlElement hidden_div = webBrowser1.Document.GetElementById("hidden_div");
            //hidden_div.InnerHtml = table.ToString();
			string[] objects = new string[1];
			objects [0] = json;
			try{
            	object jsonStr = webBrowser1.Document.InvokeScript("parseTableJson", objects);
				string msg= execDb("[{\"table\":\"customers\",\"action\":1,\"fields\":[\"lastQuery\"],\"values\":[\""+ DateTime.Today.ToString("yyyy-MM-dd") +"\"],where:\"terminal="+termID+"\"}]");
				JsonMessage<int> jsonR = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<int>>(msg);
				if(!string.IsNullOrEmpty( jsonR.Message)) throw new Exception(jsonR.Message);
			}catch(Exception ex){
				onError ("save data or update last query exception:", ex);
			}
			inQuery = false;//让waitQueryThread退出
            while (waitQueryThread != null && waitQueryThread.IsAlive)
            {
                System.Threading.Thread.Sleep(100);
            }
			if (systemExit||loginOutime)
				return;
			if (terminalQueue.Count > 0 ) {
				timer1.Interval = 1000;
			} else
				timer1.Interval = listenInterval;
			timer1.Start ();
        }

		private void setStatus(string str)
        {
            object[] objects = new object[1];
			objects[0] = DateTime.Now.ToString("HH:mm:ss") + "：" + str;
			webBrowser1.Document.InvokeScript ("setStatus", objects);				
        }

		public void OnFrameLoadFinish(){
			chromeWebBrowser1.ExecuteScript ("$(window.frames['收单日志'].document).find('.pagination-page-list:eq(0)').find('option:last').attr('selected',true);");
		}

		public delegate void delegateTwoParam(string str1,string str2);
		public delegate void delegateOnParam(string str);
        public delegate void QueryFinish();
		public delegate void QueryBankFinish(QueryResult result);


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

		public void queryJsReturn(string str){
			if (str != "ok") {
				onError (str);
				inQuery = false;
			} else {
				Console.WriteLine (str);
				//this.BeginInvoke(new QueryFinish(OnQueryFinish));
			}
		}

        private void watinQuery(object o)
        {
            while (true)
            {
				System.Threading.Thread.Sleep(500);
				object val= chromeWebBrowser1.EvaluateScript("$('#hidden_json').val()");
				if (val !=null && !val.Equals(string.Empty))
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

		string cookieStr = null;
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
				cookieStr = null;
				break;
			case "index":                    
				loadFrame ();
				chromeWebBrowser1.ExecuteScript("if($(\"#hidden_json\").length ==0){ $('<input type=\"hidden\" id=\"hidden_json\" />').appendTo(document.body); }");
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
			GoQuery (null);
        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {			
			
			if (cookieStr != chromeWebBrowser1.Document.Cookie || loginOutime) {
				cookieStr = null;
				timer1.Enabled = false;
				触发查询ToolStripMenuItem.Enabled = true;
				openLogin ();
				return;
			}
			GoQuery (null);

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
			MyHttpUtility http = new MyHttpUtility ();
			string content = "action=" + action + "&dataArrStr=";
           
			if (paraStrArr != null) {
				foreach (string str in paraStrArr) {
					content += System.Web.HttpUtility.UrlEncode (str) + ",";
				}
				content = content.Substring (0, content.Length - 1);
			}
			try {
				return http.DoPost (evalActionUrl, encryption.EncryptData (content));
			} catch (Exception ex) {
				onError ("DoPost exception:"+ content  +" ,", ex);
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

		private void startModnitorank(string[] result){
			if (!string.IsNullOrEmpty (result [2])) {
				AppendSumLog ("上传成功,终端号有：" + result [2]);
			}
			AppendSumLog("监控银行处理情况："+ result[0]);
			System.Threading.Thread monitorThread;
			if(queryBankTherads.TryGetValue(result[0],out monitorThread))
			{
				if (monitorThread.IsAlive || monitorThread.IsBackground) {
					onError ("已经有监听" + result [0] + "银行处理结果的线程！");
					return;
				} else {
					monitorThread.Abort ();
					queryBankTherads.Remove (result [0]);
				}
			}
			monitorThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(MonitorBankResult));
			monitorThread.Start(result);
			queryBankTherads.Add (result [0], monitorThread);
		}

		System.Collections.Generic.Dictionary<string,System.Threading.Thread> queryBankTherads = new Dictionary<string, System.Threading.Thread> ();

        private void timer_pay_Tick(object sender, EventArgs e)
        {
            if (systemExit) { timer_pay.Enabled = false; return; }
            if (inMonitor) return;
            inMonitor = true;
			try{
	            
	            string json = RunHttp("Home.ToRongBao");
	            JsonMessage<string[]> resultJson= Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<string[]>>(json);
	            if (!string.IsNullOrEmpty(resultJson.Message))
	            {
	                AppendSumLog("查询要结算数据失败：" + resultJson.Message);
	            }
	            else
	            {	               
					if (resultJson.Result!=null && resultJson.Result.Length ==3)
	                {
						AppendSumLog("发起结算");
						startModnitorank(resultJson.Result);
	                }
	                else
	                {
	                    //AppendSumLog("没有要结算的交易！");
	                }
	               
	            }
			}catch(Exception ex){
				onError ("发起结算失败：",ex,true);
			}
            inMonitor = false;
        }

        private void AppendSumLog(string str)
        {
            webBrowser1.Document.InvokeScript("appendSumLog", new string[] { DateTime.Now.ToString("HH:mm:ss") +":"+ str });
        }

		private void SetBankLog(string batNum,string str){
			webBrowser1.Document.InvokeScript("setBankStatus", new string[] {batNum, DateTime.Now.ToString("HH:mm:ss") +":"+ str });
		}

		private void onBankFinish(QueryResult result){			
			AppendSumLog (result.batchCurrnum + "更新结算标志");
			List<QueryItem> jsonQueryItems = new List<QueryItem> ();
			string nowTimeStr = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
			string[] fields = new string[] {
				"status",
				"results",
				"reciveDate"
			};
			try {
				if (result.batchStatus == 2) {
					//商户审核拒绝
					QueryItem jsonItem = new QueryItem () {
						table = "transactionSum",
						action = DBAction.Update,
						where = " batchCurrnum='" + result.batchCurrnum + "'",
						fields = fields,
						values = new string[]{ "-2", "商户审核拒绝", nowTimeStr }
					};
					jsonQueryItems.Add (jsonItem);
				} else {					
					if (result.batchEContent == null)
						throw new Exception ("获取明细为空");
					string logStr = "";
					foreach (QueryResult.DetailInfo detail in result.batchEContent) {
						QueryItem jsonItem = new QueryItem () {
							table = "transactionSum",
							action = DBAction.Update,
								where = " batchCurrnum='" + result.batchCurrnum + "'",//+ "' and id=" + detail.tradeNum,
							fields = fields,
							values = new string[3]
						};
						switch (detail.status) {
						case "":
						case "处理中":
							jsonItem.values [0] = "1";
							break;
						case "成功":
							jsonItem.values [0] = "3";
							break;
						case "失败":
							jsonItem.values [0] = "-2";
							break;
						default:
							throw new Exception ("处理结果明细有未定义的状态值");
						}
						logStr += detail.faren + detail.status + "; ";
						jsonItem.values [1] = detail.reason;
						jsonItem.values [2] = nowTimeStr;
						jsonQueryItems.Add (jsonItem);
					}
					SetBankLog (result.batchCurrnum, logStr);
				}
				string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject (jsonQueryItems);
				string resultStr = execDb (jsonStr);
				JsonMessage<string> jsonMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<string>> (resultStr);
				if (jsonMessage.Message != null)
					throw new Exception (jsonMessage.Message);
			} catch (Exception ex) {
				AppendSumLog (result.batchCurrnum + "更新结算标志异常:"+ ex.Message);
				onError ("更新结算标志异常:", ex);
			}
		}

		private void logOnThere(string str)
		{
			
		}


        public void MonitorBankResult(object obj)
        {
			if (systemExit) return;
			string[] arr = obj as string[];
			string batchCurrnum = arr [0];
			string batchDate = arr [1];
			string[] terminals = arr[2].ToString().Split();			 
            MyHttpUtility http = new MyHttpUtility();
			bool isFinish = false;
			try{
				string json =RunHttp("Home.GetRongBao",batchCurrnum,batchDate);
				JsonMessage<QueryResult> resultJson= Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<QueryResult>>(json);
				if (!string.IsNullOrEmpty (resultJson.Message)) {
					this.BeginInvoke (new delegateTwoParam (SetBankLog), batchCurrnum,"查询银行处理结果失败：" + resultJson.Message);
				} else {	   
					
					string statusText = "";
					switch (resultJson.Result.batchStatus) {
					case 0:
						statusText = "待确认";
						break;
					case 1:
						statusText = "待审核";
						break;
					case 2:
						statusText = "商户审核拒绝";
						isFinish = true;
						break;
					case 3:
						statusText = "处理中";
						break;
					case 4:
						isFinish = true;
						statusText = "交易完毕";
						break;
					}
					this.BeginInvoke (new delegateTwoParam (SetBankLog), batchCurrnum,"银行处理结果为：" + statusText);
					if (isFinish) {
						this.BeginInvoke (new QueryBankFinish(onBankFinish),resultJson.Result );
						this.BeginInvoke (new delegateOnParam (AppendSumLog), batchCurrnum+"监听完成");
						return;
					}
				}
			}catch(Exception ex){
				onError ("查询银行处理",ex,true);
			}
			if(!isFinish) {
				System.Threading.Thread.Sleep (30000);
				MonitorBankResult (obj);
			}
        }
	}


    public class Customer{
        public string terminal;
		public string lastQuery;
    }

}
