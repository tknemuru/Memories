using CsvHelper;
using Memories;
using Memories.Builders;
using Memories.Executors;
using Memories.Extractors;
using Memories.Filters;
using Memories.Helplers;
using Memories.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Globalization;
using YamlDotNet.Core;

public class Program
{
#pragma warning disable CS8604
    static void Main(string[] args)
    {
        AppConfig.Init();

        // 取得済のメタデータリストを読み込む
        IEnumerable<MovieFileMetadata> hasReadMetadatas = new List<MovieFileMetadata>();
        var hasReadMetadataFilePath = AppConfig.Get().GetValue<string>("metaDataCsvFilePath");
        if (File.Exists(hasReadMetadataFilePath))
        {
            using (var reader = new StreamReader(hasReadMetadataFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                hasReadMetadatas = csv.GetRecords<MovieFileMetadata>().ToList();
            }
        }

        // 取得済のメタデータは取得を省く
        var targetDir = AppConfig.Get().GetValue<string>("movieFileDir");
        string[] files = Directory.GetFiles(targetDir, "*.MOV", SearchOption.AllDirectories);
        var hasReadFileDic = hasReadMetadatas
            .ToDictionary(m => m.FileName, m => m);
        var targetFiles = files
            .Where(f => !hasReadFileDic.ContainsKey(f));

        // ローカルの動画ファイルリストを取得
        var builder = new MovieMetadataArgsBuilder();
        var ffproveExecutor = new ProcessExecutor("ffprobe");
        var extractor = new LocalMovieFileExtractor(targetFiles, builder, ffproveExecutor);
        var metadatas = extractor.Extract();

        // 取得済メタデータリストとマージ
        metadatas = metadatas.UnionBy(hasReadMetadatas, m => m.FileName);

        // 取得済のメタデータリストとして書き込む
        if (File.Exists(hasReadMetadataFilePath))
        {
            File.Delete(hasReadMetadataFilePath);
        }
        using (var writer = new StreamWriter(hasReadMetadataFilePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(metadatas);
        }

        // TODO: 後で別クラス化
        var filterMinDateTime = new DateTime(2000, 1, 1);
        metadatas = metadatas
            .Where(m => m.Duration > 0d)
            .Where(m => m.CreationDateTime > filterMinDateTime)
            .ToList();

        // 除外リストを読み込む
        var exclusionListFilePath = AppConfig.Get().GetValue<string>("exclusionListFilePath");
        IEnumerable<string> exclusions = new List<string>();
        if (File.Exists(exclusionListFilePath))
        {
            exclusions = FileHelper.ReadTextLines(exclusionListFilePath);
        }

        // 対象を絞り込む
        var rangeCandidate = AppConfig.Get().GetSection("rangeCandidate").Get<int[]>();
        var datetimeFilter = new CreationDateTimeRangeFilter(rangeCandidate, string.Empty);
        var fileCountFilter = new FileCountFilter(10);
        var secondsSettingsFilter = new TrimSecondsSettingsFilter(6);
        var exclusionFilter = new ExclusionFilter(exclusions);
        metadatas = datetimeFilter.Filter(metadatas).ToList();
        metadatas = exclusionFilter.Filter(metadatas).ToList();
        metadatas = fileCountFilter.Filter(metadatas).ToList();
        metadatas = secondsSettingsFilter
            .Filter(metadatas)
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
        var minMaxTimestamp = $"{minDatetime.ToString("yyyyMM")}-{maxDatetime.ToString("yyyyMM")}";

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
                    .SeTrimSeconds(6)
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
            var concatedFilePath = $"{tempDir}\\result.MOV";
            var concatArgs = new MovieConcatArgsBuilder()
                .SetInputFileListTextPath(fileListPath)
                .SetOutputFilePath(concatedFilePath)
                .Build();
            ffmpegExecutor.Execute(concatArgs);

            // BGMを決める
            var audioDir = AppConfig.Get().GetValue<string>("audioDir");
            var audioExtractor = new LocalAudioFileExtractor(audioDir);
            var audioFilePath = audioExtractor.Extract();

            // BGMをマージする
            var resultFilePath = $"{outputDir}\\{minMaxTimestamp}.MOV";
            var audioArgs = new MovieAudioMergeArgsBuilder()
                .SetInputMovieFilePath(concatedFilePath)
                .SetInputAudioFilePath(audioFilePath)
                .SetOutputFilePath(resultFilePath)
                .Build();
            ffmpegExecutor.Execute(audioArgs);
        }
        catch(Exception ex)
        {
            FileHelper.Log(ex.ToString());
        }
        finally
        {
            // 一時フォルダ配下の全ファイルを削除
            //FileHelper.DeleteAllFolderFiles(tempDir);
        }
    }
}
