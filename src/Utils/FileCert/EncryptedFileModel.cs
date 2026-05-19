using Lycoris.Common.Extensions;
using System.Text;

namespace Lycoris.Common.Utils.Cert
{
    /// <summary>
    /// 
    /// </summary>
    public class EncryptedFileModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        public string ContentType { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedAt { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        public byte[] Content { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        public string ContentAsString => this.Content.HasValue() ? Encoding.UTF8.GetString(this.Content).Trim() : "";

        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
