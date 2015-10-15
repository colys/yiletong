using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColysSharp.DataBase;

namespace Common
{
     [DBTable(TableName = "holiday")]
    public class Holiday:IDBEntity
    {
        [DBField(Usage = DBFieldUsage.PrimaryKey)]
        public int id { get; set; }
        public string day { get; set; }
         /// <summary>
        /// 0：不是，1：周末，2：法定假日
         /// </summary>
        public int isHoliday { get; set; }

    }
}
