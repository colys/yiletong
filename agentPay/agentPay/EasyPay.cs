using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace agentPay
{
    class EasyPay
    {
        //融宝公钥
        static string reapal_cer = ConfigurationSettings.AppSettings["tomcatcer"].ToString();
        //商户私钥
        static string merchant_p12 = ConfigurationSettings.AppSettings["clientokp12"].ToString();

        static void Main(string[] args)
        {
            //协议类参数
            string signType = "MD5";
            string batchBizid = "100000000001000";	//商户号
            string _input_charset = "gbk";
            string batchBiztype = "00000";
            string batchDate = DateTime.Now.ToString("yyyyMMdd");
            string batchVersion = "00";
            int RandomNum;
            

            //业务类参数
            Random MyRandom = new Random();
            RandomNum = MyRandom.Next(1001, 9999);
            string batchCurrnum = DateTime.Now.ToString("yyyyMMddHHmmss") + RandomNum;  //批次号
            
            //批次明细
            string Num = DateTime.Now.ToString("yyyyMMddHHmmss");       //序号
            string RealName = "测试";                                   //姓名
            string Phone = "13800138000";                               //电话
            string CardNum = "6228910000000000000";                     //卡号
            string IdNum = "110110197802180789";                        //身份证号
            string Bank = "工商银行";                                   //银行
            string Money = "0.01";                                      //金额

            //                   序号,        银行账户,       开户名,         开户行,   分行,支行,公/私, 金额,     币种,省,  市,    手机号，  证件类型， 证件号，用户协议号，商户订单号，备注
            string batchContent = Num + "," + CardNum + "," + RealName + "," + Bank + ",分行,支行,私," + Money + ",CNY,北京,北京," + Phone + ",身份证," + IdNum + ",,,";
            batchContent += "|"+Num + "," + CardNum + "," + RealName + "," + Bank + ",分行,支行,私," + Money + ",CNY,北京,北京," + Phone + ",身份证," + IdNum + ",,,";  
              
            //生成签名
            string easypay_url = EasyPay.CreatPayUrlto(
                signType,
                batchBizid,
                _input_charset,
                batchBiztype,
                batchDate,
                batchVersion,
                batchCurrnum,
                batchContent
            );

            string  newEasypay_url = "";
            string[] patten1 = easypay_url.Split('&');
            for(int i = 0; i < patten1.Length;i++ )
            { 
                if (patten1[i].IndexOf("batchContent") ==-1)
                {
                    if (i == 0)
                    {
                        newEasypay_url +=patten1[i];
                    }
                    else 
                    {
                        newEasypay_url += "&" + patten1[i];
                    }
                }
                else
                {
                    newEasypay_url += "&batchContent=" + System.Web.HttpUtility.UrlEncode(EasyPay.RSAEncryption(batchContent), System.Text.Encoding.Default);
                } 
            }
            string returnPayValue = PostDataGetHtml(newEasypay_url);
            if (returnPayValue.IndexOf("fail") > -1)
            {
				Console.WriteLine("失败");
            }else
			{
                Console.WriteLine("成功");
			}
	    }

        public static string CreatPayUrlto(string signType,string batchBizid,string _input_charset,string batchBiztype,string batchDate,string batchVersion,string batchCurrnum,string batchContent) {
            string user_key = "1ca781f111bfccb591285b70a3g123c99bd0685dce0d4375ba972f8a7gd85571";  //商户的key
            string strUrl = "";
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            dic.Add("signType", signType);
            dic.Add("batchBizid", batchBizid);
            dic.Add("_input_charset", _input_charset);
            dic.Add("batchBiztype", batchBiztype);
            dic.Add("batchDate", batchDate);
            dic.Add("batchVersion", batchVersion);
            dic.Add("batchCurrnum", batchCurrnum);
            dic.Add("batchContent", batchContent);

            //生成签名
            string signStr = GetSign(dic, user_key);
            //重新组装数据
            dic.Add("sign", signStr);

            Console.WriteLine(signStr);

            foreach (var item in dic)
            {
                strUrl += item.Key + "=" + item.Value + "&";
            }

            return strUrl.Substring(0, strUrl.Length-1);
        }

        public static string GetSign(SortedDictionary<string, string> dic, string key)
        {
            string sign = "";
            foreach (var item in dic)
            {
                sign += item.Key + "=" + item.Value + "&";
            }
            Console.WriteLine(sign.Trim('&'));

            //加密Md5
            return EasyPay.GetMD5(sign.Trim('&') + key);
        }

        public static string GetMD5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }

            return byte2String;
        }

        //post提交
        public static string PostDataGetHtml(string postData)
        {
            try
            {
                byte[] data = Encoding.Default.GetBytes(postData);
                Uri uRI = new Uri("http://entrust.reapal.com/agentpay/pay?");
                HttpWebRequest req = WebRequest.Create(uRI) as HttpWebRequest;
                req.Method = "POST";
                req.KeepAlive = true;
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = data.Length;
                req.AllowAutoRedirect = true;

                Stream outStream = req.GetRequestStream();
                outStream.Write(data, 0, data.Length);
                outStream.Close();

                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                Stream inStream = res.GetResponseStream();
                StreamReader sr = new StreamReader(inStream, System.Text.Encoding.Default);
                string htmlResult = sr.ReadToEnd();

                return htmlResult;
            }
            catch (Exception ex)
            {
                return "网络错误：" + ex.Message.ToString();
            }
        }







        //ras 加密部分
        public static string RSAEncryption(string tbData)
        {

            byte[] dataReturn = null;

            byte[] textBytes = Encoding.GetEncoding("GBK").GetBytes(tbData);
            List<byte> lst = new List<byte>();
            X509Certificate2 cert = new X509Certificate2(reapal_cer);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;

            foreach (byte b in textBytes)
            {
                lst.Add(b);
            }

            for (int i = 0; i < lst.Count; i += 100)
            {
                byte[] enData = rsa.Encrypt(lst.GetRange(i, (i + 100 > lst.Count ? lst.Count - i : 100)).ToArray(), false);
                if (dataReturn != null)
                {
                    dataReturn = dataReturn.Concat(enData).ToArray();
                }
                else
                {
                    dataReturn = enData;
                }
            }
            return Convert.ToBase64String(dataReturn);

        }

    }
}
