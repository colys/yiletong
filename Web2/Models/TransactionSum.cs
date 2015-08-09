using ColysSharp.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web2.Models
{
     [DBTable(TableName = "transactionsum")]
    public class TransactionSum
    {
        public string terminal { get; set; }
        public string status { get; set; }

        public string results { get; set; }
        public string tradeMoney { get; set; }
        public string discountMoney { get; set; }
        public string tixianfeiMoney { get; set; }
        public string finallyMoney { get; set; }
        public string faren { get; set; }
        public string id { get; set; }

        public string createDate { get; set; }

        public string uploadDate { get; set; }

        public string reciveDate { get; set; }

        public string batchCurrnum { get; set; }
    }
}