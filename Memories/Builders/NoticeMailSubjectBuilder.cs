using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Builders
{
#pragma warning disable CS8618
    /// <summary>
    /// 通知メールの件名の構築機能を提供します。
    /// </summary>
    public class NoticeMailSubjectBuilder : IBuildable
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
        public NoticeMailSubjectBuilder SetTag(string tag)
        {
            Tag = tag;
            return this;
        }

        /// <summary>
        /// 開始日時を設定します。
        /// </summary>
        /// <param name="startDateTime">開始日時</param>
        /// <returns>ビルダー</returns>
        public NoticeMailSubjectBuilder SetStartDateTime(DateTime startDateTime)
        {
            StartDateTime = startDateTime;
            return this;
        }

        /// <summary>
        /// 終了日時を設定します。
        /// </summary>
        /// <param name="endDateTime">終了日時</param>
        /// <returns>ビルダー</returns>
        public NoticeMailSubjectBuilder SetEndDateTime(DateTime endDateTime)
        {
            EndDateTime = endDateTime;
            return this;
        }

        /// <summary>
        /// 通知メールの件名を組み立てます。
        /// </summary>
        /// <returns>通知メールの件名</returns>
        public string Build()
        {
            return $"【{Tag}動画】{StartDateTime.ToString("yyyy年M月")}～{EndDateTime.ToString("yyyy年M月")}";
        }
    }
}
