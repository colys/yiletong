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
		string connStr;
		EncryptionUtility encryption;

		public HomeController()
		{
			encryption = new EncryptionUtility(System.Web.HttpContext.Current.Server.MapPath("~/Content/yiletong.key"));
			connStr =getSetting("connstr");
		}
		private string getSetting(string name,bool isFile=false)
		{
			if (System.Configuration.ConfigurationManager.AppSettings[name] == null)
			{
				throw new Exception("配置文件不正确:"+name+"！");
			}
			string val= System.Configuration.ConfigurationManager.AppSettings[name].ToString();
			if (isFile) {
				if (val.IndexOf(":") != 1)
				{
					val = Server.MapPath(val);
				}
			}
			return val;
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

		public MySqlExecute CreateMysql(bool withTrans=false){
			return new MySqlExecute ("",connStr,withTrans);
		}

		public ActionResult DoLogin()
		{
			string inputUserName = Request["username"].Trim();
			string password = Request["password"].Trim();
			string sql = "select * from users where userName = '" + inputUserName + "'";
			using (MySqlExecute mysql = CreateMysql()) {
				DataTable dt = mysql.QueryTable (sql);
				if (dt.Rows.Count == 0) {
					ViewBag.Error = "用户不存在！";
					return View ("Login");
				} else {
					if (dt.Rows [0] ["password"].Equals (password)) {
						UserName = inputUserName;
						return RedirectToAction ("Index");
					} else {
						ViewBag.Error = "密码错误！";
						return View ("Login");
					}
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


		public string GetCustomers()
		{	
			try{
			string sql = "select * from customers where status = 1";
			using (MySqlExecute mysql = CreateMysql()) {
				mysql.QueryTable (sql);
				DataTable dt = mysql.QueryTable (sql);
				return JsonConvert.SerializeObject (dt);
			}
			}catch(Exception ex){
				LogError ("GetCustomers",ex);
				throw ex;
			}
		}

		public string GetTransactions(string startDate, string endDate, string termid, string cusName, string company)
		{
			if (string.IsNullOrEmpty(startDate)) startDate = DateTime.Today.ToString("yyyy-MM-dd");
			if (string.IsNullOrEmpty(endDate )) endDate = DateTime.Today.ToString("yyyy-MM-dd");
			string sql = "select a.*,b.shanghuName from transactionLogs a left join customers b on b.terminal=a.terminal where time between '" + Convert.ToDateTime(startDate).AddDays(-1).ToString("yyyy-MM-dd") + "23:00:00'" + " and '" + endDate.Replace("-", "") + " 23:59:59'";
			if (!string.IsNullOrEmpty(termid)) sql += " and b.terminal ='" + FormatString(termid) + "' ";
			if (!string.IsNullOrEmpty(cusName)) sql += " and b.faren ='" + FormatString(cusName) + "' ";
			using (MySqlExecute mysql =  CreateMysql()) {
				DataTable dt = mysql.QueryTable (sql);
				return JsonConvert.SerializeObject (dt);
			}
		}

	

		public JsonResult ToRongBao(){
			JsonMessage resultData = new JsonMessage ();
			string errorItem = "";
			System.Data.DataTable dt = null;
			int RandomNum;
			Random MyRandom = new Random ();
			RandomNum = MyRandom.Next (1001, 9999);
			string batchCurrnum = DateTime.Now.ToString ("yyyyMMddHHmmss") + RandomNum;
			try {
				string cerFile = getSetting ("rongbao_public", true);               
				errorItem = "查询结算汇总表";
				string sql = "select a.id,a.terminal,a.finallyMoney money,b.faren,b.shanghuName,b.bankName ,b.bankName2,b.bankName3,b.province,b.bankAccount,b.city,b.tel,b.sourceAccount from transactionSum a join customers b on b.terminal = a.terminal where a.status = 0 and b.status <> -1";//查询结算汇总表
				//注意：不要用事务，防止提交融宝成功后的异常又回滚
				using (MySqlExecute mysql = CreateMysql()) {
					dt = mysql.QueryTable (sql);
					if (dt.Rows.Count == 0)
						return Json (resultData, JsonRequestBehavior.AllowGet);
					if (dt.Columns.IndexOf ("id") == -1) {
						throw new Exception ("查询送盘数据失败，没有ID列");
					}
					errorItem = "更新待上传标记";                
					foreach (DataRow dr in dt.Rows) {
						if (dr ["id"].Equals (DBNull.Value)) {
							throw new Exception ("ID列数据为空");
						}
						if (dr ["terminal"].Equals (DBNull.Value)) {
							throw new Exception ("terminal列数据为空");
						}
						if (dr ["money"].Equals (DBNull.Value)) {
							throw new Exception ("money列数据为空");
						}
						/*
sumData.faren = customerInfo.faren;
                    sumData.shanghuName = customerInfo.shanghuName;
                    sumData.bankName = customerInfo.bankName;
                    sumData.bankName2 = customerInfo.bankName2;
                    sumData.bankName3 = customerInfo.bankName3;
                    sumData.province = customerInfo.province;
                    sumData.bankAccount = customerInfo.bankAccount;
                    sumData.city = customerInfo.city;
                    sumData.tel = customerInfo.tel;
					*/
						sql = @"update transactionSum set status= 1,results='正在结算',batchCurrnum='" + batchCurrnum + "'" +
						", uploadDate='" + DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss") + "'" +
						",faren='" + dr ["faren"] + "' " +
						",shanghuName='" + dr ["shanghuName"] + "' " +
						",bankName='" + dr ["bankName"] + "' " +
						",bankName2='" + dr ["bankName2"] + "' " +
						",bankName3='" + dr ["bankName3"] + "' " +
						",province='" + dr ["province"] + "' " +
						",city='" + dr ["city"] + "' " +
						",bankAccount='" + dr ["bankAccount"] + "' " +
						",sourceAccount='" + dr ["sourceAccount"] + "' " +
						",tel='" + dr ["tel"] + "' " +
						" where id='" + dr ["id"] + "'";
						mysql.ExecuteCommand (sql);//标记状态为待上传
					}
					errorItem = "上传融宝处理";
					string batchDate = DateTime.Now.ToString ("yyyyMMdd");
					Common.RongBao.RSACryptionClass testClass = new Common.RongBao.RSACryptionClass (cerFile, "");
					string returnStr = testClass.Sent (dt, batchCurrnum, batchDate);
					errorItem = "融宝执行成功,更新成功状态";
					string affectedIDStr = "";
					foreach (DataRow dr in dt.Rows) {
						affectedIDStr += "," + dr ["terminal"];
						sql = "update transactionSum set status= 2,results='银行处理中' where id='" + dr ["id"] + "'";
						mysql.ExecuteCommand (sql);
					}
				
					errorItem = null;
					resultData.Result = new string[]{ batchCurrnum, batchDate, affectedIDStr.Substring (1) };
				}
			} catch (Exception ex) {
				resultData.Message = errorItem + "时发生异常：" + ex.Message;
				LogError (errorItem,ex);
				if (dt != null) {
					try {
						using (MySqlExecute mysql = CreateMysql()) {
							foreach (DataRow dr in dt.Rows) {
								resultData.Message = resultData.Message.Replace ('\'', ' ');
								string sql = "update transactionSum set status= -2 ,results = '" + resultData.Message + "' where batchCurrnum='" + batchCurrnum + "' and id='" + dr ["id"] + "' and status= 1";
								mysql.ExecuteCommand (sql);
							}
						}
					} catch (MySqlException e) {
						resultData.Message += " , 更新失败标记也失败！" + e.Message;
						LogError ("更新失败标记",ex);
					}
				}
			}
			return Json (resultData, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 查询融宝执行情况
		/// </summary>
		/// <returns>The rong bao.</returns>
		/// <param name="batchCurrnum">Batch currnum.</param>
		/// <param name="batchDate">Batch date.</param>
		public JsonResult GetRongBao(string batchCurrnum,string batchDate ){
			JsonMessage resultData = new JsonMessage ();
			try {
				string publicCer = getSetting ("rongbao_public",true);     
				string cerFile = getSetting("rongbao_private",true);    
				Common.RongBao.RSACryptionClass testClass = new Common.RongBao.RSACryptionClass (publicCer, cerFile);
				resultData.Result = testClass.TryGetResult ( batchCurrnum, batchDate);
			} catch (Exception ex) {
				LogError ("GetRongBao",ex);
				resultData.Message = ex.Message;
			}
			return Json(resultData,JsonRequestBehavior.AllowGet);
		}

		private void LogError(string witch,Exception ex){
			log4net.ILog log = log4net.LogManager.GetLogger(this.GetType());
			log.Error(witch, ex);
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
				string dataArrStr = (arr1.Length > 1 && arr1[1].IndexOf('=')>0) ? arr1[1].Split('=')[1] : "";
				if(dataArrStr!=""){
					arr = dataArrStr.Split(',');
					for (int i = 0; i < arr.Length; i++)
					{
						arr[i] = HttpUtility.UrlDecode(arr[i]);
					}
				}
                var actionLastPos = actionName.LastIndexOf('.');
                string methodName ,className;
                if(actionLastPos>0){
                    methodName = actionName.Substring(actionLastPos+1);
                    className= actionName.Substring(0,actionLastPos);
                }else {
                    methodName= actionName;
                    className="Common.MySqlExecute";
                }
              
                object instance=null;
                switch(className.ToUpper()){
                    case "THIS":
                    case "HOME":
                         instance= this;
                        break;
                    case "MYSQLEXECUTE":
                    case "COMMON.MYSQLEXECUTE":
                        instance= new MySqlExecute("", connStr);
                        break;
                    default:
                        Type t = Type.GetType(className);
                        if(t==null) throw new Exception(className+"找不到！");
                        instance=System.Reflection.Assembly.GetAssembly(t).CreateInstance(className);
                        if (instance == null) throw new Exception(className + "找不到！");
                        break;
                }
				object evalResult = null;

                if (instance is IDisposable)
                {
                    using (instance as IDisposable)
                    {
						evalResult = instance.GetType().InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, instance, arr);
                    }
                }
                else
                {
					evalResult = instance.GetType().InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, instance, arr);
                }
                             
				if(evalResult is JsonResult){
					return JsonConvert.SerializeObject(((JsonResult)evalResult).Data );
				}else {
					jr.Result = evalResult;
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
				jr.LogException(ex.InnerException, "call "+ actionName+" with:\n" + errorMsgAppend.ToString());
				LogError ("eval",ex.InnerException);
			}
			return JsonConvert.SerializeObject(jr);
		}



		private string FormatString(string str)
		{
			return str.Replace("'", "");
		}
	}
}
