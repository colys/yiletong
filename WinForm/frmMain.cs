using Sashulin;
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
using System.IO;
using ColysSharp.Modals;
using Newtonsoft.Json;
using System.Net;

namespace WinForm
{

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class frmMain : Form
    {
        string userName;
        string password;
        string posPlatForm = "xiandaijk";
		Queue<Customer> terminalQueue = new Queue<Customer>();
        string evalActionUrl;
        string controllerUrl;
        EncryptionUtility encryption;
		
		System.Threading.Thread queryThread;
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
            //settings.CachePath = @"C:\temp\caches";
            userName = getSetting("posusername");
            password = getSetting("pospassword");
			webBrowser1.Url = url;
            webBrowser1.ObjectForScripting = this;
            string host = getSetting("webHost");
            this.Text += "(" + host + ")";
            if (host[host.Length - 1] != '/') host += "/";
            controllerUrl = host + "Home";
            myBrowser.Initialize(new CSharpBrowserSettings() { DefaultUrl = controllerUrl + "/Customer" });
            myPage.Hide();
			timer1.Interval = 1000;            
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
        
        public void InitOK()
        {            
            
            string timeStr;
			listenInterval= Convert.ToInt32 (getSetting ("listenInterval"));
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
			JsonMessage<Dictionary<string,string>[]> jm = JsonConvert.DeserializeObject<JsonMessage<Dictionary<string,string>[]>>(qjson);
			if (jm.Message != null) {
				onError ("query transactionsum un recive data exception");
			} else {
				Dictionary<string,string>[] data = jm.Result;
				foreach (Dictionary<string,string> dic in data) {
					startModnitorank (new string[]{ dic ["batchCurrnum"], Convert.ToDateTime( dic ["uploadDate"]).ToString("yyyyMMdd"), "" });
				}
			}				
            try
            {
                EvalAction<string>("Home.RegisterClient", posPlatForm);
                registed = true;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                Close();
            }
            timer_pay.Start();
            timer1.Start();
        }

        bool registed = false;

        public T EvalAction<T>(string action, params string[] paramArr)
        {
            string qjson = RunHttp(action, paramArr);
            JsonMessage<T> jmRegister = JsonConvert.DeserializeObject<JsonMessage<T>>(qjson);
            if (jmRegister.Message != null) throw new Exception(jmRegister.Message);
            return jmRegister.Result;
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
			if (inThead )
				this.BeginInvoke (new delegateOneParam (setStatus),msg);
			else	setStatus (msg);

		}

        private void FillQueue()
        {
            string str = RunHttp("Home.GetCustomers", null);            
			//string str= execQuery("customers", "terminal,lastQuery", "status = 1", null);
            JsonMessage<Customer[]> jm = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<Customer[]>>(str);
			if (jm.Message != null) { 
				throw new Exception (jm.Message);
			} else {
				if (jm.Result == null)
					throw new Exception ("查询客户时返回空结果，可能是网络问题");
				foreach (Customer ter in jm.Result) {
                    if (string.IsNullOrEmpty(ter.lastQuery)) ter.lastQuery = DateTime.Today.ToString("yyyy-MM-dd");
                    terminalQueue.Enqueue(ter);
				}
			}
        }
       


        private void 触发查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {         
			if (inQuery) return;            
			GoQuery (null);
            timer1.Start();            
            触发查询ToolStripMenuItem.Enabled = false;
			this.tabControl1.SelectedIndex = 0;
        }
        string termID,beginDate,endDate;
        
		private void GoQuery(object o)
        {
			if (inQuery || systemExit)	return;
			if (queryThread != null && queryThread.IsAlive) return;            
            try
            {                
                inQuery = true;   
                Customer cus = getNextTerminal();
                if (cus == null) throw new Exception("get next terminal is null , may be server error");
                termID = cus.terminal;
                DateTime dtLastQuery;
                if (string.IsNullOrEmpty(cus.lastQuery))
                {
                    if (DateTime.Now.Hour < 10) { dtLastQuery = DateTime.Today.AddDays(-1); }
                    else dtLastQuery = DateTime.Today;
                }
                else dtLastQuery = Convert.ToDateTime(cus.lastQuery);
                beginDate = dtLastQuery.ToString("yyyy/MM/dd");
                if (DateTime.Now.Hour > 22)
                    endDate = DateTime.Today.AddDays(1).ToString("yyyy/MM/dd");
                else
                    endDate = DateTime.Today.ToString("yyyy/MM/dd");
                setStatus("正在查询" + termID + "的刷卡情况");
                waitQueryTime = DateTime.Now;
                queryThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(queryJingKong));
                queryThread.Start();
            }
            catch (Exception ex)
            {
                inQuery = false;
                timer1.Enabled = false;
                onError(null, ex);
            }
        }

