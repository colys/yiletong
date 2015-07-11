﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace web.Controllers
{
    public class HomeController : Controller
    {
       static string  connStr = System.Configuration.ConfigurationManager.ConnectionStrings["connstr"].ToString();


       private string UserName {
           get
           {
               if (Session["userName"] == null) return null;
               ViewBag.UserName = Session["userName"].ToString();
               return ViewBag.UserName;
           }
           set
           {
               Session["userName"] = value;
           }
       }

       public ActionResult Index()
       {
           ViewBag.UserName = UserName;
           return View();
       }

        public ActionResult Login()
        {            
            return View();
        }

        public ActionResult DoLogin()
        {
            string inputUserName = Request["username"].Trim();
            string password = Request["password"].Trim();
            string sql = "select * from users where userName = '" + inputUserName + "'";
            DataTable dt = QueryTable(sql,true);
            if (dt.Rows.Count == 0)
            {
                ViewBag.Error = "用户不存在！";
                return View("Login");
            }
            else
            {
                if (dt.Rows[0]["password"].Equals(password))
                {
                    UserName = inputUserName;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "密码错误！";
                    return View("Login");
                }
            }

        }

        public ActionResult Customer()
        {
            if (UserName == null)
            {
                return RedirectToAction("Login");
            }
            return View();
                 
        }

        public ActionResult Transaction()
        {
            if (UserName == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }



        MySqlConnection conn;
        MySqlCommand cmd;


        private void OpenMysql()
        {   
            if (conn == null) conn = new MySqlConnection(connStr);
            if (conn.State == ConnectionState.Closed) conn.Open();
        }


        private void setCommand(string sql)
        {
            if (conn == null) OpenMysql();
            if (cmd == null) cmd = new MySqlCommand(sql, conn);
            else cmd.CommandText = sql;
        }

        private void ExecuteCommand(string sql)
        {
            setCommand(sql);
            cmd.ExecuteNonQuery();
        }

        private DataTable QueryTable(string sql,bool close)
        {
            setCommand(sql);
            System.Data.DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            if (close) conn.Close();
            return dt;
        }

        private object QueryScalar(string sql)
        {
            setCommand(sql);
            return cmd.ExecuteScalar();
        }


        public string GetCustomers()
        {
            string sql = "select * from customers where status = 1";
            DataTable dt= QueryTable(sql, true);
            return JsonConvert.SerializeObject (dt);
        }

        public string GetTransactions(string startDate, string endDate, string termid, string cusName, string company)
        {
            if (string.IsNullOrEmpty(startDate)) startDate = DateTime.Today.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(endDate )) endDate = DateTime.Today.ToString("yyyy-MM-dd");
            string sql = "select a.*,b.shanghuName from transactionLogs a left join customers b on b.terminal=a.terminal where time between '" + Convert.ToDateTime(startDate).AddDays(-1).ToString("yyyyMMdd") + "230000'" + " and '" + endDate.Replace("-", "") + " 999999'";
            if (!string.IsNullOrEmpty(termid)) sql += " and b.terminal ='" + FormatString(termid) + "' ";
            if (!string.IsNullOrEmpty(cusName)) sql += " and b.faren ='" + FormatString(cusName) + "' ";
            DataTable dt = QueryTable(sql, true);
            return JsonConvert.SerializeObject(dt);
        }

        private string FormatString(string str)
        {
            return str.Replace("'", "");
        }
    }
}
