using System;
using System.Diagnostics;

public class Program
{
    static void Main(string[] args)
    {
        int startSeconds = 10; // 切り取り開始位置（秒単位）

        // 1. 動画を切り取るコマンド
        string trimCommand = $"-i \"{inputVideo}\" -ss {startSeconds} -t 3 -c copy \"{tempVideo}\"";

        // 2. BGMを追加するコマンド
        string addBgmCommand = $"-i \"{tempVideo}\" -i \"{inputBgm}\" -shortest -c:v copy -c:a aac -b:a 192k \"{outputVideo}\"";

        try
        {
            // 1. 動画を切り取る
            Console.WriteLine("切り取り処理を開始します...");
            RunFFmpegCommand(ffmpegPath, trimCommand);
            Console.WriteLine("動画の切り取りが完了しました。");

            // 2. BGMを追加する
            Console.WriteLine("BGM追加処理を開始します...");
            RunFFmpegCommand(ffmpegPath, addBgmCommand);
            Console.WriteLine("BGM付き動画の作成が完了しました。");
        }
        finally
        {
            // 一時ファイルを削除する（必要に応じて）
            if (File.Exists(tempVideo))
            {
                File.Delete(tempVideo);
            }
        }
    }

    // FFmpegコマンドを実行する共通メソッド
    static void RunFFmpegCommand(string ffmpegPath, string arguments)
    {
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
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
