using ColysSharp.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common
{

    [DBTable(TableName = "transactionlogs")]
    public class TransactionLog : IDBEntity
    {
        [DBField(Usage = DBFieldUsage.PrimaryKey)]
        public int id { get; set; }
        public string time { get; set; }

        public string timeStr { get; set; }
        public string terminal { get; set; }
        public string tradeName { get; set; }
        public string results { get; set; }
        public decimal? tradeMoney { get; set; }
        public decimal? discountMoney { get; set; }
        public decimal? tixianfeiMoney { get; set; }
        public string resultCode { get; set; }
        public decimal? finallyMoney { get; set; }
        public string faren { get; set; }
        public int? sumid { get; set; }
        [DBField(Usage = DBFieldUsage.MarkDelete)]
        public int? status { get; set; }
        public int? isValid { get; set; }

        
    }
}