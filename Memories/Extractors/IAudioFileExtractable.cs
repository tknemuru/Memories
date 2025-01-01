using Memories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Extractors
{
    /// <summary>
    /// 音声ファイルパスの抽出機能を提供します。
    /// </summary>
    public interface IAudioFileExtractable
    {
        /// <summary>
        /// 音声ファイルパスを抽出します。
        /// </summary>
        /// <returns>音声ファイルパス</returns>
        string Extract();
    }
}
