using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Net;
using System.Data;
using System.Xml;

namespace Common.RongBao
{
 
    public class RSACryptionClass
    {
        string signType = "MD5";
        string key = "gf5663d1994f8gecgca4050776bc3813cg0c3d607ac397564d65195fg7061gg2";
        string batchBizid = "100000000056373";
        string _input_charset = "gbk";
        string batchBiztype = "00000";
        
        string batchVersion = "00";
        Encoding encoding;

    
        public RSACryptionClass(string publicCer,string privateKey)
        {
            PubliCertificate = publicCer;
            this.PrivateKey = privateKey;
            encoding = Encoding.GetEncoding(_input_charset);
        }
        public string PubliCertificate { get; set; }
        public string PrivateKey { get; set; }
        //id,bankAccount,faren,bankName,bankName2,finallyMoney,province,city,phone
		public string Sent(DataTable dt,string batchCurrnum,string batchDate)
        {
			
            int batchCount = dt.Rows.Count;
            float batchAmount = 0;
            string batchContent="";
            int rowIndex = 0;
			foreach (DataRow dr in dt.Rows) {
				float money = Convert.ToSingle(dr["money"]);
				batchAmount += money;
				rowIndex++;
				//序号,银行账户,开户名,开户行,分行,支行,公/私,金额,币种,省,市,手机号,证件类型,证件号,用户协议号,商户订单号,备注
				if (rowIndex > 1)
					batchContent += "|";
                batchContent += dr["id"] + "," + dr["bankAccount"] + "," + dr["faren"] + "," + dr["bankName"] + "," + dr["bankName2"] + "," + dr["bankName3"] + ",私," + money.ToString("f2") + ",CNY," + dr["province"] + "," + dr["city"] + "," + dr["tel"] + "," + "身份证,,,,,";

			}

            string easypay_url = FormatUrl(new
            {
                batchBizid = batchBizid,
                _input_charset = _input_charset,
                batchBiztype = batchBiztype,
                batchDate = batchDate,
                batchVersion = batchVersion,
                batchCurrnum = batchCurrnum,
                batchCount = batchCount,
                batchAmount = batchAmount,
                batchContent = batchContent
            });

            string returnPayValue = PostDataGetHtml(easypay_url);
            CheckException(returnPayValue);
            return returnPayValue;
        }



        public QueryResult TryGetResult(string batchCurrnum, string batchDate, out string returnPayValue)
        {
			QueryResult qr = null;
            string easypay_url = FormatUrl(new
            {                
                _input_charset = _input_charset,
                batchBizid = batchBizid,                
                batchCurrnum = batchCurrnum,
                batchDate = batchDate,   
                batchVersion = batchVersion
            });
            MyHttpUtility http = new MyHttpUtility();
            returnPayValue = http.DoGet("http://entrust.reapal.com/agentpay/payquerybatch?" + easypay_url,false);
            if (returnPayValue.Substring(0, 6).ToUpper() == "<RESP>") {
				CheckException(returnPayValue);
            }
            else
            {
                //解密
                SecurityClass security = new SecurityClass(Encoding.GetEncoding("GBK"));
                returnPayValue = security.RSADecrypt(PrivateKey, "clientok", returnPayValue);
				qr= Common.QueryResult.FromXml(returnPayValue);
            }  
			return qr;
        }

        private void CheckException(string returnPayValue)
        {
            if (returnPayValue == null)
            {
                throw new Exception("与融宝通信异常，返回值为空");
            }
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(returnPayValue);
            XmlNode xmlNode = xml.SelectSingleNode("Resp/status");
            if (xmlNode!=null && xmlNode.InnerText == "fail")
            {
                XmlNode reasonNode = xml.SelectSingleNode("Resp/reason");
                //失败
                throw new Exception("融宝返回失败信息：" + reasonNode.InnerText);
            }
        }

        private string FormatUrl(object o)
        {
            SecurityClass security = new SecurityClass(encoding);            
            security.FromFile(PubliCertificate );
            string notEncryText="",encryText="";
            System.Collections.Generic.Dictionary<string, string> dic = new Dictionary<string, string>();            
            foreach (System.Reflection.PropertyInfo p in o.GetType().GetProperties())
            {
                dic.Add(p.Name, p.GetValue(o, null).ToString());
            }
            dic.OrderBy(x => x.Key).ThenBy(x => x.Value);
            foreach (KeyValuePair<string, string> kv in dic)
            {
                notEncryText += "&" + kv.Key + "=" + kv.Value;
                string txt = kv.Value;
                if (kv.Key == "batchContent") txt = System.Web.HttpUtility.UrlEncode(security.RSAEncrypt(txt));
                encryText += "&" + kv.Key + "=" + txt;
            }
            notEncryText = notEncryText.Substring(1);            
			string sign = security.MD5(notEncryText.Trim('&')+key);
            encryText = string.Format("sign={0}&signType={1}", sign, signType) + encryText;
            return encryText;            
        }


