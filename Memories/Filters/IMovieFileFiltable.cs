using Memories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Filters
{
    /// <summary>
    /// 動画ファイルのフィルタ機能を提供します。
    /// </summary>
    public interface IMovieFileFiltable
    {
        /// <summary>
        /// 動画ファイルのリストをフィルタします。
        /// </summary>
        /// <param name="metadatas">動画ファイルのメタ情報リスト</param>
        /// <returns>フィルタした動画ファイルのメタ情報リスト</returns>
        IEnumerable<MovieFileMetadata> Filter(IEnumerable<MovieFileMetadata> metadatas);
    }
}
