﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColysSharp.DataBase;

namespace Web2.Models
{
    [DBTable(TableName="customers")]
    public class Customer
    {
        [DBField( Usage= DBFieldUsage.PrimaryKey)]
        public int id { get; set; }

        public string terminal { get; set; }

        public string faren { get; set; }

        public string shanghuName { get; set; }

        public string tel { get; set; }

        public float discount { get; set; }

        public float fengding { get; set; }

        public float tixianfeiEles { get; set; }

        public float tixianfei { get; set; }

        public string bankName { get; set; }
        public string bankName1 { get; set; }

        public string bankName2 { get; set; }

        public string bankName3 { get; set; }

        public string bankAccount { get; set; }

        [DBField(Usage=DBFieldUsage.MarkDelete)]
        public int status { get; set; }

        public string province { get; set; }
        public string city { get; set; }
        public int sourceAccount { get; set; }

        public bool IsFengDing { get; set; }

    }
}