        //post提交
        public string PostDataGetHtml(string postData)
        {
            byte[] data = encoding.GetBytes(postData);
            //Uri uRI = new Uri("http://entrust.reapal.com/agentpay/pay?");
            MyHttpUtility http = new MyHttpUtility();
            return http.DoPost("http://entrust.reapal.com/agentpay/pay?", data);

            //HttpWebRequest req = WebRequest.Create(uRI) as HttpWebRequest;
            //req.Method = "POST";
            //req.KeepAlive = true;
            //req.ContentType = "application/x-www-form-urlencoded";
            //req.ContentLength = data.Length;
            //req.AllowAutoRedirect = true;
            //Stream outStream = req.GetRequestStream();
            //outStream.Write(data, 0, data.Length);
            //outStream.Close();
            //HttpWebResponse res = req.GetResponse() as HttpWebResponse;
            //Stream inStream = res.GetResponseStream();
            //StreamReader sr = new StreamReader(inStream, System.Text.Encoding.Default);
            //string htmlResult = sr.ReadToEnd();
            //return htmlResult;

        }


            //ras 加密部分
        public string RSAEncryption(string tbData)
        {

            byte[] dataReturn = null;

            byte[] textBytes = Encoding.GetEncoding("GBK").GetBytes(tbData);
            List<byte> lst = new List<byte>();

            X509Certificate2 cert = new X509Certificate2(PubliCertificate);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;
            
            foreach (byte b in textBytes)
            {
                lst.Add(b);
            }

            for (int i = 0; i < lst.Count; i += 100)
            {
                byte[] enData = rsa.Encrypt(lst.GetRange(i, (i + 100 > lst.Count ? lst.Count - i : i + 100)).ToArray(), false);
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




        #region 加密解密demo
        ////公开金钥加密
        //private void btnEncrypt_Click(object sender, EventArgs e)
        //{
        //    //txtPublicKeyPath.Text是公开金钥的档案路径
        //    X509Certificate2 pubcrt = new X509Certificate2(txtPublicKeyPath.Text.Trim());
        //    RSACryptoServiceProvider pubkey = (RSACryptoServiceProvider)pubcrt.PublicKey.Key;
        //    //txtBody.Text是要加密的内容
        //    byte[] orgData = Encoding.UTF8.GetBytes(txtBody.Text.Trim());
        //    //公开金钥加密
        //    byte[] encryptedData = RSAEncrypt(orgData, pubkey.ExportParameters(false), false);
        //    //将加密过的内容以Base64转成字符串
        //    txtEncryptBody.Text = Convert.ToBase64String(encryptedData);
        //}

        // //私密金钥解密
        //private void btnDecrypt_Click(object sender, EventArgs e)
        //{
            ////txtPrivateKeyPath.Text是私密金钥解密的档案路径
            //// txtPrivateKeyPass<a href="http://www.it165.net/edu/ebg/" target="_blank" class="keylink">word</a>.Text是私密金钥解密的密码
            //X509Certificate2 prvcrt = new X509Certificate2(txtPrivateKeyPath.Text.Trim(), 
            ////    txtPrivateKeyPass<a href="http://www.it165.net/edu/ebg/" target="_blank" class="keylink">word</a>.Text, X509KeyStorageFlags.Exportable);
            //RSACryptoServiceProvider prvkey = (RSACryptoServiceProvider)prvcrt.PrivateKey;
            ////将加密过的内容从Base64字符串转成Byte Array
            //byte[] encryptedData = Convert.FromBase64String(txtEncryptBody.Text);
            //System.Security.Cryptography.RSAParameters parms = prvkey.ExportParameters(true);
            ////私密金钥解密
            //byte[] decryptedData = RSADecrypt(encryptedData, parms, false);
            ////将解密出来的内容转成字符串
            //txtDecryptBody.Text = Encoding.UTF8.GetString(decryptedData);
        //}
        #endregion

        #region RSA证书加密解密(支持大数据量)
        //The key size to use maybe 1024/2048
        private const int _EncryptionKeySize = 1024;

        // The buffer size to decrypt per set
        private const int _DecryptionBufferSize = (_EncryptionKeySize / 8);

        //The buffer size to encrypt per set
        private const int _EncryptionBufferSize = _DecryptionBufferSize - 11;

        static public byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                //byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This only needs www.it165.net
                    //toinclude the public key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    ////Encrypt the passed byte array and specify OAEP padding.  
                    ////OAEP padding is only available on Microsoft Windows XP or
                    ////later.  
                    //encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                    //2012/10/19 rm 改用block
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[_EncryptionBufferSize];
                        int pos = 0;
                        int copyLength = buffer.Length;
                        while (true)
                        {
                            //Check if the bytes left to read is smaller than the buffer size, 
                            //then limit the buffer size to the number of bytes left

                            if (pos + copyLength > DataToEncrypt.Length)

                                copyLength = DataToEncrypt.Length - pos;

                            //Create a new buffer that has the correct size

                            buffer = new byte[copyLength];

                            //Copy as many bytes as the algorithm can handle at a time, 
                            //iterate until the whole input array is encoded

                            Array.Copy(DataToEncrypt, pos, buffer, 0, copyLength);

                            //Start from here in next iteration

                            pos += copyLength;

                            //Encrypt the data using the public key and add it to the memory buffer

                            //_DecryptionBufferSize is the size of the encrypted data

                            ms.Write(RSA.Encrypt(buffer, false), 0, _DecryptionBufferSize);

                            //Clear the content of the buffer, 
                            //otherwise we could end up copying the same data during the last iteration

                            Array.Clear(buffer, 0, copyLength);

                            //Check if we have reached the end, then exit

                            if (pos >= DataToEncrypt.Length)

                                break;
                        }
                        return ms.ToArray();
                    }
                }
                //return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }

        }

