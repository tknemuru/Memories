using Memories.Helplers;
using Memories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Filters
{
    /// <summary>
    /// 除外リストによるフィルタ機能を提供します。
    /// </summary>
    public class ExclusionFilter : IMovieFileFiltable
    {
        /// <summary>
        /// 除外リスト
        /// </summary>
        private Dictionary<string, string> Exclusions { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="exclusions">除外リスト</param>
        public ExclusionFilter(IEnumerable<string> exclusions)
        {
            Exclusions = exclusions
                .Distinct()
                .ToDictionary(ex => ex, ex => ex);
        }

        /// <summary>
        /// 除外リストによる期間指定の絞り込み機能を提供します。
        /// </summary>
        /// <param name="metadatas">動画ファイルのメタ情報リスト</param>
        /// <returns>フィルタした動画ファイルのメタ情報リスト</returns>
        public IEnumerable<MovieFileMetadata> Filter(IEnumerable<MovieFileMetadata> metadatas)
        {
            return metadatas
                .Where(m => !Exclusions.ContainsKey(m.FileName));
        }
    }
}
