using Memories.Builders;
using Memories.Executors;
using Memories.Helplers;
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
        /// 抽出対象のファイルリスト
        /// </summary>
        private IEnumerable<string> Files { get; set; }

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
        /// <param name="files">抽出対象のファイルリスト</param>
        /// <param name="argsBuilder">メタ情報抽出引数の構築機能</param>
        /// <param name="metaExtractor">メタ情報抽出機能</param>
        public LocalMovieFileExtractor(IEnumerable<string> files, MovieMetadataArgsBuilder argsBuilder, ProcessExecutor metaExtractor)
        {
            Files = files;
            ArgsBuilder = argsBuilder;
            MeataExtractor = metaExtractor;
        }



        /// <summary>
        /// 動画ファイル情報を抽出します。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MovieFileMetadata> Extract()
        {
            var metas = Files.Select(f =>
            {
                var args = ArgsBuilder
                       .SetInputFilePath(f)
                       .Build();
                var metaJson = MeataExtractor.Execute(args);
                var jsonObj = JObject.Parse(metaJson);

                double duration = -1;
                if (jsonObj["format"] != null && jsonObj["format"]["duration"] != null)
                {
                    duration = double.Parse(jsonObj["format"]["duration"].ToString());
                } else
                {
                    FileHelper.Log($"durationが取得できませんでした。 {f}");
                }

                DateTime creationDatetime = new DateTime(1000, 1, 1);
                var hasFoundDatetime = false;
                if (jsonObj["format"] != null && jsonObj["format"]["tags"] != null)
                {
                    if (jsonObj["format"]["tags"]["com.apple.quicktime.creationdate"] != null)
                    {
                        creationDatetime = DateTime.Parse(jsonObj["format"]["tags"]["com.apple.quicktime.creationdate"].ToString());
                        FileHelper.Log($"com.apple.quicktime.creationdateから撮影日を特定 {creationDatetime}");
                        hasFoundDatetime = true;
                    }

                    if (!hasFoundDatetime && jsonObj["format"]["tags"]["creation_time"] != null)
                    {
                        creationDatetime = DateTime.Parse(jsonObj["format"]["tags"]["creation_time"].ToString());
                        FileHelper.Log($"creation_timeから撮影日を特定 {creationDatetime}");
                        hasFoundDatetime = true;
                    }
                }

                if (!hasFoundDatetime)
                {
                    FileHelper.Log($"creationDatetimeが取得できませんでした。 {f}");
                }

                var meta = new MovieFileMetadata()
                {
                    FileName = f,
                    Duration = duration,
                    CreationDateTime = creationDatetime,
                };
                return meta;
            });
            return metas;
        }
    }
}
