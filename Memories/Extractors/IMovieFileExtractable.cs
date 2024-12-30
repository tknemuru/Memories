using Memories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Extractors
{
    /// <summary>
    /// 動画ファイル情報の抽出機能を提供します。
    /// </summary>
    public interface IMovieFileExtractable
    {
        /// <summary>
        /// 動画ファイル情報を抽出します。
        /// </summary>
        /// <returns></returns>
        IEnumerable<MovieFileMetadata> Extract();
    }
}