        static public byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                //byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    //decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                    using (MemoryStream ms = new MemoryStream(DataToDecrypt.Length))
                    {

                        //The buffer that will hold the encrypted chunks

                        byte[] buffer = new byte[_DecryptionBufferSize];

                        int pos = 0;

                        int copyLength = buffer.Length;

                        while (true)
                        {

                            //Copy a chunk of encrypted data / iteration

                            Array.Copy(DataToDecrypt, pos, buffer, 0, copyLength);

                            //Set the next start position

                            pos += copyLength;

                            //Decrypt the data using the private key

                            //We need to store the decrypted data temporarily because we don't know the size of it; 
                            //unlike with encryption where we know the size is 128 bytes. 
                            //The only thing we know is that it's between 1-117 bytes

                            byte[] resp = RSA.Decrypt(buffer, false);

                            ms.Write(resp, 0, resp.Length);

                            //Cleat the buffers

                            Array.Clear(resp, 0, resp.Length);

                            Array.Clear(buffer, 0, copyLength);

                            //Are we ready to exit?

                            if (pos >= DataToDecrypt.Length)

                                break;

                        }

                        //Return the decoded data

                        return ms.ToArray();

                    }
                }
                //return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }

        }

        #endregion


        ///// <summary>
        ///// 解密
        ///// </summary>
        ///// <param name="str"></param>
        ///// <returns></returns>
        //private static string RSADecrypt(string str)
        //{
        //    X509Certificate2 c = RSAHelper.GetCertificateFromPfxFile(reaPalPrivateKeyPath, reaPalPassword);//reaPalPrivateKeyPath,reaPalPassword 自己设置

        //    string keyPrivate = c.PrivateKey.ToXmlString(true);  // 私钥
        //    return RSAHelper.RSADecrypt(keyPrivate, str);// 解密
        //}

        // 引用的两个方法
        /// <summary>   
        /// 根据私钥证书得到证书实体，得到实体后可以根据其公钥和私钥进行加解密   
        /// 加解密函数使用DEncrypt的RSACryption类   
        /// </summary>   
        /// <param name="pfxFileName"></param>   
        /// <param name="password"></param>   
        /// <returns></returns>   
        public static X509Certificate2 GetCertificateFromPfxFile(string pfxFileName,
            string password)
        {
            try
            {
                return new X509Certificate2(pfxFileName, password, X509KeyStorageFlags.Exportable);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static string RSADecrypt(string xmlPrivateKey, string m_strDecryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPrivateKey);
            byte[] rgb = Convert.FromBase64String(m_strDecryptString);
            byte[] bytes = provider.Decrypt(rgb, false);
            return new UnicodeEncoding().GetString(bytes);
        }
     
    }  
    
    
}
