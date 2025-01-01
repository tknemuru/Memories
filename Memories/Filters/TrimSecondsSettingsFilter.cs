using Memories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Filters
{
    /// <summary>
    /// 切り抜き時間（秒）をセットする機能を提供します。
    /// </summary>
    public class TrimSecondsSettingsFilter : IMovieFileFiltable
    {
        /// <summary>
        /// 切り抜き時間（秒）
        /// </summary>
        private int TrimSeconds {  get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="trimSeconds">切り抜き時間（秒）</param>
        public TrimSecondsSettingsFilter(int trimSeconds)
        {
            TrimSeconds = trimSeconds;
        }

        /// <summary>
        /// 切り抜き時間（秒）をセットします。
        /// </summary>
        /// <param name="metadatas">動画ファイルのメタ情報リスト</param>
        /// <returns>フィルタした動画ファイルのメタ情報リスト</returns>
        public IEnumerable<MovieFileMetadata> Filter(IEnumerable<MovieFileMetadata> metadatas)
        {
            var random = new Random();
            var result = metadatas
                .Select(m =>
                {
                    var trim = Math.Min((int)m.Duration, TrimSeconds);
                    var start = random.Next((int)m.Duration - trim);
                    m.CutoutSeconds = trim;
                    m.StartSeconds = start;
                    return m;
                });
            return result;
        }
    }
}
