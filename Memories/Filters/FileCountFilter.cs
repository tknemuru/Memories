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
    /// ファイル数によるフィルタ機能を提供します。
    /// </summary>
    public class FileCountFilter : IMovieFileFiltable
    {
        /// <summary>
        /// 選択するファイル数
        /// </summary>
        private int SelectCount { get; set; }

        public FileCountFilter(int selectCount)
        {
            SelectCount = selectCount;
        }

        /// <summary>
        /// ファイル数による期間指定の絞り込み機能を提供します。
        /// </summary>
        /// <param name="metadatas">動画ファイルのメタ情報リスト</param>
        /// <returns>フィルタした動画ファイルのメタ情報リスト</returns>
        public IEnumerable<MovieFileMetadata> Filter(IEnumerable<MovieFileMetadata> metadatas)
        {
            var count = metadatas.Count();
            if (count <= SelectCount)
            {
                FileHelper.Log($"候補のファイル数より、選択するファイル数の方が多いため全件を対象にします。 候補のファイル数: {count} 選択するファイル数: {SelectCount}");
                return metadatas;
            }

            var result = new List<MovieFileMetadata>();
            var metadatasList = metadatas.ToList();
            var random = new Random();
            while(result.Count < SelectCount)
            {
                var i = random.Next(count);
                result.Add(metadatasList[i]);
            }
            return result;
        }
    }
}
