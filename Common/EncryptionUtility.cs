using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    public class EncryptionUtility
    {
        string filePath;
        byte[] SecretKey;
        private bool _ProtectKey;//是否需要使用DPAPI来保护密钥；  
        private string _AlgorithmName = "RC2";//加密算法的名称；  

        public EncryptionUtility(string file)
        {
            filePath = file;
        }

        public string AlgorithmName
        {
            get { return _AlgorithmName; }
            set { _AlgorithmName = value; }
        }

        public bool ProtectKey
        {
            get { return _ProtectKey; }
            set { _ProtectKey = value; }
        }

        /// <summary>  
        ///【根据加密算法类来生成密钥文件】；  
        /// </summary>  
        /// <param name="targetFile">保存密钥的文件</param>  
        public void GenerateKey(string targetFile)
        {
            //创建加密算法；  
            SymmetricAlgorithm Algorithm = SymmetricAlgorithm.Create(AlgorithmName);
            Algorithm.GenerateKey();//当在派生类中重写时，生成用于该算法的随机密钥；  

            //得到密钥；  
            byte[] Key = Algorithm.Key;

            if (ProtectKey)
            {
                //使用DPAPI来加密密钥；  
                Key = ProtectedData.Protect(Key, null, DataProtectionScope.LocalMachine);
            }

            //把密钥保存到key.config。  
            using (FileStream fs = new FileStream(targetFile, FileMode.Create))
            {
                fs.Write(Key, 0, Key.Length);
				fs.Close ();
            }
        }

        /// <summary>  
        /// 【从密钥文件中读取密钥】；  
        /// </summary>  
        /// <param name="algorithm">加密的算法</param>  
        /// <param name="keyFile">密钥文件路径</param>  
        private void ReadKey(SymmetricAlgorithm algorithm )
        {
            if (SecretKey == null)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    SecretKey = new byte[fs.Length];
                    fs.Read(SecretKey, 0, (int)fs.Length);
					fs.Close ();
                }
            }
            if (ProtectKey)
                algorithm.Key = ProtectedData.Unprotect(SecretKey, null, DataProtectionScope.LocalMachine);
            else
                algorithm.Key = SecretKey;            
        }
     

        /// <summary>  
        /// 【加密数据】  
        /// </summary>  
        /// <param name="data">原始数据</param>  
        /// <param name="keyFile">密钥文件路径</param>  
        /// <returns></returns>  
        public byte[] EncryptData(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            //将要加密的字符串数据转换为字符数组；  
            byte[] ClearData = Encoding.UTF8.GetBytes(data);

            //创建加密算法；  
            SymmetricAlgorithm Algorithm = SymmetricAlgorithm.Create(AlgorithmName);
            ReadKey(Algorithm);

            //加密信息；  
            MemoryStream Target = new MemoryStream();

            //生成随机的初始化向量；  
            Algorithm.GenerateIV();
            Target.Write(Algorithm.IV, 0, Algorithm.IV.Length);

            //加密实际的数据；         
            CryptoStream cs = new CryptoStream(Target, Algorithm.CreateEncryptor(), CryptoStreamMode.Write);
            //参数1：对其执行加密转换的流。  
            //参数2：要对流执行的加密转换。  
            //参数3：对加密流的访问方式。  
            cs.Write(ClearData, 0, ClearData.Length);
            cs.FlushFinalBlock();
            //将加密后的结果作为字符数组返回；  
			byte[] bys= Target.ToArray();            
			cs.Close ();
			Target.Close ();
			return bys;
        }

        
        /// <summary>  
        /// 【解密数据】  
        /// </summary>  
        /// <param name="data">原始数据</param>  
        /// <param name="keyFile">密钥文件路径</param>  
        /// <returns></returns>  
        public string DecryptData(byte[] data)
        {            
            // 对称加密：加密算法和解密算法一致；  
            SymmetricAlgorithm Algorithm = SymmetricAlgorithm.Create(AlgorithmName);
            ReadKey(Algorithm);

            // 解密信息；  
            MemoryStream Target = new MemoryStream();

            // 读取IV，并使用它初始化解密算法；  
            int ReadPos = 0;
            byte[] IV = new byte[Algorithm.IV.Length];
            Array.Copy(data, IV, IV.Length);
            Algorithm.IV = IV;
            ReadPos += Algorithm.IV.Length;

            CryptoStream cs = new CryptoStream(Target, Algorithm.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(data, ReadPos, data.Length - ReadPos);
            cs.FlushFinalBlock();

            // 从内存数据流中获得字节并将它转换为文本；  
			string str= Encoding.UTF8.GetString(Target.ToArray());
			cs.Close ();
			return str;
        }
    }
}

