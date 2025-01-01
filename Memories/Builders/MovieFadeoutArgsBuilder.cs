using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Builders
{
#pragma warning disable CS8618
    /// <summary>
    /// 動画フェードアウト実行引数の構築機能を提供します。
    /// </summary>
    public class MovieFadeoutArgsBuilder : IBuildable
    {
        /// <summary>
        /// フェードアウトする時間
        /// </summary>
        private const int FadeoutSeconds = 3;

        /// <summary>
        /// 入力元のファイルパス
        /// </summary>
        public string InputFilePath { get; private set; }

        /// <summary>
        /// 出力先のファイルパス
        /// </summary>
        public string OutputFilePath { get; private set; }

        /// <summary>
        /// 入力元ファイルの時間（秒）
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// 入力元のファイルパスを設定します。
        /// </summary>
        /// <param name="inputFilePath">入力元のファイルパス</param>
        /// <returns>ビルダー</returns>
        public MovieFadeoutArgsBuilder SetInputFilePath(string inputFilePath)
        {
            InputFilePath = inputFilePath;
            return this;
        }

        /// <summary>
        /// 出力先のファイルパスを設定します。
        /// </summary>
        /// <param name="outputFilePath">出力先のファイルパス</param>
        /// <returns>ビルダー</returns>
        public MovieFadeoutArgsBuilder SetOutputFilePath(string outputFilePath)
        {
            OutputFilePath = outputFilePath;
            return this;
        }

        /// <summary>
        /// 入力元ファイルの時間（秒）を設定します。
        /// </summary>
        /// <param name="duration">入力元ファイルの時間（秒）</param>
        /// <returns>ビルダー</returns>
        public MovieFadeoutArgsBuilder SetDuration(int duration)
        {
            Duration = duration;
            return this;
        }

        /// <summary>
        /// 動画メタデータ取得引数を組み立てます。
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            int beforeOut = Duration - FadeoutSeconds;
            return $"-i \"{InputFilePath}\" -vf \"fade=t=out:st={beforeOut}:d={FadeoutSeconds}\" -af \"afade=t=out:st={beforeOut}:d={FadeoutSeconds}\" \"{OutputFilePath}\"";
        }
    }
}
