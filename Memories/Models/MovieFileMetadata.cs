using CsvHelper.Configuration.Attributes;
using Memories.Converters;
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

        /// <summary>
        /// 開始時間（秒）
        /// </summary>
        public int StartSeconds { get; set; }

        /// <summary>
        /// 切り抜き時間（秒）
        /// </summary>
        public int TrimSeconds { get; set; }

        /// <summary>
        /// 送信済メールアドレスのリスト
        /// </summary>
        [TypeConverter(typeof(StringEnumerableConverter))]
        public List<string> HasSentAddresses {  get; set; } = new List<string>();

        /// <summary>
        /// 共有ストレージ上にアップしたファイルのURL
        /// </summary>
        public string SharedStorageFileUrl { get; set; }
    }
}
