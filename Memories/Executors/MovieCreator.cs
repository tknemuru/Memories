﻿using CsvHelper;
using Memories.Builders;
using Memories.Extractors;
using Memories.Filters;
using Memories.Helplers;
using Memories.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Executors
{
#pragma warning disable CS8604, CS8602
    /// <summary>
    /// 動画作成機能を提供します。
    /// </summary>
    public class MovieCreator
    {
        /// <summary>
        /// 動画作成パラメータ
        /// </summary>
        public MovieCreatorParameter Parameter { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MovieCreator()
        {
            Parameter = new MovieCreatorParameter();
            var filters = new List<IMovieFileFiltable>();

            // 除外リストを読み込む
            var exclusionListFilePath = AppConfig.Get().GetValue<string>("exclusionListFilePath");
            IEnumerable<string> exclusions = new List<string>();
            if (File.Exists(exclusionListFilePath))
            {
                exclusions = FileHelper.ReadTextLines(exclusionListFilePath);
            }

            var rangeCandidate = AppConfig.Get().GetSection("rangeCandidate").Get<int[]>();
            var datetimeFilter = new CreationDateTimeRangeFilter(rangeCandidate, string.Empty);
            var fileCount = AppConfig.Get().GetValue<int>("fileCount");
            var fileCountFilter = new FileCountFilter(fileCount);
            var movieSeconds = AppConfig.Get().GetValue<int>("movieSeconds");
            var secondsSettingsFilter = new TrimSecondsSettingsFilter(movieSeconds);
            var exclusionFilter = new ExclusionFilter(exclusions);

            filters.Add(datetimeFilter);
            filters.Add(fileCountFilter);
            filters.Add(secondsSettingsFilter);
            filters.Add(exclusionFilter);
            Parameter.MovieFileFilters = filters;
        }


        /// <summary>
        /// 長期間指定の動画を作成します。
        /// </summary>
        /// <param name="rangeCandidate">期間（月）の候補リスト</param>
        /// <param name="startMonth">開始月（yyyy-MM）</param>
        /// <param name="fileCount">ファイル数</param>
        /// <param name="movieSeconds">動画秒数</param>
        public void CreateLongSpanMovie(MovieCreatorParameter param)
        {
            var filters = new List<IMovieFileFiltable>();

            // 除外リストを読み込む
            var exclusionListFilePath = AppConfig.Get().GetValue<string>("exclusionListFilePath");
            IEnumerable<string> exclusions = new List<string>();
            if (File.Exists(exclusionListFilePath))
            {
                exclusions = FileHelper.ReadTextLines(exclusionListFilePath);
            }

            var datetimeFilter = new CreationDateTimeRangeFilter(param.RangeCandidate, param.StartMonth);
            var dateTimeEvenlyFilter = new DateTimeEvenlyFilter(param.FileCount);
            var secondsSettingsFilter = new TrimSecondsSettingsFilter(param.MovieSeconds);
            var exclusionFilter = new ExclusionFilter(exclusions);

            filters.Add(datetimeFilter);
            filters.Add(dateTimeEvenlyFilter);
            filters.Add(secondsSettingsFilter);
            filters.Add(exclusionFilter);
            param.MovieFileFilters = filters;

            Parameter = param;

            Create();
        }

        /// <summary>
        /// 動画を作成します。
        /// </summary>
        public void Create()
        {
            // 取得済のメタデータリストを読み込む
            var hasReadMetadataFilePath = AppConfig.Get().GetValue<string>("metaDataCsvFilePath");
            var hasReadMetadatas = CsvHelperWrapper.ReadRecords<MovieFileMetadata>(hasReadMetadataFilePath);

            // 取得済のメタデータは取得を省く
            var targetDirs = AppConfig.Get().GetSection("movieFileDirs").Get<List<string>>();
            var allFiles = new List<string>();
            foreach (var targetDir in targetDirs)
            {
                var files = Directory.GetFiles(targetDir, "*.*", SearchOption.AllDirectories)
                    .Where(file => file.EndsWith(".MOV", StringComparison.OrdinalIgnoreCase)
                        || file.EndsWith(".MP4", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                allFiles = allFiles.Union(files).ToList();
            }
            var hasReadFileDic = hasReadMetadatas
                .ToDictionary(m => m.FileName, m => m);
            var targetFiles = allFiles
                .Where(f => !hasReadFileDic.ContainsKey(f));

            // ローカルの動画ファイルリストを取得
            var builder = new MovieMetadataArgsBuilder();
            var ffproveExecutor = new ProcessExecutor("ffprobe");
            var metaExtractor = new LocalMovieFileExtractor(targetFiles, builder, ffproveExecutor);
            var metadatas = metaExtractor.Extract();

            // 取得済メタデータリストとマージ
            metadatas = metadatas.UnionBy(hasReadMetadatas, m => m.FileName);

            // 取得済のメタデータリストとして書き込む
            CsvHelperWrapper.WriteRecords(metadatas, hasReadMetadataFilePath);

            // 最小日付によるフィルタ
            var filterMinDateTime = DateTime.Parse(AppConfig.Get().GetValue<string>("minDateTime"));
            metadatas = metadatas
                .Where(m => m.Duration > 0d)
                .Where(m => m.CreationDateTime > filterMinDateTime)
                .ToList();

            // 対象を絞り込む
            foreach (var filter in Parameter.MovieFileFilters)
            {
                metadatas = filter
                    .Filter(metadatas)
                    .ToList();
            }
            metadatas = metadatas
                .OrderBy(m => m.CreationDateTime)
                .ToList();

            // 作成期間を把握しておく
            var minDatetime = metadatas
                    .Select(m => m.CreationDateTime)
                    .Min();
            var maxDatetime = metadatas
                .Select(m => m.CreationDateTime)
                .Max();
            FileHelper.Log($"作成日時の最小値: {minDatetime}");
            FileHelper.Log($"作成日時の最大値: {maxDatetime}");
            var minMaxTimestamp = $"{DateTime.Now.ToString("yyyyMMddHHmm")}-{minDatetime.ToString("yyyyMM")}-{maxDatetime.ToString("yyyyMM")}";

            // 対象をファイルに出力しておく
            var outputDir = AppConfig.Get().GetValue<string>("outputDir");
            var fileList = new FileListBuilder()
                    .SetMetadatas(metadatas)
                    .Build();
            var fileListPath = $"{outputDir}\\{minMaxTimestamp}-file-list.txt";
            FileHelper.Write(fileList, fileListPath, false);

            var tempDir = AppConfig.Get().GetValue<string>("tempDir");
            try
            {
                // 動画を切り取る
                var ffmpegExecutor = new ProcessExecutor("ffmpeg");
                foreach (var metadata in metadatas)
                {
                    var name = Path.GetFileNameWithoutExtension(metadata.FileName);
                    var timestamp = metadata.CreationDateTime.ToString("yyyyMMddHHmmssfff");
                    var trimArgs = new MovieTrimArgsBuilder()
                        .SetInputFilePath(metadata.FileName)
                        .SetOutputFilePath($"{tempDir}\\{timestamp}-{name}.MOV")
                        .SeTrimSeconds(metadata.TrimSeconds)
                        .SetStartSeconds(metadata.StartSeconds)
                        .Build();
                    ffmpegExecutor.Execute(trimArgs);
                }

                // 切り取った動画情報を取得
                string[] trimedFiles = Directory.GetFiles(tempDir, "*.MOV", SearchOption.AllDirectories);
                var trimedExtractor = new LocalMovieFileExtractor(trimedFiles, builder, ffproveExecutor);
                var trimedMetadatas = trimedExtractor.Extract()
                    .OrderBy(m => m.FileName)
                    .ToList();

                // ファイルリストを作成
                fileList = new FileListBuilder()
                    .SetMetadatas(trimedMetadatas)
                    .Build();
                fileListPath = $"{tempDir}\\file-list.txt";
                FileHelper.Write(fileList, fileListPath, false);

                // 動画を結合する
                var concatedFilePath = $"{tempDir}\\{minMaxTimestamp}-without-audio.MOV";
                var concatArgs = new MovieConcatArgsBuilder()
                    .SetInputFileListTextPath(fileListPath)
                    .SetOutputFilePath(concatedFilePath)
                    .Build();
                ffmpegExecutor.Execute(concatArgs);

                // BGMを決める
                var audioFilePath = string.Empty;
                if (string.IsNullOrEmpty(Parameter.AudioFilePath))
                {
                    var audioDir = AppConfig.Get().GetValue<string>("audioDir");
                    var audioExtractor = new LocalAudioFileExtractor(audioDir);
                    audioFilePath = audioExtractor.Extract();
                } else
                {
                    audioFilePath = Parameter.AudioFilePath;
                }

                // BGMをマージする
                var audioMergedFilePath = $"{tempDir}\\{minMaxTimestamp}-without-fadeout.MOV";
                var audioArgs = new MovieAudioMergeArgsBuilder()
                    .SetInputMovieFilePath(concatedFilePath)
                    .SetInputAudioFilePath(audioFilePath)
                    .SetOutputFilePath(audioMergedFilePath)
                    .Build();
                ffmpegExecutor.Execute(audioArgs);

                // フェードアウト設定をする
                metaExtractor = new LocalMovieFileExtractor(new string[] { audioMergedFilePath }, builder, ffproveExecutor);
                var meta = metaExtractor.Extract().First();
                var resultFilePath = $"{outputDir}\\{minMaxTimestamp}.MOV";
                var fadeoutArgs = new MovieFadeoutArgsBuilder()
                    .SetInputFilePath(audioMergedFilePath)
                    .SetOutputFilePath(resultFilePath)
                    .SetDuration((int)meta.Duration)
                    .Build();
                ffmpegExecutor.Execute(fadeoutArgs);

                // 完成ファイルをCSVレコードとして書き込む
                var resultCsvPath = AppConfig.Get().GetValue<string>("resultMovieCsvFilePath");
                var resultRecord = new MovieFileMetadata()
                {
                    FileName = resultFilePath,
                    CreationDateTime = DateTime.Now,
                };
                CsvHelperWrapper.WriteRecordAppend(resultRecord, resultCsvPath);
            }
            catch (Exception ex)
            {
                FileHelper.Log(ex.ToString());
                throw;
            }
            finally
            {
                // 一時フォルダ配下の全ファイルを履歴フォルダに移動
                var historyDir = AppConfig.Get().GetValue<string>("historyDir");
                FileHelper.MoveAllFiles(tempDir, $"{historyDir}\\{minMaxTimestamp}");
            }
        }
    }
}
