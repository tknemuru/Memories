using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Builders
{
#pragma warning disable CS8618
    /// <summary>
    /// 動画音声合成引数の構築機能を提供します。
    /// </summary>
    public class MovieAudioMergeArgsBuilder : IBuildable
    {
        /// <summary>
        /// 入力元の動画ファイルパス
        /// </summary>
        public string InputMovieFilePath { get; private set; }

        /// <summary>
        /// 入力元の音声ファイルパス
        /// </summary>
        public string InputAudioFilePath { get; private set; }

        /// <summary>
        /// 出力先のファイルパス
        /// </summary>
        public string OutputFilePath { get; private set; }

        /// <summary>
        /// 入力元の動画ファイルパスを設定します。
        /// </summary>
        /// <param name="inputFilePath">入力元の動画ファイルパス</param>
        /// <returns>ビルダー</returns>
        public MovieAudioMergeArgsBuilder SetInputMovieFilePath(string inputFilePath)
        {
            InputMovieFilePath = inputFilePath;
            return this;
        }

        /// <summary>
        /// 入力元の音声ファイルパスを設定します。
        /// </summary>
        /// <param name="inputFilePath">入力元の音声ファイルパス</param>
        /// <returns>ビルダー</returns>
        public MovieAudioMergeArgsBuilder SetInputAudioFilePath(string inputFilePath)
        {
            InputAudioFilePath = inputFilePath;
            return this;
        }

        /// <summary>
        /// 出力先のファイルパスを設定します。
        /// </summary>
        /// <param name="outputFilePath">出力先のファイルパス</param>
        /// <returns>ビルダー</returns>
        public MovieAudioMergeArgsBuilder SetOutputFilePath(string outputFilePath)
        {
            OutputFilePath = outputFilePath;
            return this;
        }

        /// <summary>
        /// 動画音声合成引数を組み立てます。
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            return $"-i \"{InputMovieFilePath}\" -i \"{InputAudioFilePath}\" -filter_complex \"[0:a]volume=1.0[a1];[1:a]volume=0.2[a2];[a1][a2]amix=inputs=2:duration=shortest\" -c:v copy -c:a aac -b:a 192k \"{OutputFilePath}\"";
        }
    }
}
