using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Common;
using System.Text;

namespace web2.Controllers
{
	public class HomeController : Controller
	{
		static string  connStr =getSetting("connstr");
		EncryptionUtility encryption;

		public HomeController()
		{
			encryption = new EncryptionUtility(System.Web.HttpContext.Current.Server.MapPath("~/Content/yiletong.key"));
		}
		private static string getSetting(string name)
		{
			if (System.Configuration.ConfigurationManager.AppSettings[name] == null)
			{
				throw new Exception("配置文件不正确:"+name+"！");
			}
			return System.Configuration.ConfigurationManager.AppSettings[name].ToString();
		}

		private string UserName {
			get
			{
				if (Session["userName"] == null) return null;
				ViewBag.UserName = Session["userName"].ToString();
				return ViewBag.UserName;
			}
			set
			{
				Session["userName"] = value;
			}
		}

		public ActionResult Index()
		{
			ViewBag.UserName = UserName;
			return View();
		}

		public ActionResult Login()
		{            
			return View();
		}

		public ActionResult DoLogin()
		{
			string inputUserName = Request["username"].Trim();
			string password = Request["password"].Trim();
			string sql = "select * from users where userName = '" + inputUserName + "'";
			DataTable dt = QueryTable(sql,true);
			if (dt.Rows.Count == 0)
			{
				ViewBag.Error = "用户不存在！";
				return View("Login");
			}
			else
			{
				if (dt.Rows[0]["password"].Equals(password))
				{
					UserName = inputUserName;
					return RedirectToAction("Index");
				}
				else
				{
					ViewBag.Error = "密码错误！";
					return View("Login");
				}
			}

		}

		public ActionResult Customer()
		{
			if (UserName == null)
			{
				return RedirectToAction("Login");
			}
			return View();

		}

		public ActionResult Transaction()
		{
			if (UserName == null)
			{
				return RedirectToAction("Login");
			}
			return View();
		}



		MySqlConnection conn;
		MySqlCommand cmd;


		private void OpenMysql()
		{   
			if (conn == null) conn = new MySqlConnection(connStr);
			if (conn.State == ConnectionState.Closed) conn.Open();
		}


		private void setCommand(string sql)
		{
			if (conn == null) OpenMysql();
			if (cmd == null) cmd = new MySqlCommand(sql, conn);
			else cmd.CommandText = sql;
		}

		private void ExecuteCommand(string sql)
		{
			setCommand(sql);
			cmd.ExecuteNonQuery();
		}

		private DataTable QueryTable(string sql,bool close)
		{
			setCommand(sql);
			System.Data.DataTable dt = new DataTable();
			MySqlDataAdapter da = new MySqlDataAdapter(cmd);
			da.Fill(dt);
			if (close) conn.Close();
			return dt;
		}

		private object QueryScalar(string sql)
		{
			setCommand(sql);
			return cmd.ExecuteScalar();
		}


		public string GetCustomers()
		{
			string sql = "select * from customers where status = 1";
			DataTable dt= QueryTable(sql, true);
			return JsonConvert.SerializeObject (dt);
		}

		public string GetTransactions(string startDate, string endDate, string termid, string cusName, string company)
		{
			if (string.IsNullOrEmpty(startDate)) startDate = DateTime.Today.ToString("yyyy-MM-dd");
			if (string.IsNullOrEmpty(endDate )) endDate = DateTime.Today.ToString("yyyy-MM-dd");
			string sql = "select a.*,b.shanghuName from transactionLogs a left join customers b on b.terminal=a.terminal where time between '" + Convert.ToDateTime(startDate).AddDays(-1).ToString("yyyy-MM-dd") + "23:00:00'" + " and '" + endDate.Replace("-", "") + " 23:59:59'";
			if (!string.IsNullOrEmpty(termid)) sql += " and b.terminal ='" + FormatString(termid) + "' ";
			if (!string.IsNullOrEmpty(cusName)) sql += " and b.faren ='" + FormatString(cusName) + "' ";
			DataTable dt = QueryTable(sql, true);
			return JsonConvert.SerializeObject(dt);
		}


