using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Memories
{
#pragma warning disable CS8618, CS8601
    /// <summary>
    /// ffmpegの実行機能を提供します。
    /// </summary>
    public class FfmpegExecutor
    {
        /// <summary>
        /// ffmpegの実行ファイルパス
        /// </summary>
        private string FfmpegPath {  get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FfmpegExecutor()
        {
            FfmpegPath = AppConfig.Get().GetSection("ffmepegPath").Get<string>();
        }

        /// <summary>
        /// ffmpegを実行します。
        /// </summary>
        /// <param name="arguments">実行引数</param>
        public void Execute(string arguments)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FfmpegPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            // 非同期で出力とエラーを読み取る
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    Console.WriteLine("STDOUT: " + e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    Console.WriteLine("STDERR: " + e.Data);
                }
            };

            process.Start();
            // 非同期でデータを読み取る
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
    }
}
