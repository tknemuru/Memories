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
    public class CutoutSecondsSettingsFilter : IMovieFileFiltable
    {
        /// <summary>
        /// 切り抜き時間（秒）
        /// </summary>
        private int CutoutSeconds {  get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cutoutSeconds">切り抜き時間（秒）</param>
        public CutoutSecondsSettingsFilter(int cutoutSeconds)
        {
            CutoutSeconds = cutoutSeconds;
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
                    var cutout = Math.Min((int)m.Duration, CutoutSeconds);
                    var start = random.Next((int)m.Duration - cutout);
                    m.CutoutSeconds = cutout;
                    m.StartSeconds = start;
                    return m;
                });
            return result;
        }
    }
}
