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

namespace WinForm
{

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class frmMain : Form
    {
        string userName="福州易乐通";
        string password = "Ylt123456";
        Queue<string> terminalQueue = new Queue<string>();
        string ActionUrl;
        EncryptionUtility encryption;
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
            ActionUrl = host + "Home/Eval";
            encryption = new EncryptionUtility(Application.StartupPath + "\\yiletong.key");
        }

        private string getNextTerminal()
        {
            if (terminalQueue.Count == 0) { FillQueue(); }
			if (terminalQueue.Count == 0)
				return null;
            return terminalQueue.Dequeue();
        }

      private void onError(string msg){
          MessageBox.Show(msg);
      }

        private void FillQueue()
        {
            string str= execQuery("customers", "terminal", "status = 1", null);
            JsonMessage<string[]> jm = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage<string[]>>(str);
            if (jm.Message!=null) { 
                onError(jm.Message);
            }
			foreach (string ter in jm.Result){
				terminalQueue.Enqueue (ter);
			}
//            terminalQueue.Enqueue("65886057");
//            terminalQueue.Enqueue("65886058");
//            terminalQueue.Enqueue("65886059");
//            terminalQueue.Enqueue("65886060");
//            terminalQueue.Enqueue("65886061");
//            terminalQueue.Enqueue("65886062");
        }

        private void 载入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
			openLogin ();
			tabControl1.SelectedIndex = 0;
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
			string js= "$('#resTree .tree-node[node-id=10051].trigger(\"click\")')";
			chromeWebBrowser1.ExecuteScript (js);
			waitQueryThread = new System.Threading.Thread (new System.Threading.ParameterizedThreadStart (watinFrameLoad));
			waitQueryThread.Start ();

		}

        System.Threading.Thread waitQueryThread;
        bool inQuery = false;

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
                setStatus(DateTime.Now.ToString("hhh:mm:ss")+ "：正在查询" + termID + "的刷卡情况");
                waitQueryThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(watinQuery));
                waitQueryThread.Start();
            }
			return true;
        }
        public void OnQueryFinish()
        {
            setStatus(DateTime.Now.ToString("hhh:mm:ss") + "：" + termID + "终端" + beginDate + "到" + endDate + "的刷卡情况:");
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
            if (terminalQueue.Count > 0) GoQuery();
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

        //private void setCommand(string sql){
        //    if(conn==null) OpenMysql();
        //    if(cmd==null) cmd= new MySqlCommand(sql,conn);
        //    else cmd.CommandText = sql;
        //}

        //private void ExecuteCommand(string sql){
        //    setCommand(sql);
        //    cmd.ExecuteNonQuery();
        //}

        //private DataTable QueryTable(string sql){
        //    setCommand(sql);
        //    System.Data.DataTable dt =new DataTable();
        //    MySqlDataAdapter da =new MySqlDataAdapter(cmd);
        //    da.Fill(dt);
        //    return dt;
        //}

        //private object QueryScalar(string sql){
        //    setCommand(sql);
        //  return  cmd.ExecuteScalar();
        //}

        public string GetNextVal(string table)
        {

            return RunHttp("GetNextVal", table);
           // return http.DoPost(ActionUrl, "action=" + encryption.EncryptData("GetNextVal") + "&dataArrStr=" + encryption.EncryptData(table));
            //if (table == null) throw new Exception("table parameter error");
            //table = GetTableName(table);
            //OpenMysql();

            //object nextVal = QueryScalar("select val from erp_sequence where tableName='" + table + "'");
            //if (nextVal == null || nextVal == DBNull.Value)
            //{
            //    nextVal = 1;
            //    ExecuteCommand("insert into erp_sequence (tableName,val) values('" + table + "','" + nextVal + "')");
            //}
            //else
            //{
            //    nextVal = Convert.ToInt32(nextVal) + 1;
            //    ExecuteCommand("update erp_sequence set val ='" + nextVal + "' where tableName=  '" + table + "'");
            //}

            //return nextVal.ToString();
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
            return http.DoPost(ActionUrl, encryption.EncryptData(content));
        }

        public string execQuery(string table,string fields,string where,string order){
            return RunHttp("ExecQuery", table, fields, where, order);
            //try{
            //    string sql = "select " + fields + " from " + GetTableName(table);
            //if (where != null && where.Length > 0) sql += " where " + where;
            //if (order != null && order.Length > 0) sql += " order by " + order;
            //DataTable dt= QueryTable(sql);
            //return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            //}catch(Exception ex){
            //    if (MessageBox.Show (ex.Message, "错误", MessageBoxButtons.RetryCancel) == DialogResult.Retry) {
            //        return execQuery (table, fields, where, order);
            //    } 
            //    return null;
            //}
            //if(conn.State== ConnectionState.Open) conn.Close ();
        }

        public string execDb(string jsonArrStr){
            return RunHttp("ExecDb", jsonArrStr);
            //try {
            //    QueryItem[] queryItems = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryItem[]> (jsonArrStr);
            //    if (queryItems == null)
            //        throw new Exception ("json error");
            //    foreach (QueryItem item in queryItems) {
            //        string sql;
            //        string table = GetTableName (item.table);
            //        switch (item.action) {
            //        case DBAction.Add:
            //            if (item.fields == null || item.fields.Length == 0)
            //                throw new Exception ("没有要插入的字段");
            //            sql = "insert into " + table + " (";
            //            for (int i = 0; i < item.fields.Length; i++)
            //                sql += item.fields [i] + ",";
            //            sql = sql.Substring (0, sql.Length - 1) + " ) values(";
            //            for (int i = 0; i < item.fields.Length; i++) {
            //                if (item.values [i] != null && item.values [i].ToUpper () != "NULL") {
            //                    sql += "'" + item.values [i].Replace ('\'', '\"') + "',";
            //                } else {
            //                    sql += "null,";
            //                }
            //            }
            //            sql = sql.Substring (0, sql.Length - 1) + ")";
            //            ExecuteCommand (sql);
            //            break;
            //        case DBAction.Update:
            //            if (item.fields == null || item.fields.Length == 0)
            //                throw new Exception ("没有要更新的字段");
            //            sql = "update " + table + " set ";
            //            for (int i = 0; i < item.fields.Length; i++) {                           
            //                if (item.values [i] != null && item.values [i].ToUpper () != "NULL") {
            //                    sql += item.fields [i] + " = '" + item.values [i].Replace ('\'', '\"') + "',";
            //                }
                           
            //            }
            //            sql = sql.Substring (0, sql.Length - 1) + " where " + item.where;
            //            ExecuteCommand (sql);
            //            break;
            //        case DBAction.Delete:
            //            sql = "delete from " + table + " where " + item.where;
            //            ExecuteCommand (sql);
            //            break;
            //        }
            //    }
            //} catch (Exception ex) {
            //    MessageBox.Show (ex.Message);
            //}
            //if(conn.State== ConnectionState.Open) conn.Close ();
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
            if (inQuery)
            {
                MessageBox.Show("正在查询，请稍候。。。");
                e.Cancel = true;
                return;
            }
            timer1.Enabled = false;
        }

	}

}
