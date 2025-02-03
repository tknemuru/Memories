using Memories.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Models
{
#pragma warning disable CS8604, CS8602, CS8618
    /// <summary>
    /// 動画作成パラメータ
    /// </summary>
    public class MovieCreatorParameter
    {
        /// <summary>
        /// 期間（月）の候補リスト
        /// </summary>
        public int[] RangeCandidate { get; set; }

        /// <summary>
        /// 開始月（yyyy-MM）
        /// </summary>
        public string StartMonth { get; set; }

        /// <summary>
        /// ファイル数
        /// </summary>
        public int FileCount { get; set; }

        /// <summary>
        /// 動画秒数
        /// </summary>
        public int MovieSeconds { get; set; }

        /// <summary>
        /// 音声ファイルパス
        /// </summary>
        public string AudioFilePath { get; set; }

        /// <summary>
        /// フィルタ機能のリスト
        /// </summary>
        public List<IMovieFileFiltable> MovieFileFilters { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MovieCreatorParameter()
        {
            MovieFileFilters = new List<IMovieFileFiltable>();
        }
    }
}
