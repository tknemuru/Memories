using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Models
{
    /// <summary>
    /// 実行モード
    /// </summary>
    public enum ExeMode
    {
        /// <summary>
        /// 動画作成
        /// </summary>
        CreateMovie = 1,

        /// <summary>
        /// 通知メール送信
        /// </summary>
        SendNoticeMail,
    }
}
