using System;
using System.Data;
using System.Collections.Generic;
namespace WinForm
{

	public interface IMonitor{
		void Start();
		/// <summary>
		/// 有客户端结算通知
		/// </summary>
		void OnNotify(string termid,bool runNow);
		/// <summary>
		/// 获取未上传代扣中心的数据
		/// </summary>
		DataTable GetData();
		/// <summary>
		/// 上传代扣中心
		/// </summary>
		void UploadData();
		void Stop();
	}

	public class BaseMonitor:IMonitor{
		System.Threading.Timer timer;
		//List<string> termList = new List<string>();

		public void Start(){
			if (timer != null)
				throw new Exception ("monitor already run");
			timer = new System.Threading.Timer (new System.Threading.TimerCallback (Do));
			timer.Change (-1, 100);
		}

		public void OnNotiry(string termid,bool runNow){
			if(runNow) timer.Change (-1, 100);
			else timer.Change (-1, 30000);
		}

		public void Do(object obj){
			try{
				
			}
			catch(Exception ex){
			}

		}

		public virtual DataTable GetData(){
			
		}

		public virtual void UpdateFlagOnUpload(DataRow dr){
			
		}

		public virtual void UploadData(DataTable dt){
			
		}
		public void Stop(){
		}
	}

	public class RongBaoMonitor:IMonitor
	{
		public RongBaoMonitor ()
		{
		}


	}
}

