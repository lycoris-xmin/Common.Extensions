using Lycoris.Common.Utils.Cert;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Lycoris.Api.Common.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class FileCertUtils
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        private readonly string _fileHeader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">AES Key (16字节)</param>
        /// <param name="iv">AES IV (16字节)</param>
        /// <param name="fileHeader">文件签名（推荐4个字母）</param>
        /// <exception cref="ArgumentException"></exception>
        public FileCertUtils(string key, string iv, string fileHeader)
        {
            _key = HexToBytes(key);
            _iv = HexToBytes(iv);

            if (_key.Length != 16 || _iv.Length != 16)
                throw new ArgumentException("Key 和 IV 必须为16字节");

            _fileHeader = fileHeader;
        }

        /// <summary>
        /// 加密并保存流到指定路径，支持自定义文件名和内容类型
        /// </summary>
        /// <param name="inputStream">输入的文件流</param>
        /// <param name="outputPath">输出加密文件路径</param>
        /// <param name="fileName">原始文件名（用于元数据）</param>
        /// <param name="contentType">MIME 类型（用于元数据）</param>
        /// <returns></returns>
        public async Task SaveAsync(Stream inputStream, string outputPath, string fileName, string contentType)
        {
            // 读取原始内容
            await using var tempStream = new MemoryStream();
            await inputStream.CopyToAsync(tempStream);
            var plainBytes = tempStream.ToArray();

            // 构造元数据
            var metadataObj = new
            {
                FileName = fileName,
                ContentType = contentType,
                CreatedAt = DateTime.UtcNow
            };
            var metadataJson = JsonSerializer.Serialize(metadataObj);
            var metadataBytes = Encoding.UTF8.GetBytes(metadataJson);

            // 加密内容
            byte[] encryptedContent;
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                await using var encryptedStream = new MemoryStream();
                await using (var cryptoStream = new CryptoStream(encryptedStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    await cryptoStream.WriteAsync(plainBytes);
                }
                encryptedContent = encryptedStream.ToArray();
            }

            // 计算校验哈希
            var combinedData = metadataBytes.Concat(encryptedContent).ToArray();
            var hashBytes = SHA256.HashData(combinedData);

            // 写入结构化加密文件
            await using var output = new FileStream(outputPath, FileMode.Create);
            await output.WriteAsync(Encoding.UTF8.GetBytes(_fileHeader[..4]));

            var metaLengthBytes = BitConverter.GetBytes(metadataBytes.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(metaLengthBytes);
            await output.WriteAsync(metaLengthBytes);

            await output.WriteAsync(metadataBytes);
            await output.WriteAsync(encryptedContent);
            await output.WriteAsync(hashBytes);
        }

        /// <summary>
        /// 解密并读取文件内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="CryptographicException"></exception>
        public async Task<EncryptedFileModel> ReadAsync(string filePath)
        {
            try
            {
                await using var input = new FileStream(filePath, FileMode.Open);

                var header = new byte[4];
                await input.ReadAsync(header);
                if (Encoding.UTF8.GetString(header) != _fileHeader[..4])
                    return new EncryptedFileModel() { ErrorMessage = "file header mismatch" };

                var lengthBytes = new byte[4];
                await input.ReadAsync(lengthBytes);
                if (BitConverter.IsLittleEndian) Array.Reverse(lengthBytes);
                int metaLength = BitConverter.ToInt32(lengthBytes);

                var metadataBytes = new byte[metaLength];
                await input.ReadAsync(metadataBytes);
                var metadataJson = Encoding.UTF8.GetString(metadataBytes);

                // 反序列化成一个临时对象，再把属性映射给最终对象
                var tempMetadata = JsonSerializer.Deserialize<EncryptedFileModel>(metadataJson);
                if (tempMetadata == null)
                    return new EncryptedFileModel() { ErrorMessage = "metadata parsing failed" };

                var remainingLength = (int)(input.Length - input.Position);
                var remainingBytes = new byte[remainingLength];
                await input.ReadAsync(remainingBytes);

                var contentBytes = remainingBytes[..^32];
                var hashStored = remainingBytes[^32..];

                var combinedData = metadataBytes.Concat(contentBytes).ToArray();
                var hashComputed = SHA256.HashData(combinedData);

                if (!hashComputed.SequenceEqual(hashStored))
                    return new EncryptedFileModel() { ErrorMessage = "file verification failed, may be damaged or tampered with" };

                using var decryptStream = new MemoryStream(contentBytes);
                using var cryptoStream = new CryptoStream(decryptStream, Aes.Create().CreateDecryptor(_key, _iv), CryptoStreamMode.Read);
                using var resultStream = new MemoryStream();
                await cryptoStream.CopyToAsync(resultStream);

                return new EncryptedFileModel
                {
                    FileName = tempMetadata.FileName,
                    ContentType = tempMetadata.ContentType,
                    CreatedAt = tempMetadata.CreatedAt,
                    Content = resultStream.ToArray(),
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new EncryptedFileModel() { ErrorMessage = ex.Message };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static byte[] HexToBytes(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have even length");

            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
                result[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return result;
        }
    }
}
