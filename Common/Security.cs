
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Common
{

    public class SecurityClass
    {

        //创建了密钥对
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        //private string privateKey = RSA.ToXmlString(true);
        //private string publicKey = RSA.ToXmlString(false);
//        static byte[] AOutput;
        Encoding encoding;
        public string publicCer;

        public SecurityClass(Encoding encoding){
            this.encoding = encoding;
        }

        public void FromFile(string cerFile)
        {
            X509Certificate2 cert = new X509Certificate2(cerFile);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;            
            publicCer = cerFile;
        }

        public  string GetKeyFromContainer(string ContainerName, bool privatekey)
        {
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = ContainerName;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);
            return rsa.ToXmlString(privatekey);
        }

        /// <summary>
        /// 加密,支持大数据量
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string RSAEncrypt(string content)
        {
            if (string.IsNullOrEmpty(content)) throw new Exception("要加密的内容为空!");
            X509Certificate2 _X509Certificate2 = new X509Certificate2(publicCer);
            using (RSACryptoServiceProvider RSACryptography = _X509Certificate2.PublicKey.Key as RSACryptoServiceProvider)
            {
                Byte[] PlaintextData = encoding.GetBytes(content);
                int MaxBlockSize = RSACryptography.KeySize / 8 - 11;    //加密块最大长度限制

                if (PlaintextData.Length <= MaxBlockSize)
                    return Convert.ToBase64String(RSACryptography.Encrypt(PlaintextData, false));

                using (MemoryStream PlaiStream = new MemoryStream(PlaintextData))
                using (MemoryStream CrypStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);

                    while (BlockSize > 0)
                    {
                        Byte[] ToEncrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                        Byte[] Cryptograph = RSACryptography.Encrypt(ToEncrypt, false);
                        CrypStream.Write(Cryptograph, 0, Cryptograph.Length);

                        BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                    }

                    return Convert.ToBase64String(CrypStream.ToArray(), Base64FormattingOptions.None);
                }
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string RSADecrypt(string privateKey,string password, string content)
        {
            X509Certificate2 x509Certificate = new X509Certificate2(privateKey,password, X509KeyStorageFlags.Exportable);
            System.Security.Cryptography.RSAParameters parms = ((RSACryptoServiceProvider)x509Certificate.PrivateKey).ExportParameters(true);
            byte[] encryptedData = Convert.FromBase64String(content);
            byte[] array= RSADecrypt(encryptedData, parms);
            return encoding.GetString(array);
        }


        public byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo)
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportParameters(RSAKeyInfo);
                using (MemoryStream ms = new MemoryStream(DataToDecrypt.Length))
                {
                    byte[] buffer = new byte[128];
                    int pos = 0;
                    int copyLength = buffer.Length;
                    while (true)
                    {
                        Array.Copy(DataToDecrypt, pos, buffer, 0, copyLength);
                        pos += copyLength;
                        byte[] resp = RSA.Decrypt(buffer, false);
                        ms.Write(resp, 0, resp.Length);
                        Array.Clear(resp, 0, resp.Length);
                        Array.Clear(buffer, 0, copyLength);
                        if (pos >= DataToDecrypt.Length) break;
                    }
                    return ms.ToArray();

                }
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public byte[] GetHash(string content)
        {          
                //FileStream objFile = File.OpenRead(filePath);
                byte[] data_Bytes = encoding.GetBytes(content);
                HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
                byte[] Hashbyte = MD5.ComputeHash(data_Bytes);                
                //objFile.Close();            
                return Hashbyte;          
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string MD5(string content)
        {
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] fromData = encoding.GetBytes(content);
			byte[] targetData = md5.ComputeHash(fromData);

			return ConvertBytesToString(targetData);
        }
        

        //获取文件hash
        public byte[] GetFileHash(string filePath)
        {
            try
            {
                FileStream objFile = File.OpenRead(filePath);
                HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
                byte[] Hashbyte = MD5.ComputeHash(objFile);
                objFile.Close();
                return Hashbyte;
            }
            catch
            {
                return null;
            }
        }

         /// <summary>
        /// 创建数字签名
         /// </summary>
         /// <param name="content"></param>
         /// <returns></returns>
        public byte[] EncryptHash(string content)
        {
            byte[] hash_Bytes = GetHash(content);
            return EncryptHash(hash_Bytes);
        }
        //创建数字签名
        public byte[] EncryptHash(byte[] hash_Bytes)
        {
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(rsa);
            RSAFormatter.SetHashAlgorithm("MD5");
            //AOutput = Rsa3.SignData(data_Bytes, "SHA1");
            return RSAFormatter.CreateSignature(hash_Bytes); ;
        }

        //验证数字签名
        public bool DecryptHash(string publicKey, byte[] hash_byte, byte[] eSignature)
        {
            try
            {
                RSACryptoServiceProvider Rsa4 = new RSACryptoServiceProvider();
                Rsa4.FromXmlString(publicKey);
            
                //bool bVerify = Rsa4.VerifyData(strData, "SHA1", AOutput);
                RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(Rsa4);
                RSADeformatter.SetHashAlgorithm("MD5");

                bool bVerify = RSADeformatter.VerifySignature(hash_byte,eSignature);

                if (bVerify)
                {
                    return true;
                }
                return false;
            }
            catch (CryptographicException e)
            {
                return false;
            }
        }

        // 将Byte[]转换成十六进制字符串
        public string ConvertBytesToString(byte[] bytes)
        {
            string bytestring = string.Empty;
            if (bytes != null && bytes.Length > 0)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytestring += bytes[i].ToString("x2");
                }
            }
            return bytestring;
        }

        //static void Main(string[] args)
        //{
        //    //打开文件，或者是获取网页数据
        //    Console.WriteLine("接收方收到电子文件。");
        //    string strMsg = "dadfdfgdgdgagdgadg";  //test data
        //    Console.WriteLine(strMsg);
        //    Class1 cls = new Class1();


        //    //对电子文件进行哈希
        //    Console.WriteLine("哈希值：");
        //    byte[] hash_data = cls.GetHash(strMsg);
        //    Console.WriteLine(ConvertBytesToString(hash_data));

        //    //创建数据签名
            
        //    Console.WriteLine("私钥：");
        //    Console.WriteLine(cls.privateKey);

        //    Console.WriteLine("用私钥进行数字签名");
        //    byte[] eSingnature = cls.EncryptHash(cls.privateKey,hash_data);
        //    Console.WriteLine(ConvertBytesToString(eSingnature));
      
        //    //测试数据
        //    string strMsgCopy = string.Copy(strMsg);
        //    Console.WriteLine("被验证的哈希值：");
        //    byte[] hash_dataCopy = cls.GetHash(strMsgCopy);
        //    Console.WriteLine(ConvertBytesToString(hash_dataCopy));

        //    Console.WriteLine("公钥：");
        //    Console.WriteLine(cls.publicKey);


        //    if (cls.DecryptHash(cls.publicKey, hash_dataCopy, eSingnature))
        //    {
        //        Console.WriteLine("通过验证，电子文件合法有效。");
        //    }

        //    else
        //    {
        //        Console.WriteLine("未通过验证，电子文件非法或被人篡改过。");

        //    }

        //    Console.ReadLine();
        //    //Class1 cls = new Class1();
        //    //string filePath = "D://公文.txt";
        //    //StreamWriter sw = File.CreateText(filePath);
        //    //sw.Write("测试公文");
        //    //sw.Close();


        //    //byte[] fileHash = GetFileHash(filePath);
        //    //string publicKey = GetKeyFromContainer("公文", false);
        //    //string privateKey = GetKeyFromContainer("公文", true);

        //    //byte[] ElectronicSignature = cls.EncryptHash(privateKey, fileHash);

        //    //string fileCopyPath = "D://公文接收.txt";
        //    //File.Copy(filePath, fileCopyPath, true);
        //    //byte[] fileCopyHash = GetFileHash(fileCopyPath);
        //    //if (cls.DecryptHash(publicKey, fileCopyHash, ElectronicSignature)) 
        //    //{ 
        //    //    Console.WriteLine("通过验证，电子文件合法有效。"); 
        //    //} 
        //    //else 
        //    //{ 
        //    //    Console.WriteLine("未通过验证，电子文件非法或被人篡改过。"); 
        //    //}

        //    //Console.Read();     
        //}
        
    }
}