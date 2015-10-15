using ColysSharp.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web2.Models
{
     [DBTable(TableName = "transactionsum")]
    public class TransactionSum:ColysSharp.DataBase.IDBEntity
    {
        public string terminal { get; set; }

         [DBField(Usage=DBFieldUsage.MarkDelete)]
        public int? status { get; set; }

        public string results { get; set; }
        public double? tradeMoney { get; set; }
        public double? discountMoney { get; set; }
        public double? tixianfeiMoney { get; set; }
        public double? finallyMoney { get; set; }
        public string faren { get; set; }
         [DBField( Usage= DBFieldUsage.PrimaryKey)]
        public int id { get; set; }

        public string createDate { get; set; }

        public string uploadDate { get; set; }

        public string reciveDate { get; set; }

        public string batchCurrnum { get; set; }
    }
}