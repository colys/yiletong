using ColysSharp.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web2.Models
{

    [DBTable(TableName="transactionlogs")]
    public class TransactionLog:IDBEntity
    {
        public string time { get; set; }
        public string terminal { get; set; }
        public string tradeName { get; set; }
        public string results { get; set; }
        public double? tradeMoney { get; set; }
        public double? discountMoney { get; set; }
        public double? tixianfeiMoney { get; set; }

        public double? finallyMoney { get; set; }
        public string faren { get; set; }
        public int? sumid { get; set; }
    }
}