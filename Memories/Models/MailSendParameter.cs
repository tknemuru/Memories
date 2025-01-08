using System;
using System.Collections;
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
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 共有ストレージ上にアップしたファイルのURL
        /// </summary>
        public string SharedStorageFileUrl { get; set; }

        /// <summary>
        /// インスタンス状態を示す文字列を返却します。
        /// </summary>
        /// <returns>インスタンス状態を示す文字列</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"送信元アドレス: {FromAddress}");
            sb.AppendLine($"送信先アドレス: {ToAddress}");
            sb.AppendLine($"件名: {Subject}");
            sb.AppendLine($"メール本文: {Body}");
            sb.AppendLine($"ファイル名: {FileName}");
            sb.AppendLine($"共有ストレージ上にアップしたファイルのURL: {SharedStorageFileUrl}");
            return sb.ToString();
        }
    }
}
