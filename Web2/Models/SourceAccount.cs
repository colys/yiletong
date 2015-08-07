using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColysSharp.DataBase;

namespace Web2.Models
{
    [DBTable(TableName="SourceAccounts")]
    public class SourceAccount:IDBEntity
    {
        [DBField(Usage = DBFieldUsage.PrimaryKey)]
        public int id { get; set; }

        [DBField(Usage = DBFieldUsage.MarkDelete)]
        public int status { get; set; }

        public string faren { get; set; }

        public string bankName { get; set; }
        public string bankAccount { get; set; }

    }
}