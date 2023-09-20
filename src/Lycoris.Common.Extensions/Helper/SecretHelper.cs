using Lycoris.Base.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Lycoris.Base.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class SecretHelper
    {
        #region ======== 基础加解密 ========
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CommonEncrypt(string text) => CommonEncrypt(text, "lycoris");

        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="text"></param> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        public static string CommonEncrypt(string text, string key)
        {
            var des = DES.Create();
            var data = Encoding.Default.GetBytes(text);
            var keyHash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(key)).ToString() ?? "";
            des.Key = Encoding.ASCII.GetBytes(keyHash[..8]);
            var ivHash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(key)).ToString() ?? "";
            des.IV = Encoding.ASCII.GetBytes(ivHash[..8]);

            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            var ret = new StringBuilder();

            foreach (byte b in ms.ToArray())
                ret.AppendFormat("{0:X2}", b);

            return ret.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string CommonDecrypt(string Text) => CommonDecrypt(Text, "lycoris");

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string CommonDecrypt(string text, string key)
        {
            var des = DES.Create();

            int len;
            len = text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }

            var keyHash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(key)).ToString() ?? "";
            des.Key = Encoding.ASCII.GetBytes(keyHash[..8]);
            var ivHash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(key)).ToString() ?? "";
            des.IV = Encoding.ASCII.GetBytes(ivHash[..8]);

            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray(), 0, ms.ToArray().Length);
        }
        #endregion

        #region ======== MD5 ========
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lower"></param>
        /// <returns></returns>
        public static string Md5Encrypt(string str, bool lower = true)
        {
            var md5 = MD5.Create();
            var buffer = Encoding.Default.GetBytes(str);
            var md5buffer = md5.ComputeHash(buffer);
            var md5Str = BitConverter.ToString(md5buffer, 0, md5buffer.Length);
            md5Str = md5Str.Replace("-", "");
            return lower ? md5Str.ToLower() : md5Str.ToUpper();
        }

        /// <summary>
        /// MD5对文件流加密
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string Md5Encrypt(Stream stream)
        {
            var md5 = MD5.Create();
            var buffer = md5.ComputeHash(stream);
            var sb = new StringBuilder();
            foreach (byte var in buffer)
                sb.Append(var.ToString("x2"));

            return sb.ToString();
        }

        /// <summary> 
        /// MD5加密(返回16位加密串) 
        /// </summary> 
        /// <param name="input"></param> 
        /// <param name="encode"></param> 
        /// <returns></returns> 
        public static string Md5Encrypt16(string input, Encoding encode)
        {
            var md5 = MD5.Create();
            string result = BitConverter.ToString(md5.ComputeHash(encode.GetBytes(input)), 4, 8);
            result = result.Replace("-", "");
            return result;
        }
        #endregion

        #region ======== AES加密解密 ========
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="cipherMode"></param>
        /// <param name="paddingMode"></param>
        /// <returns></returns>
        public static string AesEncrypt(string? data, string key, string iv = "", CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (data.IsNullOrEmpty())
                return "";

            var result = AesEncryptToByte(Encoding.UTF8.GetBytes(data!), key, iv, cipherMode, paddingMode);
            return result.HasValue() ? Convert.ToBase64String(result) : "";
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="cipherMode"></param>
        /// <param name="paddingMode"></param>
        /// <returns></returns>
        public static byte[] AesEncryptToByte(byte[] bytes, string key, string iv = "", CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            // 使用32位密钥
            byte[] key32 = new byte[32];
            byte[] byteKey = Encoding.UTF8.GetBytes(key.PadRight(key32.Length));
            // 复制密钥
            Array.Copy(byteKey, key32, key32.Length);

            // 使用16位向量
            byte[] iv16 = new byte[16];
            // 如果我们的向量不是16为，则自动补全到16位
            byte[] byteIv = Encoding.UTF8.GetBytes(iv.PadRight(iv16.Length));
            // 复制向量
            Array.Copy(byteIv, iv16, iv16.Length);

            var aes = Aes.Create();
            aes.Mode = cipherMode;
            aes.Padding = paddingMode;
            aes.Key = key32;
            aes.IV = iv16;
            byte[] result = Array.Empty<byte>();

            try
            {
                using var ms = new MemoryStream();
                using var EncryptStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                EncryptStream.Write(bytes, 0, bytes.Length);
                EncryptStream.FlushFinalBlock();
                result = ms.ToArray();
            }
            catch
            {

            }

            return result;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="cipherMode"></param>
        /// <param name="paddingMode"></param>
        /// <returns></returns>
        public static string AesDecrypt(string? data, string key, string iv = "", CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (data.IsNullOrEmpty())
                return "";

            var bytes = Convert.FromBase64String(data!);
            var result = AesDecryptToByte(bytes, key, iv, cipherMode, paddingMode);
            return result.HasValue() ? Encoding.UTF8.GetString(result) : "";
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="cipherMode"></param>
        /// <param name="paddingMode"></param>
        /// <returns></returns>
        public static byte[] AesDecryptToByte(byte[] data, string key, string iv = "", CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            // 使用32位密钥
            byte[] key32 = new byte[32];
            // 如果我们的密钥不是32为，则自动补全到32位
            byte[] byteKey = Encoding.UTF8.GetBytes(key.PadRight(key32.Length));
            // 复制密钥
            Array.Copy(byteKey, key32, key32.Length);

            // 使用16位向量
            byte[] iv16 = new byte[16];
            // 如果我们的向量不是16为，则自动补全到16位
            byte[] byteIv = Encoding.UTF8.GetBytes(iv.PadRight(iv16.Length));
            // 复制向量
            Array.Copy(byteIv, iv16, iv16.Length);

            var aes = Aes.Create();
            aes.Mode = cipherMode;
            aes.Padding = paddingMode;
            aes.Key = key32;
            aes.IV = iv16;

            byte[] result = Array.Empty<byte>();

            try
            {
                using var ms = new MemoryStream(data);
                using var DecryptStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using var msResult = new MemoryStream();
                byte[] temp = new byte[1024 * 1024];
                int len = 0;
                while ((len = DecryptStream.Read(temp, 0, temp.Length)) > 0)
                {
                    msResult.Write(temp, 0, len);
                }

                result = msResult.ToArray();
            }
            catch
            {

            }

            return result;
        }
        #endregion

        #region ======== DES加密解密 ========
        /// <summary> 
        /// DES加密 
        /// </summary> 
        /// <param name="data">加密数据</param> 
        /// <param name="key">8位字符的密钥字符串</param> 
        /// <param name="iv">8位字符的初始化向量字符串</param> 
        /// <returns></returns> 
        public static string DESEncrypt(string data, string key, string iv)
        {
            byte[] byKey = Encoding.ASCII.GetBytes(key);
            byte[] byIV = Encoding.ASCII.GetBytes(iv);

            var cryptoProvider = DES.Create();
            var ms = new MemoryStream();
            var cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            var sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary> 
        /// DES解密 
        /// </summary> 
        /// <param name="data">解密数据</param> 
        /// <param name="key">8位字符的密钥字符串(需要和加密时相同)</param> 
        /// <param name="iv">8位字符的初始化向量字符串(需要和加密时相同)</param> 
        /// <returns></returns> 
        public static string? DESDecrypt(string data, string key, string iv)
        {
            byte[] byKey = Encoding.ASCII.GetBytes(key);
            byte[] byIV = Encoding.ASCII.GetBytes(iv);
            byte[] byEnc;

            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            var cryptoProvider = DES.Create();
            var ms = new MemoryStream(byEnc);
            var cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            var sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
        #endregion

        #region ======== 3DES 加密解密 ========
        /// <summary>
        /// 3DES 加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string TripleDESEncrypt(string data, string key) => TripleDESEncrypt(data, key, null, null, CipherMode.ECB);

        /// <summary>
        /// 3DES 加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string TripleDESEncrypt(string data, string key, [NotNull] string iv) => TripleDESEncrypt(data, key, iv!, null);

        /// <summary>
        /// 3DES 加密
        /// </summary>
        /// <param name="data">待加密数据</param>
        /// <param name="key">加密密钥</param>
        /// <param name="iv">偏移量</param>
        /// <param name="encoding">字符集编号</param>
        /// <param name="cipherMode">加密模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string TripleDESEncrypt(string data, string key, string? iv, Encoding? encoding, CipherMode cipherMode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "encryption key cannot be empty");
            if (key.Length != 24)
                throw new ArgumentException("encryption key length must be 24", nameof(key));

            if (cipherMode != CipherMode.ECB)
            {
                if (string.IsNullOrEmpty(iv))
                    throw new ArgumentNullException(nameof(iv), "offset cannot be empty");
                if (iv.Length != 6)
                    throw new ArgumentException("offset length must be 6", nameof(iv));
            }

            encoding ??= Encoding.GetEncoding("GB2312");

            var DES = TripleDES.Create();

            DES.Key = encoding.GetBytes(key);
            DES.Mode = cipherMode;
            DES.Padding = padding;

            if (cipherMode != CipherMode.ECB)
                DES.IV = encoding.GetBytes(iv!);

            var DESEncrypt = DES.CreateEncryptor();

            var Buffer = encoding.GetBytes(data);

            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        /// <summary>
        /// 3DES 解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string TripleDESDecrypt(string data, string key) => TripleDESDecrypt(data, key, null, null, CipherMode.ECB);

        /// <summary>
        /// 3DES 解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string TripleDESDecrypt(string data, string key, [NotNull] string iv) => TripleDESDecrypt(data, key, iv!, null);

        /// <summary>
        /// 3DES 解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="encoding"></param>
        /// <param name="cipherMode"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string TripleDESDecrypt(string data, string key, string? iv, Encoding? encoding, CipherMode cipherMode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "encryption key cannot be empty");
            if (key.Length != 24)
                throw new ArgumentException("encryption key length must be 24", nameof(key));

            if (cipherMode != CipherMode.ECB)
            {
                if (string.IsNullOrEmpty(iv))
                    throw new ArgumentNullException(nameof(iv), "offset cannot be empty");
                if (iv.Length != 6)
                    throw new ArgumentException("offset length must be 6", nameof(iv));
            }

            encoding ??= Encoding.GetEncoding("GB2312");

            var DES = TripleDES.Create();

            DES.Key = encoding.GetBytes(key);
            DES.Mode = cipherMode;
            DES.Padding = padding;
            DES.IV = encoding.GetBytes(iv!);

            var DESDecrypt = DES.CreateDecryptor();

            try
            {
                byte[] Buffer = Convert.FromBase64String(data);
                return encoding.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region ======== Base64加密解密 ========
        /// <summary> 
        /// Base64加密 
        /// </summary> 
        /// <param name="input">需要加密的字符串</param> 
        /// <returns></returns> 
        public static string Base64Encrypt(string input)
        {
            return Base64Encrypt(input, new UTF8Encoding());
        }

        /// <summary> 
        /// Base64加密 
        /// </summary> 
        /// <param name="input">需要加密的字符串</param> 
        /// <param name="encode">字符编码</param> 
        /// <returns></returns> 
        public static string Base64Encrypt(string input, Encoding encode)
        {
            return Convert.ToBase64String(encode.GetBytes(input));
        }

        /// <summary> 
        /// Base64解密 
        /// </summary> 
        /// <param name="input">需要解密的字符串</param> 
        /// <returns></returns> 
        public static string Base64Decrypt(string input)
        {
            return Base64Decrypt(input, new UTF8Encoding());
        }

        /// <summary> 
        /// Base64解密 
        /// </summary> 
        /// <param name="input">需要解密的字符串</param> 
        /// <param name="encode">字符的编码</param> 
        /// <returns></returns> 
        public static string Base64Decrypt(string input, Encoding encode)
        {
            return encode.GetString(Convert.FromBase64String(input));
        }
        #endregion

        #region RSA加密解密
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static (RSAParameters publicKey, RSAParameters privateKey) GenerateRSAKey()
        {
            using var rsa = RSA.Create();
            return (rsa.ExportParameters(false), rsa.ExportParameters(true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static (string publicKey, string privateKey) GenerateRSAXMLKey()
        {
            using var rsa = RSA.Create();
            return (rsa.ToXmlString(false), rsa.ToXmlString(true));
        }

        /// <summary>
        /// RSA加密加密
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string publicKey, string content, string encoding = "utf-8")
        {
            string encryptedContent = string.Empty;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                byte[] encryptedData = rsa.Encrypt(Encoding.GetEncoding(encoding).GetBytes(content), false);
                encryptedContent = Convert.ToBase64String(encryptedData);
            }
            return encryptedContent;
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string RSADecrypt(string privateKey, string content, string encoding = "utf-8")
        {
            string decryptedContent = string.Empty;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                byte[] decryptedData = rsa.Decrypt(Convert.FromBase64String(content), false);
                decryptedContent = Encoding.GetEncoding(encoding).GetString(decryptedData);
            }
            return decryptedContent;
        }
        #endregion

        #region SHA
        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SHA1Encrypt(string input)
        {
            using var sha1 = SHA1.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha1.ComputeHash(inputBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SHA256Encrypt(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
        #endregion
    }
}