		public JsonResult ToRongBao(){
			var resultData = new { ErrorCode = 0, ErrorMsg = "" };
			string errorItem = "";
			System.Data.DataTable dt= null;
			try
			{
				string cerFile = getSetting("publicCer");
				if (cerFile.IndexOf(":") != 1) { 
					//相对路径
					if (cerFile[0] != '\\' && cerFile[0] != '/') { cerFile = "\\" + cerFile.Replace('/', '\\'); }
					cerFile = Server.MapPath(cerFile);
				}
				errorItem="查询结算汇总表";
				resultData.ErrorCode = 1;
				string sql="";//查询结算汇总表
				dt =QueryTable(sql,false);
				if(dt.Columns.IndexOf("id")==-1){throw new Exception("查询送盘数据失败，没有ID列");}
				errorItem="更新待上传标记";
				resultData.ErrorCode = 1;
				foreach(DataRow dr in dt.Rows){
					if(dr["id"].Equals(DBNull.Value)){throw new Exception("ID列数据为空");}
					if(dr["terminal"].Equals(DBNull.Value)){throw new Exception("terminal列数据为空");}
					if(dr["tradeMoney"].Equals(DBNull.Value)){throw new Exception("tradeMoney列数据为空");}
					sql="update transactionSum set status= 1,result='正在结算',uploadDate='"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +"' result where id="+ dr["id"];
					ExecuteCommand(sql);//标记状态为待上传
				}
				errorItem="上传融宝处理";
				resultData.ErrorCode = 1;
				Common.RongBao.RSACryptionClass testClass = new Common.RongBao.RSACryptionClass(cerFile);
				string returnStr = testClass.Test(dt);
				errorItem="融宝执行成功,更新成功状态";
				resultData.ErrorCode =3;
				foreach(DataRow dr in dt.Rows){					
					sql="update transactionSum set status= 2,result='成功' where id="+ dr["id"];
					ExecuteCommand(sql);
				}	
				errorItem=null;
				resultData.ErrorCode = 0;
			}
			catch (Exception ex) {
				resultData.ErrorMsg = errorItem + "时发生异常：" + ex.Message;
				if (dt != null) {
					foreach (DataRow dr in dt.Rows) {					
						string sql = "update transactionSum set status= -2 ,result = '" + resultData.ErrorMsg + "' where id=" + dr ["id"] + " and status= 1";	
						ExecuteCommand (sql);
					}
				}
			}
			catch(MySqlException ex){
				resultData.ErrorMsg+=" , 更新失败标记也失败！";	
			}
			return Json(resultData,JsonRequestBehavior.AllowGet);
		}



		/// <summary>
		/// 执行服务器内容
		/// </summary>     
		public string Eval()
		{
			JsonMessage jr = new JsonMessage();
			byte[] bs = Request.BinaryRead(Request.ContentLength);
			string data ;
			string[] arr =null;
			string actionName = "";
			try
			{
				data = encryption.DecryptData(bs);
				string[] arr1 = data.Split('&');

				actionName = arr1[0].Split('=')[1];
				string dataArrStr = arr1.Length > 0 ? arr1[1].Split('=')[1] : "";
				arr = dataArrStr.Split(',');
				for (int i = 0; i < arr.Length; i++)
				{
					arr[i] = HttpUtility.UrlDecode(arr[i]);
				}
				using( MySqlExecute mysql = new MySqlExecute("", connStr)){
					jr.Result = mysql.GetType().InvokeMember(actionName, System.Reflection.BindingFlags.InvokeMethod, null, mysql, arr);
				}
			}
			catch (Exception ex)
			{				
				StringBuilder errorMsgAppend = new StringBuilder();
				if (arr != null) {
					foreach (string val in arr) {
						errorMsgAppend.AppendLine (val);
					}
				}
				jr.LogException(ex, "call "+ actionName+" with:\n" + errorMsgAppend.ToString());
			}
			return JsonConvert.SerializeObject(jr);
		}



		private string FormatString(string str)
		{
			return str.Replace("'", "");
		}
	}
}
