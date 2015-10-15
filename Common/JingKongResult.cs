using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    /// <summary>
    /// 金控的查询返回结果集json结构
    /// </summary>
    public class JingKongResult
    {
        int total { get; set; }
        public List<JingKongResultItem> rows { get; set; }

        public TransactionLog[] ToTransactionArray()
        {
            if (rows == null) throw new Exception("JingKongResult.row is null");
            TransactionLog[] newItemList = new TransactionLog[rows.Count];
            for (int i = 0; i < newItemList.Length; i++)
            {
                newItemList[i] = rows[i].ToTransactionLog();
            }
            return newItemList;
        }
    }

    public class JingKongResultItem{
        public string  tdate{get;set;}
        public string stime { get; set; }
        public string rspcode { get; set; }

        public string amt { get; set; }

        public string lpName { get; set; }

        public string trname { get; set; }

        public string tid { get; set; }
        public string rspmsg { get; set; }

        public string descr { get; set; }

        public TransactionLog ToTransactionLog()
        {
            if (this.trname == null) this.trname = "";//会有空名字的交易，比如系统无法找到POS请求交易 message_type=0820
            if (this.amt == null) throw new ArgumentNullException("json row item value amt");
            if (this.amt == null) throw new ArgumentNullException("json row item value tdate");
            if (this.amt == null) throw new ArgumentNullException("json row item value stime");
            if (this.tid == null) throw new ArgumentNullException("json row item value tid");
            if (this.descr == null) throw new ArgumentNullException("json row item value descr");//descr: "20151010175846"            
            if (this.descr.Length != 14) throw new ArgumentNullException("json row item value descr length is not 14");
            TransactionLog log = new TransactionLog();            
            string dayStr = this.tdate;
            string timeStr = this.stime;
            //string minute = timeStr.Substring(2, 2);
            //if (minute.CompareTo("59") > 0) minute = "59";
            //string second = timeStr.Substring(4, 2);
            //if (second.CompareTo("59") > 0) second = "59";
            log.time = descr.Substring(0, 4) + '-' + descr.Substring(4, 2) + '-' + descr.Substring(6, 2) + " " + descr.Substring(8, 2) + ":" + descr.Substring(10, 2) + ":" + descr.Substring(12, 2);
            decimal money = Convert.ToDecimal(this.amt);
            log.isValid = 0;
            if (this.trname.IndexOf("冲正") > -1) { money = -1 * money; log.isValid = 1; }
            else if (this.trname == "消费") log.isValid = 1;
            if (this.rspcode != "00") log.isValid = 0;
            log.resultCode = this.rspcode;
            log.terminal = this.tid;
            log.tradeName = this.trname;
            log.tradeMoney = money;
            log.faren = this.lpName;
            log.results = this.rspmsg;
            log.timeStr = this.tdate + " " + this.stime;
            return log;

        }
    }
}
