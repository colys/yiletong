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
using ColysSharp.DataBase;
using ColysSharp.Modals;
using System.Net.Mail;

namespace web2.Controllers
{	
	public class HomeController : ColysSharp.Mvc.BaseController
	{
		//string connStr;
		EncryptionUtility encryption;


        public JsonResult ReloadConfig()
        {
            JsonMessage jm = new JsonMessage();
            string filePath = Server.MapPath("~/Content/QueryConfig.json");
            base.ReloadConfig(filePath);
            jm.Message= "reload 成功";
            return Json(jm, JsonRequestBehavior.AllowGet);
        }

		public HomeController()
		{
			encryption = new EncryptionUtility("TfoPqado2GvjxvC1GsmY6Q==");
			//connStr =getSetting("connstr");
		}
        string getSetting(string name, bool isFile = false) {
            return getSetting(System.Web.HttpContext.Current.Server , name, isFile);
        }
		public static string getSetting(HttpServerUtility app, string name,bool isFile=false)
		{
			if (System.Configuration.ConfigurationManager.AppSettings[name] == null)
			{
				throw new Exception("配置文件不正确:"+name+"！");
			}
			string val= System.Configuration.ConfigurationManager.AppSettings[name].ToString();
			if (isFile) {
				if (val.IndexOf(":") != 1)
				{
					val = app.MapPath(val);
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

		public ActionResult Login(string backurl)
		{
            ViewBag.backUrl = backurl;
			return View();
		}

        //public MySqlExecute CreateMysql(bool withTrans=false){
        //    return new MySqlExecute ("",connStr,withTrans);
        //}

		public ActionResult DoLogin()
		{
			string inputUserName = Request["username"].Trim();
			string password = Request["password"].Trim();
            string backUrl = Request["backurl"];
			string sql = "select * from users where userName = '" + inputUserName + "'";
            using (DBContext mysql = new DBContext())
            {
				DataTable dt = mysql.QueryTable (sql);
				if (dt.Rows.Count == 0) {
					ViewBag.Error = "用户不存在！";
					return View ("Login");
				} else {
					if (dt.Rows [0] ["password"].Equals (password)) {
						UserName = inputUserName;
                        if (string.IsNullOrEmpty(backUrl)) 
						    return RedirectToAction ("Index");
                        else
                            return RedirectToAction (backUrl);
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
                return RedirectToAction("Login", new { backurl = "Customer" });
			}
			return View();

		}

		public ActionResult Transaction()
		{
			if (UserName == null)
			{
                return RedirectToAction("Login", new { backurl = "Transaction" });
			}
			return View();
		}

		public ActionResult TransactionSum(){
			if (UserName == null)
			{
                return RedirectToAction("Login", new { backurl = "TransactionSum"});
			}
			return View();
		}

        public ActionResult MonthCalc() {
            return View();
        }

		public ActionResult SourceAccount(){
			if (UserName == null)
			{
				return RedirectToAction("Login");
			}
			return View();
		}


		public System.Data.DataTable GetCustomers()
		{	
			try{
                string sql = "select * from customers where status > -1 and frozen <> 1";
                using (DBContext mysql = new DBContext())
                {
                    return mysql.QueryTable(sql);
                }
			}catch(Exception ex){
				LogError ("GetCustomers",ex);
				throw ex;
			}
		}

        //public string GetTransactions(string startDate, string endDate, string termid, string cusName, string company)
        //{
        //    if (string.IsNullOrEmpty(startDate)) startDate = DateTime.Today.ToString("yyyy-MM-dd");
        //    if (string.IsNullOrEmpty(endDate )) endDate = DateTime.Today.ToString("yyyy-MM-dd");
        //    string sql = "select a.*,b.shanghuName from transactionLogs a left join customers b on b.terminal=a.terminal where time between '" + Convert.ToDateTime(startDate).AddDays(-1).ToString("yyyy-MM-dd") + "23:00:00'" + " and '" + endDate.Replace("-", "") + " 23:59:59'";
        //    if (!string.IsNullOrEmpty(termid)) sql += " and b.terminal ='" + FormatString(termid) + "' ";
        //    if (!string.IsNullOrEmpty(cusName)) sql += " and b.faren ='" + FormatString(cusName) + "' ";
        //    using (DBContext mysql = CreateMysql())
        //    {
        //        DataTable dt = mysql.QueryTable (sql);
        //        return JsonConvert.SerializeObject (dt);
        //    }
        //}
        bool isHoldDay = false;
        string lastTransDay;
        bool inUploadRongbao = false;
        static DateTime lastSyncTime=DateTime.Today;

        public JsonMessage SyncJingKongPosData(string terminal, string json)
        {
            if (DateTime.Now.Day != lastSyncTime.Day) {
                //每天同步一次，一次同步一个月
                SyncHolday();
            }
            lastSyncTime = DateTime.Now;
            while (inUploadRongbao) {
                System.Threading.Thread.Sleep(500);
            }
            if (clients.ContainsKey("xiandaijk")) clients["xiandaijk"].lastAccess = DateTime.Now;
            else { 
            //iis reset register again
                RegisterClient("xiandaijk");
            }
            DBContext db = new DBContext(true);
            JsonMessage resultData = new JsonMessage();
            DateTime maxTime = DateTime.MinValue;
            List<TransactionLog> newItemList = new List<TransactionLog>();
            System.Web.HttpContext.Current.Application.Lock();
            try
            {
                LogStep("获取客户信息");
                Customer customerInfo = db.QuerySingle<Customer>(new { terminal = terminal, status = ">-1 " });
                //SourceAccount customerSource = db.QuerySingle<SourceAccount>(customerInfo.sourceAccount.Value);
                if (customerInfo == null) { throw new Exception("获取客户信息is null, terminal：" + terminal); }
                if (customerInfo.discount == null) { throw new Exception("获取客户信息discount is null or NaN,terminal：" + terminal); }
                if (customerInfo.tixianfei == null) { throw new Exception("获取客户信息tixianfei is null or NaN, terminal：" + terminal); }
                if (customerInfo.tixianfeiEles == null) { throw new Exception("获取客户信息tixianfeiEles is null or NaN, terminal：" + terminal); }
                stepName = "解析json : \r\n" + json;
                JingKongResult jkResult = JsonConvert.DeserializeObject<JingKongResult>(json);
                if (jkResult == null) throw new Exception("序列化出错：" + json);
                TransactionLog[] lst = jkResult.ToTransactionArray();
                foreach (TransactionLog localItem in lst)
                {
                    //判断记录是否存在,存在的话忽略,不存在则插入
                    var whereSql = new { terminal = terminal, timeStr = localItem.timeStr, tradeName = localItem.tradeName, tradeMoney = localItem.tradeMoney };
                    stepName = localItem.time + " " + localItem.tradeName + "是否已经保存过";
                    if (db.Exists("", "TransactionLog", whereSql)) continue;
                    LogStep("分析时间，23点后算次日");
                    string tempTime;
                    DateTime datetime = Convert.ToDateTime(localItem.time);
                    if (datetime > maxTime) maxTime = datetime;
                    //tomorrw is holdday
                    if (datetime.Hour > 22) datetime = datetime.AddDays(2);
                    else datetime = datetime.AddDays(1);
                    tempTime = datetime.ToString("yyyy-MM-dd");
                    if (tempTime != lastTransDay)
                    {
                        LogStep("获取是否节假日");
                        lastTransDay = tempTime;
                        GetLocalHolday(tempTime, db);
                        //MyHttpUtility http = new MyHttpUtility();
                        //int tryCount = 10;
                        //while (true)
                        //{
                        //    try
                        //    {
                        //        string timeJson = http.DoGet("http://www.easybots.cn/api/holiday.php?d=" + tempTime);
                        //        if (string.IsNullOrEmpty(timeJson)) throw new Exception("节假日返回值为空");
                        //        string[] tempArr = timeJson.Substring(1, timeJson.Length - 2).Split(':');
                        //        if (tempArr.Length != 2) throw new Exception("节假日返回值有变动");
                        //        isHoldDay = Convert.ToInt32(tempArr[1].Replace('\"', ' ').Trim()) > 0;
                        //        break;
                        //    }
                        //    catch (System.Net.WebException ex)
                        //    {
                        //        tryCount--;
                        //        if (tryCount < 0) { GetLocalHolday(tempTime, db); break; }
                        //        LogError("使用网络节假日发生网络异常，重试"+ tryCount, ex);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        LogStep("easybots节假日返回异常(" + ex.Message + ")，尝试读取本地设置");
                                
                        //    }
                        //    break;
                        //}
                    }
                    LogStep("计算手续费");
                    calcMoney(customerInfo, localItem);
                    LogStep("保存交易记录");
                    localItem.status = 0;
                    db.DoSave("", localItem);
                    newItemList.Add(localItem);
                }
                LogStep("查询是否有结算交易");
                string sql = "select time,id from TransactionLogs where terminal='" + terminal + "' and tradeName in ('批上送结束(平账)','批上送结束(不平账)') and resultCode='00' and status =0";
                DataTable dt = db.QueryTable(sql);
                if (dt.Rows.Count > 0)
                {
                    LogStep("有" + dt.Rows.Count + "笔结算数据，开始结算");
                    foreach (DataRow dr in dt.Rows)
                    {
                        string time = dr["time"].ToString();
                        string sumLogId = dr["id"].ToString();
                        sql = "select max(time) prevTime from TransactionLogs where terminal='" + terminal + "' and Status=0 and tradeName in ('批上送结束(平账)','批上送结束(不平账)') and resultCode='00' and time < '" + time + "'";
                        object o = db.ExecScalar(sql);
                        string prevTime = o.ToString();

                        //sum log of (消费)
                        sql = "select count(0) batchCount,sum(tradeMoney) tradeMoney,sum(discountMoney) discountMoney,sum(tixianfeiMoney) tixianfeiMoney from transactionLogs ";
                        string publicWhere = " where terminal='" + terminal + "' and Status=0 and isValid = 1 and resultCode='00'";
                        if (string.IsNullOrEmpty(prevTime))
                            publicWhere += " and time < '" + time + "'";
                        else
                            publicWhere += " and time between '" + prevTime + "' and '" + time + "'";
                        sql += publicWhere;
                        DataTable dtSum = db.QueryTable(sql);
                        if (dtSum.Rows.Count == 0 || dtSum.Rows[0]["batchCount"].Equals(0) || dtSum.Rows[0]["tradeMoney"] == DBNull.Value) continue;
                        TransactionSum sumData = new TransactionSum();
                        sumData.shanghuName = customerInfo.shanghuName;
                        sumData.faren = customerInfo.faren;
                        sumData.tradeMoney = Convert.ToDouble(dtSum.Rows[0]["tradeMoney"]);
                        sumData.discountMoney = Convert.ToDouble(dtSum.Rows[0]["discountMoney"]);
                        sumData.tixianfeiMoney = Convert.ToDouble(dtSum.Rows[0]["tixianfeiMoney"]);
                        sumData.finallyMoney = Convert.ToDouble(sumData.tradeMoney) - Convert.ToDouble(sumData.discountMoney) - Convert.ToDouble(sumData.tixianfeiMoney);
                        sumData.status = 0;
                        sumData.batchCount = Convert.ToInt32(dtSum.Rows[0]["batchCount"]);
                        sumData.createDate = time;//(new Date()).Format("yyyy-MM-dd hh:mm:ss")；用结算记录的时间
                        sumData.terminal = terminal;
                        db.DoSave("", sumData);//保存结算数据
                        //更新交易记录的标记
                        sql = "update TransactionLogs set sumid=" + sumData.id + " , status =1 " + publicWhere;
                        db.ExecuteCommand(sql);
                        sql = "update TransactionLogs set sumid=" + sumData.id + " , status =1 where id = " + sumLogId;
                        db.ExecuteCommand(sql);

                    }
                }
                else
                {
                    LogStep("不需要结算");
                }
                if (maxTime == DateTime.MinValue) maxTime = DateTime.Today;
                //update lastQuery
                sql = "update Customers set lastQuery ='" + maxTime.ToString("yyyy-MM-dd") + "' where terminal='" + terminal + "' ";
                db.ExecuteCommand(sql);
                LogStep("关闭连接");
                db.Close();
                resultData.Result = newItemList;
            }
            catch (Exception ex)
            {
                resultData.Message = stepName + "时发生异常：" + ex.Message + "\r\nDBStepMessage:" + db.GetStepMsg();
                LogError(stepName, ex);
                db.Dispose();
            }
            finally {
                System.Web.HttpContext.Current.Application.UnLock();
            }

            return resultData;
        }

        private void GetLocalHolday(string tempTime, DBContext db) {
            Holiday holiday = db.QuerySingle<Holiday>(new { day = tempTime });
            if (holiday == null) { throw new Exception("使用网络节假日异常，但本地Holiday没有" + tempTime + "的记录，请设置!"); }
            isHoldDay = holiday.isHoliday > 0;
        }

        /// <summary>
        /// 同步接下来1个月的数据到数据库
        /// </summary>
        [HttpGet]
        public JsonResult SyncHolday() {
            JsonMessage jm = new JsonMessage();
            MyHttpUtility http = new MyHttpUtility();
            DateTime queryTime = DateTime.Today;
            DateTime maxTime = DateTime.Today.AddMonths(1);
            while (queryTime < maxTime)
            {
                int tryCount = 10;
                while (true)
                {
                    try
                    {
                        using (DBContext context = new DBContext())
                        {
                            string queryDate = queryTime.ToString("yyyy-MM-dd");
                            string timeJson = http.DoGet("http://www.easybots.cn/api/holiday.php?d=" + queryDate);
                            if (string.IsNullOrEmpty(timeJson)) throw new Exception("节假日返回值为空");
                            string[] tempArr = timeJson.Substring(1, timeJson.Length - 2).Split(':');
                            if (tempArr.Length != 2) throw new Exception("节假日返回值有变动");
                            int val = Convert.ToInt32(tempArr[1].Replace('\"', ' ').Trim());
                            //写到数据为                        
                            string sql = "update Holiday set isHoliday=" + val + " where day = '" + queryDate + "'";
                            int cc = context.ExecuteCommand(sql);
                            if (cc == 0)
                            {
                                sql = "insert into Holiday(day,isHoliday) values('" + queryDate + "'," + val + " )";
                                context.ExecuteCommand(sql);

                            }
                        }
                        break;
                    }
                    catch (System.Net.WebException ex)
                    {
                        tryCount--;
                        if (tryCount < 0) { jm.LogException(ex); break; }
                        LogError("使用网络节假日发生网络异常，重试" + tryCount, ex);
                    }
                    catch (Exception ex)
                    {
                        jm.LogException(ex);
                    }
                    break;
                }
                queryTime = queryTime.AddDays(1);
            }
            return Json(jm, JsonRequestBehavior.AllowGet);
        }
        private void calcMoney(Customer customerInfo, TransactionLog localItem)
        {
            if (localItem.isValid != 1) return;
            if (customerInfo.IsFengDing && null == customerInfo.fengding) throw new Exception("封顶机没配置封顶金额");
            decimal tixianfei = customerInfo.tixianfei.Value;
            if (isHoldDay) tixianfei += customerInfo.tixianfeiEles.Value;//节假日多收手续费
            localItem.discountMoney = Math.Round(localItem.tradeMoney.Value * customerInfo.discount.Value * 0.01m,2);
            if (!customerInfo.IsFengDing || localItem.discountMoney < customerInfo.fengding)
            {
                //pos ji
                localItem.discountMoney = Math.Round(localItem.tradeMoney.Value * customerInfo.discount.Value * 0.01m, 2);
                decimal totalSxf = Math.Round(localItem.tradeMoney.Value * (customerInfo.discount.Value + tixianfei) * 0.01m, 2);
                //t0
                localItem.tixianfeiMoney = totalSxf - localItem.discountMoney.Value;
            }
            else
            {
                //feng ding
                localItem.discountMoney = customerInfo.fengding;
                localItem.tixianfeiMoney = Math.Round(localItem.tradeMoney.Value * tixianfei * 0.01m, 2);
            }          
            localItem.finallyMoney = localItem.tradeMoney.Value - localItem.discountMoney.Value - localItem.tixianfeiMoney.Value;
        }
        string stepName = "begin";
        log4net.ILog infoLog;
        private void LogStep(string msg) {
            stepName = msg;
            if (infoLog == null) infoLog = log4net.LogManager.GetLogger(this.GetType());
            infoLog.Info(stepName);
        }

		public JsonResult ToRongBao(){
			JsonMessage resultData = new JsonMessage ();
			
			System.Data.DataTable dt = null;
			int RandomNum;
			Random MyRandom = new Random ();
			RandomNum = MyRandom.Next (1001, 9999);
			string batchCurrnum = DateTime.Now.ToString ("yyyyMMddHHmmss") + RandomNum;
            string selectSqlFields = "select top 1 a.id,a.terminal,a.finallyMoney money,a.sum2Id,a.tradeMoney,a.createDate, b.faren,b.shanghuName,b.bankName ,b.bankName2,b.bankName3,b.province,b.bankAccount,b.city,b.tel,b.sourceAccount,isnull(b.dayMax,0) dayMax,isnull(b.dayMin,0) dayMin,isnull(eachMin,0) eachMin , isnull(eachMax,0) eachMax,isnull(b.daifufei,0) daifufei from transactionSum a join customers b on b.terminal = a.terminal and b.status > -1";
            string sql;
            try
            {
                string todayStr = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd 23:00:00");
                string todayEndStr = DateTime.Today.ToString("yyyy-MM-dd 23:00:00");
                string batchDate = DateTime.Now.ToString("yyyyMMdd");
                DataRow dr;//要上传的数据行
                string checkPass = null;
                string terminal;
                inUploadRongbao = true;
                using (DBContext db = new DBContext(true))
                {
                    LogStep("查询结算汇总表，查询第一条结算数据");//开始
                    sql = selectSqlFields+" where a.status = 0 and b.status <> -1 and b.frozen <> 1 and createDate < '" + todayEndStr + "' order by a.createDate";//查询结算汇总表
                    //注意：不要用事务，防止提交融宝成功后的异常又回滚
                    dt = db.QueryTable(sql);
                    if (dt.Rows.Count == 0) return Json(resultData, JsonRequestBehavior.AllowGet);
                    if (dt.Columns.IndexOf("id") == -1) throw new Exception("查询送盘数据失败，没有ID列");
                    dr = dt.Rows[0];
                    if (dr["id"].Equals(DBNull.Value)) throw new Exception("ID列数据为空");
                    if (dr["terminal"].Equals(DBNull.Value)) throw new Exception("terminal列数据为空");
                    if (dr["money"].Equals(DBNull.Value)) throw new Exception("money列数据为空");
                    if (dr["sum2Id"] == DBNull.Value) dr["sum2Id"] = 0;
                    terminal = dr["terminal"].ToString();
                    if (Convert.ToInt32(dr["sum2Id"]) != -1)//如果是合并后的数据，不检测金额
                    {
                        LogStep("终端" + terminal + "开始结算上传,判断金额");
                        decimal minMoney = Convert.ToDecimal(dr["dayMin"]);
                        decimal maxMoney = Convert.ToDecimal(dr["dayMax"]);
                        decimal eachMinMoney = Convert.ToDecimal(dr["eachMin"]);
                        decimal eachMaxMoney = Convert.ToDecimal(dr["eachMax"]);
                        decimal currentMoney = Convert.ToDecimal(dr["tradeMoney"]);
                        if (currentMoney > eachMaxMoney)
                        {
                            db.ExecuteCommand("update transactionsum set status =-2,results ='单笔金额超过上限' where id=" + dr["id"]);
                            checkPass = "金额" + currentMoney + "超过单笔上限";
                        }
                        else if (currentMoney < eachMinMoney)
                        {
                            db.ExecuteCommand("update transactionsum set status =-3,results ='单笔金额未到下限' where id=" + dr["id"]);
                            checkPass = "金额" + currentMoney + "未到单笔下限";
                        }
                        //当日已结算金额,已送的不管成不成功
                        sql = string.Format("select isnull(sum(tradeMoney),0) from transactionsum where terminal='{0}' and (status = -2 or status >0)  and uploadDate between '{1}' and '{2}'", terminal, todayStr, todayEndStr);
                        decimal daySumedMoney = Convert.ToDecimal(db.ExecScalar(sql));
                        daySumedMoney += currentMoney;
                        //当日待结算的金额,该笔之前的                    
                        sql = string.Format("select isnull(sum(tradeMoney),0) from transactionsum where terminal='{0}' and status in (0,-3) and createDate <'{2}'", terminal, todayStr, dr["createDate"]);
                        decimal dayUnSumMoney = Convert.ToDecimal(db.ExecScalar(sql));
                        dayUnSumMoney += currentMoney;
                        if (daySumedMoney > maxMoney)
                        {
                            string reason = checkPass == null ? "金额超过当日上限" : checkPass;
                            db.ExecuteCommand("update transactionsum set status =-3,results ='" + reason + "' where id=" + dr["id"]);
                            checkPass = "今日金额" + daySumedMoney + "," + reason;
                        }
                        else if (dayUnSumMoney < minMoney)
                        {
                            string reason = checkPass == null ? "金额小于当日下限" : checkPass;
                            db.ExecuteCommand("update transactionsum set status =-3,results ='" + reason + "' where id=" + dr["id"]);
                            checkPass = "今日总金额" + dayUnSumMoney + "," + reason;
                        }
                        else
                        {                            
                            //满足条件后消除限额 
                            stepName = "查询是否有满足条件后消除限额的数据";
                            DataTable beforeDisabelDt = db.QueryTable("select id,tradeMoney from transactionsum where status  = -3 and terminal ='" + terminal + "' and createDate < '" + todayEndStr + "'");
                            if (beforeDisabelDt.Rows.Count > 0)
                            { //如果有恢复原来的，则组织成一条再发
                                LogStep("合并数据,之前暂停的有" + beforeDisabelDt.Rows.Count + "条");
                                decimal tempMoney = 0;
                                string canUploadIdArr = "";
                                int canUploadCount = 0;
                                for (int i = 0; i < beforeDisabelDt.Rows.Count; i++)
                                {
                                    decimal rowMoney = Convert.ToDecimal(beforeDisabelDt.Rows[i]["tradeMoney"]);
                                    if (tempMoney + rowMoney > maxMoney) continue;//如果合并后的金额还是大于当日限额，则超出部分不结
                                    tempMoney += rowMoney;
                                    canUploadCount++;
                                    canUploadIdArr += "," + beforeDisabelDt.Rows[i]["id"];
                                }
                                if (canUploadCount == 0) LogStep("汇总成一条记录，但还是超过限额,有" + beforeDisabelDt.Rows.Count + "条记录不会被上传结算");
                                else
                                {
                                    if (tempMoney > minMoney) checkPass = null;//如果当日累计达到条件，则该笔就算未达到也加入合并
                                    if (checkPass == null)
                                    {
                                        canUploadIdArr = dr["id"] + canUploadIdArr;//加上自己（满足条件的这条+原来被暂停的那条）
                                        //汇总成新的记录
                                        object scalar = db.ExecScalar("insert into transactionsum(sum2Id,tradeMoney,discountMoney,tixianfeiMoney,finallyMoney,status,terminal,createDate,batchCount) select -1,sum(tradeMoney),sum(discountMoney),sum(tixianfeiMoney),sum(finallyMoney),0," + terminal + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',sum(batchCount) from transactionsum where id in (" + canUploadIdArr + "); select @@identity");
                                        if (scalar == null) throw new Exception("汇总成新的记录异常，identity is null");
                                        int newSumId = Convert.ToInt32(scalar);
                                        //合并后作废原来的
                                        db.ExecuteCommand("update transactionsum set status = -1,sum2Id=" + newSumId + ",results= isnull(results,'')+ ' 合并到" + newSumId + "'  where id in ( " + canUploadIdArr + ")");
                                        sql =  selectSqlFields+"  where a.id=" + newSumId;
                                        dr = db.QueryTable(sql).Rows[0];//提交合并后的到rongbao
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        LogStep("终端" + terminal + "(合并),开始上传");
                    }
                    db.Close();
                }

                if (checkPass != null)
                {
                    resultData.Message = terminal + ":" + checkPass;
                }
                else
                {
                    SentRowToRongBao( dr, batchCurrnum, batchDate, terminal, ref stepName);
                    resultData.Result = new string[] { batchCurrnum, batchDate, terminal };
                }                
            }
            catch (Exception ex)
            {
                resultData.Message = stepName + "时发生异常：" + ex.Message;
                LogError(stepName, ex);
                if (dt != null)
                {
                    try
                    {
                        using (DBContext context = new DBContext())
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                resultData.Message = resultData.Message.Replace('\'', ' ');
                                string errorStatus = "-2";
                                if (ex is System.Net.WebException && ex.Message.IndexOf("The remote name could not be resolved") > -1)
                                {
                                    LogStep("网络问题，下次继续传");
                                    errorStatus = "0";
                                }
                                sql = "update transactionSum set status= " + errorStatus + " ,results = '" + resultData.Message + "' where batchCurrnum='" + batchCurrnum + "' and id='" + dr["id"] + "' and status= 1";
                                context.ExecuteCommand(sql);
                            }
                        }
                    }
                    catch (MySqlException e)
                    {
                        resultData.Message += " , 更新失败标记也失败！" + e.Message;
                        LogError("更新失败标记", ex);
                    }
                }
            }
            finally
            {
                inUploadRongbao = false;
            }
			return Json (resultData, JsonRequestBehavior.AllowGet);
		}

        /// <summary>
        /// 发送到融宝，注意：上传融宝之前提交一样事务
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="batchCurrnum"></param>
        /// <param name="batchDate"></param>
        /// <param name="terminal"></param>
        /// <param name="errorItem"></param>
        private void SentRowToRongBao(DataRow dr, string batchCurrnum, string batchDate, string terminal, ref string errorItem)
        {


            LogStep("添加代付手续费");
            decimal daifufei = Convert.ToDecimal(dr["daifufei"]);
            dr["money"] = Convert.ToDecimal(dr["money"]) - daifufei;
            string cerFile = getSetting("rongbao_public", true);
            LogStep("更新待上传标记");
            string sql = @"update transactionSum set status= 1,results='正在结算', batchCurrnum='" + batchCurrnum + "'" +
            ", uploadDate='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
            ",faren='" + dr["faren"] + "' " +
            ",shanghuName='" + dr["shanghuName"] + "' " +
            ",bankName='" + dr["bankName"] + "' " +
            ",bankName2='" + dr["bankName2"] + "' " +
            ",bankName3='" + dr["bankName3"] + "' " +
            ",province='" + dr["province"] + "' " +
            ",city='" + dr["city"] + "' " +
            ",bankAccount='" + dr["bankAccount"] + "' " +
            ",sourceAccount='" + dr["sourceAccount"] + "' " +
            ",tel='" + dr["tel"] + "' " +
            " where id='" + dr["id"] + "'";
            using (DBContext db = new DBContext())
            {
                db.ExecuteCommand(sql);//标记状态为待上传
                db.Close();
            }
            errorItem = "上传融宝处理";
            Common.RongBao.RSACryptionClass testClass = new Common.RongBao.RSACryptionClass(cerFile, "");
            string returnStr = testClass.Sent(dr.Table, batchCurrnum, batchDate);
            LogStep("融宝执行成功,更新成功状态");
            sql = "update transactionSum set status= 2,daifufei=" + daifufei + ",finallyMoney=" + dr["money"] + ",results='银行处理中' where id='" + dr["id"] + "'";
            using (DBContext db = new DBContext())
            {
                db.ExecuteCommand(sql);
                errorItem = null;
                string msgTitle = Convert.ToDateTime(dr["createDate"]).ToString("HH:mm:ss") + dr["faren"] + "消费额" + dr["tradeMoney"] + "元，准备出款" + dr["money"] + "元";
                string msgContent = "<p>终端" + dr["terminal"] + ",法人：" + dr["faren"] + ",商户：" + dr["shanghuName"] + "</p>";
                msgContent += "<p>" + dr["createDate"] + "发起结算：" + dr["tradeMoney"] + "元，需要给客户打款：" + dr["money"] + "元</p>";
                msgContent += "<p><a href='https://bgp.reapal.com/tbAgentpayInfo/agentpaylist'>点此前往融宝审核</a></p>";
                string emails = getSetting("emails");
                foreach (string mail in emails.Split(','))
                {
                    SentEmail(mail, msgContent, msgTitle, db);
                }
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public void SentEmail(string email, string content, string subject, DBContext context)
        {
            //string MailContent = "<p style='text-align:left;font-size:14px;padding-top:50px;padding-left:22px;'>亲爱的&nbsp;<font color='red'>" + this.txtStuName.Text + " </font>同学，你好：</p>";
            //MailContent += "<p style='text-align:left;font-size:14px;padding-top:5px;padding-left:22px;'>　　请你点击下面链接来修复宏天实训管理平台的登录密码：</p>";
            //MailContent += "<p style='text-align:left;font-size:14px;padding-top:5px;padding-left:22px;'>　　<a target='_blank' style='color:red;' href ='http://www.shixun.org/Acedu/UpdatePwd.aspx?sdfjsdk=" + Encrypt(StudentID) + "'>http://www.shixun.org/Acedu/UpdatePwd.aspx?sdfjsdk=" + Encrypt(StudentID) + "</a></p>";
            //MailContent += "<p style='text-align:left;font-size:14px;padding-top:5px;padding-left:22px;'>　　为了确保你的帐号安全，修改密码后将该邮件删除。</p> ";
            //MailContent += "<p style='text-align:left;font-size:14px;padding-top:5px;padding-left:22px;'>　　如果该链接已经无效，请你点击<a target='_blank' style='color:red;' href='http://www.shixun.org/Acedu/GetBackPassword.aspx'> 这里 </a>重新获取修复密码邮件。</p>";
            //MailContent += "<p style='text-align:left;font-size:14px;padding-top:5px;padding-left:22px;'>　　如果点击链接无效，请你选择并复制整个链接，打开浏览器窗口并将其粘贴到地址栏中。然后单击“转到”按钮或按键盘上的 Enter 键。</p><br /><br />";
            //MailContent += "<p style='text-align:right;font-size:14px;padding-right:100px;'>福建宏天实训中心<p><br />";
            ////获取当天的日期


            //String StrDate = Format(DateTime.Today, "yyyy年M月d日");
            //MailContent += "<p style='text-align:right;font-size:14px;padding-right:130px;'>" + DateTime.Now.ToShortDateString() + "</p>";

            SmtpClient mail = new SmtpClient();     //实例   
            mail.Host = "smtp.qq.com";     //发信主机  
            //初始化FromAddress(master@shixun.org) 与　ToEmailAddress
            MailMessage mm = new MailMessage("master@qqzi.com", email);
            mm.Subject = subject; // "圈子金融验证邮箱";
            mm.Body = content;
            mm.SubjectEncoding = System.Text.ASCIIEncoding.GetEncoding("gb2312");
            mm.BodyEncoding = System.Text.ASCIIEncoding.GetEncoding("gb2312");
            // mail.Body = "＜h2＞This is an HTML-Formatted Email Send Using ＜/span＞.＜/p＞";
            mail.Credentials = new System.Net.NetworkCredential("master@qqzi.com", "qqzi87156156");
            mm.IsBodyHtml = true;
            //mail.Credentials.GetCredential("smtp.qq.com", 25, "Network");     //发信认证主机及端口 
            try
            {
                mail.Send(mm);
            }
            catch (Exception ex)
            {
                LogError("邮件发送失败", ex);
            }
            //this.lblErrorMsg.Text = "<font color='red'>邮件发送成功,请接收Email并更改您的密码!</font>";
        }

		System.Collections.Generic.Dictionary<string,string> loginImages = new Dictionary<string, string> ();
		System.Collections.Generic.Dictionary<string,string> loginImageTexts = new Dictionary<string, string> ();
		public void NeedLoginImage(string posSer, string url){
			string img = null;
			switch(posSer){
				case "jingkong":
				img = "https://119.4.99.217:7300/mcrm/" + url;
					break;
				default:
					break;
			}
			if(img!=null){
				if(loginImages.ContainsKey(posSer)) loginImages[posSer]= img;
				else loginImages.Add(posSer,img);
				loginImageTexts.Remove (posSer);
			}
		}

		public void EnterLoginImage(string posSer,string txt){
			if (loginImageTexts.ContainsKey (posSer))
				loginImageTexts [posSer] = txt;
			else
				loginImageTexts.Add (posSer, txt);

		}

		public string GetLoginImageText(string posSer){
			string txt;
			loginImageTexts.TryGetValue (posSer, out txt);
			return txt;
		}

		/// <summary>
		/// 查询融宝执行情况
		/// </summary>
		/// <returns>The rong bao.</returns>
		/// <param name="batchCurrnum">Batch currnum.</param>
		/// <param name="batchDate">Batch date.</param>
		public JsonResult GetRongBao(string batchCurrnum,string batchDate ){
			JsonMessage resultData = new JsonMessage ();
            string responseXml = "";
            string publicCer = getSetting("rongbao_public", true);
            string cerFile = getSetting("rongbao_private", true);
            Common.RongBao.RSACryptionClass testClass = new Common.RongBao.RSACryptionClass(publicCer, cerFile);
            int tryCount = 5;
            while (true)
            {
                try
                {
                    resultData.Result = testClass.TryGetResult(batchCurrnum, batchDate, out responseXml);
                    break;
                }
                catch (System.Net.WebException webEx) {
                    tryCount--;
                    if (tryCount < 0)
                    {
                        LogError("查询融宝执行结果发生网络错误，已重试10次没有成功！：", webEx);
                        break;
                    }
                    else
                    {
                        LogError("查询融宝执行结果发生网络错误，重试" + tryCount + "：", webEx);
                        System.Threading.Thread.Sleep(5000);
                    }
                }
                catch (Exception ex)
                {                    
                    string errorMsg = "GetRongBao " + batchCurrnum + " " + batchDate + " rongbao res is :" + responseXml + "\r\n";
                    LogError(errorMsg, ex);
                    resultData.Message = ex.Message;
                    break;
                }
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
                        instance= new MySqlExecute();
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
				}
                else if (evalResult is JsonMessage) {
                    jr = evalResult as JsonMessage;
                }
                else
                {
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
				Exception ex2 = ex.InnerException==null?ex:ex.InnerException;
                ColysSharp.Utility.LogException(jr,ex2);
				LogError ("eval",ex2);
			}
			return JsonConvert.SerializeObject(jr);
		}



        public JsonResult status()
        {
            //using (DBContext context = new DBContext())
            //{
            //    string sql = "delete from transactionsum ;delete from  transactionLogs;  update customers set lastquery='2012-08-26'";
            //    context.ExecuteCommand(sql);
            //}
            PosPlatformClient timeOutItem = null;
            foreach (KeyValuePair<string, PosPlatformClient> ki in clients)
            {
                TimeSpan ts = DateTime.Now - ki.Value.lastAccess;
                if (ts.TotalMinutes > 10) ki.Value.timeout = true;
                if (ki.Value.timeout ) { timeOutItem = ki.Value; break; }
            }
            if (timeOutItem!=null)
            {
                TimeSpan ts = DateTime.Now - timeOutItem.lastAccess;
                if (ts.TotalMinutes > 20)
                {
                    clients.Remove(timeOutItem.name);
                    timeOutItem = null;
                }
            }
            var result = new { clientCount = clients.Count, versionTime = "08-27 21:40", timeout = timeOutItem != null, platform = timeOutItem != null ? timeOutItem.name : "" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult clearSum() { 
            string sql="delete from transactionSum;delete from transactionLogs;update customers set lastquery ='"+DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")+"'";
            using(DBContext db = new DBContext()){
            db.ExecuteCommand(sql);
            }
            return RedirectToAction("status");
        }
        
        static Dictionary<string, PosPlatformClient> clients = new Dictionary<string, PosPlatformClient>();
        #region 监控登录相关
        public void RegisterClient(string platform) {            
            PosPlatformClient client;
            string ip = GetClientIp();
            if (clients.TryGetValue(platform, out client))
            {
                if (!client.timeout) throw new ColysSharp.ClientException("已经有在运行的监控！");
                clients.Remove(platform);
            }
            if (client == null) client = new PosPlatformClient();
            client.ip = ip;
            client.verifyCode = null;
            client.timeout = false;
            client.lastAccess = DateTime.Now;
            client.name = platform;
            clients.Add(platform, client);
        }

        //public void RegisterOuttime(string posSer)
        //{
        //    PosPlatformClient client=clients[posSer];
        //    client.timeout = true;            
        //}

        /// <summary>
        /// 重登录，上传验证码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pws"></param>
        public string UploadForReLogin(string platform) {
            try
            {
                PosPlatformClient client = GetClient(platform);
                client.timeout = true;
                client.verifyCode = null;
                if (Request.Files.Count == 0) throw new Exception("没有要上传的验证码图片");
                string root = Server.MapPath("~/Content/VerifyCode");
                if(!System.IO.Directory.Exists(root)) System.IO.Directory.CreateDirectory(root);
                Request.Files[0].SaveAs(root + "\\" + platform + ".jpg");
                return null;
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }
        /// <summary>
        /// web端在超时的时候录入验证码
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="verifyCode"></param>
        public void SetVerifyCode(string platform, string verifyCode)
        {
            PosPlatformClient client = GetClient(platform);
            client.verifyCode = verifyCode;
        }

        /// <summary>
        /// 监控端去监视web上录入验证码了没有
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public PosPlatformClient IsVerifyCodeSet(string platform)
        {
            PosPlatformClient client = GetClient(platform);
            if (client.verifyCode == null) return null;
            else return client;
        }

        private PosPlatformClient GetClient(string platform)
        {
            PosPlatformClient client;
            if (!clients.TryGetValue(platform, out client))
            {
                RegisterClient(platform);
                client = clients[platform];
                client.lastAccess = DateTime.Now.AddMinutes(-20);
            }
            return client;
        }

        public void RegisterLogoff(string platform) {
            clients.Remove(platform);
        }

     

        #endregion
        public string QuerySql(string sql,string pwd) {
            if (pwd != "181016") return null;
            using (DBContext context = new DBContext())
            {                
                DataTable dt = context.QueryTable(sql);
                return JsonConvert.SerializeObject(dt);
            }            
        }

        public ViewResult QueryPage() {
            return View();
        }


		private string FormatString(string str)
		{
			return str.Replace("'", "");
		}


        public override string DoQuery(string permission, string queryField, string entityName, string whereField, string orderBy)
        {
            return base.DoQuery(permission, queryField, entityName, whereField, orderBy);
        }
	}

  
}
