﻿namespace WinForm
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.跳转到ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.触发查询ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sqlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.载入ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.系统ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transactionlogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.结算记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.客户列表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.节假日ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chromeWebBrowser1 = new Sashulin.ChromeWebBrowser();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.myPage = new System.Windows.Forms.TabPage();
            this.myBrowser = new System.Windows.Forms.WebBrowser();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer_pay = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.myPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.测试ToolStripMenuItem,
            this.文件ToolStripMenuItem,
            this.系统ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(783, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 测试ToolStripMenuItem
            // 
            this.测试ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.跳转到ToolStripMenuItem,
            this.触发查询ToolStripMenuItem,
            this.testToolStripMenuItem,
            this.sqlToolStripMenuItem});
            this.测试ToolStripMenuItem.Name = "测试ToolStripMenuItem";
            this.测试ToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.测试ToolStripMenuItem.Text = "测试";
            // 
            // 跳转到ToolStripMenuItem
            // 
            this.跳转到ToolStripMenuItem.Name = "跳转到ToolStripMenuItem";
            this.跳转到ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.跳转到ToolStripMenuItem.Text = "跳转到";
            this.跳转到ToolStripMenuItem.Click += new System.EventHandler(this.跳转到ToolStripMenuItem_Click);
            // 
            // 触发查询ToolStripMenuItem
            // 
            this.触发查询ToolStripMenuItem.Name = "触发查询ToolStripMenuItem";
            this.触发查询ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.触发查询ToolStripMenuItem.Text = "开始监控";
            this.触发查询ToolStripMenuItem.Click += new System.EventHandler(this.触发查询ToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.testToolStripMenuItem.Text = "test";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // sqlToolStripMenuItem
            // 
            this.sqlToolStripMenuItem.Name = "sqlToolStripMenuItem";
            this.sqlToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.sqlToolStripMenuItem.Text = "sql";
            this.sqlToolStripMenuItem.Click += new System.EventHandler(this.sqlToolStripMenuItem_Click);
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.载入ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 载入ToolStripMenuItem
            // 
            this.载入ToolStripMenuItem.Name = "载入ToolStripMenuItem";
            this.载入ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.载入ToolStripMenuItem.Text = "载入";
            this.载入ToolStripMenuItem.Click += new System.EventHandler(this.载入ToolStripMenuItem_Click);
            // 
            // 系统ToolStripMenuItem
            // 
            this.系统ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transactionlogToolStripMenuItem,
            this.结算记录ToolStripMenuItem,
            this.客户列表ToolStripMenuItem,
            this.节假日ToolStripMenuItem});
            this.系统ToolStripMenuItem.Name = "系统ToolStripMenuItem";
            this.系统ToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.系统ToolStripMenuItem.Text = "系统";
            // 
            // transactionlogToolStripMenuItem
            // 
            this.transactionlogToolStripMenuItem.Name = "transactionlogToolStripMenuItem";
            this.transactionlogToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.transactionlogToolStripMenuItem.Text = "交易查询";
            this.transactionlogToolStripMenuItem.Click += new System.EventHandler(this.添加客户ToolStripMenuItem_Click);
            // 
            // 结算记录ToolStripMenuItem
            // 
            this.结算记录ToolStripMenuItem.Name = "结算记录ToolStripMenuItem";
            this.结算记录ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.结算记录ToolStripMenuItem.Text = "结算记录";
            this.结算记录ToolStripMenuItem.Click += new System.EventHandler(this.结算记录ToolStripMenuItem_Click);
            // 
            // 客户列表ToolStripMenuItem
            // 
            this.客户列表ToolStripMenuItem.Name = "客户列表ToolStripMenuItem";
            this.客户列表ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.客户列表ToolStripMenuItem.Text = "客户列表";
            this.客户列表ToolStripMenuItem.Click += new System.EventHandler(this.客户列表ToolStripMenuItem_Click);
            // 
            // 节假日ToolStripMenuItem
            // 
            this.节假日ToolStripMenuItem.Name = "节假日ToolStripMenuItem";
            this.节假日ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.节假日ToolStripMenuItem.Text = "节假日";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.myPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 1;
            this.tabControl1.Size = new System.Drawing.Size(783, 331);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chromeWebBrowser1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage1.Size = new System.Drawing.Size(775, 305);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "现代金控";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chromeWebBrowser1
            // 
            this.chromeWebBrowser1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.chromeWebBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chromeWebBrowser1.Location = new System.Drawing.Point(2, 2);
            this.chromeWebBrowser1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chromeWebBrowser1.Name = "chromeWebBrowser1";
            this.chromeWebBrowser1.Size = new System.Drawing.Size(771, 301);
            this.chromeWebBrowser1.TabIndex = 1;
            this.chromeWebBrowser1.BrowserCreated += new System.EventHandler(this.chromeWebBrowser1_BrowserCreated);
            this.chromeWebBrowser1.BrowserNavigated += new System.EventHandler(this.chromeWebBrowser1_BrowserNavigated);
            this.chromeWebBrowser1.BrowserDocumentCompleted += new System.EventHandler(this.chromeWebBrowser1_BrowserDocumentCompleted);
            this.chromeWebBrowser1.BrowserFrameLoadEnd += new System.EventHandler(this.chromeWebBrowser1_BrowserFrameLoadEnd);
            this.chromeWebBrowser1.PageLoadFinishEventhandler += new System.EventHandler(this.chromeWebBrowser1_PageLoadFinishEventhandler);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.webBrowser1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage2.Size = new System.Drawing.Size(775, 310);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "监控中心";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(2, 2);
            this.webBrowser1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(13, 12);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(771, 306);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // myPage
            // 
            this.myPage.Controls.Add(this.myBrowser);
            this.myPage.Location = new System.Drawing.Point(4, 22);
            this.myPage.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.myPage.Name = "myPage";
            this.myPage.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.myPage.Size = new System.Drawing.Size(775, 310);
            this.myPage.TabIndex = 2;
            this.myPage.Text = "业务区";
            this.myPage.UseVisualStyleBackColor = true;
            // 
            // myBrowser
            // 
            this.myBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myBrowser.Location = new System.Drawing.Point(2, 2);
            this.myBrowser.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.myBrowser.MinimumSize = new System.Drawing.Size(13, 12);
            this.myBrowser.Name = "myBrowser";
            this.myBrowser.Size = new System.Drawing.Size(771, 306);
            this.myBrowser.TabIndex = 1;
            this.myBrowser.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // timer_pay
            // 
            this.timer_pay.Interval = 18000;
            this.timer_pay.Tick += new System.EventHandler(this.timer_pay_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 355);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmMain";
            this.Text = "福州易乐通信息有限公司";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.myPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 测试ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 跳转到ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 载入ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 触发查询ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 系统ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transactionlogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 客户列表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage myPage;
        private System.Windows.Forms.WebBrowser myBrowser;
        private Sashulin.ChromeWebBrowser chromeWebBrowser1;
        private System.Windows.Forms.ToolStripMenuItem 结算记录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 节假日ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sqlToolStripMenuItem;
        private System.Windows.Forms.Timer timer_pay;
    }
}