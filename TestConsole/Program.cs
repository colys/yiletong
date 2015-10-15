using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColysSharp.DataBase;
using Common;
using System.Data;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //string str = @"Xn66YzcX/6BDhq9w4T9MTtGp9+nuH9j/3HrsvUQnmI7MEwJgEMGeuzvIxiC0JYO1sunUtu4ct+5Fa77RUznqLXuJg3e3Osj/nNAE5gwqj/ZRLT0LWjhups4hJlUL6AuTlPhZW5bMDjBHJfMuKsGC3dd1HZ+B9egIMmU3bZJNly4H0GICgtP7q9fqZM37R7VkV7YMVuwIZWV4wFM7de3eOI4TWaOW61p5SG4QAgSJ0PGNK2qO3GZKR5BeoY2FhHLzfWq6Ixg5W+ACPzb2jm/hH+v0gudR05+2WYpGcesNM493kMwLOCu3CdQpOsPCY0cAwSAjBtDs6ZVb+GgZig97hpv1yWmcwpAPLOxiFUX+3QqDgaoPYBCAPzVNtBvklCaa3ovcvVhIBKksv7ZFtzYy5wrF1bdZeg9SS0QAdK8dBSPHcbm2h0jv0GtZE+2zaWaE+ws+ZfRUJBKpe5xannau7XN8UrAh0k7XxxW4cp+1mqvQuJB9+dwgzvJAZ0BQ38f6nu+8ao6oPivc4AXSZg8SFDWYdNAVwvM215ObCx4p5dHDzOLx3uLgCduw489mdUEJbRYks/D1ABmkVZtaaxwxOEMvsVdVttoq4wcTCbPsWciGjizq+K2Sb2rtvyQd16YwZz+ZGl+QKiPWtv2c2Wc1ob63PZ+hw63eEg7/SK0fScY=";
            //SecurityClass security = new SecurityClass(Encoding.GetEncoding("GBK"));            
            //string returnPayValue = security.RSADecrypt(@"D:\QuickDisk\MyWork\yiletong\Web2\Content\rongbao_decry.p12", "clientok", str);
            //Console.WriteLine(returnPayValue);
          //  testAutoLogin();
            string conn = "Data Source=www.qqzi.com;Initial Catalog=yiletong;User ID=yiletong;Password=yiletongpass";
            DBContext.LoadConfig("{}", new DBContextConfig() { ConnectionString = conn, DatabaseType = "sqlserver", EntityTypeFormat = "Common.{0},Common" });
            string error="";
            //SentRowToRongBao("201509051733456664", "2015-09-05 ", "66050196",ref error);
            //testflotadd();
            
        }

        private static void testflotadd() {
            
            using (DBContext context = new DBContext()) {
                Customer c = context.QuerySingle<Customer>("17");
                TransactionLog log = context.QuerySingle<TransactionLog>("7832");
                calcMoney(c, log);
                Console.Write("{0}, t0 : {1} , finallyMoney :{2}", log.discountMoney, log.tixianfeiMoney,log.finallyMoney);
            }            
           
        }


        private static void calcMoney(Customer customerInfo, TransactionLog localItem)
        {
            if (localItem.isValid != 1) return;
            if (customerInfo.IsFengDing && null == customerInfo.fengding) throw new Exception("封顶机没配置封顶金额");
            decimal tixianfei = customerInfo.tixianfei.Value;
            if (true) tixianfei += customerInfo.tixianfeiEles.Value;//节假日多收手续费
            localItem.discountMoney = Math.Round(localItem.tradeMoney.Value * customerInfo.discount.Value * 0.01m, 2);
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
            //代付手续费
            if (customerInfo.daifufei != null && customerInfo.daifufei.Value > 0)
            {
                localItem.tixianfeiMoney += customerInfo.daifufei.Value;
            }
            localItem.finallyMoney = localItem.tradeMoney.Value - localItem.discountMoney.Value - localItem.tixianfeiMoney.Value;
        }

        private static void SentRowToRongBao( string batchCurrnum, string batchDate, string terminal, ref string errorItem)
        { string selectSqlFields = "select top 1 a.id,a.terminal,a.finallyMoney money,a.sum2Id,a.tradeMoney,a.createDate, b.faren,b.shanghuName,b.bankName ,b.bankName2,b.bankName3,b.province,b.bankAccount,b.city,b.tel,b.sourceAccount,isnull(b.dayMax,0) dayMax,isnull(b.dayMin,0) dayMin,isnull(eachMin,0) eachMin , isnull(eachMax,0) eachMax,isnull(b.daifufei,0) daifufei from transactionSum a join customers b on b.terminal = a.terminal";
            

            using (DBContext db = new DBContext(true))
            {
                string sql =  selectSqlFields+"  where a.id=488";
                DataRow dr = db.QueryTable(sql).Rows[0];
                decimal daifufei = Convert.ToDecimal(dr["daifufei"]);
                dr["money"] = Convert.ToDecimal(dr["money"]) - daifufei;
                string cerFile = @"F:\colys\QuickDisk\MyWork\yiletong\Web\Content\tomcat.cer";
                //LogStep("更新待上传标记");
                sql = @"update transactionSum set status= 1,results='正在结算',daifufei=" + daifufei + ",finallyMoney=" + dr["money"] + ", batchCurrnum='" + batchCurrnum + "'" +
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

                db.ExecuteCommand(sql);//标记状态为待上传

                errorItem = "上传融宝处理";
                Common.RongBao.RSACryptionClass testClass = new Common.RongBao.RSACryptionClass(cerFile, "");
                string returnStr = testClass.Sent(dr.Table, batchCurrnum, batchDate);
                //LogStep("融宝执行成功,更新成功状态");
                sql = "update transactionSum set status= 2,results='银行处理中' where id='" + dr["id"] + "'";
                db.ExecuteCommand(sql);
                errorItem = null;
            }
        }

        static MyHttpUtility JingKongHttp = new MyHttpUtility();
        private static void testAutoLogin() {

            
            JingKongHttp.DoGet("https://119.4.99.217:7300/mcrm/login.jsp");
            Random MyRandom = new Random();
            int RandomNum = MyRandom.Next(100000000, 999999999);
            string url = "https://119.4.99.217:7300/mcrm/code/code?" + RandomNum;
            string filePath = "d:\\aaaaaaaa.jpg";
            string basePath =@"D:\QuickDisk\MyWork\yiletong\output";
            JingKongHttp.Download(url, filePath, basePath, "https://119.4.99.217:7300/mcrm/j_spring_security_check");
            Console.WriteLine("download ok, please input the code");
            string code = Console.ReadLine();
            string data = "j_username=福州易乐通&j_password=Ylt123456&codeVal=" + code;
            string response = JingKongHttp.DoPostHttps("https://119.4.99.217:7300/mcrm/j_spring_security_check", data, basePath, "https://119.4.99.217:7300/mcrm/login.jsp");
            if (response.IndexOf("<div class=\"alert alert-danger\" style=\"margin: 0px;\">验证码错误</div>") > -1)
            {
                Console.WriteLine("重登录失败，验证码" + code + "错误");
                testAutoLogin();
            }
            
                
        }
    }
}
