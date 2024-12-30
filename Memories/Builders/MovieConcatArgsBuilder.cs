using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Builders
{
#pragma warning disable CS8618
    /// <summary>
    /// 動画結合引数の構築機能を提供します。
    /// </summary>
    public class MovieConcatArgsBuilder : IBuildable
    {
        /// <summary>
        /// 入力元のファイルリストテキストパス
        /// </summary>
        public string InputFileListTextPath { get; private set; }

        /// <summary>
        /// 出力先のファイルパス
        /// </summary>
        public string OutputFilePath { get; private set; }

        /// <summary>
        /// 入力元のファイルリストテキストパスを設定します。
        /// </summary>
        /// <param name="inputFilePath">入力元のファイルリストテキストパス</param>
        /// <returns>ビルダー</returns>
        public MovieConcatArgsBuilder SetInputFileListTextPath(string inputFilePath)
        {
            InputFileListTextPath = inputFilePath;
            return this;
        }

        /// <summary>
        /// 出力先のファイルパスを設定します。
        /// </summary>
        /// <param name="outputFilePath">出力先のファイルパス</param>
        /// <returns>ビルダー</returns>
        public MovieConcatArgsBuilder SetOutputFilePath(string outputFilePath)
        {
            OutputFilePath = outputFilePath;
            return this;
        }

        /// <summary>
        /// 動画結合引数を組み立てます。
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            return $"-f concat -safe 0 -i \"{InputFileListTextPath}\" -c copy \"{OutputFilePath}\"";
        }
    }
}
