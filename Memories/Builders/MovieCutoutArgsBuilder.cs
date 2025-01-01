using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Builders
{
#pragma warning disable CS8618
    /// <summary>
    /// 動画切り抜き引数の構築機能を提供します。
    /// </summary>
    public class MovieCutoutArgsBuilder : IBuildable
    {
        /// <summary>
        /// 入力元のファイルパス
        /// </summary>
        public string InputFilePath { get; private set; }

        /// <summary>
        /// 出力先のファイルパス
        /// </summary>
        public string OutputFilePath { get; private set; }

        /// <summary>
        /// 開始時間（秒）
        /// </summary>
        public int StartSeconds {  get; private set; }

        /// <summary>
        /// 切り抜き時間（秒）
        /// </summary>
        public int CutoutSeconds { get; private set; }

        /// <summary>
        /// 入力元のファイルパスを設定します。
        /// </summary>
        /// <param name="inputFilePath">入力元のファイルパス</param>
        /// <returns>ビルダー</returns>
        public MovieCutoutArgsBuilder SetInputFilePath(string inputFilePath)
        {
            InputFilePath = inputFilePath;
            return this;
        }

        /// <summary>
        /// 出力先のファイルパスを設定します。
        /// </summary>
        /// <param name="outputFilePath">出力先のファイルパス</param>
        /// <returns>ビルダー</returns>
        public MovieCutoutArgsBuilder SetOutputFilePath(string outputFilePath)
        {
            OutputFilePath = outputFilePath;
            return this;
        }

        /// <summary>
        /// 開始時間（秒）を設定します。
        /// </summary>
        /// <param name="startSeconds">開始時間（秒）</param>
        /// <returns>ビルダー</returns>
        public MovieCutoutArgsBuilder SetStartSeconds(int startSeconds)
        {
            StartSeconds = startSeconds;
            return this;
        }

        /// <summary>
        /// 切り抜き時間（秒）を設定します。
        /// </summary>
        /// <param name="cutoutSeconds">切り抜き時間（秒）</param>
        /// <returns>ビルダー</returns>
        public MovieCutoutArgsBuilder SetIntervalSecondss(int cutoutSeconds)
        {
            CutoutSeconds = cutoutSeconds;
            return this;
        }

        /// <summary>
        /// 動画切り抜き引数を組み立てます。
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            return $"-i \"{InputFilePath}\" -ss {StartSeconds} -t {CutoutSeconds} -c copy \"{OutputFilePath}\"";
        }
    }
}
