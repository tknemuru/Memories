using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Builders
{
#pragma warning disable CS8618
    /// <summary>
    /// 動画メタデータ取得引数の構築機能を提供します。
    /// </summary>
    public class MovieMetadataArgsBuilder : IBuildable
    {
        /// <summary>
        /// 入力元のファイルパス
        /// </summary>
        public string InputFilePath { get; private set; }

        /// <summary>
        /// 入力元のファイルパスを設定します。
        /// </summary>
        /// <param name="inputFilePath">入力元のファイルパス</param>
        /// <returns>ビルダー</returns>
        public MovieMetadataArgsBuilder SetInputFilePath(string inputFilePath)
        {
            InputFilePath = inputFilePath;
            return this;
        }

        /// <summary>
        /// 動画メタデータ取得引数を組み立てます。
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            return $"-v quiet -print_format json -show_format -select_streams a:0 \"{InputFilePath}\"";
        }
    }
}
