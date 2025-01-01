using Memories;
using Memories.Builders;
using Memories.Executors;
using Memories.Extractors;
using Memories.Filters;
using Memories.Helplers;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;

public class Program
{
#pragma warning disable CS8604
    static void Main(string[] args)
    {
        AppConfig.Init();

        // ローカルの動画ファイルリストを取得
        var targetDir = AppConfig.Get().GetValue<string>("movieFileDir");
        var builder = new MovieMetadataArgsBuilder();
        var ffproveExecutor = new ProcessExecutor("ffprobe");
        var extractor = new LocalMovieFileExtractor(targetDir, builder, ffproveExecutor);
        var metadatas = extractor.Extract();
        // TODO: 後で別クラス化
        var minDateTime = new DateTime(2000, 1, 1);
        metadatas = metadatas
            .Where(m => m.Duration > 0d)
            .Where(m => m.CreationDateTime > minDateTime);

        // 対象を絞り込む
        var rangeCandidate = AppConfig.Get().GetValue<int[]>("rangeCandidate");
        var datetimeFilter = new CreationDateTimeRangeFilter(rangeCandidate, string.Empty);
        var fileCountFilter = new FileCountFilter(10);
        var secondsSettingsFilter = new CutoutSecondsSettingsFilter(3);
        metadatas = datetimeFilter.Filter(metadatas);
        metadatas = fileCountFilter.Filter(metadatas);
        metadatas = secondsSettingsFilter.Filter(metadatas);

        var tempDir = AppConfig.Get().GetValue<string>("tempDir");
        try
        {
            // 動画を切り取る
            var ffmpegExecutor = new ProcessExecutor("ffmpeg");
            foreach (var metadata in metadatas)
            {
                var trimArgs = new MovieTrimArgsBuilder()
                    .SetInputFilePath(metadata.FileName)
                    .SetOutputFilePath($"{tempDir}\\{Guid.NewGuid()}.MOV")
                    .SeTrimSeconds(3)
                    .Build();
                ffmpegExecutor.Execute(trimArgs);
            }

            // 切り取った動画情報を取得
            var trimedExtractor = new LocalMovieFileExtractor(tempDir, builder, ffproveExecutor);
            var trimedMetadatas = trimedExtractor.Extract();

            // ファイルリストを作成
            var fileList = new FileListBuilder()
                .SetMetadatas(trimedMetadatas)
                .Build();
            var fileListPath = $"{tempDir}\\file-list.txt";
            FileHelper.Write(fileList, fileListPath);

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
            var outputDir = AppConfig.Get().GetValue<string>("outputDir");
            var resultFilePath = $"{outputDir}\\result.MOV";
            var audioArgs = new MovieAudioMergeArgsBuilder()
                .SetInputMovieFilePath(concatedFilePath)
                .SetInputAudioFilePath(audioFilePath)
                .SetOutputFilePath(resultFilePath)
                .Build();
            ffmpegExecutor.Execute(audioArgs);
        }
        finally
        {
            // 一時フォルダ配下の全ファイルを削除
            FileHelper.DeleteAllFolderFiles(tempDir);
        }
    }
}
