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
        public string tradeMoney { get; set; }
        public string discountMoney { get; set; }
        public string tixianfeiMoney { get; set; }

        public string finallyMoney { get; set; }
        public string faren { get; set; }
        public string sumid { get; set; }
    }
}