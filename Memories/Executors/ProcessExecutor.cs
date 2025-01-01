using Memories.Helplers;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;

namespace Memories.Executors
{
#pragma warning disable CS8618, CS8601
    /// <summary>
    /// ファイル実行機能を提供します。
    /// </summary>
    public class ProcessExecutor
    {
        /// <summary>
        /// 実行ファイルパス
        /// </summary>
        private string ExeFilePath { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="exeFilePath">実行ファイルパス</param>
        public ProcessExecutor(string exeFilePath)
        {
            ExeFilePath = exeFilePath;
        }

        /// <summary>
        /// ffmpegを実行します。
        /// </summary>
        /// <param name="args">実行引数</param>
        public string Execute(string args)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExeFilePath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var result = new StringBuilder();
            try
            {
                // 非同期で出力とエラーを読み取る
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        result.AppendLine(e.Data);
                        FileHelper.Log(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        FileHelper.Log(e.Data);
                    }
                };

                process.Start();
                // 非同期でデータを読み取る
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            finally
            {
                // 完了後の処理
                process.CancelOutputRead();
                process.CancelErrorRead();
                process.Dispose();
            }

            return result.ToString();
        }
    }
}
