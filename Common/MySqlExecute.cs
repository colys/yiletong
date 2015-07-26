using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MySqlExecute:IDisposable
    {
		public MySqlExecute(){
			
		}
        public MySqlExecute(string tableSuffix,string connStr)
        {
            table_Suffix = tableSuffix;
            conStr = connStr;
        }
		public MySqlExecute(string tableSuffix,string connStr,bool inTran)
		{ table_Suffix = tableSuffix;
			conStr = connStr;
			withTrans = inTran;
		}
        public string GetTableName(string table)
        {
            return table + table_Suffix;
        }
		public string table_Suffix = "";

        MySqlConnection conn;
		MySqlTransaction trans;
        MySqlCommand cmd;
		public bool withTrans = false;
		public string conStr;

        private void OpenMysql()
        {
           // string conStr = getSetting("connstr");
            if (conn == null) conn = new MySqlConnection(conStr);
            if (conn.State == ConnectionState.Closed) conn.Open();
			if (withTrans)
				trans=conn.BeginTransaction ();
        }

        //private string getSetting(string name)
        //{
        //    if (System.Configuration.ConfigurationManager.AppSettings[name] == null)
        //    {
        //        MessageBox.Show("配置文件不正确！");
        //        Application.Exit();
        //    }
        //    return System.Configuration.ConfigurationManager.AppSettings[name].ToString();
        //}

        private void setCommand(string sql)
        {
            if (conn == null) OpenMysql();
            if (cmd == null) cmd = new MySqlCommand(sql, conn);
			if (trans!=null)cmd.Transaction = trans;
            else cmd.CommandText = sql;
        }

		public int ExecuteCommand(string sql)
        {
            setCommand(sql);
            return cmd.ExecuteNonQuery();
        }

		public DataTable QueryTable(string sql)
        {
            setCommand(sql);
            System.Data.DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        private object QueryScalar(string sql)
        {
            setCommand(sql);
            return cmd.ExecuteScalar();
        }

        public string GetNextVal(string table)
        {
            if (table == null) throw new Exception("table parameter error");
            table = GetTableName(table);
            OpenMysql();

            object nextVal = QueryScalar("select val from erp_sequence where tableName='" + table + "'");
            if (nextVal == null || nextVal == DBNull.Value)
            {
                nextVal = 1;
                ExecuteCommand("insert into erp_sequence (tableName,val) values('" + table + "','" + nextVal + "')");
            }
            else
            {
                nextVal = Convert.ToInt32(nextVal) + 1;
                ExecuteCommand("update erp_sequence set val ='" + nextVal + "' where tableName=  '" + table + "'");
            }
			return nextVal.ToString();
        }

        public DataTable ExecQuery(string table, string fields, string where, string order)
        {

            string sql = "select " + fields + " from " + GetTableName(table);
            if (where != null && where.Length > 0) sql += " where " + where;
            if (order != null && order.Length > 0) sql += " order by " + order;
            DataTable dt = QueryTable(sql);
			Close ();
            return dt;
            //return Newtonsoft.Json.JsonConvert.SerializeObject(dt);            
        }

        public int ExecDb(string jsonArrStr)
        {
			withTrans = true;
            int changeCount = 0;
            QueryItem[] queryItems = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryItem[]>(jsonArrStr);
            if (queryItems == null)
                throw new Exception("json error");
			OpenMysql ();

            foreach (QueryItem item in queryItems)
            {
                string sql;
                string table = GetTableName(item.table);
                switch (item.action)
                {
                    case DBAction.Add:
                        if (item.fields == null || item.fields.Length == 0)
                            throw new Exception("没有要插入的字段");
                        sql = "insert into " + table + " (";
                        for (int i = 0; i < item.fields.Length; i++)
                            sql += item.fields[i] + ",";
                        sql = sql.Substring(0, sql.Length - 1) + " ) values(";
                        for (int i = 0; i < item.fields.Length; i++)
                        {
                            if (item.values[i] != null && item.values[i].ToUpper() != "NULL")
                            {
                                sql += "'" + item.values[i].Replace('\'', '\"') + "',";
                            }
                            else
                            {
                                sql += "null,";
                            }
                        }
                        sql = sql.Substring(0, sql.Length - 1) + ")";
                        changeCount+=ExecuteCommand(sql);
                        break;
                    case DBAction.Update:
                        if (item.fields == null || item.fields.Length == 0)
                            throw new Exception("没有要更新的字段");
                        sql = "update " + table + " set ";
                        for (int i = 0; i < item.fields.Length; i++)
                        {
                            if (item.values[i] != null && item.values[i].ToUpper() != "NULL")
                            {
                                sql += item.fields[i] + " = '" + item.values[i].Replace('\'', '\"') + "',";
                            }

                        }
                        sql = sql.Substring(0, sql.Length - 1) + " where " + item.where;
                        changeCount += ExecuteCommand(sql);
                        break;
                    case DBAction.Delete:
                        sql = "delete from " + table + " where " + item.where;
                        changeCount += ExecuteCommand(sql);
                        break;
                }
            }

			Close();
            return changeCount;
        }

		public void Close(){
			if (conn!=null && conn.State == ConnectionState.Open) {
				if (trans != null)
					trans.Commit ();
				conn.Close ();
			}
		}


        public void Dispose()
        {
			if (conn != null) {
				if (conn.State == ConnectionState.Open) {
					if (trans != null)
						trans.Rollback ();
					conn.Close ();
				}
				conn.Dispose ();
			}
        }
    }


    public enum DBAction { Add = 0, Delete = 2, Update = 1 }

    public class QueryItem
    {
        public String table;
        public DBAction action;
        public String[] fields;
        public String[] values;
        public String where;
    }
}
