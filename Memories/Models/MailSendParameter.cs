using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Models
{
#pragma warning disable CS8618
    /// <summary>
    /// メール送信パラメータ
    /// </summary>
    public class MailSendParameter
    {
        /// <summary>
        /// 送信元アドレス
        /// </summary>
        public string FromAddress {  get; set; }

        /// <summary>
        /// 送信先のアドレス
        /// </summary>
        public string ToAddress { get; set; }

        /// <summary>
        /// 件名
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// メール本文
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 2段階認証用アプリパスワード
        /// </summary>
        public string AppPassword { get; set; }

        /// <summary>
        /// 添付ファイル
        /// </summary>
        public List<Attachment> Attachments { get; private set; } = new List<Attachment>();

        /// <summary>
        /// 添付ファイルの添付前のオリジナルパス
        /// </summary>
        public List<string> OriginalFilePaths { get; private set; } = new List<string>();

        /// <summary>
        /// 添付ファイルを追加します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public void AddAttachment(string filePath)
        {
            Attachments.Add(new Attachment(filePath));
            OriginalFilePaths.Add(filePath);
        }
    }
}
