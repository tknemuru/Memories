using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Models
{
#pragma warning disable CS8618
    /// <summary>
    /// 動画ファイルのメタデータを保持します。
    /// </summary>
    public class MovieFileMetadata
    {
        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 動画時間（秒）
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreationDateTime { get; set; }
    }
}
