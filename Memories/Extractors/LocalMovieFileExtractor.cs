using Memories.Builders;
using Memories.Executors;
using Memories.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Extractors
{
#pragma warning disable CS8618, CS8602
    /// <summary>
    /// ローカルの動画ファイル情報を抽出する機能を提供します。
    /// </summary>
    public class LocalMovieFileExtractor : IMovieFileExtractable
    {
        /// <summary>
        /// 抽出対象のディレクトリ
        /// </summary>
        private string TargetDir { get; set; }

        /// <summary>
        /// メタ情報抽出引数の構築機能
        /// </summary>
        private MovieMetadataArgsBuilder ArgsBuilder { get; set; }

        /// <summary>
        /// メタ情報抽出機能
        /// </summary>
        private ProcessExecutor MeataExtractor { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetDir">抽出対象のディレクトリ</param>
        /// <param name="argsBuilder">メタ情報抽出引数の構築機能</param>
        /// <param name="metaExtractor">メタ情報抽出機能</param>
        public LocalMovieFileExtractor(string targetDir, MovieMetadataArgsBuilder argsBuilder, ProcessExecutor metaExtractor)
        {
            TargetDir = targetDir;
            ArgsBuilder = argsBuilder;
            MeataExtractor = metaExtractor;
        }



        /// <summary>
        /// 動画ファイル情報を抽出します。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MovieFileMetadata> Extract()
        {
            string[] files = Directory.GetFiles(TargetDir, "*.MOV", SearchOption.AllDirectories);
            var metas = files.Select(f =>
            {
                var args = ArgsBuilder
                       .SetInputFilePath(f)
                       .Build();
                var metaJson = MeataExtractor.Execute(args);
                var jsonObj = JObject.Parse(metaJson);
                var meta = new MovieFileMetadata()
                {
                    FileName = f,
                    Duration = double.Parse(jsonObj["format"]["duration"].ToString()),
                    CreationDateTime = DateTime.Parse(jsonObj["format"]["tags"]["com.apple.quicktime.creationdate"].ToString()),
                };
                return meta;
            });
            return metas;
        }
    }
}
