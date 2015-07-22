using System;
using System.Collections.Generic;

namespace Common
{
	public class QueryResult
	{
		public QueryResult ()
		{
			
		}

		public static QueryResult FromXml(string xml){
			xml= xml.Replace("<Resp>","<QueryResult>").Replace("</Resp>","</QueryResult>");
			xml = @"<?xml version='1.0'?> 
			" + xml;
			System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(QueryResult) );
			//System.Xml.XmlReader reader = System.Xml.XmlReader.Create (new StringReader(xml));
			object result = xmlSerializer.Deserialize (new System.IO.StringReader (xml));
			//reader.Close ();
			return result as QueryResult;
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
				foreach (string str in value) {
					string[] arr = str.Split (',');
					if (arr.Length < 8) throw new Exception ("batchContent字符串不对");
					DetailInfo info = new DetailInfo () {
						tradeNum = arr [0],
						faren = arr [2],
						money = arr [6],
						status = arr [arr.Length - 2],
						reason = arr [arr.Length - 1]
					};
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

