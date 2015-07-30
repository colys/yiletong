using System;
using System.Collections.Generic;
using System.Xml;

namespace Common
{
	public class QueryResult
	{
		public QueryResult ()
		{
			
		}

		public static QueryResult FromXml(string xmlResult){
			string xml= xmlResult.Replace("<Resp>","<QueryResult>").Replace("</Resp>","</QueryResult>");
			xml = @"<?xml version='1.0'?> 
			" + xml;
			System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(QueryResult) );
			//System.Xml.XmlReader reader = System.Xml.XmlReader.Create (new StringReader(xml));
			object result = xmlSerializer.Deserialize (new System.IO.StringReader (xml));
			//reader.Close ();
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xmlResult);
			XmlNodeList xmlNodes = xmlDoc.SelectNodes ("Resp/batchContent/detailInfo");
			List<string> lst = new List<string> ();
			foreach (XmlNode node in xmlNodes) {
				lst.Add (node.InnerText);
			}
			QueryResult qr= result as QueryResult;
			qr.batchContent = lst;
			return qr;
		}

		public string _input_charset;

		public string batchBizid;

		public string batchVersion;

		public string batchDate;

		public string batchCurrnum;

		public int batchStatus;

		List<string> _batchContent;

		public List<string> batchContent {
			get { return _batchContent; }
			set {
				_batchContent = value;
				if (value == null)
					return;
				batchEContent = new List<DetailInfo> ();
				foreach (string str in value) {
					string[] arr = str.Split (',');
					if (arr.Length < 8) throw new Exception ("batchContent字符串不对");
					DetailInfo info = new DetailInfo () {
						tradeNum = arr [0],
						faren = arr [2],
						money = arr [7],
						status = arr [arr.Length - 2],
						reason = arr [arr.Length - 1]
					};
					batchEContent.Add (info);
				}
			}
		}

		public List<DetailInfo> batchEContent{ get; set; }

		public string sign;

		public class DetailInfo{
			public string tradeNum;
			public string faren;
			public string money;
			public string reason;
			public string status;
		}
	}


}

