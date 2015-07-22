using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sashulin;

namespace WinForm
{
    
    static class Program
    {
        public static string[] mainArgs;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
//			string privateKey = Application.StartupPath+"\\rongbao_decry.p12";
//			string publicKey =  Application.StartupPath+"\\tomcat.cer";
//			Common.RongBao.RSACryptionClass rbclass = new Common.RongBao.RSACryptionClass(publicKey, privateKey);
//            try
//            {
//                System.Data.DataTable dt =new System.Data.DataTable();
//                string[] strArr = "id,bankAccount,faren,bankName,bankName2,bankName3,finallyMoney,province,city,phone".Split(',');
//                foreach(string str in strArr){dt.Columns.Add(str);}
//                System.Data.DataRow dr = dt.NewRow();
//                dr["id"]="2015072801";
//                dr["bankAccount"]="6225882712901831";
//                dr["bankName"]="中国招商银行";
//                dr["bankName2"]="武汉分行";
//                dr["bankName3"] = "汉阳支行";
//                dr["faren"]="吕存库";
//                dr["finallyMoney"]="100";
//                dr["province"]="湖北";
//                dr["city"]="武汉";
//                dr["phone"]="17097913883";
//                dt.Rows.Add(dr);
//                //string result = rbclass.Sent(dt, "2015072201");
//                //MessageBox.Show(result);
//				Common.QueryResult qr = rbclass.TryGetResult("2015072201", "20150722");
//				MessageBox.Show(qr.batchStatus.ToString());
//            }
//            catch (Exception ex) {
//                MessageBox.Show(ex.Message);
//            }
            Application.Run(new frmMain());
        }
    }
}
