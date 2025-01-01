using Memories.Helplers;
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
    public class LocalAudioFileExtractor : IAudioFileExtractable
    {
        /// <summary>
        /// 抽出対象のディレクトリ
        /// </summary>
        private string TargetDir { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetDir">抽出対象のディレクトリ</param>
        public LocalAudioFileExtractor(string targetDir)
        {
            TargetDir = targetDir;
        }

        /// <summary>
        /// 音声ファイルパスを抽出します。
        /// </summary>
        /// <returns>音声ファイルパス</returns>
        public string Extract()
        {
            string[] files = Directory.GetFiles(TargetDir, "*.mp3", SearchOption.AllDirectories);
            var random = new Random();
            var index = random.Next(files.Length);
            FileHelper.Log($"音声ファイルを決定。 {files[index]}");
            return files[index];
        }
    }
}
