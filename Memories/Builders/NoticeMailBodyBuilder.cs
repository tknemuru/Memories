using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Builders
{
#pragma warning disable CS8618
    /// <summary>
    /// 通知メールの本文の構築機能を提供します。
    /// </summary>
    public class NoticeMailBodyBuilder : IBuildable
    {
        /// <summary>
        /// タグ
        /// </summary>
        public string Tag { get; private set; }

        /// <summary>
        /// 開始日時
        /// </summary>
        public DateTime StartDateTime { get; private set; }

        /// <summary>
        /// 終了日時
        /// </summary>
        public DateTime EndDateTime { get; private set; }

        /// <summary>
        /// タグを設定します。
        /// </summary>
        /// <param name="tag">タグ</param>
        /// <returns>ビルダー</returns>
        public NoticeMailBodyBuilder SetTag(string tag)
        {
            Tag = tag;
            return this;
        }

        /// <summary>
        /// 開始日時を設定します。
        /// </summary>
        /// <param name="startDateTime">開始日時</param>
        /// <returns>ビルダー</returns>
        public NoticeMailBodyBuilder SetStartDateTime(DateTime startDateTime)
        {
            StartDateTime = startDateTime;
            return this;
        }

        /// <summary>
        /// 終了日時を設定します。
        /// </summary>
        /// <param name="endDateTime">終了日時</param>
        /// <returns>ビルダー</returns>
        public NoticeMailBodyBuilder SetEndDateTime(DateTime endDateTime)
        {
            EndDateTime = endDateTime;
            return this;
        }

        /// <summary>
        /// 通知メールの本文を組み立てます。
        /// </summary>
        /// <returns>通知メールの本文</returns>
        public string Build()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{Tag}動画のお届けです！");
            builder.AppendLine($"今回は、{StartDateTime.ToString("yyyy年M月")}～{EndDateTime.ToString("yyyy年M月")} の動画をピックアップして作成しました。");
            builder.AppendLine($"お楽しみください！");
            return builder.ToString();
        }
    }
}