        private void queryJingKong(object o){
            int tryCount = 10;
            while (true)
            {                
                try
                {
                    Random MyRandom = new Random();
                    int RandomNum = MyRandom.Next(1001, 9999);
                    string json = JingKongHttp.DoPostHttps("https://119.4.99.217:7300/mcrm/bca/txnlog_findBy", "beginstdate=" + beginDate + "&endstdate=" + endDate + "&branchId=65023903&refno=&mid=&tid=" + termID + "&midName=&transid=&rspcode=&mgrid=&rsp=&lpName=&rows=200&page=1&shuijishu=" + RandomNum, Application.StartupPath, "https://119.4.99.217:7300/mcrm/jsp/bca/bcatxnlog.jsp");
                    this.BeginInvoke(new delegateOneParam(OnQueryFinish), json);
                    break;
                }
                catch (System.Net.WebException)
                {
                    tryCount--;
                    if (tryCount < 0) break;
                    System.Threading.Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new delegateOneParam(OnQueryFinish), "");
                    onError("queryJingKong", ex, true);
                    break;
                }                
            }
        }
		 
		private bool loginOutime = true;

        public void OnQueryFinish(string json)
        {
            try
            {
                if (json == "")
                {
                    inQuery = false;
                    setStatus(termID + "终端查询失败");
                }
                else
                {
                    setStatus(termID + "终端" + beginDate + "到" + endDate + "的刷卡情况:");
                    if (json == "\"{\\\"flag\\\":\\\"login\\\",\\\"info\\\":\\\"    \\\",\\\"key\\\":null,\\\"mssiMsg\\\":\\\"成功\\\"}\"")
                    {
                        loginOutime = true;
                    }
                    else if (json.Length > 50 && json.Substring(0, 50).ToUpper().IndexOf("<!DOCTYPE") > -1)
                    {
                        loginOutime = true;
                    }
                    else if (json == "\"This session has been expired (possibly due to multiple concurrent logins being attempted as the same user).\"")
                    {
                        loginOutime = true;
                    }
                    else
                    {

                        try
                        {
                            string str = RunHttp("Home.SyncJingKongPosData", termID, json);
                            JsonMessage<TransactionLog[]> resultJson = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<TransactionLog[]>>(str);
                            if (resultJson.Message != null) { throw new Exception(resultJson.Message); }
                            if (resultJson.Result.Length == 0)
                            {
                                setStatuDetal("没有刷卡数据!");
                            }
                            else
                            {
                                foreach (TransactionLog log in resultJson.Result)
                                {
                                    setStatuDetal(log.tradeName + "," + log.time + "," + log.tradeMoney);
                                    addTerminalView(Newtonsoft.Json.JsonConvert.SerializeObject(log));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            onError("SyncJingKongPosData exception:", ex);
                        }
                    }
                }
                inQuery = false;
                if (systemExit || loginOutime)
                    return;
                if (terminalQueue.Count == 0)
                {
                    timer1.Interval = listenInterval;
                }
                else
                    timer1.Interval = 1000;
            }
            catch (Exception ex)
            {
                inQuery = false;
                onError("OnQueryFinish", ex);
            }
        }


        private void setStatus(string str)
        {
            object[] objects = new object[1];
            objects[0] = DateTime.Now.ToString("HH:mm:ss") + "：" + str;
            webBrowser1.Document.InvokeScript("setStatus", objects);
        }
        private void setStatuDetal(string str)
        {
            object[] objects = new object[1];
            objects[0] = str;
            webBrowser1.Document.InvokeScript("setStatusDetail", objects);
        }
        private void addTerminalView(string str)
        {
            object[] objects = new object[1];
            objects[0] = str;
            webBrowser1.Document.InvokeScript("addTerminalView", objects);
        }
	

		public delegate void delegateTwoParam(string str1,string str2);
		public delegate void delegateOneParam(string str);
		public delegate void delegateNoParam();		
		public delegate void QueryBankFinish(QueryResult result);




        //public void queryJsReturn(string str){
        //    if (str != "ok") {
        //        log4net.ILog log = log4net.LogManager.GetLogger(this.GetType());
        //        log.Error ("query ajax return error:" + str);
        //        this.BeginInvoke(new delegateOneParam(OnQueryFinish), false);
        //    } else {
        //        //Console.WriteLine (str);
        //        this.BeginInvoke(new delegateOneParam(OnQueryFinish), true);
        //    }
        //}

		DateTime waitQueryTime;

//        private void watinQuery(object o)
//		{
//			try {
//				while (true && !systemExit) {
//					System.Threading.Thread.Sleep (500);
//					object val = chromeWebBrowser1.EvaluateScript ("$('#hidden_json').val()");
//					if (val != null && !val.Equals (string.Empty)) {				   
//						this.BeginInvoke (new QueryFinish (OnQueryFinish),true);
//						while (inQuery) {
//							System.Threading.Thread.Sleep (100);
//						}
//						return;
//					} else {
//						System.Threading.Thread.Sleep (300);
//					}
//					if((DateTime.Now - waitQueryTime).TotalMinutes > 1){
//						throw new Exception("ajax query timeout,may be web is login");
//					}
//				}
//			} catch (System.Threading.ThreadAbortException) {
//				inQuery = false;
//			}
//			catch(Exception ex){
//				this.BeginInvoke (new QueryFinish (OnQueryFinish),false);
//				onError ("watin Query", ex, true);
//			}
        //		}


		string cookieStr = null;
       

       



        private void timer1_Tick_1(object sender, EventArgs e)
        {						
            if (loginOutime)
            {				
				timer1.Enabled = false;
				触发查询ToolStripMenuItem.Enabled = true;				
                UploadVerifyCode();				
			}else GoQuery (null);

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

       
        public string execQuery(string table,string fields,string where,string order){
            return RunHttp("ExecQuery", table, fields, where, order);
        
        }

        public string execDb(string jsonArrStr){
            return RunHttp("ExecDb", jsonArrStr);        
		}
	

        #endregion

        MyHttpUtility webHttp = new MyHttpUtility();
        private string RunHttp(string action,params string[] paraStrArr)
		{			
			string content = "action=" + action + "&dataArrStr=";           
			if (paraStrArr != null) {
				foreach (string str in paraStrArr) {
					content += System.Web.HttpUtility.UrlEncode (str) + ",";
				}
				content = content.Substring (0, content.Length - 1);
			}
			try {
				return webHttp.DoPost (evalActionUrl, encryption.EncryptData (content));
			} catch (Exception ex) {
				onError ("DoPost exception:"+ content  +" ,", ex);
				return null;
			}
		}
      

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("您真的要退出吗？", "提示", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) e.Cancel = true;
            systemExit = true;			
			tabControl1.SelectedIndex = 0;
			if (queryThread != null && queryThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
				queryThread.Abort ();
            if (moniterLoginThread != null && moniterLoginThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                moniterLoginThread.Abort();            
			foreach (KeyValuePair<string,System.Threading.Thread> kv in queryBankTherads) {
				if (kv.Value != null && kv.Value.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
					kv.Value.Abort ();
			}
			System.Threading.Thread.Sleep (100);
			while (inQuery || inMonitor)
            {                
				System.Threading.Thread.Sleep (500);
            }			
            timer_pay.Enabled = false;
            timer1.Enabled = false;
            if (registed) RunHttp("Home.RegisterLogoff", posPlatForm);//加上判断，防止因为已有监控自动关闭，又logoff掉其它的
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
			if (systemExit || inMonitor || loginOutime) { return; }            
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
			while (!(isFinish ||systemExit)) {
				try {
					string json = RunHttp ("Home.GetRongBao", batchCurrnum, batchDate);
					JsonMessage<QueryResult> resultJson = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<QueryResult>> (json);
					if (!string.IsNullOrEmpty (resultJson.Message)) {
						this.BeginInvoke (new delegateTwoParam (SetBankLog), batchCurrnum, "查询银行处理结果异常：" + resultJson.Message);
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
						this.BeginInvoke (new delegateTwoParam (SetBankLog), batchCurrnum, "银行处理结果为：" + statusText);
						
					}
                    if (isFinish)
                    {
                        this.BeginInvoke(new QueryBankFinish(onBankFinish), resultJson.Result);
                        this.BeginInvoke(new delegateOneParam(AppendSumLog), batchCurrnum + "监听完成");
                        return;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(150000);
                    }
				} catch (Exception ex) {
					if(!systemExit)	onError ("查询银行处理", ex, true);
				}		 

			}
        }

        private void myBrowser_PageLoadFinishEventhandler(object sender, EventArgs e)
        {
            if (myBrowser.Url.ToLower().IndexOf("login") > -1) {
                myBrowser.ExecuteScript("$('#username').val('" + getSetting("webusername") + "')");
                myBrowser.ExecuteScript("$('#password').val('" + getSetting("webpassword") + "')");
                myBrowser.ExecuteScript("$('button[type=submit]').click()");
            }
        }
        
        System.Threading.Thread moniterLoginThread;

        #region 登录相关
        MyHttpUtility JingKongHttp = new MyHttpUtility();
        string loginUrl = "https://119.4.99.217:7300/mcrm/login.jsp";
        //上传验证码
        public void UploadVerifyCode() {
            if(moniterLoginThread!=null && moniterLoginThread.IsAlive) return;
            try
            {

                JingKongHttp.DoGet(loginUrl);
                 Random MyRandom = new Random();
                int RandomNum = MyRandom.Next(100000000, 999999999);
                string url = "https://119.4.99.217:7300/mcrm/code/code?" + RandomNum;
                string filePath = Application.StartupPath + "\\"+posPlatForm+".jpg";
                JingKongHttp.Download(url, filePath, Application.StartupPath, "https://119.4.99.217:7300/mcrm/j_spring_security_check");
                
                //上传
                url = controllerUrl + "/UploadForReLogin?platform="+posPlatForm;
                WebClient webClient = new WebClient();
                byte[] resultByte= webClient.UploadFile(url, filePath);
                string errorMsg= Encoding.UTF8.GetString(resultByte);
                if (!string.IsNullOrEmpty(errorMsg)) throw new Exception(errorMsg);
                //切到web
                tabControl1.SelectedIndex = 1;
                myBrowser.Reload();//刷新，输入验证码
                moniterLoginThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(MoniterVerifyCodeIsSet));
                moniterLoginThread.Start();
            }
            catch (Exception ex) {
                onError("重新登录", ex);
            }
        }

        //拿到验证码后，执行登录
        public void DoRelogin(string verifyCode) {
            string data = "j_username=" + userName + "&j_password=" + password + "&codeVal=" + verifyCode;
            
            string response = JingKongHttp.DoPostHttps("https://119.4.99.217:7300/mcrm/j_spring_security_check", data, Application.StartupPath,"https://119.4.99.217:7300/mcrm/login.jsp");
            if (response.IndexOf("<div class=\"alert alert-danger\" style=\"margin: 0px;\">验证码错误</div>") > -1)
            {
                //MessageBox.Show("重登录失败，验证码" + verifyCode + "错误");
                UploadVerifyCode();
            }
            else
            {
                //if (response.IndexOf("您的密码已经过期，请修改您的初始密码") > -1) {
                //    MessageBox.Show("金控密码已经过期");
                //    this.Close();
                //    return;
                //}
                loginOutime = false;
                EvalAction<string>("Home.RegisterClient", posPlatForm);
                registed = true;
                //继续监控
                触发查询ToolStripMenuItem_Click(null, null);
            }
        }

        public void MoniterVerifyCodeIsSet(object o) {
            while (true)
            {
                try
                {
                    PosPlatformClient client = EvalAction<PosPlatformClient>("Home.IsVerifyCodeSet", posPlatForm);
                    if (client == null) System.Threading.Thread.Sleep(3000);
                    else
                    {
                        this.BeginInvoke(new delegateOneParam(DoRelogin), client.verifyCode);
                        break;
                    }
                }
                catch (System.Threading.ThreadAbortException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    onError("获取web输入的验证码", ex, true);
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }
        #endregion
    }


    public class Customer{
        public string terminal;
		public string lastQuery;
    }

}
