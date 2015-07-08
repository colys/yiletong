using Sashulin;
using Sashulin.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormDemo
{

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class frmMain : Form
    {
        string userName="福州易乐通";
        string password = "Ylt123456";
        Queue<string> terminalQueue = new Queue<string>();
        public frmMain()
        {
            InitializeComponent();
            CSharpBrowserSettings settings = new CSharpBrowserSettings();
            settings.DefaultUrl = "www.baidu.com";
            //settings.UserAgent = "Mozilla/5.0 (Linux; Android 4.2.1; en-us; Nexus 4 Build/JOP40D) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Mobile Safari/535.19";
            settings.CachePath = @"C:\temp\caches";
            chromeWebBrowser1.Initialize(settings);
            webBrowser1.Url =new Uri( Application.StartupPath + "\\main.html");
            webBrowser1.ObjectForScripting = this;
            
        }

        private string getNextTerminal()
        {
            if (terminalQueue.Count == 0) { FillQueue(); }
            return terminalQueue.Dequeue();
        }

        private void FillQueue()
        {
            terminalQueue.Enqueue("65886057");
            terminalQueue.Enqueue("65886058");
            terminalQueue.Enqueue("65886059");
            terminalQueue.Enqueue("65886060");
            terminalQueue.Enqueue("65886061");
            terminalQueue.Enqueue("65886062");
        }

        private void 载入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chromeWebBrowser1.OpenUrl("https://119.4.99.217:7300/mcrm/login.jsp");
        }

        private void 触发登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CwbElement buttons = chromeWebBrowser1.Document.GetElementById("submitButton");
            buttons.Click();
        }

        private void 跳转到ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadFrame();
            
        }

        private void loadFrame()
        {
              CwbElement leftMenuUl = chromeWebBrowser1.Document.GetElementById("resTree");
              if (leftMenuUl == null)
              {
                  MessageBox.Show("请先载入！");
                  return;
              }
             foreach(CwbElement ele in leftMenuUl.ChildElements){
                 if (ele.ChildElements[0].GetAttribute("node-id").Equals("8053"))
                 {
                     CwbElement subUl = ele.ChildElements[1];
                     foreach (CwbElement subEle in subUl.ChildElements)
                     {
                         if (subEle.ChildElements[0].GetAttribute("node-id").Equals("10051"))
                         {
                             subEle.ChildElements[0].Click();  
                         }
                     }
                 }
             }
        }

        System.Threading.Thread waitQueryThread;
        bool inQuery = false;

        private void 触发查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object frameUrl = chromeWebBrowser1.EvaluateScript("window.frames['收单日志']==null");
            if (frameUrl == null || frameUrl.Equals(true))
            {
                MessageBox.Show("请先跳转到收单日志");
                return;
            }
            timer1.Start();
            GoQuery();
            tabControl1.SelectedIndex = 1;
            触发查询ToolStripMenuItem.Enabled = false;
        }
        string termID,beginDate,endDate;
        private void GoQuery()
        {
            if (waitQueryThread != null && waitQueryThread.IsAlive) return;
            if (inQuery) return;
            inQuery = true;
            //CwbElement tabFrame = chromeWebBrowser1.Document.GetElementById("tab1");
            //if(tabFrame==null) return;
            object notFunction = chromeWebBrowser1.EvaluateScript("typeof( window.frames['收单日志'].queryByCondition) == 'undefined'");
            if (notFunction != null && notFunction.Equals(true))
            {
                MessageBox.Show("完蛋了，查询方法被修改了！");
            }
            else
            {

                termID = getNextTerminal();
                object date1 = chromeWebBrowser1.EvaluateScript(" $(window.frames['收单日志'].document).find('#beginstdate').next().find('.combo-value').val()");
                object date2 = chromeWebBrowser1.EvaluateScript(" $(window.frames['收单日志'].document).find('#endstdate').next().find('.combo-value').val()");
                if (date1!=null) beginDate = date1.ToString();
                else date1 = "找不到";
                if (date2 != null) endDate = date2.ToString();
                else endDate = "找不到";
               string js=@"
$(window.frames['收单日志'].document).find('#tid').val('" + termID + @"');
$(window.frames['收单日志'].document).find('.pagination-page-list:eq(0)').find('option:last').attr('selected',true);
window.frames['收单日志'].queryByCondition();";
                chromeWebBrowser1.ExecuteScript(js);
                //chromeWebBrowser1.ExecuteScript("");
                setStatus("正在查询" + termID + "的刷卡情况");
                waitQueryThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(watinQuery));
                waitQueryThread.Start();
            }
        }

        private void setStatus(string str)
        {
            object[] objects = new object[1];
            objects[0] =str;
            webBrowser1.Document.InvokeScript("setStatus", objects);
        }

        public void OnQueryFinish()
        {
            setStatus( termID+"终端"+beginDate+"到"+ endDate + "的刷卡情况:");
            object table= chromeWebBrowser1.EvaluateScript("$(window.frames['收单日志'].document).find('.datagrid-btable:eq(1)')[0].outerHTML");            
            HtmlElement hidden_div = webBrowser1.Document.GetElementById("hidden_div");
            hidden_div.InnerHtml = table.ToString();
            object[] objects = new object[1];             
            object jsonStr = webBrowser1.Document.InvokeScript("parseTableJson", objects);
//            if(jsonStr!=null) MessageBox.Show(jsonStr.ToString());
            inQuery = false;
        }

        public delegate void QueryFinish();
        private void watinQuery(object o)
        {
            while (true)
            {
               object isViable= chromeWebBrowser1.EvaluateScript("$(window.frames['收单日志'].document).find('.datagrid-mask-msg').is(':visible')");
               if (isViable != null && isViable.Equals(false))
               {
                   this.BeginInvoke(new QueryFinish(OnQueryFinish));
                   return;
               }
               else System.Threading.Thread.Sleep(100);
            }
        }

        private void chromeWebBrowser1_PageLoadFinishEventhandler(object sender, EventArgs e)
        {
            //if (chromeWebBrowser1.Document == null) return;
            //int pos = chromeWebBrowser1.Url.LastIndexOf("/");
            //string pageName = chromeWebBrowser1.Url.Substring(pos + 1).ToLower();
            //switch (pageName)
            //{
            //    case "login.jsp":
            //        //还要有登录失败的判断
            //        goLogin();
            //        break;
            //}
           // MessageBox.Show(pageName);
        }

        private void goLogin()
        {
            //if (chromeWebBrowser1.Document == null) return;
            //CwbElement userNameEle = chromeWebBrowser1.Document.GetElementById("username");
            //if (userNameEle!=null) userNameEle.Value = userName;

            //CwbElement passwordEle = chromeWebBrowser1.Document.GetElementById("password");
            //if(passwordEle!=null)passwordEle.Value = password;
            chromeWebBrowser1.ExecuteScript("$('#username').val('"+userName+"')");
            chromeWebBrowser1.ExecuteScript("$('#password').val('" + password + "')");
            chromeWebBrowser1.ExecuteScript("$('input[name=codeVal]').focus()");
             
        }

        private void chromeWebBrowser1_BrowserNavigated(object sender, EventArgs e)
        {
            
        }

        private void chromeWebBrowser1_BrowserDocumentCompleted(object sender, EventArgs e)
        {
            if (chromeWebBrowser1.Document == null) return;
            int pos = chromeWebBrowser1.Url.LastIndexOf("/");
            string pageName = chromeWebBrowser1.Url.Substring(pos + 1).ToLower();
            switch (pageName)
            {
                case "login.jsp":
                case "j_spring_security_check":
                    //还要有登录失败的判断
                    goLogin();
                    break;
                case "index":
                    string jsPath = "http://localhost:22163/scripts/main.js";//file:///" + Application.StartupPath.Replace("\\","/") + "/main.js";
                    string jsScript = @"$('<div></div>').attr('id','hidden_div').appendTo(document.body)";
                    //chromeWebBrowser1.ExecuteScript(" $(\"" + jsScript + "\")appendTo(document.body)");
                    //loadFrame();
                    chromeWebBrowser1.ExecuteScript(jsScript);
                    break;

            } 
        }

        private void chromeWebBrowser1_BrowserFrameLoadEnd(object sender, EventArgs e)
        {
            
        }

        private void chromeWebBrowser1_BrowserCreated(object sender, EventArgs e)
        {
            chromeWebBrowser1.LoadHtml("点击菜单 文件->载入");
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chromeWebBrowser1.ExecuteScript("test()");
        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (!inQuery) GoQuery();
        }

     
    }
}
